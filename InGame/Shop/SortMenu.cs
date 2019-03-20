using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SortMenu : MonoBehaviour {

    [SerializeField]
    private int menu_id = 0;
    [SerializeField]
    private string menu_description = "";

    private bool onMouseEnter = false;
    private GameObject sortTooltip;

    void Start () {
        if (GameObject.FindGameObjectWithTag("ShopCanvas") != null)
            sortTooltip = GameObject.FindGameObjectWithTag("ShopCanvas").GetComponent<Shop>().sortTooltip;
    }
	
	void Update () {
		if(onMouseEnter)
        {
            sortTooltip.transform.position = Input.mousePosition;
            Vector2 TooltipPos = Input.mousePosition;

            // 툴팁이 화면 위를 넘어가면 아래로 뒤집음
            if (sortTooltip.GetComponent<RectTransform>().localPosition.y + sortTooltip.GetComponent<RectTransform>().sizeDelta.y > 340)
                TooltipPos.y -= (5 + sortTooltip.GetComponent<RectTransform>().sizeDelta.y) * Screen.height / 720;
            else
                TooltipPos.y += 5 * Screen.height / 720;

            // 툴팁이 화면 오른쪽을 넘어가면 왼쪽으로 뒤집음
            if (sortTooltip.GetComponent<RectTransform>().localPosition.x + sortTooltip.GetComponent<RectTransform>().sizeDelta.x > 620)
                TooltipPos.x -= (10 + sortTooltip.GetComponent<RectTransform>().sizeDelta.x) * Screen.width / 1280;
            else
                TooltipPos.x += 10 * Screen.width / 1280;

            sortTooltip.transform.position = TooltipPos;
        }
	}

    public void PointerEnter()
    {
        onMouseEnter = true;

        GetComponent<Image>().enabled = true;
        if (sortTooltip != null)
        {
            sortTooltip.SetActive(true);

            float tooltip_height = 30;
            sortTooltip.transform.GetChild(0).GetComponent<Text>().text = menu_description.Replace("\\n", "\n");
            Canvas.ForceUpdateCanvases();

            int description_lineCount = sortTooltip.transform.GetChild(0).GetComponent<Text>().cachedTextGenerator.lineCount;
            tooltip_height += 16.0f * (description_lineCount - 1);
            sortTooltip.GetComponent<RectTransform>().sizeDelta = new Vector2(sortTooltip.GetComponent<RectTransform>().sizeDelta.x, tooltip_height);
        }
    }

    public void PointerExit()
    {
        onMouseEnter = false;

        if (transform.parent.GetComponent<ItemSort>().selectedID != menu_id)
            GetComponent<Image>().enabled = false;

        if (sortTooltip != null)
            sortTooltip.SetActive(false);
    }

    public void PointerClick()
    {
        transform.parent.GetComponent<ItemSort>().SortRefresh(menu_id, this.gameObject);
    }
}
