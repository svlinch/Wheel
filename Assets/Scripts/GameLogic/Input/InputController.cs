using UnityEngine;
using Assets.Scripts.Units;

namespace Assets.Scripts.GameLogic
{
    public class InputController
    {
        public bool HandleUpdate()
        {
#if UNITY_ANDROID 
            if (Input.touchCount > 0)
            {
                var touches = Input.touches;
                if (touches[0].phase == TouchPhase.Ended)
                {
                    HandleMouseDown(touches[0].position);
                }
            }
#endif
#if UNITY_EDITOR
            if (Input.GetMouseButtonDown(0))
            {
                HandleMouseDown(Input.mousePosition);
            }
#endif
            return true;
        }

        private void HandleMouseDown(Vector3 clickPosition)
        {
            var ray = Camera.main.ScreenPointToRay(clickPosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.parent != null)
                {
                    var trap = hit.transform.GetComponentInParent<Trap>();
                    if (trap != null)
                    {
                        trap.HandleClick();
                    }
                }
            }
        }
    }
}