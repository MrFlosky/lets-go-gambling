using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.HID;
using UnityEngine.UI;

public class PlayerAim : MonoBehaviour
{
    [Header("Raycast")]
    [SerializeField] private float rayDistance = 100f;

    private Clickable currentClickable;
    private Dictionary<PointerEventData.InputButton, GameObject> pressedUI = new Dictionary<PointerEventData.InputButton, GameObject>();
    private GameObject hoveredUI;

    private Camera cam;

    void Awake()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        Vector2 center = new(Screen.width * 0.5f, Screen.height * 0.5f);
        UpdateUIHover(center);
    }



    public void OnLeftClick(InputAction.CallbackContext context)
    {
        if (context.started)
            Press(PointerEventData.InputButton.Left);
        else if (context.canceled)
            Release(PointerEventData.InputButton.Left);
    }

    public void OnRightClick(InputAction.CallbackContext context)
    {
        if (context.started)
            Press(PointerEventData.InputButton.Right);
        else if (context.canceled)
            Release(PointerEventData.InputButton.Right);
    }
    
    private void Press(PointerEventData.InputButton button)
    {
        Vector2 center = new(Screen.width * 0.5f, Screen.height * 0.5f);

        PointerEventData data = new PointerEventData(EventSystem.current)
        {
            position = center,
            button = button
        };

        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(data, results);

        if (results.Count > 0)
        {
            GameObject target = results[0].gameObject;

            pressedUI[button] = target;
            ExecuteEvents.Execute(target, data, ExecuteEvents.pointerDownHandler);
            return;
        }

        // World fallback
        Ray ray = cam.ScreenPointToRay(center);
        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance))
        {
            currentClickable = hit.collider.GetComponent<Clickable>();
            currentClickable?.Press(button);
        }
    }

    
    private void Release(PointerEventData.InputButton button)
    {
        Vector2 center = new(Screen.width * 0.5f, Screen.height * 0.5f);

        if (pressedUI.TryGetValue(button, out GameObject target))
        {
            PointerEventData data = new PointerEventData(EventSystem.current)
            {
                position = center,
                button = button
            };

            ExecuteEvents.Execute(target, data, ExecuteEvents.pointerUpHandler);
            ExecuteEvents.Execute(target, data, ExecuteEvents.pointerClickHandler);

            pressedUI.Remove(button);
            return;
        }

        currentClickable?.Release(button);
        if (pressedUI.Count == 0)
            currentClickable = null;
    }

    
    private void UpdateUIHover(Vector2 screenPos)
    {
        PointerEventData data = new PointerEventData(EventSystem.current)
        {
            position = screenPos
        };

        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(data, results);

        GameObject newHovered = results.Count > 0 ? results[0].gameObject : null;

        if (newHovered == hoveredUI)
            return;

        if (hoveredUI != null)
            ExecuteEvents.Execute(hoveredUI, data, ExecuteEvents.pointerExitHandler);

        if (newHovered != null)
            ExecuteEvents.Execute(newHovered, data, ExecuteEvents.pointerEnterHandler);

        hoveredUI = newHovered;
    }

}
