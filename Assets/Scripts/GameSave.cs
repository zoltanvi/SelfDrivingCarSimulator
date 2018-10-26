using System.Collections.Generic;


[System.Serializable]
public class GameSave
{

	#region Manager értékei
	public int SelectionMethod;
	public int MutationChance;
	public float MutationRate;
	public int CarCount;
	public int LayersCount;
	public int NeuronPerLayerCount;
	public bool Navigator;
	public int TrackNumber;
	#endregion

	#region GeneticAlgorithm értékei
	// Az összes autó neurális hálójának értékeit tárolja
	public float[][][][] SavedCarNetworks;
	public int GenerationCount;
	#endregion

	#region Statisztikai adatok
	public List<float> MaxFitness;
	public List<float> MedianFitness;
	#endregion

}

