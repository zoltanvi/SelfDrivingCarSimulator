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

	TextResources textResources;

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

	private List<string> selections = new List<string>();

	private List<string> mutationChances = new List<string>() {
		"30%", "40%", "50%", "60%", "70%"
	};

	private List<string> mutationRates = new List<string>() {
		"2%", "2.5%", "3%", "3.5%", "4%"
	};


	// Use this for initialization
	void Start()
	{
		textResources = new TextResources();
		selections.Add(textResources.GetString("options_selection_tournament"));
		selections.Add(textResources.GetString("options_selection_top50"));
		selections.Add(textResources.GetString("options_selection_20random"));


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
		Master.Instance.Manager.TrackNumber = TrackNumber;
	}

	public void SetOptionValues()
	{

		Master.Instance.Manager.CarCount = NumberOfCars;
		Master.Instance.Manager.LayersCount = NumberOfLayers;
		Master.Instance.Manager.NeuronPerLayerCount = NeuronPerLayer;
		Master.Instance.Manager.TrackNumber = TrackNumber;
		Master.Instance.Manager.SelectionMethod = SelectionMethodE; // Ez a managerben van lekezelve
		Master.Instance.Manager.Navigator = Navigator;
		Master.Instance.Manager.DemoMode = DemoMode;

		// Az érték beállítása a dropdownból kinyert adat szerint
		switch (MutationChanceE)
		{
			case 0:
				Master.Instance.Manager.MutationChance = 30;
				break;
			case 1:
				Master.Instance.Manager.MutationChance = 40;
				break;
			case 2:
				Master.Instance.Manager.MutationChance = 50;
				break;
			case 3:
				Master.Instance.Manager.MutationChance = 60;
				break;
			case 4:
				Master.Instance.Manager.MutationChance = 70;
				break;
			default:
				Master.Instance.Manager.MutationChance = 50;
				break;
		}

		switch (MutationRateE)
		{
			case 0:
				Master.Instance.Manager.MutationRate = 2f;
				break;
			case 1:
				Master.Instance.Manager.MutationRate = 2.5f;
				break;
			case 2:
				Master.Instance.Manager.MutationRate = 3f;
				break;
			case 3:
				Master.Instance.Manager.MutationRate = 3.5f;
				break;
			case 4:
				Master.Instance.Manager.MutationRate = 4f;
				break;
			default:
				Master.Instance.Manager.MutationRate = 3f;
				break;
		}

		Master.Instance.Manager.GotOptionValues = true;

		Debug.Log("Gave values to manager!");

	}

	public void ShowTooltipMsg(int tooltip)
	{
		switch (tooltip)
		{
			// Number of cars
			case 0:
				tooltipTitle.text = textResources.GetString("tooltip_num_of_cars");
				tooltipText.text = textResources.GetString("tooltip_num_of_cars_desc");
				break;
			// Selection method
			case 1:
				tooltipTitle.text = textResources.GetString("tooltip_selection_method");
				tooltipText.text = textResources.GetString("tooltip_selection_method_desc");
				break;
			// Mutation possibility
			case 2:
				tooltipTitle.text = textResources.GetString("tooltip_mutation_possibility");
				tooltipText.text = textResources.GetString("tooltip_mutation_possibility_desc");
				break;
			// Mutation rate
			case 3:
				tooltipTitle.text = textResources.GetString("tooltip_mutation_rate");
				tooltipText.text = textResources.GetString("tooltip_mutation_rate_desc");
				break;
			// Number of layers
			case 4:
				tooltipTitle.text = textResources.GetString("tooltip_num_of_layers");
				tooltipText.text = textResources.GetString("tooltip_num_of_layers_desc");
				break;
			// Neuron per layer
			case 5:
				tooltipTitle.text = textResources.GetString("tooltip_neuron_per_layer");
				tooltipText.text = textResources.GetString("tooltip_neuron_per_layer_desc");
				break;
			// Navigator
			case 6:
				tooltipTitle.text = textResources.GetString("tooltip_navigator");
				tooltipText.text = textResources.GetString("tooltip_navigator_desc");
				break;
			// Demo mode
			case 7:
				tooltipTitle.text = textResources.GetString("tooltip_demo_mode");
				tooltipText.text = textResources.GetString("tooltip_demo_mode_desc");
				break;
			default:
				tooltipTitle.text = "";
				tooltipText.text = "";
				break;
		}
	}

	public void ChangedLanguage(){
		selections.Clear();
		selectionDropdown.ClearOptions();

		selections.Add(textResources.GetString("options_selection_tournament"));
		selections.Add(textResources.GetString("options_selection_top50"));
		selections.Add(textResources.GetString("options_selection_20random"));

		selectionDropdown.AddOptions(selections);
	}

}
