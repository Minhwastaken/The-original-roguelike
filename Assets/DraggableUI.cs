using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Item Item;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 originalPosition;
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalPosition = rectTransform.anchoredPosition;
        canvasGroup.blocksRaycasts = false; // Optional: let UI underneath be interactable
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / GetCanvasScaleFactor();
        RaycastHit2D Hit = Physics2D.Raycast(Mouse.Instance.Cam.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        //Debug.Log("Is Dragging");
        if(Hit)
        {
            Debug.Log(Hit.transform.gameObject.name);
            if(Hit.transform.gameObject.layer == 3)
            {
                //Debug.Log("Something");
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;

        if (true)
        {
            rectTransform.anchoredPosition = originalPosition;
        }
        RaycastHit2D Hit = Physics2D.Raycast(Mouse.Instance.Cam.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        if(Hit)
        {
            if(Hit.transform.gameObject.layer == 3)
            {
                //Debug.Log("Equip");
                GameManager.Instance.EquipItem(Item ,Hit.transform.gameObject.GetComponent<Unit>());
            }
        }
    }

    private float GetCanvasScaleFactor()
    {
        Canvas canvas = GetComponentInParent<Canvas>();
        return canvas ? canvas.scaleFactor : 1f;
    }
}
