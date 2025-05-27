using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DeckManager deckManager;
    [SerializeField] private BlackjackRoundManager roundManager;

    // hand prefab for each player
    [SerializeField] public GameObject handPrefab;
    // parent in which each hand is instantiated under
    [SerializeField] public Transform handsParent;

    // each player hand
    [SerializeField] private List<BJHandManager> playerHands = new List<BJHandManager>();
    // the dealer hand
    [SerializeField] private BJHandManager dealerHand;

    public void AddNewPlayerHand()
    {
        // Spawn in a new hand using prefab
        var newHandObj = Instantiate(handPrefab, handsParent);

        // get the hand's manager
        var hand = newHandObj.GetComponent<BJHandManager>();
        // if it is not null
        if (hand != null)
        {
            // set its name in hierarchy
            hand.name = $"PlayerHand_{playerHands.Count}";
            // add it total player hands
            playerHands.Add(hand);
        }
    }

    public void BeginGame()
    {
        // transfer our hands over to the round manager
        roundManager.SetHands(playerHands, dealerHand);
        // start the round
        roundManager.StartRound();
    }
}
