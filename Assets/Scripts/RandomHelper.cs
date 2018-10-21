using System.Collections;
using System.Collections.Generic;
using System;

public sealed class RandomHelper
{

    private static int m_seed = 999;
    private static readonly object padlock = new object();
    private static Random m_random = null;

    static RandomHelper() { }

    public static int Seed
    {
        get
        {
            return m_seed;
        }

        set
        {
            m_seed = value;
            m_random = new Random(m_seed);
        }
    }

	/// <summary>
	///	Returns a non-negative random integer.
	/// </summary>
    public static int NextInt()
    {
		CheckSeed();
        lock (padlock)
        {
            int randomNumber = m_random.Next();
            return randomNumber;
        }
    }

	/// <summary>
	/// Returns a non-negative random integer that is less or equal than max [inclusive].
	/// </summary>
	public static int NextInt(int max)
    {
		CheckSeed();
        lock (padlock)
        {
            int randomNumber = m_random.Next(max + 1);
            return randomNumber;
        }
    }


	/// <summary>
	/// Returns a random integer between min [inclusive] and max [inclusive].
	/// </summary>
	public static int NextInt(int min, int max)
    {
		CheckSeed();
        lock (padlock)
        {
            int randomNumber = m_random.Next(min, max + 1);
            return randomNumber;
        }
    }

	/// <summary>
	/// Returns a random floating-point number between min [inclusive] and max [inclusive].
	/// </summary>
	public static float NextFloat(float min, float max)
    {
		CheckSeed();
        lock (padlock)
        {
            float randomNumber = (float)m_random.NextDouble() * (max - min) + min;
            return randomNumber;
        }
    }

	private static void CheckSeed()
	{
		if(m_random == null)
		{
			throw new Exception("You have to set the seed before use any random number!");
		}
	}



}
