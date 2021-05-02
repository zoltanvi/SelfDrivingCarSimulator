/*
Copyright (C) 2021 zoltanvi

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

using UnityEngine;

public class CarController : MonoBehaviour
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

    public int Id { get; set; }
    public bool IsAlive { get; set; }

    [SerializeField] private Transform m_CenterOfMass;
    [SerializeField] private WheelCollider[] m_WheelColliders = new WheelCollider[4];
    [SerializeField] private Transform[] m_WheelMeshes = new Transform[4];
    private const string WallLayerName = "Environment";
    public bool IsPlayerControlled { get; set; }

    [HideInInspector] public float Steer;
    [HideInInspector] public float Accelerate;
    private float m_OldRotation;

    private Rigidbody m_Rigidbody;

    // 1 meter/sec = 2.23693629 miles/hour	
    public float CurrentSpeed => m_Rigidbody.velocity.magnitude * 2.23693629f;
    private Rigidbody m_CarRigidbody;

    // wheelColliders / wheelMeshes [0] : Front Left
    // wheelColliders / wheelMeshes [1] : Front Right
    // wheelColliders / wheelMeshes [2] : Back Left
    // wheelColliders / wheelMeshes [3] : Back Right

    private void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();

        // Inicializálás
        IsAlive = true;
        JointSpring[] suspensions = new JointSpring[4];
        WheelFrictionCurve[] wheelFrictionCurveForward = new WheelFrictionCurve[4];
        WheelFrictionCurve[] wheelFrictionCurveSideways = new WheelFrictionCurve[4];


        #region Setting 4 wheels
        for (int i = 0; i < 4; i++)
        {
            // A rugo beallitasa.
            suspensions[i] = m_WheelColliders[i].suspensionSpring;
            suspensions[i].spring = CarParameters.Spring;

            // Forward Friction - swiftness beallitasa.
            wheelFrictionCurveForward[i] = m_WheelColliders[i].forwardFriction;
            wheelFrictionCurveForward[i].stiffness = CarParameters.ForwardSwiftness;

            // Sideways Friction - swiftness beallitasa.
            wheelFrictionCurveSideways[i] = m_WheelColliders[i].sidewaysFriction;
            wheelFrictionCurveSideways[i].stiffness = CarParameters.SidewaysSwiftness;

            m_WheelColliders[i].suspensionSpring = suspensions[i];
            m_WheelColliders[i].forwardFriction = wheelFrictionCurveForward[i];
            m_WheelColliders[i].sidewaysFriction = wheelFrictionCurveSideways[i];
        }
        #endregion

        // Megadja az auto tomegkozeppontjat, hogy ne boruljon fel.
        m_CarRigidbody = GetComponent<Rigidbody>();
        m_CarRigidbody.centerOfMass = m_CenterOfMass.localPosition;
    }

    private void FixedUpdate()
    {
        // Ha az autó játékos által van irányítva, akkor megkapja a vezérlést (billentyűzet)
        if (IsPlayerControlled)
        {
            // Kanyarodas (balra jobbra).
            Steer = Input.GetAxis("Horizontal");
            // Gyorsulas (fel le).
            Accelerate = Input.GetAxis("Vertical");
        }

        for (int i = 0; i < 4; i++)
        {
            // Ha az autó nem ütközött falnak, csak akkor lehet irányítani a motort
            if (IsAlive)
            {
                // Ha előre megy az autó
                if (Accelerate >= 0)
                {
                    m_WheelColliders[i].motorTorque = CarParameters.Accelerate * (float)Accelerate;
                    m_WheelColliders[i].brakeTorque = 0f;
                }
                // Ha előre megy az autó és a fék nyomva van
                else if (Accelerate < 0 && CurrentSpeed > 5 && Vector3.Angle(transform.forward, m_Rigidbody.velocity) < 50f)
                {
                    m_WheelColliders[i].brakeTorque = CarParameters.Brake * -(float)Accelerate;
                    m_WheelColliders[i].motorTorque = 0f;
                }
                // Ha tolat az autó
                else
                {
                    m_WheelColliders[i].motorTorque = CarParameters.Shunt * (float)Accelerate;
                    m_WheelColliders[i].brakeTorque = 0f;
                }
            }
            else
            {
                m_WheelColliders[i].motorTorque = 0;
            }
        }

        // Jobb tapadása lesz az autónak
        m_WheelColliders[0].attachedRigidbody.AddForce(
            (-transform.up) * CarParameters.DownForce * m_WheelColliders[0].attachedRigidbody.velocity.magnitude);

        // Elso kerekek maximum fordulasi szoge.
        float finalAngle = (float)Steer * CarParameters.TurnAngle;

        // Ha az autó nem ütközött falnak, csak akkor lehet kormányozni a kerekeket
        if (IsAlive)
        {
            // Az elso kerekek megkapjak a fordulasi szoget.
            m_WheelColliders[0].steerAngle = finalAngle;
            m_WheelColliders[1].steerAngle = finalAngle;
        }
        else
        {
            m_WheelColliders[0].steerAngle = 0;
            m_WheelColliders[1].steerAngle = 0;
        }

        // this if is needed to avoid gimbal lock problems that will make the car suddenly shift direction
        if (Mathf.Abs(m_OldRotation - transform.eulerAngles.y) < 10f)
        {
            var turnAdjust = (transform.eulerAngles.y - m_OldRotation) * CarParameters.SteerHelper;
            Quaternion velRotation = Quaternion.AngleAxis(turnAdjust, Vector3.up);
            m_CarRigidbody.velocity = velRotation * m_CarRigidbody.velocity;
        }
        m_OldRotation = transform.eulerAngles.y;

        UpdateMeshes();
    }

    /// <summary>
    /// Ha az autó falnak ütközött és még nem fagyott meg,
    /// akkor megfagyasztja
    /// </summary>
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.layer != LayerMask.NameToLayer(WallLayerName) || Id == -1) return;

        Master.Instance.Manager.FreezeCar(m_CarRigidbody, Id, IsAlive);
        IsAlive = false;
        Master.Instance.Manager.Cars[Id].IsAlive = false;
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

            m_WheelColliders[i].GetWorldPose(out pos, out quat);

            m_WheelMeshes[i].position = pos;
            m_WheelMeshes[i].rotation = quat;
        }
    }

    /// <summary>
    /// Meghívja az autó ID-jére a Manager FreezeCar metódusát.
    /// </summary>
    public void Freeze()
    {
        Master.Instance.Manager.FreezeCar(m_CarRigidbody, Id, IsAlive);
    }
}
