using System.Collections.Generic;
using System;

namespace Assets.Scripts.GameData
{
    [Serializable]
    public class UpgradeTrees: TemplateClass<UpgradeTreeTemplate>
    {
    }

    [Serializable]
    public class UpgradeTreeTemplate
    {
        public string Id;
        public List<UpgradeTemplate> Upgrades;
    }

    [Serializable]
    public class UpgradeTemplate
    {
        public string Id;
        public int Price;
        public string ActivateCondition;
        public UpgradeChange[] Changes;
        public WeaponChange[] WeaponChanges;
        public string[] SpecialChanges;
    }

    [Serializable]
    public class UpgradeChange
    {
        public EChangeType Type;
        public string TargetId;
        public string FormulaId;
    }

    [Serializable]
    public class WeaponChange
    {
        public EChangeType Type;
        public WeaponTemplate Weapon;
    }
}
