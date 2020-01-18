using Crosstales.FB;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuController2 : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown m_SelectionMethodDropdown;
    [SerializeField] private TMP_Dropdown m_MutationPossibilityDropdown;
    [SerializeField] private TMP_Dropdown m_MutationRateDropdown;

    [SerializeField] private Slider m_NumberOfCarsSlider;
    [SerializeField] private Slider m_NumberOfLayersSlider;
    [SerializeField] private Slider m_NeuronPerLayerSlider;
    [SerializeField] private Slider m_NumberOfSimulationsSlider;
    [SerializeField] private TextMeshProUGUI m_NumberOfCars;
    [SerializeField] private TextMeshProUGUI m_NumberOfLayers;
    [SerializeField] private TextMeshProUGUI m_NeuronPerLayer;
    [SerializeField] private TextMeshProUGUI m_NumberOfSimulations;
    [SerializeField] private TextMeshProUGUI m_TooltipTitle;
    [SerializeField] private TextMeshProUGUI m_TooltipText;

    [SerializeField] private Toggle m_NavigatorToggle;
    [SerializeField] private Toggle m_DemoModeToggle;
    [SerializeField] private Toggle m_StopConditionToggle;

    [SerializeField] private Image m_Track01Image;
    [SerializeField] private Image m_Track02Image;
    [SerializeField] private Image m_Track03Image;

    [SerializeField] private Image m_LoadTrack01Image;
    [SerializeField] private Image m_LoadTrack02Image;
    [SerializeField] private Image m_LoadTrack03Image;

    [SerializeField] private TMP_InputField m_SeedInputField;
    [SerializeField] private TMP_InputField m_StopGenerationInputField;
    [SerializeField] private TMP_InputField m_PathInputField;


    private readonly Color m_White = new Color(1.0f, 1.0f, 1.0f);
    private readonly Color m_Gray = new Color(0.254717f, 0.254717f, 0.254717f);

    private List<string> m_Selections = new List<string>();
    private List<string> m_MutationChances = new List<string> { "30%", "40%", "50%", "60%", "70%" };
    private List<string> m_MutationRates = new List<string> { "2%", "2.5%", "3%", "3.5%", "4%" };

    private Master m_Master;

    // Use this for initialization
    private void Start()
    {
        m_Master = Master.Instance;
        m_Selections.Add(LocalizationManager.Instance.GetLocalizedValue("options_selection_method_tournament"));
        m_Selections.Add(LocalizationManager.Instance.GetLocalizedValue("options_selection_method_top50"));
        m_Selections.Add(LocalizationManager.Instance.GetLocalizedValue("options_selection_method_20random"));
        m_Selections.Add(LocalizationManager.Instance.GetLocalizedValue("options_selection_method_roulette"));

        PopulateDropdowns();

        // Default értékek, ha nem megy bele a felhasználó a beállításokba
        m_SeedInputField.text = RandomHelper.Seed.ToString();
        UpdateAllUIElement();
    }

    public void UpdateAllUIElement()
    {
        Configuration config = m_Master.EditConfiguration;

        m_NumberOfCars.text = config.CarCount.ToString();
        m_NumberOfCarsSlider.value = config.CarCount;
        m_SelectionMethodDropdown.value = config.SelectionMethod;
        m_MutationPossibilityDropdown.value = GetMutationChanceDropdownIndex(config.MutationChance);
        m_MutationRateDropdown.value = GetMutationRateDropdownIndex(config.MutationRate);
        m_NumberOfLayers.text = config.LayersCount.ToString();
        m_NumberOfLayersSlider.value = config.LayersCount;
        m_NeuronPerLayer.text = config.NeuronPerLayerCount.ToString();
        m_NeuronPerLayerSlider.value = config.NeuronPerLayerCount;
        m_NavigatorToggle.isOn = config.Navigator;
        m_DemoModeToggle.isOn = config.DemoMode;
        m_StopConditionToggle.isOn = config.StopConditionActive;
        m_StopGenerationInputField.text = config.StopGenerationNumber.ToString();
        UpdateMapSelection(config.TrackNumber);
        UpdateNumberOfSimulations();
    }

    private int GetMutationRateDropdownIndex(float mutationRate)
    {
        if (mutationRate == 2f)
        {
            return 0;
        }
        if (mutationRate == 2.5f)
        {
            return 1;
        }
        if (mutationRate == 3f)
        {
            return 2;
        }
        if (mutationRate == 3.5f)
        {
            return 3;
        }
        if (mutationRate == 4f)
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
        m_NumberOfCars.text = m_NumberOfCarsSlider.value.ToString();
        m_Master.EditConfiguration.CarCount = (int)m_NumberOfCarsSlider.value;
    }

    public void UpdateSelectionMethod()
    {
        m_Master.EditConfiguration.SelectionMethod = m_SelectionMethodDropdown.value;
    }

    public void UpdateMutationPossibility()
    {
        m_Master.EditConfiguration.MutationChance = GetMutationChanceFromIndex(m_MutationPossibilityDropdown.value);
    }

    public void UpdateMutationRate()
    {
        m_Master.EditConfiguration.MutationRate = GetMutationRateFromIndex(m_MutationRateDropdown.value);
    }

    public void UpdateNumberOfLayers()
    {
        m_NumberOfLayers.text = m_NumberOfLayersSlider.value.ToString();
        m_Master.EditConfiguration.LayersCount = (int)m_NumberOfLayersSlider.value;
    }

    public void UpdateNeuronPerLayer()
    {
        m_NeuronPerLayer.text = m_NeuronPerLayerSlider.value.ToString();
        m_Master.EditConfiguration.NeuronPerLayerCount = (int)m_NeuronPerLayerSlider.value;
    }

    public void UpdateNumberOfSimulations()
    {
        m_NumberOfSimulations.text = m_NumberOfSimulationsSlider.value.ToString();
    }

    public void UpdateStopGenerationNumber()
    {
        int stopNumber = 0;
        int.TryParse(m_StopGenerationInputField.text, out stopNumber);
        if (stopNumber != 0)
        {
            m_Master.EditConfiguration.StopGenerationNumber = stopNumber;
        }
    }

    public void UpdateMapSelection(int selection)
    {
        m_Master.EditConfiguration.TrackNumber = selection;

        switch (selection)
        {
            case 0:
                m_Track01Image.color = m_White;
                m_Track02Image.color = m_Gray;
                m_Track03Image.color = m_Gray;
                break;

            case 1:
                m_Track01Image.color = m_Gray;
                m_Track02Image.color = m_White;
                m_Track03Image.color = m_Gray;
                break;

            case 2:
                m_Track01Image.color = m_Gray;
                m_Track02Image.color = m_Gray;
                m_Track03Image.color = m_White;
                break;
        }
    }

    public void UpdateLoadMapSelection(int selection)
    {
        m_Master.Manager.LoadTrackNumber = selection;

        switch (selection)
        {
            case 0:
                m_LoadTrack01Image.color = m_White;
                m_LoadTrack02Image.color = m_Gray;
                m_LoadTrack03Image.color = m_Gray;
                break;

            case 1:
                m_LoadTrack01Image.color = m_Gray;
                m_LoadTrack02Image.color = m_White;
                m_LoadTrack03Image.color = m_Gray;
                break;

            case 2:
                m_LoadTrack01Image.color = m_Gray;
                m_LoadTrack02Image.color = m_Gray;
                m_LoadTrack03Image.color = m_White;
                break;
        }
    }

    public void ToggleStopConditionActive()
    {
        m_Master.EditConfiguration.StopConditionActive = m_StopConditionToggle.isOn;
    }

    public void ToggleNavigatorModeActive()
    {
        m_Master.EditConfiguration.Navigator = m_NavigatorToggle.isOn;
    }

    public void ToggleDemoModeActive()
    {
        m_Master.EditConfiguration.DemoMode = m_DemoModeToggle.isOn;
    }

    public void NewSeedInputText()
    {
        m_SeedInputField.text = RandomHelper.Seed.ToString();
    }

    public void SetSeedFromText()
    {
        int seedNum;
        int.TryParse(m_SeedInputField.text, out seedNum);
        RandomHelper.Seed = seedNum;

        m_Master.Manager.ShowPopUp(LocalizationManager.Instance.GetLocalizedValue("options_seed_set") + RandomHelper.Seed);
    }

    #endregion UPDATE METODUSOK VEGE ====================================================

    public void ChangedLanguage()
    {
        m_Selections.Clear();
        m_SelectionMethodDropdown.ClearOptions();

        m_Selections.Add(LocalizationManager.Instance.GetLocalizedValue("options_selection_method_tournament"));
        m_Selections.Add(LocalizationManager.Instance.GetLocalizedValue("options_selection_method_top50"));
        m_Selections.Add(LocalizationManager.Instance.GetLocalizedValue("options_selection_method_20random"));
        m_Selections.Add(LocalizationManager.Instance.GetLocalizedValue("options_selection_method_roulette"));


        m_SelectionMethodDropdown.AddOptions(m_Selections);
    }

    private void PopulateDropdowns()
    {
        m_SelectionMethodDropdown.AddOptions(m_Selections);
        m_MutationPossibilityDropdown.AddOptions(m_MutationChances);
        m_MutationRateDropdown.AddOptions(m_MutationRates);
    }

    public void ShowTooltipMsg(int tooltip)
    {
        switch (tooltip)
        {
            // Number of cars
            case 0:
                m_TooltipTitle.text = LocalizationManager.Instance.GetLocalizedValue("tooltip_title_number_of_cars");
                m_TooltipText.text = LocalizationManager.Instance.GetLocalizedValue("tooltip_description_number_of_cars");
                break;
            // Selection method
            case 1:
                m_TooltipTitle.text = LocalizationManager.Instance.GetLocalizedValue("tooltip_title_selection_method");
                m_TooltipText.text = LocalizationManager.Instance.GetLocalizedValue("tooltip_description_selection_method");
                break;
            // Mutation possibility
            case 2:
                m_TooltipTitle.text = LocalizationManager.Instance.GetLocalizedValue("tooltip_title_mutation_possibility");
                m_TooltipText.text = LocalizationManager.Instance.GetLocalizedValue("tooltip_description_mutation_possibility");
                break;
            // Mutation rate
            case 3:
                m_TooltipTitle.text = LocalizationManager.Instance.GetLocalizedValue("tooltip_title_mutation_rate");
                m_TooltipText.text = LocalizationManager.Instance.GetLocalizedValue("tooltip_description_mutation_rate");
                break;
            // Number of layers
            case 4:
                m_TooltipTitle.text = LocalizationManager.Instance.GetLocalizedValue("tooltip_title_number_of_layers");
                m_TooltipText.text = LocalizationManager.Instance.GetLocalizedValue("tooltip_description_number_of_layers");
                break;
            // Neuron per layer
            case 5:
                m_TooltipTitle.text = LocalizationManager.Instance.GetLocalizedValue("tooltip_title_neuron_per_layer");
                m_TooltipText.text = LocalizationManager.Instance.GetLocalizedValue("tooltip_description_neuron_per_layer");
                break;
            // Navigator
            case 6:
                m_TooltipTitle.text = LocalizationManager.Instance.GetLocalizedValue("tooltip_title_navigator");
                m_TooltipText.text = LocalizationManager.Instance.GetLocalizedValue("tooltip_description_navigator");
                break;
            // Demo mode
            case 7:
                m_TooltipTitle.text = LocalizationManager.Instance.GetLocalizedValue("tooltip_title_demo_mode");
                m_TooltipText.text = LocalizationManager.Instance.GetLocalizedValue("tooltip_description_demo_mode");
                break;
            // Stop condition
            case 8:
                m_TooltipTitle.text = LocalizationManager.Instance.GetLocalizedValue("tooltip_title_stop_at_generation");
                m_TooltipText.text = LocalizationManager.Instance.GetLocalizedValue("tooltip_description_stop_at_generation");
                break;
            // Map selection
            case 9:
                m_TooltipTitle.text = LocalizationManager.Instance.GetLocalizedValue("tooltip_title_map");
                m_TooltipText.text = LocalizationManager.Instance.GetLocalizedValue("tooltip_description_map");
                break;
            // Number of simulations
            case 10:
                m_TooltipTitle.text = LocalizationManager.Instance.GetLocalizedValue("tooltip_title_number_of_simulations");
                m_TooltipText.text = LocalizationManager.Instance.GetLocalizedValue("tooltip_description_number_of_simulations");

                break;
            // Seed
            case 11:
                m_TooltipTitle.text = LocalizationManager.Instance.GetLocalizedValue("tooltip_title_seed");
                m_TooltipText.text = LocalizationManager.Instance.GetLocalizedValue("tooltip_description_seed");
                break;
            // Save path
            case 12:
                m_TooltipTitle.text = LocalizationManager.Instance.GetLocalizedValue("tooltip_title_save_path");
                m_TooltipText.text = LocalizationManager.Instance.GetLocalizedValue("tooltip_description_save_path");
                break;
            default:
                m_TooltipTitle.text = string.Empty;
                m_TooltipText.text = string.Empty;
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

        m_PathInputField.text = path;
    }

    public void SetDefaultSaveLocation()
    {
        Master.Instance.DefaultSaveLocation = m_PathInputField.text;
    }

    private void ShowPopUp(string message)
    {
        Master.Instance.PopUpText.ShowPopUp(message);
    }
}
