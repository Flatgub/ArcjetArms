using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayContext 
{
    public Entity player { get; private set; }
    public HexGrid grid { get; private set; }
    public InterfaceManager ui { get; private set; }

    public GameplayContext(Entity player, HexGrid grid, InterfaceManager ui)
    {
        this.player = player;
        this.grid = grid;
        this.ui = ui;
    }
}
