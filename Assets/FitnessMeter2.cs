using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FitnessMeter2 : MonoBehaviour
{

	#region Variables
	[SerializeField] private Transform checkpoints;
	[SerializeField] private Transform car;
	private Transform[] stack = new Transform[2];
	private Transform[] cps;
	private int nextCP = 0;
	private double fitness = 0f;
	[SerializeField] Text fitnessText;

	private Stack<double> st = new Stack<double>();
	//private int nextWP = 0;

	#endregion

	#region Unity Methods

	#region start and update
	void Start()
	{
		int i = 0;
		cps = new Transform[checkpoints.childCount];

		foreach (Transform cp in checkpoints)
		{
			cps[i++] = cp;
		}
	}


	void Update()
	{
		fitnessText.text = "fitness: " + fitness;
		
	}
	#endregion

	void OnTriggerExit(Collider cpCollider)
	{
		if (cpCollider.gameObject.layer == LayerMask.NameToLayer("Checkpoints"))
		{

			if (nextCP == 0)
			{
				double myDistance = Vector3.Distance(car.position, cps[nextCP].position);
				double originDistance = Vector3.Distance(new Vector3(0f, 0f, 0f), cps[nextCP].position);

				if (myDistance < originDistance)
				{
					Debug.Log("nextCP++");
					fitness += originDistance;
					st.Push(originDistance);
					//elozoertek = originDistance;
					nextCP++;
				}
				if (myDistance > originDistance && cpCollider.gameObject.transform.name == cps[nextCP - 1].name)
				{
					Debug.Log("megint atmentel azon, amin elozoleg!");
					//fitness -= elozoertek;
					fitness -= st.Pop();
					nextCP--;
				}
			}
			else
			{
				double myDistance = Vector3.Distance(car.position, cps[nextCP].position);
				double cpDistance = Vector3.Distance(cps[nextCP - 1].position, cps[nextCP].position);

				if (myDistance < cpDistance)
				{
					Debug.Log("nextCP++");
					fitness += cpDistance;
					st.Push(cpDistance);
					//elozoertek = cpDistance;
					nextCP++;
				}
				if (myDistance > cpDistance && cpCollider.gameObject.transform.name == cps[nextCP - 1].name)
				{
					Debug.Log("megint atmentel azon, amin elozoleg!");
					fitness -= st.Pop();

					//	fitness -= elozoertek;
					nextCP--;
					
				}
			}
		}


	}

	void Push(Transform transform)
	{
		if (stack[1] == null)
		{
			stack[1] = transform;
		}
		else
		{
			stack[0] = stack[1];
			stack[1] = transform;
		}
	}

	Transform PeekFirst()
	{
		return stack[0];
	}

	Transform PeekSecond()
	{
		return stack[1];
	}

	#endregion

}
