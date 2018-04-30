using System.Collections;
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

	public int Selection { get; set; }
	public int MutationChance { get; set; }
	public int MutationRate { get; set; }
	public int NumberOfCars { get; set; }
	public int NumberOfLayers { get; set; }
	public int NeuronPerLayer { get; set; }
	public bool Navigator { get; set; }

	private List<string> selections = new List<string>() {
		"Top 50%", "Tournament method", "Tournament + worst 20% full random"
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
		NumberOfCars = 20;
		NumberOfLayers = 3;
		NeuronPerLayer = 6;
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
}
