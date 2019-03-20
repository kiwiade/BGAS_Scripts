using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MouseOverImage : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IScrollHandler {

    private Image myImage;
    private Color myColor;
    private Color mouseoverColor;

    private ScrollRect searchScroll;

	void Start () {
        myImage = GetComponent<Image>();
        myColor = myImage.color;

        mouseoverColor = new Color(43f/255f, 96f/255f, 93f/255f, 60f/255f);

        searchScroll = GameObject.FindGameObjectWithTag("SearchView").GetComponent<ScrollRect>();
    }

    public void PointerEnter()
    {
        myImage.color = mouseoverColor;
    }

    public void PointerExit()
    {
        myImage.color = myColor;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        searchScroll.OnBeginDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        searchScroll.OnDrag(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        searchScroll.OnEndDrag(eventData);
    }

    public void OnScroll(PointerEventData eventData)
    {
        searchScroll.OnScroll(eventData);
    }
}
