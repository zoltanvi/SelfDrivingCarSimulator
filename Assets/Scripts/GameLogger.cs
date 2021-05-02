/*
Copyright (C) 2021 zoltanvi

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

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

