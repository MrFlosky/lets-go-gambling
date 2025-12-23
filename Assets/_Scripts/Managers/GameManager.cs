using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private BlackJackManager blackJackManager;
    [SerializeField] private DeckManager deckManager;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] private BlackJackState state;

    [Header("Variables")] 
    [SerializeField] private float dealDelay = 0.35f;

    private void Start()
    {
        StartCoroutine(GameLoop());
    }

    private IEnumerator GameLoop()
    {
        while (true)
        {
            yield return StartCoroutine(BettingState());
            yield return StartCoroutine(DealingState());
            yield return StartCoroutine(PlayerTurnState());
            yield return StartCoroutine(DealerTurnState());
            yield return StartCoroutine(ResolutionState());
        }
    }
    
    private IEnumerator BettingState()
    {
        state = BlackJackState.Betting;
        
        yield return new WaitForSeconds(0.5f);
    }

    private IEnumerator DealingState()
    {
        state = BlackJackState.Dealing;

        yield return StartCoroutine(blackJackManager.DealHands(dealDelay));

        yield return new WaitForSeconds(1f);
    }

    private IEnumerator PlayerTurnState()
    {
        state = BlackJackState.PlayerTurn;

        blackJackManager.playerTurnDone = false;

        while (!blackJackManager.playerTurnDone)
        {
            yield return null;
        }
        
        yield return new WaitForSeconds(1f);
    }

    private IEnumerator DealerTurnState()
    {
        state = BlackJackState.DealerTurn;
        
        blackJackManager.RevealDealerSecondCard();
        
        blackJackManager.dealerTurnDone = blackJackManager.CheckEndGame();
        
        while (!blackJackManager.dealerTurnDone)
        {
            blackJackManager.DealerTurn();
            yield return new WaitForSeconds(dealDelay);
        }
        
        yield return new WaitForSeconds(2f);
    }

    private IEnumerator ResolutionState()
    {
        state = BlackJackState.Resolution;
        
        blackJackManager.CheckWin();
        
        yield return new WaitForSeconds(2f);
    }
    
    public bool IsPlayerTurn()
    {
        return state == BlackJackState.PlayerTurn;
    }

    public bool IsDealerTurn()
    {
        return state == BlackJackState.DealerTurn;
    }

}
