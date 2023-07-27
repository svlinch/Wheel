using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class TrapStatusCanvas : MonoBehaviour
    {
        [SerializeField]
        private Slider _healthBar;
        [SerializeField]
        private Canvas _canvas;

        public void ChangeState(bool state)
        {
            _canvas.enabled = state;
        }

        public void ChangeHealth(float newAmount)
        {
            _healthBar.value = Mathf.Max(0.15f, newAmount);
        }
    }
}