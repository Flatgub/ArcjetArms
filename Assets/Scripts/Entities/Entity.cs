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
    public Hex Position { get; private set; }
    public SpriteRenderer appearance;

    public HealthComponent Health { get;
        private set;
    }

    public void Initialize()
    {
        appearance = GetComponent<SpriteRenderer>();
        Health = GetComponent<HealthComponent>();
        Health.OnDeath += Die;
    }

    public void AddToGrid(HexGrid grid, Hex pos)
    {
        this.grid = grid;
        MoveTo(pos);
        grid.AddEntityToGrid(this);
    }

    public void MoveTo(Hex pos)
    {
        if (pos == null)
        {
            throw new ArgumentNullException("Cannot move to null position");
        }
        Position = pos;
        //transform.position = grid.GetWorldPosition(pos);
        //TODO: Calculate travel time using speed somehow?
        LeanTween.moveLocal(gameObject, grid.GetWorldPosition(pos), 0.1f);
    }

    public void ReceiveDamage(Entity attacker, int damage)
    {
        Debug.Log("i'm been attacked by " + attacker.entityName);
        Health.ApplyDamage(damage);
    }

    public void DealDamageTo(Entity victim, int damage)
    {
        Debug.Log("i'm attacking " + victim.entityName);
        victim.ReceiveDamage(this, damage);
    }

    public void Die()
    {
        Debug.Log("i'm dead");
    }

}
