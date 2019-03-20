using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


public class StatClass
{
    private static StatClass _instance;
    public static StatClass Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new StatClass();
            }
            return _instance;
        }
    }

    private StatClass()
    {
        SetJson();
    }

    public class Stat
    {
        public int Level = 0;
        public int Exp = 0;
        public int RequireExp = 0;
        public float Hp = 0;
        public float MaxHp = 0;
        public float Mp = 0;
        public float MaxMp = 0;
        public float AttackDamage = 0;
        public float AbilityPower = 0;
        public float AttackSpeed = 0;
        public float AttackDef = 0;
        public float AbilityDef = 0;
        public float CoolTimeDecrease = 0;
        public float CriticalPercentage = 0;
        public float MoveSpeed = 0;
        public float AttackRange = 0;
        public int Gold = 0;
        public float FirstCreateTime = 0;
        public float RespawnTime = 0;
        public float ExpIncrease = 0;
        public float HealthRegen = 0;
        public float ManaRegen = 0;
        public float UP_Hp = 0;
        public float UP_Mp = 0;
        public float UP_HpRegen = 0;
        public float UP_MpRegen = 0;
        public float UP_AttackDamage = 0;
        public float UP_AttackSpeed = 0;
        public float UP_Def = 0;
        public float UP_MagicDef = 0;

        public Stat ClassCopy()
        {
            return (Stat)this.MemberwiseClone();
        }
    }
    public Dictionary<string, Stat> characterData = new Dictionary<string, Stat>();
    public int[] requireExp = new int[17]
        {280, 380, 480, 580, 680, 780, 880, 980, 1080, 1180
        , 1280, 1380, 1480, 1580, 1680, 1780, 1880};


    public void SetJson()
    {
        string json = System.IO.File.ReadAllText(Application.streamingAssetsPath + "/Json/AOS_Stats.json");
        JObject parse = JObject.Parse(json);

        string[] dataName = new string[] {"Jungle_Frog", "Jungle_Frog2", "Jungle_Blue", "Jungle_Blue2"
            , "Jungle_BWolf", "Jungle_BWolf2", "Jungle_SWolf", "Jungle_SWolf2", "Jungle_BKalnal"
            , "Jungle_BKalnal2", "Jungle_SKalnal", "Jungle_SKalnal2", "Jungle_BGolem"
            , "Jungle_SGolem", "Jungle_Crab", "Jungle_Red", "Jungle_Red2", "Jungle_Dragon1"
            , "Jungle_Dragon2", "Jungle_Dragon3", "Jungle_Dragon4", "Jungle_ElderDragon"
            ,"Minion_Warrior","Minion_Magician","Minion_Super","Minion_Siege"
            ,"Ashe", "Garen", "Mundo", "Alistar", "Ahri"
            ,"Jungle_RiftHerald","Jungle_RiftHerald2","Jungle_Baron","Jungle_Baron2"};

        for (int i = 0; i < dataName.Length; ++i)
        {
            Stat stat = new Stat();

            stat.Level = parse.SelectToken(dataName[i]).SelectToken("Level").Value<int>();
            stat.Exp = parse.SelectToken(dataName[i]).SelectToken("Exp").Value<int>();
            stat.RequireExp = parse.SelectToken(dataName[i]).SelectToken("RequireExp").Value<int>();
            stat.Hp = parse.SelectToken(dataName[i]).SelectToken("Hp").Value<float>();
            stat.MaxHp = parse.SelectToken(dataName[i]).SelectToken("MaxHp").Value<float>();
            stat.Mp = parse.SelectToken(dataName[i]).SelectToken("Mp").Value<float>();
            stat.MaxMp = parse.SelectToken(dataName[i]).SelectToken("MaxMp").Value<float>();
            stat.AttackDamage = parse.SelectToken(dataName[i]).SelectToken("Attack_Damage").Value<float>();
            stat.AbilityPower = parse.SelectToken(dataName[i]).SelectToken("Ability_Power").Value<float>();
            stat.AttackSpeed = parse.SelectToken(dataName[i]).SelectToken("Attack_Speed").Value<float>();
            stat.AttackDef = parse.SelectToken(dataName[i]).SelectToken("Attack_Def").Value<float>();
            stat.AbilityDef = parse.SelectToken(dataName[i]).SelectToken("Ability_Def").Value<float>();
            stat.CoolTimeDecrease = parse.SelectToken(dataName[i]).SelectToken("CoolTime_Decrease").Value<float>();
            stat.CriticalPercentage = parse.SelectToken(dataName[i]).SelectToken("Critical_Percentage").Value<float>();
            stat.MoveSpeed = parse.SelectToken(dataName[i]).SelectToken("Move_Speed").Value<float>() / 50f;
            stat.AttackRange = parse.SelectToken(dataName[i]).SelectToken("Attack_Range").Value<float>();
            stat.Gold = parse.SelectToken(dataName[i]).SelectToken("Gold").Value<int>();
            stat.FirstCreateTime = parse.SelectToken(dataName[i]).SelectToken("first_Create_Time").Value<float>();
            stat.RespawnTime = parse.SelectToken(dataName[i]).SelectToken("Respawn_Time").Value<float>();
            stat.ExpIncrease = parse.SelectToken(dataName[i]).SelectToken("Exp_Increase").Value<float>();
            stat.HealthRegen = parse.SelectToken(dataName[i]).SelectToken("Health_Regen").Value<float>();
            stat.ManaRegen = parse.SelectToken(dataName[i]).SelectToken("Mana_Regen").Value<float>();
            stat.UP_Hp = parse.SelectToken(dataName[i]).SelectToken("UP_HP").Value<float>();
            stat.UP_Mp = parse.SelectToken(dataName[i]).SelectToken("UP_MP").Value<float>();
            stat.UP_MagicDef = parse.SelectToken(dataName[i]).SelectToken("UP_MagicDef").Value<float>();
            stat.UP_MpRegen = parse.SelectToken(dataName[i]).SelectToken("UP_MPRegen").Value<float>();
            stat.UP_AttackDamage = parse.SelectToken(dataName[i]).SelectToken("UP_AttackDamage").Value<float>();
            stat.UP_AttackSpeed = parse.SelectToken(dataName[i]).SelectToken("UP_AttackSpeed").Value<float>();
            stat.UP_Def = parse.SelectToken(dataName[i]).SelectToken("UP_Def").Value<float>();
            stat.UP_MagicDef = parse.SelectToken(dataName[i]).SelectToken("UP_MagicDef").Value<float>();

            characterData.Add(dataName[i], stat);
        }
    }
}

