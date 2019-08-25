

[System.Serializable]
public class Configuration {

    public int CarCount { get; set; }
    public int SelectionMethod { get; set; }
    public int MutationChance { get; set; }
    public float MutationRate { get; set; }
    public int LayersCount { get; set; }
    public int NeuronPerLayerCount { get; set; }
    public int TrackNumber { get; set; }
    public bool Navigator { get; set; }
    public bool StopConditionActive { get; set; }
    public int StopGenerationNumber { get; set; }
    public int Seed { get; set; }

}
