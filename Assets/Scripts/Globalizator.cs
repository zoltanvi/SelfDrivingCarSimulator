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

    TextResources tR;

    void Start()
    {
        tR = new TextResources();
    }


    public void SetLanguage(GameLanguage language)
    {
        TextResources.Language = language;

        #region menu
        start.text = tR.GetString("menu_start");
        options.text = tR.GetString("menu_options");
        load.text = tR.GetString("menu_load");
        exit.text = tR.GetString("menu_exit");
        version.text = tR.GetString("menu_dev") + "\n" + tR.GetString("menu_version");
        #endregion

        #region options
        numOfCars.text = tR.GetString("options_num_of_cars");
        selectionMethod.text = tR.GetString("options_selection_method");
        mutation.text = tR.GetString("options_mutation");
        possibility.text = tR.GetString("options_possibility");
        rate.text = tR.GetString("options_rate");
        numOfLayers.text = tR.GetString("options_num_of_layers");
        neuronPerLayer.text = tR.GetString("options_neuron_per_layer");
        navigator.text = tR.GetString("options_navigator");
        demoMode.text = tR.GetString("options_demo_mode");
        optionsBack.text = tR.GetString("menu_back");

        map.text = tR.GetString("options_map");
        #endregion

        #region load
        selectMap.text = tR.GetString("load_select_map");
        loadLoad.text = tR.GetString("menu_load");
        loadBack.text = tR.GetString("menu_back");
        #endregion

        #region ingame menu
        join.text = tR.GetString("ingame_join");
        disconnect.text = tR.GetString("ingame_disconnect");
        save.text = tR.GetString("ingame_save_game");
        saveStats.text = tR.GetString("ingame_save_stats");
        normalSpeed.text = tR.GetString("ingame_normal_speed");
        fastForward2.text = tR.GetString("ingame_ff_2");
        fastForward5.text = tR.GetString("ingame_ff_5");
        exitToMenu.text = tR.GetString("ingame_exit");
        #endregion

        #region hud
        remainingTime.text = tR.GetString("hud_remaining_time");
        freezeTime.text = tR.GetString("hud_freeze_time");
        generation.text = tR.GetString("hud_generation");
        population.text = tR.GetString("hud_population");
        playingCars.text = tR.GetString("hud_playing_cars");
        mutationRate.text = tR.GetString("hud_mutation_rate");
        creature.text = tR.GetString("hud_creature");
        fitness.text = tR.GetString("hud_fitness");
        max.text = tR.GetString("hud_max");
        median.text = tR.GetString("hud_median");
        #endregion


    }



}
