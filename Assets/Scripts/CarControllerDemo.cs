using UnityEngine;


public class CarControllerDemo : MonoBehaviour
{
    private class CarParameters
    {
        public const float TurnAngle = 55f;
        public const float Accelerate = 750f;
        public const float Brake = 400000f;
        public const float Shunt = 450f;
        public const float Spring = 10000f;
        public const float ForwardSwiftness = 1.0f;
        public const float SidewaysSwiftness = 1.7f;

        public const float DownForce = 100.0f;
        public const float SteerHelper = 0.6f;
    }

    [SerializeField] private Transform centerOfMass;
    [SerializeField] private WheelCollider[] wheelColliders = new WheelCollider[4];
    [SerializeField] private Transform[] wheelMeshes = new Transform[4];
    public bool IsPlayerControlled { get; set; }

    [HideInInspector] public float Steer;
    [HideInInspector] public float Accelerate;
    private float oldRotation;

    private Rigidbody myRigidbody;
    private float maxVelocity = 0f;

    // 1 meter/sec = 2.23693629 miles/hour	
    public float CurrentSpeed { get { return myRigidbody.velocity.magnitude * 2.23693629f; } }
    private Rigidbody carRigidbody;

    // wheelColliders / wheelMeshes [0] : Front Left
    // wheelColliders / wheelMeshes [1] : Front Right
    // wheelColliders / wheelMeshes [2] : Back Left
    // wheelColliders / wheelMeshes [3] : Back Right

    private void Start()
    {
        myRigidbody = GetComponent<Rigidbody>();

        // Inicializálás
        JointSpring[] suspensions = new JointSpring[4];
        WheelFrictionCurve[] wheelFrictionCurveForward = new WheelFrictionCurve[4];
        WheelFrictionCurve[] wheelFrictionCurveSideways = new WheelFrictionCurve[4];


        #region Setting 4 wheels
        for (int i = 0; i < 4; i++)
        {
            // A rugo beallitasa.
            suspensions[i] = wheelColliders[i].suspensionSpring;
            suspensions[i].spring = CarParameters.Spring;

            // Forward Friction - swiftness beallitasa.
            wheelFrictionCurveForward[i] = wheelColliders[i].forwardFriction;
            wheelFrictionCurveForward[i].stiffness = CarParameters.ForwardSwiftness;

            // Sideways Friction - swiftness beallitasa.
            wheelFrictionCurveSideways[i] = wheelColliders[i].sidewaysFriction;
            wheelFrictionCurveSideways[i].stiffness = CarParameters.SidewaysSwiftness;

            wheelColliders[i].suspensionSpring = suspensions[i];
            wheelColliders[i].forwardFriction = wheelFrictionCurveForward[i];
            wheelColliders[i].sidewaysFriction = wheelFrictionCurveSideways[i];
        }
        #endregion

        // Megadja az auto tomegkozeppontjat, hogy ne boruljon fel.
        carRigidbody = GetComponent<Rigidbody>();
        carRigidbody.centerOfMass = centerOfMass.localPosition;
    }

    private void FixedUpdate()
    {
        if(maxVelocity < myRigidbody.velocity.magnitude)
        {
            maxVelocity = myRigidbody.velocity.magnitude;
            Debug.Log($"Max velocity: {maxVelocity}");
        }


        // Kanyarodas (balra jobbra).
        Steer = Input.GetAxis("Horizontal");
        // Gyorsulas (fel le).
        Accelerate = Input.GetAxis("Vertical");


        for (int i = 0; i < 4; i++)
        {
            // Ha előre megy az autó
            if (Accelerate >= 0)
            {
                wheelColliders[i].motorTorque = CarParameters.Accelerate * (float)Accelerate;
                wheelColliders[i].brakeTorque = 0f;
            }
            // Ha előre megy az autó és a fék nyomva van
            else if (Accelerate < 0 && CurrentSpeed > 5 && Vector3.Angle(transform.forward, myRigidbody.velocity) < 50f)
            {
                wheelColliders[i].brakeTorque = CarParameters.Brake * -(float)Accelerate;
                wheelColliders[i].motorTorque = 0f;
            }
            // Ha tolat az autó
            else
            {
                wheelColliders[i].motorTorque = CarParameters.Shunt * (float)Accelerate;
                wheelColliders[i].brakeTorque = 0f;
            }

        }

        // Jobb tapadása lesz az autónak
        wheelColliders[0].attachedRigidbody.AddForce(
            (-transform.up) * CarParameters.DownForce * wheelColliders[0].attachedRigidbody.velocity.magnitude);

        // Elso kerekek maximum fordulasi szoge.
        float finalAngle = (float)Steer * CarParameters.TurnAngle;


        // Az elso kerekek megkapjak a fordulasi szoget.
        wheelColliders[0].steerAngle = finalAngle;
        wheelColliders[1].steerAngle = finalAngle;


        // this if is needed to avoid gimbal lock problems that will make the car suddenly shift direction
        if (Mathf.Abs(oldRotation - transform.eulerAngles.y) < 10f)
        {
            var turnadjust = (transform.eulerAngles.y - oldRotation) * CarParameters.SteerHelper;
            Quaternion velRotation = Quaternion.AngleAxis(turnadjust, Vector3.up);
            carRigidbody.velocity = velRotation * carRigidbody.velocity;
        }
        oldRotation = transform.eulerAngles.y;

        UpdateMeshes();
    }


    /// <summary>
    /// Updateli az autó kerekeinek mesh-eit (amit látsz).
    /// </summary>
    private void UpdateMeshes()
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

