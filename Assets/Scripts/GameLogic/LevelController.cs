using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.BezierLines;
using Assets.Scripts.GameData;
using Zenject;
using Assets.Scripts.Utility;
using Assets.Scripts.Factories;
using Assets.Scripts.Events;
using Assets.Scripts.Units;

namespace Assets.Scripts.GameLogic
{
    public class LevelController : MonoBehaviour, IHandleUpdate
    {
        #region Injection
        private BalanceService _balanceService;
        private ITimeService _timeService;
        private PoolFactory _factory;
        private SubscriptionHolder _subscriptions;
        private EventService _eventService;

        [Inject]
        public void Inject(BalanceService balanceService, ITimeService timeService, MainFactory factory, EventService eventService)
        {
            _eventService = eventService;
            _balanceService = balanceService;
            _timeService = timeService;
            _factory = new PoolFactory(factory, "emeny");

            _timer = new GameTimer(_timeService, 0f, true);
            _subscriptions = new SubscriptionHolder(_eventService);
            _subscriptions.Subscribe<EndLevelEvent>(HandleEndLevel);
        }
        #endregion

        [SerializeField]
        private BezierSpline[] _splines;
        [SerializeField]
        private Transform _enemyHolder;
        [SerializeField]
        private GameObject _prefabTest;

        private LevelTemplate _template;
        private List<Enemy> _enemies;

        private GameTimer _timer;
        private int _waveIndex;
        private bool _waitingForWave;

        public void PrepareLevel(int levelIndex)
        {
            _template = _balanceService.GetLevelTemplate(levelIndex);
            _enemies = new List<Enemy>();
            _waveIndex = 0;
            UpdateTimer();
        }

        public bool HandleUpdate()
        {
            if (_waitingForWave && _timer.HandleUpdate())
            {
                SpawnWave();
            }

            for (int i = 0; i < _enemies.Count; i++)
            {
                if (_enemies[i].RemoveCheck())
                {
                    _enemies[i].Remove();
                    _factory.ReturnToPool(_enemies[i].gameObject, _enemies[i].GetModel().Template.GetId());
                    _enemies.RemoveAt(i);
                    i--;
                }
            }
            foreach (var enemy in _enemies)
            {
                enemy.HandleUpdate(_timeService.GetDeltaTime(true));
            }

            return CheckWinCondition();
        }

        private void UpdateTimer()
        {
            _waitingForWave = true;
            _timer.CheckoutTimer(_template.Waves[_waveIndex].Delay);
        }

        private void SpawnWave()
        {
            var wave = _template.Waves[_waveIndex];
            foreach (var spawn in wave.Spawns)
            {
                var description = FactoryDescriptionBuilder.Object()
                  .PrefabName(string.Format("{0}{1}", StaticPathTranslator.ENEMY_PATH, spawn.EnemyId)).Kind(spawn.EnemyId).Build();
                var newGo = _factory.Create(description).GetComponent<Enemy>();
                newGo.Setup(_splines[spawn.SplineIndex], _balanceService.GetEnemyTemplate(spawn.EnemyId));
                _enemies.Add(newGo);
            }
            PrepareNextWave();
        }

        private void PrepareNextWave()
        {
            if (_template.Waves.Length > ++_waveIndex)
            {
                UpdateTimer();
            }
            else
            {
                _waitingForWave = false;
            }
        }

        private bool CheckWinCondition()
        {
            if (_waitingForWave || _enemies.Count > 0)
            {
                return true;
            }
            _eventService.SendMessage(new EndLevelEvent(true, _template.Index, _template.Reward.MaxReward));
            return false;
        }

        private bool HandleEndLevel(EndLevelEvent e)
        {
            for (int i = 0; i < _enemies.Count; i++)
            {
                _enemies[i].Remove();
                _factory.ReturnToPool(_enemies[i].gameObject, _enemies[i].GetModel().Template.GetId());
                _enemies.RemoveAt(i);
                i--;
            }
            return true;
        }
    }
}