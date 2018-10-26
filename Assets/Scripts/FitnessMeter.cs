using UnityEngine;


public class FitnessMeter : MonoBehaviour
{

	[SerializeField] private GameObject waypointsRoot;
	[SerializeField] private Transform carCenterPoint;

	[HideInInspector] public Transform[] waypoints;
	[HideInInspector] public Transform prevPoint, currentPoint, nextPoint;
	public int NextPointIndex = 1;

	// AbsoluteFitness: a palyahoz viszonyitott fitness.
	[HideInInspector] public float AbsoluteFitness = 0;

	// RelativeFitness: a currentPointhoz viszonyitott tavolsag.
	private float relativeFitness = 0;
	// SavedFitness: a mar elhagyott waypointok tavolsaganak osszege.
	private float savedFitness = 0;
	// Az auto sorszama - tobb autot managel a CarGameController osztaly
	private int carID;
	private bool controlledByPlayer = false;


	private void Start()
	{
		waypointsRoot = Master.Instance.Manager.CurrentWayPoint;

		if (!this.gameObject.GetComponent<CarController>().IsPlayerControlled)
		{
			carID = this.gameObject.GetComponent<CarController>().Id;
			Master.Instance.Manager.Cars[carID].Fitness = AbsoluteFitness;
		}
		else
		{
			controlledByPlayer = true;
			Master.Instance.Manager.PlayerFitness = AbsoluteFitness;
		}

		waypoints = new Transform[waypointsRoot.transform.childCount];

		int index = 0;
		foreach (Transform wp in waypointsRoot.transform)
		{
			waypoints[index++] = wp;
		}

		// Szamon tartjuk az autohoz legkozelebbi 3 wayPoint-ot.
		prevPoint = waypoints[waypoints.Length - 1];
		currentPoint = waypoints[0];
		nextPoint = waypoints[1];


	}

	public void Reset()
	{
		NextPointIndex = 1;
		prevPoint = waypoints[waypoints.Length - 1];
		currentPoint = waypoints[0];
		nextPoint = waypoints[1];
		AbsoluteFitness = 0;
		relativeFitness = 0;
		savedFitness = 0;
	}

	private void CheckNegativeFitness()
	{
		float prevCarDistance = Vector3.Distance(prevPoint.position, carCenterPoint.position);
		float nextCarDistance = Vector3.Distance(carCenterPoint.position, nextPoint.position);
		float centerCarDistance = Vector3.Distance(currentPoint.position, carCenterPoint.position);

		relativeFitness = centerCarDistance;
		if (prevCarDistance < nextCarDistance && relativeFitness > 0)
		{
			relativeFitness *= -1;
		}
	}

	// Eggyel elore lepteti a kornyezo pontok indexet.
	private void FollowingPoints()
	{
		// Ha az index tulfutna a waypointok szaman, akkor az elejetol kezdi (kor a palya).
		NextPointIndex = (NextPointIndex + 1) > waypoints.Length - 1 ? 0 : (NextPointIndex + 1);

		prevPoint = currentPoint;
		currentPoint = nextPoint;
		nextPoint = waypoints[NextPointIndex];

		CheckNegativeFitness();
	}

	// Eggyel vissza lepteti a kornyezo pontok indexet.
	void PreviousPoints()
	{
		// Ha az index tul alacsony lenne, akkor a vegerol kezdi (kor a palya).
		int prevIndex = (NextPointIndex - 3) < 0 ? (((NextPointIndex - 3) + waypoints.Length) % waypoints.Length) : (NextPointIndex - 3);
		NextPointIndex = (NextPointIndex - 1) < 0 ? (((NextPointIndex - 1) + waypoints.Length) % waypoints.Length) : (NextPointIndex - 1);

		nextPoint = currentPoint;
		currentPoint = prevPoint;
		prevPoint = waypoints[prevIndex];

		CheckNegativeFitness();
	}

	private void Update()
	{
		CalculateFitness();
	}

	// Kiszamolja az auto fitness-et.
	private void CalculateFitness()
	{
		float centerCarDistance = Vector3.Distance(currentPoint.position, carCenterPoint.position);
		float prevCenterDistance = Vector3.Distance(prevPoint.position, currentPoint.position);
		float centerNextDistance = Vector3.Distance(currentPoint.position, nextPoint.position);
		float prevCarDistance = Vector3.Distance(prevPoint.position, carCenterPoint.position);
		float nextCarDistance = Vector3.Distance(carCenterPoint.position, nextPoint.position);
		relativeFitness = centerCarDistance;

		// Ha a prevPointhoz van kozelebb az auto, akkor visszafele halad.
		if (prevCarDistance < nextCarDistance && relativeFitness > 0)
		{
			relativeFitness *= -1;
		}

		// Ha a currentPoint es a nextPoint tavolsa kisebb, mint a
		// currentPoint es a carCenterPoint tavolsaga,
		// es a nextPointhoz van kozelebb az auto, akkor az atment a nextPointon.
		if (centerCarDistance > centerNextDistance && nextCarDistance < prevCarDistance)
		{
			savedFitness += centerNextDistance;
			FollowingPoints();
		}

		// Ha a currentPoint es a prevPoint tavolsa kisebb, mint a
		// currentPoint es a carCenterPoint tavolsaga,
		// es a prevPointhoz van kozelebb az auto, akkor az atment a prevPointon.
		if (centerCarDistance > prevCenterDistance && prevCarDistance < nextCarDistance)
		{
			savedFitness -= prevCenterDistance;
			PreviousPoints();
		}

		// Az autonak a palyahoz viszonyitott elorehaladasa.
		AbsoluteFitness = savedFitness + relativeFitness;

		// Az auto fitness erteket atadja a CarGameManager-nek
		if (!controlledByPlayer)
		{
			Master.Instance.Manager.Cars[carID].Fitness = AbsoluteFitness;
		}
		else
		{
			Master.Instance.Manager.PlayerFitness = AbsoluteFitness;
		}
	}

}

