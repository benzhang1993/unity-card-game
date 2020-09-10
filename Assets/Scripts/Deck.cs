using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck
{
    private List<Card> deck;
    private List<Card> discard;
    private static Random rng = new Random();

    public Deck()
    {
        deck = new List<Card>();
    }

    public void shuffle()
    {
        for (int i = 0; i < deck.Count; i++) {
         Card temp = deck[i];
         int randomIndex = Random.Range(i, deck.Count);
         deck[i] = deck[randomIndex];
         deck[randomIndex] = temp;
     }
    }

    public Card draw()
    {
        Card cardToDraw = deck[0];
        deck.RemoveAt(0);
        return cardToDraw;
    }

    // TODO - remove card from deck after drawing
    public List<Card> draw(int numberOfCards)
    {
        if(numberOfCards > deck.Count)
        {
            throw new System.InvalidOperationException("Deck size is: " + deck.Count + ", unable to draw " + numberOfCards);
        }
        
        List<Card> cardsToDraw = new List<Card>();
        for(int i = 0; i < numberOfCards; i++)
        {
            cardsToDraw.Add(deck[i]);
        }
        return cardsToDraw;
    }

    public void putCardOnTop(Card card)
    {
        deck.Insert(0, card);
    }

    public void loadCardArray(Card[] cards)
    {
        deck.Clear();
        foreach(Card card in cards)
        {
            deck.Add(card);
        }
    }
}
