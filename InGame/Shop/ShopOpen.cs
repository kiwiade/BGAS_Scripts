using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopOpen : MonoBehaviour {

    private AOSMouseCursor cursor;
    [SerializeField]
    private GameObject shopCanvas;
    private GameObject optionCanvas;
    private GameObject uiCanvas;

    private RaycastHit hit;
    private bool mouseChanged = false;

    void Start () {
        optionCanvas = GameObject.FindGameObjectWithTag("OptionCanvas");
        uiCanvas = GameObject.FindGameObjectWithTag("UICanvas");
        if (!cursor)
            cursor = GameObject.FindGameObjectWithTag("MouseCursor").GetComponent<AOSMouseCursor>();
    }
	
	void Update () {
        ShopMouseOverCheck();
    }

    private void ShopMouseOverCheck()
    {
        bool find = false;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 300.0f, 1 << LayerMask.NameToLayer("Home")))
        {
            // 옵션창이 꺼져있을때만 검사함.
            if (!optionCanvas.activeInHierarchy)
            {
                // UI부분과 겹치면 안찾음.
                bool uiCrash = false;
                if (uiCanvas != null)
                {
                    GraphicRaycaster uiGraphicRaycaster = uiCanvas.GetComponent<GraphicRaycaster>();
                    PointerEventData pointEventData = new PointerEventData(null);
                    pointEventData.position = Input.mousePosition;
                    List<RaycastResult> results = new List<RaycastResult>();
                    uiGraphicRaycaster.Raycast(pointEventData, results);
                    foreach (RaycastResult result in results)
                    {
                        if (result.gameObject.transform.GetComponentInParent<GraphicRaycaster>().Equals(uiGraphicRaycaster))
                        {
                            uiCrash = true;
                            break;
                        }
                    }
                }

                if (!uiCrash)
                {
                    // 상점이 꺼져있고 마우스가 안바꼈을때면 마우스바꿈
                    if (!shopCanvas.activeSelf && cursor && !mouseChanged)
                    {
                        if (cursor.CurrentCursor != 5)
                        {
                            cursor.SetCursor(5, Vector2.zero);
                            mouseChanged = true;
                        }
                    }
                    find = true;
                }
                // UI랑 충돌했을때 마우스가 바껴있으면 원래대로 돌림
                else
                {
                    if (mouseChanged)
                    {
                        if (cursor.CurrentCursor != cursor.PreCursor)
                        {
                            cursor.SetCursor(cursor.PreCursor, Vector2.zero);
                            mouseChanged = false;
                            find = false;
                        }
                    }
                }
            }
        }
        else
        {
            if (mouseChanged)
            {
                if (cursor.CurrentCursor != cursor.PreCursor)
                {
                    cursor.SetCursor(cursor.PreCursor, Vector2.zero);
                    mouseChanged = false;
                }
            }
        }

        // 찾았을때 클릭하면 상점 오픈
        if (Input.GetMouseButton(0) && find)
        {
            if (cursor)
            {
                if (cursor.CurrentCursor != 0)
                {
                    cursor.SetCursor(0, Vector2.zero);
                    mouseChanged = false;
                }
            }

            if (!shopCanvas.activeSelf)
                shopCanvas.SetActive(true);
        }
    }
}
