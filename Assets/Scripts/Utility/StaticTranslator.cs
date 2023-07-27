namespace Assets.Scripts
{
    public static class StaticParameterTranslator
    {
        public static readonly string MATERIALS_BONUS = "MaterialsBonus";
        public static readonly string SLOTS = "Slots";
        public static readonly string COOLDOWN = "Cooldown";
        public static readonly string MAIN_TRAP = "Main";
        public static readonly string FOUNDAMENT = "Foundament";
        public static readonly string PRICE = "Price";
        public static readonly string HEALTH = "Health";
        public static readonly string CURRENT_HEALTH = "CHealth";
        public static readonly string DAMAGE = "Damage";
        public static readonly string SPEED = "Speed";
    }

    public static class StaticPathTranslator
    {
        public static readonly string LEVEL_ITEM = "Prefabs/UI/LevelItem";
        public static readonly string UPGRADE_TREE = "Prefabs/UI/Tree";
        public static readonly string UPGRADE_ITEM = "Prefabs/UI/UpgradeItem";
        public static readonly string TRAP_BUTTON = "Prefabs/UI/ShopItem";
        public static readonly string TRAP_PATH = "Prefabs/Base/";
        public static readonly string ENEMY_PATH = "Prefabs/Enemy/";
        public static readonly string BATTLE_PATH = "Prefabs/Battle/";
    }

    public static class StaticTextTranslator
    {
        public static readonly string WIN_TEXT = "Win";
        public static readonly string LOSE_TEXT = "Lose";
    }
}