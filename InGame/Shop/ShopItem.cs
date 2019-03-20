using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ShopItem : Singleton<ShopItem> {

    private void Awake()
    {
        ItemRead();
    }

    public class Item
    {
        // 기본정보
        public int id = 0;
        public string name = "";
        public int price = 0;
        public string iconName = "";

        // 하위템
        public int subitemID1 = 0;
        public int subitemID2 = 0;
        public int subitemID3 = 0;

        // 액티브여부
        public bool active = false;
        public int activeCooldown = 0;

        // 스탯
        public int attackDamage = 0;
        public int attackSpeed = 0;
        public int criticalPercent = 0;
        public int lifeSteal = 0;

        public int abilityPower = 0;
        public int mana = 0;
        public int manaRegen = 0;
        public int cooldownReduce = 0;

        public int armor = 0;
        public int magicResist = 0;
        public int health = 0;
        public int healthRegen = 0;

        public float movementSpeed = 0;

        // 상점 분류용
        public bool consumable = false;
        public bool boots = false;
        public bool accessory = false;

        public string effect_kind = "";
        public string effect_description = "";
        public string additional_kind = "";
        public string additional_description = "";

        public Item ClassCopy()
        {
            return (Item)this.MemberwiseClone();
        }
    }

    public Dictionary<int, Item> itemlist = new Dictionary<int, Item>();
    public List<Item> sortedItemList = new List<Item>();
    public List<Item> searchItemList = new List<Item>();
    public List<Item> makingItemList = new List<Item>();

    // 아이템리스트에서 아이템을 읽어오는 함수
    private void ItemRead()
    {
        itemlist.Clear();

        string fileName = Application.streamingAssetsPath;
        fileName = Path.Combine(fileName, "csv/itemlist.csv");
        if (File.Exists(fileName) == false)
            return;

        FileStream fStream = new FileStream(fileName, FileMode.Open);
        if (fStream != null)
        {
            StreamReader streamReader = new StreamReader(fStream);
            string itemcsv = streamReader.ReadToEnd();
            string[] lines = itemcsv.Split("\r\n".ToCharArray());

            foreach (string line in lines)
            {
                Item newitem = new Item();

                if (line.Length > 0)
                {
                    string[] data = line.Split(',');

                    newitem.id = int.Parse(data[0]);
                    newitem.name = data[1];
                    newitem.price = int.Parse(data[2]);
                    newitem.iconName = data[3];

                    if(data[4] != string.Empty)
                        newitem.subitemID1 = int.Parse(data[4]);
                    if(data[5] != string.Empty)
                        newitem.subitemID2 = int.Parse(data[5]);
                    if(data[6] != string.Empty)
                        newitem.subitemID3 = int.Parse(data[6]);

                    if (data[7] != string.Empty)
                        newitem.active = true;
                    if (data[8] != string.Empty)
                        newitem.activeCooldown = int.Parse(data[8]);

                    if (data[9] != string.Empty)
                        newitem.attackDamage = int.Parse(data[9]);
                    if (data[10] != string.Empty)
                        newitem.attackSpeed = int.Parse(data[10]);
                    if (data[11] != string.Empty)
                        newitem.criticalPercent = int.Parse(data[11]);
                    if (data[12] != string.Empty)
                        newitem.lifeSteal = int.Parse(data[12]);

                    if (data[13] != string.Empty)
                        newitem.abilityPower = int.Parse(data[13]);
                    if (data[14] != string.Empty)
                        newitem.mana = int.Parse(data[14]);
                    if (data[15] != string.Empty)
                        newitem.manaRegen = int.Parse(data[15]);
                    if (data[16] != string.Empty)
                        newitem.cooldownReduce = int.Parse(data[16]);

                    if (data[17] != string.Empty)
                        newitem.armor = int.Parse(data[17]);
                    if (data[18] != string.Empty)
                        newitem.magicResist = int.Parse(data[18]);
                    if (data[19] != string.Empty)
                        newitem.health = int.Parse(data[19]);
                    if (data[20] != string.Empty)
                        newitem.healthRegen = int.Parse(data[20]);

                    if (data[21] != string.Empty)
                        newitem.movementSpeed = float.Parse(data[21]);

                    if (data[22] != string.Empty)
                        newitem.consumable = true;
                    if (data[23] != string.Empty)
                        newitem.boots = true;
                    if (data[24] != string.Empty)
                        newitem.accessory = true;

                    if (data[25] != string.Empty)
                        newitem.effect_kind = data[25];
                    if (data[26] != string.Empty)
                        newitem.effect_description = data[26];
                    if (data[27] != string.Empty)
                        newitem.additional_kind = data[27];
                    if (data[28] != string.Empty)
                        newitem.additional_description = data[28];

                    itemlist[newitem.id] = newitem;
                }
            }
            streamReader.Close();
            fStream.Close();
        }
    }
}
