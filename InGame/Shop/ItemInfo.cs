using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemInfo : MonoBehaviour {

    public ShopItem.Item myItem;

    private GameObject itemTooltip;
    private Sprite iconImage;

    private float tooltipHeight = 55;
    private bool onMouseEnter = false;
    private bool selected = false;
    private bool searchSelected = false;
    [HideInInspector]
    public bool viewSelected = false;

    private GameObject shopCanvas = null;
    private GameObject uiItemTooltip;

    // 아이템 업그레이드 하위템 검색용
    [HideInInspector]
    public bool[] search = new bool[6] { false, false, false, false, false, false };
    private bool subitemCheck1 = false;
    private bool subitemCheck2 = false;
    private bool subitemCheck3 = false;
    private bool[,] subSubitemCheck = new bool[3, 3];

    private void Start()
    {
        shopCanvas = GameObject.FindGameObjectWithTag("ShopCanvas");

        if (myItem != null)
            iconImage = Resources.Load<Sprite>("Item_Image/" + myItem.iconName);

        if (shopCanvas != null)
            itemTooltip = shopCanvas.GetComponent<Shop>().itemTooltip;

        GameObject UICanvasObject = GameObject.FindGameObjectWithTag("UICanvas");
        if (UICanvasObject != null)
        {
            UICanvas UIcanvas = UICanvasObject.GetComponent<UICanvas>();
            uiItemTooltip = UIcanvas.itemTooltip;
        }
    }

    private void Update()
    {
        if (onMouseEnter)
        {
            itemTooltip.transform.position = Input.mousePosition;
            Vector2 TooltipPos = Input.mousePosition;

            // 툴팁이 화면 위를 넘어가면 아래로 뒤집음
            if (itemTooltip.GetComponent<RectTransform>().localPosition.y + itemTooltip.GetComponent<RectTransform>().sizeDelta.y > 340)
                TooltipPos.y -= (5 + itemTooltip.GetComponent<RectTransform>().sizeDelta.y) * Screen.height / 720;
            else
                TooltipPos.y += 5 * Screen.height / 720;

            // 툴팁이 화면 오른쪽을 넘어가면 왼쪽으로 뒤집음
            if (itemTooltip.GetComponent<RectTransform>().localPosition.x + itemTooltip.GetComponent<RectTransform>().sizeDelta.x > 620)
                TooltipPos.x -= (10 + itemTooltip.GetComponent<RectTransform>().sizeDelta.x) * Screen.width / 1280;
            else
                TooltipPos.x += 10 * Screen.width / 1280;

            itemTooltip.transform.position = TooltipPos;
        }

        // 선택되서 테두리가 켜진 이후에 다른게 선택되면 기존 테두리 끔
        if(selected)
        {
            if (shopCanvas.GetComponent<Shop>().selectedItemID != myItem.id 
                || shopCanvas.GetComponent<Shop>().selectedObject != gameObject)
            {
                selected = false;
                transform.Find("SelectBorder").GetComponent<Image>().enabled = false;
            }
        }

        if(searchSelected)
        {
            if (shopCanvas.GetComponent<Shop>().selectedItemID != myItem.id
                || shopCanvas.GetComponent<Shop>().selectedObject != gameObject)
            {
                searchSelected = false;
                GetComponent<Outline>().effectColor = new Color(69f / 255f, 66f / 255f, 47f / 255f, 128f / 255f);
            }
        }

        if (viewSelected)
        {
            if (shopCanvas.GetComponent<Shop>().selectedItemID != myItem.id
                || shopCanvas.GetComponent<Shop>().selectedObject != gameObject)
            {
                viewSelected = false;
            }
        }
    }

    public void BasicSetting()
    {
        if (myItem != null)
        {
            iconImage = Resources.Load<Sprite>("Item_Image/" + myItem.iconName);
            transform.Find("ItemIcon").GetComponent<Image>().sprite = iconImage;

            if (myItem.price == 0)
                transform.Find("Price").GetComponent<Text>().text = "무료";
            else
                transform.Find("Price").GetComponent<Text>().text = myItem.price.ToString();
        }
    }

    // price update
    public void ItemCheck()
    {
        subitemCheck1 = false;
        subitemCheck2 = false;
        subitemCheck3 = false;

        System.Array.Clear(subSubitemCheck, 0, subSubitemCheck.Length);
        System.Array.Clear(search, 0, search.Length);

        int priceminus = 0;

        for (int i=0; i<6; i++)
        {
            // 해당슬롯에 아이템이 없으면 다음으로
            if (PlayerData.Instance.item[i] == 0)
                continue;

            // 하위템1을 체크안했으면 체크함. 해서 true가 되었으면 넘어가질 것
            if(subitemCheck1 == false)
            {
                if(myItem.subitemID1 == PlayerData.Instance.item[i] && myItem.subitemID1 != 0)
                {
                    subitemCheck1 = true;
                    search[i] = true;
                    priceminus += ShopItem.Instance.itemlist[myItem.subitemID1].price;
                    continue;
                }
                else if (myItem.subitemID1 != 0)
                {
                    ShopItem.Item myItemSub1 = ShopItem.Instance.itemlist[myItem.subitemID1];

                    if(subSubitemCheck[0, 0] == false)
                    {
                        if (myItemSub1.subitemID1 == PlayerData.Instance.item[i] && myItemSub1.subitemID1 != 0)
                        {
                            subSubitemCheck[0, 0] = true;
                            search[i] = true;
                            priceminus += ShopItem.Instance.itemlist[myItemSub1.subitemID1].price;
                            continue;
                        }
                    }
                    if (subSubitemCheck[0, 1] == false)
                    {
                        if (myItemSub1.subitemID2 == PlayerData.Instance.item[i] && myItemSub1.subitemID2 != 0)
                        {
                            subSubitemCheck[0, 1] = true;
                            search[i] = true;
                            priceminus += ShopItem.Instance.itemlist[myItemSub1.subitemID2].price;
                            continue;
                        }
                    }
                    if (subSubitemCheck[0, 2] == false)
                    {
                        if (myItemSub1.subitemID3 == PlayerData.Instance.item[i] && myItemSub1.subitemID3 != 0)
                        {
                            subSubitemCheck[0, 2] = true;
                            search[i] = true;
                            priceminus += ShopItem.Instance.itemlist[myItemSub1.subitemID3].price;
                            continue;
                        }
                    }
                }
            }

            // 하위템2을 체크안했으면 체크함. 해서 true가 되었으면 넘어가질 것
            if (subitemCheck2 == false)
            {
                if (myItem.subitemID2 == PlayerData.Instance.item[i] && myItem.subitemID2 != 0)
                {
                    subitemCheck2 = true;
                    search[i] = true;
                    priceminus += ShopItem.Instance.itemlist[myItem.subitemID2].price;
                    continue;
                }
                else if(myItem.subitemID2 != 0)
                {
                    ShopItem.Item myItemSub2 = ShopItem.Instance.itemlist[myItem.subitemID2];

                    if (subSubitemCheck[1, 0] == false)
                    {
                        if (myItemSub2.subitemID1 == PlayerData.Instance.item[i] && myItemSub2.subitemID1 != 0)
                        {
                            subSubitemCheck[1, 0] = true;
                            search[i] = true;
                            priceminus += ShopItem.Instance.itemlist[myItemSub2.subitemID1].price;
                            continue;
                        }
                    }
                    if (subSubitemCheck[1, 1] == false)
                    {
                        if (myItemSub2.subitemID2 == PlayerData.Instance.item[i] && myItemSub2.subitemID2 != 0)
                        {
                            subSubitemCheck[1, 1] = true;
                            search[i] = true;
                            priceminus += ShopItem.Instance.itemlist[myItemSub2.subitemID2].price;
                            continue;
                        }
                    }
                    if (subSubitemCheck[1, 2] == false)
                    {
                        if (myItemSub2.subitemID3 == PlayerData.Instance.item[i] && myItemSub2.subitemID3 != 0)
                        {
                            subSubitemCheck[1, 2] = true;
                            search[i] = true;
                            priceminus += ShopItem.Instance.itemlist[myItemSub2.subitemID3].price;
                            continue;
                        }
                    }
                }
            }

            // 하위템3을 체크안했으면 체크함. 해서 true가 되었으면 넘어가질 것
            if (subitemCheck3 == false)
            {
                if (myItem.subitemID3 == PlayerData.Instance.item[i] && myItem.subitemID3 != 0)
                {
                    subitemCheck3 = true;
                    search[i] = true;
                    priceminus += ShopItem.Instance.itemlist[myItem.subitemID3].price;
                    continue;
                }
                else if(myItem.subitemID3 != 0)
                {
                    ShopItem.Item myItemSub3 = ShopItem.Instance.itemlist[myItem.subitemID3];

                    if (subSubitemCheck[2, 0] == false)
                    {
                        if (myItemSub3.subitemID1 == PlayerData.Instance.item[i] && myItemSub3.subitemID1 != 0)
                        {
                            subSubitemCheck[2, 0] = true;
                            search[i] = true;
                            priceminus += ShopItem.Instance.itemlist[myItemSub3.subitemID1].price;
                            continue;
                        }
                    }
                    if (subSubitemCheck[2, 1] == false)
                    {
                        if (myItemSub3.subitemID2 == PlayerData.Instance.item[i] && myItemSub3.subitemID2 != 0)
                        {
                            subSubitemCheck[2, 1] = true;
                            search[i] = true;
                            priceminus += ShopItem.Instance.itemlist[myItemSub3.subitemID2].price;
                            continue;
                        }
                    }
                    if (subSubitemCheck[2, 2] == false)
                    {
                        if (myItemSub3.subitemID3 == PlayerData.Instance.item[i] && myItemSub3.subitemID3 != 0)
                        {
                            subSubitemCheck[2, 2] = true;
                            search[i] = true;
                            priceminus += ShopItem.Instance.itemlist[myItemSub3.subitemID3].price;
                            continue;
                        }
                    }
                }
            }
        }

        myItem.price = ShopItem.Instance.itemlist[myItem.id].price - priceminus;
        if (myItem.price != 0)
            transform.Find("Price").GetComponent<Text>().text = myItem.price.ToString();
        else
            transform.Find("Price").GetComponent<Text>().text = "무료";
    }

    // material check
    public void MaterialCheck()
    {
        Shop shop = GameObject.FindGameObjectWithTag("ShopCanvas").GetComponent<Shop>();

        if (subitemCheck1 && shop.subitem1[0] != null)
            shop.subitem1[0].transform.Find("CheckImage").gameObject.SetActive(true);
        if (subitemCheck2 && shop.subitem1[1] != null)
            shop.subitem1[1].transform.Find("CheckImage").gameObject.SetActive(true);
        if (subitemCheck3 && shop.subitem1[2] != null)
            shop.subitem1[2].transform.Find("CheckImage").gameObject.SetActive(true);

        for(int i=0; i<3; i++)
        {
            for(int j=0; j<3; j++)
            {
                if(subSubitemCheck[i,j] && shop.subitem2[i, j] != null)
                    shop.subitem2[i,j].transform.Find("CheckImage").gameObject.SetActive(true);
            }
        }
    }

    public void TooltipOn()
    {
        if (itemTooltip != null)
        {
            itemTooltip.SetActive(true);
            onMouseEnter = true;

            tooltipHeight = 55;

            //아이콘, 이름, 가격 세팅
            itemTooltip.transform.GetChild(0).GetComponent<Image>().sprite = iconImage;
            itemTooltip.transform.GetChild(1).GetComponent<Text>().text = myItem.name;
            if (myItem.price == 0)
                itemTooltip.transform.GetChild(3).GetComponent<Text>().text = "무료";
            else
                itemTooltip.transform.GetChild(3).GetComponent<Text>().text = myItem.price.ToString();

            //스탯체크하여 존재하는 스탯만 출력
            string stat_string = "";
            if (myItem.attackDamage != 0)
                stat_string += "공격력 +" + myItem.attackDamage.ToString() + "\n";
            if (myItem.attackSpeed != 0)
                stat_string += "공격 속도 +" + myItem.attackSpeed.ToString() + "%\n";
            if (myItem.criticalPercent != 0)
                stat_string += "치명타 확률 +" + myItem.criticalPercent.ToString() + "%\n";
            if (myItem.lifeSteal != 0)
                stat_string += "생명력 흡수 +" + myItem.lifeSteal.ToString() + "%\n";
            if (myItem.abilityPower != 0)
                stat_string += "주문력 +" + myItem.abilityPower.ToString() + "\n";
            if (myItem.mana != 0)
                stat_string += "마나 +" + myItem.mana.ToString() + "\n";
            if (myItem.manaRegen != 0)
                stat_string += "기본 마나 재생 +" + myItem.manaRegen.ToString() + "%\n";
            if (myItem.cooldownReduce != 0)
                stat_string += "재사용 대기시간 감소 +" + myItem.cooldownReduce.ToString() + "%\n";
            if (myItem.armor != 0)
                stat_string += "방어력 +" + myItem.armor.ToString() + "\n";
            if (myItem.magicResist != 0)
                stat_string += "마법 저항력 +" + myItem.magicResist.ToString() + "\n";
            if (myItem.health != 0)
                stat_string += "체력 +" + myItem.health.ToString() + "\n";
            if (myItem.healthRegen != 0)
                stat_string += "기본 체력 재생 +" + myItem.healthRegen.ToString() + "%\n";
            if (myItem.movementSpeed != 0)
                stat_string += "이동 속도 +" + myItem.movementSpeed.ToString() + "\n";

            itemTooltip.transform.GetChild(4).GetComponent<Text>().text = stat_string;

            if (stat_string != string.Empty)
            {
                int stat_lineCount = stat_string.Split('\n').Length - 1;
                tooltipHeight += 15.0f * stat_lineCount;
            }

            //효과 출력
            if (myItem.effect_description != string.Empty)
            {
                // 스탯과 효과사이 간격
                if (stat_string != string.Empty)
                    tooltipHeight += 8.0f;

                // 효과종류와 설명의 위치를 스탯 밑으로
                itemTooltip.transform.GetChild(5).GetComponent<RectTransform>().anchoredPosition = 
                    new Vector2(itemTooltip.transform.GetChild(5).GetComponent<RectTransform>().anchoredPosition.x,
                        -tooltipHeight);

                // 텍스트를 갱신
                if (myItem.effect_kind != string.Empty)
                {
                    itemTooltip.transform.GetChild(5).GetComponent<Text>().text =
                        "<color=#FFEB17>" + myItem.effect_kind + ":</color> " + myItem.effect_description;
                }
                else
                    itemTooltip.transform.GetChild(5).GetComponent<Text>().text = myItem.effect_description;

                // 자동으로 줄바꿈되는경우 cachedTextGenerator를 통해 줄바꿈결과를 받을수 있으나 다음 프레임이되야 갱신됨.
                // 그래서 Canvas.ForceUpdateCanvases() 함수를 통해 이전까지의 결과를 미리 업데이트하고 반영된 데이터를 얻음.
                Canvas.ForceUpdateCanvases();
                int description_lineCount = itemTooltip.transform.GetChild(5).GetComponent<Text>().cachedTextGenerator.lineCount;
                tooltipHeight += 15.0f * description_lineCount;

                // 추가효과가 없는경우 공백으로.
                itemTooltip.transform.GetChild(6).GetComponent<Text>().text = "";

                // 추가효과가 있다면 효과와 마찬가지로 출력
                if (myItem.additional_description != string.Empty)
                {
                    tooltipHeight += 8.0f;

                    itemTooltip.transform.GetChild(6).GetComponent<RectTransform>().anchoredPosition =
                    new Vector2(itemTooltip.transform.GetChild(6).GetComponent<RectTransform>().anchoredPosition.x,
                        -tooltipHeight);

                    itemTooltip.transform.GetChild(6).GetComponent<Text>().text =
                        "<color=#FFEB17>" + myItem.additional_kind + ":</color> " + myItem.additional_description;

                    Canvas.ForceUpdateCanvases();
                    int additional_lineCount = itemTooltip.transform.GetChild(6).GetComponent<Text>().cachedTextGenerator.lineCount;
                    tooltipHeight += 15.0f * additional_lineCount;
                }
            }
            else
            {
                itemTooltip.transform.GetChild(5).GetComponent<Text>().text = "";
                itemTooltip.transform.GetChild(6).GetComponent<Text>().text = "";
            }

            //전체 길이에 맞게 크기조정
            tooltipHeight += 5.0f;
            itemTooltip.GetComponent<RectTransform>().sizeDelta = new Vector2(itemTooltip.GetComponent<RectTransform>().sizeDelta.x, tooltipHeight);
        }
    }

    public void TooltipOff()
    {
        if (itemTooltip != null)
            itemTooltip.SetActive(false);

        onMouseEnter = false;
    }

    public void ItemSelect()
    {
        if (myItem != null)
            shopCanvas.GetComponent<Shop>().ItemSelect(myItem.id, gameObject);
    }

    public void MaterialItemSelect()
    {
        if (myItem != null)
            shopCanvas.GetComponent<Shop>().MaterialItemSelect(myItem.id, gameObject);
    }

    public void ItemSelectBorder()
    {
        selected = true;
        
        GameObject sb = transform.Find("SelectBorder").gameObject;
        if (sb.GetComponent<Image>().enabled == false)
            sb.GetComponent<Image>().enabled = true;
    }

    public void SearchItemSelectBorder()
    {
        searchSelected = true;

        GetComponent<Outline>().effectColor = new Color(46f / 255f, 159f / 255f, 131f / 255f, 128f / 255f);
    }

    public void ItemViewMouseEnter()
    {
        if (myItem != null)
        {
            TooltipOn();
            if (itemTooltip != null)
            {
                iconImage = Resources.Load<Sprite>("Item_Image/" + myItem.iconName);
                itemTooltip.transform.GetChild(0).GetComponent<Image>().sprite = iconImage;
                itemTooltip.transform.GetChild(3).GetComponent<Text>().text = "판매 가격: " + (myItem.price * 0.7f).ToString();
            }
        }
    }

    public void ItemViewMouseExit()
    {
        TooltipOff();
    }

    public void ItemViewSelect(int ViewNum)
    {
        ItemSelect();

        if (myItem != null)
        {
            shopCanvas.GetComponent<Shop>().MyItemSelect(ViewNum);

            viewSelected = true;
        }
    }

    public void UITooltipOn()
    {
        if (myItem != null)
        {
            if (uiItemTooltip != null)
            {
                uiItemTooltip.SetActive(true);

                float tooltip_height = 30;
                uiItemTooltip.transform.Find("ItemName").GetComponent<Text>().text = myItem.name;
                uiItemTooltip.transform.Find("SellPrice").GetComponent<Text>().text = "판매 가격: <color=#E4B803>" + (myItem.price * 0.7f).ToString() + "</color>";

                //스탯체크하여 존재하는 스탯만 출력
                string stat_string = "";
                if (myItem.attackDamage != 0)
                    stat_string += "공격력 +" + myItem.attackDamage.ToString() + "\n";
                if (myItem.attackSpeed != 0)
                    stat_string += "공격 속도 +" + myItem.attackSpeed.ToString() + "%\n";
                if (myItem.criticalPercent != 0)
                    stat_string += "치명타 확률 +" + myItem.criticalPercent.ToString() + "%\n";
                if (myItem.lifeSteal != 0)
                    stat_string += "생명력 흡수 +" + myItem.lifeSteal.ToString() + "%\n";
                if (myItem.abilityPower != 0)
                    stat_string += "주문력 +" + myItem.abilityPower.ToString() + "\n";
                if (myItem.mana != 0)
                    stat_string += "마나 +" + myItem.mana.ToString() + "\n";
                if (myItem.manaRegen != 0)
                    stat_string += "기본 마나 재생 +" + myItem.criticalPercent.ToString() + "%\n";
                if (myItem.cooldownReduce != 0)
                    stat_string += "재사용 대기시간 감소 +" + myItem.cooldownReduce.ToString() + "%\n";
                if (myItem.armor != 0)
                    stat_string += "방어력 +" + myItem.armor.ToString() + "\n";
                if (myItem.magicResist != 0)
                    stat_string += "마법 저항력 +" + myItem.magicResist.ToString() + "\n";
                if (myItem.health != 0)
                    stat_string += "체력 +" + myItem.health.ToString() + "\n";
                if (myItem.healthRegen != 0)
                    stat_string += "기본 체력 재생 +" + myItem.healthRegen.ToString() + "%\n";
                if (myItem.movementSpeed != 0)
                    stat_string += "이동 속도 +" + myItem.movementSpeed.ToString() + "\n";

                uiItemTooltip.transform.Find("Status").GetComponent<Text>().text = stat_string;

                if (stat_string != string.Empty)
                {
                    tooltip_height += 5.0f;

                    int stat_lineCount = stat_string.Split('\n').Length - 1;
                    tooltip_height += 15.0f * stat_lineCount;
                }

                //효과 출력
                if (myItem.effect_description != string.Empty)
                {
                    uiItemTooltip.transform.Find("Effect_Description").GetComponent<RectTransform>().anchoredPosition =
                        new Vector3(uiItemTooltip.transform.Find("Effect_Description").GetComponent<RectTransform>().anchoredPosition.x, -tooltip_height);

                    // 텍스트를 갱신
                    if (myItem.effect_kind != string.Empty)
                    {
                        uiItemTooltip.transform.Find("Effect_Description").GetComponent<Text>().text =
                            "<color=#FFEB17>" + myItem.effect_kind + ":</color> " + myItem.effect_description;
                    }
                    else
                        uiItemTooltip.transform.Find("Effect_Description").GetComponent<Text>().text = myItem.effect_description;

                    Canvas.ForceUpdateCanvases();
                    int description_lineCount = uiItemTooltip.transform.Find("Effect_Description").GetComponent<Text>().cachedTextGenerator.lineCount;
                    tooltip_height += 15.0f * description_lineCount;

                    // 추가효과가 없는경우 공백으로.
                    uiItemTooltip.transform.Find("Additional_Description").GetComponent<Text>().text = "";

                    // 추가효과가 있다면 효과와 마찬가지로 출력
                    if (myItem.additional_description != string.Empty)
                    {
                        tooltip_height += 8.0f;

                        uiItemTooltip.transform.Find("Additional_Description").GetComponent<RectTransform>().anchoredPosition =
                            new Vector2(uiItemTooltip.transform.Find("Additional_Description").GetComponent<RectTransform>().anchoredPosition.x, -tooltip_height);

                        uiItemTooltip.transform.Find("Additional_Description").GetComponent<Text>().text =
                            "<color=#FFEB17>" + myItem.additional_kind + ":</color> " + myItem.additional_description;

                        Canvas.ForceUpdateCanvases();
                        int additional_lineCount = uiItemTooltip.transform.Find("Additional_Description").GetComponent<Text>().cachedTextGenerator.lineCount;
                        tooltip_height += 15.0f * additional_lineCount;
                    }
                }
                else
                {
                    uiItemTooltip.transform.Find("Effect_Description").GetComponent<Text>().text = "";
                    uiItemTooltip.transform.Find("Additional_Description").GetComponent<Text>().text = "";
                }

                tooltip_height += 5.0f;
                uiItemTooltip.GetComponent<RectTransform>().sizeDelta = new Vector2(uiItemTooltip.GetComponent<RectTransform>().sizeDelta.x, tooltip_height);
            }
        }
    }

    public void UITooltipOff()
    {
        if (uiItemTooltip != null)
            uiItemTooltip.SetActive(false);
    }
}
