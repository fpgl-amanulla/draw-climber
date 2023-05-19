using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

namespace project.climber
{
    public class MeshGenerator : MonoBehaviour
    {
        public GameObject leftArmF, rightArmF;
        public GameObject leftArmB, rightArmB;
        public Camera _mainCamera;

        //public Camera _drawCamera;
        public MeshCollider drawArea;
        public float lineThickness = .25f;

        private GameObject _drawing;
        private Mesh _drawingMesh;

        public PaintSelector paintSelector;
        private bool IsCursorInDrawArea => drawArea.bounds.Contains(GetMousePos(11));

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (paintSelector.IsCursorInDrawArea)
                    StartCoroutine(Draw());
            }

            if (Input.GetMouseButtonUp(0))
            {
                StopAllCoroutines();

                if (_drawing == null && _drawing.GetComponent<MeshFilter>() == null)
                {
                    Destroy(_drawing);
                    return;
                }

                Redraw();
                CalculateNormals();

                _drawingMesh = _drawing.GetComponent<MeshFilter>().mesh;

                SetNewMesh(leftArmF, rightArmF, leftArmB, rightArmB);
                Destroy(_drawing);
            }
        }

        private void SetNewMesh(params GameObject[] wheels)
        {
            for (int i = 0; i < wheels.Length; i++)
            {
                MeshFilter meshFilter = wheels[i].GetComponent<MeshFilter>();

                Mesh newMesh = new Mesh
                {
                    vertices = _drawingMesh.vertices,
                    triangles = _drawingMesh.triangles,
                    normals = _drawingMesh.normals
                };


                meshFilter.mesh = newMesh;
                Vector3 centerPoint = GetCenterPoint(meshFilter);
                Vector3[] displacementVertices = GetDisplacementVertices(meshFilter.mesh.vertices, centerPoint);
                meshFilter.mesh.vertices = displacementVertices;
                meshFilter.mesh.RecalculateBounds();

                Destroy(wheels[i].GetComponent<MeshCollider>());
                wheels[i].AddComponent<MeshCollider>().convex = true;
                wheels[i].GetComponent<MeshCollider>().sharedMesh = newMesh;
            }
        }

        private Vector3 GetCenterPoint(MeshFilter originalFilter)
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

        private Vector3[] GetDisplacementVertices(Vector3[] vertices, Vector3 center)
        {
            Vector3[] displacedVertices = new Vector3[vertices.Length];

            for (int i = 0; i < vertices.Length; i++)
            {
                displacedVertices[i] = vertices[i] - center;
            }

            return displacedVertices;
        }


        public GameObject GetMesh() => _drawing;

        private IEnumerator Draw()
        {
            _drawing = new GameObject("Drawing")
            {
                transform =
                {
                    localScale = new Vector3(1, 1, 0)
                },
                gameObject =
                {
                    layer = LayerMask.NameToLayer("Line")
                }
            };

            _drawing.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = _drawing.AddComponent<MeshRenderer>();
            meshRenderer.material.color = Color.black;
            meshRenderer.material.SetFloat(Shader.PropertyToID("_Glossiness"), 0);

            Mesh mesh = new Mesh();

            List<Vector3> vertices = new List<Vector3>(new Vector3[8]);

            Vector3 startPos = GetMousePos();
            Vector3 temp = new Vector3(startPos.x, startPos.y, 0.5f);
            for (int i = 0; i < vertices.Count; i++)
            {
                vertices[i] = temp;
            }

            List<int> triangles = new List<int>(new int[36]);

            //Font Face
            triangles[0] = 0;
            triangles[1] = 2;
            triangles[2] = 1;
            triangles[3] = 0;
            triangles[4] = 3;
            triangles[5] = 2;

            //Top Face
            triangles[6] = 2;
            triangles[7] = 3;
            triangles[8] = 4;
            triangles[9] = 2;
            triangles[10] = 4;
            triangles[11] = 5;

            //Right Face
            triangles[12] = 1;
            triangles[13] = 2;
            triangles[14] = 5;
            triangles[15] = 1;
            triangles[16] = 5;
            triangles[17] = 6;

            //Left Face
            triangles[18] = 0;
            triangles[19] = 7;
            triangles[20] = 4;
            triangles[21] = 0;
            triangles[22] = 4;
            triangles[23] = 3;

            //Back Face
            triangles[24] = 5;
            triangles[25] = 4;
            triangles[26] = 7;
            triangles[27] = 5;
            triangles[28] = 7;
            triangles[29] = 6;

            //Back Face
            triangles[30] = 0;
            triangles[31] = 6;
            triangles[32] = 7;
            triangles[33] = 0;
            triangles[34] = 1;
            triangles[35] = 6;

            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();

            _drawing.GetComponent<MeshFilter>().mesh = mesh;

            Vector3 lastMousePos = startPos;

            while (IsCursorInDrawArea)
            {
                float minDistance = .1f;
                float distance = Vector3.Distance(GetMousePos(), lastMousePos);
                while (distance < minDistance)
                {
                    distance = Vector3.Distance(GetMousePos(), lastMousePos);
                    yield return null;
                }

                vertices.AddRange(new Vector3[4]);
                triangles.AddRange(new int[30]);

                int vIndex = vertices.Count - 8;

                int vIndex0 = vIndex + 3;
                int vIndex1 = vIndex + 2;
                int vIndex2 = vIndex + 1;
                int vIndex3 = vIndex + 0;

                int vIndex4 = vIndex + 4;
                int vIndex5 = vIndex + 5;
                int vIndex6 = vIndex + 6;
                int vIndex7 = vIndex + 7;

                Vector3 currentMousePos = GetMousePos();
                Vector3 mouseForwardVector = (currentMousePos - lastMousePos).normalized;

                Vector3 topRightVertex =
                    currentMousePos + Vector3.Cross(mouseForwardVector, Vector3.back) * lineThickness;
                Vector3 bottomRightVertex =
                    currentMousePos + Vector3.Cross(mouseForwardVector, Vector3.forward) * lineThickness;
                Vector3 topLeftVertex = new Vector3(topRightVertex.x, topRightVertex.y, 1);
                Vector3 bottomLeftVertex = new Vector3(bottomRightVertex.x, bottomRightVertex.y, 1);

                vertices[vIndex4] = topLeftVertex;
                vertices[vIndex5] = topRightVertex;
                vertices[vIndex6] = bottomRightVertex;
                vertices[vIndex7] = bottomLeftVertex;

                int tIndex = triangles.Count - 30;

                //Top Face
                triangles[tIndex + 0] = vIndex2;
                triangles[tIndex + 1] = vIndex3;
                triangles[tIndex + 2] = vIndex4;
                triangles[tIndex + 3] = vIndex2;
                triangles[tIndex + 4] = vIndex4;
                triangles[tIndex + 5] = vIndex5;

                triangles[tIndex + 6] = vIndex1;
                triangles[tIndex + 7] = vIndex2;
                triangles[tIndex + 8] = vIndex5;
                triangles[tIndex + 9] = vIndex1;
                triangles[tIndex + 10] = vIndex5;
                triangles[tIndex + 11] = vIndex6;


                triangles[tIndex + 12] = vIndex0;
                triangles[tIndex + 13] = vIndex7;
                triangles[tIndex + 14] = vIndex4;
                triangles[tIndex + 15] = vIndex0;
                triangles[tIndex + 16] = vIndex4;
                triangles[tIndex + 17] = vIndex3;

                triangles[tIndex + 18] = vIndex5;
                triangles[tIndex + 19] = vIndex4;
                triangles[tIndex + 20] = vIndex7;
                triangles[tIndex + 21] = vIndex5;
                triangles[tIndex + 22] = vIndex7;
                triangles[tIndex + 23] = vIndex6;

                triangles[tIndex + 24] = vIndex0;
                triangles[tIndex + 25] = vIndex6;
                triangles[tIndex + 26] = vIndex7;
                triangles[tIndex + 27] = vIndex0;
                triangles[tIndex + 28] = vIndex1;
                triangles[tIndex + 29] = vIndex6;

                mesh.vertices = vertices.ToArray();
                mesh.triangles = triangles.ToArray();

                lastMousePos = GetMousePos();
                yield return null;
            }
        }

        private void Redraw()
        {
            Mesh mesh = _drawing.GetComponent<MeshFilter>().mesh;

            Vector3[] vertices = mesh.vertices;

            for (int i = 1; i < vertices.Length; i++)
            {
                vertices[i] = new Vector3(vertices[i].x + (vertices[0].x * -1), vertices[i].y + (vertices[0].y * -1),
                    vertices[i].z + (vertices[0].z * -1));
            }

            vertices[0] = Vector3.zero;
            mesh.vertices = vertices;
        }

        private void CalculateNormals()
        {
            new MeshImporter(_drawing).Import();
            ProBuilderMesh proBuilderMesh = _drawing.GetComponent<ProBuilderMesh>();

            Normals.CalculateNormals(proBuilderMesh);

            proBuilderMesh.ToMesh();
            proBuilderMesh.Refresh();
        }

        private Vector3 GetMousePos(float z = 10)
        {
            return _mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, z));
        }
    }
}