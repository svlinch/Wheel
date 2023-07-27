using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Assets.Scripts.Factories;
using Assets.Scripts.GameData;
using Assets.Scripts.GameLogic;

namespace Assets.Scripts.UI
{
    public class LevelsCanvas : MonoBehaviour, IInitialize
    {
        #region Injection
        private GamePlayController _gamePlayController;
        private IObjectFactory _factory;
        private PlayerData _data;
        private BalanceService _balanceService;

        [Inject]
        private void Inject(GamePlayController gamePlayController, MainFactory factory, PlayerData data, BalanceService balanceService)
        {
            _gamePlayController = gamePlayController;
            _factory = factory;
            _balanceService = balanceService;
            _data = data;
        }
        #endregion

        [SerializeField]
        private Canvas _canvas;
        [SerializeField]
        private ScrollRect _scrollRect;

        private List<LevelItem> _levels = new List<LevelItem>();

        public IEnumerator Initialize()
        {
            var levels = _balanceService.GetLevels();
            var description = FactoryDescriptionBuilder.Object()
                .Parent(_scrollRect.content)
                .PrefabName(StaticPathTranslator.LEVEL_ITEM)
                .Build();
            for (int i = 0; i < levels.Count; i++)
            {
                var newItem = _factory.Create(description).GetComponent<LevelItem>();
                newItem.Initialize(levels[i], LevelClickHandle, i);
                _levels.Add(newItem);
                yield return null;
            }
            _scrollRect.content.sizeDelta = new Vector2(_scrollRect.content.sizeDelta.x,
                _levels[0].transform.GetComponent<RectTransform>().rect.height * _levels.Count +
                _levels.Count * _scrollRect.content.GetComponent<VerticalLayoutGroup>().spacing);
        }

        public Type Type()
        {
            return GetType();
        }

        public void Open()
        {
            var lastLevel = _data.GetLastLevelIndex();
            for (int i = 0; i <= lastLevel; i++)
            {
                _levels[i].SetState(true);
            }
            _canvas.enabled = true;
        }

        private void LevelClickHandle(int index)
        {
            _canvas.enabled = false;
            _gamePlayController.StartSpecialLevel(index);
        }
    }
}