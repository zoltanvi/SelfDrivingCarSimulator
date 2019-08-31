using TMPro;
using UnityEngine;


public class Globalizator : MonoBehaviour
{

	#region menu
	//public TextMeshProUGUI start;
	//public TextMeshProUGUI options;
	//public TextMeshProUGUI load;
	//public TextMeshProUGUI exit;
	public TextMeshProUGUI version;
	#endregion

	#region options
	//public TextMeshProUGUI numOfCars;
	//public TextMeshProUGUI selectionMethod;
	//public TextMeshProUGUI mutation;
	//public TextMeshProUGUI possibility;
	//public TextMeshProUGUI rate;
	//public TextMeshProUGUI numOfLayers;
	//public TextMeshProUGUI neuronPerLayer;
	//public TextMeshProUGUI navigator;
	//public TextMeshProUGUI demoMode;
	public TextMeshProUGUI optionsBack;
	//public TextMeshProUGUI map;
	#endregion

	#region load
	public TextMeshProUGUI selectMap;
	public TextMeshProUGUI loadLoad;
	public TextMeshProUGUI loadBack;
	#endregion

	#region ingame menu
	public TextMeshProUGUI join;
	public TextMeshProUGUI disconnect;
	public TextMeshProUGUI save;
	public TextMeshProUGUI saveStats;
	public TextMeshProUGUI normalSpeed;
	public TextMeshProUGUI fastForward2;
	public TextMeshProUGUI fastForward5;
	public TextMeshProUGUI exitToMenu;
	#endregion

	#region hud
	//public TextMeshProUGUI remainingTime;
	//public TextMeshProUGUI freezeTime;
	//public TextMeshProUGUI generation;
	//public TextMeshProUGUI population;
	//public TextMeshProUGUI playingCars;
	//public TextMeshProUGUI mutationRate;
	//public TextMeshProUGUI creature;
	//public TextMeshProUGUI fitness;
	//public TextMeshProUGUI max;
	//public TextMeshProUGUI median;
	#endregion


	private void Start()
	{
		SetLanguage(GameLanguage.English);
	}


	public void SetLanguage(GameLanguage language)
	{
		TextResources.Language = language;

		#region menu
		//start.text = TextResources.GetValue("menu_start");
		//options.text = TextResources.GetValue("menu_options");
		//load.text = TextResources.GetValue("menu_load");
		//exit.text = TextResources.GetValue("menu_exit");
		version.text = TextResources.GetValue("menu_dev") + "\n" + TextResources.GetValue("menu_version");
		#endregion

		#region options
		//numOfCars.text = TextResources.GetValue("options_num_of_cars");
		//selectionMethod.text = TextResources.GetValue("options_selection_method");
		//mutation.text = TextResources.GetValue("options_mutation");
		//possibility.text = TextResources.GetValue("options_possibility");
		//rate.text = TextResources.GetValue("options_rate");
		//numOfLayers.text = TextResources.GetValue("options_num_of_layers");
		//neuronPerLayer.text = TextResources.GetValue("options_neuron_per_layer");
		//navigator.text = TextResources.GetValue("options_navigator");
		//demoMode.text = TextResources.GetValue("options_demo_mode");
		optionsBack.text = TextResources.GetValue("menu_back");

		//map.text = TextResources.GetValue("options_map");
		#endregion

		#region load
		selectMap.text = TextResources.GetValue("load_select_map");
		loadLoad.text = TextResources.GetValue("menu_load");
		loadBack.text = TextResources.GetValue("menu_back");
		#endregion

		#region ingame menu
		join.text = TextResources.GetValue("ingame_join");
		disconnect.text = TextResources.GetValue("ingame_disconnect");
		save.text = TextResources.GetValue("ingame_save_game");
		saveStats.text = TextResources.GetValue("ingame_save_stats");
		normalSpeed.text = TextResources.GetValue("ingame_normal_speed");
		fastForward2.text = TextResources.GetValue("ingame_ff_2");
		fastForward5.text = TextResources.GetValue("ingame_ff_5");
		exitToMenu.text = TextResources.GetValue("ingame_exit");
		#endregion

		#region hud
		//remainingTime.text = TextResources.GetValue("hud_remaining_time");
		//freezeTime.text = TextResources.GetValue("hud_freeze_time");
		//generation.text = TextResources.GetValue("hud_generation");
		//population.text = TextResources.GetValue("hud_population");
		//playingCars.text = TextResources.GetValue("hud_playing_cars");
		//mutationRate.text = TextResources.GetValue("hud_mutation_rate");
		//creature.text = TextResources.GetValue("hud_creature");
		//fitness.text = TextResources.GetValue("hud_fitness");
		//max.text = TextResources.GetValue("hud_max");
		//median.text = TextResources.GetValue("hud_median");
		#endregion


	}

}

