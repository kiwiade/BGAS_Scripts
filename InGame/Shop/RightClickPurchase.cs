using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RightClickPurchase : MonoBehaviour, IPointerClickHandler
{
    private bool oneClicked = false;
    private float firstClickTime = 0;
    private readonly float doubleClickCheckInterval = 0.4f;

    void Update() {
        DoubleClickCheck();
    }

    private void DoubleClickCheck()
    {
        if (oneClicked)
        {
            if ((Time.time - firstClickTime) > doubleClickCheckInterval)
                oneClicked = false;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            GameObject.FindGameObjectWithTag("ShopCanvas").GetComponent<Shop>().ItemPurchase();
        }

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            // 첫클릭하면 변수 true, 첫클릭한 시간체크
            if (!oneClicked)
            {
                oneClicked = true;
                firstClickTime = Time.time;
            }
            // 더블클릭이면
            else
            {
                oneClicked = false;

                GameObject.FindGameObjectWithTag("ShopCanvas").GetComponent<Shop>().ItemPurchase();
            }
        }    
    }
}
