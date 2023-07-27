using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Assets.Scripts.Utility;

namespace Assets.Scripts.UI
{
    public class SettingsCanvas : MonoBehaviour
    {
        #region Injection
        private ITimeService _timeService;

        [Inject]
        private void Inject(ITimeService timeService)
        {
            _timeService = timeService;
        }
        #endregion

        [SerializeField]
        private Button _resumeButton;
        [SerializeField]
        private Button _openButton;
        [SerializeField]
        private GameObject _partToHide;

        private void Awake()
        {
            _resumeButton.onClick.AddListener(HandleResume);
            _openButton.onClick.AddListener(HandleOpen);
        }

        private void HandleOpen()
        {
            _partToHide.SetActive(true);
            _timeService.SetPauseState(true);
        }

        private void HandleResume()
        {
            _partToHide.SetActive(false);
            _timeService.SetPauseState(false);
        }
    }
}