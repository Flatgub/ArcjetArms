using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

/// <summary>
/// A Deck is a collection of cards with various methods for drawing, merging and shuffling
/// </summary>
public class Deck : IEnumerable<Card>
{
    private readonly List<Card> cards;

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

    public Deck(DeckTemplate template)
    {
        cards = new List<Card>();
        template.ConvertToDeck(this);
    }

    /// <summary>
    /// Puts a card on the top of the deck
    /// </summary>
    /// <param name="card">The card to add</param>
    public void AddToTop(Card card)
    {
        cards.Insert(0, card);
    }

    /// <summary>
    /// Puts a card on the bottom of the deck
    /// </summary>
    /// <param name="card">The card to add</param>
    public void AddToBottom(Card card)
    {
        cards.Add(card);
    }

    /// <summary>
    /// Gets and removes the top card from the deck
    /// </summary>
    /// <returns>The top card from the deck</returns>
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

    /// <summary>
    /// Takes all the cards in this deck and moves them to another deck instead.
    /// </summary>
    /// <param name="other">The deck to move all the carsd to</param>
    /// <param name="addToTop">Whether the cards should be added to the top instead of the 
    /// bottom of the other deck</param>
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

    // helper for debugging
    public void PrintContents()
    {
        
        StringBuilder sb = new StringBuilder("Contains: \n");
        foreach(Card c in cards)
        {
            sb.AppendLine("  " + c.cardData.title);
        }
        Debug.Log(sb.ToString());
    }

    public void Clear()
    {
        cards.Clear();
    }
}
