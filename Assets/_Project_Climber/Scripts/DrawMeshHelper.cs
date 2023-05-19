using UnityEngine;

namespace project.climber
{
    public static class DrawMeshHelper
    {
        public static Vector3 GetCenterPoint(MeshFilter originalFilter)
        {
            Vector3[] vertices = originalFilter.mesh.vertices;
            Vector3 center = Vector3.zero;

            for (int i = 0; i < vertices.Length; i++)
            {
                center += vertices[i];
            }

            center /= vertices.Length;
            return center;
        }

        public static Vector3[] GetDisplacementVertices(Vector3[] vertices, Vector3 center)
        {
            Vector3[] displacedVertices = new Vector3[vertices.Length];

            for (int i = 0; i < vertices.Length; i++)
            {
                displacedVertices[i] = vertices[i] - center;
            }

            return displacedVertices;
        }
    }
}