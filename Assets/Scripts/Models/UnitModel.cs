using System.Collections.Generic;
using Assets.Scripts.GameData;

namespace Assets.Scripts.Models
{
    public class UnitModel
    {
        public UnitTemplateHolder Template;
        private Dictionary<string, float> _parameters = new Dictionary<string, float>();

        public UnitModel(UnitTemplateHolder template)
        {
            Template = template;
            _parameters[StaticParameterTranslator.HEALTH] = template.GetNumericParameters()[StaticParameterTranslator.HEALTH];
        }

        public float ReadParameter(string key)
        {
            return _parameters[key];
        }

        public void UpdateParameter(string key, float value)
        {
            if (!_parameters.ContainsKey(key))
            {
                _parameters.Add(key, 0f);
            }
            _parameters[key] += value;
        }
    }
}