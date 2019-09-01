using Crosstales.FB;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuController2 : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown selectionMethodDropdown;
    [SerializeField] private TMP_Dropdown mutationPossibilityDropdown;
    [SerializeField] private TMP_Dropdown mutationRateDropdown;

    [SerializeField] private Slider numberOfCarsSlider;
    [SerializeField] private Slider numberOfLayersSlider;
    [SerializeField] private Slider neuronPerLayerSlider;
    [SerializeField] private Slider numberOfSimulationsSlider;
    [SerializeField] private TextMeshProUGUI numberOfCars;
    [SerializeField] private TextMeshProUGUI numberOfLayers;
    [SerializeField] private TextMeshProUGUI neuronPerLayer;
    [SerializeField] private TextMeshProUGUI numberOfSimulations;
    [SerializeField] private TextMeshProUGUI tooltipTitle;
    [SerializeField] private TextMeshProUGUI tooltipText;

    [SerializeField] private Toggle navigatorToggle;
    [SerializeField] private Toggle demoModeToggle;
    [SerializeField] private Toggle stopConditionToggle;

    [SerializeField] private Image track01Image;
    [SerializeField] private Image track02Image;
    [SerializeField] private Image track03Image;

    [SerializeField] private Image loadTrack01Image;
    [SerializeField] private Image loadTrack02Image;
    [SerializeField] private Image loadTrack03Image;

    [SerializeField] private TMP_InputField seedInputField;
    [SerializeField] private TMP_InputField stopGenerationInputField;
    [SerializeField] private TMP_InputField pathInputField;


    private readonly Color white = new Color(1.0f, 1.0f, 1.0f);
    private readonly Color gray = new Color(0.254717f, 0.254717f, 0.254717f);

    private List<string> selections = new List<string>();
    private List<string> mutationChances = new List<string>() { "30%", "40%", "50%", "60%", "70%" };
    private List<string> mutationRates = new List<string>() { "2%", "2.5%", "3%", "3.5%", "4%" };

    private Master master;

    // Use this for initialization
    private void Start()
    {
        master = Master.Instance;
        selections.Add(LocalizationManager.Instance.GetLocalizedValue("options_selection_method_tournament"));
        selections.Add(LocalizationManager.Instance.GetLocalizedValue("options_selection_method_top50"));
        selections.Add(LocalizationManager.Instance.GetLocalizedValue("options_selection_method_20random"));
        selections.Add(LocalizationManager.Instance.GetLocalizedValue("options_selection_method_roulette"));

        PopulateDropdowns();

        // Default értékek, ha nem megy bele a felhasználó a beállításokba
        seedInputField.text = RandomHelper.Seed.ToString();
        UpdateAllUIElement();
    }

    public void UpdateAllUIElement()
    {
        Configuration config = master.EditConfiguration;

        numberOfCars.text = config.CarCount.ToString();
        numberOfCarsSlider.value = config.CarCount;
        selectionMethodDropdown.value = config.SelectionMethod;
        mutationPossibilityDropdown.value = GetMutationChanceDropdownIndex(config.MutationChance);
        mutationRateDropdown.value = GetMutationRateDropdownIndex(config.MutationRate);
        numberOfLayers.text = config.LayersCount.ToString();
        numberOfLayersSlider.value = config.LayersCount;
        neuronPerLayer.text = config.NeuronPerLayerCount.ToString();
        neuronPerLayerSlider.value = config.NeuronPerLayerCount;
        navigatorToggle.isOn = config.Navigator;
        demoModeToggle.isOn = config.DemoMode;
        stopConditionToggle.isOn = config.StopConditionActive;
        stopGenerationInputField.text = config.StopGenerationNumber.ToString();
        UpdateMapSelection(config.TrackNumber);
        UpdateNumberOfSimulations();
    }

    private int GetMutationRateDropdownIndex(float mutationRate)
    {
        if (mutationRate == 2f)
        {
            return 0;
        }
        else if (mutationRate == 2.5f)
        {
            return 1;
        }
        else if (mutationRate == 3f)
        {
            return 2;
        }
        else if (mutationRate == 3.5f)
        {
            return 3;
        }
        else if (mutationRate == 4f)
        {
            return 4;
        }

        return 0;
    }

    private float GetMutationRateFromIndex(int dropdownIndex)
    {
        switch (dropdownIndex)
        {
            case 0:
            return 2f;
            case 1:
            return 2.5f;
            case 2:
            return 3f;
            case 3:
            return 3.5f;
            case 4:
            return 4f;
            default:
            return 2f;
        }
    }

    private int GetMutationChanceDropdownIndex(int mutationChance)
    {
        switch (mutationChance)
        {
            case 30:
            return 0;
            case 40:
            return 1;
            case 50:
            return 2;
            case 60:
            return 3;
            case 70:
            return 4;
            default:
            return 0;
        }
    }

    private int GetMutationChanceFromIndex(int dropdownIndex)
    {
        switch (dropdownIndex)
        {
            case 0:
            return 30;
            case 1:
            return 40;
            case 2:
            return 50;
            case 3:
            return 60;
            case 4:
            return 70;
            default:
            return 30;
        }
    }

    #region UPDATE METODUSOK ============================================================
    public void UpdateNumberOfCars()
    {
        numberOfCars.text = numberOfCarsSlider.value.ToString();
        master.EditConfiguration.CarCount = (int)numberOfCarsSlider.value;
    }

    public void UpdateSelectionMethod()
    {
        master.EditConfiguration.SelectionMethod = selectionMethodDropdown.value;
    }

    public void UpdateMutationPossibility()
    {
        master.EditConfiguration.MutationChance = GetMutationChanceFromIndex(mutationPossibilityDropdown.value);
    }

    public void UpdateMutationRate()
    {
        master.EditConfiguration.MutationRate = GetMutationRateFromIndex(mutationRateDropdown.value);
    }

    public void UpdateNumberOfLayers()
    {
        numberOfLayers.text = numberOfLayersSlider.value.ToString();
        master.EditConfiguration.LayersCount = (int)numberOfLayersSlider.value;
    }

    public void UpdateNeuronPerLayer()
    {
        neuronPerLayer.text = neuronPerLayerSlider.value.ToString();
        master.EditConfiguration.NeuronPerLayerCount = (int)neuronPerLayerSlider.value;
    }

    public void UpdateNumberOfSimulations()
    {
        numberOfSimulations.text = numberOfSimulationsSlider.value.ToString();
    }

    public void UpdateStopGenerationNumber()
    {
        int stopNumber = 0;
        int.TryParse(stopGenerationInputField.text, out stopNumber);
        if (stopNumber != 0)
        {
            master.EditConfiguration.StopGenerationNumber = stopNumber;
        }
    }

    public void UpdateMapSelection(int selection)
    {
        master.EditConfiguration.TrackNumber = selection;

        switch (selection)
        {
            case 0:
            track01Image.color = white;
            track02Image.color = gray;
            track03Image.color = gray;
            break;

            case 1:
            track01Image.color = gray;
            track02Image.color = white;
            track03Image.color = gray;
            break;

            case 2:
            track01Image.color = gray;
            track02Image.color = gray;
            track03Image.color = white;
            break;
        }
    }

    public void UpdateLoadMapSelection(int selection)
    {
        master.Manager.LoadTrackNumber = selection;

        switch (selection)
        {
            case 0:
            loadTrack01Image.color = white;
            loadTrack02Image.color = gray;
            loadTrack03Image.color = gray;
            break;

            case 1:
            loadTrack01Image.color = gray;
            loadTrack02Image.color = white;
            loadTrack03Image.color = gray;
            break;

            case 2:
            loadTrack01Image.color = gray;
            loadTrack02Image.color = gray;
            loadTrack03Image.color = white;
            break;
        }
    }

    public void ToggleStopConditionActive()
    {
        master.EditConfiguration.StopConditionActive = stopConditionToggle.isOn;
    }

    public void ToggleNavigatorModeActive()
    {
        master.EditConfiguration.Navigator = navigatorToggle.isOn;
    }

    public void ToggleDemoModeActive()
    {
        master.EditConfiguration.DemoMode = demoModeToggle.isOn;
    }

    public void NewSeedInputText()
    {
        seedInputField.text = RandomHelper.Seed.ToString();
    }

    #endregion UPDATE METODUSOK VEGE ====================================================

    public void ChangedLanguage()
    {
        selections.Clear();
        selectionMethodDropdown.ClearOptions();

        selections.Add(LocalizationManager.Instance.GetLocalizedValue("options_selection_method_tournament"));
        selections.Add(LocalizationManager.Instance.GetLocalizedValue("options_selection_method_top50"));
        selections.Add(LocalizationManager.Instance.GetLocalizedValue("options_selection_method_20random"));
        selections.Add(LocalizationManager.Instance.GetLocalizedValue("options_selection_method_roulette"));


        selectionMethodDropdown.AddOptions(selections);
    }

    private void PopulateDropdowns()
    {
        selectionMethodDropdown.AddOptions(selections);
        mutationPossibilityDropdown.AddOptions(mutationChances);
        mutationRateDropdown.AddOptions(mutationRates);
    }

    public void ShowTooltipMsg(int tooltip)
    {
        switch (tooltip)
        {
            // Number of cars
            case 0:
            tooltipTitle.text = LocalizationManager.Instance.GetLocalizedValue("tooltip_title_number_of_cars");
            tooltipText.text = LocalizationManager.Instance.GetLocalizedValue("tooltip_description_number_of_cars");
            break;
            // Selection method
            case 1:
            tooltipTitle.text = LocalizationManager.Instance.GetLocalizedValue("tooltip_title_selection_method");
            tooltipText.text = LocalizationManager.Instance.GetLocalizedValue("tooltip_description_selection_method");
            break;
            // Mutation possibility
            case 2:
            tooltipTitle.text = LocalizationManager.Instance.GetLocalizedValue("tooltip_title_mutation_possibility");
            tooltipText.text = LocalizationManager.Instance.GetLocalizedValue("tooltip_description_mutation_possibility");
            break;
            // Mutation rate
            case 3:
            tooltipTitle.text = LocalizationManager.Instance.GetLocalizedValue("tooltip_title_mutation_rate");
            tooltipText.text = LocalizationManager.Instance.GetLocalizedValue("tooltip_description_mutation_rate");
            break;
            // Number of layers
            case 4:
            tooltipTitle.text = LocalizationManager.Instance.GetLocalizedValue("tooltip_title_number_of_layers");
            tooltipText.text = LocalizationManager.Instance.GetLocalizedValue("tooltip_description_number_of_layers");
            break;
            // Neuron per layer
            case 5:
            tooltipTitle.text = LocalizationManager.Instance.GetLocalizedValue("tooltip_title_neuron_per_layer");
            tooltipText.text = LocalizationManager.Instance.GetLocalizedValue("tooltip_description_neuron_per_layer");
            break;
            // Navigator
            case 6:
            tooltipTitle.text = LocalizationManager.Instance.GetLocalizedValue("tooltip_title_navigator");
            tooltipText.text = LocalizationManager.Instance.GetLocalizedValue("tooltip_description_navigator");
            break;
            // Demo mode
            case 7:
            tooltipTitle.text = LocalizationManager.Instance.GetLocalizedValue("tooltip_title_demo_mode");
            tooltipText.text = LocalizationManager.Instance.GetLocalizedValue("tooltip_description_demo_mode");
            break;
            // Stop condition
            case 8:
            tooltipTitle.text = LocalizationManager.Instance.GetLocalizedValue("tooltip_title_stop_at_generation");
            tooltipText.text = LocalizationManager.Instance.GetLocalizedValue("tooltip_description_stop_at_generation");
            break;
            // Map selection
            case 9:
            tooltipTitle.text = LocalizationManager.Instance.GetLocalizedValue("tooltip_title_map");
            tooltipText.text = LocalizationManager.Instance.GetLocalizedValue("tooltip_description_map");
            break;
            // Number of simulations
            case 10:
            tooltipTitle.text = LocalizationManager.Instance.GetLocalizedValue("tooltip_title_number_of_simulations");
            tooltipText.text = LocalizationManager.Instance.GetLocalizedValue("tooltip_description_number_of_simulations");

            break;
            // Seed
            case 11:
            tooltipTitle.text = LocalizationManager.Instance.GetLocalizedValue("tooltip_title_seed");
            tooltipText.text = LocalizationManager.Instance.GetLocalizedValue("tooltip_description_seed");
            break;
            // Save path
            case 12:
            tooltipTitle.text = LocalizationManager.Instance.GetLocalizedValue("tooltip_title_save_path");
            tooltipText.text = LocalizationManager.Instance.GetLocalizedValue("tooltip_description_save_path");
            break;
            default:
            tooltipTitle.text = string.Empty;
            tooltipText.text = string.Empty;
            break;
        }
    }

    public void BrowsePath()
    {
        string path = FileBrowser.OpenSingleFolder(LocalizationManager.Instance.GetLocalizedValue("filebrowser_select_default_save_location"));

        if (path.Length == 0)
        {
            ShowPopUp(LocalizationManager.Instance.GetLocalizedValue("popup_no_folder_selected"));
            Debug.Log(LocalizationManager.Instance.GetLocalizedValue("popup_no_folder_selected"));
            return;
        }
        else
        {
            pathInputField.text = path;
        }
    }

    public void SetDefaultSaveLocation()
    {
        Master.Instance.DefaultSaveLocation = pathInputField.text;
    }

    private void ShowPopUp(string message)
    {
        Master.Instance.popUp.ShowPopUp(message);
    }

}
