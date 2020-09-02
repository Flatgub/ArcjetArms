using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GridHelper
{
    /// <summary>
    /// Starting at a location, get all the tiles in a line moving in the given direction up to the
    /// maximum length, or until an obstacle is hit.
    /// </summary>
    /// <param name="grid">the HexGrid to cast the line on</param>
    /// <param name="start">the hex location to start the line at</param>
    /// <param name="direction">the direciton to move in, such as <see cref="Hex.NorthEast"/></param>
    /// <param name="length">The max length of the line, not including the start location</param>
    /// <param name="stopAtObstructions">whether the line should stop at obstructions</param>
    /// <param name="includeStart">whether the starting location should be included in the list</param>
    /// <returns></returns>
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
            pos += direction;
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

    public static void RemoveOccupiedHexes(HexGrid grid, List<Hex> input)
    {
        for (int i = input.Count - 1; i >= 0; i--)
        {
            if(grid.IsHexOccupied(input[i])) 
            {
                input.RemoveAt(i);
            }
        }
    }
}
