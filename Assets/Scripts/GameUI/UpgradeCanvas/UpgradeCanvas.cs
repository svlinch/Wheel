using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using TMPro;
using System.Linq;
using Assets.Scripts.Factories;
using UnityEngine.UI;
using Assets.Scripts.GameData;
using Assets.Scripts.GameLogic;
using Assets.Scripts.Events;

namespace Assets.Scripts.UI
{
    public class UpgradeCanvas : MonoBehaviour, IInitialize
    {
        #region Injection
        private SubscriptionHolder _subscriptions;
        private PlayerData _data;
        private BalanceService _balanceService;
        private IObjectFactory _factory;
        private GamePlayController _gamePlayController;
        private LevelsCanvas _levelsCanvas;

        [Inject]
        public void Inject(EventService eventService, PlayerData data, BalanceService balanceService, MainFactory factory, GamePlayController gamePlayController,
                        LevelsCanvas levelsCanvas)
        {
            _data = data;
            _balanceService = balanceService;
            _factory = factory;
            _gamePlayController = gamePlayController;
            _levelsCanvas = levelsCanvas;

            _subscriptions = new SubscriptionHolder(eventService);
            _subscriptions.Subscribe<UpgradeTaken>(HandleUpgradeTaken);
        }

        #endregion

        [SerializeField]
        private Canvas _canvas;
        [SerializeField]
        private TextMeshProUGUI _pointsText;
        [SerializeField]
        private Button _nextLevelButton;
        [SerializeField]
        private Button _menuButton;
        [SerializeField]
        private ScrollRect _rect;
        private float _startWidth;

        private Dictionary<string, UpgradeTree> _trees = new Dictionary<string, UpgradeTree>();

        public IEnumerator Initialize()
        {
            var maxWidth = 0f;
            var trees = _balanceService.GetUpgradeTrees();

            var description = FactoryDescriptionBuilder.Object()
                .PrefabName(StaticPathTranslator.UPGRADE_TREE)
                .Parent(_rect.content).Build();

            var _startWidth = _rect.content.rect.width;

            foreach (var tree in trees)
            {
                var treeObject = _factory.Create(description).GetComponent<UpgradeTree>();
                treeObject.Initialize(tree);
                _trees.Add(tree.Id, treeObject);

                yield return null;

                var width = treeObject.GetComponent<RectTransform>().rect.width;
                if (maxWidth < width)
                {
                    maxWidth = width;
                }
            }

            var group = _rect.content.GetComponent<VerticalLayoutGroup>();
            var item = _trees.First().Value;
            _rect.content.sizeDelta = new Vector2(maxWidth + group.padding.left + item.GetComponent<HorizontalLayoutGroup>().padding.left - _rect.content.rect.width,
                                                  group.padding.top + group.padding.bottom + group.spacing * (trees.Count - 1) + trees.Count * item.GetComponent<RectTransform>().sizeDelta.y);

            _nextLevelButton.onClick.AddListener(NextLevelHandleClick);
            _menuButton.onClick.AddListener(MenuHandleClick);
        }

        public void Open()
        {
            var takenUpgrades = _data.GetTrapsUpgradesList();
            var points = _data.GetUpgradePoints();
            foreach (var tree in _trees)
            {
                var currentProgress = takenUpgrades.FirstOrDefault(x => tree.Key.Equals(x.TreeId));
                if (currentProgress != null)
                {
                    tree.Value.Checkout(currentProgress, points);
                }
            }
            _pointsText.text = points.ToString();
            _rect.content.anchoredPosition = new Vector2(_rect.content.rect.width - _startWidth, 0f);
            _canvas.enabled = true;
        }

        public Type Type()
        {
            return GetType();
        }

        private bool HandleUpgradeTaken(UpgradeTaken e)
        {
            var takenUpgrades = _data.GetTrapsUpgradesList().First(x => x.TreeId.Equals(e.TreeId));
            var points = _data.GetUpgradePoints();

            _trees[e.TreeId].Checkout(takenUpgrades, points);
            _pointsText.text = points.ToString();
            return true;
        }

        private void NextLevelHandleClick()
        {
            _canvas.enabled = false;
            _gamePlayController.StartGame(true);
        }

        private void MenuHandleClick()
        {
            _canvas.enabled = false;
            _levelsCanvas.Open();
        }
    }
}