using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Draw : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField] private Player player;

    [SerializeField] private Camera cam2; //for drawing

    [SerializeField] private GameObject linePrefab;
    private GameObject drawingLine; //current temp line

    private List<Vector2> touchPoints = new List<Vector2>();

    [SerializeField] float reductionRatio = 0.3f;

    public void OnPointerDown(PointerEventData eventData)
    {
        Vector2 input = cam2.ScreenToWorldPoint(Input.mousePosition + new Vector3(0, 0, cam2.transform.position.z * -1));
        CreateLine(input);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 input = cam2.ScreenToWorldPoint(Input.mousePosition + new Vector3(0, 0, cam2.transform.position.z * -1));
        UpdateLine(input);

        //a.text = "mouse pos : " + Input.mousePosition;
        //b.text = "screentoworld : " + input;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        //print(Vector2.Distance(touchPoints[0], touchPoints[touchPoints.Count - 1]));

      //  if (touchPoints.Count > 10) //to prevent dead leg
      //  {
            player.transform.position += new Vector3(0, 3, 0);

            GameObject.Destroy(player.RightWheel.GetChild(1).gameObject); //DELETE PREVIOUS WHEELS
            GameObject.Destroy(player.LeftWheel.GetChild(1).gameObject); //DELETE PREVIOUS WHEELS
            player.HingeJoint.transform.localPosition = Vector3.zero; //Reset Motor for a little bug

            var wheel1 = Instantiate(drawingLine, Vector3.zero, Quaternion.identity);
            wheel1.layer = LayerMask.NameToLayer("Wheel");
            Transform pivot1 = new GameObject("Wheel_Right").transform;
            pivot1.position = touchPoints[0];
            wheel1.transform.SetParent(pivot1);

            pivot1.SetParent(player.RightWheel);
            pivot1.localPosition = Vector3.zero;
            pivot1.localEulerAngles = Vector3.zero;
            pivot1.localScale *= reductionRatio;

            ////

            var wheel2 = Instantiate(drawingLine, Vector3.zero, Quaternion.identity);
            wheel2.layer = LayerMask.NameToLayer("Wheel");
            Transform pivot2 = new GameObject("Wheel_Left").transform;
            pivot2.position = touchPoints[0];
            wheel2.transform.SetParent(pivot2);

            pivot2.SetParent(player.LeftWheel);
            pivot2.localPosition = Vector3.zero;
            pivot2.localEulerAngles = Vector3.zero + new Vector3(0, 0, 180);
            pivot2.localScale *= reductionRatio;
      //  }

        //GameObject pivot3 = new GameObject("fdsafag");
        //pivot3.transform.position = touchPoints[0];
        //drawingLine.transform.SetParent(pivot3.transform);
        //pivot3.transform.SetParent(cam2.transform);
        //pivot3.transform.localPosition = Vector3.zero;
        GameObject.Destroy(drawingLine);
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
            wheelObject.LineRenderer.SetPosition(wheelObject.LineRenderer.positionCount-1, touchPoints[touchPoints.Count - 1]);

            wheelObject.EdgeCollider.points = touchPoints.ToArray();
        }
    }
}