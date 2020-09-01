using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardRendererFactory : MonoBehaviour
{
    private static CardRendererFactory factoryInstance;
    private CardRenderer cardPrefab;

    /// <summary>
    /// Get the singleton instance of the CardRenderer, or make one if it doesn't yet exist
    /// </summary>
    public static CardRendererFactory GetFactory
    {
        get
        {
            if (factoryInstance == null)
            {
                GameObject obj = new GameObject();
                factoryInstance = obj.AddComponent<CardRendererFactory>();
            }
            return factoryInstance;
        }
    }

    public void Awake()
    {
        //FIXME: perhaps don't hardcode the directory for CardPrefab?
        cardPrefab = Resources.Load<CardRenderer>("Prefabs/CardPrefab");
    }

    /// <summary>
    /// Create a new CardRenderer object and tie it to a card
    /// </summary>
    /// <param name="forCard"></param>
    /// <returns>A CardRenderer component, attached to new card prefab</returns>
    public CardRenderer CreateCardRenderer(Card forCard)
    {
        //construct new card prefab based on the input card
        CardRenderer instance = Instantiate(cardPrefab);
        instance.TieTo(forCard);
        
        return instance;
    }

    /// <summary>
    /// Create a new CardRenderer object and based on a specific CardData
    /// </summary>
    /// <param name="basedOn"></param>
    /// <returns>A CardRenderer component, attached to new card prefab</returns>
    public CardRenderer CreateCardRenderer(CardData basedOn)
    {
        //construct new card prefab based on the input carddata
        CardRenderer instance = Instantiate(cardPrefab);
        instance.ShowCardData(basedOn);
        return instance;
    }
}
