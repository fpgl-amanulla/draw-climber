using UnityEngine;

namespace project.climber
{
    public class Wheel : MonoBehaviour
    {
        [SerializeField] private LineRenderer lineRenderer;
        public LineRenderer LineRenderer { get { return lineRenderer; } 
        }

        [SerializeField] private EdgeCollider2D edgeCollider;
        public EdgeCollider2D EdgeCollider { get { return edgeCollider; } }
    }
}
