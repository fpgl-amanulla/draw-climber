using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private Transform rightWheelForward;
    [SerializeField] private Transform leftWheelForward;
    [SerializeField] private Transform rightWheelBack;
    [SerializeField] private Transform leftWheelBack;

    public Transform RightWheelForward => rightWheelForward;
    public Transform LeftWheelForward => leftWheelForward;
    public Transform RightWheelBack => rightWheelBack;
    public Transform LeftWheelBack => leftWheelBack;


    [SerializeField] private HingeJoint2D joint;
    public HingeJoint2D HingeJoint { get { return joint; } }


    //[SerializeField] private Rigidbody2D rb;

    //[SerializeField] float fakeGravity = 3.0f;

    //public bool onTheGround;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Finish"))
        {
            GameManager.Instance.Win();
        }
    }

    //void LateUpdate()
    //{
    //    if(!onTheGround) rb.AddForce(-transform.up * fakeGravity);
    //}
}