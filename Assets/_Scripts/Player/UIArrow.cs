using UnityEngine;
using UnityEngine.EventSystems;

public class UIArrow : ClickableUI
{
    [Header("Refs")]
    [SerializeField] private LineRenderer line;
    [SerializeField] private Camera uiCamera;
    [SerializeField] private Canvas canvas;
    [SerializeField] private RectTransform startPoint;
    [SerializeField] private RectTransform endPoint;
    
    private RectTransform rect;
    private bool hovering;
    private bool arrowLocked;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        
        if (line != null)
        {
            line.positionCount = 2;
            line.useWorldSpace = true;
            line.enabled = false;
        }
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        hovering = true;
        if (line != null)
            line.enabled = true;
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        hovering = false;
        if (!arrowLocked && line != null)
            line.enabled = false;
    }

    private void Update()
    {
        if ((!hovering && !arrowLocked) || line == null)
        {
            return;
        }

        Vector3 startWorld = startPoint.position;

        Vector3 endWorld;
        if (endPoint != null)
        {
            endWorld = endPoint.position;
        }
        else
        {
            Vector2 screenPos = Input.mousePosition;
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, screenPos, uiCamera, out var local))
                return;

            local.x = Mathf.Clamp(local.x, rect.rect.xMin, rect.rect.xMax);
            local.y = Mathf.Clamp(local.y, rect.rect.yMin, rect.rect.yMax);
            endWorld = rect.TransformPoint(local);
        }

        line.SetPosition(0, startWorld);
        line.SetPosition(1, endWorld);
    }

    public void SetEndPoint(RectTransform endPoint)
    {
        arrowLocked = true;
        this.endPoint = endPoint;

        if (line != null)
            line.enabled = true;
    }

    public void RemoveEndPoint()
    {
        arrowLocked = false;
        endPoint = null;

        if (!hovering && line != null)
            line.enabled = false;
    }
}
