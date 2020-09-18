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
        string newAssetName = statBlockType.Name.Substring(1) + "StatBlock.asset";
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

    [MenuItem("Assets/Cards/CreateNewCard")]
    static void CreateCard()
    {
        string path = "Assets/Resources/Cards";
        AssetDatabase.CreateFolder(path, "NewCard");
        AssetDatabase.CopyAsset("Assets/Scripts/Cards/CardDataTemplate.cs",
            path + "/NewCard/CNewCard.cs");
        AssetDatabase.SaveAssets();

        path = "Assets/Resources/Cards/NewCard";

        UnityEngine.Object folder = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
        Selection.activeObject = folder;
        EditorGUIUtility.PingObject(folder);
    }

    [MenuItem("Assets/Cards/CreateNewCard",true)]
    static bool CreateCardValidator()
    {
        if (!(Selection.activeObject is UnityEditor.DefaultAsset))
        {
            return false;
        }
        //if(AssetDatabase.GetAssetPath(Selection.activeObject))

        return (AssetDatabase.GetAssetPath(Selection.activeObject) == "Assets/Resources/Cards");
    }
}
