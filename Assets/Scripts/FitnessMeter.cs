using UnityEngine;
using UnityEngine.UI;

public class FitnessMeter : MonoBehaviour
{

	#region Variables
	[SerializeField] Transform waypointsRoot;
	[SerializeField] Transform carCenterPoint;
	[SerializeField] Text fitnessText;
	[SerializeField] Text wrongwayText;

	Transform[] waypoints;
	Transform prevPoint, currentPoint, nextPoint;

	int nextIndex = 1;

	// AbsoluteFitness: a palyahoz viszonyitott fitness.
	double absoluteFitness = 0;
	// RelativeFitness: a currentPointhoz viszonyitott tavolsag.
	double relativeFitness = 0;
	// SavedFitness: a mar elhagyott waypointok tavolsaganak osszege.
	double savedFitness = 0;
	#endregion



	#region Unity Methods
	void Start()
	{
		waypoints = new Transform[waypointsRoot.childCount];

		int index = 0;
		foreach (Transform wp in waypointsRoot)
		{
			waypoints[index++] = wp;
		}

		// Szamon tartjuk az autohoz legkozelebbi 3 wayPoint-ot.
		prevPoint = waypoints[waypoints.Length - 1];
		currentPoint = waypoints[0];
		nextPoint = waypoints[1];
	}

	// Lepteti eggyel elore a kornyezo pontok indexet.
	void FollowingPoints()
	{
		// Ha az index tulfutna a waypointok szaman, akkor az elejetol kezdi (kor a palya).
		nextIndex = (nextIndex + 1) > waypoints.Length - 1 ? 0 : (nextIndex + 1);

		prevPoint = currentPoint;
		currentPoint = nextPoint;
		nextPoint = waypoints[nextIndex];

	}

	// Lepteti eggyel vissza a kornyezo pontok indexet.
	void PreviousPoints()
	{
		// Ha az index tul alacsony lenne, akkor a vegerol kezdi (kor a palya).
		int prevIndex = (nextIndex - 3) < 0 ? (((nextIndex - 3) + waypoints.Length) % waypoints.Length) : (nextIndex - 3);
		nextIndex = (nextIndex - 1) < 0 ? (((nextIndex - 1) + waypoints.Length) % waypoints.Length) : (nextIndex - 1);

		nextPoint = currentPoint;
		currentPoint = prevPoint;
		prevPoint = waypoints[prevIndex];

	}

	void Update()
	{

		double centerCarDistance = Vector3.Distance(currentPoint.position, carCenterPoint.position);
		double prevCenterDistance = Vector3.Distance(prevPoint.position, currentPoint.position);
		double centerNextDistance = Vector3.Distance(currentPoint.position, nextPoint.position);
		double prevCarDistance = Vector3.Distance(prevPoint.position, carCenterPoint.position);
		double nextCarDistance = Vector3.Distance(carCenterPoint.position, nextPoint.position);

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
		absoluteFitness = savedFitness + relativeFitness;

		// Ha a rajttol visszafele megy az auto, megjelenik a WRONG WAY felirat.
		if (absoluteFitness < 0)
		{
			wrongwayText.text = "WRONG WAY";
		}
		else
		{
			wrongwayText.text = "";
		}

		fitnessText.text = "Fitness: " + string.Format("{0:0.0000}", absoluteFitness);
	}

	#endregion

}