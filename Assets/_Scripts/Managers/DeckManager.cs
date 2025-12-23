using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    [Header("Sprites")]
    public Sprite cardBackSprite;
    
    [Header("Deck Settings")]
    [Tooltip("How many sets of 52 cards are in total deck")]
    [SerializeField] private int deckCount;
    
    [Tooltip("Seed for the deck generation")]
    [SerializeField] private int seed;
    
    [Header("Deck")]
    public List<CardInstance> deck;

    private void Start()
    {
        CreateDeck();
        ShuffleDeck();
    }

    private void CreateDeck()
    {
        deck ??= new List<CardInstance>();

        deck.Clear();

        for (int i = 0; i < deckCount; i++)
        {
            foreach (CardSuit suit in Enum.GetValues(typeof(CardSuit)))
            {
                foreach (CardRank rank in Enum.GetValues(typeof(CardRank)))
                {
                    CardInstance card = new CardInstance
                    {
                        suit = suit,
                        rank = rank,
                        value = GetCardValue(rank)
                    };

                    deck.Add(card);
                }
            }
        }
    }
    
    private int GetCardValue(CardRank rank)
    {
        return rank switch
        {
            CardRank.Ace => 11,
            CardRank.Jack => 10,
            CardRank.Queen => 10,
            CardRank.King => 10,
            _ => (int)rank
        };
    }
    
    private void ShuffleDeck()
    {
        System.Random rng = new System.Random(seed);

        for (int i = deck.Count - 1; i > 0; i--)
        {
            int j = rng.Next(0, i + 1);

            // swap
            (deck[i], deck[j]) = (deck[j], deck[i]);
        }
    }
}