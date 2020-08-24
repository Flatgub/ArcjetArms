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
    private SpriteRenderer appearance;

    public void Initialize(HexGrid grid, Hex pos)
    {
        this.grid = grid;
        this.position = pos;
    }

    public void Awake()
    {
        appearance = GetComponent<SpriteRenderer>();
    }

    public Hex GetPosition()
    {
        return position;
    }

    public void MoveTo(Hex h)
    {
        position = h;
        transform.position = grid.GetWorldPosition(h);
    }

}
