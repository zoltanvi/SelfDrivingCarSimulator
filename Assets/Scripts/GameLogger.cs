using System.IO;
public class GameLogger
{

	static string path = "Assets/Resources/SavedNetworks.txt";
	static string pathAvgFitness = "Assets/Resources/AverageFitness.txt";
	static string pathMedianFitness = "Assets/Resources/MedianFitness.txt";

	public static void WriteData(string str)
	{
		using (StreamWriter file = new StreamWriter(path, true))
		{
			file.WriteLine(str);
		}
	}

	public static void WriteAvgFitnessData(string str)
	{
		using (StreamWriter file = new StreamWriter(pathAvgFitness, true))
		{
			file.WriteLine(str);
		}
	}

	public static void WriteMedianFitnessData(string str)
	{
		using (StreamWriter file = new StreamWriter(pathMedianFitness, true))
		{
			file.WriteLine(str);
		}
	}

}
