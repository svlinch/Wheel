using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;
using System.Linq;

namespace Assets.Scripts.GameData
{
    [Serializable]
    public class PlayerTemplate
    {
        public int UpgradePoints;
        public int Slots;
        public List<LevelProgress> ProgressList;
        public List<UpgradeProgress> UpgradesListTraps;
        public List<UpgradeProgress> UpgradesListBase;

        public PlayerTemplate()
        {
            UpgradePoints = 100;
            ProgressList = new List<LevelProgress>();
            UpgradesListTraps = new List<UpgradeProgress>() { new UpgradeProgress("Wall", new List<string>() { "Wall" }),
                                                          new UpgradeProgress("Main", new List<string>()),
                                                          new UpgradeProgress("Foundament", new List<string>()) };
            UpgradesListBase = new List<UpgradeProgress>();
            Slots = 3;
        }

        public ReadOnlyCollection<UpgradeProgress> GetTrapsUpgradesList()
        {
            return UpgradesListTraps.AsReadOnly();
        }

        public int GetLastLevelIndex()
        {
            return ProgressList.Count > 0 ? ProgressList.Count : 0;
        }

        public void AddLevelProgress(int index, int reward)
        {
            if (ProgressList.Count == index)
            {
                ProgressList.Add(new LevelProgress());
                UpgradePoints += reward;
            }
        }

        public void AddTrapUpgrade(string tree, string upgrade)
        {
            var upgradeTree = UpgradesListTraps.FirstOrDefault(x => x.TreeId.Equals(tree));
            if (upgradeTree != null)
            {
                upgradeTree.Upgrades.Add(upgrade);
            }
            else
            {
                UpgradesListTraps.Add(new UpgradeProgress(tree, new List<string> { upgrade }));
            }
        }

        public void AddUpgradePoints(int number)
        {
            UpgradePoints += number;
        }
    }

    [Serializable]
    public class LevelProgress
    {
        public int Score;
    }

    [Serializable]
    public class UpgradeProgress
    {
        public string TreeId;
        public List<string> Upgrades;

        public UpgradeProgress(string id, List<string> upgrades)
        {
            TreeId = id;
            Upgrades = upgrades;
        }
    }
}