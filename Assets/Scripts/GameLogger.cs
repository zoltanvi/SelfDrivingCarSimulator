using System.IO;
public class GameLogger
{

	static string path = "Assets/Resources/SavedNetworks.txt";

	public static void WriteData(string str)
	{
		using (StreamWriter file = new StreamWriter(path, true))
		{
			file.WriteLine(str);
		}
	}

}
