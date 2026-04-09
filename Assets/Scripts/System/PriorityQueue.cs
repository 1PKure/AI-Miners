using System.Collections.Generic;

public class PriorityQueue<T>
{
    private readonly List<(T Item, float Priority)> elements = new List<(T, float)>();

    public int Count => elements.Count;

    public void Enqueue(T item, float priority)
    {
        elements.Add((item, priority));
    }

    public T Dequeue()
    {
        int bestIndex = 0;

        for (int i = 1; i < elements.Count; i++)
        {
            if (elements[i].Priority < elements[bestIndex].Priority)
                bestIndex = i;
        }

        T bestItem = elements[bestIndex].Item;
        elements.RemoveAt(bestIndex);
        return bestItem;
    }

    public bool IsEmpty()
    {
        return elements.Count == 0;
    }
}