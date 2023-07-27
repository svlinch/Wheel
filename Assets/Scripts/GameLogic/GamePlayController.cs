using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Assets.Scripts.Utility;
using Assets.Scripts.GameData;
using Assets.Scripts.Events;

namespace Assets.Scripts.GameLogic
{
    public class GamePlayController : MonoBehaviour
    {
        #region Injection
        private PlayerData _playerData;
        private ITimeService _timeService;
        private EventService _eventService;
        private SubscriptionHolder _subscriptions;

        [Inject]
        public void Inject(PlayerData playerData, ITimeService timeService, EventService eventService)
        {
            _playerData = playerData;
            _timeService = timeService;
            _eventService = eventService;
            _subscriptions = new SubscriptionHolder(_eventService);
            _subscriptions.Subscribe<GameStateChangedEvent>(HandleGameStateChanged);
            _subscriptions.Subscribe<EndLevelEvent>(HandleEndLevel);
        }
        #endregion

        [SerializeField]
        private LevelController _levelController;
        [SerializeField]
        private BaseController _baseController;

        private List<IHandleUpdate> _updates = new List<IHandleUpdate>();

        private int _currentLevel;

        public void StartGame(bool nextLevel)
        {
            if (nextLevel)
            {
                _currentLevel = _playerData.GetLastLevelIndex();
            }
            PrepareLevel();
        }

        public void StartSpecialLevel(int index)
        {
            _currentLevel = index;
            PrepareLevel();
        }

        private void PrepareLevel()
        {
            _levelController.PrepareLevel(_currentLevel);
            _baseController.PrepareLevel(_currentLevel);

            _timeService.SetPauseState(false);
            _eventService.SendMessage(new GameStateChangedEvent(EGameState.BaseState));
        }

        private void Update()
        {
            if (_timeService.Pause)
            {
                return;
            }

            foreach (var toUpdate in _updates)
            {
                if (!toUpdate.HandleUpdate())
                {
                    break;
                }
            }
        }

        private bool HandleGameStateChanged(GameStateChangedEvent e)
        {
            switch (e.NewState)
            {
                case EGameState.BeforeGameState:
                    _updates.Clear();
                    break;
                case EGameState.BaseState:
                    _updates.Clear();
                    _updates.Add(_baseController);
                    break;
                case EGameState.GamePlayState:
                    _updates.Clear();
                    _updates.Add(_baseController);
                    _updates.Add(_levelController);
                    break;
            }
            return true;
        }

        private bool HandleEndLevel(EndLevelEvent e)
        {
            _eventService.SendMessage(new GameStateChangedEvent(EGameState.BeforeGameState));
            return true;
        }
    }
}