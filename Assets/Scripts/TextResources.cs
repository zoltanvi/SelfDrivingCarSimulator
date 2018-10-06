using System.Collections.Generic;

public class TextResources
{

    private Dictionary<string, string> m_Hungarian;
    private Dictionary<string, string> m_English;

    private static GameLanguage m_language = GameLanguage.ENGLISH;
    public const string versionNumber = "4.0";

    public static GameLanguage Language
    {
        get
        {
            return m_language;
        }

        set
        {
            m_language = value;
        }
    }

    public TextResources()
    {
        m_English = new Dictionary<string, string>(){
			#region menu
			{"menu_start", "START"},
            {"menu_options", "OPTIONS"},
            {"menu_load", "LOAD"},
            {"menu_exit", "EXIT"},
            {"menu_back", "BACK"},
            {"menu_dev", "Developer: Zoltán Vicze"},
            {"menu_version", "version " + versionNumber},			
			#endregion

			#region options
			{"options_num_of_cars", "NUMBER OF CARS"},
            {"options_selection_method", "SELECTION METHOD"},
            {"options_mutation", "MUTATION"},
            {"options_possibility", "POSSIBILITY"},
            {"options_rate", "RATE"},
            {"options_num_of_layers", "NUMBER OF LAYERS"},
            {"options_neuron_per_layer", "NEURON PER LAYER"},
            {"options_navigator", "NAVIGATOR"},
            {"options_demo_mode", "DEMO MODE"},
            {"options_selection_top50", "Top 50%"},
            {"options_selection_tournament", "Tournament method"},
            {"options_selection_20random", "Tournament and 20% random each round"},
            {"options_map", "MAP"},
			#endregion

			#region tooltip
			{"tooltip_num_of_cars", "Number of cars"},
            {"tooltip_selection_method", "Selection method"},
            {"tooltip_mutation_possibility", "Mutation possibility"},
            {"tooltip_mutation_rate", "Mutation rate"},
            {"tooltip_num_of_layers", "Number of layers"},
            {"tooltip_neuron_per_layer", "Neuron per layer"},
            {"tooltip_navigator", "Navigator"},
            {"tooltip_demo_mode", "Demo mode"},

            {"tooltip_num_of_cars_desc", "The size of the population."},
            {"tooltip_selection_method_desc", "Selection is the stage of a genetic algorithm " +
                    "in which individual genomes are chosen from a population for " +
                    "later breeding (using the crossover operator)."},
            {"tooltip_mutation_possibility_desc", "The chance that the mutation will occur during recombination."},
            {"tooltip_mutation_rate_desc", "The rate of the mutation that will occur during recombination."},
            {"tooltip_num_of_layers_desc", "It specifies how many neuron layers will be in a single neural network. " +
                    "\n\nA car's brain is it's neural network. " +
                    "These cars' neural networks are built up by neural layers " +
                    "which are built up by neurons."},
            {"tooltip_neuron_per_layer_desc", "It specifies how many neurons will be in a single neuron layer. " +
                    "A car's brain is it's neural network. " +
                    "These cars' neural networks are built up by neural layers " +
                    "which are built up by neurons."},
            {"tooltip_navigator_desc", "A map have checkpoints between it's walls, roughly in the middle. " +
                    "If you connect the next two points, they form lines. The lines form angles." +
                    "If the navigator option is checked, the cars will get the next 3 angle as its input."},
            {"tooltip_demo_mode_desc", "-- If this is checked, it overwrites all of your settings! --\n" +
                    "Neural networks start with values from a state reached in a previous run.\n" +
                    "(For faster demonstration)"},
			#endregion

			#region load menu
			{"load_select_map", "SELECT MAP"},
            {"load_select_game", "Select a saved game to load"},
			#endregion

			#region HUD
			{"hud_remaining_time", "Remaining Time"},
            {"hud_freeze_time", "Freeze Time"},
            {"hud_generation", "Generation"},
            {"hud_population", "Population"},
            {"hud_playing_cars", "Playing cars"},
            {"hud_mutation_rate", "Mutation rate"},
            {"hud_creature", "Creature"},
            {"hud_fitness", "Fitness"},
            {"hud_max", "Max"},
            {"hud_median", "Median"},
			#endregion

			#region ingame menu
			{"ingame_join", "JOIN GAME"},
            {"ingame_disconnect", "DISCONNECT"},
            {"ingame_save_game", "SAVE GAME"},
            {"ingame_save_stats", "SAVE STATISTICS"},
            {"ingame_ff_2", "FAST FORWARD (2X)"},
            {"ingame_ff_5", "FAST FORWARD (5X)"},
            {"ingame_normal_speed", "NORMAL SPEED"},
            {"ingame_exit", "EXIT TO MAIN MENU"}			
			#endregion
		};


        m_Hungarian = new Dictionary<string, string>(){
			#region menu
			{"menu_start", "START"},
            {"menu_options", "BEÁLLÍTÁSOK"},
            {"menu_load", "BETÖLTÉS"},
            {"menu_exit", "KILÉPÉS"},
            {"menu_back", "VISSZA"},
            {"menu_dev", "Fejlesztö: Vicze Zoltán"},
            {"menu_version", "verzió " + versionNumber},			
			#endregion

			#region options
			{"options_num_of_cars", "AUTÓK SZÁMA"},
            {"options_selection_method", "KIVÁLASZTÁSI MÓD"},
            {"options_mutation", "MUTÁCIÓ"},
            {"options_possibility", "ESÉLYE"},
            {"options_rate", "MÉRTÉKE"},
            {"options_num_of_layers", "RÉTEGEK SZÁMA"},
            {"options_neuron_per_layer", "NEURON RÉTEGENKÉNT"},
            {"options_navigator", "NAVIGÁTOR"},
            {"options_demo_mode", "DEMÓ MÓD"},
            {"options_selection_top50", "Top 50%"},
            {"options_selection_tournament", "Tournament mód"},
            {"options_selection_20random", "Tournament és 20% random minden körben"},
            {"options_map", "PÁLYA"},
			#endregion

			#region tooltip
			{"tooltip_num_of_cars", "Autók száma"},
            {"tooltip_selection_method", "Kiválasztási mód"},
            {"tooltip_mutation_possibility", "Mutáció esélye"},
            {"tooltip_mutation_rate", "Mutáció mértéke"},
            {"tooltip_num_of_layers", "Rétegek száma"},
            {"tooltip_neuron_per_layer", "Neuron rétegenként"},
            {"tooltip_navigator", "Navigátor"},
            {"tooltip_demo_mode", "Demó mód"},

            {"tooltip_num_of_cars_desc", "A populáció mérete."},
            {"tooltip_selection_method_desc",
                "A szelekció a genetikus algoritmus egyik fázisa, " +
                "amelyben egyének kerülnek kiválasztásra késöbbi " +
                "szaporítás céljából (a rekombinációs fázisban)."},
            {"tooltip_mutation_possibility_desc", "Az esély, hogy a mutáció elöfordul rekombináció közben."},
            {"tooltip_mutation_rate_desc", "A mutáció mértéke, ami rekombináció közben történhet."},
            {"tooltip_num_of_layers_desc", "Meghatározza, hogy hány neuronrétegböl álljon a neurális hálózat." +
                "\n\nEgy autó agya az ö neurális hálózata. " +
                "Ezek a neurális hálózatok neuronrétegekböl állnak, amelyek pedig neuronokból állnak."},
            {"tooltip_neuron_per_layer_desc", "Meghatározza, hogy hány neuronból álljon egy neuronréteg." +
                "\n\nEgy autó agya az ö neurális hálózata. " +
                "Ezek a neurális hálózatok neuronrétegekböl állnak, amelyek pedig neuronokból állnak."},
            {"tooltip_navigator_desc", "A pályán ellenörzöpontok találhatóak a falak között, nagyjából középen elhelyezve. " +
	            "Ha két egymást követö pontot összekötünk, egyenest alkotnak. Két egymást követö vonal meghatároz egy szöget." + 
	            "Ha a navigátor opció be van jelölve, az autó megkapja inputként a következö 3 ilyen szöget is."},
            {"tooltip_demo_mode_desc", "-- Ha ez az opció be van jelölve, minden más beállítást felülír! --\n" +
                    "Az autók neurális hálózatai egy demó módra szánt mentést kapnak, nem véletlenszerüen fognak inicializálódni.\n" +
                    "(A gyorsabb demonstráció érdekében)"},
			#endregion

			#region load menu
			{"load_select_map", "PÁLYA KIVÁLASZTÁSA"},
            {"load_select_game", "Válassz egy mentést"},
			#endregion

			#region HUD
			{"hud_remaining_time", "Fennmaradó idö"},
            {"hud_freeze_time", "Fagyasztási idö"},
            {"hud_generation", "Generáció"},
            {"hud_population", "Populáció"},
            {"hud_playing_cars", "Versenyben"},
            {"hud_mutation_rate", "Mutáció mértéke"},
            {"hud_creature", "Autó"},
            {"hud_fitness", "Fitnessz"},
            {"hud_max", "Max"},
            {"hud_median", "Medián"},
			#endregion

			#region ingame menu
			{"ingame_join", "CSATLAKOZÁS"},
            {"ingame_disconnect", "LECSATLAKOZÁS"},
            {"ingame_save_game", "JÁTÉK MENTÉSE"},
            {"ingame_save_stats", "STAT MENTÉSE"},
            {"ingame_ff_2", "IDÖGYORSÍTÁS (2X)"},
            {"ingame_ff_5", "IDÖGYORSÍTÁS (5X)"},
            {"ingame_normal_speed", "NORMÁL SEBESSÉG"},
            {"ingame_exit", "FÖMENÜ"}			
			#endregion
	

		};


    }

    public string GetString(string key)
    {
        string answer = "";
        switch (m_language)
        {
            case GameLanguage.ENGLISH:
                answer = m_English[key];
                break;

            case GameLanguage.HUNGARIAN:
                answer = m_Hungarian[key];
                break;

        }
        return answer;
    }

}

public enum GameLanguage
{
    HUNGARIAN,
    ENGLISH
}