using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExpTooltip : MonoBehaviour {

    [SerializeField]
    private GameObject tooltip;

    private ChampionData myChampionData;
    private bool isMouseOver = false;

    // Use this for initialization
    void Start () {
        GameObject Player = GameObject.FindGameObjectWithTag("Player");
        if (Player == null)
        {
            StructureSetting.instance.ActiveTrue();
            Player = GameObject.FindGameObjectWithTag("Player");
        }
        myChampionData = Player.GetComponent<ChampionData>();
    }
	
	void Update () {
		if(isMouseOver)
        {
            tooltip.transform.Find("Text").GetComponent<Text>().text = myChampionData.myStat.Exp.ToString() + " / " + myChampionData.myStat.RequireExp.ToString() + " 경험치";
            tooltip.transform.position = Input.mousePosition + new Vector3(-tooltip.GetComponent<RectTransform>().sizeDelta.x*0.75f, tooltip.GetComponent<RectTransform>().sizeDelta.y);
        }
	}

    public void PointerEnter()
    {
        if(tooltip != null)
        {
            tooltip.SetActive(true);
            isMouseOver = true;
        }
    }

    public void PointerExit()
    {
        if (tooltip != null)
        {
            tooltip.SetActive(false);
            isMouseOver = false;
        }
    }
}
