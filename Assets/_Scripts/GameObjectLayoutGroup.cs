using UnityEngine;

public class GameObjectLayoutGroup : MonoBehaviour
{
    // define options for how we are laying out our Objects
    private enum LayoutAxis
    {
        Horizontal,
        Vertical
    }

    // how we are laying out our objects
    [SerializeField] private LayoutAxis layoutAxis = LayoutAxis.Horizontal;
    
    [Tooltip("Maximum distance the layout group will span (center to edge is half of this).")]
    [SerializeField] private float maxDistance = 10f;

    void LateUpdate()
    {
        // constantly try to update children's positions
        ArrangeChildren();
    }

    private void ArrangeChildren()
    {
        // get total number of children
        var childCount = transform.childCount;
        if (childCount == 0) return;
        
        // this is the max distance we can move
        var totalSpan = Mathf.Min(maxDistance, maxDistance);

        // If only one child, position it at the center
        if (childCount == 1)
        {
            // set it in the center
            transform.GetChild(0).localPosition = Vector3.zero;
            return;
        }

        // Calculate dynamic spacing so that children fit within maxDistance
        var spacing = totalSpan / (childCount - 1);
        var startOffset = totalSpan / 2f;

        // for each child
        for (var i = 0; i < childCount; i++)
        {
            // if we are Horizontal, use offset * spacing away for X, otherwise use Y
            var newPos = layoutAxis == LayoutAxis.Horizontal
                ? new Vector3(startOffset + i * -spacing, 0f, 0f)
                : new Vector3(0f, startOffset + i * spacing, 0f);

            // then set the local position of the child at that position, to new Pos
            transform.GetChild(i).localPosition = newPos;
        }
    }
}
