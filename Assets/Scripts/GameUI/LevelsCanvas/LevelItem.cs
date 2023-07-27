using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.GameData;
using System;
using TMPro;

namespace Assets.Scripts.UI
{
    public class LevelItem : MonoBehaviour
    {
        [SerializeField]
        private Button _button;
        [SerializeField]
        private TextMeshProUGUI _levelNumberText;
        [SerializeField]
        private TextMeshProUGUI _wavesText;

        private Action<int> _callback;
        private int _index;

        public void Initialize(LevelTemplate template, Action<int> callback, int index)
        {
            _callback = callback;
            _index = index;

            _wavesText.text = string.Format(" waves: {0}", template.Waves.Length.ToString());
            _levelNumberText.text = string.Format(" level: {0}", _index.ToString());

            _button.onClick.AddListener(HandleClick);
            SetState(false);
        }

        public void SetState(bool state)
        {
            _button.interactable = state;
        }

        private void HandleClick()
        {
            _callback.Invoke(_index);
        }
    }
}
