using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    [Header("Item Prefabs")]
    [SerializeField]
    private GameObject item;
    [SerializeField]
    private GameObject searchItem;
    [SerializeField]
    private GameObject makingItem;
    [SerializeField]
    private GameObject materialItem;

    [Space]
    [Header("Tooltips")]
    public GameObject itemTooltip;
    public GameObject sortTooltip;

    [Space]
    [Header("Contents")]
    [SerializeField]
    private GameObject itemContent;
    [SerializeField]
    private GameObject searchContent;
    [SerializeField]
    private GameObject makingContent;
    [SerializeField]
    private GameObject materialItemContent;
    [SerializeField]
    private GameObject description;

    [Space]
    [SerializeField]
    private int lineMaxItemCount = 5;
    [SerializeField]
    private int searchLineMaxCount = 3;

    [Space]
    [Header("MakingLines")]
    [SerializeField]
    private GameObject lineContent;
    [SerializeField]
    private GameObject line2_1;
    [SerializeField]
    private GameObject line2_2;
    [SerializeField]
    private GameObject line2_3;
    [SerializeField]
    private GameObject line3_2;
    [SerializeField]
    private GameObject line3_3;

    [Space]
    [Header("SelectInfo")]
    public int selectedItemID = 100;
    public GameObject selectedObject = null;
    [SerializeField]
    private bool myItemSelected = false;
    [SerializeField]
    private int selectedViewNum = 0;

    [HideInInspector]
    public GameObject[] subitem1 = new GameObject[3];
    public GameObject[,] subitem2 = new GameObject[3, 3];

    private readonly Vector3 itemStartPos = new Vector3(-130f, -5f);
    private readonly Vector3 searchItemStartPos = new Vector3(-158f, -5f);
    private readonly int itemWidth = 60;
    private readonly int itemHeight = 70;
    private readonly int searchItemWidth = 155;
    private readonly int searchItemHeight = 56;
    private readonly int makingItemWidth = 50;

    void Start()
    {
        // 기본적으로 전체 아이템 생성
        ItemCreate(0);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ShopClose();
        }
    }

    public void ShopClose()
    {
        SoundManager.Instance.PlaySound(SoundManager.Instance.UI_Close);
        gameObject.SetActive(false);
    }

    private void UIRefresh()
    {
        GameObject.FindGameObjectWithTag("ItemView").GetComponent<ItemView>().StatusUpdate();
        GameObject.FindGameObjectWithTag("ItemUI").GetComponent<ItemUI>().StatusUpdate();
    }

    private void PriceRefresh()
    {
        foreach (Transform item in itemContent.transform)
        {
            item.GetComponent<ItemInfo>().ItemCheck();
        }
        foreach (Transform item in materialItemContent.transform)
        {
            item.GetComponent<ItemInfo>().ItemCheck();
        }
    }

    #region 구매, 판매, 업그레이드, 되돌리기
    // 실제 처리는 PlayerData에서 하고 여기서는 UI 갱신만 담당
    private void ItemUpgrade()
    {
        int price = selectedObject.GetComponent<ItemInfo>().myItem.price;
        PlayerData.Instance.ItemUpgrade(selectedObject.GetComponent<ItemInfo>().search, selectedItemID, price, ShopItem.Instance.itemlist[selectedItemID].accessory);
        UIRefresh();
        PriceRefresh();
        MaterialCheck();
    }

    public void ItemPurchase()
    {
        if (selectedItemID != 100)
        {
            bool[] search = selectedObject.GetComponent<ItemInfo>().search;
            for (int i = 0; i < 6; i++)
            {
                if (search[i] == true)
                {
                    ItemUpgrade();
                    return;
                }
            }
            int price = selectedObject.GetComponent<ItemInfo>().myItem.price;
            PlayerData.Instance.ItemPurchase(selectedItemID, price, ShopItem.Instance.itemlist[selectedItemID].accessory);
        }
        UIRefresh();
        PriceRefresh();
        MaterialCheck();
    }

    public void ItemSell()
    {
        if (myItemSelected)
        {
            if (selectedItemID != 100)
            {
                PlayerData.Instance.ItemSell(selectedViewNum, selectedItemID, ShopItem.Instance.itemlist[selectedItemID].price);
                selectedItemID = 100;
                myItemSelected = false;
            }
        }
        UIRefresh();
        PriceRefresh();
        MaterialCheck();
        SoundManager.Instance.PlaySound(SoundManager.Instance.Shop_Sell);
    }

    public void ItemUndo()
    {
        // 되돌리고 판매했을때만 true로 리턴
        bool sell = PlayerData.Instance.ItemUndo();
        if (sell)
            myItemSelected = false;

        UIRefresh();
        PriceRefresh();
        MaterialCheck();
        SoundManager.Instance.PlaySound(SoundManager.Instance.Button_Click);
    }
    #endregion

    public void ItemSelect(int itemID, GameObject itemObject)
    {
        int childCount = materialItemContent.transform.childCount;

        //제작창, 재료아이템창, 설명창 변경
        selectedItemID = itemID;
        selectedObject = itemObject;

        MakingItemSearch(itemID);
        MaterialItemSearch(itemID);
        DescriptionUpdate(itemID);

        myItemSelected = false;
        selectedViewNum = 0;

        foreach (Transform item in materialItemContent.transform)
        {
            item.GetComponent<ItemInfo>().ItemCheck();
        }
        MaterialCheck(childCount);
        SoundManager.Instance.PlaySound(SoundManager.Instance.Button_Click);
    }

    public void MaterialCheck(int prevChildCount = 0)
    {
        foreach (Transform t in materialItemContent.transform)
        {
            t.Find("CheckImage").gameObject.SetActive(false);
        }

        // 구매, 판매시에 재료아이템창이 변하지않는경우는 그냥 0번을 불러줌
        // 재료아이템창이 변하는경우 아직 프레임이 지나지않아 이전 오브젝트들이 destroy되지않아
        // 0번은 삭제될 이전아이템이므로 이전자식의 개수뒤(새아이템)의 자식을 불러줘야함
        if (selectedItemID != 100)
            materialItemContent.transform.GetChild(prevChildCount).GetComponent<ItemInfo>().MaterialCheck();
    }

    public void MyItemSelect(int ViewNum)
    {
        myItemSelected = true;
        selectedViewNum = ViewNum;
    }

    public void MaterialItemSelect(int itemID, GameObject itemObject)
    {
        // 재료아이템창 빼고 다 변경
        selectedItemID = itemID;
        selectedObject = itemObject;

        MakingItemSearch(itemID);
        DescriptionUpdate(itemID);
    }

    // 제작아이템을 검색하여 제작창에 만들어주는 함수
    public void MakingItemSearch(int itemID)
    {
        ShopItem.Instance.makingItemList.Clear();
        for (int i = 0; i < ShopItem.Instance.itemlist.Count; i++)
        {
            if (ShopItem.Instance.itemlist[i + 1].subitemID1 == itemID
                || ShopItem.Instance.itemlist[i + 1].subitemID2 == itemID
                || ShopItem.Instance.itemlist[i + 1].subitemID3 == itemID)
            {
                ShopItem.Instance.makingItemList.Add(ShopItem.Instance.itemlist[i + 1]);
            }
        }

        foreach (Transform tr in makingContent.transform)
        {
            Destroy(tr.gameObject);
        }

        int count = 0;
        foreach (ShopItem.Item i in ShopItem.Instance.makingItemList)
        {
            GameObject createdItem = Instantiate(makingItem, makingContent.transform);
            createdItem.GetComponent<RectTransform>().localPosition = new Vector3(count * makingItemWidth, 0);
            count++;

            createdItem.GetComponent<ItemInfo>().myItem = i.ClassCopy();
            createdItem.GetComponent<Image>().sprite = Resources.Load<Sprite>("Item_Image/" + i.iconName);
        }
        GameObject.FindGameObjectWithTag("Making").GetComponent<MakingItem>().ItemCount(count);
    }

    // 재료 아이템 생성
    public void MaterialItemSearch(int itemID)
    {
        ShopItem.Item selectItem = ShopItem.Instance.itemlist[itemID];

        foreach (Transform tr in materialItemContent.transform)
        {
            Destroy(tr.gameObject);
        }
        foreach (Transform tr in lineContent.transform)
        {
            Destroy(tr.gameObject);
        }

        GameObject createdItem = Instantiate(materialItem, materialItemContent.transform);
        createdItem.transform.localPosition = Vector3.zero;
        createdItem.GetComponent<ItemInfo>().myItem = selectItem.ClassCopy();
        createdItem.GetComponent<ItemInfo>().BasicSetting();

        for (int i = 0; i < 3; i++)
        {
            subitem1[i] = null;
            for (int j = 0; j < 3; j++)
            {
                subitem2[i, j] = null;
            }
        }
        SubItemCreate(selectItem, 2, createdItem);
    }

    private void SubItemCreate(ShopItem.Item pItem, int floor, GameObject Parent, int parentcheckid = 100)
    {
        if (floor > 3)
            return;

        int distance2 = 0;
        int distance3 = 0;
        if (floor == 2)
        {
            distance2 = 70;
            distance3 = 100;
        }
        else if (floor == 3)
        {
            distance2 = 30;
            distance3 = 45;
        }

        int subitem_count = 0;
        if (pItem.subitemID1 != 0)
            subitem_count++;
        if (pItem.subitemID2 != 0)
            subitem_count++;
        if (pItem.subitemID3 != 0)
            subitem_count++;

        if (subitem_count > 0)
        {
            if (subitem_count == 1)
            {
                GameObject subItem1 = Instantiate(materialItem, materialItemContent.transform);
                subItem1.transform.localPosition = Parent.transform.localPosition + new Vector3(0, -60);
                subItem1.GetComponent<ItemInfo>().myItem = ShopItem.Instance.itemlist[pItem.subitemID1].ClassCopy();
                subItem1.GetComponent<ItemInfo>().BasicSetting();

                SubItemCreate(ShopItem.Instance.itemlist[pItem.subitemID1], floor + 1, subItem1, 0);

                GameObject subLine = Instantiate(line2_1, lineContent.transform);
                subLine.transform.localPosition = Parent.transform.localPosition + new Vector3(0, -52);

                if (floor == 2)
                    subitem1[0] = subItem1;
                else if (floor == 3)
                    subitem2[parentcheckid, 0] = subItem1;
            }
            else if (subitem_count == 2)
            {
                GameObject subItem1 = Instantiate(materialItem, materialItemContent.transform);
                subItem1.transform.localPosition = Parent.transform.localPosition + new Vector3(-distance2, -60);
                subItem1.GetComponent<ItemInfo>().myItem = ShopItem.Instance.itemlist[pItem.subitemID1].ClassCopy();
                subItem1.GetComponent<ItemInfo>().BasicSetting();

                SubItemCreate(ShopItem.Instance.itemlist[pItem.subitemID1], floor + 1, subItem1, 0);


                GameObject subItem2 = Instantiate(materialItem, materialItemContent.transform);
                subItem2.transform.localPosition = Parent.transform.localPosition + new Vector3(distance2, -60);
                subItem2.GetComponent<ItemInfo>().myItem = ShopItem.Instance.itemlist[pItem.subitemID2].ClassCopy();
                subItem2.GetComponent<ItemInfo>().BasicSetting();

                SubItemCreate(ShopItem.Instance.itemlist[pItem.subitemID2], floor + 1, subItem2, 1);


                GameObject subLine = null;
                if (floor == 2)
                    subLine = Instantiate(line2_2, lineContent.transform);
                else if (floor == 3)
                    subLine = Instantiate(line3_2, lineContent.transform);

                subLine.transform.localPosition = Parent.transform.localPosition + new Vector3(0, -52);

                if (floor == 2)
                {
                    subitem1[0] = subItem1;
                    subitem1[1] = subItem2;
                }
                else if (floor == 3)
                {
                    subitem2[parentcheckid, 0] = subItem1;
                    subitem2[parentcheckid, 1] = subItem2;
                }
            }
            else if (subitem_count == 3)
            {
                GameObject subItem1 = Instantiate(materialItem, materialItemContent.transform);
                subItem1.transform.localPosition = Parent.transform.localPosition + new Vector3(-distance3, -60);
                subItem1.GetComponent<ItemInfo>().myItem = ShopItem.Instance.itemlist[pItem.subitemID1].ClassCopy();
                subItem1.GetComponent<ItemInfo>().BasicSetting();

                SubItemCreate(ShopItem.Instance.itemlist[pItem.subitemID1], floor + 1, subItem1, 0);


                GameObject subItem2 = Instantiate(materialItem, materialItemContent.transform);
                subItem2.transform.localPosition = Parent.transform.localPosition + new Vector3(0, -60);
                subItem2.GetComponent<ItemInfo>().myItem = ShopItem.Instance.itemlist[pItem.subitemID2].ClassCopy();
                subItem2.GetComponent<ItemInfo>().BasicSetting();

                SubItemCreate(ShopItem.Instance.itemlist[pItem.subitemID2], floor + 1, subItem2, 1);


                GameObject subItem3 = Instantiate(materialItem, materialItemContent.transform);
                subItem3.transform.localPosition = Parent.transform.localPosition + new Vector3(distance3, -60);
                subItem3.GetComponent<ItemInfo>().myItem = ShopItem.Instance.itemlist[pItem.subitemID3].ClassCopy();
                subItem3.GetComponent<ItemInfo>().BasicSetting();

                SubItemCreate(ShopItem.Instance.itemlist[pItem.subitemID3], floor + 1, subItem3, 2);


                GameObject subLine = null;
                if (floor == 2)
                    subLine = Instantiate(line2_3, lineContent.transform);
                else if (floor == 3)
                    subLine = Instantiate(line3_3, lineContent.transform);

                subLine.transform.localPosition = Parent.transform.localPosition + new Vector3(0, -52);

                if (floor == 2)
                {
                    subitem1[0] = subItem1;
                    subitem1[1] = subItem2;
                    subitem1[2] = subItem3;
                }
                else if (floor == 3)
                {
                    subitem2[parentcheckid, 0] = subItem1;
                    subitem2[parentcheckid, 1] = subItem2;
                    subitem2[parentcheckid, 2] = subItem3;
                }
            }
        }
    }

    public void DescriptionUpdate(int itemID)
    {
        // 처음 실행될때 항목들을 켜줌 (처음에는 선택이 없으니 꺼져있을테니)
        description.transform.Find("ItemIcon").GetComponent<Image>().enabled = true;
        description.transform.Find("ItemName").GetComponent<Text>().enabled = true;
        description.transform.Find("GoldImage").GetComponent<Image>().enabled = true;
        description.transform.Find("Price").GetComponent<Text>().enabled = true;
        description.transform.Find("Status").GetComponent<Text>().enabled = true;
        description.transform.Find("Effect_Description").GetComponent<Text>().enabled = true;
        description.transform.Find("Additional_Description").GetComponent<Text>().enabled = true;

        ShopItem.Item selectItem = ShopItem.Instance.itemlist[itemID];
        // 아이콘 이름 가격
        Sprite Icon_image = Resources.Load<Sprite>("Item_Image/" + selectItem.iconName);
        description.transform.Find("ItemIcon").GetComponent<Image>().sprite = Icon_image;
        description.transform.Find("ItemName").GetComponent<Text>().text = selectItem.name;

        if (ShopItem.Instance.itemlist[itemID].price == 0)
            description.transform.Find("Price").GetComponent<Text>().text = "무료";
        else
            description.transform.Find("Price").GetComponent<Text>().text = selectItem.price.ToString();

        //스탯체크
        float height = 68;

        string stat_string = "";
        if (selectItem.attackDamage != 0)
            stat_string += "공격력 +" + selectItem.attackDamage.ToString() + "\n";
        if (selectItem.attackSpeed != 0)
            stat_string += "공격 속도 +" + selectItem.attackSpeed.ToString() + "%\n";
        if (selectItem.criticalPercent != 0)
            stat_string += "치명타 확률 +" + selectItem.criticalPercent.ToString() + "%\n";
        if (selectItem.lifeSteal != 0)
            stat_string += "생명력 흡수 +" + selectItem.lifeSteal.ToString() + "%\n";
        if (selectItem.abilityPower != 0)
            stat_string += "주문력 +" + selectItem.abilityPower.ToString() + "\n";
        if (selectItem.mana != 0)
            stat_string += "마나 +" + selectItem.mana.ToString() + "\n";
        if (selectItem.manaRegen != 0)
            stat_string += "기본 마나 재생 +" + selectItem.manaRegen.ToString() + "%\n";
        if (selectItem.cooldownReduce != 0)
            stat_string += "재사용 대기시간 감소 +" + selectItem.cooldownReduce.ToString() + "%\n";
        if (selectItem.armor != 0)
            stat_string += "방어력 +" + selectItem.armor.ToString() + "\n";
        if (selectItem.magicResist != 0)
            stat_string += "마법 저항력 +" + selectItem.magicResist.ToString() + "\n";
        if (selectItem.health != 0)
            stat_string += "체력 +" + selectItem.health.ToString() + "\n";
        if (selectItem.healthRegen != 0)
            stat_string += "기본 체력 재생 +" + selectItem.healthRegen.ToString() + "%\n";
        if (selectItem.movementSpeed != 0)
            stat_string += "이동 속도 +" + selectItem.movementSpeed.ToString() + "\n";

        description.transform.Find("Status").GetComponent<Text>().text = stat_string;

        if (stat_string != string.Empty)
        {
            int stat_lineCount = stat_string.Split('\n').Length - 1;
            height += 17.0f * stat_lineCount;
        }


        // 효과
        if (selectItem.effect_description != string.Empty)
        {
            // 스탯과 효과사이 간격
            if (stat_string != string.Empty)
                height += 8.0f;

            GameObject Effect_Discription = description.transform.Find("Effect_Description").gameObject;

            // 효과종류와 설명의 위치를 스탯 밑으로
            Effect_Discription.GetComponent<RectTransform>().anchoredPosition =
                new Vector2(Effect_Discription.GetComponent<RectTransform>().anchoredPosition.x,
                    -height);

            // 텍스트를 갱신
            if (selectItem.effect_kind != string.Empty)
            {
                Effect_Discription.GetComponent<Text>().text =
                    "<color=#FFEB17>" + selectItem.effect_kind + ":</color> " + selectItem.effect_description;
            }
            else
                Effect_Discription.GetComponent<Text>().text = selectItem.effect_description;

            // 자동으로 줄바꿈되는경우 cachedTextGenerator를 통해 줄바꿈결과를 받을수 있으나 다음 프레임이되야 갱신됨.
            // 그래서 Canvas.ForceUpdateCanvases() 함수를 통해 이전까지의 결과를 미리 업데이트하고 반영된 데이터를 얻음.
            Canvas.ForceUpdateCanvases();
            int description_lineCount = Effect_Discription.GetComponent<Text>().cachedTextGenerator.lineCount;
            height += 17.0f * description_lineCount;

            // 추가효과가 없는경우 공백으로.
            GameObject Additional_Discription = description.transform.Find("Additional_Description").gameObject;
            Additional_Discription.GetComponent<Text>().text = "";

            // 추가효과가 있다면 효과와 마찬가지로 출력
            if (selectItem.additional_description != string.Empty)
            {
                height += 8.0f;

                Additional_Discription.GetComponent<RectTransform>().anchoredPosition =
                new Vector2(Additional_Discription.GetComponent<RectTransform>().anchoredPosition.x,
                    -height);

                Additional_Discription.GetComponent<Text>().text =
                    "<color=#FFEB17>" + selectItem.additional_kind + ":</color> " + selectItem.additional_description;
            }
        }
        else
        {
            description.transform.Find("Effect_Description").GetComponent<Text>().text = "";
            description.transform.Find("Additional_Description").GetComponent<Text>().text = "";
        }
    }

    // 검색어 입력시 아이템을 검색하여 검색창에 만들어주는 함수
    public void ItemSearch(string input)
    {
        ShopItem.Instance.searchItemList.Clear();
        for (int i = 0; i < ShopItem.Instance.itemlist.Count; i++)
        {
            if (ShopItem.Instance.itemlist[i + 1].name.Contains(input))
            {
                ShopItem.Instance.searchItemList.Add(ShopItem.Instance.itemlist[i + 1]);
            }
        }

        foreach (Transform tr in searchContent.transform)
        {
            Destroy(tr.gameObject);
        }

        int count = 0;
        foreach (ShopItem.Item i in ShopItem.Instance.searchItemList)
        {
            GameObject createdItem = Instantiate(searchItem, searchContent.transform);
            createdItem.GetComponent<RectTransform>().localPosition = searchItemStartPos + new Vector3(count % searchLineMaxCount * searchItemWidth, count / searchLineMaxCount * -searchItemHeight);
            count++;

            createdItem.GetComponent<ItemInfo>().myItem = i.ClassCopy();
            createdItem.GetComponent<ItemInfo>().BasicSetting();
            createdItem.transform.Find("Name").GetComponent<Text>().text = i.name;
        }

        // 검색결과가 3줄이상이면 스크롤 적용
        if ((ShopItem.Instance.searchItemList.Count - 1) / searchLineMaxCount >= 2)
        {
            float SearchHeight = (((ShopItem.Instance.searchItemList.Count - 1) / searchLineMaxCount) + 1) * 57f;
            searchContent.GetComponent<RectTransform>().sizeDelta = new Vector2(searchContent.GetComponent<RectTransform>().sizeDelta.x, SearchHeight);
        }
        // 아니면 원래대로
        else
            searchContent.GetComponent<RectTransform>().sizeDelta = new Vector2(searchContent.GetComponent<RectTransform>().sizeDelta.x, 150);
    }

    // 메인 아이템 화면에 아이템을 만들어주는 함수
    public void ItemCreate(int id)
    {
        foreach (Transform tr in itemContent.transform)
        {
            Destroy(tr.gameObject);
        }
        ShopItem.Instance.sortedItemList.Clear();

        // 정렬
        if (id == 0)
            CreateItemAll();
        else if (id == 1)
            CreateItemSpecial();
        else if (id == 2)
            CreateItemConsumable();
        else if (id == 3)
            CreateItemAccessory();
        else if (id == 4)
            CreateItemDefense();
        else if (id == 5)
            CreateItemArmor();
        else if (id == 6)
            CreateItemMagicResist();
        else if (id == 7)
            CreateItemHealth();
        else if (id == 8)
            CreateItemHealthRegen();
        else if (id == 9)
            CreateItemAttack();
        else if (id == 10)
            CreateItemAttackDamage();
        else if (id == 11)
            CreateItemAttackSpeed();
        else if (id == 12)
            CreateItemCritical();
        else if (id == 13)
            CreateItemLifeSteal();
        else if (id == 14)
            CreateItemMagic();
        else if (id == 15)
            CreateItemAbilityPower();
        else if (id == 16)
            CreateItemMana();
        else if (id == 17)
            CreateItemManaRegen();
        else if (id == 18)
            CreateItemCooldownReduce();
        else if (id == 19)
            CreateItemMove();
        else if (id == 20)
            CreateItemBoots();
        else if (id == 21)
            CreateItemMovementSpeed();


        // 선택된 리스트에 있는 아이템 생성
        int count = 0;
        foreach (ShopItem.Item i in ShopItem.Instance.sortedItemList)
        {
            GameObject createdItem = Instantiate(item, itemContent.transform);
            createdItem.transform.localPosition = itemStartPos + new Vector3(count % lineMaxItemCount * itemWidth, count / lineMaxItemCount * -itemHeight);
            count++;

            createdItem.GetComponent<ItemInfo>().myItem = i.ClassCopy();
            createdItem.GetComponent<ItemInfo>().BasicSetting();
        }

        // 검색결과가 6줄이상이면 스크롤 적용
        if ((ShopItem.Instance.sortedItemList.Count - 1) / lineMaxItemCount >= 5)
        {
            float ListHeight = (((ShopItem.Instance.sortedItemList.Count - 1) / lineMaxItemCount) + 1) * 70f;
            itemContent.GetComponent<RectTransform>().sizeDelta = new Vector2(itemContent.GetComponent<RectTransform>().sizeDelta.x, ListHeight);
        }
        // 아니면 원래대로
        else
            itemContent.GetComponent<RectTransform>().sizeDelta = new Vector2(itemContent.GetComponent<RectTransform>().sizeDelta.x, 400);
    }

    #region 카테고리별 아이템 생성
    public void CreateItemAll()
    {
        for (int i = 0; i < ShopItem.Instance.itemlist.Count; i++)
        {
            ShopItem.Instance.sortedItemList.Add(ShopItem.Instance.itemlist[i + 1]);
        }
    }

    public void CreateItemSpecial()
    {
        for (int i = 0; i < ShopItem.Instance.itemlist.Count; i++)
        {
            if (ShopItem.Instance.itemlist[i + 1].consumable == true || ShopItem.Instance.itemlist[i + 1].accessory == true)
            {
                ShopItem.Instance.sortedItemList.Add(ShopItem.Instance.itemlist[i + 1]);
            }
        }
    }

    public void CreateItemConsumable()
    {
        for (int i = 0; i < ShopItem.Instance.itemlist.Count; i++)
        {
            if (ShopItem.Instance.itemlist[i + 1].consumable == true)
            {
                ShopItem.Instance.sortedItemList.Add(ShopItem.Instance.itemlist[i + 1]);
            }
        }
    }

    public void CreateItemAccessory()
    {
        for (int i = 0; i < ShopItem.Instance.itemlist.Count; i++)
        {
            if (ShopItem.Instance.itemlist[i + 1].accessory == true)
            {
                ShopItem.Instance.sortedItemList.Add(ShopItem.Instance.itemlist[i + 1]);
            }
        }
    }

    public void CreateItemDefense()
    {
        for (int i = 0; i < ShopItem.Instance.itemlist.Count; i++)
        {
            if (ShopItem.Instance.itemlist[i + 1].armor != 0 || ShopItem.Instance.itemlist[i + 1].magicResist != 0
                || ShopItem.Instance.itemlist[i + 1].health != 0 || ShopItem.Instance.itemlist[i + 1].healthRegen != 0)
            {
                ShopItem.Instance.sortedItemList.Add(ShopItem.Instance.itemlist[i + 1]);
            }
        }
    }

    public void CreateItemArmor()
    {
        for (int i = 0; i < ShopItem.Instance.itemlist.Count; i++)
        {
            if (ShopItem.Instance.itemlist[i + 1].armor != 0)
            {
                ShopItem.Instance.sortedItemList.Add(ShopItem.Instance.itemlist[i + 1]);
            }
        }
    }

    public void CreateItemMagicResist()
    {
        for (int i = 0; i < ShopItem.Instance.itemlist.Count; i++)
        {
            if (ShopItem.Instance.itemlist[i + 1].magicResist != 0)
            {
                ShopItem.Instance.sortedItemList.Add(ShopItem.Instance.itemlist[i + 1]);
            }
        }
    }

    public void CreateItemHealth()
    {
        for (int i = 0; i < ShopItem.Instance.itemlist.Count; i++)
        {
            if (ShopItem.Instance.itemlist[i + 1].health != 0)
            {
                ShopItem.Instance.sortedItemList.Add(ShopItem.Instance.itemlist[i + 1]);
            }
        }
    }

    public void CreateItemHealthRegen()
    {
        for (int i = 0; i < ShopItem.Instance.itemlist.Count; i++)
        {
            if (ShopItem.Instance.itemlist[i + 1].healthRegen != 0)
            {
                ShopItem.Instance.sortedItemList.Add(ShopItem.Instance.itemlist[i + 1]);
            }
        }
    }

    public void CreateItemAttack()
    {
        for (int i = 0; i < ShopItem.Instance.itemlist.Count; i++)
        {
            if (ShopItem.Instance.itemlist[i + 1].attackDamage != 0 || ShopItem.Instance.itemlist[i + 1].attackSpeed != 0
                || ShopItem.Instance.itemlist[i + 1].criticalPercent != 0 || ShopItem.Instance.itemlist[i + 1].lifeSteal != 0)
            {
                ShopItem.Instance.sortedItemList.Add(ShopItem.Instance.itemlist[i + 1]);
            }
        }
    }

    public void CreateItemAttackDamage()
    {
        for (int i = 0; i < ShopItem.Instance.itemlist.Count; i++)
        {
            if (ShopItem.Instance.itemlist[i + 1].attackDamage != 0)
            {
                ShopItem.Instance.sortedItemList.Add(ShopItem.Instance.itemlist[i + 1]);
            }
        }
    }

    public void CreateItemAttackSpeed()
    {
        for (int i = 0; i < ShopItem.Instance.itemlist.Count; i++)
        {
            if (ShopItem.Instance.itemlist[i + 1].attackSpeed != 0)
            {
                ShopItem.Instance.sortedItemList.Add(ShopItem.Instance.itemlist[i + 1]);
            }
        }
    }

    public void CreateItemCritical()
    {
        for (int i = 0; i < ShopItem.Instance.itemlist.Count; i++)
        {
            if (ShopItem.Instance.itemlist[i + 1].criticalPercent != 0)
            {
                ShopItem.Instance.sortedItemList.Add(ShopItem.Instance.itemlist[i + 1]);
            }
        }
    }

    public void CreateItemLifeSteal()
    {
        for (int i = 0; i < ShopItem.Instance.itemlist.Count; i++)
        {
            if (ShopItem.Instance.itemlist[i + 1].lifeSteal != 0)
            {
                ShopItem.Instance.sortedItemList.Add(ShopItem.Instance.itemlist[i + 1]);
            }
        }
    }

    public void CreateItemMagic()
    {
        for (int i = 0; i < ShopItem.Instance.itemlist.Count; i++)
        {
            if (ShopItem.Instance.itemlist[i + 1].abilityPower != 0 || ShopItem.Instance.itemlist[i + 1].mana != 0
                || ShopItem.Instance.itemlist[i + 1].manaRegen != 0 || ShopItem.Instance.itemlist[i + 1].cooldownReduce != 0)
            {
                ShopItem.Instance.sortedItemList.Add(ShopItem.Instance.itemlist[i + 1]);
            }
        }
    }

    public void CreateItemAbilityPower()
    {
        for (int i = 0; i < ShopItem.Instance.itemlist.Count; i++)
        {
            if (ShopItem.Instance.itemlist[i + 1].abilityPower != 0)
            {
                ShopItem.Instance.sortedItemList.Add(ShopItem.Instance.itemlist[i + 1]);
            }
        }
    }

    public void CreateItemMana()
    {
        for (int i = 0; i < ShopItem.Instance.itemlist.Count; i++)
        {
            if (ShopItem.Instance.itemlist[i + 1].mana != 0)
            {
                ShopItem.Instance.sortedItemList.Add(ShopItem.Instance.itemlist[i + 1]);
            }
        }
    }

    public void CreateItemManaRegen()
    {
        for (int i = 0; i < ShopItem.Instance.itemlist.Count; i++)
        {
            if (ShopItem.Instance.itemlist[i + 1].manaRegen != 0)
            {
                ShopItem.Instance.sortedItemList.Add(ShopItem.Instance.itemlist[i + 1]);
            }
        }
    }

    public void CreateItemCooldownReduce()
    {
        for (int i = 0; i < ShopItem.Instance.itemlist.Count; i++)
        {
            if (ShopItem.Instance.itemlist[i + 1].cooldownReduce != 0)
            {
                ShopItem.Instance.sortedItemList.Add(ShopItem.Instance.itemlist[i + 1]);
            }
        }
    }

    public void CreateItemMove()
    {
        for (int i = 0; i < ShopItem.Instance.itemlist.Count; i++)
        {
            if (ShopItem.Instance.itemlist[i + 1].boots == true || ShopItem.Instance.itemlist[i + 1].movementSpeed != 0)
            {
                ShopItem.Instance.sortedItemList.Add(ShopItem.Instance.itemlist[i + 1]);
            }
        }
    }

    public void CreateItemBoots()
    {
        for (int i = 0; i < ShopItem.Instance.itemlist.Count; i++)
        {
            if (ShopItem.Instance.itemlist[i + 1].boots == true)
            {
                ShopItem.Instance.sortedItemList.Add(ShopItem.Instance.itemlist[i + 1]);
            }
        }
    }

    public void CreateItemMovementSpeed()
    {
        for (int i = 0; i < ShopItem.Instance.itemlist.Count; i++)
        {
            if (ShopItem.Instance.itemlist[i + 1].movementSpeed != 0)
            {
                ShopItem.Instance.sortedItemList.Add(ShopItem.Instance.itemlist[i + 1]);
            }
        }
    }
    #endregion
}
