using UnityEngine;

public class FollowCar : MonoBehaviour
{

	[SerializeField] private float movementSpeed = 0.5f; // 8f
	[SerializeField] private float rotationSpeed = 0.5f; // 10f

	public Transform targetCar;

	public Vector3 offset = new Vector3(0.62f, 5.83f, -7.5f);
	private bool controlledByPlayer = false;

	void Start()
	{
		controlledByPlayer = GameManager.Instance.manualControl;
	}

	void FixedUpdate()
	{

		// Az auto poziciojat koveti a kamera
		Vector3 desiredPosition = targetCar.position + (targetCar.rotation * offset);
		Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * movementSpeed);
		transform.position = smoothedPosition;

		// Ha irányítjuk az egyik autót, akkor a kamera az autóra néz, nem fog annyira forogni
		if (controlledByPlayer)
		{
			Quaternion smoothedRotation = Quaternion.Slerp(transform.rotation, targetCar.rotation, Time.deltaTime * rotationSpeed);
			transform.rotation = smoothedRotation;
			transform.LookAt(targetCar);
		}
		else
		{
			// Lágyabb kamera forgás, azonban zavaró ha akkor is így forog, amikor vezetjük az autót
			Quaternion toRot = Quaternion.LookRotation(targetCar.position - transform.position, targetCar.up);
			Quaternion curRot = Quaternion.Slerp(transform.rotation, toRot, Time.deltaTime * rotationSpeed);
			transform.rotation = curRot;

		}


	}

}
