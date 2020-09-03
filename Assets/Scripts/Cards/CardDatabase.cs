using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor.VersionControl;
using UnityEngine;

/// <summary>
/// A static singleton class that is used to load and then access all the cards in the game.
/// </summary>
public static class CardDatabase
{
    private static readonly SortedList<int, CardData> allCards;
    private static readonly string cardsDirectory;
    public static readonly Sprite[] EnergyCostFrames;

    static CardDatabase()
    {
        allCards = new SortedList<int, CardData>();
        cardsDirectory = Application.dataPath + "/Resources/Cards";
        EnergyCostFrames = Resources.LoadAll<Sprite>("Cards/CardPriceSprites");
    }

    /// <summary>
    /// Load all cards from the resources folders into the allCards list.
    /// </summary>
    public static void LoadAllCards()
    {
        LoadAllCardsInFolder(cardsDirectory);
    }

    /// <summary>
    /// Get the card from the 
    /// </summary>
    /// <param name="id">the ID of the card to get</param>
    /// <exception cref="IndexOutOfRangeException">Thrown if no card exists for that ID</exception>
    /// <returns></returns>
    public static CardData GetCardDataByID(int id)
    {
        if (allCards.TryGetValue(id, out CardData cd))
        {
            return cd;
        }
        else
        {
            throw new IndexOutOfRangeException("No such card exists for ID " + id);
        }
    }

    public static Card CreateCardFromID(int id)
    {
        CardData data = GetCardDataByID(id);
        return new Card(data);
    }

    /// <summary>
    /// Get an Enumerator for all cards currently loaded
    /// </summary>
    /// <returns>an IEnumerator for all cards</returns>
    public static IEnumerator GetCardEnumerator()
    {
        return allCards.GetEnumerator();
    }

    public static List<int> GetAllIDs()
    {
        return new List<int>(allCards.Keys);
    }

    /// <summary>
    /// Recursively load all CardData assets from this folder and any folders belowit
    /// </summary>
    /// <param name="folder">The folder to load files from</param>
    private static void LoadAllCardsInFolder(string folder)
    {
        DirectoryInfo dirInfo = new DirectoryInfo(folder);

        //collect all cards at this directory
        foreach (FileInfo card in dirInfo.GetFiles("*.asset"))
        {
            string cardpath = ConvertRealDirToResourceDir(card.FullName);
            cardpath = cardpath.Substring(0, cardpath.Length - 6); //remove .asset from the path
            CardData cardasset = Resources.Load<CardData>(cardpath);
            allCards.Add(cardasset.cardID, cardasset);
        }
        
        //recursively load cards in subdirectories
        foreach (DirectoryInfo d in dirInfo.GetDirectories())
        {
            LoadAllCardsInFolder(d.FullName);
        }
    }

    /// <summary>
    /// Convert an absolute directory into a Resources.Load compatible directory
    /// directory (baz/qux)
    /// </summary>
    /// <remarks>
    /// for example, converts "E:/foo/bar/Assets/Resources/baz/qux" into just "baz/qux" </remarks>
    /// <param name="dir">the directory to convert</param>
    /// <returns>a Resources.Load compatable shortened directory</returns>
    private static string ConvertRealDirToResourceDir(string dir)
    {
        dir = dir.Replace("\\", "/");
        int assetStart = dir.IndexOf("/Resource") + 11;
        return dir.Substring(assetStart, dir.Length - assetStart);
    }
}
