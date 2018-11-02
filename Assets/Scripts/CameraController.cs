using UnityEngine;


public class CameraController : MonoBehaviour
{
	[Range(0f, 5f)] [SerializeField] private const float movementSpeed = 3.2f;
	[Range(0f, 5f)] [SerializeField] private const float rotationSpeed = 3.8f;

	public Transform CameraTarget;

	private readonly Vector3 offset = new Vector3(0.62f, 5.83f, -7.5f);

	private void Awake()
	{
		DontDestroyOnLoad(gameObject);
	}


	private void FixedUpdate()
	{

		// Az auto poziciojat koveti a kamera
		Vector3 desiredPosition = CameraTarget.position + (CameraTarget.rotation * offset);
		Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * movementSpeed);
		transform.position = smoothedPosition;

		// Ha irányítjuk az egyik autót, akkor a kamera az autóra néz, nem fog annyira forogni
		if (Master.Instance.Manager.ManualControl && Master.Instance.Manager.IsPlayerAlive)
		{
			Quaternion smoothedRotation = Quaternion.Slerp(transform.rotation, CameraTarget.rotation, Time.deltaTime * rotationSpeed);
			transform.rotation = smoothedRotation;
			transform.LookAt(CameraTarget);
		}
		else
		{
			// Lágyabb kamera forgás, ha nem mi vezetjük az autót
			Quaternion toRot = Quaternion.LookRotation(CameraTarget.position - transform.position, CameraTarget.up);
			Quaternion curRot = Quaternion.Slerp(transform.rotation, toRot, Time.deltaTime * rotationSpeed);
			transform.rotation = curRot;
		}


	}

}
