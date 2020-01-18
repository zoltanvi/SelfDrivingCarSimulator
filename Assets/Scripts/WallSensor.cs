using UnityEngine;

public class WallSensor : MonoBehaviour
{
    // Az autohoz tartozo erzekelok kezdopontja
    [SerializeField] private Transform m_RayOriginPoint;
    private float m_LineLength;
    private int m_RayCount;

    // Az erzekelok csak ezen layeren levo object-eket erzekelik 
    [SerializeField] private const string RayLayerName = "Environment";
    [SerializeField] private Rigidbody m_CarRigidbody;
    private bool m_ControlledByPlayer;

    // Az erzekelok lathatatlanok, egy tombelem egy lathato vonalat tart
    private GameObject[] m_RayHolders;

    private int Id { get; set; }

    // Az erzekelok altal mert tavolsagokat es a fitnesst  tartalmazza
    private float[] m_CarNeuronInputs;

    // A raycastHit-ben vannak az erzekelok adatai tarolva.
    private RaycastHit[] m_RaycastHit;

    #region Navigator változói
    private Vector3[] m_Points;
    private int m_One, m_Two, m_Three, m_Four;
    private Vector3 m_First, m_Second, m_Third, m_Fourth;
    private float m_FirstAngle, m_SecondAngle, m_ThirdAngle;
    private FitnessMeter m_FitnessMeter;
    public Transform[] Waypoints;
    #endregion

    private void Start()
    {
        m_RayCount = Master.Instance.Manager.CarSensorCount;
        m_LineLength = Master.Instance.Manager.CarSensorLength;
        // Beallitja az auto sorszamat
        Id = gameObject.GetComponent<CarController>().Id;

        // Inicializalja az erzekeloket
        m_RaycastHit = new RaycastHit[m_RayCount];
        m_RayHolders = new GameObject[m_RayCount];
        // Inicializalja az erzekeloket reprezentalo vonalakat
        InitializeLines();

        if (gameObject.GetComponent<CarController>().IsPlayerControlled)
        {
            m_ControlledByPlayer = true;
        }

        if (Master.Instance.Manager.Configuration.Navigator)
        {
            // Inicializalja a neuralis halo inputjait
            // carNeuronInputs = new double[rayCount + 4];
            m_CarNeuronInputs = new float[m_RayCount + 4];

            m_FitnessMeter = gameObject.GetComponent<FitnessMeter>();
            m_Points = new Vector3[5];
            for (int i = 0; i < m_Points.Length; i++)
            {
                m_Points[i] = new Vector3();
            }

            Waypoints = new Transform[Master.Instance.Manager.CurrentWayPoint.transform.childCount];

            int index = 0;
            foreach (Transform wp in Master.Instance.Manager.CurrentWayPoint.transform)
            {
                Waypoints[index++] = wp;
            }
        }
        else
        {
            // Inicializalja a neuralis halo inputjait
            m_CarNeuronInputs = new float[m_RayCount + 1];
        }
    }

    private void FixedUpdate()
    {
        // Erzekelo sugarak letrahozasa
        CreateRays(m_RayCount);

        // Erzekelo adatok tarolasa a carNeuronInputs tombben
        for (int i = 0; i < m_RayCount; i++)
        {
            // Ha valamivel utkozik az erzekelo sugar akkor a mert adat tarolasa
            // a carNeuronInputs tombben, ellenben a vonal hosszat tarolja.
            if (null != m_RaycastHit[i].collider)
            {
                m_CarNeuronInputs[i] = m_RaycastHit[i].distance;
            }
            else
            {
                m_CarNeuronInputs[i] = m_LineLength;
            }
        }
        // A neuralis halo utolso inputja az auto sebessege
        m_CarNeuronInputs[m_RayCount] = m_CarRigidbody.velocity.magnitude;

        if (!m_ControlledByPlayer)
        {
            // Atadja az erzekelo adatokat es az auto sebesseget a CarGameManagernek
            Master.Instance.Manager.Cars[Id].Inputs = m_CarNeuronInputs;
        }

        if (Master.Instance.Manager.Configuration.Navigator)
        {
            SetAnglesInput();
        }
    }

    // Letrehozza az erzekelo sugarakat.
    private void CreateRays(int quantity)
    {
        // Az angleBase = a sugarak kozotti szog nagysaga.
        float angleBase = 180f / (quantity + 1);

        // Ha 5 erzekelo van, 45 fok lesz az erzekelok kozott
        if (quantity == 5)
        {
            for (int i = 0; i < quantity; i++)
            {
                // A jelenlegi sugar szoge balrol jobbra szamitva.
                Quaternion lineRotation =
                    Quaternion.AngleAxis((45 * i), (m_RayOriginPoint.up));

                // A sugar kezdo es vegpontjai.
                Vector3 rayOrigin = m_RayOriginPoint.position;
                Vector3 rayDirection = lineRotation * (-m_RayOriginPoint.right);

                Physics.Raycast(rayOrigin, rayDirection, out m_RaycastHit[i], m_LineLength, LayerMask.GetMask(RayLayerName));

                // Megrajzolja a sugarat. 
                m_RayHolders[i].GetComponent<LineRenderer>().SetPosition(0, rayOrigin);
                m_RayHolders[i].GetComponent<LineRenderer>().SetPosition(1, rayOrigin + rayDirection * m_LineLength);
            }
        }
        else
        {
            for (int i = 1; i < (quantity + 1); i++)
            {
                // A jelenlegi sugar szoge balrol jobbra szamitva.
                Quaternion lineRotation =
                    Quaternion.AngleAxis((angleBase * i), (m_RayOriginPoint.up));

                // A sugar kezdo es vegpontjai.
                Vector3 rayOrigin = m_RayOriginPoint.position;
                Vector3 rayDirection = lineRotation * (-m_RayOriginPoint.right);

                // Letrehozza a sugarat es eltarolja hogy hozzaernek-e a falhoz.
                Physics.Raycast(rayOrigin, rayDirection, out m_RaycastHit[i - 1], m_LineLength, LayerMask.GetMask(RayLayerName));

                // Megrajzolja a sugarat. 
                m_RayHolders[i - 1].GetComponent<LineRenderer>().SetPosition(0, rayOrigin);
                m_RayHolders[i - 1].GetComponent<LineRenderer>().SetPosition(1, rayOrigin + rayDirection * m_LineLength);
            }
        }
    }

    // Inicializalja az erzekeloket reprezentalo vonalakat.
    private void InitializeLines()
    {
        Material lineMat = new Material(Shader.Find("Sprites/Default"));
        for (int i = 0; i < m_RayHolders.Length; i++)
        {
            m_RayHolders[i] = new GameObject("RayHolder_" + Id);
            m_RayHolders[i].transform.parent = Master.Instance.Manager.RayHolderRoot.transform;
            LineRenderer lineRenderer = m_RayHolders[i].AddComponent<LineRenderer>();
            lineRenderer.positionCount = 2;
            lineRenderer.numCapVertices = 5;
            lineRenderer.startWidth = 0.05f;
            lineRenderer.endWidth = 0.05f;
            lineRenderer.useWorldSpace = false;
            lineRenderer.material = lineMat;
            lineRenderer.startColor = new Color(0.8378353f, 0f, 1.0f, 0.02f);
            lineRenderer.endColor = new Color(0f, 0.8818119f, 1f, 0f);
        }
    }

    private void SetAnglesInput()
    {
        // Indexek
        m_One = ((m_FitnessMeter.NextPointIndex + 1) > Waypoints.Length - 1) ? 0 : (m_FitnessMeter.NextPointIndex + 1);
        m_Two = ((m_One + 1) > Waypoints.Length - 1) ? 0 : (m_One + 1);
        m_Three = ((m_Two + 1) > Waypoints.Length - 1) ? 0 : (m_Two + 1);
        m_Four = ((m_Three + 1) > Waypoints.Length - 1) ? 0 : (m_Three + 1);

        // Pontok
        m_Points[0] = Waypoints[m_FitnessMeter.NextPointIndex].position;
        m_Points[1] = Waypoints[m_One].position;
        m_Points[2] = Waypoints[m_Two].position;
        m_Points[3] = Waypoints[m_Three].position;
        m_Points[4] = Waypoints[m_Four].position;

        // Vektorok
        m_First = m_Points[1] - m_Points[0];
        m_Second = m_Points[2] - m_Points[1];
        m_Third = m_Points[3] - m_Points[2];
        m_Fourth = m_Points[4] - m_Points[3];

        // Szögek
        m_FirstAngle = Vector3.Angle(m_First, m_Second);
        m_SecondAngle = Vector3.Angle(m_Second, m_Third);
        m_ThirdAngle = Vector3.Angle(m_Third, m_Fourth);

        // Beírja az input tömbbe a szögeket
        Master.Instance.Manager.Cars[Id].Inputs[Master.Instance.Manager.CarSensorCount + 1] = m_FirstAngle;
        Master.Instance.Manager.Cars[Id].Inputs[Master.Instance.Manager.CarSensorCount + 2] = m_SecondAngle;
        Master.Instance.Manager.Cars[Id].Inputs[Master.Instance.Manager.CarSensorCount + 3] = m_ThirdAngle;
    }
}