using System;

namespace Assets.Scripts.GameData
{
    [Serializable]
    public class Levels: TemplateClass<LevelTemplate>
    {
    }

    [Serializable]
    public class LevelTemplate
    {
        public int Index;
        public int Materials;
        public LevelReward Reward;
        public Wave[] Waves;
    }

    [Serializable]
    public class LevelReward
    {
        public int MaxReward;
    }
    
    [Serializable]
    public class Wave
    {
        public float Delay;
        public Spawn[] Spawns;
    }

    [Serializable]
    public class Spawn
    {
        public string EnemyId;
        public int SplineIndex;
    }
}
