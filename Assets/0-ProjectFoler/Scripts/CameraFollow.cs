using UnityEngine;

public class CameraFollow : MonoBehaviour
{
	[SerializeField] private Transform player;

	public float smoothTime = 0.5f;

	private Vector3 velocity;

	private Vector3 offset;

	private Vector3 playerPos;

	private void Start()
	{
		offset = transform.position - player.position;
	}

	private void FixedUpdate()
	{
		transform.position = Vector3.SmoothDamp(transform.position, player.position + offset, ref velocity, smoothTime);
		playerPos = player.position + offset;
	}
}