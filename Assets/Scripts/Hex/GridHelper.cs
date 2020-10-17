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
    /// <param name="includeHit">Whether the list should include the hit obstruction, if applicable</param>
    public static List<Hex> CastLineInDirection(HexGrid grid, Hex start, Hex direction, int length,
        bool stopAtObstructions = true, bool includeStart = true, bool includeHit = false)
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
                if (includeHit)
                {
                    output.Add(pos);
                }
                break;
            }
        }

        return output;
    }

    /// <summary>
    /// Find all hexes within range of the given hex, either using range as "walking distance" or
    /// straight line distance
    /// </summary>
    /// <param name="grid">The HexGrid to search on</param>
    /// <param name="start">The starting location of the search</param>
    /// <param name="range">The maximum distance a hex can be from the starting position</param>
    /// <param name="stopAtObstructions">whether the search should continue through obstructions
    /// </param>
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

    /// <summary>
    /// Find all entities within range of a given hex
    /// </summary>
    /// <param name="grid">The HexGrid to search on</param>
    /// <param name="start">The starting location of the search</param>
    /// <param name="range">The maximum distance an entity can be from the starting position</param>
    public static List<Entity> GetEntitiesInRange(HexGrid grid, Hex start, int range)
    {
        List<Hex> candidates = GetHexesInRange(grid, start, range, stopAtObstructions: false);
        List<Entity> ents = grid.GetAllEntities();
        List<Entity> output = new List<Entity>();
        foreach (Entity e in ents)
        {
            if (candidates.Contains(e.Position))
            {
                output.Add(e);
            }
        }
        return output;
    }

    /// <summary>
    /// Find the path
    /// </summary>
    /// <param name="grid">The HexGrid to path on</param>
    /// <param name="start">The starting location of the path</param>
    /// <param name="end">The desired destination of the path</param>
    /// <param name="maxLength">The max length the path is allowed to be</param>
    /// <param name="stopAtObstructions">whether the path should go around obstructions</param>
    /// <param name="ignoreEnd">Whether the end should always be considered unobstructed</param>
    /// <returns>A list of hexes that represents the path, or null if no path exists</returns>
    public static List<Hex> GetPathToHex(HexGrid grid, Hex start, Hex end,
        int maxLength = int.MaxValue, bool stopAtObstructions = true, bool ignoreEnd = true)
    {
        Dictionary<Hex, int> seenHexes = new Dictionary<Hex, int>(); //pairs are <Hex, Distance>
        Dictionary<Hex, Hex> trace = new Dictionary<Hex, Hex>(); //pairs are <Node, Parent>
        Queue<Hex> frontier = new Queue<Hex>();

        frontier.Enqueue(start);
        seenHexes.Add(start, 0);
        bool success = false;

        while (frontier.Count != 0)
        {
            Hex cell = frontier.Dequeue();

            int distance = seenHexes[cell];

            List<Hex> edges = cell.GetAllNeighbours();
            foreach (Hex e in edges)
            {
                //don't explore out of bounds
                if (!grid.Contains(e))
                {
                    continue;
                }

                //don't explore into obstructions (unless we're allowed to)
                if (stopAtObstructions && (!ignoreEnd || e != end) && grid.IsHexOccupied(e))
                {
                    continue;
                }

                if (distance + 1 <= maxLength && !seenHexes.ContainsKey(e))
                {
                    frontier.Enqueue(e);
                    seenHexes.Add(e, distance + 1);
                    trace.Add(e, cell);
                    if (e == end)
                    {
                        success = true;
                        break;
                    }
                }
            }
            if (success)
            {
                break;
            }
        }

        if (success)
        {
            List<Hex> output = new List<Hex>();
            Hex backtrack = end;
            do {
                output.Add(backtrack);
                backtrack = trace[backtrack];
            } while (backtrack != start);

            output.Reverse();

            return output;
        }
        else
        {
            Debug.LogWarning("failed to find path from " + start + " to " + end);
            return null;
        }
    }

    /// <summary>
    /// Removes all the hexes from a list of hexes that are occupied, leaving only free spaces
    /// </summary>
    /// <param name="grid">The grid to work with</param>
    /// <param name="input">The list to work on</param>
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

    /// <summary>
    /// Find the nearest hex to the input position given a list of other hexes
    /// </summary>
    /// <param name="list">The list </param>
    /// <param name="pos"></param>
    /// <returns>The hex from list that was closest to pos</returns>
    public static Hex GetNearestHexInList(List<Hex> list, Hex pos)
    {
        if (list.Count == 0)
        {
            return null;
        }

        Hex nearest = list[0];
        int dist = nearest.DistanceTo(pos);

        foreach (Hex h in list)
        {
            int newDist = h.DistanceTo(pos);
            if (newDist < dist)
            {
                dist = newDist;
                nearest = h;
            }
        }

        return nearest;
    }

    /// <summary>
    /// Get all of the entities that exist in a given set of hexes
    /// </summary>
    /// <param name="grid">The grid to work with</param>
    /// <param name="list">The list of hexes to check for entities</param>
    /// <returns>A list of the entities that were found in those positions</returns>
    public static List<Entity> GetEntitiesFromPositions(HexGrid grid, List<Hex> list)
    {
        List<Entity> output = new List<Entity>();
        foreach (Hex h in list)
        {
            if (grid.GetEntityAtHex(h) is Entity ent)
            {
                output.Add(ent);
            }
        }
        return output;
    }

}
