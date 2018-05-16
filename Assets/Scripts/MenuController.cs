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

	public int SelectionMethodE { get; set; }
	public int MutationChanceE { get; set; }
	public int MutationRateE { get; set; }
	public int NumberOfCars { get; set; }
	public int NumberOfLayers { get; set; }
	public int NeuronPerLayer { get; set; }
	public bool Navigator { get; set; }
	public int TrackNumber { get; set; }

	private List<string> selections = new List<string>() {
		"Tournament method", "Top 50%", "Tournament + worst 20% full random"
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
		SelectionMethodE = 0;           // Tournament
		MutationChanceE = 2;        // 50%
		MutationRateE = 3;      // 3.5%

		numOfCarsSlider.value = NumberOfCars;
		numOfLayersSlider.value = NumberOfLayers;
		neuronPerLayerSlider.value = NeuronPerLayer;
		selectionDropdown.value = SelectionMethodE;
		mutationChanceDropdown.value = MutationChanceE;
		mutationRateDropdown.value = MutationRateE;

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
	}

	public void SetOptionValues()
	{

		Manager.Instance.CarCount = NumberOfCars;
		Manager.Instance.LayersCount = NumberOfLayers;
		Manager.Instance.NeuronPerLayerCount = NeuronPerLayer;

		Manager.Instance.SelectionMethod = SelectionMethodE; // Ez a managerben van lekezelve

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



}
