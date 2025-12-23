using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Clickable : MonoBehaviour
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

    public virtual void Press(PointerEventData.InputButton button)
    {
        switch (button)
        {
            case PointerEventData.InputButton.Left:
                onLeftPress?.Invoke();
                break;

            case PointerEventData.InputButton.Right:
                onRightPress?.Invoke();
                break;
        }
    }

    public virtual void Release(PointerEventData.InputButton button)
    {
        switch (button)
        {
            case PointerEventData.InputButton.Left:
                onLeftRelease?.Invoke();
                break;

            case PointerEventData.InputButton.Right:
                onRightRelease?.Invoke();
                break;
        }
    }

    public virtual void HoverEnter()
    {
        onHoverEnter?.Invoke();
    }

    public virtual void HoverExit()
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