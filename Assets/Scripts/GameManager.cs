﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int mapRadius;

    public HexGrid worldGrid;
    private EntityFactory factory;
    private Entity player;
    public InterfaceManager interfaceManager;

    // Start is called before the first frame update
    void Start()
    {
        if (worldGrid == null)
        {
            worldGrid = GameObject.FindObjectOfType<HexGrid>();
        }
        worldGrid.GenerateMap(mapRadius);
        factory = EntityFactory.GetFactory;
        player = factory.CreateEntity(worldGrid, new Hex(0, 0));
        interfaceManager.SelectFromCandidates(player.GetPosition().GetAllNeighbours()
            .FindAll((_) => !worldGrid.IsHexOccupied(_)));

        interfaceManager.OnSelectionMade += (pos) =>
        {
            player.MoveTo(pos);
            interfaceManager.SelectFromCandidates(player.GetPosition().GetAllNeighbours()
                .FindAll((_) => !worldGrid.IsHexOccupied(_)));
        };
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Hex mouseHex = worldGrid.GetHexUnderMouse();
            if (!(mouseHex is null))
            {
                if (!worldGrid.IsHexOccupied(mouseHex))
                {
                    player.MoveTo(mouseHex);
                }
            }
        }

    }
}
