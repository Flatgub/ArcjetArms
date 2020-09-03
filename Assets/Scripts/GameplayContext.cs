﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayContext 
{
    public GameManager Manager { get; private set; }
    public Entity Player { get; private set; }
    public HexGrid Grid { get; private set; }
    public InterfaceManager Ui { get; private set; }
    public Entity ActiveEntity { get; set; }
    public Card ActiveCard { get; set;}

    public GameplayContext(GameManager gm, Entity player, HexGrid grid, InterfaceManager ui,
        Entity active = null, Card card = null)
    {
        Manager = gm;
        Player = player;
        Grid = grid;
        Ui = ui;
        ActiveEntity = active;
        ActiveCard = card;
    }

    public Coroutine StartCoroutineOnManager(IEnumerator coroutine)
    {
        return Manager.StartCoroutine(coroutine);
    }
}
