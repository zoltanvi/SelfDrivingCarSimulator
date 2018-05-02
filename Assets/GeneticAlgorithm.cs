using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneticAlgorithm : MonoBehaviour
{

	#region Prefabs
	[SerializeField] private GameObject blueCarPrefab;
	[SerializeField] private GameObject redCarPrefab;
	[SerializeField] private GameObject blueCarMesh;
	#endregion

	#region Materials
	[SerializeField] private Material blueMat;
	[SerializeField] private Material blueMatTransparent;
	[SerializeField] private Material wheelMat;
	[SerializeField] private Material wheelMatTransparent;
	#endregion

	[SerializeField] private CameraDrone cameraDrone;
	[SerializeField] private GameObject UIStats;


	public int PopulationSize { get; set; }
	public Car[] Cars;
	private Stat[] stats;
	private Queue<GameObject> carPool;

	#region Player joins
	private GameObject playerCar;
	public double PlayerFitness { get; set; }
	private bool isPlayerAlive = false;
	#endregion

	#region Genetic algorithm settings
	public float MutationChance { get; set; }
	public float MutationRate { get; set; }
	#endregion

	void Start()
	{

	}


	void Update()
	{

	}
}
