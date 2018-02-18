using UnityEngine;

public class CarController : MonoBehaviour {

     [SerializeField] private float m_Downforce = 100.0f;
     [SerializeField] private CarStats m_carStats;
     [SerializeField] private Transform m_CenterOfMass;
     [SerializeField] private WheelCollider[] m_WheelColliders = new WheelCollider[4];
     [SerializeField] private Transform[] m_WheelMeshes = new Transform[4];
     private Rigidbody m_Rigidbody;

	// wheelColliders / wheelMeshes [0] : Front Left
	// wheelColliders / wheelMeshes [1] : Front Right
	// wheelColliders / wheelMeshes [2] : Back Left
	// wheelColliders / wheelMeshes [3] : Back Right


	void Start()
     {

          // felfuggeszteseket beallitjuk a carStats-bol
          JointSpring[] suspensions = new JointSpring[4];
          // fswiftness-eket beallitjuk a carStats-bol
          WheelFrictionCurve[] wheelFrictionCurveForward = new WheelFrictionCurve[4];
          WheelFrictionCurve[] wheelFrictionCurveSideways = new WheelFrictionCurve[4];

		#region Setting 4 wheels
		for (int i = 0; i < 4; i++)
          {
               // rugo beallitasa
               suspensions[i] = m_WheelColliders[i].suspensionSpring;
               suspensions[i].spring = m_carStats.spring;

               // Forward Friction - swiftness beallitasa
               wheelFrictionCurveForward[i] = m_WheelColliders[i].forwardFriction;
               wheelFrictionCurveForward[i].stiffness = m_carStats.forwardSwiftness;

               // Sideways Friction - swiftness beallitasa
               wheelFrictionCurveSideways[i] = m_WheelColliders[i].sidewaysFriction;
               wheelFrictionCurveSideways[i].stiffness = m_carStats.sidewaysSwiftness;

               m_WheelColliders[i].suspensionSpring = suspensions[i];
               m_WheelColliders[i].forwardFriction = wheelFrictionCurveForward[i];
               m_WheelColliders[i].sidewaysFriction = wheelFrictionCurveSideways[i];
          }
		#endregion

		// megadjuk az auto tomegkozeppontjat
		m_Rigidbody = GetComponent<Rigidbody>();
          m_Rigidbody.centerOfMass = m_CenterOfMass.localPosition;
     }

     void FixedUpdate()
     {
          // kanyarodas (balra jobbra)
          float steer = Input.GetAxis("Horizontal");
          // gyorsulas (fel le)
          float accelerate = Input.GetAxis("Vertical");

          for (int i = 0; i < 4; i++)
          {
               //a motor nyomatéka = max nyomatek * gyorsulas
               m_WheelColliders[i].motorTorque = m_carStats.maxTorque * accelerate;
          }

          m_WheelColliders[0].attachedRigidbody.AddForce(-transform.up * m_Downforce * m_WheelColliders[0].attachedRigidbody.velocity.magnitude);

          // Debug.Log("The speed is: " + wheelColliders[0].motorTorque);

          // maximum fordulasi szog
          float finalAngle = steer * m_carStats.turnAngle;

          // az elso kerekek megkapjak a fordulasi szoget
          m_WheelColliders[0].steerAngle = finalAngle;
          m_WheelColliders[1].steerAngle = finalAngle;

          UpdateMeshes();
     }

     void UpdateMeshes()
     {
          for (int i = 0; i < 4; i++)
          {
               // a kerek pozicioja
               Vector3 pos;
               // a kerek forgasi szoge
               Quaternion quat;

               m_WheelColliders[i].GetWorldPose(out pos, out quat);
               
               m_WheelMeshes[i].position = pos;
               m_WheelMeshes[i].rotation = quat;

          }
     }



}
