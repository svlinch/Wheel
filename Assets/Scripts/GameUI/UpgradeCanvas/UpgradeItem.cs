using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.GameData;
using Assets.Scripts.Events;
using TMPro;
using System.Linq;
using Zenject;

namespace Assets.Scripts.UI
{
    public class UpgradeItem : MonoBehaviour
    {
        #region Injection
        private EventService _eventService;

        [Inject]
        private void Inject(EventService eventService)
        {
            _eventService = eventService;
        }
        #endregion

        public enum EUpgradeState
        {
            Blocked,
            Taken,
            Available
        }

        [SerializeField]
        private Button _button;
        [SerializeField]
        private TextMeshProUGUI _descriptionText;
        [SerializeField]
        private TextMeshProUGUI _priceText;

        private UpgradeTemplate _template;
        private string _treeId;

        public EUpgradeState State { get; private set; }

        public void Initialize(UpgradeTemplate template, string treeId)
        {
            _template = template;
            _treeId = treeId;

            _descriptionText.text = template.Id;
            _priceText.text = template.Price.ToString();

            _button.onClick.AddListener(HandleClick);
        }

        public void SetState(EUpgradeState state)
        {
            State = state;
            switch (State)
            {
                case EUpgradeState.Available: _button.interactable = true; break;
                case EUpgradeState.Blocked: _button.interactable = false; break;
                case EUpgradeState.Taken: _button.interactable = false; break;
            }
        }

        public void CheckPrice(int points)
        {
            if (points >= _template.Price)
            {
                SetState(EUpgradeState.Available);
            }
            else
            {
                SetState(EUpgradeState.Blocked);
            }
        }

        public bool CheckCondition(UpgradeProgress progress)
        {
            if (string.IsNullOrEmpty(_template.ActivateCondition))
            {
                return true;
            }
            var condition = progress.Upgrades.Exists(x => x.Equals(_template.ActivateCondition));
            return condition;
        }

        private void HandleClick()
        {
            _eventService.SendMessage(new UpgradeTaken(_treeId, _template.Id, _template.Price));
        }
    }
}