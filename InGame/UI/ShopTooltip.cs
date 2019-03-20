using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopTooltip : MonoBehaviour {

    private string titlename = "챔피언 아이템 상점";
    private string hotkey = "P";
    private string description = "아이템 상점에 가까이 있을 때만 물건을 살 수 있습니다. 상점은 소환사의 제단 근처에 있습니다.";

    private string titlename2 = "귀환";
    private string hotkey2 = "B";
    private string status = "<color=#D08005>클릭하여 사용</color>";
    private string description2 = "8초 뒤 챔피언을 소환사의 제단으로 순간이동시킵니다. 이 때 피해를 입으면 순간이동은 취소됩니다.";

    private GameObject midTooltip;
    private GameObject itemTooltip;
    
    void Start () {
        UICanvas uiCanvas = GameObject.FindGameObjectWithTag("UICanvas").GetComponent<UICanvas>();
        midTooltip = uiCanvas.tooltip;
        itemTooltip = uiCanvas.itemTooltip;
    }
	

    public void TooltipOn()
    {
        if (midTooltip != null)
        {
            midTooltip.SetActive(true);

            float tooltip_height = 30;
            midTooltip.transform.Find("TitleText").GetComponent<Text>().text = titlename;
            midTooltip.transform.Find("HotKey").GetComponent<Text>().text = "[" + hotkey + "]";
            midTooltip.transform.Find("Title_Description").GetComponent<Text>().text = description;
            midTooltip.transform.Find("Cooldown").GetComponent<Text>().text = "";
            tooltip_height += 5.0f;
            Canvas.ForceUpdateCanvases();

            int description_lineCount = midTooltip.transform.Find("Title_Description").GetComponent<Text>().cachedTextGenerator.lineCount;
            tooltip_height += 15.0f * description_lineCount;

            midTooltip.transform.Find("Line1").gameObject.SetActive(false);
            midTooltip.transform.Find("Line2").gameObject.SetActive(false);
            midTooltip.transform.Find("Additional_Description").GetComponent<Text>().text = "";
            midTooltip.transform.Find("Additional_Description2").GetComponent<Text>().text = "";

            tooltip_height += 5.0f;
            midTooltip.GetComponent<RectTransform>().sizeDelta = new Vector2(midTooltip.GetComponent<RectTransform>().sizeDelta.x, tooltip_height);
        }
    }

    public void TooltipOff()
    {
        if (midTooltip != null)
            midTooltip.SetActive(false);
    }

    public void RecallTooltipOn()
    {
        if (itemTooltip != null)
        {
            itemTooltip.SetActive(true);

            float tooltip_height = 30;
            itemTooltip.transform.Find("ItemName").GetComponent<Text>().text = titlename2;
            itemTooltip.transform.Find("SellPrice").GetComponent<Text>().text = "[" + hotkey2 + "]";
            itemTooltip.transform.Find("Status").GetComponent<Text>().text = status;
            tooltip_height += 20.0f;

            itemTooltip.transform.Find("Effect_Description").GetComponent<RectTransform>().anchoredPosition =
                        new Vector3(itemTooltip.transform.Find("Effect_Description").GetComponent<RectTransform>().anchoredPosition.x, -tooltip_height);
            itemTooltip.transform.Find("Effect_Description").GetComponent<Text>().text = description2;
            itemTooltip.transform.Find("Additional_Description").GetComponent<Text>().text = "";

            Canvas.ForceUpdateCanvases();
            int description_lineCount = itemTooltip.transform.Find("Effect_Description").GetComponent<Text>().cachedTextGenerator.lineCount;
            tooltip_height += 15.0f * description_lineCount;

            tooltip_height += 5.0f;
            itemTooltip.GetComponent<RectTransform>().sizeDelta = new Vector2(itemTooltip.GetComponent<RectTransform>().sizeDelta.x, tooltip_height);
        }
    }

    public void RecallTooltipOff()
    {
        if (itemTooltip != null)
            itemTooltip.SetActive(false);
    }
}
