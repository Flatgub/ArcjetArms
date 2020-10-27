using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameplayContext 
{
    public static GameManager Manager { get; private set; }
    public static Entity Player { get; private set; }
    public static HexGrid Grid { get; private set; }
    public static InterfaceManager Ui { get; private set; }
    public static Entity ActiveEntity { get; set; }
    public static Entity EntityUnderMouse { get; set; }
    public static Card ActiveCard { get; set;}
    public static GearLoadout CurrentLoadout { get; set; }
    public static InventoryCollection CurrentInventory { get; set; }
    public static EncounterTemplate ChosenTemplate;
    public static int CurrentDifficulty { get; set; }

    public static bool InDebugMode = true;

    public static string OverworldMap { get; set; }

    public static void InitializeForEncounter(GameManager gm, Entity player, HexGrid grid, InterfaceManager ui)
    {
        Manager = gm;
        Player = player;
        Grid = grid;
        Ui = ui;
    }

    public static void Clear()
    {
        Manager = null;
        Player = null;
        Grid = null;
        Ui = null;
        EntityUnderMouse = null;
        ActiveEntity = null;
        ActiveCard = null;
    }

    public static Coroutine StartCoroutineOnManager(IEnumerator coroutine)
    {
        return Manager.StartCoroutine(coroutine);
    }
}
