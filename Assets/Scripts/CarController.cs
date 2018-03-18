using UnityEngine;

public class CarController : MonoBehaviour
{

	[Header("The grip of the car")]
	[SerializeField] private float downForce = 100.0f;

	public CarStats carStats;
	[Header("The car's center of mass")]
	[SerializeField] private Transform centerOfMass;
	[SerializeField] private WheelCollider[] wheelColliders = new WheelCollider[4];
	[SerializeField] private Transform[] wheelMeshes = new Transform[4];
	private string wallLayerName = "Environment";
	private int carIndex;

	[HideInInspector] public double steer;
	[HideInInspector] public double accelerate;

	private Rigidbody carRigidbody;

	// wheelColliders / wheelMeshes [0] : Front Left
	// wheelColliders / wheelMeshes [1] : Front Right
	// wheelColliders / wheelMeshes [2] : Back Left
	// wheelColliders / wheelMeshes [3] : Back Right


	void Start()
	{
		// Inicializálás
		carIndex = carStats.index;
		carStats.isAlive = true;
		JointSpring[] suspensions = new JointSpring[4];
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

		// Megadja az auto tomegkozeppontjat, hogy ne boruljon fel.
		carRigidbody = GetComponent<Rigidbody>();
		carRigidbody.centerOfMass = centerOfMass.localPosition;
	}

	void FixedUpdate()
	{
		// A 0 indexű autót vezetheti a player, ha be van pipálva a checkbox a GameManagerben
		if (carIndex == 0 && CarGameManager.Instance.manualControl)
		{
			// Kanyarodas (balra jobbra).
			steer = Input.GetAxis("Horizontal");
			// Gyorsulas (fel le).
			accelerate = Input.GetAxis("Vertical");
		}


		for (int i = 0; i < 4; i++)
		{
			// Ha az autó nem ütközött falnak, csak akkor lehet irányítani a motort
			if (carStats.isAlive)
			{
				// A motor nyomateka = max nyomatek * gyorsulas.
				wheelColliders[i].motorTorque = carStats.maxTorque * (float)accelerate;
			}
			else
			{
				wheelColliders[i].motorTorque = 0;
			}
		}

		// Jobb tapadása lesz az autónak
		wheelColliders[0].attachedRigidbody.AddForce(
			(-transform.up) * downForce * wheelColliders[0].attachedRigidbody.velocity.magnitude);

		// Elso kerekek maximum fordulasi szoge.
		float finalAngle = (float)steer * carStats.turnAngle;

		// Ha az autó nem ütközött falnak, csak akkor lehet kormányozni a kerekeket
		if (carStats.isAlive)
		{
			// Az elso kerekek megkapjak a fordulasi szoget.
			wheelColliders[0].steerAngle = finalAngle;
			wheelColliders[1].steerAngle = finalAngle;
		}
		else
		{
			wheelColliders[0].steerAngle = 0;
			wheelColliders[1].steerAngle = 0;
		}

		UpdateMeshes();
	}

	// Ha az auto falnak utkozott, freeze.
	void OnCollisionEnter(Collision collision)
	{
		if (collision.collider.gameObject.layer == LayerMask.NameToLayer(wallLayerName))
		{
			CarGameManager.Instance.FreezeCar(carRigidbody, carIndex, this.transform, ref carStats.isAlive);
		}
	}

	// A kerekek mesh-eit updateli
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

	public void Freeze()
	{
		CarGameManager.Instance.FreezeCar(carRigidbody, carIndex, this.transform, ref carStats.isAlive);
	}

}
