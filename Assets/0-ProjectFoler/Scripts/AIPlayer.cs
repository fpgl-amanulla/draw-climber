using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayer : MonoBehaviour
{
    [SerializeField] private HingeJoint2D joint;
    public HingeJoint2D HingeJoint { get { return joint; } }

    [SerializeField] private Transform rightWheel;
    [SerializeField] private Transform leftWheel;

    [Header("AI Wheels")]
    [SerializeField] private GameObject[] floorWheels;
    [SerializeField] private GameObject[] hugeFloorWheels;
    [SerializeField] private GameObject[] curvedRoadWheels;
    [SerializeField] private GameObject[] straightRoadWheels;

    public enum aiStatus
    {
        StraightRoad,
        CurvedRoad,
        Floor,
        HugeFloor
    }
    private aiStatus _aiStatus;

    [SerializeField] private float changeTimer = 10f;


    private bool isColliding = false;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Finish"))
        {
            GameManager.Instance.GameOver();
        }

        if (isColliding)
            return;

        //AI
        if (collision.gameObject.CompareTag("CurvedRoad"))
        {
            _aiStatus = aiStatus.CurvedRoad;
            print("curved");
            CreateWheel();
        }
        else if (collision.gameObject.CompareTag("Floor"))
        {
            _aiStatus = aiStatus.Floor;
            print("flooor");
            CreateWheel();
        }
        else if (collision.gameObject.CompareTag("HugeFloor"))
        {
            _aiStatus = aiStatus.HugeFloor;
            print("hugefloor");
            CreateWheel();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (isColliding)
            return;

        if (collision.gameObject.CompareTag("CurvedRoad") || collision.gameObject.CompareTag("Floor") || collision.gameObject.CompareTag("HugeFloor"))
        {
            //STRAIGHT ROAD
            _aiStatus = aiStatus.StraightRoad;
            CreateWheel();
        }
    }

    public GameObject GetRandomAIWheel(GameObject[] wheelArr)
    {
        return wheelArr[Random.Range(0, wheelArr.Length)];
    }

    public void CreateWheel()
    {
        isColliding = true;

        GameObject wheelPrefab = null;

        switch (_aiStatus)
        {
            case aiStatus.StraightRoad:
                wheelPrefab = GetRandomAIWheel(straightRoadWheels);
                break;
            case aiStatus.CurvedRoad:
                wheelPrefab = GetRandomAIWheel(curvedRoadWheels);
                break;
            case aiStatus.Floor:
                wheelPrefab = GetRandomAIWheel(floorWheels);
                break;
            case aiStatus.HugeFloor:
                wheelPrefab = GetRandomAIWheel(hugeFloorWheels);
                break;
        }

        //transform.position += new Vector3(0, 3, 0);

        GameObject.Destroy(rightWheel.GetChild(1).gameObject); //DELETE PREVIOUS WHEELS
        GameObject.Destroy(leftWheel.GetChild(1).gameObject); //DELETE PREVIOUS WHEELS
        HingeJoint.transform.localPosition = Vector3.zero; //Reset Motor for a little bug

        var rw = Instantiate(wheelPrefab, Vector3.zero, Quaternion.identity, rightWheel);
        rw.transform.localPosition = Vector3.zero;
        rw.transform.localEulerAngles = Vector3.zero;

        var rl = Instantiate(wheelPrefab, Vector3.zero, Quaternion.identity, leftWheel);
        rl.transform.localPosition = Vector3.zero;
        rl.transform.localEulerAngles = Vector3.zero + new Vector3(0, 0, 180);

        changeTimer = 10f;

        StartCoroutine(Reset());
    }

    IEnumerator Reset()
    {
        yield return new WaitForEndOfFrame();
        isColliding = false;
    }

    private void Update()
    {
        if(changeTimer >= 0)
        {
            changeTimer -= Time.deltaTime;
        }
        else
        {
            CreateWheel();
        }

       // if(Input.GetKeyDown(KeyCode.K)) //TEST
       //     CreateWheel();
    }
}