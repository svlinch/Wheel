using System.Collections;
using System.Collections.ObjectModel;
using System;
using Assets.Scripts.Events;
using Assets.Scripts.Saves;

namespace Assets.Scripts.GameData
{
    public class PlayerData : IInitialize
    {
        #region Injection
        private ISaveSystem _saveSystem;
        private EventService _eventService;
        private SubscriptionHolder _subscriptions;

        public PlayerData(ISaveSystem saveSystem, EventService eventService)
        {
            _saveSystem = saveSystem;
            _eventService = eventService;
            _subscriptions = new SubscriptionHolder(_eventService);
            _subscriptions.Subscribe<TrapUpgradeTaken>(HandleUpgradeTaken);
            _subscriptions.Subscribe<EndLevelEvent>(HandleEndLevel);
            _subscriptions.Subscribe<UpgradeTaken>(HandleUpgradeTaken);
        }
        #endregion

        private PlayerTemplate _playerTemplate;

        public IEnumerator Initialize()
        {
            _playerTemplate = _saveSystem.Load();
            yield return null;
        }

        public int GetLastLevelIndex()
        {
            return _playerTemplate.GetLastLevelIndex();
        }

        public ReadOnlyCollection<UpgradeProgress> GetTrapsUpgradesList()
        {
            return _playerTemplate.GetTrapsUpgradesList();
        }

        public int GetUpgradePoints()
        {
            return _playerTemplate.UpgradePoints;
        }

        public Type Type()
        {
            return GetType();
        }

        private bool HandleUpgradeTaken(TrapUpgradeTaken e)
        {
            _playerTemplate.AddTrapUpgrade(e.TreeId, e.UpgradeId);
            return true;
        }

        private bool HandleEndLevel(EndLevelEvent e)
        {
            if (e.Win)
            {
                _playerTemplate.AddLevelProgress(e.Index, e.Reward);
            }
            return true;
        }

        private bool HandleUpgradeTaken(UpgradeTaken e)
        {
            _playerTemplate.AddUpgradePoints(-e.Price);
            _playerTemplate.AddTrapUpgrade(e.TreeId, e.UpgradeId);
            return true;
        }
    }
}
