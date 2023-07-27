using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Zenject;
using Assets.Scripts.GameData;
using Assets.Scripts.Factories;

namespace Assets.Scripts.UI
{
    public class UpgradeTree : MonoBehaviour
    {
        #region Injection

        private IObjectFactory _factory;

        [Inject]
        private void Inject(MainFactory factory)
        {
            _factory = factory;
        }

        #endregion

        [SerializeField]
        private TextMeshProUGUI _treeTitle;

        private UpgradeTreeTemplate _template;
        private Dictionary<string, UpgradeItem> _upgrades = new Dictionary<string, UpgradeItem>();

        public void Initialize(UpgradeTreeTemplate template)
        {
            _template = template;

            _treeTitle.text = _template.Id;

            var description = FactoryDescriptionBuilder.Object()
                                .Parent(transform)
                                .PrefabName(StaticPathTranslator.UPGRADE_ITEM)
                                .Build();
            foreach (var upgrade in template.Upgrades)
            {
                var newItem = _factory.Create(description).GetComponent<UpgradeItem>();
                newItem.Initialize(upgrade, _template.Id);
                _upgrades.Add(upgrade.Id, newItem);
            }
        }

        public void Checkout(UpgradeProgress progress, int points)
        {
            if (progress == null)
            {
                foreach (var upgrade in _template.Upgrades)
                {
                    _upgrades[upgrade.Id].SetState(UpgradeItem.EUpgradeState.Blocked);
                }
                _upgrades[_template.Upgrades[0].Id].CheckPrice(points);
            }
            else
            {
                foreach (var takenUpgrade in progress.Upgrades)
                {
                    _upgrades[takenUpgrade].SetState(UpgradeItem.EUpgradeState.Taken);
                }
                foreach (var upgrade in _template.Upgrades)
                {
                    if (_upgrades[upgrade.Id].State != UpgradeItem.EUpgradeState.Taken)
                    {
                        if (_upgrades[upgrade.Id].CheckCondition(progress))
                        {
                            _upgrades[upgrade.Id].CheckPrice(points);
                        }
                        else
                        {
                            _upgrades[upgrade.Id].SetState(UpgradeItem.EUpgradeState.Blocked);
                        }
                    }
                }
            }
        }
    }
}