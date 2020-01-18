using UnityEngine;

public class FitnessMeter : MonoBehaviour
{
    [SerializeField] private GameObject m_WaypointsRoot;
    [SerializeField] private Transform m_CarPoint;

    [HideInInspector] public Transform[] Waypoints;
    // PreviousPoint: a visszafele irányba lévő ELŐZŐ pont
    // CurrentPoint: ehhez a referencia ponthoz képest számoljuk a távolságot
    // NextPoint: az előre irányba lévő KÖVETKEZŐ pont
    [HideInInspector] public Transform PreviousPoint;
    [HideInInspector] public Transform CurrentPoint;
    [HideInInspector] public Transform NextPoint;

    public int NextPointIndex = 1;

    // AbsoluteFitness: a palyahoz viszonyitott fitness.
    [HideInInspector] public float AbsoluteFitness;

    // RelativeFitness: a currentPoint-hoz viszonyitott tavolsag.
    private float m_RelativeFitness;
    // SavedFitness: a mar elhagyott waypointok tavolsaganak osszege.
    private float m_SavedFitness;

    private int m_CarId;
    private bool m_ControlledByPlayer;

    private void Start()
    {
        m_WaypointsRoot = Master.Instance.Manager.CurrentWayPoint;

        if (!gameObject.GetComponent<CarController>().IsPlayerControlled)
        {
            m_CarId = gameObject.GetComponent<CarController>().Id;
            Master.Instance.Manager.Cars[m_CarId].Fitness = AbsoluteFitness;
        }
        else
        {
            m_ControlledByPlayer = true;
            Master.Instance.Manager.PlayerFitness = AbsoluteFitness;
        }

        Waypoints = new Transform[m_WaypointsRoot.transform.childCount];

        int index = 0;
        foreach (Transform wp in m_WaypointsRoot.transform)
        {
            Waypoints[index++] = wp;
        }

        // Szamon tartjuk az autohoz legkozelebbi 3 wayPoint-ot.
        PreviousPoint = Waypoints[Waypoints.Length - 1];
        CurrentPoint = Waypoints[0];
        NextPoint = Waypoints[1];
    }

    public void Reset()
    {
        NextPointIndex = 1;
        PreviousPoint = Waypoints[Waypoints.Length - 1];
        CurrentPoint = Waypoints[0];
        NextPoint = Waypoints[1];
        AbsoluteFitness = 0;
        m_RelativeFitness = 0;
        m_SavedFitness = 0;
    }

    private void CheckNegativeFitness()
    {
        float prevCarDistance = Vector3.Distance(PreviousPoint.position, m_CarPoint.position);
        float nextCarDistance = Vector3.Distance(m_CarPoint.position, NextPoint.position);
        float centerCarDistance = Vector3.Distance(CurrentPoint.position, m_CarPoint.position);

        m_RelativeFitness = centerCarDistance;
        if (prevCarDistance < nextCarDistance && m_RelativeFitness > 0)
        {
            m_RelativeFitness *= -1;
        }
    }

    // Eggyel elore lepteti a kornyezo pontok indexet.
    private void StepForwardPoints()
    {
        // Ha az index tulfutna a waypointok szaman, akkor az elejetol kezdi (kor a palya).
        NextPointIndex = (NextPointIndex + 1) > Waypoints.Length - 1 ? 0 : (NextPointIndex + 1);

        PreviousPoint = CurrentPoint;
        CurrentPoint = NextPoint;
        NextPoint = Waypoints[NextPointIndex];

        CheckNegativeFitness();
    }

    // Eggyel vissza lepteti a kornyezo pontok indexet.
    void StepBackwardPoints()
    {
        // Ha az index tul alacsony lenne, akkor a vegerol kezdi (kor a palya).
        int prevIndex = (NextPointIndex - 3) < 0 ? (((NextPointIndex - 3) + Waypoints.Length) % Waypoints.Length) : (NextPointIndex - 3);
        NextPointIndex = (NextPointIndex - 1) < 0 ? (((NextPointIndex - 1) + Waypoints.Length) % Waypoints.Length) : (NextPointIndex - 1);

        NextPoint = CurrentPoint;
        CurrentPoint = PreviousPoint;
        PreviousPoint = Waypoints[prevIndex];

        CheckNegativeFitness();
    }

    private void Update()
    {
        CalculateFitness();
    }

    // Kiszamolja az auto fitness-et.
    private void CalculateFitness()
    {
        // A referencia pont és az autó közötti távolság
        float centerCarDistance = Vector3.Distance(CurrentPoint.position, m_CarPoint.position);
        // Az előző pont és az autó közötti távolság 
        float prevCarDistance = Vector3.Distance(PreviousPoint.position, m_CarPoint.position);
        // A következő pont és az autó közötti távolság
        float nextCarDistance = Vector3.Distance(m_CarPoint.position, NextPoint.position);

        // Az előző és a referencia pont közötti távolság
        float prevCenterDistance = Vector3.Distance(PreviousPoint.position, CurrentPoint.position);
        // A következő és a referencia pont közötti távolság
        float centerNextDistance = Vector3.Distance(CurrentPoint.position, NextPoint.position);

        if (m_ControlledByPlayer)
        {
            Debug.DrawLine(CurrentPoint.position + Vector3.up, m_CarPoint.position + Vector3.up, Color.green); // centerCar
            Debug.DrawLine(NextPoint.position + Vector3.up, m_CarPoint.position + Vector3.up, Color.yellow);   // nextCar
            Debug.DrawLine(CurrentPoint.position + Vector3.up, NextPoint.position + Vector3.up, Color.white);  // centerNext
        }

        m_RelativeFitness = centerCarDistance;

        // Ha a prevPointhoz van kozelebb az auto, akkor visszafele halad.
        if (prevCarDistance < nextCarDistance && m_RelativeFitness > 0)
        {
            m_RelativeFitness *= -1;
        }

        // HA ÁTMENT A NEXTPOINT-ON
        // Ha a currentPoint es a nextPoint tavolsa kisebb, mint a
        // currentPoint es a carCenterPoint tavolsaga,
        // es a nextPointhoz van kozelebb az auto, akkor az atment a nextPointon.
        if (centerCarDistance > centerNextDistance && nextCarDistance < prevCarDistance)
        {
            m_SavedFitness += centerNextDistance;
            StepForwardPoints();
        }

        // HA VISSZAFELE ÁTMENT A PREVPOINT-ON
        // Ha a currentPoint es a prevPoint tavolsa kisebb, mint a
        // currentPoint es a carCenterPoint tavolsaga,
        // es a prevPointhoz van kozelebb az auto, akkor az atment a prevPointon.
        if (centerCarDistance > prevCenterDistance && prevCarDistance < nextCarDistance)
        {
            m_SavedFitness -= prevCenterDistance;
            StepBackwardPoints();
        }

        // Az autonak a palyahoz viszonyitott elorehaladasa.
        AbsoluteFitness = m_SavedFitness + m_RelativeFitness;

        // Az auto fitness erteket atadja a CarGameManager-nek
        if (!m_ControlledByPlayer)
        {
            Master.Instance.Manager.Cars[m_CarId].Fitness = AbsoluteFitness;
        }
        else
        {
            Master.Instance.Manager.PlayerFitness = AbsoluteFitness;
        }
    }
}

