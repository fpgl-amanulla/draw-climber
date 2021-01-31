using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private Transform rightWheel;
    [SerializeField] private Transform leftWheel;

    public Transform RightWheel { get { return rightWheel; } }
    public Transform LeftWheel { get { return leftWheel; } }


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