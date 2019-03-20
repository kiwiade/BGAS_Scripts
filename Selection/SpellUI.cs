﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SpellUI : Photon.MonoBehaviour
{
    //현재 열려있는 스펠이 어떤건지 체크
    private bool isSpell1Actived = false;
    private bool isSpell2Actived = false;

    [SerializeField]
    private Text explanation = null;
    [SerializeField]
    private GameObject[] icons = null;
    [SerializeField]
    private GameObject selectBorder;

    [Space]
    [SerializeField]
    private GameObject spellButton1;
    [SerializeField]
    private GameObject spellButton2;

    // 다른 플레이어들한테 보내줄때 몇번 스펠을 바꿀지 보내주는용도.
    private int spellNum;
    private string currentSpell;

    private SelectionLayoutGroup selectionLayoutGroup;

    // 가진 스펠저장
    private string spell_D = "";
    private string spell_F = "";

    private bool isPick = false;

    private void Awake()
    {
        PhotonNetwork.OnEventCall += PhotonNetwork_OnEventCall;

        if (PlayerPrefs.HasKey("D_Spell"))
            PlayerPrefs.DeleteKey("D_Spell");
        if (PlayerPrefs.HasKey("F_Spell"))
            PlayerPrefs.DeleteKey("F_Spell");
    }

    void OnDestroy()
    {
        PhotonNetwork.OnEventCall -= PhotonNetwork_OnEventCall;
    }

    void Start()
    {
        gameObject.SetActive(false);
        basicSpellSetting();
    }

    public void SelectChampion()
    {
        isPick = true;
    }

    private void basicSpellSetting()
    {
        // 저장된 정보가 있으면 받아서 입력
        if (PlayerPrefs.HasKey("D_Spell"))
            spell_D = PlayerPrefs.GetString("D_Spell");
        if (PlayerPrefs.HasKey("F_Spell"))
            spell_F = PlayerPrefs.GetString("F_Spell");

        // 확인해서 없으면 d점화 f플래시. 있었으면 그거 받아서 업데이트
        if (spell_D.Equals(string.Empty) || spell_F.Equals(string.Empty))
        {
            currentSpell = "ignite";
            isSpell1Actived = true;
            spellSelect();

            currentSpell = "flash";
            isSpell2Actived = true;
            spellSelect();
        }
        else
        {
            currentSpell = spell_D;
            isSpell1Actived = true;
            spellSelect();

            currentSpell = spell_F;
            isSpell2Actived = true;
            spellSelect();
        }
    }

    private void PhotonNetwork_OnEventCall(byte eventCode, object content, int senderId)
    {
        if (this == null)
            return;

        if (eventCode.Equals(PhotonEventCode.SPELL_SHARE))
        {
            object[] datas = content as object[];
            if (datas.Length.Equals(4))
            {
                // 같은팀만 받음 (같은팀만 스펠이 보여야하니)
                if (PhotonNetwork.player.GetTeam().ToString().Equals((string)datas[1]))
                {
                    selectionLayoutGroup = GameObject.FindGameObjectWithTag("RedTeamLayout").GetComponent<SelectionLayoutGroup>();
                    foreach (SelectListing Prefab in selectionLayoutGroup.selectListings)
                    {
                        // 해당 유저찾아서 몇번에 무슨스펠 적용하라고 함수보냄
                        if (Prefab.PhotonPlayer.NickName == (string)datas[0])
                        {
                            Prefab.GetComponent<SpellSelect>().ChangeSpellImage((string)datas[2], (string)datas[3]);
                            break;
                        }
                    }
                }
            }
        }
    }

    public int SpellNumConvert(string name)
    {
        // 정화 탈진 점멸 유체화 회복 강타 순간이동 점화 방어막 (0~8)
        switch (name)
        {
            case "cleanse":
                return 0;
            case "exhaust":
                return 1;
            case "flash":
                return 2;
            case "ghost":
                return 3;
            case "heal":
                return 4;
            case "smite":
                return 5;
            case "teleport":
                return 6;
            case "ignite":
                return 7;
            case "barrier":
                return 8;
            default:
                return 10;
        }
    }

    //스펠 적용
    public void spellSelect()
    {
        if (!isPick)
        {
            if (SoundManager.Instance != null)
                SoundManager.Instance.Button_UI_Sound();

            if (isSpell1Actived)
            {
                spellNum = 1;
                isSpell1Actived = false;

                // 고른게 f에 있으면 위치변환
                if (spell_F == currentSpell)
                {
                    spell_F = spell_D;
                    spellButton2.GetComponent<Image>().sprite = Resources.Load<Sprite>("Spell/" + spell_F);
                    PlayerData.Instance.spellF = PlayerData.Instance.spellD;
                }
                spell_D = currentSpell;
                PlayerPrefs.SetString("D_Spell", spell_D);
                PlayerData.Instance.spellD = SpellNumConvert(spell_D);
                SelectBorderChange();

                spellButton1.GetComponent<Image>().sprite = Resources.Load<Sprite>("Spell/" + spell_D);
            }
            else if (isSpell2Actived)
            {
                spellNum = 2;
                isSpell2Actived = false;

                // 고른게 d에 있으면 위치변환
                if (spell_D == currentSpell)
                {
                    spell_D = spell_F;
                    spellButton1.GetComponent<Image>().sprite = Resources.Load<Sprite>("Spell/" + spell_D);
                    PlayerData.Instance.spellD = PlayerData.Instance.spellF;
                }
                spell_F = currentSpell;
                PlayerPrefs.SetString("F_Spell", spell_F);
                PlayerData.Instance.spellF = SpellNumConvert(spell_F);
                SelectBorderChange();

                spellButton2.GetComponent<Image>().sprite = Resources.Load<Sprite>("Spell/" + spell_F);
            }

            // 다른사람들에게 RaiseEvent 쏴줌
            object[] datas = new object[] { (string)PhotonNetwork.player.NickName, (string)PhotonNetwork.player.GetTeam().ToString(), (string)spell_D, (string)spell_F };
            PhotonNetwork.RaiseEvent((byte)PhotonEventCode.SPELL_SHARE, datas, true, new RaiseEventOptions()
            {
                CachingOption = EventCaching.DoNotCache,
                Receivers = ReceiverGroup.All
            });
            gameObject.SetActive(false);
        }
    }

    // 스펠에 마우스오버시 설명 변경
    public void OnPointerEnter(string name)
    {
        currentSpell = name;
        explanation_Change(name);
    }

    public void OnPointerExit()
    {
        if (isSpell1Actived)
        {
            explanation_Change(spell_D);
        }
        else if (isSpell2Actived)
        {
            explanation_Change(spell_F);
        }
    }

    public void explanation_Change(string name)
    {
        switch (name)
        {
            case "cleanse":
                explanation.GetComponent<Text>().text = "<size=32>정화</size>\n"
                    + "챔피언에 걸린 모든 이동 불가와(제압 및 공중으로 띄우는 효과 제외) 소환사 주문에 의한 해로운 효과를 제거하고 새로 적용되는 이동 불과 효과들의 지속 시간을 3초간 65 % 감소시킵니다.";
                break;
            case "exhaust":
                explanation.GetComponent<Text>().text = "<size=32>탈진</size>\n"
                    + "적 챔피언을 지치게 만들어 2.5초 동안 이동 속도를 30% 낮추며, 이 동안 가하는 피해량을 40% 낮춥니다.";
                break;
            case "flash":
                explanation.GetComponent<Text>().text = "<size=32>점멸</size>\n"
                    + "커서 방향으로 챔피언이 짧은 거리를 순간이동합니다.";
                break;
            case "ghost":
                explanation.GetComponent<Text>().text = "<size=32>유체화</size>\n"
                    + "챔피언이 10초 동안 이동 속도가 상승합니다.이동 속도는 2초 동안 점차 빨라져 레벨에 따라 최대 28 ~ 45% 까지 상승합니다.";
                break;
            case "heal":
                explanation.GetComponent<Text>().text = "<size=32>회복</size>\n"
                    + "챔피언과 대상 아군의 체력을 90-345만큼 (챔피언 레벨에 따라 변동) 회복시키고 2초 동안 이동 속도가 30% 증가합니다.";
                break;
            case "smite":
                explanation.GetComponent<Text>().text = "<size=32>강타</size>\n"
                    + "대상 에픽 및 대형/중형 몬스터, 혹은 적 미니언에게 390-1000(챔피언 레벨에 따라 변동)의 고정 피해를 입힙니다. 강타를 사용하면 자신의 최대체력의 10%만큼 회복합니다.";
                break;
            case "teleport":
                explanation.GetComponent<Text>().text = "<size=32>순간이동</size>\n"
                    + "4.5초 동안 정신 집중을 한 후 근처의 아군 미니언이나 포탑, 혹은 와드로 순간이동합니다";
                break;
            case "ignite":
                explanation.GetComponent<Text>().text = "<size=32>점화</size>\n"
                    + "적 챔피언을 불태워 5초 동안 80~505의 고정 피해(챔피언 레벨에 따라 변동)를 입힙니다.";
                break;
            case "barrier":
                explanation.GetComponent<Text>().text = "<size=32>보호막</size>\n"
                    + "2초 동안 방어막으로 감싸 피해를 115~455(챔피언 레벨에 따라 변동)만큼 흡수합니다.";
                break;
            default:
                break;
        }
    }

    void Update()
    {
        SpellWindowCloseCheck();
    }

    // 마우스 클릭시 스펠선택창 밖이면 창 꺼버림
    private void SpellWindowCloseCheck()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // 그래픽레이캐스트 쏴서 스펠배경이 안맞으면 끔
            GraphicRaycaster gr = transform.parent.GetComponent<GraphicRaycaster>();
            PointerEventData ped = new PointerEventData(null);
            ped.position = Input.mousePosition;
            List<RaycastResult> results = new List<RaycastResult>();
            gr.Raycast(ped, results);

            bool find = false;
            foreach (RaycastResult rr in results)
            {
                if (rr.gameObject.name == "SpellBackground" || rr.gameObject.name == "SpellButton1" || rr.gameObject.name == "SpellButton2")
                {
                    find = true;
                    return;
                }
            }

            if (!find)
            {
                gameObject.SetActive(false);
                isSpell1Actived = false;
                isSpell2Actived = false;
            }
        }
    }

    public void SelectBorderChange()
    {
        foreach (GameObject icon in icons)
        {
            if (isSpell1Actived)
            {
                if (icon.name == spell_D)
                {
                    selectBorder.transform.position = icon.transform.position;
                    break;
                }
            }
            else if (isSpell2Actived)
            {
                if (icon.name == spell_F)
                {
                    selectBorder.transform.position = icon.transform.position;
                    break;
                }
            }
        }
    }

    //버튼이 눌러지면 SpellUI창을 띄움
    public void Spell1Click()
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.Button_UI_Sound();

        if (!gameObject.activeSelf)
        {
            isSpell1Actived = true;
            isSpell2Actived = false;

            gameObject.SetActive(true);
            explanation_Change(spell_D);
            SelectBorderChange();
        }
        else
        {
            if (isSpell2Actived)
            {
                isSpell1Actived = true;
                isSpell2Actived = false;

                explanation_Change(spell_D);
                SelectBorderChange();
            }
            else
            {
                isSpell1Actived = false;
                gameObject.SetActive(false);
            }
        }
    }

    public void Spell2Click()
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.Button_UI_Sound();

        if (!gameObject.activeSelf)
        {
            isSpell1Actived = false;
            isSpell2Actived = true;

            gameObject.SetActive(true);
            explanation_Change(spell_F);
            SelectBorderChange();
        }
        else
        {
            if (isSpell1Actived)
            {
                isSpell1Actived = false;
                isSpell2Actived = true;

                explanation_Change(spell_F);
                SelectBorderChange();
            }
            else
            {
                isSpell2Actived = false;
                gameObject.SetActive(false);
            }
        }
    }
}