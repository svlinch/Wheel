using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.BezierLines;
using Assets.Scripts.GameData;
using Assets.Scripts.UI;
using Assets.Scripts.Models;

namespace Assets.Scripts.Units
{
    public class Enemy : MonoBehaviour
    {
        [SerializeField]
        private EnemyStatusCanvas _statusCanvas;
        private EnemyMoveController _moveController;
        private UnitModel _model;

        private List<ImpactModel> _impacts = new List<ImpactModel>();

        public void Setup(BezierSpline spline, UnitTemplate template)
        {
            _moveController = new EnemyMoveController();
            _moveController.Setup(spline, transform);
            _model = new UnitModel(new UnitTemplateHolder(template));
            _model.UpdateParameter(StaticParameterTranslator.SPEED, template.NumericParameters[StaticParameterTranslator.SPEED]);
            _statusCanvas.CheckoutHealth(_model.ReadParameter(StaticParameterTranslator.HEALTH) / _model.Template.GetNumericParameters()[StaticParameterTranslator.HEALTH]);
        }

        public bool RemoveCheck()
        {
            return _model.ReadParameter(StaticParameterTranslator.HEALTH) <= 0f;
        }

        public void HandleUpdate(float dTime)
        {
            _moveController.HandleUpdate(dTime);
            UpdateImpacts(dTime);
        }

        public void HandleDamageEvent(Dictionary<string, float> numeric, List<ImpactTemplate> impacts)
        {
            HandleDamage(numeric);
            ApplyImpacts(impacts);
        }

        public void Remove()
        {
            _impacts.Clear();
        }

        public UnitModel GetModel()
        {
            return _model;
        }

        private void ApplyImpacts(List<ImpactTemplate> impacts)
        {
            foreach (var impact in impacts)
            {
                _impacts.Add(new ImpactModel(impact));
            }
        }

        private void UpdateImpacts(float dTime)
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
            HandleDamage(changedHealth);
        }

        private void HandleDamage(Dictionary<string, float> numeric)
        {
            foreach (var key in numeric.Keys)
            {
                _model.UpdateParameter(key, numeric[key]);
            }
            _statusCanvas.CheckoutHealth(_model.ReadParameter(StaticParameterTranslator.HEALTH) / _model.Template.GetNumericParameters()[StaticParameterTranslator.HEALTH]);
        }

        private void HandleDamage(float change)
        {
            _model.UpdateParameter(StaticParameterTranslator.HEALTH, change);
            _statusCanvas.CheckoutHealth(_model.ReadParameter(StaticParameterTranslator.HEALTH) / _model.Template.GetNumericParameters()[StaticParameterTranslator.HEALTH]);
        }
    }
}