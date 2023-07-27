using Assets.Scripts.GameData;

namespace Assets.Scripts.Events
{
    public class GameStateChangedEvent
    {
        public EGameState NewState;

        public GameStateChangedEvent(EGameState state)
        {
            NewState = state;
        }
    }
}