using UnityEngine;

public class CarController : MonoBehaviour {

     //maximum nyomatek
     public CarStats m_carStats;
     public Transform centerOfMass;
     public WheelCollider[] wheelColliders = new WheelCollider[4];
     public Transform[] wheelMeshes = new Transform[4];
     private Rigidbody m_rigidbody;

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

          for (int i = 0; i < 4; i++)
          {

               // rugo beallitasa
               suspensions[i] = wheelColliders[i].suspensionSpring;
               suspensions[i].spring = m_carStats.spring;

               // Forward Friction - swiftness beallitasa
               wheelFrictionCurveForward[i] = wheelColliders[i].forwardFriction;
               wheelFrictionCurveForward[i].stiffness = m_carStats.forwardSwiftness;

               // Sideways Friction - swiftness beallitasa
               wheelFrictionCurveSideways[i] = wheelColliders[i].sidewaysFriction;
               wheelFrictionCurveSideways[i].stiffness = m_carStats.sidewaysSwiftness;

               wheelColliders[i].suspensionSpring = suspensions[i];
               wheelColliders[i].forwardFriction = wheelFrictionCurveForward[i];
               wheelColliders[i].sidewaysFriction = wheelFrictionCurveSideways[i];


          }


          // megadjuk az auto tomegkozeppontjat
          m_rigidbody = GetComponent<Rigidbody>();
          m_rigidbody.centerOfMass = centerOfMass.localPosition;
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
               wheelColliders[i].motorTorque = m_carStats.maxTorque * accelerate;
          }

         // Debug.Log("The speed is: " + wheelColliders[0].motorTorque);

          // maximum fordulasi szog
          float finalAngle = steer * m_carStats.turnAngle;

          // az elso kerekek megkapjak a fordulasi szoget
          wheelColliders[0].steerAngle = finalAngle;
          wheelColliders[1].steerAngle = finalAngle;

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

               wheelColliders[i].GetWorldPose(out pos, out quat);
               
               wheelMeshes[i].position = pos;
               wheelMeshes[i].rotation = quat;

          }
     }

}
