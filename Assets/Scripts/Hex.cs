using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.Jobs.LowLevel.Unsafe;
using UnityEditor.UIElements;
using UnityEngine.EventSystems;

/// <summary>
/// Hex represents a point in 3 axis hexagonal cube coordinate space.
/// This class contains various helper methods to make working in hexagonal easier
/// </summary>
/// 
/// See https://www.redblobgames.com/grids/hexagons/ for more information on this implementation
/// of hexagonal coordinate systems. (the implementation here is based on theirs)
public class Hex
{
    // Cube coordinates use 3 axes which we refer to as q, r and s, but to ensure the q + r + s = 0
    // restriction we encapsulate the axis so that they're readonly outside of this class.
    public int Q { get; private set; }
    public int R { get; private set; }
    public int S { get; private set; }

    // constants for directions
    public static readonly Hex NorthEast = new Hex(1, 0, -1);
    public static readonly Hex NorthWest = new Hex(0, 1, -1);
    public static readonly Hex East = new Hex(1, -1, 0);
    public static readonly Hex West = new Hex(-1, 1, 0);
    public static readonly Hex SouthEast = new Hex(0, -1, 1);
    public static readonly Hex SouthWest = new Hex(-1, 0, 1);
    public static readonly Hex[] Directions = {NorthEast, East, SouthEast,
        SouthWest, West, NorthWest};

    /// <summary>Construct a hex from cube coordinates</summary>
    /// <remarks>Cube coordinates must conform to the restriction "q + r + s == 0" </remarks>
    public Hex(int q, int r, int s)
    {
        if (q + r + s != 0)
        {
            throw new ArgumentException(String.Format("q + r + s doesn't equal 0, (got {0},{1},{2})", q, r, s));
        }

        this.Q = q;
        this.R = r;
        this.S = s;
    }

    /// <summary>Construct a hex from axial coodinates</summary>
    /// <remarks>Cube coordinate s can be derived from only q and r</remarks>
    public Hex(int q, int r)
    {
        this.Q = q;
        this.R = r;
        this.S = -q - r;
    }

    /// <summary> Get the length of the vector to this coordinate</summary>
    public int Length()
    {
        return (Math.Abs(Q) + Math.Abs(R) + Math.Abs(S)) / 2;
    }

    /// <summary>Get the distance between this hex and another</summary>
    public int DistanceTo(Hex other)
    {
        Hex difference = this - other;
        return difference.Length();
    }

    /// <summary>
    /// Get the hexagon neighbouring this one in the given direction.
    /// </summary>
    /// <param name="dir">Which direction to use (0 to 5 for NorthEast to SouthWest)</param>
    public Hex GetNeighbour(int dir)
    {
        if (dir < 0 || dir > 5)
        {
            throw new ArgumentOutOfRangeException(String.Format("dir must be between 0 and 5, got " + dir));
        }
        return this + Directions[dir];
    }

    /// <summary>
    /// Get all neighbouring hexes surrounding this one.
    /// </summary>
    /// <returns>A list of 6 hexes</returns>
    public List<Hex> GetAllNeighbours()
    {
        List<Hex> neighbours = new List<Hex>();
        for (int i = 0; i <= 5; i++)
        {
            neighbours.Add(GetNeighbour(i));
        }
        return neighbours;
    }

    /// <summary>
    /// Rotate the coordinate counterclockwise around 0,0,0
    /// </summary>
    public void RotateLeft()
    {
        int tempQ = Q;
        int tempR = R;
        Q = -S;
        R = -tempQ;
        S = -tempR;
    }

    /// <summary>
    /// Rotate the coordinate clockwise around 0,0,0
    /// </summary>
    public void RotateRight()
    {
        int tempQ = Q;
        Q = -R;
        R = -S;
        S = -tempQ;
    }

    // default implementation
    public override bool Equals(object obj)
    {
        return obj is Hex hex &&
               Q == hex.Q &&
               R == hex.R &&
               S == hex.S;
    }

    // this implementation for GetHashCode begins to have collisions for sufficiently large
    // coordinates, but for our purposes its fine, we'll never have coordinates larger than this.
    public override int GetHashCode()
    {
        return R * 31 + (Q * 491);
    }

    // operator overrides for ease of use
    public static Hex operator +(Hex h) => h;
    public static Hex operator -(Hex h) => new Hex(-h.Q, -h.R, -h.S);
    public static Hex operator +(Hex a, Hex b) => new Hex(a.Q + b.Q, a.R + b.R, a.S + b.S);
    public static Hex operator -(Hex a, Hex b) => new Hex(a.Q - b.Q, a.R - b.R, a.S - b.S);
    public static Hex operator *(Hex h, int k) => new Hex(h.Q * k, h.R * k, h.S * k);
    public static bool operator ==(Hex a, Hex b) => a is Hex && b is Hex && a.Q == b.Q &&
                                                     a.R == b.R && a.S == b.S;
    public static bool operator !=(Hex a, Hex b) => !(a == b);
}

/// <summary>
/// Fractional hex represents a hexagonal cube coordinate that isn't limited to the grid
/// only for use in a few specific calculations
/// </summary>
public class FractionalHex
{
    public double Q { get; private set; }
    public double R { get; private set; }
    public double S { get; private set; }

    public FractionalHex(double q, double r, double s)
    {
        Q = r;
        R = r;
        S = s;
    }

    /// <summary>
    /// Round a fractional hex into whole number hex
    /// </summary>
    /// <returns>The closest <c>Hex</c> to this fractional hex</returns>
    public Hex RoundToHex()
    {
        int roundQ = (int)Math.Round(Q, MidpointRounding.AwayFromZero);
        int roundR = (int)Math.Round(R, MidpointRounding.AwayFromZero);
        int roundS = (int)Math.Round(S, MidpointRounding.AwayFromZero);
        double fracQ = Math.Abs(roundQ - Q);
        double fracR = Math.Abs(roundR - R);
        double fracS = Math.Abs(roundS - S);
        if (fracQ > fracR && fracQ > fracS)
        {
            roundQ = -roundR - roundS;
        }
        else if (fracR > fracS)
        {
            roundR = -roundQ - roundS;
        }
        else
        {
            roundS = -roundQ - roundR;
        }
        return new Hex(roundQ, roundR, roundS);
    }
}