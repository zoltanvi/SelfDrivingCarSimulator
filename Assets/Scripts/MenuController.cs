using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuController : MonoBehaviour
{

	[SerializeField] private Dropdown selectionDropdown;
	[SerializeField] private Dropdown mutationChanceDropdown;
	[SerializeField] private Dropdown mutationRateDropdown;

	[SerializeField] private Slider numOfCarsSlider;
	[SerializeField] private Slider numOfLayersSlider;
	[SerializeField] private Slider neuronPerLayerSlider;

	[SerializeField] private TextMeshProUGUI numOfCars;
	[SerializeField] private TextMeshProUGUI numOfLayers;
	[SerializeField] private TextMeshProUGUI neuronPerLayer;
	[SerializeField] private Toggle navigator;

	[SerializeField] private Image track01Image;
	[SerializeField] private Image track02Image;
	[SerializeField] private Image track03Image;

	[SerializeField] private Image track01ImageLoad;
	[SerializeField] private Image track02ImageLoad;
	[SerializeField] private Image track03ImageLoad;

	[SerializeField] private TextMeshProUGUI tooltipTitle;
	[SerializeField] private TextMeshProUGUI tooltipText;


	private Color white = new Color(1.0f, 1.0f, 1.0f);
	private Color gray = new Color(0.254717f, 0.254717f, 0.254717f);


	public int SelectionMethodE { get; set; }
	public int MutationChanceE { get; set; }
	public int MutationRateE { get; set; }
	public int NumberOfCars { get; set; }
	public int NumberOfLayers { get; set; }
	public int NeuronPerLayer { get; set; }
	public int TrackNumber { get; set; }
	public bool Navigator { get; set; }
	public bool DemoMode { get; set; }

	private List<string> selections = new List<string>() {
		"Tournament method", "Top 50%", "Tournament and 20% random each round"
	};

	private List<string> mutationChances = new List<string>() {
		"30%", "40%", "50%", "60%", "70%"
	};

	private List<string> mutationRates = new List<string>() {
		"2%", "2.5%", "3%", "3.5%", "4%"
	};


	// Use this for initialization
	void Start()
	{
		PopulateDropdowns();

		// Default értékek, ha nem megy bele a felhasználó a beállításokba
		NumberOfCars = 20;
		NumberOfLayers = 3;
		NeuronPerLayer = 6;
		SelectionMethodE = 1;           // Top 50%
		MutationChanceE = 2;        // 50%
		MutationRateE = 2;      // 3.5%

		numOfCarsSlider.value = NumberOfCars;
		numOfLayersSlider.value = NumberOfLayers;
		neuronPerLayerSlider.value = NeuronPerLayer;
		selectionDropdown.value = SelectionMethodE;
		mutationChanceDropdown.value = MutationChanceE;
		mutationRateDropdown.value = MutationRateE;
		navigator.isOn = Navigator;

	}

	void PopulateDropdowns()
	{
		selectionDropdown.AddOptions(selections);
		mutationChanceDropdown.AddOptions(mutationChances);
		mutationRateDropdown.AddOptions(mutationRates);
	}

	public void SetCarCount()
	{
		NumberOfCars = (int)numOfCarsSlider.value;
	}

	public void SetNumberOfLayers()
	{
		NumberOfLayers = (int)numOfLayersSlider.value;
	}

	public void SetNeuronPerLayer()
	{
		NeuronPerLayer = (int)neuronPerLayerSlider.value;
	}

	// Update is called once per frame
	void Update()
	{
		numOfCars.text = NumberOfCars.ToString();
		numOfLayers.text = NumberOfLayers.ToString();
		neuronPerLayer.text = NeuronPerLayer.ToString();

		switch (TrackNumber)
		{
			case 0:
				track01Image.color = white;
				track02Image.color = gray;
				track03Image.color = gray;

				track01ImageLoad.color = white;
				track02ImageLoad.color = gray;
				track03ImageLoad.color = gray;
				break;

			case 1:
				track01Image.color = gray;
				track02Image.color = white;
				track03Image.color = gray;

				track01ImageLoad.color = gray;
				track02ImageLoad.color = white;
				track03ImageLoad.color = gray;
				break;
				
			case 2:
				track01Image.color = gray;
				track02Image.color = gray;
				track03Image.color = white;

				track01ImageLoad.color = gray;
				track02ImageLoad.color = gray;
				track03ImageLoad.color = white;
				break;
				
			default:
				track01Image.color = white;
				track02Image.color = gray;
				track03Image.color = gray;

				track01ImageLoad.color = white;
				track02ImageLoad.color = gray;
				track03ImageLoad.color = gray;
				break;
		}
	}

	public void SetLoadTrackNumber()
	{
		Manager.Instance.TrackNumber = TrackNumber;
	}

	public void SetOptionValues()
	{

		Manager.Instance.CarCount = NumberOfCars;
		Manager.Instance.LayersCount = NumberOfLayers;
		Manager.Instance.NeuronPerLayerCount = NeuronPerLayer;
		Manager.Instance.TrackNumber = TrackNumber;
		Manager.Instance.SelectionMethod = SelectionMethodE; // Ez a managerben van lekezelve
		Manager.Instance.Navigator = Navigator;
		Manager.Instance.DemoMode = DemoMode;

		// Az érték beállítása a dropdownból kinyert adat szerint
		switch (MutationChanceE)
		{
			case 0:
				Manager.Instance.MutationChance = 30;
				break;
			case 1:
				Manager.Instance.MutationChance = 40;
				break;
			case 2:
				Manager.Instance.MutationChance = 50;
				break;
			case 3:
				Manager.Instance.MutationChance = 60;
				break;
			case 4:
				Manager.Instance.MutationChance = 70;
				break;
			default:
				Manager.Instance.MutationChance = 50;
				break;
		}

		switch (MutationRateE)
		{
			case 0:
				Manager.Instance.MutationRate = 2f;
				break;
			case 1:
				Manager.Instance.MutationRate = 2.5f;
				break;
			case 2:
				Manager.Instance.MutationRate = 3f;
				break;
			case 3:
				Manager.Instance.MutationRate = 3.5f;
				break;
			case 4:
				Manager.Instance.MutationRate = 4f;
				break;
			default:
				Manager.Instance.MutationRate = 3f;
				break;
		}

		Manager.Instance.GotOptionValues = true;

		Debug.Log("Gave values to manager!");

	}

	public void ShowTooltipMsg(int tooltip)
	{
		switch (tooltip)
		{
			// Number of cars
			case 0:
				tooltipTitle.text = "Number of cars";
				tooltipText.text = "The size of the population.";
				break;
			// Selection method
			case 1:
				tooltipTitle.text = "Selection method";
				tooltipText.text = "Selection is the stage of a genetic algorithm " +
					"in which individual genomes are chosen from a population for " +
					"later breeding (using the crossover operator).";
				break;
			// Mutation possibility
			case 2:
				tooltipTitle.text = "Mutation possibility";
				tooltipText.text = "The chance that the mutation will occur during recombination.";
				break;
			// Mutation rate
			case 3:
				tooltipTitle.text = "Mutation rate";
				tooltipText.text = "The rate of the mutation that will occur during recombination.";
				break;
			// Number of layers
			case 4:
				tooltipTitle.text = "Number of layers";
				tooltipText.text = "It specifies how many neuron layers will be in a single neural network. " +
					"\n\nA car's brain is it's neural network. " +
					"These cars' neural networks are built up by neural layers " +
					"which are built up by neurons.";
				break;
			// Neuron per layer
			case 5:
				tooltipTitle.text = "Neuron per layer";
				tooltipText.text = "It specifies how many neurons will be in a single neuron layer. " + 
					"A car's brain is it's neural network. " +
					"These cars' neural networks are built up by neural layers " +
					"which are built up by neurons.";
				break;
			// Navigator
			case 6:
				tooltipTitle.text = "Navigator";
				tooltipText.text = "A map have checkpoints between it's walls, roughly in the middle. " +
					"If you connect the next two points, they form lines. The lines form angles." +
					"If the navigator option is checked, the cars will get the next 3 angle as its input.";
				break;
			case 7:
				tooltipTitle.text = "Demo mode";
				tooltipText.text = "-- If this is checked, it overwrites all of your settings! --\n" +
					"Neural networks start with values from a state reached in a previous run.\n" +
					"(For faster demonstration)";
				break;
			default:
				tooltipTitle.text = "";
				tooltipText.text = "";
				break;
		}
	}



}
