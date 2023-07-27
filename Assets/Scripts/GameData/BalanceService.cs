using System;
using System.Linq;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Assets.Scripts.Utility;
using Assets.Scripts.Events;

namespace Assets.Scripts.GameData
{
    public class BalanceService : IInitialize
    {
        #region Injection
        private PlayerData _playerData;
        private SubscriptionHolder _subscriptiopns;

        public BalanceService(PlayerData playerData, EventService eventService)
        {
            _playerData = playerData;
            _subscriptiopns = new SubscriptionHolder(eventService);
            _subscriptiopns.Subscribe<UpgradeTaken>(HandleUpgradeTaken);
        }
        #endregion

        private BalanceData _data;

        private List<UnitTemplateHolder> _upgradedTraps;
        private Dictionary<string, Formula> _formulas;

        public IEnumerator Initialize()
        {
            _data = new BalanceData();
            _data.LevelTemplates = JsonConvert.DeserializeObject<Levels>(Resources.Load<TextAsset>("LevelTemplates").ToString());
            _data.TrapTemplates = JsonConvert.DeserializeObject<Units>(Resources.Load<TextAsset>("TrapTemplates").ToString());
            _data.EnemyTemplates = JsonConvert.DeserializeObject<Units>(Resources.Load<TextAsset>("EnemyTemplates").ToString());
            _data.TreeTemplates = JsonConvert.DeserializeObject<UpgradeTrees>(Resources.Load<TextAsset>("UpgradeTemplates").ToString());
            yield return null;
            _formulas = new Dictionary<string, Formula>();
            var formulas = JsonConvert.DeserializeObject<Formulas>(Resources.Load<TextAsset>("Formulas").ToString());
            foreach (var formula in formulas.Templates)
            {
                _formulas.Add(formula.Kind, new Formula(formula.Formula));
            }
            yield return null;
            InitializeTraps();
        }

        public LevelTemplate GetLevelTemplate(int index)
        {
            if (_data.LevelTemplates.Templates.Count > index)
            {
                return _data.LevelTemplates.Templates[index];
            }
            else
            {
                Debug.LogWarning(string.Format("Level with index {0} does not exist in the Levels Json", index));
                return _data.LevelTemplates.Templates[_data.LevelTemplates.Templates.Count - 1];
            }
        }

        public ReadOnlyCollection<LevelTemplate> GetLevels()
        {
            return _data.LevelTemplates.Templates.AsReadOnly();
        }

        public Formula GetFormula(string key)
        {
            return _formulas[key];
        }

        public Type Type()
        {
            return GetType();
        }

        public ReadOnlyCollection<UpgradeTreeTemplate> GetUpgradeTrees()
        {
            return _data.TreeTemplates.Templates.AsReadOnly();
        }

        public ReadOnlyCollection<UnitTemplateHolder> GetUpgradedTraps()
        {
            return _upgradedTraps.AsReadOnly();
        }

        public UnitTemplateHolder GetUpgradedTrap(string key)
        {
            var result = _upgradedTraps.First(x => x.GetId().Equals(key));
            return result;
        }

        public UnitTemplate GetEnemyTemplate(string key)
        {
            return _data.EnemyTemplates.Templates.First(x => x.Id.Equals(key));
        }

        private void InitializeTraps()
        {
            _upgradedTraps = new List<UnitTemplateHolder>();
            var upgrades = _playerData.GetTrapsUpgradesList();
            foreach (var upgrade in upgrades)
            {
                var initialTemplate = _data.TrapTemplates.Templates.FirstOrDefault(x => x.Id.Equals(upgrade.TreeId));
                if (initialTemplate != null)
                {
                    var clonedTemplate = CloneUtil.Clone<UnitTemplate>(initialTemplate);
                    var templateHolder = new UnitTemplateHolder(clonedTemplate);
                    ApplyAllUpgrades(templateHolder, upgrade);
                    _upgradedTraps.Add(templateHolder);
                }
            }
        }

        private void ApplyAllUpgrades(UnitTemplateHolder template, UpgradeProgress upgradeInfo)
        {
            var tree = _data.TreeTemplates.Templates.FirstOrDefault(x => x.Id.Equals(upgradeInfo.TreeId));
            if (tree == null)
            {
                return;
            }
            foreach (var upgrade in upgradeInfo.Upgrades)
            {
                ApplyUpgrade(template, upgrade, tree);
            }
        }

        private void ApplyUpgrade(UnitTemplateHolder template, string upgradeId, UpgradeTreeTemplate tree)
        {
            var upgrade = tree.Upgrades.First(x => x.Id.Equals(upgradeId));
            if (upgrade.Changes != null)
            {
                var formulaResult = 0f;
                foreach (var change in upgrade.Changes)
                {
                    formulaResult = _formulas[change.FormulaId].GetResult(template.GetNumericParameters(), null, null, null, null);
                    switch (change.Type)
                    {
                        case EChangeType.Plus:
                            template.GetNumericParameters()[change.TargetId] += formulaResult; break;
                        case EChangeType.Minus:
                            template.GetNumericParameters()[change.TargetId] -= formulaResult; break;
                        case EChangeType.Multiply:
                            template.GetNumericParameters()[change.TargetId] *= formulaResult; break;
                        case EChangeType.Replace:
                            template.GetNumericParameters()[change.TargetId] = formulaResult; break;
                    }
                }
            }

            if (upgrade.WeaponChanges != null)
            {
                foreach (var weaponUpgrade in upgrade.WeaponChanges)
                {
                    Debug.Log(" checking " + weaponUpgrade.Weapon.Id);
                    switch (weaponUpgrade.Type)
                    {
                        case EChangeType.Minus:
                            var weaponToRemove = template.GetWeapons().FirstOrDefault(x => x.Id.Equals(weaponUpgrade.Weapon.Id));
                            if (weaponToRemove != null)
                            {
                                template.GetWeapons().Remove(weaponToRemove);
                            }
                            break;
                        case EChangeType.Plus:
                            template.GetWeapons().Add(weaponUpgrade.Weapon);
                            break;
                        case EChangeType.Replace:
                            var weaponToReplace = template.GetWeapons().FirstOrDefault(x => x.Id.Equals(weaponUpgrade.Weapon.Id));
                            if (weaponToReplace != null)
                            {
                                weaponToReplace = weaponUpgrade.Weapon;
                            }
                            break;
                    }
                }
            }
        }

        private bool HandleUpgradeTaken(UpgradeTaken e)
        {
            var trap = _upgradedTraps.FirstOrDefault(x => x.GetId().Equals(e.TreeId));
            if (trap == null)
            {
                var initialTemplate = _data.TrapTemplates.Templates.First(x => x.Id.Equals(e.TreeId));
                trap = new UnitTemplateHolder(CloneUtil.Clone<UnitTemplate>(initialTemplate));
                _upgradedTraps.Add(trap);
            }
            var upgradeTree = _data.TreeTemplates.Templates.First(x => x.Id.Equals(e.TreeId));

            ApplyUpgrade(trap, e.UpgradeId, upgradeTree);
            return true;
        }
    }
}