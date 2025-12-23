using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class VisualizeHand : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BlackJackHand hand;
    [SerializeField] private DeckManager deck;
    [SerializeField] private CardVisual cardVisualPrefab;

    [Header("Layout")]
    [SerializeField] private Vector3 startingCardLocation;
    [SerializeField] private Vector3 cardSpacing;

    private int lastCardCount = 0;
    private readonly List<CardVisual> visuals = new();

    private void Awake()
    {
        if (deck == null)
            deck = FindObjectOfType<DeckManager>();
    }

    private void Update()
    {
        // if we added new card(s)
        if (hand.GetCardCount() != lastCardCount)
        {
            // while we still have cards to add
            while (visuals.Count < hand.GetCardCount())
            {
                // make a new card visual
                CreateCard(visuals.Count);
            }
            
            // set new count
            lastCardCount = hand.GetCardCount();
        }
    }


    private void CreateCard(int cardIndex)
    {
        // get the card to make
        CardInstance card = hand.GetCardAt(cardIndex);
        
        Vector3 localPos = startingCardLocation + cardSpacing * cardIndex;

        // create the visual for it based off prefab
        CardVisual visual = Instantiate(cardVisualPrefab, transform);
        
        // set rotation and position
        visual.transform.localPosition = localPos;

        // set the visual of the card
        visual.SetFrontOfCard(card);
        visual.SetBackOfCard(deck.cardBackSprite);
        visual.SetFaceDown(card.faceDown);
        visual.SetSortingOrder(cardIndex);
        
        // play animation
        visual.FallDown();
        
        visuals.Add(visual);
    }

    public void RefreshCard(int cardIndex)
    {
        if (cardIndex < 0 || cardIndex >= visuals.Count)
            return;

        var card = hand.GetCardAt(cardIndex);
        visuals[cardIndex].SetFrontOfCard(card);
        visuals[cardIndex].SetFaceDown(card.faceDown);
    }
    
    public void RemoveLastCard()
    {
        if (visuals.Count == 0)
            return;

        var last = visuals[^1];
        visuals.RemoveAt(visuals.Count - 1);
        Destroy(last.gameObject);

        lastCardCount = visuals.Count;
    }


    public void ClearCards()
    {
        foreach (var visual in visuals)
        {
            Destroy(visual.gameObject);
        }

        visuals.Clear();
        lastCardCount = 0;
    }

}