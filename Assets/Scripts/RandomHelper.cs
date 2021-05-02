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

using System;

public sealed class RandomHelper
{
    public const int DefaultSeed = 999;
    private static int s_Seed = DefaultSeed;
    private static readonly object Padlock = new object();
    private static Random s_Random;

    static RandomHelper() { }

    public static int Seed
    {
        get
        {
            return s_Seed;
        }

        set
        {
            lock (Padlock)
            {
                s_Seed = value;
                s_Random = new Random(s_Seed);
            }
        }
    }

    /// <summary>
    /// Generate a new seed for the RandomHelper
    /// </summary>
    public static void GenerateNewSeed()
    {
        Seed = Guid.NewGuid().GetHashCode();
    }

    /// <summary>
    ///	Returns a non-negative random integer.
    /// </summary>
    public static int NextInt()
    {
        CheckSeed();
        lock (Padlock)
        {
            int randomNumber = s_Random.Next();
            return randomNumber;
        }
    }

    /// <summary>
    /// Returns a non-negative random integer that is less or equal than max [inclusive].
    /// </summary>
    public static int NextInt(int max)
    {
        CheckSeed();
        lock (Padlock)
        {
            int randomNumber = s_Random.Next(max + 1);
            return randomNumber;
        }
    }

    /// <summary>
    /// Returns a random integer between min [inclusive] and max [inclusive].
    /// </summary>
    public static int NextInt(int min, int max)
    {
        CheckSeed();
        lock (Padlock)
        {
            int randomNumber = s_Random.Next(min, max + 1);
            return randomNumber;
        }
    }

    /// <summary>
    /// Returns a random floating-point number between min [inclusive] and max [inclusive].
    /// </summary>
    public static float NextFloat(float min, float max)
    {
        CheckSeed();
        lock (Padlock)
        {
            float randomNumber = (float)s_Random.NextDouble() * (max - min) + min;
            return randomNumber;
        }
    }

    private static void CheckSeed()
    {
        if (s_Random == null)
        {
            throw new Exception("You have to set the seed before use any random number!");
        }
    }
}

