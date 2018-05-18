using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GeneticAlgorithm : MonoBehaviour
{



	protected CameraDrone cameraDrone;
	protected GameObject UIStats;


	public int PopulationSize { get; set; }

	protected Stat[] stats;

	protected Queue<GameObject> carPool;

	#region Player joins
	protected GameObject playerCar;
	public double PlayerFitness { get; set; }
	protected bool isPlayerAlive = false;
	#endregion

	#region Genetic algorithm settings
	public float MutationChance { get; set; }
	public float MutationRate { get; set; }
	#endregion

	void Awake()
	{
		PopulationSize = Manager.Instance.CarCount;
		// Megfelelő SelectionMethod switch-case
		MutationChance = Manager.Instance.MutationChance; // 30-70 int %
		MutationRate = Manager.Instance.MutationRate;	// 2-4 float %


	}


	public abstract void RecombineAndMutate();



}
