using System;
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

    public static List<Hex> GetHexesInRange(HexGrid grid, Hex start, int range,
        bool stopAtObstructions = true)
    {
        Dictionary<Hex, int> seenHexes = new Dictionary<Hex, int>(); //pairs are <Hex, Distance>
        Queue<Hex> frontier = new Queue<Hex>();
        List<Hex> output = new List<Hex>();

        frontier.Enqueue(start);
        seenHexes.Add(start, 0);

        while (frontier.Count != 0)
        {
            Hex cell = frontier.Dequeue();
            int distance = seenHexes[cell];

            output.Add(cell);

            List<Hex> edges = cell.GetAllNeighbours();
            foreach(Hex e in edges)
            {
                if (!grid.Contains(e) || (stopAtObstructions && grid.IsHexOccupied(e)))
                {
                    continue;
                }

                if (distance + 1 <= range && !seenHexes.ContainsKey(e))
                {
                    frontier.Enqueue(e);
                    seenHexes.Add(e, distance + 1);
                }
            }
        }

        return output;
    }

    public static List<Entity> GetEntitiesInRange(HexGrid grid, Hex start, int range)
    {
        throw new NotImplementedException();
    }

    public static List<Hex> GetPathToHex(HexGrid grid, Hex start, Hex end,
        int maxLength = int.MaxValue, bool stopAtObstructions = true)
    {
        throw new NotImplementedException();
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

    public static List<Entity> GetAdjacentEntities(HexGrid grid, Hex pos)
    {
        List<Hex> neighbours = pos.GetAllNeighbours();
        List<Entity> output = new List<Entity>();

        foreach (Hex n in neighbours)
        {
            Entity ent = grid.GetEntityAtHex(n);
            if (ent is Entity)
            {
                output.Add(ent);
            }
            
        }

        return output;
    }
}
