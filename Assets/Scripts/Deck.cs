using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck
{
    private List<GameObject> deck;
    private List<GameObject> discard;
    private static Random rng = new Random();

    public Deck()
    {
        deck = new List<GameObject>();
        discard = new List<GameObject>();
    }

    public void shuffle()
    {
        for (int i = 0; i < deck.Count; i++) {
         GameObject temp = deck[i];
         int randomIndex = Random.Range(i, deck.Count);
         deck[i] = deck[randomIndex];
         deck[randomIndex] = temp;
     }
    }

    public GameObject draw()
    {
        GameObject cardToDraw = deck[0];
        deck.RemoveAt(0);
        return cardToDraw;
    }

    public List<GameObject> draw(int numberOfCards)
    {
        if(numberOfCards > deck.Count + discard.Count)
        {
            throw new System.InvalidOperationException("Deck size is: " + deck.Count + ", unable to draw " + numberOfCards);
        }
        
        List<GameObject> cardsToDraw = new List<GameObject>();

        if(numberOfCards > deck.Count)
        {
            foreach(GameObject card in discard)
            {
                deck.Add(card);
                
            }
            discard.Clear();
        }
        
        
        for(int i = 0; i < numberOfCards; i++)
        {
            // remove could cause issues if not removing only the first card;
            cardsToDraw.Add(deck[0]);
            deck.Remove(deck[0]);
        }
        
        return cardsToDraw;
    }

    public void putCardOnTop(GameObject card)
    {
        deck.Insert(0, card);
    }

    public void setDeck(List<GameObject> cards)
    {
        deck.Clear();
        deck = cards;
    }

    public void sendToDiscard(GameObject card)
    {
        discard.Add(card);
    }
}
