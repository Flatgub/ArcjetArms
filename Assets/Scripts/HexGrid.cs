﻿using System;
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

    public Text text;
    
    Camera mainCamera;

    public void Awake()
    {
        mainCamera = Camera.main;
        hexSprite = Resources.Load<Sprite>("Sprites/HexagonPointy");

        layout = new HexLayout(OrientationTransform.PointyTopLayout, size, transform.position);
        allHexes = new HashSet<Hex>();        
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
    /// <param name="h">The <c>Hex</c> coordinate to spawn a sprite at</param>
    private void SpawnHexSprite(Hex h)
    {
        Vector2 hexPos = layout.HexToWorld(h);
        GameObject hex = new GameObject("HexSprite");
        hex.transform.parent = transform;
        hex.transform.position = hexPos;
        SpriteRenderer spr = hex.AddComponent<SpriteRenderer>();
        spr.sprite = hexSprite;
    }

    /// <summary>
    /// Check whether a given Hex location is within the grid
    /// </summary>
    /// <param name="h">the location to check</param>
    public bool ContainsHex(Hex h)
    {
        return allHexes.Contains(h);
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
        if (ContainsHex(outhex))
        {
            return outhex;
        }
        else
        {
            return null;
        } 
    }

    public void AddEntityToGrid(Entity ent)
    {
        Vector2 worldpos = layout.HexToWorld(ent.GetPosition());
        ent.transform.position = worldpos;
    }

    public Vector2 GetWorldPosition(Hex pos)
    {
        return layout.HexToWorld(pos);
    }

    /*
    public void Update()
    {
        Hex hexAtMouse = GetHexUnderMouse();
        if (hexAtMouse is Hex)
        {
            text.text = hexAtMouse.ToString();
        }*/
   
}










