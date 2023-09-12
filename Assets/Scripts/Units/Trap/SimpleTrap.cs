using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Utility;
using Assets.Scripts.GameData;
using Assets.Scripts.Events;
using Assets.Scripts.Models;

namespace Assets.Scripts.Units
{
    public class SimpleTrap : Trap
    {
        private Dictionary<WeaponTemplate, GameTimer> _timers = new Dictionary<WeaponTemplate, GameTimer>();

        public override void Setup(int index)
        {
            _indexInList = index;
        }

        public override void SetModel(UnitModel model)
        {
            _model = model;
            _statusCanvas.ChangeHealth(_model.ReadParameter(StaticParameterTranslator.HEALTH) / _model.Template.GetNumericParameters()[StaticParameterTranslator.HEALTH]);
        }

        public override void HandleUpdate(float dTime)
        {
            UpdateImpacts(dTime);
            UpdateWeapons(dTime);
        }

        public override void SetBattleState(bool state)
        {
            _statusCanvas.ChangeHealth(1f);
            _statusCanvas.ChangeState(state);
            if (state)
            {
                CheckoutWeapons();
            }
        }

        public override void SetSelected(bool state)
        {
            _visualPart.GetComponentInChildren<MeshRenderer>().material.color = state ? Color.green : Color.white;
        }

        public override void HandleDamageEvent(Dictionary<string, float> numeric, List<ImpactTemplate> impacts)
        {
            ApplyChanges(numeric);
            ApplyImpacts(impacts);
        }

        public override bool RemoveCheck()
        {
            return _model.ReadParameter(StaticParameterTranslator.HEALTH) <= 0f;
        }

        public override void Remove()
        {
            SetBattleState(false);
            _impacts.Clear();
        }

        public override void HandleClick()
        {
            _eventService.SendMessage(new BaseElementClicked(_indexInList, _model, _visualPart.transform.position));
        }

        protected override void ApplyChanges(Dictionary<string, float> numeric)
        {
            foreach(var key in numeric.Keys)
            {
                _model.UpdateParameter(key, numeric[key]);
            }
            _statusCanvas.ChangeHealth(_model.ReadParameter(StaticParameterTranslator.HEALTH) / _model.Template.GetNumericParameters()[StaticParameterTranslator.HEALTH]);
        }
        
        protected override void ApplyDamage(float change)
        {
            _model.UpdateParameter(StaticParameterTranslator.HEALTH, change);
            _statusCanvas.ChangeHealth(_model.ReadParameter(StaticParameterTranslator.HEALTH) / _model.Template.GetNumericParameters()[StaticParameterTranslator.HEALTH]);
        }

        protected override void ApplyImpacts(List<ImpactTemplate> impacts)
        {
            if (impacts == null)
            {
                return;
            }
            foreach (var impact in impacts)
            {
                _impacts.Add(new ImpactModel(impact));
            }
        }

        protected override void UpdateImpacts(float dTime)
        {
            var changedHealth = 0;
            for (int i = 0; i < _impacts.Count; i++)
            {
                var result = _impacts[i].Checkout(dTime);
                if (result.Item2)
                {
                    changedHealth += _impacts[i].GetValue();
                }
                if (result.Item1)
                {
                    _impacts.RemoveAt(i);
                    i--;
                }
            }
            ApplyDamage(changedHealth);
        }

        protected virtual void CheckoutWeapons()
        {
            var weapons = _model.Template.GetWeapons();
            foreach (var weapon in weapons)
            {
                if (weapon.Type == EWeaponType.OnTimer)
                {
                    var newTimer = new GameTimer(weapon.NumericParameters[StaticParameterTranslator.COOLDOWN]);
                    _timers.Add(weapon, newTimer);
                }
            }
        }

        protected virtual void UpdateWeapons(float dTime)
        {
            var weapons = _model.Template.GetWeapons();
            foreach (var weapon in _timers.Keys)
            {
                if (_timers[weapon].HandleUpdate(dTime))
                {
                    _timers[weapon].CheckoutTimer(weapon.NumericParameters[StaticParameterTranslator.COOLDOWN]);
                    _eventService.SendMessage(new WeaponActionEvent(_spawnPoint, _directionHelp, this, weapon));
                }
            }
        }

        protected override void HandleContact(Transform enemy)
        {
            var enemyScript = enemy.GetComponentInParent<Enemy>();
            if (enemyScript != null)
            {
                _eventService.SendMessage(new DamageEvent(this, enemyScript));
            }
            else
            {
                Debug.LogError(enemy.name + " does not have Enemy script");
            }
        }
    }
}