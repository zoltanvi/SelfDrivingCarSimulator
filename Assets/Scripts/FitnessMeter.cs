using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FitnessMeter : MonoBehaviour
{

	#region Variables
	[SerializeField] private Transform m_Checkpoints;
	[SerializeField] private Text m_fitnessText;
	private Dictionary<string, int> m_CheckpointDictionary;
	private string m_Layername = "Checkpoints";
	private Stack<string> m_CheckpointStack = new Stack<string>();
	private float m_fitness = 0f;
	#endregion

	#region Unity Methods

	void Start()
	{
		m_CheckpointDictionary = new Dictionary<string, int>();
		int i = 0;
		foreach (Transform checkpoint in m_Checkpoints)
		{
			m_CheckpointDictionary.Add(checkpoint.name, i++);
			//	Debug.Log(checkpoint.name + ", " + (i - 1) + "added!");
		}

	}

	void OnTriggerEnter(Collider other)
	{
		string triggeredCheckpointName = other.gameObject.name;

		if (other.gameObject.layer == LayerMask.NameToLayer(m_Layername))
		{

			if (m_CheckpointStack.Count == 0)
			{
				m_CheckpointStack.Push(triggeredCheckpointName);
			}
			else if (m_CheckpointStack.Peek() == triggeredCheckpointName)   // visszafele megy az auto
			{
				//TODO: levonni a fitness ertekebol 
				m_CheckpointStack.Pop();
				m_fitness -= 1f;
			}
			else
			{   //elore megy az auto
				m_CheckpointStack.Push(triggeredCheckpointName);
				m_fitness += 1;
			}

			if (m_CheckpointDictionary.ContainsKey(triggeredCheckpointName))
			{

				//	Debug.Log(other.gameObject.name + " was in the collection!");
			}
		}
	}

	void FixedUpdate()
	{
		m_fitnessText.text = "fitness: " + m_fitness;
	}


	#endregion

}
