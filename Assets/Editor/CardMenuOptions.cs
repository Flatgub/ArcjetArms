using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CardMenuOptions : MonoBehaviour
{
    [MenuItem("Assets/Cards/CreateStatBlock")]
    static void CreateCardStatBlock()
    {
        MonoScript buildingFrom = Selection.activeObject as MonoScript;
        Type statBlockType = buildingFrom.GetClass();

        string scriptPath = AssetDatabase.GetAssetPath(buildingFrom);
        string scriptDirectory = System.IO.Path.GetDirectoryName(scriptPath).Replace("\\","/");
        string newAssetName = "New" + statBlockType.Name + "_StatBlock.asset";
        string fullPath = scriptDirectory + "/" + newAssetName;


        ScriptableObject asset = ScriptableObject.CreateInstance(statBlockType);
        AssetDatabase.CreateAsset(asset, fullPath);

        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = asset;
    }

    
    [MenuItem("Assets/Cards/CreateStatBlock", true)]
    static bool CreateCardStatBlockValidator()
    {
        if (Selection.activeObject.GetType() != typeof(MonoScript))
        {
            return false;
        }
        MonoScript scr = Selection.activeObject as MonoScript;
        return scr.GetClass().IsSubclassOf(typeof(CardData));
    }
}
