using System.Collections.Generic;
using System;
using UnityEngine;
using Zenject;
using Assets.Scripts.Factories;
using Assets.Scripts.GameData;
using Assets.Scripts.Events;
using Assets.Scripts.Utility;
using Assets.Scripts.Models;
using Assets.Scripts.Units;

namespace Assets.Scripts.GameLogic
{
    public class BaseController : MonoBehaviour, IHandleUpdate
    {
        #region Injection
        private EventService _eventService;
        private PlayerData _playerData;
        private BalanceService _balanceService;
        private SubscriptionHolder _subscriptions;
        private PoolFactory _factory;
        private ITimeService _timeService;

        [Inject]
        public void Inject(EventService eventService, PlayerData playerData, BalanceService balanceService, MainFactory factory, ITimeService timeService)
        {
            _eventService = eventService;
            _playerData = playerData;
            _balanceService = balanceService;
            _factory = new PoolFactory(factory, "traps");
            _timeService = timeService;

            _inputController = new InputController();
            _subscriptions = new SubscriptionHolder(_eventService);
            _subscriptions.Subscribe<BaseElementClicked>(HandleBaseElemenyClicked);
            _subscriptions.Subscribe<GameStateChangedEvent>(HandleGameStateChanged);
            _subscriptions.Subscribe<TrapButtonClicked>(HandleTrapButtonClicked);
            _subscriptions.Subscribe<ResetTrapButtonClicked>(HandleResetTrapButtonClicked);
            _subscriptions.Subscribe<EndLevelEvent>(HandleEndLevel);
        }
        #endregion

        [SerializeField]
        private Transform[] _buildPositions;
        [SerializeField]
        private Transform _buildingsParent;
        [SerializeField]
        private Trap _mainTrap;

        private InputController _inputController;

        private int _activeElementIndex;
        private int _buildResources;
        private Func<bool> _currentUpdateAction;

        private List<Trap> _traps = new List<Trap>();

        public void PrepareLevel(int levelIndex)
        {
            var mainTrap = _balanceService.GetUpgradedTrap(StaticParameterTranslator.MAIN_TRAP);

            CreateFoundaments((int)mainTrap.GetNumericParameters()[StaticParameterTranslator.SLOTS]);
            CreateMainTrap();

            _buildResources = (int)mainTrap.GetNumericParameters()[StaticParameterTranslator.MATERIALS_BONUS] + _balanceService.GetLevelTemplate(levelIndex).Materials;
            _eventService.SendMessage(new BuildResourcesChanged(_buildResources));
            _activeElementIndex = -1;
        }

        public bool HandleUpdate()
        {
            if (_currentUpdateAction != null)
            {
                return _currentUpdateAction.Invoke();
            }
            return true;
        }

        private bool TrapsUpdate()
        {
            for (int i = 0; i < _traps.Count; i++)
            {
                if (_traps[i].RemoveCheck())
                {
                    if (_traps[i].GetModel().Template.GetId().Equals(StaticParameterTranslator.MAIN_TRAP))
                    {
                        _eventService.SendMessage(new EndLevelEvent(false));
                        return false;
                    }
                    _traps[i].Remove();
                    _factory.ReturnToPool(_traps[i].gameObject, _traps[i].GetModel().Template.GetId());
                    _traps.RemoveAt(i);
                    i--;
                }
            }
            foreach (var trap in _traps)
            {
                trap.HandleUpdate(_timeService.GetDeltaTime(true));
            }
            return true;
        }

        private bool HandleBaseElemenyClicked(BaseElementClicked e)
        {
            if (_activeElementIndex >= 0)
            {
                _traps[_activeElementIndex].SetSelected(false);
            }
            if (_activeElementIndex == e.Index)
            {
                _activeElementIndex = -1;
                return true;
            }
            _activeElementIndex = e.Index;
            _traps[_activeElementIndex].SetSelected(true);
            return true;
        }

        private bool HandleGameStateChanged(GameStateChangedEvent e)
        {
            switch (e.NewState)
            {
                case EGameState.BeforeGameState: _currentUpdateAction = null; break;
                case EGameState.BaseState: _currentUpdateAction = (() => _inputController.HandleUpdate()); break;
                case EGameState.GamePlayState:
                    foreach (var trap in _traps)
                    {
                        trap.SetBattleState(true);
                    }
                    _currentUpdateAction = (() => TrapsUpdate());
                    break;
            }
            return true;
        }

        private bool HandleTrapButtonClicked(TrapButtonClicked e)
        {
            SetTrap(e.Template);
            _buildResources -= Mathf.RoundToInt(e.Template.GetNumericParameters()[StaticParameterTranslator.PRICE]);
            _eventService.SendMessage(new BuildResourcesChanged(_buildResources));
            return true;
        }

        private bool HandleResetTrapButtonClicked(ResetTrapButtonClicked e)
        {
            //trap to pool
            _traps[_activeElementIndex].Remove();
            _factory.ReturnToPool(_traps[_activeElementIndex].gameObject, _traps[_activeElementIndex].GetModel().Template.GetId());
            //return materials
            _buildResources += e.Price;
            _eventService.SendMessage(new BuildResourcesChanged(_buildResources));
            //found from pool
            var description = FactoryDescriptionBuilder.Object()
                .Type(EObjectType.Base)
                .Kind(StaticParameterTranslator.FOUNDAMENT)
                .Parent(_buildingsParent)
                .PositionType(EPositionType.World)
                .Position(_buildPositions[_activeElementIndex].position).Build();
            var foundament = _factory.Create(description);
            if (foundament != null)
            {
                var trap = foundament.GetComponent<Trap>();
                trap.Setup(_activeElementIndex);
                _traps[_activeElementIndex] = trap;
            }
            _activeElementIndex = -1;
            return true;
        }

        private bool HandleEndLevel(EndLevelEvent e)
        {
            //delete traps
            for (int i = 0; i < _traps.Count; i++)
            {
                _traps[i].Remove();
                _factory.ReturnToPool(_traps[i].gameObject, _traps[i].GetModel().Template.GetId());
                _traps.RemoveAt(i);
                i--;
            }
            return true;
        }

        private void SetTrap(UnitTemplateHolder template)
        {
            //found to pool
            _traps[_activeElementIndex].Remove();
            _factory.ReturnToPool(_traps[_activeElementIndex].gameObject, StaticParameterTranslator.FOUNDAMENT);
            //trap from pool 
            var descriptionBuilder = FactoryDescriptionBuilder.Object()
                .PrefabName(string.Format("{0}{1}", StaticPathTranslator.TRAP_PATH, template.GetId()))
                .Kind(template.GetId())
                .Parent(_buildingsParent)
                .PositionType(EPositionType.World);
            var description = descriptionBuilder.Position(_buildPositions[_activeElementIndex].position).Build();
            var newTrap = _factory.Create(description);
            if (newTrap != null)
            {
                var trapScript = newTrap.GetComponent<Trap>();
                trapScript.Setup(_activeElementIndex);
                trapScript.SetModel(new UnitModel(template));
                _traps[_activeElementIndex] = trapScript;
            }
        }

        private void CreateFoundaments(int slots)
        {
            var descriptionBuilder = FactoryDescriptionBuilder.Object()
                .Type(EObjectType.Base)
                .Kind(StaticParameterTranslator.FOUNDAMENT)
                .Parent(_buildingsParent)
                .PositionType(EPositionType.World);
            for (int i = 0; i < slots; i++)
            {
                var description = descriptionBuilder.Position(_buildPositions[i].position).Build();
                var foundament = _factory.Create(description);
                if (foundament != null)
                {
                    var trap = foundament.GetComponent<Trap>();
                    trap.Setup(i);
                    trap.SetModel(new UnitModel(_balanceService.GetUpgradedTrap(StaticParameterTranslator.FOUNDAMENT)));
                    _traps.Add(trap);
                }
            }
        }

        private void CreateMainTrap()
        {
            var mainTrapDescription = FactoryDescriptionBuilder.Object()
                .Type(EObjectType.Base)
                .Kind(StaticParameterTranslator.MAIN_TRAP)
                .Parent(_buildingsParent)
                .PositionType(EPositionType.World);
            var mainTrap = _factory.Create(mainTrapDescription.Build()).GetComponent<Trap>();
            mainTrap.SetModel(new UnitModel(_balanceService.GetUpgradedTrap(StaticParameterTranslator.MAIN_TRAP)));
            _traps.Add(mainTrap);
        }
    }
}
