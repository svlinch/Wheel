using UnityEngine;
using UnityEngine.UI;
using Zenject;
using TMPro;
using Assets.Scripts.Events;
using Assets.Scripts.GameLogic;

namespace Assets.Scripts.UI
{
    public class EndLevelCanvas : MonoBehaviour
    {
        #region Injection
        private SubscriptionHolder _subscriptions;
        private EventService _eventService;
        private GamePlayController _gamePlayController;
        private UpgradeCanvas _upgradeCanvas;

        [Inject]
        public void Inject(EventService eventService, GamePlayController gamePlayController, UpgradeCanvas upgradeCanvas)
        {
            _eventService = eventService;
            _gamePlayController = gamePlayController;
            _upgradeCanvas = upgradeCanvas;

            _subscriptions = new SubscriptionHolder(_eventService);
            _subscriptions.Subscribe<EndLevelEvent>(HandleEndLevel);
        }
        #endregion

        [SerializeField]
        private Canvas _canvas;
        [SerializeField]
        private Button _restartButton;
        [SerializeField]
        private Button _nextLevelButton;
        [SerializeField]
        private Button _upgradesButton;
        [SerializeField]
        private TextMeshProUGUI _titleText;

        private void Awake()
        {
            _canvas.enabled = false;
            _restartButton.onClick.AddListener(HandleRestartClicked);
            _nextLevelButton.onClick.AddListener(HandleNextLevelClicked);
            _upgradesButton.onClick.AddListener(HandleUpgradesClicked);
        }

        private void HandleRestartClicked()
        {
            _canvas.enabled = false;
            _gamePlayController.StartGame(false);
        }

        private void HandleNextLevelClicked()
        {
            _canvas.enabled = false;
            _gamePlayController.StartGame(true);
        }

        private void HandleUpgradesClicked()
        {
            _canvas.enabled = false;
            _upgradeCanvas.Open();
        }

        private bool HandleEndLevel(EndLevelEvent e)
        {
            if (e.Win)
            {
                _titleText.text = StaticTextTranslator.WIN_TEXT;
                _nextLevelButton.interactable = true;
            }
            else
            {
                _titleText.text = StaticTextTranslator.LOSE_TEXT;
                _nextLevelButton.interactable = false;
            }
            _canvas.enabled = true;
            return true;
        }
    }
}