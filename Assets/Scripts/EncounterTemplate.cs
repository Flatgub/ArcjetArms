using System.Collections.Generic;
using UnityEngine;
class EncounterTemplate : ScriptableObject
{
    public List<Hex> terrainPieces;
    public List<Hex> playerSpawnPoints;
    public List<Hex> enemySpawnPoints;
    public int minEnemies;
    public int maxEnemies;

    public void Awake()
    {
        terrainPieces = new List<Hex>();
        playerSpawnPoints = new List<Hex>();
        enemySpawnPoints = new List<Hex>();
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