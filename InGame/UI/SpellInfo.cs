using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellInfo : MonoBehaviour
{
    [SerializeField]
    private string spellkey = "";

    private GameObject tooltip;
    private ChampionData myChampionData;

    private int mySpellNum = 0;
    private float mySpellCooldown = 0;
    private string mySpellName = "";
    private string mySpellDescription = "";
    
    void Start()
    {
        UICanvas uiCanvas = GameObject.FindGameObjectWithTag("UICanvas").GetComponent<UICanvas>();
        tooltip = uiCanvas.tooltip;

        GameObject Player = GameObject.FindGameObjectWithTag("Player");
        if (Player == null)
        {
            StructureSetting.instance.ActiveTrue();
            Player = GameObject.FindGameObjectWithTag("Player");
        }

        myChampionData = Player.GetComponent<ChampionData>();

        if (spellkey == "D")
        {
            mySpellNum = myChampionData.spell_D;
            mySpellCooldown = myChampionData.Cooldown_D;
        }
        else if (spellkey == "F")
        {
            mySpellNum = myChampionData.spell_F;
            mySpellCooldown = myChampionData.Cooldown_F;
        }

        SetSpellNameDescription(mySpellNum);
    }

    public void SetSpellNameDescription(int spellNum)
    {
        switch (spellNum)
        {
            // 정화 탈진 점멸 유체화 회복 강타 순간이동 점화 방어막
            case 0:
                mySpellName = "정화";
                mySpellDescription = "챔피언에 걸린 모든 이동 불가와 (제압 및 공중으로 띄우는 효과 제외) 소환사 주문에 의한 해로운 효과를 제거하고 새로 적용되는 이동 불가 효과들의 지속시간을 3초간 65% 감소시킵니다.";
                break;
            case 1:
                mySpellName = "탈진";
                mySpellDescription = "적 챔피언을 지치게 만들어 2.5초 동안 이동 속도를 30% 낮추며, 가하는 피해량을 40% 낮춥니다.";
                break;
            case 2:
                mySpellName = "점멸";
                mySpellDescription = "커서 방향으로 챔피언이 짧은 거리를 순간이동합니다.";
                break;
            case 3:
                mySpellName = "유체화";
                mySpellDescription = "챔피언이 10초 동안 이동 속도가 상승합니다. 이동 속도는 2초동안 점차 빨라져 28%까지 상승합니다.";
                break;
            case 4:
                mySpellName = "회복";
                mySpellDescription = "챔피언과 대상 아군의 체력을 90만큼 회복시키고 2초 동안 이동 속도가 30% 증가합니다.";
                break;
            case 5:
                mySpellName = "강타";
                mySpellDescription = "대상 에픽 및 대형/중형 몬스터, 혹은 적 미니언에게 390의 고정 피해를 입힙니다. 강타를 사용하면 자신의 최대체력의 10%만큼 회복됩니다.";
                break;
            case 6:
                mySpellName = "순간이동";
                mySpellDescription = "4.5초 동안 정신 집중을 한 후 근처의 아군 미니언이나 포탑, 혹은 와드로 순간이동합니다.\n\n다시 사용하면 순간이동이 취소되며 재사용 대기시간이 적용되지 않습니다.";
                break;
            case 7:
                mySpellName = "점화";
                mySpellDescription = "적 챔피언을 불태워 5초 동안 80의 고정 피해를 입힙니다.";
                break;
            case 8:
                mySpellName = "방어막";
                mySpellDescription = "2초 동안 방어막으로 감싸 피해를 115만큼 흡수합니다.";
                break;
            default:
                break;
        }
    }

    public void TooltipOn()
    {
        if (tooltip != null)
        {
            tooltip.SetActive(true);

            float tooltip_height = 30;
            tooltip.transform.Find("TitleText").GetComponent<Text>().text = mySpellName;
            tooltip.transform.Find("HotKey").GetComponent<Text>().text = "[" + spellkey + "]";
            tooltip.transform.Find("Title_Description").GetComponent<Text>().text = "소모값 없음";
            tooltip.transform.Find("Cooldown").GetComponent<Text>().text = "재사용 대기시간 " + Mathf.RoundToInt(mySpellCooldown).ToString() + "초";
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
                = mySpellDescription.Replace("\n\n", "\n");
            Canvas.ForceUpdateCanvases();

            int a_description_lineCount = tooltip.transform.Find("Additional_Description").GetComponent<Text>().cachedTextGenerator.lineCount;
            tooltip_height += 19.0f * a_description_lineCount;

            tooltip_height += 5.0f;
            tooltip.GetComponent<RectTransform>().sizeDelta = new Vector2(tooltip.GetComponent<RectTransform>().sizeDelta.x, tooltip_height);
        }
    }

    public void ToooltipOff()
    {
        if (tooltip != null)
            tooltip.SetActive(false);
    }
}