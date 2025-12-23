using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackJackManager : MonoBehaviour
{
    [Header("References")]
    public DeckManager deckManager;
    public List<BlackJackHand> hands;
    private readonly List<BlackJackHand> startingHands = new();
 
    [Header("Prefabs")]
    public BlackJackHand handPrefab;
    
    [Header("Values")]
    public int currentHand;
    public bool gameStarted = false;
    public bool playerTurnDone;
    public bool dealerTurnDone;
    private int startingPlayerHands;

    private void Awake()
    {
        startingHands.Clear();
        startingHands.AddRange(hands);
    }

    public IEnumerator DealHands(float dealDelay)
    {
        var deck = deckManager.deck;
        int totalDeals = hands.Count * 2;
        gameStarted = true;
        
        // deal 2 cards to each hand
        for (var i = 0; i < totalDeals; i++)
        {
            // get index of hand
            var handIndex = i % hands.Count;
            
            // check if it needs to be face down
            var faceDown = (i == totalDeals - 1);

            // get card and remove it from deck
            CardInstance card = deck[0];
            deck.RemoveAt(0);

            // add it to hand
            hands[handIndex].DealCardToHand(card, faceDown);
            
            yield return new WaitForSeconds(dealDelay);
        }
        
        // start with first hand
        currentHand = 0;
        
        // find blackjacks
        foreach (var hand in hands)
        {
            if (hand.GetCardCount() == 2 && hand.GetHandTotal() == 21)
                hand.isBlackjack = true;
        }
    }
    
    private void DealToHand(int handIndex)
    {
        var deck = deckManager.deck;

        CardInstance card = deck[0];
        deck.RemoveAt(0);

        hands[handIndex].DealCardToHand(card);
    }

    private void DealSplitStartCard()
    {
        // dont deal to dealer
        if (currentHand >= hands.Count - 1)
            return;

        var hand = hands[currentHand];

        // check if hand has one card, give card if it does
        if (hand.isSplitHand && hand.GetCardCount() == 1 && !hand.isBusted && !hand.isStanding)
        {
            DealToHand(currentHand);
            NextPlayerIfDone();
        }
    }


    public void HitHand()
    {
        var deck = deckManager.deck;

        CardInstance card = deck[0];
        deck.RemoveAt(0);

        var hand = hands[currentHand];
        hand.DealCardToHand(card);

        NextPlayerIfDone();
    }

    public void StandHand()
    {
        hands[currentHand].isStanding = true;
        currentHand++;
        
        if (currentHand >= hands.Count - 1)
        {
            playerTurnDone = true;
        }
        NextPlayerIfDone();
    }

    public void SplitHand()
    {
        var current = hands[currentHand];

        // if hand is a pair
        if (current.HasPair())
        {
            // create new hand
            var newHand = Instantiate(handPrefab, current.transform.parent);
            newHand.transform.localScale = Vector3.one * 0.745f;
            newHand.transform.localPosition = current.transform.localPosition;
            
            // move current hand right
            current.transform.position = new Vector3(current.transform.position.x + .15f, current.transform.position.y, current.transform.position.z);
            
            // move new hand left
            newHand.transform.position = new Vector3(newHand.transform.position.x - .15f, newHand.transform.position.y, newHand.transform.position.z);

            newHand.isSplitHand = true;
            current.isSplitHand = true;
            
            // add it to total hands
            hands.Insert(currentHand + 1, newHand);

            // add card to new hand
            newHand.DealCardToHand(current.PopCard());
            
            // remove visual from current hand
            current.GetComponent<VisualizeHand>().RemoveLastCard();
            
            // deal new card to hand
            DealToHand(currentHand);

            // go to the next player if finished
            NextPlayerIfDone();
        }
    }

    public void DoubleHand()
    {
        var current = hands[currentHand];

        if (current.GetCardCount() == 2)
        {
            var deck = deckManager.deck;
            
            // deal new card to hand
            CardInstance card = deck[0];
            deck.RemoveAt(0);

            // add it to hand
            current.DealCardToHand(card);
            current.isStanding = true;

            StandHand();
        }
}

    public void DealerTurn()
    {
        var dealerHand = hands[^1];

        if (dealerHand.GetHandTotal() < 17 || (dealerHand.GetHandTotal() == 17 && dealerHand.IsSoft()))
        {
            DealToDealer();
        }
        else
        {
            dealerTurnDone = true;
        }
    }

    public void RevealDealerSecondCard()
    {
        var dealerHand = hands[^1];

        // ensure we are on 2 cards
        if (dealerHand.GetCardCount() < 2)
            return;

        dealerHand.SetCardFaceDown(1, false);
    }


    private void DealToDealer()
    {
        var deck = deckManager.deck;

        CardInstance card = deck[0];
        deck.RemoveAt(0);

        hands[^1].DealCardToHand(card);
    }

    public void CheckWin()
    {
        var dealerHand = hands[^1];
        for (var i = 1; i < hands.Count; i++)
        {
            var hand = hands[i - 1];

            // Player bust
            if (hand.isBusted)
            {
                print($"Hand {i} has lost (busted)");
                continue;
            }

            // Dealer bust
            if (dealerHand.isBusted)
            {
                print($"Hand {i} has won (dealer busted)");
                continue;
            }

            // Blackjack checks
            if (hand.isBlackjack && !dealerHand.isBlackjack)
            {
                print($"Hand {i} has won (blackjack)");
                continue;
            }

            // BlackJack
            if (!hand.isBlackjack && dealerHand.isBlackjack)
            {
                print($"Hand {i} has lost (dealer blackjack)");
                continue;
            }

            // Compare totals
            if (hand.GetHandTotal() > dealerHand.GetHandTotal())
            {
                print($"Hand {i} has won");
            }
            else if (hand.GetHandTotal() < dealerHand.GetHandTotal())
            {
                print($"Hand {i} has lost");
            }
            else
            {
                print($"Hand {i} has pushed");
            }
        }


        ResetBJ();
    }

    private void ResetBJ()
    {
        currentHand = 0;
        playerTurnDone = false;
        dealerTurnDone = false;
        for (var i = hands.Count - 1; i >= 0; i--)
        {
            var hand = hands[i];
            if (!startingHands.Contains(hand))
            {
                hands.RemoveAt(i);
                Destroy(hand.gameObject);
            }
        }

        foreach (var hand in hands)
            hand.ClearHand();
    }

    public bool CheckEndGame()
    {
        for (int i = 0; i < hands.Count - 1; i++)
        {
            var hand = hands[i];
            if (!hand.isBusted && !hand.isBlackjack)
                return false;
        }
        return true;
    }
    
    private void NextPlayerIfDone()
    {
        while (currentHand < hands.Count - 1)
        {
            var hand = hands[currentHand];

            if (!hand.isBusted && !hand.isStanding && !hand.isBlackjack)
                return;

            // finish hand
            hands[currentHand].isStanding = true;
            currentHand++;

            // give split card a card
            DealSplitStartCard();

            // player is done
            if (currentHand >= hands.Count - 1)
            {
                playerTurnDone = true;
                return;
            }
        }
    }


}

public enum BlackJackState
{
    Betting,
    Dealing,
    PlayerTurn,
    DealerTurn,
    Resolution
}

