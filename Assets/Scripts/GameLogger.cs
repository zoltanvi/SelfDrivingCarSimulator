using System.IO;

public class GameLogger
{
    private const string Path = "Assets/Resources/SavedNetworks.txt";
    private const string PathAvgFitness = "Assets/Resources/AverageFitness.txt";
    private const string PathMedianFitness = "Assets/Resources/MedianFitness.txt";

    public static void WriteData(string str)
    {
        using (StreamWriter file = new StreamWriter(Path, true))
        {
            file.WriteLine(str);
        }
    }

    public static void WriteAvgFitnessData(string str)
    {
        using (StreamWriter file = new StreamWriter(PathAvgFitness, true))
        {
            file.WriteLine(str);
        }
    }

    public static void WriteMedianFitnessData(string str)
    {
        using (StreamWriter file = new StreamWriter(PathMedianFitness, true))
        {
            file.WriteLine(str);
        }
    }
}

