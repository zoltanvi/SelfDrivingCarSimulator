using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FitnessMeter : MonoBehaviour
{
	/*
	#region Variables
	[SerializeField] private Transform car;
	[SerializeField] private Transform m_Checkpoints;
	private Transform[] m_CheckpointTransforms;
	private Transform[] m_CheckpointStack = new Transform[3];
	[SerializeField] private Text m_fitnessText;
	private Dictionary<string, int> m_CheckpointDictionary;
	private string m_Layername = "Checkpoints";
	//private Stack<string> m_CheckpointStack = new Stack<string>();
	private float m_fitness = 0f;
	private int i = 0;
	#endregion

	#region Unity Methods

	void Start()
	{
		m_CheckpointTransforms = new Transform[m_Checkpoints.childCount];
		m_CheckpointDictionary = new Dictionary<string, int>();
		foreach (Transform checkpoint in m_Checkpoints)
		{
			m_CheckpointTransforms[i] = checkpoint;
			m_CheckpointDictionary.Add(checkpoint.name, i++);
		}

		--i;

	}

	#region trigger
	//void OnTriggerExit(Collider other)
	//{


	//	string triggeredCheckpointName = other.gameObject.name;

	//	// ha athaladt a checkpointon az auto
	//	if (other.gameObject.layer == LayerMask.NameToLayer(m_Layername))
	//	{

	//		if (m_CheckpointStack[2] == null)
	//		{
	//			m_CheckpointStack[2] = other.transform;
	//		}
	//		else
	//		{
	//			m_CheckpointStack[0] = m_CheckpointStack[1];
	//			m_CheckpointStack[1] = m_CheckpointStack[2];
	//			m_CheckpointStack[2] = other.transform;
	//		}

	//		int? a = m_CheckpointDictionary[m_CheckpointStack[0].name];
	//		int? b = m_CheckpointDictionary[m_CheckpointStack[1].name];
	//		int? c = m_CheckpointDictionary[m_CheckpointStack[2].name];

	//		if (b == 0 && c == i)	// ha a rajttól hátra fele mentünk 1 cp-t 
	//		{
	//			m_fitness -= Vector3.Distance(m_CheckpointStack[1].position, m_CheckpointStack[2].position);
	//		}
	//		else if (b == 0 && c > b)	// ha a rajttól előre fele mentünk 1 cp-t
	//		{
	//			m_fitness += Vector3.Distance(m_CheckpointStack[1].position, m_CheckpointStack[2].position);
	//		}





	//		m_fitness += Vector3.Distance(m_CheckpointStack[0].position, m_CheckpointStack[1].position);

	//	}
	//}
	#endregion

	void FixedUpdate()
	{
		
		m_fitness = Time.deltaTime;
		m_fitnessText.text = "fitness: " + m_fitness;
	}


	#endregion


	*/
}
