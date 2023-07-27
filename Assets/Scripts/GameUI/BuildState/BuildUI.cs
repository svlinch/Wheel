using System.Collections.Generic;
using UnityEngine;
using Zenject;
using TMPro;
using UnityEngine.UI;
using Assets.Scripts.Factories;
using Assets.Scripts.GameData;
using Assets.Scripts.Events;

namespace Assets.Scripts.UI
{
    public class BuildUI : MonoBehaviour
    {
        #region Injection
        private EventService _eventService;
        private SubscriptionHolder _subscriptions;
        private BalanceService _balanceService;
        private IObjectFactory _factory;

        [Inject]
        public void Inject(EventService eventService, BalanceService balanceService, MainFactory factory)
        {
            _eventService = eventService;
            _balanceService = balanceService;
            _factory = factory;

            _subscriptions = new SubscriptionHolder(_eventService);
            _subscriptions.Subscribe<BuildResourcesChanged>(HandleResourcesChanged);
            _subscriptions.Subscribe<GameStateChangedEvent>(HandleGameStateChanged);
            _subscriptions.Subscribe<BaseElementClicked>(HandleBaseElementClicked);
            _subscriptions.Subscribe<TrapButtonClicked>(HandleTrapButtonClicked);

            _resetButton.onClick.AddListener(ResetButtonClickHandle);
            _startBattleButton.onClick.AddListener(StartButtonClickHandle);
        }
        #endregion

        [SerializeField]
        private Canvas _canvas;
        [SerializeField]
        private TextMeshProUGUI _resourcedField;
        [SerializeField]
        private Button _resetButton;
        [SerializeField]
        private Button _startBattleButton;
        [SerializeField]
        private Transform _buttonsHolder;

        private UnitTemplateHolder _currentActiveTrap;
        private int _currentMaterials;
        private Dictionary<string, TrapButton> _trapButtons = new Dictionary<string, TrapButton>();

        private bool HandleResourcesChanged(BuildResourcesChanged e)
        {
            _currentMaterials = e.Number;
            _resourcedField.text = e.Number.ToString();
            return true;
        }

        private bool HandleGameStateChanged(GameStateChangedEvent e)
        {
            switch (e.NewState)
            {
                case EGameState.BaseState:
                    _canvas.enabled = true;
                    CheckoutShopButtons();
                    SetState(false, false);
                    break;
                default: _canvas.enabled = false; break;
            }
            return true;
        }

        private bool HandleBaseElementClicked(BaseElementClicked e)
        {
            if (e.Model == null)
            {
                CheckoutTrapsPrices();
                _currentActiveTrap = null;
                SetState(true, false);
            }
            else
            {
                _currentActiveTrap = e.Model.Template;
                SetState(false, true);
            }
            return true;
        }

        private bool HandleTrapButtonClicked(TrapButtonClicked e)
        {
            SetState(false, false);
            return true;
        }

        private void ResetButtonClickHandle()
        {
            SetState(true, false);
            _eventService.SendMessage(new ResetTrapButtonClicked(Mathf.RoundToInt(_currentActiveTrap.GetNumericParameters()[StaticParameterTranslator.PRICE])));
        }

        private void StartButtonClickHandle()
        {
            _eventService.SendMessage(new GameStateChangedEvent(EGameState.GamePlayState));
        }

        private void CheckoutShopButtons()
        {
            var descriptionBuilder = FactoryDescriptionBuilder.Object()
                   .PrefabName(StaticPathTranslator.TRAP_BUTTON)
                   .Parent(_buttonsHolder);

            var traps = _balanceService.GetUpgradedTraps();
            for (int i = 0; i < traps.Count; i++)
            {
                if (traps[i].CheckBuildable())
                {
                    continue;
                }
                if (_trapButtons.ContainsKey(traps[i].GetId()))
                {
                    _trapButtons[traps[i].GetId()].UpdateTemplate(traps[i]);
                }
                else
                {
                    var description = descriptionBuilder.Build();
                    var button = _factory.Create(description);
                    if (button != null)
                    {
                        var buttonScript = button.GetComponent<TrapButton>();
                        buttonScript.Initialize(traps[i]);
                        _trapButtons.Add(traps[i].GetId(), buttonScript);
                    }
                }
            }
        }

        private void CheckoutTrapsPrices()
        {
            foreach (var key in _trapButtons.Keys)
            {
                _trapButtons[key].CheckoutPrice(_currentMaterials);
            }
        }

        private void SetState(bool shop, bool resetButton)
        {
            _buttonsHolder.gameObject.SetActive(shop);
            _resetButton.gameObject.SetActive(resetButton);
        }
    }
}