﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour
{
    [SerializeField]
    private GameObject shopCanvas = null;

    [SerializeField]
    private GameObject[] myItem = new GameObject[6];
    [SerializeField]
    private GameObject accessory = null;
    [SerializeField]
    private Text priceText = null;

    void Update()
    {
        StatusUpdate();
    }

    public void ShopButton()
    {
        if (shopCanvas.activeSelf)
        {
            SoundManager.Instance.PlaySound(SoundManager.Instance.UI_Close);
            shopCanvas.SetActive(false);
        }
        else
        {
            SoundManager.Instance.PlaySound(SoundManager.Instance.UI_Open);
            shopCanvas.SetActive(true);
        }
    }

    public void StatusUpdate()
    {
        priceText.text = PlayerData.Instance.gold.ToString();

        for (int i = 0; i < PlayerData.Instance.item.Length; i++)
        {
            if (PlayerData.Instance.item[i] != 0)
            {
                ShopItem.Item it = ShopItem.Instance.itemlist[PlayerData.Instance.item[i]];
                myItem[i].GetComponent<ItemInfo>().myItem = it.ClassCopy();
                myItem[i].transform.Find("Icon").GetComponent<Image>().sprite = Resources.Load<Sprite>("Item_Image/" + it.iconName);
                myItem[i].transform.Find("Icon").GetComponent<Image>().color = Color.white;
            }
            else
            {
                myItem[i].GetComponent<ItemInfo>().myItem = null;
                myItem[i].transform.Find("Icon").GetComponent<Image>().sprite = null;
                myItem[i].transform.Find("Icon").GetComponent<Image>().color = new Color(14f / 255f, 26f / 255f, 23f / 255f, 1);
            }
        }

        if (PlayerData.Instance.accessoryItem != 0)
        {
            ShopItem.Item it = ShopItem.Instance.itemlist[PlayerData.Instance.accessoryItem];
            accessory.GetComponent<ItemInfo>().myItem = it.ClassCopy();
            accessory.transform.Find("Icon").GetComponent<Image>().sprite = Resources.Load<Sprite>("Item_Image/" + it.iconName);
            accessory.transform.Find("Icon").GetComponent<Image>().color = Color.white;
        }
        else
        {
            accessory.GetComponent<ItemInfo>().myItem = null;
            accessory.transform.Find("Icon").GetComponent<Image>().sprite = null;
            accessory.transform.Find("Icon").GetComponent<Image>().color = new Color(14f / 255f, 26f / 255f, 23f / 255f, 1);
        }
    }
}
