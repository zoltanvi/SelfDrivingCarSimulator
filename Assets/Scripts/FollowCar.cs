using UnityEngine;

public class FollowCar : MonoBehaviour
{

	[SerializeField] private float movementSpeed = 8f;
	[SerializeField] private float rotationSpeed = 10f;
	public Transform targetCar;

	public Vector3 offset = new Vector3(0.62f, 5.83f, -7.5f);


	void FixedUpdate()
	{
		Vector3 desiredPosition =
		  targetCar.position + (targetCar.rotation * offset);
		Vector3 smoothedPosition =
		  Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * movementSpeed);
		transform.position = smoothedPosition;

		Quaternion smoothedRotation =
		  Quaternion.Slerp(transform.rotation, targetCar.rotation, Time.deltaTime * rotationSpeed);
		transform.rotation = smoothedRotation;
		transform.LookAt(targetCar);

		//Quaternion toRot = 
		//	Quaternion.LookRotation(targetCar.position - transform.position, targetCar.up);
		//Quaternion curRot = 
		//	Quaternion.Slerp(transform.rotation, toRot, Time.deltaTime * rotationSpeed);
		//transform.rotation = curRot;


	}

}
