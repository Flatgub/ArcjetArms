using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayContext 
{
    public Entity Player { get; private set; }
    public HexGrid Grid { get; private set; }
    public InterfaceManager Ui { get; private set; }

    public GameplayContext(Entity player, HexGrid grid, InterfaceManager ui)
    {
        this.Player = player;
        this.Grid = grid;
        this.Ui = ui;
    }
}
