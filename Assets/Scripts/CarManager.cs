using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarManager : MonoBehaviour
{
	/*
	[SerializeField][Range(0, 20)]
	private int numberOfCars = 1;
	[SerializeField]
	private GameObject carPreFab;
	[SerializeField]
	FollowCar followCar;
	[SerializeField]
	private GameObject myUI;
	private UIPrinter myUIPrinter;

	public static string[] carDistances;
	public static double[] carFitness;

	public static int carRIndex = 0;
	public static int carFIndex = 0;

	private Transform bestCar;
	private Transform[] cars;


	void Start()
	{
		carDistances = new string[numberOfCars];
		carFitness = new double[numberOfCars];

		cars = new Transform[numberOfCars];

		myUI = GameObject.Find("myUI");
		myUIPrinter = myUI.GetComponent<UIPrinter>();


		for (int i = 0; i < numberOfCars; i++)
		{
			cars[i] = Instantiate(carPreFab, transform.position, transform.rotation).transform;
		}

		followCar.targetCar = cars[0];
	}




	void Update()
	{

		int bestCarIndex = BestCarIndex();
		followCar.targetCar = cars[bestCarIndex];
		myUIPrinter.SensorDistances = carDistances[bestCarIndex];
		myUIPrinter.FitnessValue = carFitness[bestCarIndex];
	}

	private int BestCarIndex()
	{
		int index = 0;
		double bestFitness = 0;
		for (int i = 0; i < numberOfCars; i++)
		{
			if (bestFitness < carFitness[i])
			{
				bestFitness = carFitness[i];
				index = i;
			}
		}
		return index;
	}
	*/

}
