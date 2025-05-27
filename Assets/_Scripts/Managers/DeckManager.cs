using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    // total number of decks in full deck
    [SerializeField] private int numberOfDecks = 1;
    // set seed for cards
    [SerializeField] private int seed = 0;
    
    // default card data SO
    [SerializeField] private CardData defaultCard;
    
    // prefab for each card
    [SerializeField] private GameObject cardPrefab;
    
    // background sprite for each card
    [SerializeField] private Sprite cardBackground;
    
    // all the card sprites (A-K, and all suits)
    [SerializeField] private Sprite[] cardSprites = new Sprite[52];
    
    // current deck
    [SerializeField] private List<CardData> currentDeck = new List<CardData>();

    // all the hands
    [SerializeField] private List<BJHandManager> allHands = new List<BJHandManager>();
    
    private void Start()
    {
        // create the deck and shuffle it
        BuildDeck();
        ShuffleDeck();
    }
    
    public void RegisterHand(BJHandManager hand)
    {
        // if we don't already have the hand, add it
        if (!allHands.Contains(hand))
            allHands.Add(hand);
    }

    public void UnregisterHand(BJHandManager hand)
    {
        // if we already have the hand, remove it
        if (allHands.Contains(hand))
            allHands.Remove(hand);
    }
    
    void BuildDeck()
    {
        // clear the current deck
        currentDeck.Clear();

        // for each type of card * number of decks we want in the full deck
        for (var i = 0; i < cardSprites.Length * numberOfDecks; i++)
        {
            // get the sprite
            var sprite = cardSprites[i % cardSprites.Length];
            // create a new copy of the default card
            var card = defaultCard.Clone();

            // set the card's name and sprite value based on the sprite we are on
            card.name = sprite.name;
            card.SetCardData(sprite.name, sprite, cardBackground);
            
            // split the name into the parts of a card (jack_of_hearts => jack,hearts)
            var parts = sprite.name.Split("_of_");
            var valueStr = parts[0];
            var suitStr = parts[1];

            // set the suit to upper
            card.cardSuit = suitStr.ToUpper();

            // get the value based on the valueStr
            int value;
            switch (valueStr)
            {
                case "ace":
                    value = 1;
                    break;
                case "jack":
                    value = 11;
                    break;
                case "queen":
                    value = 12;
                    break;
                case "king":
                    value = 13;
                    break;
                default:
                    int.TryParse(valueStr, out value);
                    break;
            }

            // set the value
            card.cardValue = value;
            // add it to the deck
            currentDeck.Add(card);
        }
    }
    
    public void ShuffleDeck()
    {
        // set the seed in System.Random
        var rng = new System.Random(seed);
        // get deck count
        var n = currentDeck.Count;
        // for each card
        while (n > 1)
        {
            // decrease by 1
            n--;
            // get next value from seed
            var k = rng.Next(n + 1);
            // swap the two cards
            (currentDeck[n], currentDeck[k]) = (currentDeck[k], currentDeck[n]);
        }
    }
    
    // Draws a card to the specified hand
    public CardData DrawCardToHand(BJHandManager hand, bool faceDown = false)
    {
        // if the current deck is empty
        if (currentDeck.Count == 0)
        {
            // send warning
            Debug.LogWarning("Deck is empty!");
            return null;
        }

        // otherwise, get topdecked
        var card = currentDeck[0];
        // remove that card from the deck
        currentDeck.RemoveAt(0);
        // and add it to that hand
        hand.AddCard(card, faceDown);

        // then return the card for more data
        return card;
    }
}