using UnityEngine;
using Zenject;
using Assets.Scripts.Utility;
using Assets.Scripts.GameData;
using Assets.Scripts.Factories;
using Assets.Scripts.GameLogic;

namespace Assets.Scripts
{
    public class MainInstaller : MonoInstaller
    {
        [SerializeField]
        private ESaveSystemType _saveSystemType;

        public override void InstallBindings()
        {
            Container.Bind<EventService>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<TimeService>().AsSingle().NonLazy();
            BindSaveSystem();
            Container.Bind<PlayerData>().AsSingle().NonLazy();
            Container.Bind<BalanceService>().AsSingle().NonLazy();
            Container.Bind<MainFactory>().AsSingle().NonLazy();
            Container.Bind<DamageManager>().AsSingle().NonLazy();
        }

        private void BindSaveSystem()
        {
            switch (_saveSystemType)
            {
                case ESaveSystemType.PlayerPrefs: Container.BindInterfacesAndSelfTo<PlayerPrefsSystem>().AsSingle(); return;
                case ESaveSystemType.Storage: return;
            }
        }
    }
}
