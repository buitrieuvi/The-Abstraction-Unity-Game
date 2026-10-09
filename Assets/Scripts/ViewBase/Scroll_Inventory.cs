using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Use instead of ScrollRect so wheel input and dragging share one controller.
[AddComponentMenu("UI/Scroll ItemCtrls")]
public class Scroll_Inventory : ScrollRect
{
    private const float WheelSmoothTime = 0.12f;
    private float targetY;
    private float wheelVelocity;
    private bool smoothingWheel;
    private bool updatingScrollbars;
    private bool dragging;
    private GridLayoutGroup grid;

    protected override void Awake()
    {
        base.Awake();
        horizontal = false;
        vertical = true;
        if (content != null)
            grid = content.GetComponent<GridLayoutGroup>();
    }

    public override void OnScroll(PointerEventData eventData)
    {
        if (!IsActive() || content == null || dragging)
            return;

        Canvas.ForceUpdateCanvases();
        float delta = eventData.scrollDelta.y;
        if (Mathf.Abs(delta) < Mathf.Abs(eventData.scrollDelta.x))
            delta = eventData.scrollDelta.x;

        if (Mathf.Approximately(delta, 0f))
            return;

        if (!smoothingWheel)
            targetY = content.anchoredPosition.y;

        StopMovement();
        // One wheel event moves one row, independent of device scroll magnitude.
        float rowHeight = grid != null
            ? Mathf.Max(1f, grid.cellSize.y + grid.spacing.y)
            : Mathf.Max(1f, scrollSensitivity);
        targetY = Mathf.Clamp(targetY - Mathf.Sign(delta) * rowHeight, 0f, MaxScrollY);
        smoothingWheel = true;
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            dragging = true;
            CancelWheel();
        }
        base.OnBeginDrag(eventData);
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            dragging = false;
        base.OnEndDrag(eventData);
    }

    protected override void LateUpdate()
    {
        UpdateColumns();

        if (smoothingWheel && content != null)
        {
            targetY = Mathf.Clamp(targetY, 0f, MaxScrollY);
            Vector2 position = content.anchoredPosition;
            position.y = Mathf.SmoothDamp(position.y, targetY, ref wheelVelocity,
                WheelSmoothTime, Mathf.Infinity, Time.unscaledDeltaTime);

            if (Mathf.Abs(position.y - targetY) < 0.1f)
            {
                position.y = targetY;
                CancelWheel();
            }

            SetContentAnchoredPosition(position);
            StopMovement();
        }

        updatingScrollbars = true;
        base.LateUpdate();
        updatingScrollbars = false;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        if (verticalScrollbar != null)
            verticalScrollbar.onValueChanged.AddListener(OnScrollbarChanged);
    }

    protected override void OnDisable()
    {
        if (verticalScrollbar != null)
            verticalScrollbar.onValueChanged.RemoveListener(OnScrollbarChanged);
        CancelWheel();
        dragging = false;
        base.OnDisable();
    }

    private float MaxScrollY => Mathf.Max(0f, content.rect.height -
        (viewport != null ? viewport.rect.height : ((RectTransform)transform).rect.height));

    private void UpdateColumns()
    {
        if (grid == null || content == null)
            return;

        float width = content.rect.width - grid.padding.horizontal;
        int columns = Mathf.Max(1, Mathf.FloorToInt(
            (width + grid.spacing.x) / Mathf.Max(1f, grid.cellSize.x + grid.spacing.x)));

        if (grid.constraint == GridLayoutGroup.Constraint.FixedColumnCount &&
            grid.constraintCount == columns)
            return;

        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = columns;
        LayoutRebuilder.ForceRebuildLayoutImmediate(content);
    }

    private void OnScrollbarChanged(float value)
    {
        if (updatingScrollbars)
            return;
        CancelWheel();
        StopMovement();
    }

    private void CancelWheel()
    {
        smoothingWheel = false;
        wheelVelocity = 0f;
    }
}
