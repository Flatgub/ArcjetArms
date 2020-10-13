using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewTerrain", menuName = "Terrain Type", order = 1)]
public class TerrainType : ScriptableObject
{
    public string title;
    public Sprite[] images;
}
