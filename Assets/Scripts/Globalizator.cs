using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Globalizator : MonoBehaviour
{

    #region menu
    public TextMeshProUGUI start;
    public TextMeshProUGUI options;
    public TextMeshProUGUI load;
    public TextMeshProUGUI exit;
    public TextMeshProUGUI version;
    #endregion

    #region options
    public TextMeshProUGUI numOfCars;
    public TextMeshProUGUI selectionMethod;
    public TextMeshProUGUI mutation;
    public TextMeshProUGUI possibility;
    public TextMeshProUGUI rate;
    public TextMeshProUGUI numOfLayers;
    public TextMeshProUGUI neuronPerLayer;
    public TextMeshProUGUI navigator;
    public TextMeshProUGUI demoMode;
    public TextMeshProUGUI optionsBack;

    public TextMeshProUGUI map;
    public TextMeshProUGUI tooltipTitle;
    public TextMeshProUGUI tooltipText;
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
    public TextMeshProUGUI remainingTime;
    public TextMeshProUGUI freezeTime;
    public TextMeshProUGUI generation;
    public TextMeshProUGUI population;
    public TextMeshProUGUI playingCars;
    public TextMeshProUGUI mutationRate;
    public TextMeshProUGUI creature;
    public TextMeshProUGUI fitness;
    public TextMeshProUGUI max;
    public TextMeshProUGUI median;
    #endregion

    TextResources textResources;

    void Start()
    {
        textResources = new TextResources();
        SetLanguage(GameLanguage.ENGLISH);
        
    }


    public void SetLanguage(GameLanguage language)
    {
        TextResources.Language = language;

        #region menu
        start.text = textResources.GetValue("menu_start");
        options.text = textResources.GetValue("menu_options");
        load.text = textResources.GetValue("menu_load");
        exit.text = textResources.GetValue("menu_exit");
        version.text = textResources.GetValue("menu_dev") + "\n" + textResources.GetValue("menu_version");
        #endregion

        #region options
        numOfCars.text = textResources.GetValue("options_num_of_cars");
        selectionMethod.text = textResources.GetValue("options_selection_method");
        mutation.text = textResources.GetValue("options_mutation");
        possibility.text = textResources.GetValue("options_possibility");
        rate.text = textResources.GetValue("options_rate");
        numOfLayers.text = textResources.GetValue("options_num_of_layers");
        neuronPerLayer.text = textResources.GetValue("options_neuron_per_layer");
        navigator.text = textResources.GetValue("options_navigator");
        demoMode.text = textResources.GetValue("options_demo_mode");
        optionsBack.text = textResources.GetValue("menu_back");

        map.text = textResources.GetValue("options_map");
        #endregion

        #region load
        selectMap.text = textResources.GetValue("load_select_map");
        loadLoad.text = textResources.GetValue("menu_load");
        loadBack.text = textResources.GetValue("menu_back");
        #endregion

        #region ingame menu
        join.text = textResources.GetValue("ingame_join");
        disconnect.text = textResources.GetValue("ingame_disconnect");
        save.text = textResources.GetValue("ingame_save_game");
        saveStats.text = textResources.GetValue("ingame_save_stats");
        normalSpeed.text = textResources.GetValue("ingame_normal_speed");
        fastForward2.text = textResources.GetValue("ingame_ff_2");
        fastForward5.text = textResources.GetValue("ingame_ff_5");
        exitToMenu.text = textResources.GetValue("ingame_exit");
        #endregion

        #region hud
        remainingTime.text = textResources.GetValue("hud_remaining_time");
        freezeTime.text = textResources.GetValue("hud_freeze_time");
        generation.text = textResources.GetValue("hud_generation");
        population.text = textResources.GetValue("hud_population");
        playingCars.text = textResources.GetValue("hud_playing_cars");
        mutationRate.text = textResources.GetValue("hud_mutation_rate");
        creature.text = textResources.GetValue("hud_creature");
        fitness.text = textResources.GetValue("hud_fitness");
        max.text = textResources.GetValue("hud_max");
        median.text = textResources.GetValue("hud_median");
        #endregion


    }



}
