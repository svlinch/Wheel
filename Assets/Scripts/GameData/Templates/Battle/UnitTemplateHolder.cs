using System.Collections.Generic;

namespace Assets.Scripts.GameData
{
    public class UnitTemplateHolder
    {
        private UnitTemplate _template;

        public bool CheckBuildable()
        {
            return _template.DisableBuild;
        }

        public UnitTemplateHolder(UnitTemplate template)
        {
            _template = template;
        }

        public List<WeaponTemplate> GetWeapons()
        {
            return _template.Weapons;
        }

        public Dictionary<string, float> GetNumericParameters()
        {
            return _template.NumericParameters;
        }

        public string GetId()
        {
            return _template.Id;
        }
    }
}
