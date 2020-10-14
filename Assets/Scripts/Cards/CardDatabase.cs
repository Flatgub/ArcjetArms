using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// A static singleton class that is used to load and then access all the cards in the game.
/// </summary>
public static class CardDatabase
{
    private static readonly SortedList<int, CardData> allCards;
    private static readonly Dictionary<string, CardData> cardsByName;
    public static readonly Sprite[] EnergyCostFrames;
    private static bool loaded = false;

    static CardDatabase()
    {
        allCards = new SortedList<int, CardData>();
        cardsByName = new Dictionary<string, CardData>();
        EnergyCostFrames = Resources.LoadAll<Sprite>("Cards/CardPriceSprites");
    }

    /// <summary>
    /// Load all cards from the resources folders into the allCards list.
    /// </summary>
    public static void LoadAllCards()
    {
        if (!loaded)
        {
            LoadAllCardsInFolder("Cards");
            loaded = true;
        }
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

    public static CardData GetCardDataByName(string name)
    {
        if (cardsByName.TryGetValue(name, out CardData cd))
        {
            return cd;
        }
        else
        {
            throw new IndexOutOfRangeException("No such card exists for name '" + name + "'");
        }
    }

    public static int GetCardIDByName(string name)
    {
        if (cardsByName.TryGetValue(name, out CardData cd))
        {
            return cd.cardID;
        }
        else
        {
            throw new IndexOutOfRangeException("No such card exists for name '" + name + "'");
        }
    }

    public static Card CreateCardFromID(int id)
    {
        CardData data = GetCardDataByID(id);
        return new Card(data);
    }

    public static Card CreateCardFromName(string name)
    {
        CardData data = GetCardDataByName(name);
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
        CardData[] cardsLoaded = Resources.LoadAll<CardData>(folder);
        foreach (CardData cardasset in cardsLoaded)
        {
            //Debug.Log("loading card " + cardasset.cardID + ", " + cardasset.title);
            if (!CheckForConflict(cardasset))
            {
                allCards.Add(cardasset.cardID, cardasset);
                cardsByName.Add(cardasset.title, cardasset);
            }
        }
    }

    private static bool CheckForConflict(CardData card)
    {
        if (allCards.ContainsKey(card.cardID))
        {
            CardData conflict = allCards[card.cardID];

            string warning =
                string.Format("Card {0} has conflicting ID with {1} (id {2}), skipping...",
                card.title, conflict.title, card.cardID);

            Debug.LogWarning(warning);
            return true;
        }
        else if (cardsByName.ContainsKey(card.title))
        {
            string warning =
                string.Format("Card {0} has conflicting name, {1} is already in use, skipping... ",
                card.cardID, card.title);

            Debug.LogWarning(warning);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Convert an absolute directory into a Resources.Load compatible directory
    /// directory (baz/qux)
    /// </summary>
    /// <remarks>
    /// for example, converts "E:/foo/bar/Assets/Resources/baz/qux" into just "baz/qux" </remarks>
    /// <param name="dir">the directory to convert</param>
    /// <returns>a Resources.Load compatable shortened directory</returns>
    public static string ConvertRealDirToResourceDir(string dir)
    {
        dir = dir.Replace("\\", "/");
        int assetStart = dir.IndexOf("/Resource") + 11;
        return dir.Substring(assetStart, dir.Length - assetStart);
    }
}
