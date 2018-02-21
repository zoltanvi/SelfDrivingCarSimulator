using UnityEngine;
using UnityEngine.UI;

public class FitnessMeter : MonoBehaviour {

	#region Variables
	[SerializeField] Transform waypointsRoot;
	[SerializeField] Transform car;
	[SerializeField] Text text;
	[SerializeField] Text wrongwayText;
	Transform[] waypoints;
	Transform prevPoint, centerPoint, nextPoint;

	int next = 1;

	double fitness = 0;
	double relativeFitness = 0;
	double savedFitness = 0;
	#endregion

	#region Unity Methods

	
	void Start () 
	{
		waypoints = new Transform[waypointsRoot.childCount];
		int i = 0;
		foreach (Transform wp in waypointsRoot)
		{
			waypoints[i++] = wp;
		}
		prevPoint = waypoints[waypoints.Length - 1];
		centerPoint = waypoints[0];
		nextPoint = waypoints[1];
	}

	void NextPoints()
	{
		next = (next + 1) > waypoints.Length - 1 ? 0 : (next + 1);

		prevPoint = centerPoint;
		centerPoint = nextPoint;
		nextPoint = waypoints[next];
		
	}

	void PrevPoints()
	{
		int prev = (next - 3) < 0 ? (((next - 3) + waypoints.Length) % waypoints.Length) : (next - 3);
		next = (next - 1) < 0 ? (((next - 1) + waypoints.Length) % waypoints.Length) : (next - 1);

		nextPoint = centerPoint;
		centerPoint = prevPoint;
		prevPoint = waypoints[prev];

	}

	void Update () 
	{
		relativeFitness = Vector3.Distance(centerPoint.position, car.position);


		double centerCarDistance = Vector3.Distance(centerPoint.position, car.position);
		double prevCenterDistance = Vector3.Distance(prevPoint.position, centerPoint.position);
		double centerNextDistance = Vector3.Distance(centerPoint.position, nextPoint.position);
		double prevCarDistance = Vector3.Distance(prevPoint.position, car.position);
		double nextCarDistance = Vector3.Distance(car.position, nextPoint.position);

		

		if (prevCarDistance < nextCarDistance && relativeFitness > 0)
		{
			relativeFitness *= -1;
		}

		if (centerCarDistance > centerNextDistance && nextCarDistance < prevCarDistance)
		{
			savedFitness += centerNextDistance;
			NextPoints();
		}

		if (centerCarDistance > prevCenterDistance && prevCarDistance < nextCarDistance)
		{
			savedFitness -= prevCenterDistance;
			PrevPoints();
		}


		fitness = savedFitness + relativeFitness;
		if(fitness < 0)
		{
			wrongwayText.text = "WRONG WAY";
		} else 
		{
			wrongwayText.text = "";
		}

		text.text = "Fitness: " + string.Format("{0:0.000}", fitness);
	}

	#endregion
	
}
