using UnityEngine;
using System.Text;

public class GeneticAlgorithmTopHalf : GeneticAlgorithm
{
    private int[] m_TopHalfId;

    protected override void Selection()
    {
        m_TopHalfId = new int[PopulationSize / 2];

        for (int i = 0; i < m_TopHalfId.Length; i++)
        {
            m_TopHalfId[i] = FitnessRecords[i].Id;
        }

        for (int i = 0; i < PopulationSize; i++)
        {
            // A top 50%-ból kirandomol egyet, ő lesz a bal szülő
            int random = RandomHelper.NextInt(0, m_TopHalfId.Length - 1);
            CarPairs[i][0] = m_TopHalfId[random];

            do
            {
                // A top 50%-ból kirandomol egyet, ő lesz a jobb szülő
                // Ha megegyezik a bal szülővel, újat randomol.
                random = RandomHelper.NextInt(0, m_TopHalfId.Length - 1);
                CarPairs[i][1] = m_TopHalfId[random];
            } while (CarPairs[i][0] == CarPairs[i][1]);
        }

#if UNITY_EDITOR
        StringBuilder sb = new StringBuilder();
        foreach (var carPair in CarPairs)
        {
            sb.Append($"{carPair[0]} :: {carPair[1]} \n");
        }
        Debug.Log(sb.ToString());
#endif
    }
}
