using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// HexLayout is used to represent the set of transforms required to convert a point in worldspace
/// into hexagonal space and vice versa. 
/// </summary>
public class HexLayout
{
    // this layout represents a hex grid rotated such that the top of each hex is flat
    public static readonly OrientationTransform FlatTopLayout =
        new OrientationTransform(3.0 / 2.0, 0.0, Math.Sqrt(3.0) / 2.0, Math.Sqrt(3.0),
            2.0 / 3.0, 0.0, -1.0 / 3.0, Math.Sqrt(3.0) / 3.0);

    // this layout represents a hex grid rotated such that the top of each hex is pointy
    public static readonly OrientationTransform PointyTopLayout =
        new OrientationTransform(Math.Sqrt(3.0), Math.Sqrt(3.0) / 2.0, 0.0, 3.0 / 2.0,
            Math.Sqrt(3.0) / 3.0, -1.0 / 3.0, 0.0, 2.0 / 3.0);

    public double size;
    public Vector2 origin;
    public OrientationTransform orientation;

    /// <summary>
    /// Construct a HexLayout  
    /// </summary>
    /// <param name="orientation">Which orientation the hex grid is in, FlatTop or Pointy</param>
    /// <param name="size">How large the hexes are in this grid</param>
    /// <param name="origin">Where the center of the grid is measured from</param>
    public HexLayout(OrientationTransform orientation, double size, Vector2 origin)
    {
        this.size = size;
        this.origin = origin;
        this.orientation = orientation;
    }

    /// <summary>
    /// Convert a hexagonal coordinate to a 2D worldspace coordinate
    /// </summary>
    /// <param name="h"></param>
    /// <returns>A Vector2 of the 2D coordinates at the center of the provided hex</returns>
    public Vector2 HexToWorld(Hex h)
    {
        float x = (float)((orientation.f0 * h.Q + orientation.f1 * h.R) * size) + origin.x;
        float y = (float)((orientation.f2 * h.Q + orientation.f3 * h.R) * size) + origin.y;
        return new Vector2(x, y);
    }

    /// <summary>
    /// Convert a 2D worldspace coordinate into its matching hexagonal coordinate. 
    /// </summary>
    /// <remarks>World coordinates don't align perfectly to hex coodinates, so the result of this
    /// function must be rounded to obtain a proper <c>Hex</c> using 
    /// <see cref="FractionalHex.RoundToHex"/></remarks>
    /// <param name="v">a 2D position in worldspace</param>
    /// <returns>A FractionalHex, representing the given position</returns>
    public FractionalHex WorldToHex(Vector2 v)
    {
        float x = (float)((v.x - origin.x) / size);
        float y = (float)((v.y - origin.y) / size);
        double q = orientation.b0 * x + orientation.b1 * y;
        double r = orientation.b2 * x + orientation.b3 * y;
        return new FractionalHex(q, r, -q - r);
    }
    
}

/// <summary>
/// Wrapper class for the 2x2 matrix that represents the orientation of the hex grid
/// </summary>
public class OrientationTransform
{
    public readonly double f0, f1, f2, f3; //forward 2x2 transform matrix
    public readonly double b0, b1, b2, b3; //inverse 2x2 transform matrix

    public OrientationTransform(double f0, double f1, double f2, double f3,
        double b0, double b1, double b2, double b3)
    {
        this.f0 = f0;
        this.f1 = f1;
        this.f2 = f2;
        this.f3 = f3;
        this.b0 = b0;
        this.b1 = b1;
        this.b2 = b2;
        this.b3 = b3;
    }
}