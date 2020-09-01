using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GridHelper
{
    public static List<Hex> CastLineInDirection(HexGrid grid, Hex start, Hex direction, int length,
        bool stopAtObstructions = true, bool includeStart = true)
    {
        List<Hex> output = new List<Hex>();

        if (includeStart)
        {
            output.Add(start);
        }

        Hex pos = start;
        for (int i = 0; i < length; i++)
        {
            pos = pos + direction;
            if (!grid.IsHexOccupied(pos) || !stopAtObstructions)
            {
                output.Add(pos);
            }
            else
            {
                break;
            }
        }

        return output;
    }
}
