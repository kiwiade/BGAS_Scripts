using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillInfo : MonoBehaviour
{
    [SerializeField]
    private string skillkey = "";

    private SkillClass.Skill2 mySkill = new SkillClass.Skill2();

    private GameObject tooltip;
    private ChampionData myChampionData;
    private GameObject player;


    void Start()
    {
        UICanvas uiCanvas = GameObject.FindGameObjectWithTag("UICanvas").GetComponent<UICanvas>();
        tooltip = uiCanvas.tooltip;

        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            StructureSetting.instance.ActiveTrue();
            player = GameObject.FindGameObjectWithTag("Player");
        }

        myChampionData = player.GetComponent<ChampionData>();
        SkillApply();
    }
    

    private void SkillApply()
    {
        switch (skillkey)
        {
            case "Passive":
                mySkill.Name = myChampionData.mySkill.passiveName;
                mySkill.Description = myChampionData.mySkill.passiveDescription;
                mySkill.Cooldown[0] = myChampionData.mySkill.passiveCooldown;
                mySkill.Damage[0] = myChampionData.mySkill.passiveDamage;
                mySkill.Astat = myChampionData.mySkill.passiveAstat;
                mySkill.Avalue = myChampionData.mySkill.passiveAvalue;
                mySkill.skillLevel = 0;
                break;

            case "Q":
                mySkill.Name = myChampionData.mySkill.qName;
                mySkill.Description = myChampionData.mySkill.qDescription;
                mySkill.Range = myChampionData.mySkill.qRange;
                mySkill.Mana = myChampionData.mySkill.qMana;
                mySkill.Cooldown = myChampionData.mySkill.qCooldown;
                mySkill.Damage = myChampionData.mySkill.qDamage;
                mySkill.Astat = myChampionData.mySkill.qAstat;
                mySkill.Avalue = myChampionData.mySkill.qAvalue;
                mySkill.skillLevel = 0;
                break;

            case "W":
                mySkill.Name = myChampionData.mySkill.wName;
                mySkill.Description = myChampionData.mySkill.wDescription;
                mySkill.Range = myChampionData.mySkill.wRange;
                mySkill.Mana = myChampionData.mySkill.wMana;
                mySkill.Cooldown = myChampionData.mySkill.wCooldown;
                mySkill.Damage = myChampionData.mySkill.wDamage;
                mySkill.Astat = myChampionData.mySkill.wAstat;
                mySkill.Avalue = myChampionData.mySkill.wAvalue;
                mySkill.skillLevel = 0;
                break;

            case "E":
                mySkill.Name = myChampionData.mySkill.eName;
                mySkill.Description = myChampionData.mySkill.eDescription;
                mySkill.Range = myChampionData.mySkill.eRange;
                mySkill.Mana = myChampionData.mySkill.eMana;
                mySkill.Cooldown = myChampionData.mySkill.eCooldown;
                mySkill.Damage = myChampionData.mySkill.eDamage;
                mySkill.Astat = myChampionData.mySkill.eAstat;
                mySkill.Avalue = myChampionData.mySkill.eAvalue;
                mySkill.skillLevel = 0;
                break;

            case "R":
                mySkill.Name = myChampionData.mySkill.rName;
                mySkill.Description = myChampionData.mySkill.rDescription;
                mySkill.Range = myChampionData.mySkill.rRange;
                mySkill.Mana = myChampionData.mySkill.rMana;
                mySkill.Cooldown = myChampionData.mySkill.rCooldown;
                mySkill.Damage = myChampionData.mySkill.rDamage;
                mySkill.Astat = myChampionData.mySkill.rAstat;
                mySkill.Avalue = myChampionData.mySkill.rAvalue;
                mySkill.skillLevel = 0;
                break;
        }
    }

    private void SkillLevelRefresh()
    {
        if(!myChampionData)
        {
            myChampionData = player.GetComponent<ChampionData>();
        }

        switch (skillkey)
        {
            case "Q":
                mySkill.skillLevel = myChampionData.skill_Q - 1;
                break;
            case "W":
                mySkill.skillLevel = myChampionData.skill_W - 1;
                break;
            case "E":
                mySkill.skillLevel = myChampionData.skill_E - 1;
                break;
            case "R":
                mySkill.skillLevel = myChampionData.skill_R - 1;
                break;

            default:
                break;
        }

        if (mySkill.skillLevel < 0)
            mySkill.skillLevel = 0;
    }

    public int Acalculate(string Astat, float Avalue)
    {
        float result = 0;
        switch (Astat)
        {
            case "AD":
                result = myChampionData.totalStat.AttackDamage;
                break;

            case "AP":
                result = myChampionData.totalStat.AbilityPower;
                break;

            case "DEF":
                result = myChampionData.totalStat.AttackDef;
                break;

            case "MDEF":
                result = myChampionData.totalStat.AbilityDef;
                break;

            case "maxHP":
                result = myChampionData.totalStat.MaxHp;
                break;

            case "maxMP":
                result = myChampionData.totalStat.MaxMp;
                break;

            case "minusHP":
                result = myChampionData.totalStat.MaxHp - myChampionData.totalStat.Hp;
                break;

            case "Critical":
                result = myChampionData.totalStat.CriticalPercentage;
                break;

            default:
                break;
        }

        result *= Avalue;

        return Mathf.RoundToInt(result);
    }

    public string AstatColor(string Astat)
    {
        string colorstring;
        string colorcode;

        switch (Astat)
        {
            case "AD":
                colorcode = "#D97800";
                break;
            case "AP":
                colorcode = "#83DA84";
                break;
            case "Critical":
                colorcode = "#999999";
                break;
            default:
                colorcode = "#FF1010";
                break;
        }
        colorstring = "<color=" + colorcode + ">";

        return colorstring;
    }

    public void TooltipOn()
    {
        SkillLevelRefresh();
        if (tooltip != null)
        {
            tooltip.SetActive(true);

            float tooltip_height = 30;
            tooltip.transform.Find("TitleText").GetComponent<Text>().text = mySkill.Name;
            if (skillkey != "Passive")
            {
                tooltip.transform.Find("TitleText").GetComponent<Text>().text += " (" + (mySkill.skillLevel + 1).ToString() + "레벨)";
                tooltip.transform.Find("HotKey").GetComponent<Text>().text = "[" + skillkey + "]";
                if (mySkill.Mana[mySkill.skillLevel] == 0)
                    tooltip.transform.Find("Title_Description").GetComponent<Text>().text = "소모값 없음";
                else
                {
                    if (myChampionData.isNoMP)
                        tooltip.transform.Find("Title_Description").GetComponent<Text>().text = "체력 " + mySkill.Mana[mySkill.skillLevel].ToString();
                    else
                        tooltip.transform.Find("Title_Description").GetComponent<Text>().text = "마나 " + mySkill.Mana[mySkill.skillLevel].ToString();

                }
                if (mySkill.Cooldown[mySkill.skillLevel] == 0)
                    tooltip.transform.Find("Cooldown").GetComponent<Text>().text = "재사용 대기시간 없음";
                else
                    tooltip.transform.Find("Cooldown").GetComponent<Text>().text = "재사용 대기시간 " + mySkill.Cooldown[mySkill.skillLevel].ToString() + "초";
            }
            else
            {
                tooltip.transform.Find("HotKey").GetComponent<Text>().text = "";
                tooltip.transform.Find("Title_Description").GetComponent<Text>().text = "";
                tooltip.transform.Find("Cooldown").GetComponent<Text>().text = "";
                tooltip_height -= 20.0f;
            }
            tooltip_height += 5.0f;
            Canvas.ForceUpdateCanvases();

            int description_lineCount = tooltip.transform.Find("Title_Description").GetComponent<Text>().cachedTextGenerator.lineCount;
            tooltip_height += 15.0f * description_lineCount;

            // Line1 표시
            tooltip_height += 5.0f;
            tooltip.transform.Find("Line1").gameObject.SetActive(true);
            tooltip.transform.Find("Line1").GetComponent<RectTransform>().anchoredPosition =
                new Vector3(tooltip.transform.Find("Line1").GetComponent<RectTransform>().anchoredPosition.x, -tooltip_height);
            tooltip_height += 10.0f;

            // 추가설명 갱신
            tooltip.transform.Find("Additional_Description").GetComponent<RectTransform>().anchoredPosition =
                new Vector3(tooltip.transform.Find("Additional_Description").GetComponent<RectTransform>().anchoredPosition.x, -tooltip_height);
            tooltip.transform.Find("Additional_Description").GetComponent<Text>().text
                = mySkill.Description.Replace("@", mySkill.Damage[mySkill.skillLevel].ToString())
                .Replace("(+$)", AstatColor(mySkill.Astat) + "(+" + Acalculate(mySkill.Astat, mySkill.Avalue).ToString() + ")</color>")
                .Replace("(+$%)", AstatColor(mySkill.Astat) + "(+" + Acalculate(mySkill.Astat, mySkill.Avalue).ToString() + "%)</color>")
                .Replace("$", AstatColor(mySkill.Astat) + Acalculate(mySkill.Astat, mySkill.Avalue).ToString() + "</color>")
                .Replace("기본 지속 효과:", "<color=#D97800>기본 지속 효과:</color>")
                .Replace("사용 시:", "<color=#D97800>사용 시:</color>")
                .Replace("사용 효과:", "<color=#D97800>사용 효과:</color>")
                .Replace("활성화/비활성화:", "<color=#D97800>활성화/비활성화:</color>")
                .Replace("\\n", "\n");
            Canvas.ForceUpdateCanvases();

            int a_description_lineCount = tooltip.transform.Find("Additional_Description").GetComponent<Text>().cachedTextGenerator.lineCount;
            tooltip_height += 19.0f * a_description_lineCount;

            tooltip_height += 5.0f;
            tooltip.GetComponent<RectTransform>().sizeDelta = new Vector2(tooltip.GetComponent<RectTransform>().sizeDelta.x, tooltip_height);
        }
    }

    public void TooltipOff()
    {
        if (tooltip != null)
            tooltip.SetActive(false);
    }
}
