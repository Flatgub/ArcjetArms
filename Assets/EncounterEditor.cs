using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class EncounterEditor : MonoBehaviour
{
    public HexGrid grid;
    public GameObject terrainMarkerPrefab;
    public GameObject enemyMarkerPrefab;
    public GameObject playerMarkerPrefab;
    public int mapRadius;

    public Dictionary<Hex, GameObject> terrainPositions;
    public Dictionary<Hex, GameObject> enemySpawns;
    public Dictionary<Hex, GameObject> playerSpawns;

    public InputField templateNameField;
    public string templateName;

    public int brushMode;

    // Start is called before the first frame update
    void Start()
    {
        terrainPositions = new Dictionary<Hex, GameObject>();
        enemySpawns = new Dictionary<Hex, GameObject>();
        playerSpawns = new Dictionary<Hex, GameObject>();
        grid.GenerateMap(mapRadius);
    }

    private void MakeTerrainMarker(Hex pos)
    {
        GameObject marker = Instantiate(terrainMarkerPrefab, transform);
        marker.SetActive(true);
        marker.transform.position = grid.GetWorldPosition(pos);
        terrainPositions.Add(pos, marker);
    }
    
    private void MakeEnemySpawn(Hex pos)
    {
        GameObject marker = Instantiate(enemyMarkerPrefab, transform);
        marker.SetActive(true);
        marker.transform.position = grid.GetWorldPosition(pos);
        enemySpawns.Add(pos, marker);
    }

    private void MakePlayerSpawn(Hex pos)
    {
        GameObject marker = Instantiate(playerMarkerPrefab, transform);
        marker.SetActive(true);
        marker.transform.position = grid.GetWorldPosition(pos);
        playerSpawns.Add(pos, marker);
    }

    // Update is called once per frame
    void Update()
    {
        if (grid.GetHexUnderMouse() is Hex mousehex)
        {
            if (Input.GetMouseButtonDown(0))
            {
                switch (brushMode)
                {
                    case 0: //Terrain brush
                    {
                        if (terrainPositions.TryGetValue(mousehex, out GameObject marker))
                        {
                            Destroy(marker);
                            terrainPositions.Remove(mousehex);
                        }
                        else
                        {
                            MakeTerrainMarker(mousehex);
                        }
                    };break;

                    case 1: //enemy brush
                    {
                        if (enemySpawns.TryGetValue(mousehex, out GameObject marker))
                        {
                            Destroy(marker);
                            enemySpawns.Remove(mousehex);
                        }
                        else
                        {
                            MakeEnemySpawn(mousehex);
                        }
                    };break;

                    case 2:
                    {//playerbrush 
                        if (playerSpawns.TryGetValue(mousehex, out GameObject marker))
                        {
                            Destroy(marker);
                            playerSpawns.Remove(mousehex);
                        }
                        else
                        {
                            MakePlayerSpawn(mousehex);
                        }
                    };break;
                }

                
            }

            //enemyText.enabled = true;
            //enemyText.text = "ENEMY HEALTH: " + entUnderMouse.Health;
            //GameplayContext.EntityUnderMouse = entUnderMouse;
        }
    }

    public void SaveEncounter()
    {
        EncounterTemplate template = ScriptableObject.CreateInstance<EncounterTemplate>();

        //terrain
        foreach (KeyValuePair<Hex, GameObject> pair in terrainPositions)
        {
            template.AddTerrain(pair.Key);
        }

        //enemies
        template.minEnemies = enemySpawns.Count;
        template.maxEnemies = enemySpawns.Count;
        foreach (KeyValuePair<Hex, GameObject> pair in enemySpawns)
        {
            template.AddEnemySpawn(pair.Key);
        }

        //playerspawns
        //terrain
        foreach (KeyValuePair<Hex, GameObject> pair in playerSpawns)
        {
            template.AddPlayerSpawn(pair.Key);
        }

        AssetDatabase.CreateAsset(template, "Assets/Resources/EncounterTemplates/"+templateName+".asset");

        AssetDatabase.SaveAssets();
        
    }

    public void ClearMap()
    {
        foreach (KeyValuePair<Hex, GameObject> pair in terrainPositions)
        {
            Destroy(pair.Value);
        }
        terrainPositions.Clear();

        foreach (KeyValuePair<Hex, GameObject> pair in enemySpawns)
        {
            Destroy(pair.Value);
        }
        enemySpawns.Clear();

        foreach (KeyValuePair<Hex, GameObject> pair in playerSpawns)
        {
            Destroy(pair.Value);
        }
        playerSpawns.Clear();
    }

    public void LoadEncounter()
    {
        EncounterTemplate template = Resources.Load<EncounterTemplate>("EncounterTemplates/" + templateName);
        if (template is EncounterTemplate)
        {
            ClearMap();

            foreach (Hex pos in template.terrainPieces)
            {
                MakeTerrainMarker(pos);
            }

            foreach (Hex pos in template.enemySpawnPoints)
            {
                MakeEnemySpawn(pos);
            }

            foreach (Hex pos in template.playerSpawnPoints)
            {
                MakePlayerSpawn(pos);
            }
        }
        else
        {
            Debug.LogWarning("unable to load template '" + templateName + "'");
        }
    }

    public void UpdateTemplateName(string name)
    {
        templateName = templateNameField.text;
    }

    public void ChangeBrush(int brush)
    {
        brushMode = brush;
    }
}

