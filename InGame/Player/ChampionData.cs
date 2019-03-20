using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChampionData : Photon.MonoBehaviour, IPunObservable
{
    public Skills playerSkill;
    // 기본스탯, 스킬
    public StatClass.Stat myStat = new StatClass.Stat();
    public SkillClass.Skill mySkill = new SkillClass.Skill();
    public ShopItem.Item itemStat = new ShopItem.Item();
    public StatClass.Stat totalStat = null;

    // 챔피언명
    [HideInInspector]
    public string championName = "Ashe";

    // 스킬레벨
    public int skill_Q = 0;
    public int skill_W = 0;
    public int skill_E = 0;
    public int skill_R = 0;
    public float Cooldown_Passive = 0;
    public float Cooldown_Q = 0;
    public float Cooldown_W = 0;
    public float Cooldown_E = 0;
    public float Cooldown_R = 0;
    public float current_Cooldown_Passive = 0;
    public float current_Cooldown_Q = 0;
    public float current_Cooldown_W = 0;
    public float current_Cooldown_E = 0;
    public float current_Cooldown_R = 0;
    // 스킬 쿨타임도중 스킬레벨업하여 쿨감소 방지하기위해
    public float temp_Cooldown_Q = 0;
    public float temp_Cooldown_W = 0;
    public float temp_Cooldown_E = 0;
    public float temp_Cooldown_R = 0;
    public float mana_Q = 0;
    public float mana_W = 0;
    public float mana_E = 0;
    public float mana_R = 0;

    // 스펠쿨타임. ID는 PlayerData에서 받아옴.(왜냐면 캐릭터 선택창에서 받아와야하니까)
    public int spell_D = 0;
    public int spell_F = 0;
    public float Cooldown_D = 0;
    public float Cooldown_F = 0;
    public float current_Cooldown_D = 0;
    public float current_Cooldown_F = 0;

    //아이템
    public int[] item = null;
    public int accessoryItem = 0;

    // 귀환시간
    private float recallTime = 8.0f;
    private float currentRecallTime = 8.0f;
    [HideInInspector]
    public bool isRecallStart = false;
    [HideInInspector]
    public Vector3 redPos;
    [HideInInspector]
    public Vector3 bluePos;

    //킬데스어시cs
    public int kill = 0;
    public int death = 0;
    public int assist = 0;
    public int cs = 0;

    // 오브젝트 받아옴
    public GameObject UIRecall;
    public UIStat UIStat;
    public UIIcon UIIcon;
    private UISkill UISkill;
    public UIRightTop UIRightTop;

    // 경험치 테스트용
    private float testTime = 0;
    private float regenTime = 0;

    // 컨트롤로 스킬찍게 체크
    private bool ctrlcheck = false;

    //UI캔버스 찾았는지 체크
    private bool isUICanvasFind = false;

    //리콜시 못움직이게 현재위치 저장
    private Vector3 currentPos = Vector3.zero;
    //리콜시 체력 저장
    private float currentHp = 0;
    //채팅시 스펠 못하게
    private ChatFunction chatFunction;
    private PlayerData playerData;

    // 애니메이션을 위해 astar Target을 받아옴
    Pathfinding.AIDestinationSetter myAIDestinationSetter;
    public Animator myAnimator;
    private PlayerSpell playerSpell;

    //스킬 사용에 체력을 쓰는 애들 용
    public bool isNoMP = false;

    //스킬 쓰면 일정 시간 데미지, 물방, 마방, 이속 올라가는 거 반영
    public float skillPlusAtkDam = 0;
    public float skillPlusAtkDef = 0;
    public float skillPlusAbilDef = 0;
    public float skillPlusSpeed = 0;
    public AIPath theAIPath;
    PhotonView myPhotonView;
    public bool canSkill = true;

    // 포션체크
    private bool hpPotion = false;
    private bool mpPotion = false;
    private float hpPotionTimeCheck = 15.0f;
    private float mpPotionTimeCheck = 15.0f;
    private float hpPotionCycle = 0.5f;
    private float mpPotionCycle = 0.5f;

    private CsTextPool csTextPool;//CSText
    private AudioSource audioSource;
    bool effectOnce = false;
    bool effectOnce2 = false;
    bool recallComplete = false;

    private StackImage theStackImage = null;

    private void Awake()
    {
        myPhotonView = GetComponent<PhotonView>();
        if (photonView.isMine)
        {
            championName = PlayerData.Instance.championName;
            if (championName.Contains("Mundo"))
                isNoMP = true;
            SetSpell();
            SetStatSkill(championName);

            item = PlayerData.Instance.item;
        }
        else
        {
            item = new int[6];
        }
        totalStat = myStat.ClassCopy();

        myAIDestinationSetter = GetComponent<Pathfinding.AIDestinationSetter>();
        myAnimator = GetComponent<Animator>();
        theAIPath = GetComponent<AIPath>();
        playerSpell = GetComponent<PlayerSpell>();
        playerData = PlayerData.Instance;
        audioSource = GetComponent<AudioSource>();

        int ran = Random.Range(1, 9);
        redPos = new Vector3(4 + ran, 0.5f, 10f);
        bluePos = new Vector3(262 + ran, 0.5f, 270f);
    }

    private void OnLevelWasLoaded(int level)
    {
        PhotonNetwork.isMessageQueueRunning = true;
        if (UnityEngine.SceneManagement.SceneManager.GetSceneByBuildIndex(level).name.Equals("InGame"))
            Invoke("FindUICanvas", 3f);
    }

    public void FindUICanvas()
    {
        UICanvas UIcanvas = GameObject.FindGameObjectWithTag("UICanvas").GetComponent<UICanvas>();
        UIStat = UIcanvas.stat.GetComponent<UIStat>();
        UIIcon = UIcanvas.icon.GetComponent<UIIcon>();
        UISkill = UIcanvas.skill.GetComponent<UISkill>();
        UIRecall = UIcanvas.recall;
        UIRightTop = UIcanvas.rightTopUI.GetComponent<UIRightTop>();
        isUICanvasFind = true;

        chatFunction = GameObject.FindGameObjectWithTag("ChatManager").GetComponentInChildren<ChatFunction>();
        csTextPool = GameObject.FindGameObjectWithTag("CSText").GetComponent<CsTextPool>();
    }

    void Update()
    {
        if (!photonView.isMine)
            return;

        if (isUICanvasFind)
        {
            // Ctrl눌렀는지 체크
            CtrlCheck();

            // 스킬
            SkillCheck();

            //스펠
            SpellCheck();

            // Recall
            RecallCheck();

            // 체력, 마나재생
            HealthManaRegen();

            // 아이템
            ItemCheck();

            ////경험치 증가 테스트
            //testTime += Time.deltaTime;
            //if (testTime >= 1.0f && mystat.Level < 18)
            //{
            //    mystat.Exp += 150;
            //    testTime = 0;
            //}

            if (totalStat.Level == 18)
                return;

            if (myStat.Exp > myStat.RequireExp)
                LevelUp();
        }
        if (recallComplete)
        {
            theAIPath.canMove = true;
            if (PhotonNetwork.player.GetTeam().ToString().Contains("red"))
            {
                transform.position = redPos;
                playerSkill.TheChampionAtk.AStarTargetObj.transform.position = redPos;
            }
            else if (PhotonNetwork.player.GetTeam().ToString().Contains("blue"))
            {
                transform.position = bluePos;
                playerSkill.TheChampionAtk.AStarTargetObj.transform.position = bluePos;
            }
            recallComplete = false;
        }
    }

    public void HealthManaRegen()
    {
        if (playerData.isDead || totalStat.Hp <= 0)
            return;

        regenTime += Time.deltaTime;
        if (regenTime >= 0.5f)
        {
            regenTime -= 0.5f;
            if (totalStat.Hp < totalStat.MaxHp)
                totalStat.Hp += totalStat.HealthRegen * 0.1f;
            if (totalStat.Mp < totalStat.MaxMp)
                totalStat.Mp += totalStat.ManaRegen * 0.1f;

            if (totalStat.Hp > totalStat.MaxHp)
                totalStat.Hp = totalStat.MaxHp;
            if (totalStat.Mp > totalStat.MaxMp)
                totalStat.Mp = totalStat.MaxMp;
        }
    }

    public void ItemCheck()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            PotionCheck(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            PotionCheck(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            PotionCheck(3);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            PotionCheck(4);
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            PotionCheck(5);
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            PotionCheck(6);
        }

        if (hpPotion)
        {
            if (!effectOnce)
            {
                effectOnce = true;
                playerSpell.SendEffect("HealPotion", transform.position, PhotonNetwork.player.GetTeam().ToString().ToLower(), myPhotonView.viewID);
                if (theStackImage == null)
                {
                    theStackImage = GameObject.FindGameObjectWithTag("StackImage").GetComponent<StackImage>();
                }
                if (!theStackImage.ImageDic["HPPotion"].gameObject.activeInHierarchy)
                {
                    theStackImage.ImageDic["HPPotion"].gameObject.SetActive(true);
                }
            }
            // 15초동안 150회복. 초당 10회복. 0.5초당 5회복
            hpPotionCycle -= Time.deltaTime;

            if (hpPotionCycle < 0)
            {
                if (totalStat.Hp < totalStat.MaxHp)
                    totalStat.Hp += 5;
                if (totalStat.Hp > totalStat.MaxHp)
                    totalStat.Hp = totalStat.MaxHp;
                hpPotionCycle = 0.5f;
            }

            // 15초가 다 지나면 포션사용가능
            hpPotionTimeCheck -= Time.deltaTime;
            theStackImage.TextDic["HPPotion"].text = Mathf.FloorToInt(hpPotionTimeCheck).ToString();
            if (hpPotionTimeCheck < 0)
            {
                effectOnce = false;
                hpPotionTimeCheck = 15.0f;
                hpPotion = false;
                theStackImage.TextDic["HPPotion"].text = "";
                theStackImage.ImageDic["HPPotion"].gameObject.SetActive(false);
            }
        }

        if (mpPotion)
        {
            if (!effectOnce2)
            {
                effectOnce2 = true;
                playerSpell.SendEffect("HealPotion", transform.position, PhotonNetwork.player.GetTeam().ToString().ToLower(), myPhotonView.viewID);
                if (theStackImage == null)
                {
                    theStackImage = GameObject.FindGameObjectWithTag("StackImage").GetComponent<StackImage>();
                }
                if (!theStackImage.ImageDic["MPPotion"].gameObject.activeInHierarchy)
                {
                    theStackImage.ImageDic["MPPotion"].gameObject.SetActive(true);
                }
            }
            // 15초동안 100회복. 초당 6.666회복. 0.5초당 3.3333회복
            mpPotionCycle -= Time.deltaTime;

            if (mpPotionCycle < 0)
            {

                if (totalStat.Mp < totalStat.MaxMp)
                    totalStat.Mp += 5;
                if (totalStat.Mp > totalStat.MaxMp)
                    totalStat.Mp = totalStat.MaxMp;
                mpPotionCycle = 0.5f;
            }

            // 15초가 다 지나면 포션사용가능
            mpPotionTimeCheck -= Time.deltaTime;
            theStackImage.TextDic["MPPotion"].text = Mathf.FloorToInt(mpPotionTimeCheck).ToString();
            if (mpPotionTimeCheck < 0)
            {
                effectOnce2 = false;
                mpPotionTimeCheck = 15.0f;
                mpPotion = false;
                theStackImage.TextDic["MPPotion"].text = "";
                theStackImage.ImageDic["MPPotion"].gameObject.SetActive(false);
            }
        }
    }

    public void PotionCheck(int itemSlotNum)
    {
        // 일단은 bool값 검사해서 사용중이면 못사용하게함.
        // 체력포션이면
        if (item[itemSlotNum - 1] == 2 && !hpPotion)
        {
            PlayerData.Instance.item[itemSlotNum - 1] = 0;
            PlayerData.Instance.ItemUpdate();
            PlayerData.Instance.ItemUndoListReset();
            hpPotion = true;
        }
        // 마나포션이면
        else if (item[itemSlotNum - 1] == 3 && !mpPotion)
        {
            PlayerData.Instance.item[itemSlotNum - 1] = 0;
            PlayerData.Instance.ItemUpdate();
            PlayerData.Instance.ItemUndoListReset();
            mpPotion = true;
        }
    }

    public void CtrlCheck()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            ctrlcheck = true;
        }
        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            ctrlcheck = false;
        }
    }

    public void SkillCheck()
    {
        if (ctrlcheck && Input.GetKeyDown(KeyCode.Q))
        {
            if (UISkill.GetSkillPoint() >= 1 && UISkill.skillUpButton[0].activeSelf)
                UISkill.SkillUp("Q");
        }
        if (ctrlcheck && Input.GetKeyDown(KeyCode.W))
        {
            if (UISkill.GetSkillPoint() >= 1 && UISkill.skillUpButton[1].activeSelf)
                UISkill.SkillUp("W");
        }
        if (ctrlcheck && Input.GetKeyDown(KeyCode.E))
        {
            if (UISkill.GetSkillPoint() >= 1 && UISkill.skillUpButton[2].activeSelf)
                UISkill.SkillUp("E");
        }
        if (ctrlcheck && Input.GetKeyDown(KeyCode.R))
        {
            if (UISkill.GetSkillPoint() >= 1 && UISkill.skillUpButton[3].activeSelf)
                UISkill.SkillUp("R");
        }

        if (!playerSkill || !chatFunction)
            return;

        // 현재쿨타임이 0일때만(쿨타임이 안돌아갈때만) 스킬 써짐
        if (canSkill && !playerData.isDead)
        {
            if (!playerSkill.isSkillIng && !chatFunction.chatInput.IsActive())
            {
                if (Input.GetKeyDown(KeyCode.Q) && !ctrlcheck)
                {
                    if (current_Cooldown_Q == 0 && skill_Q >= 1)
                    {
                        if (isNoMP)
                        {
                            if (totalStat.Hp - 2 > mana_Q)
                            {
                                playerSkill.QCasting();
                            }
                        }
                        else if (totalStat.Mp >= mana_Q)
                        {
                            playerSkill.QCasting();
                        }
                    }
                }
                if (Input.GetKeyDown(KeyCode.W) && !ctrlcheck)
                {
                    if (current_Cooldown_W == 0 && skill_W >= 1)
                    {
                        if (isNoMP)
                        {
                            if (totalStat.Hp - 2 > mana_W)
                            {
                                playerSkill.WCasting();
                            }
                        }
                        else if (totalStat.Mp >= mana_W)
                        {
                            playerSkill.WCasting();
                        }
                    }
                }
                if (Input.GetKeyDown(KeyCode.E) && !ctrlcheck)
                {
                    if (current_Cooldown_E == 0 && skill_E >= 1)
                    {
                        if (isNoMP)
                        {
                            if (totalStat.Hp - 2 > mana_E)
                            {
                                playerSkill.ECasting();
                            }
                        }
                        else if (totalStat.Mp >= mana_E)
                        {
                            playerSkill.ECasting();
                        }
                    }
                }
                if (Input.GetKeyDown(KeyCode.R) && !ctrlcheck)
                {
                    if (current_Cooldown_R == 0 && skill_R >= 1)
                    {
                        if (isNoMP)
                        {
                            if (totalStat.Hp - 2 > mana_R)
                            {
                                playerSkill.RCasting();
                            }
                        }
                        else if (totalStat.Mp >= mana_R)
                        {
                            playerSkill.RCasting();
                        }
                    }
                }
            }
        }
        // 스킬쿨이 돌고있으면 시간마다 점점 쿨감소
        if (current_Cooldown_Q != 0)
        {
            current_Cooldown_Q -= Time.deltaTime;
            if (current_Cooldown_Q < 0)
            {
                current_Cooldown_Q = 0;
                UISkill.skillDisabledImage[1].SetActive(false);
                UISkill.skillCooldownImage[1].fillAmount = 0;
                UISkill.skillCooldownText[1].text = "";
            }
            else
            {
                // 이미지와 텍스트 갱신
                UISkill.skillCooldownImage[1].fillAmount = current_Cooldown_Q / temp_Cooldown_Q;
                if (current_Cooldown_Q > 1.0f)
                    UISkill.skillCooldownText[1].text = Mathf.FloorToInt(current_Cooldown_Q).ToString();
                else
                    UISkill.skillCooldownText[1].text = current_Cooldown_Q.ToString("N1");
            }
        }
        if (current_Cooldown_W != 0)
        {
            current_Cooldown_W -= Time.deltaTime;
            if (current_Cooldown_W < 0)
            {
                current_Cooldown_W = 0;
                UISkill.skillDisabledImage[2].SetActive(false);
                UISkill.skillCooldownImage[2].fillAmount = 0;
                UISkill.skillCooldownText[2].text = "";
            }
            else
            {
                // 이미지와 텍스트 갱신
                UISkill.skillCooldownImage[2].fillAmount = current_Cooldown_W / temp_Cooldown_W;
                if (current_Cooldown_W > 1.0f)
                    UISkill.skillCooldownText[2].text = Mathf.FloorToInt(current_Cooldown_W).ToString();
                else
                    UISkill.skillCooldownText[2].text = current_Cooldown_W.ToString("N1");
            }
        }
        if (current_Cooldown_E != 0)
        {
            current_Cooldown_E -= Time.deltaTime;
            if (current_Cooldown_E < 0)
            {
                current_Cooldown_E = 0;
                UISkill.skillDisabledImage[3].SetActive(false);
                UISkill.skillCooldownImage[3].fillAmount = 0;
                UISkill.skillCooldownText[3].text = "";
            }
            else
            {
                // 이미지와 텍스트 갱신
                UISkill.skillCooldownImage[3].fillAmount = current_Cooldown_E / temp_Cooldown_E;
                if (current_Cooldown_E > 1.0f)
                    UISkill.skillCooldownText[3].text = Mathf.FloorToInt(current_Cooldown_E).ToString();
                else
                    UISkill.skillCooldownText[3].text = current_Cooldown_E.ToString("N1");
            }
        }
        if (current_Cooldown_R != 0)
        {
            current_Cooldown_R -= Time.deltaTime;
            if (current_Cooldown_R < 0)
            {
                current_Cooldown_R = 0;
                UISkill.skillDisabledImage[4].SetActive(false);
                UISkill.skillCooldownImage[4].fillAmount = 0;
                UISkill.skillCooldownText[4].text = "";
            }
            else
            {
                // 이미지와 텍스트 갱신
                UISkill.skillCooldownImage[4].fillAmount = current_Cooldown_R / temp_Cooldown_R;
                if (current_Cooldown_R > 1.0f)
                    UISkill.skillCooldownText[4].text = Mathf.FloorToInt(current_Cooldown_R).ToString();
                else
                    UISkill.skillCooldownText[4].text = current_Cooldown_R.ToString("N1");
            }
        }

    }
    public void UsedQ()
    {
        if (isRecallStart)
            RecallCancel();

        temp_Cooldown_Q = Cooldown_Q;
        current_Cooldown_Q = Cooldown_Q;
        if (isNoMP)
            totalStat.Hp -= mana_Q;
        else
            totalStat.Mp -= mana_Q;
        if (Cooldown_Q != 0)
            UISkill.skillDisabledImage[1].SetActive(true);
        ChampionSound.instance.Skill(playerData.championName, 0, audioSource);
    }

    public void UsedW()
    {
        if (isRecallStart)
            RecallCancel();

        temp_Cooldown_W = Cooldown_W;
        current_Cooldown_W = Cooldown_W;
        if (isNoMP)
            totalStat.Hp -= mana_W;
        else
            totalStat.Mp -= mana_W;

        if (Cooldown_W != 0)
            UISkill.skillDisabledImage[2].SetActive(true);
        ChampionSound.instance.Skill(playerData.championName, 1, audioSource);
    }

    public void UsedE()
    {
        if (isRecallStart)
            RecallCancel();

        temp_Cooldown_E = Cooldown_E;
        current_Cooldown_E = Cooldown_E;
        if (isNoMP)
            totalStat.Hp -= mana_E;
        else
            totalStat.Mp -= mana_E;

        if (Cooldown_W != 0)
            UISkill.skillDisabledImage[3].SetActive(true);
        ChampionSound.instance.Skill(playerData.championName, 2, audioSource);

    }

    public void UsedR()
    {
        if (isRecallStart)
            RecallCancel();

        temp_Cooldown_R = Cooldown_R;
        current_Cooldown_R = Cooldown_R;
        if (isNoMP)
            totalStat.Hp -= mana_R;
        else
            totalStat.Mp -= mana_R;

        if (Cooldown_R != 0)
            UISkill.skillDisabledImage[4].SetActive(true);

    }


    public void SpellCheck()
    {
        if (chatFunction.chatInput.IsActive())
            return;

        if (Input.GetKeyDown(KeyCode.D) && !isRecallStart && !playerSkill.TheChampionAtk.isStun && !playerData.isDead)
        {
            if (current_Cooldown_D == 0)
            {
                if (isRecallStart)
                    RecallCancel();

                playerSpell.Call_SpellD();

                if (spell_D != 5 && spell_D != 6 && spell_D != 7)
                {
                    current_Cooldown_D = Cooldown_D;
                    if (Cooldown_D != 0)
                        UISkill.spellDisabledImage[0].SetActive(true);
                }
                else
                {
                    playerSpell.cursor.SetCursor(3, Vector2.zero);
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.F) && !isRecallStart && !playerSkill.TheChampionAtk.isStun && !playerData.isDead)
        {
            if (current_Cooldown_F == 0)
            {
                if (isRecallStart)
                    RecallCancel();

                playerSpell.Call_SpellF();

                if (spell_F != 5 && spell_F != 6 && spell_F != 7)
                {
                    current_Cooldown_F = Cooldown_F;
                    if (Cooldown_F != 0)
                        UISkill.spellDisabledImage[1].SetActive(true);
                }
                else
                {
                    playerSpell.cursor.SetCursor(3, Vector2.zero);
                }
            }
        }
        if (spell_D == 5 || spell_D == 6 || spell_D == 7)
        {
            if ((spell_D == 5 && playerSpell.SmiteTargetset))
            {
                playerSpell.SmiteTargetset = false;
                current_Cooldown_D = Cooldown_D;
                if (Cooldown_D != 0)
                    UISkill.spellDisabledImage[0].SetActive(true);
            }
            else if ((spell_D == 6 && playerSpell.TeleportingOnce))
            {
                current_Cooldown_D = Cooldown_D;
                if (Cooldown_D != 0)
                    UISkill.spellDisabledImage[0].SetActive(true);
            }
            else if ((spell_D == 7 && playerSpell.IgniteTargetset))
            {
                playerSpell.IgniteTargetset = false;
                current_Cooldown_D = Cooldown_D;
                if (Cooldown_D != 0)
                    UISkill.spellDisabledImage[0].SetActive(true);
            }
        }
        if (spell_F == (5) || spell_F == (6) || spell_F == (7))
        {
            if ((spell_F == 5 && playerSpell.SmiteTargetset))
            {
                playerSpell.SmiteTargetset = false;
                current_Cooldown_F = Cooldown_F;
                if (Cooldown_F != 0)
                    UISkill.spellDisabledImage[1].SetActive(true);
            }
            else if ((spell_F == 6 && playerSpell.TeleportingOnce))
            {
                current_Cooldown_F = Cooldown_F;
                if (Cooldown_F != 0)
                    UISkill.spellDisabledImage[1].SetActive(true);
            }
            else if ((spell_F == 7 && playerSpell.IgniteTargetset))
            {
                playerSpell.IgniteTargetset = false;
                current_Cooldown_F = Cooldown_F;
                if (Cooldown_F != 0)
                    UISkill.spellDisabledImage[1].SetActive(true);
            }
        }
        if (current_Cooldown_D != 0)
        {
            current_Cooldown_D -= Time.deltaTime;
            if (current_Cooldown_D < 0)
            {
                current_Cooldown_D = 0;
                UISkill.spellDisabledImage[0].SetActive(false);
                UISkill.spellCooldownImage[0].fillAmount = 0;
                UISkill.spellCooldownText[0].text = "";
            }
            else
            {
                // 이미지와 텍스트 갱신
                UISkill.spellCooldownImage[0].fillAmount = current_Cooldown_D / Cooldown_D;
                if (current_Cooldown_D > 1.0f)
                    UISkill.spellCooldownText[0].text = Mathf.FloorToInt(current_Cooldown_D).ToString();
                else
                    UISkill.spellCooldownText[0].text = current_Cooldown_D.ToString("N1");
            }
        }
        if (current_Cooldown_F != 0)
        {
            current_Cooldown_F -= Time.deltaTime;
            if (current_Cooldown_F < 0)
            {
                current_Cooldown_F = 0;
                UISkill.spellDisabledImage[1].SetActive(false);
                UISkill.spellCooldownImage[1].fillAmount = 0;
                UISkill.spellCooldownText[1].text = "";
            }
            else
            {
                // 이미지와 텍스트 갱신
                UISkill.spellCooldownImage[1].fillAmount = current_Cooldown_F / Cooldown_F;
                if (current_Cooldown_F > 1.0f)
                    UISkill.spellCooldownText[1].text = Mathf.FloorToInt(current_Cooldown_F).ToString();
                else
                    UISkill.spellCooldownText[1].text = current_Cooldown_F.ToString("N1");
            }
        }
    }

    public void RecallCheck()
    {
        if (!isRecallStart)
            if (chatFunction.chatInput.IsActive())
                return;

        if (Input.GetKeyDown(KeyCode.B) && !playerData.isDead && !playerSkill.TheChampionAtk.isStun)
        {
            ChampionSound.instance.PlayPlayerFx(SoundManager.Instance.Recall);
            //이펙트 부르기
            Recall();
        }
        if (isRecallStart)
        {
            playerSpell.SendEffect("Recall", transform.position, PhotonNetwork.player.GetTeam().ToString());
            currentRecallTime -= Time.deltaTime;
            UIRecall.GetComponent<RecallUI>().recallProgressBar.value = currentRecallTime / recallTime;
            UIRecall.GetComponent<RecallUI>().remainTime.text = currentRecallTime.ToString("N1");

            //리콜시 제자리에서 리콜하도록
            transform.position = currentPos;
            if (currentRecallTime <= 0)
            {
                isRecallStart = false;
                UIRecall.SetActive(false);
                RecallComplete();
            }
            RecallCancelCheck();
        }
    }

    public void SetStatSkill(string championName)
    {
        myStat = StatClass.Instance.characterData[championName].ClassCopy();
        mySkill = SkillClass.Instance.skillData[championName].ClassCopy();
        Cooldown_Passive = mySkill.passiveCooldown;
    }

    public void SetSpell()
    {
        spell_D = PlayerData.Instance.spellD;
        spell_F = PlayerData.Instance.spellF;
        //스펠 쿨세팅
        switch (spell_D)
        {
            //정화 탈진 점멸 유체화 회복 강타 순간이동 점화 방어막
            // 정화
            case 0:
                Cooldown_D = 210;
                break;
            // 탈진
            case 1:
                Cooldown_D = 210;
                break;
            // 점멸
            case 2:
                Cooldown_D = 300;
                break;
            // 유체화
            case 3:
                Cooldown_D = 180;
                break;
            // 회복
            case 4:
                Cooldown_D = 240;
                break;
            // 강타
            case 5:
                Cooldown_D = 15;
                break;
            // 순간이동
            case 6:
                Cooldown_D = 360;
                break;
            // 점화
            case 7:
                Cooldown_D = 210;
                break;
            // 방어막
            case 8:
                Cooldown_D = 180;
                break;
            default:
                break;
        }
        switch (spell_F)
        {
            //정화 탈진 점멸 유체화 회복 강타 순간이동 점화 방어막
            // 정화
            case 0:
                Cooldown_F = 210;
                break;
            // 탈진
            case 1:
                Cooldown_F = 210;
                break;
            // 점멸
            case 2:
                Cooldown_F = 300;
                break;
            // 유체화
            case 3:
                Cooldown_F = 180;
                break;
            // 회복
            case 4:
                Cooldown_F = 240;
                break;
            // 강타
            case 5:
                Cooldown_F = 15;
                break;
            // 순간이동
            case 6:
                Cooldown_F = 360;
                break;
            // 점화
            case 7:
                Cooldown_F = 210;
                break;
            // 방어막
            case 8:
                Cooldown_F = 180;
                break;
            default:
                break;
        }
    }

    public void LevelUp()
    {
        if (myStat.Level >= 18)
            return;

        myStat.Level++;
        myStat.Exp -= myStat.RequireExp;
        myStat.Hp += myStat.UP_Hp;
        myStat.MaxHp += myStat.UP_Hp;
        myStat.Mp += myStat.UP_Mp;
        myStat.MaxMp += myStat.UP_Mp;
        myStat.HealthRegen += myStat.UP_HpRegen;
        myStat.ManaRegen += myStat.UP_MpRegen;
        myStat.AttackDamage += myStat.UP_AttackDamage;
        myStat.AttackDef += myStat.UP_Def;
        myStat.AbilityDef += myStat.UP_MagicDef;

        if (myStat.Level <= 17)
            myStat.RequireExp = StatClass.Instance.requireExp[myStat.Level - 1];
        else if (myStat.Level == 18)
        {
            myStat.RequireExp = 0;
            myStat.Exp = 0;
        }
        // 공속은 계산법이 복잡해서 일단 UI상에서만 계산하여 표시

        totalStat.Hp += myStat.UP_Hp;
        totalStat.Mp += myStat.UP_Mp;
        TotalStatUpdate();

        if (photonView.isMine)
        {
            ChampionSound.instance.PlayPlayerFx(SoundManager.Instance.LevelUp);
            UIStat.Refresh();
            UIIcon.LevelUp();
            UISkill.LevelUp();
        }
    }

    public void Recall()
    {
        if (isRecallStart || playerData.isDead || playerSkill.TheChampionAtk.isStun)
            return;

        currentPos = transform.position;
        currentHp = totalStat.Hp;

        isRecallStart = true;
        UIRecall.SetActive(true);

        currentRecallTime = recallTime;
    }

    public void RecallComplete()
    {
        ChampionSound.instance.PlayPlayerFx(SoundManager.Instance.Recall_Complete);

        theAIPath.canMove = true;
        if (PhotonNetwork.player.GetTeam().ToString().Contains("red"))
        {
            transform.position = redPos;
            playerSkill.TheChampionAtk.AStarTargetObj.transform.position = redPos;
        }
        else if (PhotonNetwork.player.GetTeam().ToString().Contains("blue"))
        {
            transform.position = bluePos;
            playerSkill.TheChampionAtk.AStarTargetObj.transform.position = bluePos;
        }
        myStat.Hp = myStat.MaxHp;
        myStat.Mp = myStat.MaxMp;

        recallComplete = true;

        Camera.main.transform.position = new Vector3(transform.position.x, Camera.main.transform.position.y, transform.position.z)
            + Camera.main.GetComponent<RTS_Cam.RTS_Camera>().targetOffset;
    }

    public void RecallCancelCheck()
    {
        // 마우스 클릭했을때, 공격받았을때,
        if (Input.GetMouseButtonDown(1) || currentHp > totalStat.Hp)
        {
            ChampionSound.instance.PlayerFx.Stop();
            RecallCancel();
        }
    }

    public void RecallCancel()
    {
        //리콜 이펙트 중단시킬것.
        isRecallStart = false;
        if(UIRecall)
        UIRecall.SetActive(false);
    }

    public void ItemUpdate(int[] item, int accessoryitem)
    {
        this.item = item;
        this.accessoryItem = accessoryitem;

        if (UIStat == null)
            FindUICanvas();

        ItemStatUpdate();

        if (UIStat != null)
            UIStat.Refresh();
    }

    public void ItemStatUpdate()
    {
        // 장착한 item의 stat합계를 reset
        ItemStatReset(itemStat);

        // itemlist 돌면서 0번이 아니면 itemstat에 stat을 더해줌.
        foreach (int i in item)
        {
            if (i == 0)
                continue;

            ShopItem.Item tempitem = ShopItem.Instance.itemlist[i];

            itemStat.attackDamage += tempitem.attackDamage;
            itemStat.attackSpeed += tempitem.attackSpeed;
            itemStat.criticalPercent += tempitem.criticalPercent;
            itemStat.lifeSteal += tempitem.lifeSteal;

            itemStat.abilityPower += tempitem.abilityPower;
            if (isNoMP)
                itemStat.mana = 0;
            else
                itemStat.mana += tempitem.mana;
            itemStat.manaRegen += tempitem.manaRegen;
            itemStat.cooldownReduce += tempitem.cooldownReduce;

            itemStat.armor += tempitem.armor;
            itemStat.magicResist += tempitem.magicResist;
            itemStat.health += tempitem.health;
            itemStat.healthRegen += tempitem.healthRegen;

            itemStat.movementSpeed += tempitem.movementSpeed / 50f;
        }

        TotalStatUpdate();
    }

    public void ItemStatReset(ShopItem.Item itemstat)
    {
        itemstat.attackDamage = 0;
        itemstat.attackSpeed = 0;
        itemstat.criticalPercent = 0;
        itemstat.lifeSteal = 0;

        itemstat.abilityPower = 0;
        itemstat.mana = 0;
        itemstat.manaRegen = 0;
        itemstat.cooldownReduce = 0;

        itemstat.armor = 0;
        itemstat.magicResist = 0;
        itemstat.health = 0;
        itemstat.healthRegen = 0;

        itemstat.movementSpeed = 0;
    }

    // 경험치는 여기서 관리안함.
    public void TotalStatUpdate()
    {
        totalStat.Level = myStat.Level;

        TotalStatDamDefUpdate();
        TotalStatSpeedUpdate();

        //totalstat.Attack_Damage = mystat.Attack_Damage + itemstat.attack_damage;
        totalStat.AttackSpeed = myStat.AttackSpeed + itemStat.attackSpeed;
        totalStat.CriticalPercentage = myStat.CriticalPercentage + itemStat.criticalPercent;

        // 흡혈 스탯이 없었던가?
        //totalstat.life = mystat.life + itemstat.attack_damage;

        totalStat.AbilityPower = myStat.AbilityPower + itemStat.abilityPower;
        totalStat.MaxMp = myStat.MaxMp + itemStat.mana;
        totalStat.ManaRegen = myStat.ManaRegen * (1 + itemStat.manaRegen / 100f);
        totalStat.CoolTimeDecrease = myStat.CoolTimeDecrease + itemStat.cooldownReduce;

        //totalstat.Attack_Def = mystat.Attack_Def + itemstat.armor;
        //totalstat.Ability_Def = mystat.Ability_Def + itemstat.magic_resist;
        totalStat.MaxHp = myStat.MaxHp + itemStat.health;
        totalStat.HealthRegen = myStat.HealthRegen * (1 + itemStat.healthRegen / 100f);

        //totalstat.Move_Speed = mystat.Move_Speed + itemstat.movement_speed;
        // 체젠 마젠은 %니까 100%증가면 2배의속도가 되게 (1 + x/100)을 곱해줌
    }

    public void TotalStatDamDefUpdate()
    {
        totalStat.AttackDamage = myStat.AttackDamage + itemStat.attackDamage + skillPlusAtkDam;
        totalStat.AttackDef = myStat.AttackDef + itemStat.armor + skillPlusAtkDef;
        totalStat.AbilityDef = myStat.AbilityDef + itemStat.magicResist + skillPlusAbilDef;
    }

    public void TotalStatSpeedUpdate()
    {
        totalStat.MoveSpeed = myStat.MoveSpeed + itemStat.movementSpeed + skillPlusSpeed;
        if (theAIPath != null)
            theAIPath.maxSpeed = totalStat.MoveSpeed;
    }

    /// <summary>
    /// 킬냈을때 cs 골드 경험치 올려주는 함수임
    /// </summary>
    /// <param name="name">오브젝트 이름 넣어라</param>
    /// <param name="type">0 챔피언 / 1 미니언 / 2 타워 / 3 정글몹</param>
    /// <param name="pos"> cs 텍스트 띄울 위치</param>
    public void Kill_CS_Gold_Exp(string name, int type, Vector3 pos = default(Vector3))
    {
        // 일단 골드증가
        int csGold = PlayerData.Instance.KillGold(name, type);

        // 챔피언이면
        if (type == 0)
        {
            // 원래는 킬낸 챔피언의 레벨 ~ 다음 레벨의 경험치의 50%를 준다고함. 그냥 200줌
            myStat.Exp += 200;
            kill++;
            csTextPool.getCsText(pos, "+ " + csGold.ToString());
            return;
        }
        // 타워면
        else if (type == 2)
        {
            // 억제기부수면 글로벌 경험치
            if (name.Contains("Suppressor"))
            {
                this.photonView.RPC("GlobalExp", PhotonTargets.All, PhotonNetwork.player.GetTeam().ToString(), 100);
            }
            csTextPool.getCsText(pos, "+ " + csGold.ToString());
            return;
        }
        // 미니언이면
        else if (type == 1)
        {
            cs++;
            // 미니언은 골드와 cs만 먹고
            // 경험치는 미니언이 죽을때 근처에 있는 애들한테 줌
            csTextPool.getCsText(pos, "+ " + csGold.ToString());
        }
        // 정글이면
        else if (type == 3)
        {
            if (name.Contains("Raptor_Big")) // 칼날부리
            {
                myStat.Exp += 25;
                cs += 2;
                csTextPool.getCsText(pos, "+ " + csGold.ToString());
            }
            else if (name.Contains("Raptor_Small")) // 칼날부리 작은애
            {
                myStat.Exp += 23;
                csTextPool.getCsText(pos, "+ " + csGold.ToString());
            }
            else if (name.Contains("Wolf")) // 늑대
            {
                myStat.Exp += 86;
                cs += 2;
                csTextPool.getCsText(pos, "+ " + csGold.ToString());
            }
            else if (name.Contains("Wolf_Small")) // 늑대 작은애
            {
                myStat.Exp += 33;
                cs += 1;
                csTextPool.getCsText(pos, "+ " + csGold.ToString());
            }
            else if (name.Contains("Krug_Big")) // 작골
            {
                myStat.Exp += 133;
                cs += 1;
                csTextPool.getCsText(pos, "+ " + csGold.ToString());
            }
            else if (name.Contains("Krug_Small")) // 작골 작은애
            {
                myStat.Exp += 47;
                csTextPool.getCsText(pos, "+ " + csGold.ToString());
            }
            else if (name.Contains("Gromp")) // 두꺼비
            {
                myStat.Exp += 153;
                cs += 4;
                csTextPool.getCsText(pos, "+ " + csGold.ToString());
            }
            // 블루 레드
            else if (name.Contains("B_Sentinel") || name.Contains("R_Sentinel") || name.Contains("Dragon") || name.Contains("Rift_Herald"))
            {
                myStat.Exp += 148;
                cs += 4;
                csTextPool.getCsText(pos, "+ " + csGold.ToString());
            }
            // 용
            else if (name.Contains("Dragon"))
            {
                myStat.Exp += 200;
                cs += 4;
                csTextPool.getCsText(pos, "+ " + csGold.ToString());
            }
            // 전령
            else if (name.Contains("Rift_Herald"))
            {
                this.photonView.RPC("GlobalExp", PhotonTargets.All, PhotonNetwork.player.GetTeam().ToString(), 300);
                cs += 4;
                csTextPool.getCsText(pos, "+ " + csGold.ToString());
            }
            else if (name.Contains("Baron")) // 내셔남작
            {
                this.photonView.RPC("GlobalExp", PhotonTargets.All, PhotonNetwork.player.GetTeam().ToString(), 700);
                cs += 4;
                csTextPool.getCsText(pos, "+ " + csGold.ToString());
            }
        }

        // 오른쪽 위 UI cs 갱신
        UIRightTop.CSUpdate();
    }

    [PunRPC]
    public void MinionExp(int exp)
    {
        myStat.Exp += exp;
    }

    [PunRPC]
    public void GlobalExp(string team, int exp)
    {
        // 같은팀일 경우 같이 경험치 냠냠
        if (PhotonNetwork.player.GetTeam().ToString().Equals(team))
        {
            myStat.Exp += exp;
        }
    }

    [PunRPC]
    public void GlobalGold(string team, int gold)
    {
        // 같은팀일 경우 같이 경험치 냠냠
        if (PhotonNetwork.player.GetTeam().ToString().Equals(team))
        {
            PlayerData.Instance.gold += gold;
        }
    }

    [PunRPC]
    public void AssistUP()
    {
        assist++;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            // We own mystat player: send the others our data
            // 기본정보

            stream.SendNext(championName);
            stream.SendNext(spell_D);
            stream.SendNext(spell_F);
            stream.SendNext(kill);
            stream.SendNext(death);
            stream.SendNext(assist);
            stream.SendNext(cs);
            stream.SendNext(skill_Q);
            stream.SendNext(skill_W);
            stream.SendNext(skill_E);
            stream.SendNext(skill_R);

            // 스탯
            TotalStatSend(stream);
            stream.SendNext(myStat.AttackSpeed);

            // 아이템
            for (int i = 0; i < item.Length; i++)
            {
                stream.SendNext(item[i]);
            }
            stream.SendNext(accessoryItem);
        }
        else
        {
            // Network player, receive data
            // 기본정보
            championName = (string)stream.ReceiveNext();
            spell_D = (int)stream.ReceiveNext();
            spell_F = (int)stream.ReceiveNext();
            kill = (int)stream.ReceiveNext();
            death = (int)stream.ReceiveNext();
            assist = (int)stream.ReceiveNext();
            cs = (int)stream.ReceiveNext();
            skill_Q = (int)stream.ReceiveNext();
            skill_W = (int)stream.ReceiveNext();
            skill_E = (int)stream.ReceiveNext();
            skill_R = (int)stream.ReceiveNext();

            // 스탯
            TotalStatReceive(stream);
            myStat.AttackSpeed = (float)stream.ReceiveNext();

            // 아이템
            for (int i = 0; i < item.Length; i++)
            {
                item[i] = (int)stream.ReceiveNext();
            }
            accessoryItem = (int)stream.ReceiveNext();
        }
    }

    public void TotalStatSend(PhotonStream stream)
    {
        stream.SendNext(totalStat.AbilityDef);
        stream.SendNext(totalStat.AbilityPower);
        stream.SendNext(totalStat.AttackDamage);
        stream.SendNext(totalStat.AttackDef);
        stream.SendNext(totalStat.AttackRange);
        stream.SendNext(totalStat.AttackSpeed);
        stream.SendNext(totalStat.CoolTimeDecrease);
        stream.SendNext(totalStat.CriticalPercentage);
        stream.SendNext(totalStat.Exp);
        stream.SendNext(totalStat.Gold);
        stream.SendNext(totalStat.HealthRegen);
        stream.SendNext(totalStat.Hp);
        stream.SendNext(totalStat.Level);
        stream.SendNext(totalStat.ManaRegen);
        stream.SendNext(totalStat.MaxHp);
        stream.SendNext(totalStat.MaxMp);
        stream.SendNext(totalStat.MoveSpeed);
        stream.SendNext(totalStat.Mp);
        stream.SendNext(totalStat.RequireExp);
        stream.SendNext(totalStat.RespawnTime);
        stream.SendNext(totalStat.UP_AttackSpeed);
    }

    public void TotalStatReceive(PhotonStream stream)
    {
        totalStat.AbilityDef = (float)stream.ReceiveNext();
        totalStat.AbilityPower = (float)stream.ReceiveNext();
        totalStat.AttackDamage = (float)stream.ReceiveNext();
        totalStat.AttackDef = (float)stream.ReceiveNext();
        totalStat.AttackRange = (float)stream.ReceiveNext();
        totalStat.AttackSpeed = (float)stream.ReceiveNext();
        totalStat.CoolTimeDecrease = (float)stream.ReceiveNext();
        totalStat.CriticalPercentage = (float)stream.ReceiveNext();
        totalStat.Exp = (int)stream.ReceiveNext();
        totalStat.Gold = (int)stream.ReceiveNext();
        totalStat.HealthRegen = (float)stream.ReceiveNext();
        totalStat.Hp = (float)stream.ReceiveNext();
        totalStat.Level = (int)stream.ReceiveNext();
        totalStat.ManaRegen = (float)stream.ReceiveNext();
        totalStat.MaxHp = (float)stream.ReceiveNext();
        totalStat.MaxMp = (float)stream.ReceiveNext();
        totalStat.MoveSpeed = (float)stream.ReceiveNext();
        totalStat.Mp = (float)stream.ReceiveNext();
        totalStat.RequireExp = (int)stream.ReceiveNext();
        totalStat.RespawnTime = (float)stream.ReceiveNext();
        totalStat.UP_AttackSpeed = (float)stream.ReceiveNext();
    }
}