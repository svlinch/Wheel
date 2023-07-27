namespace Assets.Scripts.Events
{
    public class TrapUpgradeTaken
    {
        public string TreeId;
        public string UpgradeId;

        public TrapUpgradeTaken(string tree, string upgrade)
        {
            TreeId = tree;
            UpgradeId = upgrade;
        }
    }
}