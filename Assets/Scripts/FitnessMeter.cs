using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FitnessMeter : MonoBehaviour
{

	#region Variables
	#endregion

	#region Unity Methods

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == LayerMask.NameToLayer("Checkpoints"))
		{
			Debug.Log(other.gameObject.name);
		}
		else
		{
			Debug.Log("wall... i think");
		}
	}

	void Update()
	{

	}

	#endregion

}
