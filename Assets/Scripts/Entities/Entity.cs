using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// EntityBase prepresents any entity which might exist on the hex grid, be it a player or an
/// enemy, all code and properties which are common among all entities are shared here.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(HealthComponent))]
public class Entity : MonoBehaviour
{
    public string entityName = "Unnamed";

    private HexGrid grid;
    private Hex position;
    public SpriteRenderer appearance;

    public HealthComponent Health { get;
        private set;
    }

    public void Initialize()
    {
        appearance = GetComponent<SpriteRenderer>();
        Health = GetComponent<HealthComponent>();
    }

    public void AddToGrid(HexGrid grid, Hex pos)
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
        //transform.position = grid.GetWorldPosition(pos);
        //TODO: Calculate travel time using speed somehow?
        LeanTween.moveLocal(gameObject, grid.GetWorldPosition(pos), 0.1f);
    }

}
