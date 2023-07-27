using Assets.Scripts.GameData;

namespace Assets.Scripts.Saves
{
    public interface ISaveSystem
    {
        public PlayerTemplate Load();
        public void Save(PlayerTemplate data);
    }
}