using UnityEngine;

public class CarController : MonoBehaviour
{

	[SerializeField] private float downforce = 100.0f;
	[SerializeField] private CarStats carStats;
	[SerializeField] private Transform centerOfMass;
	[SerializeField] private WheelCollider[] wheelColliders = new WheelCollider[4];
	[SerializeField] private Transform[] wheelMeshes = new Transform[4];
	private Rigidbody carRigidbody;

	// wheelColliders / wheelMeshes [0] : Front Left
	// wheelColliders / wheelMeshes [1] : Front Right
	// wheelColliders / wheelMeshes [2] : Back Left
	// wheelColliders / wheelMeshes [3] : Back Right


	void Start()
	{

		// A felfuggeszteseket beallitjuk a carStats-bol.
		JointSpring[] suspensions = new JointSpring[4];
		// A forward swiftnesseket beallitjuk a carStats-bol.
		WheelFrictionCurve[] wheelFrictionCurveForward = new WheelFrictionCurve[4];
		WheelFrictionCurve[] wheelFrictionCurveSideways = new WheelFrictionCurve[4];

		#region Setting 4 wheels
		for (int i = 0; i < 4; i++)
		{
			// A rugo beallitasa.
			suspensions[i] = wheelColliders[i].suspensionSpring;
			suspensions[i].spring = carStats.spring;

			// Forward Friction - swiftness beallitasa.
			wheelFrictionCurveForward[i] = wheelColliders[i].forwardFriction;
			wheelFrictionCurveForward[i].stiffness = carStats.forwardSwiftness;

			// Sideways Friction - swiftness beallitasa.
			wheelFrictionCurveSideways[i] = wheelColliders[i].sidewaysFriction;
			wheelFrictionCurveSideways[i].stiffness = carStats.sidewaysSwiftness;

			wheelColliders[i].suspensionSpring = suspensions[i];
			wheelColliders[i].forwardFriction = wheelFrictionCurveForward[i];
			wheelColliders[i].sidewaysFriction = wheelFrictionCurveSideways[i];
		}
		#endregion

		// Megadjuk az auto tomegkozeppontjat, hogy ne boruljon fel.
		carRigidbody = GetComponent<Rigidbody>();
		carRigidbody.centerOfMass = centerOfMass.localPosition;
	}

	void FixedUpdate()
	{
		// Kanyarodas (balra jobbra).
		float steer = Input.GetAxis("Horizontal");
		// Gyorsulas (fel le).
		float accelerate = Input.GetAxis("Vertical");

		for (int i = 0; i < 4; i++)
		{
			// A motor nyomateka = max nyomatek * gyorsulas.
			wheelColliders[i].motorTorque = carStats.maxTorque * accelerate;
		}

		wheelColliders[0].attachedRigidbody.AddForce(-transform.up * downforce * wheelColliders[0].attachedRigidbody.velocity.magnitude);

		// Elso kerekek maximum fordulasi szoge.
		float finalAngle = steer * carStats.turnAngle;

		// Az elso kerekek megkapjak a fordulasi szoget.
		wheelColliders[0].steerAngle = finalAngle;
		wheelColliders[1].steerAngle = finalAngle;

		UpdateMeshes();
	}

	void UpdateMeshes()
	{
		for (int i = 0; i < 4; i++)
		{
			// A kerek pozicioja.
			Vector3 pos;
			// A kerek forgasi szoge.
			Quaternion quat;

			wheelColliders[i].GetWorldPose(out pos, out quat);

			wheelMeshes[i].position = pos;
			wheelMeshes[i].rotation = quat;

		}
	}

}
