namespace Assets.Scripts.Events
{
    public class UpgradeTaken
    {
        public string TreeId;
        public string UpgradeId;
        public int Price;

        public UpgradeTaken(string treeId, string upgradeId, int price)
        {
            TreeId = treeId;
            UpgradeId = upgradeId;
            Price = price;
        }
    }
}