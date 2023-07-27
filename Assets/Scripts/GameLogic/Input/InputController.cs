using UnityEngine;
using Assets.Scripts.Units;

namespace Assets.Scripts.GameLogic
{
    public class InputController
    {
        public bool HandleUpdate()
        {
            if (Input.GetMouseButtonDown(0))
            {
                HandleMouseDown(Input.mousePosition);
            }
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