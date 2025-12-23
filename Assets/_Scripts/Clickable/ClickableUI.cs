using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ClickableUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Left Click")]
    [SerializeField] private UnityEvent onLeftPress;
    [SerializeField] private UnityEvent onLeftRelease;

    [Header("Right Click")]
    [SerializeField] private UnityEvent onRightPress;
    [SerializeField] private UnityEvent onRightRelease;

    [Header("Hover")]
    [SerializeField] private UnityEvent onHoverEnter;
    [SerializeField] private UnityEvent onHoverExit;
    
    [Header("Enable/Disable")]
    [SerializeField] private UnityEvent onEnable;
    [SerializeField] private UnityEvent onDisable;

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        switch (eventData.button)
        {
            case PointerEventData.InputButton.Left:
                onLeftPress?.Invoke();
                break;

            case PointerEventData.InputButton.Right:
                onRightPress?.Invoke();
                break;
        }
    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
        switch (eventData.button)
        {
            case PointerEventData.InputButton.Left:
                onLeftRelease?.Invoke();
                break;

            case PointerEventData.InputButton.Right:
                onRightRelease?.Invoke();
                break;
        }
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        onHoverEnter?.Invoke();
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        onHoverExit?.Invoke();
    }
    
    public virtual void OnEnable()
    {
        onEnable?.Invoke();
    }

    public virtual void OnDisable()
    {
        onDisable?.Invoke();
    }
}