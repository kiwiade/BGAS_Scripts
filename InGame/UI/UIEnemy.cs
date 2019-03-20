using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIEnemy : MonoBehaviour
{
    private StatClass.Stat stat;
    private StatClass.Stat originStat;

    [SerializeField]
    private Text attackDamageText;
    [SerializeField]
    private Text abilityPowerText;
    [SerializeField]
    private Text defenceText;
    [SerializeField]
    private Text magicResistText;
    [SerializeField]
    private Text attackSpeedText;
    [SerializeField]
    private Text cooldownReduceText;
    [SerializeField]
    private Text criticalText;
    [SerializeField]
    private Text moveSpeedText;

    [Space]
    [SerializeField]
    private Image icon;
    [SerializeField]
    private Image[] itemicon;
    [SerializeField]
    private Text levelText;
    [SerializeField]
    private Text csText;
    [SerializeField]
    private Text kdaText;

    [Space]
    [SerializeField]
    private ProgressBar healthBar;
    [SerializeField]
    private ProgressBar manaBar;

    [HideInInspector]
    public GameObject selectedObject;

    private ChampionData selectedChampionData;
    private MinionBehavior selectedMinionBehaviour;
    private TowerBehaviour selectedTowerBehaviour;
    private MonsterBehaviour selectedMonsterBehaviour;

    enum SelectType
    {
        Player = 1,
        Tower = 2,
        Minion = 3,
        Monster = 4
    }
    private SelectType selectType;

    void Update()
    {
        if (selectedObject)
        {
            StatUpdate();

            // 플레이어면 kda, cs도 갱신해주기
            if (selectType == SelectType.Player)
            {
                ItemUpdate();
            }

            HealthBarUpdate();

            // 선택된 오브젝트가 죽어서 풀로 돌아가고 액티브 꺼지면 나도 끔
            if(!selectedObject.activeSelf)
                gameObject.SetActive(false);

            // 챔피언 또는 혹시 안꺼진 오브젝트라도 스탯 확인하여 HP가 1보다 작아지면 끔
            if(stat.Hp < 1)
                gameObject.SetActive(false);
        }
    }

    public void ApplyObject(GameObject go)
    {
        selectedObject = go;

        // 챔피언을 눌렀을때
        if (go.layer.Equals(LayerMask.NameToLayer("Champion")))
        {
            selectType = SelectType.Player;
            selectedChampionData = go.GetComponent<ChampionData>();

            // 챔피언의 스탯을 가져와서 스탯업데이트
            stat = selectedChampionData.totalStat;
            originStat = selectedChampionData.myStat;
            StatUpdate();

            // 챔피언의 아이콘을 가져와서 아이콘 업데이트
            icon.sprite = Resources.Load<Sprite>("Champion/ChampionIcon/" + selectedChampionData.championName);

            // 챔피언의 아이템을 가져와서 아이템 업데이트
            ItemUpdate();
        }

        else if (go.layer.Equals(LayerMask.NameToLayer("Monster")))
        {
            selectType = SelectType.Monster;
            selectedMonsterBehaviour = go.GetComponent<MonsterBehaviour>();
            stat = selectedMonsterBehaviour.stat;
            StatUpdate();
            string monsterName = go.name;
            icon.sprite = Resources.Load<Sprite>("Icon/Monster/" + monsterName.Split('(')[0]);
        }

        // 미니언을 눌렀을때
        else if (go.CompareTag("Minion"))
        {
            selectType = SelectType.Minion;
            selectedMinionBehaviour = go.GetComponent<MinionBehavior>();

            stat = selectedMinionBehaviour.stat;
            StatUpdate();

            if (selectedMinionBehaviour.name.Contains("Red"))
            {
                if (selectedMinionBehaviour.name.Contains("Magician"))
                    icon.sprite = Resources.Load<Sprite>("Icon/Minion_caster_red");
                else if (selectedMinionBehaviour.name.Contains("Melee"))
                    icon.sprite = Resources.Load<Sprite>("Icon/Minion_melee_red");
                else if (selectedMinionBehaviour.name.Contains("Siege"))
                    icon.sprite = Resources.Load<Sprite>("Icon/Minion_siege_red");
            }
            else if (selectedMinionBehaviour.name.Contains("Blue"))
            {
                if (selectedMinionBehaviour.name.Contains("Magician"))
                    icon.sprite = Resources.Load<Sprite>("Icon/Minion_caster_blue");
                else if (selectedMinionBehaviour.name.Contains("Melee"))
                    icon.sprite = Resources.Load<Sprite>("Icon/Minion_melee_blue");
                else if (selectedMinionBehaviour.name.Contains("Siege"))
                    icon.sprite = Resources.Load<Sprite>("Icon/Minion_siege_blue");
            }
            else
                icon.sprite = null;

            for (int i = 0; i < itemicon.Length; i++)
            {
                itemicon[i].gameObject.GetComponent<ItemInfo>().myItem = null;
                itemicon[i].sprite = null;
                itemicon[i].color = new Color(14f / 255f, 26f / 255f, 23f / 255f, 1);
            }
        }

        // 타워를 눌렀을때
        else if (go.CompareTag("Tower"))
        {
            selectType = SelectType.Tower;
            selectedTowerBehaviour = go.GetComponent<TowerBehaviour>();

            stat = selectedTowerBehaviour.towerstat;
            StatUpdate();

            if (selectedTowerBehaviour.Team.Equals("Red"))
                icon.sprite = Resources.Load<Sprite>("Icon/Tower_Icon_Red");
            else if (selectedTowerBehaviour.Team.Equals("Blue"))
                icon.sprite = Resources.Load<Sprite>("Icon/Tower_Icon_Blue");
            else
                icon.sprite = null;

            for (int i = 0; i < itemicon.Length; i++)
            {
                itemicon[i].gameObject.GetComponent<ItemInfo>().myItem = null;
                itemicon[i].sprite = null;
                itemicon[i].color = new Color(14f / 255f, 26f / 255f, 23f / 255f, 1);
            }
        }
    }

    public void StatUpdate()
    {
        if (stat == null)
            return;

        attackDamageText.text = Mathf.RoundToInt(stat.AttackDamage).ToString();
        abilityPowerText.text = Mathf.RoundToInt(stat.AbilityPower).ToString();
        defenceText.text = Mathf.RoundToInt(stat.AttackDef).ToString();
        magicResistText.text = Mathf.RoundToInt(stat.AbilityDef).ToString();

        if (selectType == SelectType.Player)
        {
            float AS = originStat.AttackSpeed * (1 + (stat.UP_AttackSpeed * (stat.Level - 1) + (stat.AttackSpeed - originStat.AttackSpeed)) / 100);
            attackSpeedText.text = System.Math.Round(AS, 2).ToString();
        }
        else
            attackSpeedText.text = System.Math.Round(stat.AttackSpeed, 2).ToString();

        cooldownReduceText.text = Mathf.RoundToInt(stat.CoolTimeDecrease).ToString();
        criticalText.text = Mathf.RoundToInt(stat.CriticalPercentage).ToString();
        moveSpeedText.text = Mathf.RoundToInt(stat.MoveSpeed * 50f).ToString();

        levelText.text = stat.Level.ToString();
    }

    public void ItemUpdate()
    {
        for (int i = 0; i < 6; i++)
        {
            if (selectedChampionData.item[i] != 0)
            {
                ShopItem.Item it = ShopItem.Instance.itemlist[selectedChampionData.item[i]];
                // 원본의 주소를 가져오므로 변경해서는 myItem을 변경해서는 안됨.
                itemicon[i].gameObject.GetComponent<ItemInfo>().myItem = it;
                itemicon[i].sprite = Resources.Load<Sprite>("Item_Image/" + it.iconName);
                itemicon[i].color = Color.white;
            }
            else
            {
                itemicon[i].gameObject.GetComponent<ItemInfo>().myItem = null;
                itemicon[i].sprite = null;
                itemicon[i].color = new Color(14f / 255f, 26f / 255f, 23f / 255f, 1);
            }
        }

        if (selectedChampionData.accessoryItem != 0)
        {
            ShopItem.Item it = ShopItem.Instance.itemlist[selectedChampionData.accessoryItem];
            itemicon[6].gameObject.GetComponent<ItemInfo>().myItem = it;
            itemicon[6].sprite = Resources.Load<Sprite>("Item_Image/" + it.iconName);
            itemicon[6].color = Color.white;
        }
        else
        {
            itemicon[6].gameObject.GetComponent<ItemInfo>().myItem = null;
            itemicon[6].sprite = null;
            itemicon[6].color = new Color(14f / 255f, 26f / 255f, 23f / 255f, 1);
        }
    }

    public void HealthBarUpdate()
    {
        healthBar.value = stat.Hp / stat.MaxHp;
        healthBar.text = Mathf.FloorToInt(stat.Hp).ToString() + " / " + Mathf.FloorToInt(stat.MaxHp).ToString();

        manaBar.value = stat.Mp / stat.MaxMp;
        manaBar.text = Mathf.FloorToInt(stat.Mp).ToString() + " / " + Mathf.FloorToInt(stat.MaxMp).ToString();
    }
}
