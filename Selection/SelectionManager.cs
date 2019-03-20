using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public class SelectionManager : Photon.PunBehaviour
{
    [SerializeField]
    private Text titleText;
    public Text timerText;
    [SerializeField]
    private GameObject readyButton;
    [SerializeField]
    private Canvas loadingScreen;

    [HideInInspector]
    public float timer = 90.0f;
    private bool isSelectFinished = false;
    private bool isLoadStarted = false;

    private SelectionLayoutGroup selectionLayoutGroup;
    private AsyncOperation async;
    private string selectedChampionName;

    void Awake()
    {
        loadingScreen.gameObject.SetActive(false);
    }

    void Start()
    {
        selectionLayoutGroup = GameObject.FindGameObjectWithTag("RedTeamLayout").GetComponent<SelectionLayoutGroup>();
        // 타이머연동 RaiseEvent 등록
        PhotonNetwork.OnEventCall += TimerShare;

        if (SoundManager.Instance != null)
            SoundManager.Instance.SelectionRoom_Start_Sound();
    }

    private void OnDestroy()
    {
        PhotonNetwork.OnEventCall -= TimerShare;
    }

    void Update()
    {
        MasterTimerShare();
        TimeOutCheck();
    }

    // 마스터 클라이언트의 타이머를 다른 클라이언트들에 적용
    private void MasterTimerShare()
    {
        if (PhotonNetwork.isMasterClient)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
                timer = 0;

            timerText.text = Mathf.FloorToInt(timer).ToString();

            PhotonNetwork.RaiseEvent((byte)PhotonEventCode.TIMER_SHARE, timer, true, new RaiseEventOptions()
            {
                CachingOption = EventCaching.DoNotCache,
                Receivers = ReceiverGroup.Others
            });
        }
    }

    private void TimeOutCheck()
    {
        // 선택완료가 아닐때 타이머가 0이되면 전투준비로 바꾸고 다시 5초의 대기시간 줌
        if (!isSelectFinished)
        {
            if (timer <= 0)
            {
                isSelectFinished = true;
                titleText.text = "전투 준비!";
                timer = 5.5f;

                if (SoundManager.Instance != null)
                    SoundManager.Instance.Button_Click_Sound();

                // 유저중 한명이라도 선택완료가 아니면 룸으로 이동
                if (PhotonNetwork.isMasterClient)
                {
                    foreach (SelectListing Prefab in selectionLayoutGroup.selectListings)
                    {
                        if (!Prefab.isSelect)
                        {
                            PhotonNetwork.room.IsOpen = true;
                            PhotonNetwork.LoadLevelAsync("Room");
                        }
                    }
                }
            }
        }
        // 이후 다시 5초가 지나서 0초가되면 게임시작
        else
        {
            if (timer <= 0)
            {
                //게임시작
                if (!isLoadStarted)
                {
                    isLoadStarted = true;
                    PhotonNetwork.isMessageQueueRunning = false;
                    StartCoroutine(LoadInGameScene());
                }
            }
        }
    }

    private IEnumerator LoadInGameScene()
    {
        loadingScreen.gameObject.SetActive(true);
        yield return new WaitForSeconds(3f);
        PhotonNetwork.LoadLevel("InGame");
    }

    public void SelectCompleteButton()
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.Button_Click_Sound();

        foreach (SelectListing Prefab in selectionLayoutGroup.selectListings)
        {
            if (Prefab.PhotonPlayer == PhotonNetwork.player)
            {
                if (Prefab.selectedChampionName.Equals(string.Empty))
                    return;
                
                titleText.text = "전투 준비!";
                readyButton.GetComponent<Button>().interactable = false;
                readyButton.GetComponent<Image>().color = Color.gray;
                PlayerData.Instance.championName = Prefab.selectedChampionName;

                if (Prefab.selectedChampionName.Contains("Ashe") || Prefab.selectedChampionName.Contains("ashe"))
                {
                    selectedChampionName = "JeAshe";
                }
                else if (Prefab.selectedChampionName.Contains("Mundo") || Prefab.selectedChampionName.Contains("mundo"))
                {
                    selectedChampionName = "GeoMundo";
                }
                else if (Prefab.selectedChampionName.Contains("Alistar") || Prefab.selectedChampionName.Contains("alistar"))
                {
                    selectedChampionName = "HaYeongsoo";
                }
                Prefab.isSelect = true;
                Prefab.timerText.gameObject.SetActive(false);
                Prefab.championNameText.text = selectedChampionName;

                // 챔피언 미리생성
                GetComponent<PlayerCreator>().MakeChampion();

                this.photonView.RPC("SelectComplete", PhotonTargets.AllViaServer);
                break;
            }
        }

    }

    public void LeaveButton()
    {
        PhotonNetwork.LeaveRoom(true);

        if (SoundManager.Instance != null)
            SoundManager.Instance.Button_Click_Sound();
    }

    [PunRPC]
    public void SelectComplete(PhotonMessageInfo info) // 버튼의 스위치 온/오프
    {
        //같은팀이면 챔피언명 뜸
        foreach (SelectListing Prefab in selectionLayoutGroup.selectListings)
        {
            if (Prefab.PhotonPlayer == info.sender)
            {
                if (PhotonNetwork.player.GetTeam().Equals(info.sender.GetTeam()))
                {
                    if (Prefab.selectedChampionName.Contains("Ashe") || Prefab.selectedChampionName.Contains("ashe"))
                    {
                        selectedChampionName = "JeAshe";
                    }
                    else if (Prefab.selectedChampionName.Contains("Mundo") || Prefab.selectedChampionName.Contains("mundo"))
                    {
                        selectedChampionName = "GeoMundo";
                    }
                    else if (Prefab.selectedChampionName.Contains("Alistar") || Prefab.selectedChampionName.Contains("alistar"))
                    {
                        selectedChampionName = "HaYeongsoo";
                    }
                    Prefab.championNameText.text = selectedChampionName;
                }
                else
                {
                    Prefab.championNameText.text = "준비 완료";
                }
                Prefab.isSelect = true;
                break;
            }
        }

        if (PhotonNetwork.isMasterClient)
        {
            // 유저중 한명이라도 선택완료가 아니면 리턴
            foreach (SelectListing Prefab in selectionLayoutGroup.selectListings)
            {
                if (!Prefab.isSelect)
                    return;
            }
            // 모두 선택완료라면 Timer를 0으로 만들어서 바로 시작대기로 만듬
            timer = 0;
        }
    }

    public void TimerShare(byte eventcode, object content, int senderid)
    {
        if (eventcode == PhotonEventCode.TIMER_SHARE)
        {
            timer = (float)content;

            if (timerText != null)
                timerText.text = Mathf.FloorToInt(timer).ToString();
        }
    }
}