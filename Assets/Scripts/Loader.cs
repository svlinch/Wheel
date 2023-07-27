using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Assets.Scripts.GameData;
using Assets.Scripts.GameLogic;
using Assets.Scripts.UI;

namespace Assets.Scripts
{
    public class Loader : MonoBehaviour
    {
        private List<IInitialize> _initializationList;
        private GamePlayController _gamePlayController;

        [Inject]
        private void Inject(BalanceService balanceService, PlayerData playerData, GamePlayController gamePlayController, UpgradeCanvas upgradeCanvas, LevelsCanvas levelsCanvas)
        {
            _initializationList = new List<IInitialize>();
            _initializationList.Add(playerData as IInitialize);
            _initializationList.Add(balanceService as IInitialize);
            _initializationList.Add(upgradeCanvas as IInitialize);
            _initializationList.Add(levelsCanvas as IInitialize);
            _gamePlayController = gamePlayController;
        }

        private IEnumerator Start()
        {
            foreach (var toInit in _initializationList)
            {
                yield return StartCoroutine(toInit.Initialize());
                Debug.Log(string.Format("{0} initialized", toInit.Type().ToString()));
            }
            _gamePlayController.StartGame(true);
        }
    }
}