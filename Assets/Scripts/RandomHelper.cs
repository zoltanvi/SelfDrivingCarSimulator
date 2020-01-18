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

