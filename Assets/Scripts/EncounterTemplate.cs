using System.Collections.Generic;
using UnityEngine;
class EncounterTemplate : ScriptableObject
{
    public List<Hex> terrainPieces;

    public void Awake()
    {
        terrainPieces = new List<Hex>();
    }

    public void AddTerrain(Hex pos)
    {
        terrainPieces.Add(pos);
    }

}