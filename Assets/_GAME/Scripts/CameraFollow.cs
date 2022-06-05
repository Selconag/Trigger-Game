using UnityEngine;

public class CameraFollow : MonoBehaviour
{
	private Transform target;
	[Range(0, 3f)]
	private float smoothSpeed = 0.125f;
	[Tooltip("Determines the offset distance of target between camera to follow.")]
	[SerializeField] private Vector3 m_OffsetToTarget;


	private void Start()
	{
		target = null;
		FindCamera();
	}

	void FixedUpdate()
	{
		if (target == null) return;
		Vector3 desiredPosition = target.position + m_OffsetToTarget;
		Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
		transform.position = smoothedPosition;

		transform.LookAt(target);
	}

	private void FindCamera()
	{
		target = GameObject.FindGameObjectWithTag("Player").transform;
	}


}

