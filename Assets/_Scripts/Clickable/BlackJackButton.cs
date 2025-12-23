using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BlackJackButton : ClickableUI
{
    [Header("References")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Image buttonImage;
    
    [Header("BlackJack Action")]
    [SerializeField] private UnityEvent onActivate;
    private bool callFunction = false;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        buttonImage = GetComponent<Image>();
    }

    private void Update()
    {
        bool canInteract = gameManager.IsPlayerTurn();

        buttonImage.raycastTarget = canInteract;
        buttonImage.color = canInteract ? Color.white : new Color(1,1,1,0.5f);
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        callFunction = true;
        base.OnPointerEnter(eventData);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        callFunction = false;
        base.OnPointerExit(eventData);
    }

    public override void OnDisable()
    {
        if (callFunction && gameManager.IsPlayerTurn())
        {
            onActivate?.Invoke();
        }
        callFunction = false;
        base.OnDisable();
    }
}