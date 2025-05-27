using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BJHandManager : MonoBehaviour
{
    // types of hands
    public enum HandOwner
    {
        Player,
        Dealer
    }

    // the type of our specific hand
    [SerializeField] private HandOwner owner;
    // where each card will spawn
    [SerializeField] private Transform cardParent;
    // prefab for each card
    [SerializeField] private GameObject cardPrefab;
    // reference to deck
    [SerializeField] private DeckManager deckManager;

    // current card the player has
    [SerializeField] List<GameObject> currentCards = new List<GameObject>();
    // spacing between each card
    [SerializeField] private float cardXSpacing = 2f, cardYSpacing = 2f;
    
    // total value of each hand, and text to display
    [SerializeField] private TMP_Text handText;
    private int handValue = 0;

    [SerializeField] private GameObject handPuck;
    
    public int GetHandValue() => GetHandValueWithSoftStatus().value;

    private void Start()
    {
        // register this hand to the deck
        if (deckManager != null)
        {
            deckManager.RegisterHand(this);
        }
    }
    
    // draw a card to this hand
    public void DrawCard(bool faceDown = false)
    {
        if (deckManager != null)
        {
            deckManager.DrawCardToHand(this, faceDown);
        }
        else
        {
            Debug.LogWarning("DeckManager not assigned to " + gameObject.name);
        }
    }
    
    // add a card to the hand
    public void AddCard(CardData cardData, bool faceDown = false)
    {
        // instantiate new card
        var newCard = Instantiate(cardPrefab, cardParent);
        // get the name of the card from the data
        newCard.name = cardData.cardName;
        
        // add CardVisual and assign data
        var visual = newCard.GetComponent<CardVisual>();
        visual.cardData = cardData;
        visual.isFaceDown = faceDown;

        // get the sprite renderer
        var spriteRenderer = newCard.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            // set the sprite to the sprite in the card data, or the back if it is face down
            spriteRenderer.sprite = faceDown ? cardData.cardBackground : cardData.cardSprite;
            // set above the rest of the cards in hand
            spriteRenderer.sortingOrder = currentCards.Count;
        }
        
        switch (owner)
        {
            case HandOwner.Player:
            {
                var localScale = cardPrefab.transform.localScale;
                var offsetX = currentCards.Count * cardXSpacing * localScale.x;
                var offsetY = currentCards.Count * cardYSpacing * localScale.y;
                
                if (isDoubleDown && currentCards.Count == 2)
                {
                    newCard.transform.localPosition = new Vector3(1.25f * offsetX, 0f, 1.25f * offsetY);
                    newCard.transform.localRotation = Quaternion.Euler(90, 0f, -90f);
                }
                else
                {
                    newCard.transform.localPosition = new Vector3(offsetX, 0f, offsetY);
                }

                break;
            }


            case HandOwner.Dealer:
            {
                // Position the card with spacing as a dealer would
                var offsetX = currentCards.Count * cardXSpacing * cardPrefab.transform.localScale.x;
                newCard.transform.localPosition = new Vector3(-offsetX, 0f, 0f);
                break;
            }
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        // add card to current cards
        currentCards.Add(newCard);
        
        if (!faceDown) UpdateHandText();
    }

    public void ClearHand()
    {
        // for each card held
        foreach (var card in currentCards)
        {
            // remove card
            Destroy(card);
        }
        // clear all cards
        currentCards.Clear();

        // clear value of BJ Hand
        UpdateHandText();
        
        isDoubleDown = false;

        handValue = 0;
        
        SetPuckColor(Color.clear);
    }

    public (int value, bool isSoft) GetHandValueWithSoftStatus()
    {
        var total = 0;
        var aceCount = 0;

        foreach (var cardObj in currentCards)
        {
            var visual = cardObj.GetComponent<CardVisual>();
            if (visual == null || visual.cardData == null)
                continue;

            var value = visual.cardData.cardValue;

            switch (value)
            {
                case 1:
                    aceCount++;
                    total += 1;
                    break;
                case >= 11 and <= 13:
                    total += 10;
                    break;
                default:
                    total += value;
                    break;
            }
        }

        bool isSoft = false;

        if (aceCount > 0 && total + 10 <= 21)
        {
            total += 10;
            isSoft = true;
        }

        return (total, isSoft);
    }



    public List<GameObject> GetCurrentCards()
    {
        // get current cards
        return currentCards;
    }

    public HandOwner GetOwner()
    {
        // get owner of the cards
        return owner;
    }

    public void RevealAllCards()
    {
        foreach (var card in currentCards)
        {
            var visual = card.GetComponent<CardVisual>();
            if (visual != null)
            {
                visual.Reveal();
            }
        }

        if (owner == HandOwner.Dealer)
        {
            UpdateHandText();
        }
    }

    private void UpdateHandText()
    {
        if (handText == null) return;

        var (value, isSoft) = GetHandValueWithSoftStatus();

        if (owner == HandOwner.Player)
        {
            if (IsBlackjack())
            {
                handText.text = $"<color=yellow>{value}</color>";
            }
            else if (isSoft)
            {
                // Show both possible values, e.g., "7 / 17"
                int baseValue = value - 10; // since soft means one ace is counted as 11
                handText.text = $"{baseValue} / {value}";
            }
            else
            {
                handText.text = value.ToString();
            }
        }
        else // Dealer
        {
            handText.text = value.ToString();
        }
    }

    private bool isDoubleDown = false;
    public void MarkDoubleDown()
    {
        isDoubleDown = true;
    }
    
    public bool IsBlackjack()
    {
        return currentCards.Count == 2 && GetHandValue() == 21;
    }
    
    public bool IsBusted()
    {
        return GetHandValue() > 21;
    }
    
    public CardData GetCardData(int index)
    {
        if (index < 0 || index >= currentCards.Count) return null;
        var visual = currentCards[index].GetComponent<CardVisual>();
        return visual != null ? visual.cardData : null;
    }

    public bool CanSplit()
    {
        if (currentCards.Count != 2) return false;

        var first = GetCardData(0);
        var second = GetCardData(1);

        if (first == null || second == null) return false;

        // Treat all face cards and 10s as the same for splitting
        var firstIsFaceOrTen = first.cardValue >= 10;
        var secondIsFaceOrTen = second.cardValue >= 10;

        if (firstIsFaceOrTen && secondIsFaceOrTen) return true;

        return first.cardValue == second.cardValue;
    }


    public int TotalCards()
    {
        return currentCards.Count;
    }
    
    public void SetPuckColor(Color color)
    {
        if (handPuck != null)
        {
            var renderer = handPuck.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = color;
            }
        }
    }

}