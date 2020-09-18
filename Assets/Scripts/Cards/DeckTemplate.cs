using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckTemplate 
{
    private readonly Dictionary<int, int> cardsInTemplate;

    public DeckTemplate()
    {
        cardsInTemplate = new Dictionary<int, int>();
    }

    public void AddCardID(int id, int numberOf = 1)
    {
        cardsInTemplate.TryGetValue(id, out int number);
        cardsInTemplate[id] = number + numberOf;
    }

    public void RemoveCardID(int id, int numberOf = 1)
    {
        cardsInTemplate.TryGetValue(id, out int number);
        if (number > numberOf)
        {
            cardsInTemplate[id] = number - numberOf;
        }
        else
        {
            cardsInTemplate.Remove(id);
        }
    }

    public Deck ConvertToDeck()
    {
        Deck deck = new Deck();
        foreach (KeyValuePair<int, int> cardType in cardsInTemplate)
        {
            int id = cardType.Key;
            int numberOf = cardType.Value;
            for (int i = 0; i < numberOf; i++)
            {
                deck.AddToTop(CardDatabase.CreateCardFromID(id));
            }
        }
        return deck;
    }

    public void ConvertToDeck(Deck deck)
    {
        deck.Clear();
        foreach (KeyValuePair<int, int> cardType in cardsInTemplate)
        {
            int id = cardType.Key;
            int numberOf = cardType.Value;
            for (int i = 0; i < numberOf; i++)
            {
                deck.AddToTop(CardDatabase.CreateCardFromID(id));
            }
        }
    }
}
