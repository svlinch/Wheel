using UnityEngine;
using UnityEngine.UI;
using Zenject;
using TMPro;
using Assets.Scripts.GameData;
using Assets.Scripts.Events;

namespace Assets.Scripts.UI
{
    public class TrapButton : MonoBehaviour
    {
        #region Injection
        private EventService _eventService;

        [Inject]
        public void Inject(EventService eventService)
        {
            _eventService = eventService;
        }
        #endregion

        [SerializeField]
        private Button _button;
        [SerializeField]
        private TextMeshProUGUI _descriptionField;

        private UnitTemplateHolder _trapTemplate;

        public void Initialize(UnitTemplateHolder template)
        {
            _trapTemplate = template;

            _descriptionField.text = string.Format("{0}: {1}", _trapTemplate.GetId(), _trapTemplate.GetNumericParameters()[StaticParameterTranslator.PRICE]);
            _button.onClick.AddListener(ClickHandle);
        }

        public void CheckoutPrice(int price)
        {
            _button.interactable = _trapTemplate.GetNumericParameters()[StaticParameterTranslator.PRICE] <= price;
        }

        public void UpdateTemplate(UnitTemplateHolder template)
        {
            _trapTemplate = template;
            _descriptionField.text = string.Format("{0}: {1}", _trapTemplate.GetId(), _trapTemplate.GetNumericParameters()[StaticParameterTranslator.PRICE]);
        }

        private void ClickHandle()
        {
            _eventService.SendMessage(new TrapButtonClicked(_trapTemplate));
        }
    }
}