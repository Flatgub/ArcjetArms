using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// EntityBase prepresents any entity which might exist on the hex grid, be it a player or an
/// enemy, all code and properties which are common among all entities are shared here.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class Entity : MonoBehaviour
{
    private HexGrid grid;
    private Hex position;
    public SpriteRenderer appearance;

    public void Awake()
    {
        appearance = GetComponent<SpriteRenderer>();
    }

    public void Initialize(HexGrid grid, Hex pos)
    {
        this.grid = grid;
        MoveTo(pos);
        grid.AddEntityToGrid(this);
        
    }

    public Hex GetPosition()
    {
        return position;
    }

    public void MoveTo(Hex pos)
    {
        if (pos == null)
        {
            throw new ArgumentNullException("Cannot move to null position");
        }
        position = pos;
        transform.position = grid.GetWorldPosition(pos);
    }

}
