using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemScroll : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IScrollHandler
{
    private ScrollRect itemScrollRect;

    void Start () {
        itemScrollRect = GameObject.FindGameObjectWithTag("ItemList").GetComponent<ScrollRect>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        itemScrollRect.OnBeginDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        itemScrollRect.OnDrag(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        itemScrollRect.OnEndDrag(eventData);
    }

    public void OnScroll(PointerEventData eventData)
    {
        itemScrollRect.OnScroll(eventData);
    }
}
