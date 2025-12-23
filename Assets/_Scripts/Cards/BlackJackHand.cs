using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class BlackJackHand : MonoBehaviour
{
    [Header("Hand Values")]
    public int handTotal;
    private int aceTotal;
    
    [Header("Hand Flags")]
    public bool isBusted;
    public bool isStanding;
    public bool isBlackjack;
    public bool isSplitHand;

    [Header("Cards")]
    [SerializeField] private List<CardInstance> cards;
    
    [Header("References")]
    [SerializeField] private VisualizeHand visualizeHand;

    public void DealCardToHand(CardInstance card, bool faceDown = false)
    {
        // set face down value
        card.faceDown = faceDown;
        
        // add card to hand
        cards.Add(card);
        
        // add card value to hand
        handTotal += card.value;
        
        // if ace, add 1 to ace total
        if (card.rank == CardRank.Ace)
            aceTotal++;
        
        // if hand value is greater than 21, and we have an ace, subtract 10 and remove 1 ace count
        while (handTotal > 21 && aceTotal > 0)
        {
            handTotal -= 10;
            aceTotal--;
        }

        if (handTotal > 21)
            isBusted = true;

        // blackjack check
        if (!isSplitHand && cards.Count == 2 && handTotal == 21)
            isBlackjack = true;

        // stand on 21
        if (!isBusted && handTotal == 21)
            isStanding = true;
    }

    public void ClearHand()
    {
        handTotal = 0;
        aceTotal = 0;
        isBusted = false;
        isStanding = false;
        isBlackjack = false;
        isSplitHand = false;
        cards.Clear();
        visualizeHand.ClearCards();
    }

    public int GetCardCount()
    {
        return cards.Count;
    }

    public int GetHandTotal()
    {
        return handTotal;
    }

    public int GetAceCount()
    {
        return aceTotal;
    }
    
    public bool IsSoft()
    {
        return aceTotal > 0 && handTotal <= 17;
    }

    public bool HasPair()
    {
        if (cards.Count != 2)
            return false;

        return cards[0].rank == cards[1].rank;
    }

    public CardInstance PopCard()
    {
        // get last card
        var card = cards[^1];
        
        // remove last card
        cards.RemoveAt(cards.Count - 1);
        
        Recalculate();
        
        return card;
    }
    
    public void SetCardFaceDown(int index, bool faceDown)
    {
        if (index < 0 || index >= cards.Count)
            return;

        var card = cards[index];
        card.faceDown = faceDown;
        cards[index] = card; // IMPORTANT if CardInstance is a struct

        if (visualizeHand != null)
            visualizeHand.RefreshCard(index);
    }

    
    private void Recalculate()
    {
        handTotal = 0;
        aceTotal = 0;
        isBusted = false;
        isBlackjack = false;
        
        for (int i = 0; i < cards.Count; i++)
        {
            handTotal += cards[i].value;
            if (cards[i].rank == CardRank.Ace)
                aceTotal++;
        }
        
        int softAces = aceTotal;
        while (handTotal > 21 && softAces > 0)
        {
            handTotal -= 10;
            softAces--;
        }

        // Bust check
        if (handTotal > 21)
            isBusted = true;

        // blackjack check
        if (!isSplitHand && cards.Count == 2 && handTotal == 21)
            isBlackjack = true;

        // stand on 21
        if (!isBusted && handTotal == 21)
            isStanding = true;
    }
    
    public CardInstance GetCardAt(int index)
    {
        return cards[index];
    }

}
