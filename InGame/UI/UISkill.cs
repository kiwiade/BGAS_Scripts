using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISkill : MonoBehaviour
{
    [SerializeField]
    private Image[] skillIcon;
    public GameObject[] skillDisabledImage;
    public Image[] skillCooldownImage;
    public Text[] skillCooldownText;
    [SerializeField]
    private Text levelUpText;

    public GameObject[] skillUpButton;
    public GameObject[] skillUpButton2;
    [SerializeField]
    private GameObject[] skillLevelLamp;

    [Space]
    [SerializeField]
    private Image[] spellIcon;
    public GameObject[] spellDisabledImage;
    public Image[] spellCooldownImage;
    public Text[] spellCooldownText;

    [Space]
    [SerializeField]
    private ProgressBar healthBar;
    [SerializeField]
    private ProgressBar manaBar;
    [SerializeField]
    private Text healthRegenText;
    [SerializeField]
    private Text manaRegenText;

    private int skillpoint = 1;
    private ChampionData myChanmpionData;
    private PlayerData playerData;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj == null)
        {
            StructureSetting.instance.ActiveTrue();
            playerObj = GameObject.FindGameObjectWithTag("Player");
        }
        myChanmpionData = playerObj.GetComponent<ChampionData>();
        playerData = PlayerData.Instance;

        skillIcon[0].sprite = Resources.Load<Sprite>("Champion/ChampionSkill/" + myChanmpionData.championName + "/Passive");
        skillIcon[1].sprite = Resources.Load<Sprite>("Champion/ChampionSkill/" + myChanmpionData.championName + "/Q");
        skillIcon[2].sprite = Resources.Load<Sprite>("Champion/ChampionSkill/" + myChanmpionData.championName + "/W");
        skillIcon[3].sprite = Resources.Load<Sprite>("Champion/ChampionSkill/" + myChanmpionData.championName + "/E");
        skillIcon[4].sprite = Resources.Load<Sprite>("Champion/ChampionSkill/" + myChanmpionData.championName + "/R");

        spellIcon[0].sprite = Resources.Load<Sprite>("Spell/" + myChanmpionData.spell_D);
        spellIcon[1].sprite = Resources.Load<Sprite>("Spell/" + myChanmpionData.spell_F);
    }

    void Update()
    {
        ProgressRefresh();
    }

    public void SkillUp(string Hotkey)
    {
        skillpoint--;
        levelUpText.text = "레벨 업! +" + skillpoint.ToString();

        switch (Hotkey)
        {
            case "Q":
                myChanmpionData.skill_Q++;
                myChanmpionData.Cooldown_Q = myChanmpionData.mySkill.qCooldown[myChanmpionData.skill_Q - 1];
                myChanmpionData.mana_Q = myChanmpionData.mySkill.qMana[myChanmpionData.skill_Q - 1];
                skillLevelLamp[0].transform.Find(myChanmpionData.skill_Q.ToString()).Find("on").gameObject.SetActive(true);
                skillDisabledImage[1].SetActive(false);
                break;
            case "W":
                myChanmpionData.skill_W++;
                myChanmpionData.Cooldown_W = myChanmpionData.mySkill.wCooldown[myChanmpionData.skill_W - 1];
                myChanmpionData.mana_W = myChanmpionData.mySkill.wMana[myChanmpionData.skill_W - 1];
                skillLevelLamp[1].transform.Find(myChanmpionData.skill_W.ToString()).Find("on").gameObject.SetActive(true);
                skillDisabledImage[2].SetActive(false);
                break;
            case "E":
                myChanmpionData.skill_E++;
                myChanmpionData.Cooldown_E = myChanmpionData.mySkill.eCooldown[myChanmpionData.skill_E - 1];
                myChanmpionData.mana_E = myChanmpionData.mySkill.eMana[myChanmpionData.skill_E - 1];
                skillLevelLamp[2].transform.Find(myChanmpionData.skill_E.ToString()).Find("on").gameObject.SetActive(true);
                skillDisabledImage[3].SetActive(false);
                break;
            case "R":
                myChanmpionData.skill_R++;
                myChanmpionData.Cooldown_R = myChanmpionData.mySkill.rCooldown[myChanmpionData.skill_R - 1];
                myChanmpionData.mana_R = myChanmpionData.mySkill.rMana[myChanmpionData.skill_R - 1];
                skillLevelLamp[3].transform.Find(myChanmpionData.skill_R.ToString()).Find("on").gameObject.SetActive(true);
                skillDisabledImage[4].SetActive(false);
                break;
            default:
                break;
        }

        // 갱신
        if (skillpoint != 0)
        {
            SkillLimit();
        }
        else
        {
            levelUpText.gameObject.SetActive(false);
            for (int i = 0; i < 4; i++)
            {
                skillUpButton[i].SetActive(false);
                skillUpButton2[i].SetActive(false);
            }
        }

        // 툴팁 off
        GameObject.FindGameObjectWithTag("UICanvas").GetComponent<UICanvas>().tooltip.SetActive(false);
    }

    public void LevelUp()
    {
        skillpoint++;
        levelUpText.text = "레벨 업! +" + skillpoint.ToString();
        levelUpText.gameObject.SetActive(true);

        SkillLimit();
    }

    public void ProgressRefresh()
    {
        if (myChanmpionData == null)
            return;
        healthBar.value = myChanmpionData.totalStat.Hp / myChanmpionData.totalStat.MaxHp;
        healthBar.text = Mathf.FloorToInt(myChanmpionData.totalStat.Hp).ToString() + " / " + Mathf.FloorToInt(myChanmpionData.totalStat.MaxHp).ToString();

        manaBar.value = myChanmpionData.totalStat.Mp / myChanmpionData.totalStat.MaxMp;
        manaBar.text = Mathf.FloorToInt(myChanmpionData.totalStat.Mp).ToString() + " / " + Mathf.FloorToInt(myChanmpionData.totalStat.MaxMp).ToString();

        if (playerData.isDead)
        {
            healthRegenText.text = "";
            manaRegenText.text = "";
        }
        else
        {
            if (myChanmpionData.totalStat.Hp != myChanmpionData.totalStat.MaxHp)
                healthRegenText.text = "+" + (myChanmpionData.totalStat.HealthRegen * 0.2f).ToString("N1");
            else
                healthRegenText.text = "";
            if (myChanmpionData.totalStat.Mp != myChanmpionData.totalStat.MaxMp)
                manaRegenText.text = "+" + (myChanmpionData.totalStat.ManaRegen * 0.2f).ToString("N1");
            else
                manaRegenText.text = "";
        }
    }

    private void SkillLimit()
    {
        int level = myChanmpionData.myStat.Level;
        int q_level = myChanmpionData.skill_Q;
        int w_level = myChanmpionData.skill_W;
        int e_level = myChanmpionData.skill_E;
        int r_level = myChanmpionData.skill_R;

        for (int i = 0; i < 4; i++)
        {
            skillUpButton[i].SetActive(false);
            skillUpButton2[i].SetActive(false);
        }

        // 궁극기 스킬렙 6, 11, 16 제한
        if (level >= 6 && level < 11 && r_level < 1)
            skillUpButton[3].SetActive(true);
        else if (level >= 11 && level < 16 && r_level < 2)
            skillUpButton[3].SetActive(true);
        else if (level >= 16 && r_level < 3)
            skillUpButton[3].SetActive(true);
        else
            skillUpButton2[3].SetActive(true);

        // 스킬마다 1 3 5 7 9렙에 스킬레벨제한 풀림 Q
        if (level < 3 && q_level < 1)
            skillUpButton[0].SetActive(true);
        else if (level >= 3 && level < 5 && q_level < 2)
            skillUpButton[0].SetActive(true);
        else if (level >= 5 && level < 7 && q_level < 3)
            skillUpButton[0].SetActive(true);
        else if (level >= 7 && level < 9 && q_level < 4)
            skillUpButton[0].SetActive(true);
        else if (level >= 9 && q_level < 5)
            skillUpButton[0].SetActive(true);
        else
            skillUpButton2[0].SetActive(true);

        // W
        if (level < 3 && w_level < 1)
            skillUpButton[1].SetActive(true);
        else if (level >= 3 && level < 5 && w_level < 2)
            skillUpButton[1].SetActive(true);
        else if (level >= 5 && level < 7 && w_level < 3)
            skillUpButton[1].SetActive(true);
        else if (level >= 7 && level < 9 && w_level < 4)
            skillUpButton[1].SetActive(true);
        else if (level >= 9 && w_level < 5)
            skillUpButton[1].SetActive(true);
        else
            skillUpButton2[1].SetActive(true);

        // E
        if (level < 3 && e_level < 1)
            skillUpButton[2].SetActive(true);
        else if (level >= 3 && level < 5 && e_level < 2)
            skillUpButton[2].SetActive(true);
        else if (level >= 5 && level < 7 && e_level < 3)
            skillUpButton[2].SetActive(true);
        else if (level >= 7 && level < 9 && e_level < 4)
            skillUpButton[2].SetActive(true);
        else if (level >= 9 && e_level < 5)
            skillUpButton[2].SetActive(true);
        else
            skillUpButton2[2].SetActive(true);
    }

    public int GetSkillPoint()
    {
        return skillpoint;
    }
}
