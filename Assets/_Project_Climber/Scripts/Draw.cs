using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace project.climber
{
    public class Draw : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        [SerializeField] private MeshGenerator meshGenerator;
        [SerializeField] private GameObject cube;

        [SerializeField] private Player player;

        [SerializeField] private Camera cam2; //for drawing

        [SerializeField] private GameObject linePrefab;
        private GameObject drawingLine; //current temp line

        private List<Vector2> touchPoints = new List<Vector2>();

        [SerializeField] float reductionRatio = 0.3f;

        public void OnPointerDown(PointerEventData eventData)
        {
            Vector2 input =
                cam2.ScreenToWorldPoint(Input.mousePosition + new Vector3(0, 0, cam2.transform.position.z * -1));
            CreateLine(input);

            //StartCoroutine(meshGenerator.Draw());
        }

        public void OnDrag(PointerEventData eventData)
        {
            Vector2 input =
                cam2.ScreenToWorldPoint(Input.mousePosition + new Vector3(0, 0, cam2.transform.position.z * -1));
            UpdateLine(input);

            //a.text = "mouse pos : " + Input.mousePosition;
            //b.text = "screentoworld : " + input;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            //print(Vector2.Distance(touchPoints[0], touchPoints[touchPoints.Count - 1]));

            //meshGenerator.StopAllCoroutines();

            //  if (touchPoints.Count > 10) //to prevent dead leg
            //  {
            player.transform.position += new Vector3(0, 3, 0);

            Destroy(player.RightWheelForward.GetChild(1).gameObject); //DELETE PREVIOUS WHEELS
            Destroy(player.LeftWheelForward.GetChild(1).gameObject); //DELETE PREVIOUS WHEELS
            Destroy(player.RightWheelBack.GetChild(1).gameObject); //DELETE PREVIOUS WHEELS
            Destroy(player.LeftWheelBack.GetChild(1).gameObject); //DELETE PREVIOUS WHEELS

            //player.HingeJoint.transform.localPosition = Vector3.zero; //Reset Motor for a little bug

            InitWheel("Wheel_Right_Forward", player.RightWheelForward);
            InitWheel("Wheel_Left_Forward", player.LeftWheelForward);
            InitWheel("Wheel_Right_Back", player.RightWheelBack);
            InitWheel("Wheel_Left_Back", player.LeftWheelBack);

            //GameObject pivot3 = new GameObject("fdsafag");
            //pivot3.transform.position = touchPoints[0];
            //drawingLine.transform.SetParent(pivot3.transform);
            //pivot3.transform.SetParent(cam2.transform);
            //pivot3.transform.localPosition = Vector3.zero;
        }

        private void InitWheel(string wheelName, Transform wheel)
        {
            var wheel1 = Instantiate(drawingLine, Vector3.zero, Quaternion.identity);

            wheel1.layer = LayerMask.NameToLayer("Wheel");
            Transform pivot1 = new GameObject(wheelName).transform;
            pivot1.position = touchPoints[touchPoints.Count / 2];
            wheel1.transform.SetParent(pivot1);

            pivot1.transform.GetChild(0).GetComponent<LineRenderer>().startWidth = .5f;
            pivot1.transform.GetChild(0).GetComponent<LineRenderer>().endWidth = .5f;

            pivot1.SetParent(wheel);
            pivot1.localPosition = Vector3.zero;
            pivot1.localEulerAngles = Vector3.zero;
            pivot1.localScale *= reductionRatio;

            //meshGenerator.GenerateMesh(pivot1.transform.GetChild(0).GetComponent<LineRenderer>());
            //Destroy(drawingLine);
        }

        public void CreateLine(Vector2 firstTouchPos)
        {
            touchPoints.Clear();
            touchPoints.Add(firstTouchPos);
            touchPoints.Add(firstTouchPos);

            drawingLine = Instantiate(linePrefab);
            var wheelObject = drawingLine.GetComponent<Wheel>();

            wheelObject.LineRenderer.SetPosition(0, touchPoints[0]);
            wheelObject.LineRenderer.SetPosition(1, touchPoints[1]);

            wheelObject.EdgeCollider.points = touchPoints.ToArray();
        }

        public void UpdateLine(Vector2 newTouchPos)
        {
            if (Vector2.Distance(newTouchPos, touchPoints[touchPoints.Count - 1]) > .2f)
            {
                touchPoints.Add(newTouchPos);

                var wheelObject = drawingLine.GetComponent<Wheel>();

                wheelObject.LineRenderer.positionCount += 1;
                wheelObject.LineRenderer.SetPosition(wheelObject.LineRenderer.positionCount - 1,
                    touchPoints[touchPoints.Count - 1]);

                wheelObject.EdgeCollider.points = touchPoints.ToArray();
            }
        }
    }
}