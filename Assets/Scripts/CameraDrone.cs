using UnityEngine;

public class CameraDrone : MonoBehaviour
{

	[SerializeField] private float movementSpeed = 0.5f;
	[SerializeField] private float rotationSpeed = 0.5f;

	public Transform Target;

	private Vector3 offset = new Vector3(0.62f, 5.83f, -7.5f);
	private bool isPlayerControlling = false;

	void Awake()
	{
		DontDestroyOnLoad(gameObject);
	}

	void Start()
	{
		isPlayerControlling = GameManager.Instance.manualControl;
	}

	void FixedUpdate()
	{

		// Az auto poziciojat koveti a kamera
		Vector3 desiredPosition = Target.position + (Target.rotation * offset);
		Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * movementSpeed);
		transform.position = smoothedPosition;

		// Ha irányítjuk az egyik autót, akkor a kamera az autóra néz, nem fog annyira forogni
		if (isPlayerControlling)
		{
			Quaternion smoothedRotation = Quaternion.Slerp(transform.rotation, Target.rotation, Time.deltaTime * rotationSpeed);
			transform.rotation = smoothedRotation;
			transform.LookAt(Target);
		}
		else
		{
			// Lágyabb kamera forgás, azonban zavaró ha akkor is így forog, amikor vezetjük az autót
			Quaternion toRot = Quaternion.LookRotation(Target.position - transform.position, Target.up);
			Quaternion curRot = Quaternion.Slerp(transform.rotation, toRot, Time.deltaTime * rotationSpeed);
			transform.rotation = curRot;

		}


	}

}