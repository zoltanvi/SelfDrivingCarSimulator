using System;


public sealed class RandomHelper
{

	
	public const int DefaultSeed = 999;
	private static int m_seed = DefaultSeed;
	private static readonly object Padlock = new object();
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
			lock (Padlock)
			{
				m_seed = value;
				m_random = new Random(m_seed);
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
		lock (Padlock)
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
		lock (Padlock)
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
		lock (Padlock)
		{
			float randomNumber = (float)m_random.NextDouble() * (max - min) + min;
			return randomNumber;
		}
	}

	private static void CheckSeed()
	{
		if (m_random == null)
		{
			throw new Exception("You have to set the seed before use any random number!");
		}
	}

}

