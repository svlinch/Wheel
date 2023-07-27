using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class EnemyStatusCanvas : MonoBehaviour
    {
        [SerializeField]
        private Slider _healthBar;

        public void CheckoutHealth(float newValue)
        {
            _healthBar.value = Mathf.Max(0.15f, newValue);
        }
    }
}