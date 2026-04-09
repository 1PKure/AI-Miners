using System.Collections.Generic;

public class DoubleEntryTable<R, C, V>
{
    private Dictionary<R, Dictionary<C, V>> table = new Dictionary<R, Dictionary<C, V>>();

    public void Add(R row, C column, V value)
    {
        if (!table.ContainsKey(row))
        {
            table[row] = new Dictionary<C, V>();
        }

        table[row][column] = value;
    }

    public bool TryGetValue(R row, C column, out V value)
    {
        value = default;

        if (table.TryGetValue(row, out var innerTable))
        {
            if (innerTable.TryGetValue(column, out value))
            {
                return true;
            }
        }

        return false;
    }
}