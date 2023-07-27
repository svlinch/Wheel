using System;
using System.Collections.Generic;

namespace Assets.Scripts.GameData
{
    [Serializable]
    public class Units : TemplateClass<UnitTemplate>
    {
    }

    [Serializable]
    public class UnitTemplate
    {
        public string Id;
        public bool DisableBuild;
        public Dictionary<string, float> NumericParameters;
        public List<string> StringParameters;
        public List<WeaponTemplate> Weapons;
    }
}