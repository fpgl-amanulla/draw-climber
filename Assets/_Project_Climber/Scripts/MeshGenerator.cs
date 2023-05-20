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
        [Header("Wheels")] [SerializeField] private GameObject leftArmF;
        [SerializeField] private GameObject rightArmF;
        [SerializeField] private GameObject leftArmB;
        [SerializeField] private GameObject rightArmB;

        [Header("Camera")] [SerializeField] private Camera _mainCamera;

        [Header("DrawArea")] [SerializeField] private MeshCollider drawArea;
        [SerializeField] private float lineThickness = .25f;

        private GameObject _drawing;

        private Mesh _drawingMesh;

        private bool _isDrawingStarted;

        private Coroutine _drawCoroutine;

        private bool IsCursorInDrawArea => drawArea.bounds.Contains(GetMousePos(11));

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (IsCursorInDrawArea)
                {
                    _isDrawingStarted = true;
                    _drawCoroutine = StartCoroutine(DrawMesh());
                }
            }

            if (Input.GetMouseButton(0) && !IsCursorInDrawArea && _isDrawingStarted) StopDrawMeshCallBack();

            if (Input.GetMouseButtonUp(0))
            {
                if (!_isDrawingStarted) return;
                StopDrawMeshCallBack();
            }
        }

        private void StopDrawMeshCallBack()
        {
            _isDrawingStarted = false;
            StopCoroutine(_drawCoroutine);

            RedrawMesh();
            CalculateNormals();

            _drawingMesh = _drawing.GetComponent<MeshFilter>().mesh;

            ApplyNewWheelMesh(leftArmF, rightArmF, leftArmB, rightArmB);
            Destroy(_drawing);
        }

        private void ApplyNewWheelMesh(params GameObject[] wheels)
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
                Vector3 centerPoint = DrawMeshHelper.GetCenterPoint(meshFilter);
                Vector3[] displacementVertices =
                    DrawMeshHelper.GetDisplacementVertices(meshFilter.mesh.vertices, centerPoint);
                meshFilter.mesh.vertices = displacementVertices;
                meshFilter.mesh.RecalculateBounds();

                // Vector3 originScale = wheels[i].transform.localScale;
                // wheels[i].transform.localScale += Vector3.one * .25f;

                Destroy(wheels[i].GetComponent<MeshCollider>());
                wheels[i].AddComponent<MeshCollider>().convex = true;
                wheels[i].GetComponent<MeshCollider>().sharedMesh = newMesh;
            }
        }

        private IEnumerator DrawMesh()
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

            List<int> triangles = new List<int>(new int[36])
            {
                //Font Face
                [0] = 0,
                [1] = 2,
                [2] = 1,
                [3] = 0,
                [4] = 3,
                [5] = 2,
                //Top Face
                [6] = 2,
                [7] = 3,
                [8] = 4,
                [9] = 2,
                [10] = 4,
                [11] = 5,
                //Right Face
                [12] = 1,
                [13] = 2,
                [14] = 5,
                [15] = 1,
                [16] = 5,
                [17] = 6,
                //Left Face
                [18] = 0,
                [19] = 7,
                [20] = 4,
                [21] = 0,
                [22] = 4,
                [23] = 3,
                //Back Face
                [24] = 5,
                [25] = 4,
                [26] = 7,
                [27] = 5,
                [28] = 7,
                [29] = 6,
                //Back Face
                [30] = 0,
                [31] = 6,
                [32] = 7,
                [33] = 0,
                [34] = 1,
                [35] = 6
            };

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

        private void RedrawMesh()
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

        private Vector3 GetMousePos(float z = 10) =>
            _mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, z));
    }
}