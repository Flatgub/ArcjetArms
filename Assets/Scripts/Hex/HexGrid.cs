using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// HexGrid is used to handle things
/// </summary>
public class HexGrid : MonoBehaviour
{

    public double size;
    private HexLayout layout;
    private Sprite hexSprite;

    private HashSet<Hex> allHexes;
    private List<Entity> allEntities;
    
    Camera mainCamera; //fixme: maybe don't store this here?

    public void Awake()
    {
        mainCamera = Camera.main;
        hexSprite = Resources.Load<Sprite>("Sprites/HexagonPointy");

        layout = new HexLayout(OrientationTransform.PointyTopLayout, size, transform.position);
        allHexes = new HashSet<Hex>();
        allEntities = new List<Entity>();
    }

    /// <summary>
    /// Generate a circular Hex grid of a given radius
    /// </summary>
    /// <param name="radius"></param>
    /// <returns><c>true</c> if the map was successfully created, or <c>false</c> if a map
    /// already existed</returns>
    public bool GenerateMap(int radius)
    {
        if (allHexes.Count > 0)
        {
            return false; // cannot generate a map when a map already exists
        }

        for (int q = -radius; q <= radius; q++)
        {
            int r1 = Math.Max(-radius, -q - radius);
            int r2 = Math.Min(radius, -q + radius);
            for (int r = r1; r <= r2; r++)
            {
                var h = new Hex(q, r);
                if (!allHexes.Contains(h))
                {
                    allHexes.Add(h);
                    SpawnHexSprite(h);
                }
            }
        }

        return true;
    }

    /// <summary>
    /// Create a <c>GameObject</c> and <c>SpriteRenderer</c> to visualize a given hex location in
    /// the scene
    /// </summary>
    /// <param name="pos">The <c>Hex</c> coordinate to spawn a sprite at</param>
    private void SpawnHexSprite(Hex pos)
    {
        Vector2 hexPos = layout.HexToWorld(pos);
        GameObject hex = new GameObject("HexSprite");
        hex.transform.parent = transform;
        hex.transform.position = hexPos;
        SpriteRenderer spr = hex.AddComponent<SpriteRenderer>();
        spr.sprite = hexSprite;
    }

    /// <summary>
    /// Check whether a given Hex location is within the grid
    /// </summary>
    /// <param name="pos">the location to check</param>
    public bool Contains(Hex pos)
    {
        return allHexes.Contains(pos);
    }

    /// <summary>
    /// Get the Hex tile under the mouse if the mouse is currently over the grid.
    /// </summary>
    /// <returns>A <c>Hex</c> coordinate if the mouse is currently over the grid,
    /// otherwise <c>null</c> if the mouse is out of bounds</returns>
    public Hex GetHexUnderMouse()
    {
        Vector2 mousepos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Hex outhex = layout.WorldToHex(mousepos).RoundToHex();
        if (Contains(outhex))
        {
            return outhex;
        }
        else
        {
            return null;
        } 
    }

    public bool IsHexOccupied(Hex pos)
    {
        return !(GetEntityAtHex(pos) is null) || !Contains(pos);
    }

    public void AddEntityToGrid(Entity ent)
    {
        allEntities.Add(ent);
        ent.MoveTo(ent.GetPosition());
    }

    //TODO: this method is only performs half the transaction which is dangerous, change it soon
    /// <summary>
    /// Remove the given entity from this grid.
    /// </summary>
    /// <remarks>Note: This does not change what grid the entity think's its on, only the grid is
    /// affected by this removal. After calling <c>RemoveEntityFromGrid</c> you must also change
    /// the entity itself to tell it what grid to be on.
    /// <param name="ent"></param>
    public void RemoveEntityFromGrid(Entity ent)
    {
        allEntities.Remove(ent);
    }

    /// <summary>
    /// Get the entity at the current position, if one exists
    /// </summary>
    /// <param name="pos">The position to check</param>
    /// <returns>Either the <see cref="Entity"/> at <c>pos</c> if one exists, or <c>null</c> if
    /// there is no entity there. </returns>
    public Entity GetEntityAtHex(Hex pos)
    {
        foreach (Entity e in allEntities)
        {
            if (e.GetPosition() == pos)
            {
                return e;
            }
        }
        return null;
    }

    /// <summary>
    /// Get a list of all the entities currently on the grid.
    /// </summary>
    public List<Entity> GetAllEntities()
    {
        return new List<Entity>(allEntities); //return a shallow copy
    }

    /// <summary>
    /// Get the worldspace position of a <c>Hex</c> position
    /// </summary>
    /// <param name="pos">The <c>Hex</c> coordinate to convert</param>
    /// <returns>A Vector2 for the center of <c>pos</c> in worldspace</returns>
    public Vector2 GetWorldPosition(Hex pos)
    {
        return layout.HexToWorld(pos);
    }


   
}










