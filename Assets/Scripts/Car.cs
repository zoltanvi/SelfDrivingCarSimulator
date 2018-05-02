using UnityEngine;

public class Car
{
	public int ID { get; set; }
	public double Fitness { get; set; }
	public double[] Inputs { get; set; }
	public GameObject GameObject { get; set; }
	public Transform Transform { get; set; }
	public CarController CarController { get; set; }
	public NeuralNetwork NeuralNetwork { get; set; }
	public double PrevFitness { get; set; }
	public string InputText { get; set; }

}

