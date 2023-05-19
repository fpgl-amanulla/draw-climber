using UnityEngine;

namespace project.climber
{
    public class PaintSelector : MonoBehaviour
    {
        public bool IsCursorInDrawArea { get; private set; }

        private void OnMouseDown()
        {
            IsCursorInDrawArea = true;
        }

        private void OnMouseExit()
        {
            IsCursorInDrawArea = false;
        }
    }
}