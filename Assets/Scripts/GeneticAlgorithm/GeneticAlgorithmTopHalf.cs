using UnityEngine;
using System.Text;

public class GeneticAlgorithmTopHalf : GeneticAlgorithm
{
	private int[] topHalfID;

	protected override void Selection()
	{
		int r = 0;
		topHalfID = new int[PopulationSize / 2];

		for (int i = 0; i < topHalfID.Length; i++)
		{
			topHalfID[i] = Stats[i].Id;
		}

		for (int i = 0; i < PopulationSize; i++)
		{
			// A top 50%-ból kirandomol egyet, ő lesz a bal szülő
			// TODO: modified
			// r = Random.Range(0, topHalfID.Length);
			r = RandomHelper.NextInt(0, topHalfID.Length - 1);
			carPairs[i][0] = topHalfID[r];

			do
			{
				// A top 50%-ból kirandomol egyet, ő lesz a jobb szülő
				// Ha megegyezik a bal szülővel, újat randomol.
				// TODO: modified
				// r = Random.Range(0, topHalfID.Length);
				r = RandomHelper.NextInt(0, topHalfID.Length - 1);
				carPairs[i][1] = topHalfID[r];
			} while (carPairs[i][0] == carPairs[i][1]);
		}

#if UNITY_EDITOR
		StringBuilder sb = new StringBuilder();
		for (int i = 0; i < carPairs.Length; i++)
		{
			sb.Append(string.Format("{0} :: {1} \n", carPairs[i][0], carPairs[i][1]));
		}
		Debug.Log(sb.ToString());
#endif
	}
}
