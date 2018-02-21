using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OldFitness02 : MonoBehaviour
{

	#region Variables
	[SerializeField] private Transform checkpoints;
	[SerializeField] private Transform car;
	private Transform[] cps;
	private int nextCP = 1;
	private Transform zeroCP, oneCP;
	private double fitness = 0;
	[SerializeField] Text fitnessText;


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

			if (nextCP == 1) // ha a rajttól indulunk épp
			{
				double myDistance = Vector3.Distance(car.position, cps[nextCP].position);
				double originDistance = Vector3.Distance(cps[(nextCP - 1) < 0 ? cps.Length - 1 : nextCP - 1].position, cps[nextCP].position);

				if (myDistance < originDistance) // és az autóval előre indultunk
				{
					fitness += originDistance;

					nextCP = (nextCP + 1) > (cps.Length - 1) ? 0 : nextCP + 1;
				}
				if (myDistance > originDistance && cpCollider.gameObject.transform.name == cps[(nextCP - 1) < 0 ? cps.Length-1 : nextCP-1].name)
				{
					// de ha a rajttól hátra indulunk el

					double fd = Vector3.Distance(cps[(nextCP - 1) < 0 ? cps.Length - 1 : nextCP - 1].position, cps[nextCP].position);
					fitness -= fd;

					nextCP = (nextCP - 1) < 0 ? (cps.Length - 1) : (nextCP - 1);
				}
			}
			else
			{

				double myDistance = Vector3.Distance(car.position, cps[nextCP].position);
				double cpDistance = Vector3.Distance(cps[(nextCP - 1) < 0 ? cps.Length - 1 : nextCP - 1].position, cps[nextCP].position);

				if (myDistance < cpDistance) // ha közelebb vagyok a következő checkpointhoz mint az előtte lévő cp
				{
					fitness += cpDistance;
					nextCP = (nextCP + 1) > (cps.Length - 1) ? 0 : nextCP + 1;

				}
				if (myDistance > cpDistance && cpCollider.gameObject.transform.name == cps[(nextCP - 1) < 0 ? cps.Length - 1 : nextCP - 1].name)
				{

					double fd = Vector3.Distance(cps[(nextCP - 1) < 0 ? cps.Length - 1 : nextCP - 1].position, cps[nextCP].position);
					fitness -= fd;
					nextCP = (nextCP - 1) < 0 ? (cps.Length - 1) : (nextCP - 1);

				}
			}
		}


	}
		


	#endregion

}
