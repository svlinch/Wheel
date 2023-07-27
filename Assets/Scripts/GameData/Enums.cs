namespace Assets.Scripts.GameData
{
    public enum ESaveSystemType
    {
        PlayerPrefs,
        Storage
    }

    public enum EChangeType
    {
        Plus,
        Minus,
        Multiply,
        Replace
    }

    public enum EWeaponType
    {
        OnTouch,
        OnTimer
    }

    public enum ETargetType
    {
        Self,
        Common
    }

    public enum EGameState
    {
        BeforeGameState,
        BaseState,
        GamePlayState
    }
}
