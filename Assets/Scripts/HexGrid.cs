using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

/// <summary>
/// HexGrid is used to handle things
/// </summary>
public class HexGrid : MonoBehaviour
{

    public double size;
    private HexLayout layout;
    private HashSet<Hex> map;

    public void Start()
    {
        layout = new HexLayout(HexLayout.PointyTopLayout, size, transform.position);
        map = new HashSet<Hex>();
        int map_radius = 2;
        for (int q = -map_radius; q <= map_radius; q++) {
            int r1 = Math.Max(-map_radius, -q - map_radius);
            int r2 = Math.Min(map_radius, -q + map_radius);
            for (int r = r1; r <= r2; r++)
            {
                var h = new Hex(q, r);
                if (!map.Contains(h))
                {
                    map.Add(h);
                } 
            }
        }
    }

    public void Update()
    {
        Vector2 mousepos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //Debug.Log(layout.WorldToHex(mousepos).RoundToHex());   
    }

    public void OnDrawGizmos()
    {

        if (!(layout is HexLayout))
        {
            return;
        }

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, 1);

        
        foreach (Hex h in map)
        {
            Vector2 pos = layout.HexToWorld(h);
            if (h == Hex.NorthEast || h == Hex.SouthWest)
            {
                Gizmos.color = Color.green;
            }
            else if (h == Hex.NorthWest || h == Hex.SouthEast)
            {
                Gizmos.color = Color.yellow;
            }
            else
            {
                Gizmos.color = Color.red;
            }
            Gizmos.DrawWireSphere(new Vector3(pos.x, pos.y, 0), 0.5f);
        }
    }





}




