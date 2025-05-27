using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackjackRoundManager : MonoBehaviour
{
    [Header("References")] [SerializeField]
    private DeckManager deckManager;

    [SerializeField] private List<BJHandManager> playerHands;
    [SerializeField] private BJHandManager dealerHand;

    // amount of time between each card dealt
    [SerializeField] private float dealDelay = 0.5f;

    private int currentHandIndex = 0;
    private BJHandManager CurrentHand => playerHands[currentHandIndex];
    
    bool roundStarted = false;

    private void Start()
    {
        foreach (var hand in playerHands)
        {
            hand.SetPuckColor(Color.clear);
        }
        dealerHand.SetPuckColor(Color.clear);
    }

    public void StartRound()
    {
        currentHandIndex = 0;
        
        // clear the dealer and player hands
        dealerHand.ClearHand();
        foreach (var hand in playerHands)
        {
            hand.ClearHand();
        }
        
        // Cleanup: remove all split hands added during the round
        for (int i = playerHands.Count - 1; i >= 0; i--)
        {
            if (playerHands[i].name.Contains("_Split"))
            {
                Destroy(playerHands[i].gameObject);
                playerHands.RemoveAt(i);
            }
        }

        // start dealing each card
        StartCoroutine(DealInitialCardsCoroutine());
    }

    private IEnumerator DealInitialCardsCoroutine()
    {
        // deal two cards to each hand
        for (var i = 0; i < 2; i++)
        {
            // for every hand in player hands
            foreach (var hand in playerHands)
            {
                // draw a card to their hand
                yield return StartCoroutine(DealCardWithDelay(hand));

            }
            
            // draw card to dealer hand
            yield return StartCoroutine(DealCardWithDelay(dealerHand, i == 1));

        }
        
        UpdateHandPucks();
        roundStarted = true;
    }

    public void SetHands(List<BJHandManager> players, BJHandManager dealer)
    {
        // set the hands of our manager
        playerHands = players;
        dealerHand = dealer;
    }

    private IEnumerator MoveToNextHandCoroutine()
    {
        currentHandIndex++;

        if (currentHandIndex < playerHands.Count)
        {
            Debug.Log($"Now playing: Player {currentHandIndex}");
            UpdateHandPucks();
            
            var hand = playerHands[currentHandIndex];
            while (hand.TotalCards() < 2)
            {
                yield return StartCoroutine(DealCardWithDelay(hand));
            }
        }
        else
        {
            StartCoroutine(DealerTurn());
        }
    }

    
    public void HitHand()
    {
        if (!roundStarted) return;

        var hand = CurrentHand;
        deckManager.DrawCardToHand(hand);

        var (value, _) = hand.GetHandValueWithSoftStatus();
        if (value > 21)
        {
            Debug.Log($"Player {currentHandIndex} busted!");
            StartCoroutine(MoveToNextHandCoroutine());
        }

    }

    public void StandHand()
    {
        if (!roundStarted) return;

        Debug.Log($"Player {currentHandIndex} stands.");
        StartCoroutine(MoveToNextHandCoroutine());
    }
    
    private IEnumerator DealerTurn()
    {
        Debug.Log("Dealer's turn starts.");
        UpdateHandPucks();
    
        // Flip dealer's face-down card (you'll need to implement Flip())
        dealerHand.RevealAllCards();

        yield return new WaitForSeconds(1f);

        while (true)
        {
            var (value, isSoft) = dealerHand.GetHandValueWithSoftStatus();

            // Dealer hits on soft 17
            if (value < 17 || (value == 17 && isSoft))
            {
                deckManager.DrawCardToHand(dealerHand);
                yield return new WaitForSeconds(dealDelay);
            }
            else
            {
                break;
            }
        }


        Debug.Log("Dealer stands. Evaluate results.");
        EvaluateResults();
    }
    
    private void EvaluateResults()
    {
        var (dealerValue, _) = dealerHand.GetHandValueWithSoftStatus();

        for (var i = 0; i < playerHands.Count; i++)
        {
            var (playerValue, _) = playerHands[i].GetHandValueWithSoftStatus();
        
            string result;
            if (playerValue > 21) result = "Lose";
            else if (dealerValue > 21 || playerValue > dealerValue) result = "Win";
            else if (playerValue == dealerValue) result = "Push";
            else result = "Lose";

            Debug.Log($"Player {i}: {result}");
        }
        
        roundStarted = false;
    }

    public void DoubleDownHand()
    {
        if (!roundStarted) return;

        var hand = CurrentHand;
        
        if (hand.TotalCards() > 2) return;
        
        Debug.Log($"Player {currentHandIndex} Doubles Down.");
        hand.MarkDoubleDown();
        deckManager.DrawCardToHand(hand);

        var (value, _) = hand.GetHandValueWithSoftStatus();
        if (value > 21)
        {
            Debug.Log($"Player {currentHandIndex} busted!");
        }

        StartCoroutine(MoveToNextHandCoroutine());
    }


    public void SplitHand()
    {
        if (!roundStarted) return;

        var hand = CurrentHand;
        var currentIndex = hand.transform.GetSiblingIndex();

        if (!hand.CanSplit()) return;

        var firstCard = hand.GetCardData(0);
        var secondCard = hand.GetCardData(1);

        // Remove both cards from the original hand
        foreach (var card in hand.GetCurrentCards())
        {
            Destroy(card);
        }
        hand.GetCurrentCards().Clear();
        hand.ClearHand();

        // Add first card back to current hand
        hand.AddCard(firstCard);

        // Spawn the new hand directly to the left of the current hand
        var gameManager = FindObjectOfType<GameManager>();
        var newHandObj = Instantiate(gameManager.handPrefab, gameManager.handsParent);

        // Match naming and position
        newHandObj.name = hand.name + "_Split";
        newHandObj.transform.SetSiblingIndex(currentIndex);

        // Shift the original hand right so new hand appears to the left
        hand.transform.SetSiblingIndex(currentIndex + 1);

        var newHand = newHandObj.GetComponent<BJHandManager>();
        if (newHand != null)
        {
            newHand.AddCard(secondCard);

            // Insert the new hand before the current hand
            playerHands.Insert(currentHandIndex, newHand);
        }
        
        StartCoroutine(DealCardWithDelay(newHand));
        UpdateHandPucks();
    }


    
    private void UpdateHandPucks()
    {
        for (var i = 0; i < playerHands.Count; i++)
        {
            var color = (i == currentHandIndex) ? Color.yellow : Color.clear;
            playerHands[i].SetPuckColor(color);
        }

        dealerHand.SetPuckColor(currentHandIndex >= playerHands.Count ? Color.yellow : Color.clear);
    }

    private IEnumerator DealCardWithDelay(BJHandManager hand, bool faceDown = false)
    {
        deckManager.DrawCardToHand(hand, faceDown);
        yield return new WaitForSeconds(dealDelay);
    }
}