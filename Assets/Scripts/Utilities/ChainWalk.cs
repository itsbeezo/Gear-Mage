using System;
using System.Collections.Generic;

// Finds every node connected to a start node through neighbor links (the gear chain). Plain C#, so
// EditMode tests can use it with ints. Breadth-first, each node once, so cycles are safe.
public static class ChainWalk
{
    // Clears `into`, then fills it with start and everything reachable from it, nearest first.
    // Nodes for which isAlive is false (destroyed gears) are skipped and not walked through.
    public static void Collect<T>(T start, Func<T, IReadOnlyList<T>> neighborsOf, Func<T, bool> isAlive, List<T> into)
        where T : class
    {
        into.Clear();
        if (start == null || !isAlive(start)) return;

        HashSet<T> seen = new HashSet<T> { start };
        into.Add(start);

        for (int i = 0; i < into.Count; i++)
        {
            IReadOnlyList<T> neighbors = neighborsOf(into[i]);
            for (int n = 0; n < neighbors.Count; n++)
            {
                T neighbor = neighbors[n];
                if (isAlive(neighbor) && seen.Add(neighbor)) into.Add(neighbor);
            }
        }
    }
}
