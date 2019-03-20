using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIStat : MonoBehaviour
{
    [HideInInspector]
    public StatClass.Stat stat = null;
    public StatClass.Stat originStat = null;

    public Text attackDamageText;
    public Text abilityPowerText;
    public Text defenceText;
    public Text magicResistText;
    public Text attackSpeedText;
    public Text cooldownReduceText;
    public Text criticalText;
    public Text moveSpeedText;

    void Start()
    {
        stat = GameObject.FindGameObjectWithTag("Player").GetComponent<ChampionData>().totalStat;
        originStat = GameObject.FindGameObjectWithTag("Player").GetComponent<ChampionData>().myStat;
        Refresh();
    }

    public void Refresh()
    {
        attackDamageText.text = Mathf.RoundToInt(stat.AttackDamage).ToString();
        abilityPowerText.text = Mathf.RoundToInt(stat.AbilityPower).ToString();
        defenceText.text = Mathf.RoundToInt(stat.AttackDef).ToString();
        magicResistText.text = Mathf.RoundToInt(stat.AbilityDef).ToString();
        float AS = originStat.AttackSpeed * (1 + (stat.UP_AttackSpeed * (stat.Level - 1) + (stat.AttackSpeed - originStat.AttackSpeed)) / 100);
        attackSpeedText.text = System.Math.Round(AS, 2).ToString();
        cooldownReduceText.text = Mathf.RoundToInt(stat.CoolTimeDecrease).ToString();
        criticalText.text = Mathf.RoundToInt(stat.CriticalPercentage).ToString();
        moveSpeedText.text = Mathf.RoundToInt(stat.MoveSpeed * 50f).ToString();
    }
}
