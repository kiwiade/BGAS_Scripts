using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIIcon : MonoBehaviour
{
    private StatClass.Stat stat = null;

    [SerializeField]
    private GameObject statUI;
    [SerializeField]
    private Image championIcon;
    public Image reviveCoverImage;
    public Text reviveTimeText;
    [SerializeField]
    private Text levelText;
    [SerializeField]
    private Image expBar;

    private void FindPlayer()
    {   
        GameObject Player = GameObject.FindGameObjectWithTag("Player");
        if (Player == null)
        {
            StructureSetting.instance.ActiveTrue();
            Player = GameObject.FindGameObjectWithTag("Player");
        }
        ChampionData cd = Player.GetComponent<ChampionData>();
        stat = cd.myStat;
        championIcon.sprite = Resources.Load<Sprite>("Champion/ChampionIcon/" + cd.championName);
    }

    void Start()
    {
        FindPlayer();
    }

    void Update()
    {
        ExpUpdate();
    }

    private void ExpUpdate()
    {
        float expPercent = (float)stat.Exp / (float)stat.RequireExp;
        expBar.fillAmount = expPercent;
    }

    public void LevelUp()
    {
        levelText.text = stat.Level.ToString();
    }

    public void StatButton()
    {
        if (statUI.activeSelf)
            statUI.SetActive(false);
        else
            statUI.SetActive(true);
    }
}
