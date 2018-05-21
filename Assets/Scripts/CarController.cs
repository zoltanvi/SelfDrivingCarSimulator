using UnityEngine;

public class CarController : MonoBehaviour
{
	private CarParameters carParameters;

	public int ID { get; set; }
	public bool IsAlive { get; set; }

	[SerializeField] private Transform centerOfMass;
	[SerializeField] private WheelCollider[] wheelColliders = new WheelCollider[4];
	[SerializeField] private Transform[] wheelMeshes = new Transform[4];
	private string wallLayerName = "Environment";
	public bool IsPlayerControlled { get; set; }

	[HideInInspector] public double steer;
	[HideInInspector] public double accelerate;
	private float oldRotation;

	private Rigidbody myRigidbody;

	// 1 meter/sec = 2.23693629 miles/hour	
	public float CurrentSpeed { get { return myRigidbody.velocity.magnitude * 2.23693629f; } }


	private Rigidbody carRigidbody;

	// wheelColliders / wheelMeshes [0] : Front Left
	// wheelColliders / wheelMeshes [1] : Front Right
	// wheelColliders / wheelMeshes [2] : Back Left
	// wheelColliders / wheelMeshes [3] : Back Right


	void Awake()
	{
		carParameters = new CarParameters();
	}

	void Start()
	{
		myRigidbody = GetComponent<Rigidbody>();

		// Inicializálás
		IsAlive = true;
		JointSpring[] suspensions = new JointSpring[4];
		WheelFrictionCurve[] wheelFrictionCurveForward = new WheelFrictionCurve[4];
		WheelFrictionCurve[] wheelFrictionCurveSideways = new WheelFrictionCurve[4];


		#region Setting 4 wheels
		for (int i = 0; i < 4; i++)
		{
			// A rugo beallitasa.
			suspensions[i] = wheelColliders[i].suspensionSpring;
			suspensions[i].spring = carParameters.Spring;

			// Forward Friction - swiftness beallitasa.
			wheelFrictionCurveForward[i] = wheelColliders[i].forwardFriction;
			wheelFrictionCurveForward[i].stiffness = carParameters.ForwardSwiftness;

			// Sideways Friction - swiftness beallitasa.
			wheelFrictionCurveSideways[i] = wheelColliders[i].sidewaysFriction;
			wheelFrictionCurveSideways[i].stiffness = carParameters.SidewaysSwiftness;

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
		// Ha az autó játékos által van irányítva, akkor megkapja a vezérlést (billentyűzet)
		if (IsPlayerControlled)
		{
			// Kanyarodas (balra jobbra).
			steer = Input.GetAxis("Horizontal");
			// Gyorsulas (fel le).
			accelerate = Input.GetAxis("Vertical");
		}


		for (int i = 0; i < 4; i++)
		{
			// Ha az autó nem ütközött falnak, csak akkor lehet irányítani a motort
			if (IsAlive)
			{
				// Ha előre megy az autó
				if (accelerate >= 0)
				{
					wheelColliders[i].motorTorque = carParameters.Accelerate * (float)accelerate;
					wheelColliders[i].brakeTorque = 0f;
				}
				// Ha előre megy az autó és a fék nyomva van
				else if (accelerate < 0 && CurrentSpeed > 5 && Vector3.Angle(transform.forward, myRigidbody.velocity) < 50f)
				{
					wheelColliders[i].brakeTorque = carParameters.Brake * -(float)accelerate;
					wheelColliders[i].motorTorque = 0f;
				}
				// Ha tolat az autó
				else
				{
					wheelColliders[i].motorTorque = carParameters.Shunt * (float)accelerate;
					wheelColliders[i].brakeTorque = 0f;
				}
			}
			else
			{
				wheelColliders[i].motorTorque = 0;
			}
		}

		// Jobb tapadása lesz az autónak
		wheelColliders[0].attachedRigidbody.AddForce(
			(-transform.up) * carParameters.DownForce * wheelColliders[0].attachedRigidbody.velocity.magnitude);

		// Elso kerekek maximum fordulasi szoge.
		float finalAngle = (float)steer * carParameters.TurnAngle;

		// Ha az autó nem ütközött falnak, csak akkor lehet kormányozni a kerekeket
		if (IsAlive)
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

		// this if is needed to avoid gimbal lock problems that will make the car suddenly shift direction
		if (Mathf.Abs(oldRotation - transform.eulerAngles.y) < 10f)
		{
			var turnadjust = (transform.eulerAngles.y - oldRotation) * carParameters.SteerHelper;
			Quaternion velRotation = Quaternion.AngleAxis(turnadjust, Vector3.up);
			carRigidbody.velocity = velRotation * carRigidbody.velocity;
		}
		oldRotation = transform.eulerAngles.y;
		
		UpdateMeshes();
	}

	/// <summary>
	/// Ha az autó falnak ütközött és még nem fagyott meg,
	/// akkor megfagyasztja
	/// </summary>
	void OnCollisionEnter(Collision collision)
	{
		if (collision.collider.gameObject.layer == LayerMask.NameToLayer(wallLayerName))
		{
			Manager.Instance.FreezeCar(carRigidbody, ID, IsAlive);
			IsAlive = false;
			Manager.Instance.Cars[ID].IsAlive = false;
		}
	}

	/// <summary>
	/// Updateli az autó kerekeinek mesh-eit (amit látsz).
	/// </summary>
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

	/// <summary>
	/// Meghívja az autó ID-jére a Manager FreezeCar metódusát.
	/// </summary>
	public void Freeze()
	{
		Manager.Instance.FreezeCar(carRigidbody, ID, IsAlive);
	}

}
