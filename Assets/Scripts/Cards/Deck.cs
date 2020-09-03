using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Deck : IEnumerable<Card>
{
    private List<Card> cards;

    public int Count
    {
        get
        {
            return cards.Count;
        }
    }

    public Deck()
    {
        cards = new List<Card>();
    }

    public void AddToTop(Card card)
    {
        cards.Insert(0, card);
    }

    public void AddToBottom(Card card)
    {
        cards.Add(card);
    }

    public Card TakeFromTop()
    {
        if (cards.Count == 0)
        {
            return null;
        }
        Card card = cards[0];
        cards.RemoveAt(0);
        return card;
    }

    public void MergeAllInto(Deck other, bool addToTop = false)
    {
        for (var i = cards.Count - 1; i >= 0; i--)
        {
            Card c = TakeFromTop();
            if (addToTop)
            {
                other.AddToTop(c);
            }
            else
            {
                other.AddToBottom(c);
            }
        }
    }

    /// <summary>
    /// Perform a Fisher-Yates shuffle on the deck
    /// </summary>
    public void Shuffle()
    {
        for (int i = cards.Count - 1; i > 0; i--)
        {
            int swapInd = Random.Range(0, i);
            Card swap = cards[swapInd];
            cards[swapInd] = cards[i];
            cards[i] = swap;
        }
    }    

    public IEnumerator<Card> GetEnumerator()
    {
        return cards.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return cards.GetEnumerator();
    }

    public void PrintContents()
    {
        
        StringBuilder sb = new StringBuilder("Contains: \n");
        foreach(Card c in cards)
        {
            sb.AppendLine("  " + c.cardData.title);
        }
        Debug.Log(sb.ToString());
    }
}
