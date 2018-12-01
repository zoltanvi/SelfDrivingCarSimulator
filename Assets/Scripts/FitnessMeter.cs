using UnityEngine;


public class FitnessMeter : MonoBehaviour
{
	[SerializeField] private GameObject waypointsRoot;
	[SerializeField] private Transform carPoint;

	[HideInInspector] public Transform[] waypoints;
	// prevPoint: a visszafele irányba lévő ELŐZŐ pont
	// currentPoint: ehhez a referencia ponthoz képest számoljuk a távolságot
	// nextPoint: az előre irányba lévő KÖVETKEZŐ pont
	[HideInInspector] public Transform prevPoint, currentPoint, nextPoint;

	public int NextPointIndex = 1;

	// AbsoluteFitness: a palyahoz viszonyitott fitness.
	[HideInInspector] public float AbsoluteFitness = 0;

	// RelativeFitness: a currentPoint-hoz viszonyitott tavolsag.
	private float relativeFitness = 0;
	// SavedFitness: a mar elhagyott waypointok tavolsaganak osszege.
	private float savedFitness = 0;

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
		float prevCarDistance = Vector3.Distance(prevPoint.position, carPoint.position);
		float nextCarDistance = Vector3.Distance(carPoint.position, nextPoint.position);
		float centerCarDistance = Vector3.Distance(currentPoint.position, carPoint.position);

		relativeFitness = centerCarDistance;
		if (prevCarDistance < nextCarDistance && relativeFitness > 0)
		{
			relativeFitness *= -1;
		}
	}

	// Eggyel elore lepteti a kornyezo pontok indexet.
	private void StepForwardPoints()
	{
		// Ha az index tulfutna a waypointok szaman, akkor az elejetol kezdi (kor a palya).
		NextPointIndex = (NextPointIndex + 1) > waypoints.Length - 1 ? 0 : (NextPointIndex + 1);

		prevPoint = currentPoint;
		currentPoint = nextPoint;
		nextPoint = waypoints[NextPointIndex];

		CheckNegativeFitness();
	}

	// Eggyel vissza lepteti a kornyezo pontok indexet.
	void StepBackwardPoints()
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
		// A referencia pont és az autó közötti távolság
		float centerCarDistance = Vector3.Distance(currentPoint.position, carPoint.position);
		// Az előző pont és az autó közötti távolság 
		float prevCarDistance = Vector3.Distance(prevPoint.position, carPoint.position);
		// A következő pont és az autó közötti távolság
		float nextCarDistance = Vector3.Distance(carPoint.position, nextPoint.position);

		// Az előző és a referencia pont közötti távolság
		float prevCenterDistance = Vector3.Distance(prevPoint.position, currentPoint.position);
		// A következő és a referencia pont közötti távolság
		float centerNextDistance = Vector3.Distance(currentPoint.position, nextPoint.position);

		if (controlledByPlayer)
		{
			Debug.DrawLine(currentPoint.position + Vector3.up, carPoint.position + Vector3.up, Color.green); // centerCar
			Debug.DrawLine(nextPoint.position + Vector3.up, carPoint.position + Vector3.up, Color.yellow);   // nextCar
			Debug.DrawLine(currentPoint.position + Vector3.up, nextPoint.position + Vector3.up, Color.white);// centerNext


			//Debug.DrawLine(prevPoint.position + Vector3.up, carPoint.position + Vector3.up, Color.magenta);  // prevCar
			//Debug.DrawLine(currentPoint.position + Vector3.up, prevPoint.position + Vector3.up, Color.magenta);  



		}


		relativeFitness = centerCarDistance;

		//relativeFitness = prevCenterDistance + centerNextDistance / (prevCarDistance + nextCarDistance);
		//relativeFitness *= prevCarDistance;

		// Ha a prevPointhoz van kozelebb az auto, akkor visszafele halad.
		if (prevCarDistance < nextCarDistance && relativeFitness > 0)
		{
			relativeFitness *= -1;
		}

		// HA ÁTMENT A NEXTPOINT-ON
		// Ha a currentPoint es a nextPoint tavolsa kisebb, mint a
		// currentPoint es a carCenterPoint tavolsaga,
		// es a nextPointhoz van kozelebb az auto, akkor az atment a nextPointon.
		if (centerCarDistance > centerNextDistance && nextCarDistance < prevCarDistance)
		{
			savedFitness += centerNextDistance;
			StepForwardPoints();
		}

		//if(centerCarDistance > nextCarDistance && nextCarDistance < prevCarDistance)
		//{
		//	savedFitness += centerNextDistance;
		//	StepForwardPoints();
		//}



		// HA VISSZAFELE ÁTMENT A PREVPOINT-ON
		// Ha a currentPoint es a prevPoint tavolsa kisebb, mint a
		// currentPoint es a carCenterPoint tavolsaga,
		// es a prevPointhoz van kozelebb az auto, akkor az atment a prevPointon.
		if (centerCarDistance > prevCenterDistance && prevCarDistance < nextCarDistance)
		{
			savedFitness -= prevCenterDistance;
			StepBackwardPoints();
		}

		//if (centerCarDistance > prevCarDistance && prevCarDistance < nextCarDistance)
		//{
		//	savedFitness -= prevCenterDistance;
		//	StepBackwardPoints();
		//}

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

