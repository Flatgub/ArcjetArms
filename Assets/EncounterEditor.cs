using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class EncounterEditor : MonoBehaviour
{
    public HexGrid grid;
    public GameObject terrainMarkerPrefab;
    public int mapRadius;

    public Dictionary<Hex, GameObject> terrainPositions;

    public InputField templateNameField;
    public string templateName;

    // Start is called before the first frame update
    void Start()
    {
        terrainPositions = new Dictionary<Hex, GameObject>();
        grid.GenerateMap(mapRadius);
    }

    private void MakeTerrainMarker(Hex pos)
    {
        GameObject marker = Instantiate(terrainMarkerPrefab, transform);
        marker.SetActive(true);
        marker.transform.position = grid.GetWorldPosition(pos);
        terrainPositions.Add(pos, marker);
    }

    // Update is called once per frame
    void Update()
    {
        if (grid.GetHexUnderMouse() is Hex mousehex)
        {
            if (Input.GetMouseButtonDown(0))
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
            }

            //enemyText.enabled = true;
            //enemyText.text = "ENEMY HEALTH: " + entUnderMouse.Health;
            //GameplayContext.EntityUnderMouse = entUnderMouse;
        }
    }

    public void SaveEncounter()
    {
        EncounterTemplate template = ScriptableObject.CreateInstance<EncounterTemplate>();

        foreach (KeyValuePair<Hex, GameObject> pair in terrainPositions)
        {
            template.AddTerrain(pair.Key);
        }

        AssetDatabase.CreateAsset(template, "Assets/Resources/EncounterTemplates/"+templateName+".asset");

        AssetDatabase.SaveAssets();
        
    }

    public void LoadEncounter()
    {
        EncounterTemplate template = Resources.Load<EncounterTemplate>("EncounterTemplates/" + templateName);
        if (template is EncounterTemplate)
        {
            foreach (KeyValuePair<Hex, GameObject> pair in terrainPositions)
            {
                Destroy(pair.Value);
            }

            terrainPositions.Clear();

            foreach (Hex pos in template.terrainPieces)
            {
                MakeTerrainMarker(pos);
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
}

