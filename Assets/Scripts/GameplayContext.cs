﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayContext 
{
    public GameManager Manager { get; private set; }
    public Entity Player { get; private set; }
    public HexGrid Grid { get; private set; }
    public InterfaceManager Ui { get; private set; }

    public GameplayContext(GameManager gm, Entity player, HexGrid grid, InterfaceManager ui)
    {
        this.Manager = gm;
        this.Player = player;
        this.Grid = grid;
        this.Ui = ui;
    }

    public Coroutine StartCoroutineOnManager(IEnumerator coroutine)
    {
        return Manager.StartCoroutine(coroutine);
    }
}
