using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float rotationSpeed = 5f;
    public float raycastDistance = 1f;

    private void Update()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, raycastDistance))
        {
            // Calculate the desired rotation to match the ground normal
            Quaternion targetRotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
            // Smoothly rotate the player towards the ground
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // if (Physics.Raycast(transform.position, Vector3.right, out RaycastHit fhit, raycastDistance / 2))
        // {
        //     Quaternion targetRotation = Quaternion.FromToRotation(transform.up, fhit.normal) * transform.rotation;
        //     transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        // }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * raycastDistance);
    }
}