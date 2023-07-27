using System.Collections.Generic;
using System;

namespace Assets.Scripts.GameData
{
    [Serializable]
    public class WeaponTemplate
    {
        public string Id;
        public EWeaponType Type;
        public ETargetType TargetType;
        public int Priority;
        public Dictionary<string, float> NumericParameters;
        public string Formula;
        public string ChangeParameter;
        public string Spawn;
        public ImpactTemplate Impact;
    }
}
