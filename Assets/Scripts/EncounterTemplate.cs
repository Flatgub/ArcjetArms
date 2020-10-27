using System.Collections.Generic;
using UnityEngine;
public class EncounterTemplate : ScriptableObject
{
    public List<PODHex> terrainPieces;
    public List<PODHex> playerSpawnPoints;
    public List<PODHex> enemySpawnPoints;
    public int minEnemies;
    public int maxEnemies;

    public void Awake()
    {
        terrainPieces = new List<PODHex>();
        playerSpawnPoints = new List<PODHex>();
        enemySpawnPoints = new List<PODHex>();
    }

    public void AddTerrain(Hex pos)
    {
        terrainPieces.Add(pos);
    }

    public void AddPlayerSpawn(Hex pos)
    {
        playerSpawnPoints.Add(pos);
    }

    public void AddEnemySpawn(Hex pos)
    {
        enemySpawnPoints.Add(pos);
    }

}