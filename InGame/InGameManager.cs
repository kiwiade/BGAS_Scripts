using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameManager : Photon.PunBehaviour
{
    public int loadedPlayer = 0;
    public List<int> redIncomingEvents = new List<int>();
    public List<int> blueIncomingEvents = new List<int>();

    public List<GameObject> redTeamPlayer = new List<GameObject>();
    public List<GameObject> blueTeamPlayer = new List<GameObject>();

    private List<Vector3> redPos = new List<Vector3>();
    private List<Vector3> bluePos = new List<Vector3>();

    [SerializeField]
    private List<GameObject> wellTowers = new List<GameObject>();

    public int minionDeadCount = 0;
    public Vector3 RedPos
    {
        get
        {
            return redPos[0];
        }
    }

    public Vector3 BluePos
    {
        get
        {
            return bluePos[0];
        }
    }

    // 팀킬은 tab갱신할때 거기서 갱신. 타워와 드래곤은 죽을때 hitme에서 불러주기
    public int blueTeamTowerKill = 0;
    public int redTeamTowerKill = 0;
    public int blueTeamDragonKill = 0;
    public int redTeamDragonKill = 0;

    // 초기 동기화용 변수
    public bool isLoaded = false; // 로딩 완료?
    public bool runOnce = false;
    private bool timerOnce = false;
    public bool isGameEnd = false; // 게임 종료?

    public float waitingTime = 10f;

    public SystemMessage sysmsg;
    private GameObject startingWall;
    private GameObject logo;
    private InGameTimer ingameTimer;
    private RTS_Cam.RTS_Camera mainCam;
    [SerializeField]
    private GameObject nexusRed;
    [SerializeField]
    private GameObject nexusBlue;
    public string team;
    public AudioSource BGM;
    //4 6 8 10 12 / 5 10 
    //262 4 6 8 70 /5 270
    private void Awake()
    {
        startingWall = transform.GetChild(0).gameObject;
        logo = transform.GetChild(1).gameObject;
        ingameTimer = GetComponent<InGameTimer>();
        mainCam = Camera.main.GetComponent<RTS_Cam.RTS_Camera>();

        if (!sysmsg)
            sysmsg = GameObject.FindGameObjectWithTag("SystemMsg").GetComponent<SystemMessage>();

        //이벤트 수신
        PhotonNetwork.OnEventCall += SceneLoaded_Received;

        //포지션 세팅
        for (int i = 0; i < 5; i++)
        {
            redPos.Add(new Vector3(4 + (i * 2), 0.5f, 10f));
        }
        for (int i = 0; i < 5; i++)
        {
            bluePos.Add(new Vector3(262 + (i * 2), 0.5f, 270f));
        }
    }

    private void Start()
    {
        team = PhotonNetwork.player.GetTeam().ToString().ToLower();
        if (!BGM)
        {
            BGM = GameObject.FindGameObjectWithTag("BGMSource").GetComponent<AudioSource>();
            BGM.PlayDelayed(10f);
            BGM.volume = 1f;
        }
    }

    private void SceneLoaded_Received(byte eventCode, object content, int senderId)
    {
        if (this == null)
            return;

        object[] datas = content as object[];
        GameObject temp = null;
        if (eventCode.Equals(PhotonEventCode.LOADING_COMPLETE)) // 마스터클라 전용  챔피언 오브젝트 등록
        {
            int receiveViewID = (int)datas[0];
            foreach (int viewID in redIncomingEvents)
            {
                if (viewID == receiveViewID)
                    return;
            }
            foreach (int viewID in blueIncomingEvents)
            {
                if (viewID == receiveViewID)
                    return;
            }

            loadedPlayer++;

            if ((string)datas[1] == "red")
            {
                redIncomingEvents.Add(receiveViewID);
                temp = PhotonView.Find(redIncomingEvents[redIncomingEvents.Count - 1]).gameObject;
                redTeamPlayer.Add(temp);
                temp.transform.position = redPos[redIncomingEvents.Count - 1];
            }
            else if ((string)datas[1] == "blue")
            {
                blueIncomingEvents.Add(receiveViewID);
                temp = PhotonView.Find(blueIncomingEvents[blueIncomingEvents.Count - 1]).gameObject;
                blueTeamPlayer.Add(temp);
                temp.transform.position = bluePos[blueIncomingEvents.Count - 1];
            }

            SetChampionMove();
        }
        else if (eventCode.Equals(PhotonEventCode.RELEASE_STARTING_WALL)) // 모든 클라이언트 수신. 벽해제
        {
            for (int i = 0; i < wellTowers.Count; ++i)
                wellTowers[i].SetActive(true);
            runOnce = true;
            StartCoroutine(StartingWall_Off());
        }
        else if (eventCode.Equals(PhotonEventCode.SYNC_USER_VIEWID))// 타 클라이언트 수신. 매니저 동기화용
        {
            temp = null;
            int redTotalCount = (int)datas[datas.Length - 1];
            int redCount = 0;
            int blueCount = 0;
            for (int i = 0; i < datas.Length - 1; i++)//blue
            {
                temp = PhotonView.Find((int)datas[i]).gameObject;
                if (i < redTotalCount)
                {
                    temp.transform.position = redPos[redCount];
                    redCount++;
                    redTeamPlayer.Add(temp);
                }
                else
                {
                    temp.transform.position = bluePos[blueCount];
                    blueCount++;
                    blueTeamPlayer.Add(temp);
                }
                SetChampionMove();
            }
        }
        else if (eventCode.Equals(PhotonEventCode.GAME_ENDED))//게임종료
        {
            isGameEnd = true;
            // 게임한번 끝나고 새게임시작할때 이전 인게임매니저에 이벤트가 들어올때가 있어서 끝날때 빼줌
            PhotonNetwork.OnEventCall -= SceneLoaded_Received;

            //진팀이랑 같은가.
            if ((string)datas[0] == "red")//진 팀이 레드인가?
            {
                StartCoroutine(EndSequence("red"));//레드 넥서스 꽝)
            }
            else if ((string)datas[0] == "blue")
            {
                StartCoroutine(EndSequence("blue"));//블루 넥서스 꽝
            }
        }
    }


    private void Event_StartingWall_Off()
    {
        byte evcode = PhotonEventCode.RELEASE_STARTING_WALL;
        object[] datas = { evcode };
        RaiseEventOptions op = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(evcode, datas, true, op);
        
    }

    private void Event_SendViewID()
    {
        byte evcode = PhotonEventCode.SYNC_USER_VIEWID;

        object[] datas = new object[redIncomingEvents.Count + blueIncomingEvents.Count + 1];
        for (int i = 0; i < redIncomingEvents.Count; i++)
        {
            datas[i] = redIncomingEvents[i];
        }
        for (int i = 0; i < blueIncomingEvents.Count; i++)
        {
            datas[redIncomingEvents.Count + i] = blueIncomingEvents[i];
        }
        datas[redIncomingEvents.Count + blueIncomingEvents.Count] = redIncomingEvents.Count;

        RaiseEventOptions op = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
        PhotonNetwork.RaiseEvent(evcode, datas, true, op);
    }

    public void GameEnded(string Team)//모두에게 진팀 보내주기
    {
        byte evcode = PhotonEventCode.GAME_ENDED;
        object[] datas = new object[] { (string)Team };
        RaiseEventOptions op = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(evcode, datas, true, op);
        PhotonNetwork.SendOutgoingCommands();
    }

    IEnumerator EndSequence(string team)
    {
        yield return new WaitForSeconds(0.5f);
        // 상점이나 옵션 열려있으면 꺼버림
        var shop = GameObject.FindGameObjectWithTag("ShopCanvas");
        if (shop != null)
            shop.SetActive(false);

        var option = GameObject.FindGameObjectWithTag("OptionCanvas");
        if (option != null)
            option.SetActive(false);

        if (team.Equals("red"))
        {
            mainCam.SetTarget(nexusRed.transform);
            yield return new WaitForSeconds(1f);
            //넥서스 꽝 -> 넥서스 파괴전에 레이즈 이벤트로 다 보내서 WinLose시스템 메세지 출력할것.
            nexusRed.GetComponent<SuppressorBehaviour>().bomb = true;
        }
        else if (team.Equals("blue"))
        {
            mainCam.SetTarget(nexusBlue.transform);
            //넥서스 꽝 -> 넥서스 파괴전에 레이즈 이벤트로 다 보내서 WinLose시스템 메세지 출력할것.
            yield return new WaitForSeconds(1f);
            nexusBlue.GetComponent<SuppressorBehaviour>().bomb = true;
        }
        yield return null;
    }

    IEnumerator StartingWall_Off()
    {
        yield return new WaitForSeconds(waitingTime);
        PhotonNetwork.automaticallySyncScene = false;

        for (int i = 0; i < startingWall.transform.childCount - 1; i++)
        {
            Destroy(startingWall);
            startingWall.GetComponentInChildren<Pathfinding.NavmeshCut>().enabled = false;
        }
        Destroy(logo);
        Invoke("Annoucement", 3f);

        // 로딩이끝나면 카메라를 다시 나에게로 돌려줌
        if (PhotonNetwork.player.GetTeam().ToString().Equals("red"))
        {
            foreach (GameObject go in redTeamPlayer)
            {
                if (go.GetPhotonView().owner.Equals(PhotonNetwork.player))
                {
                    mainCam.SetTarget(go.transform);
                    break;
                }
            }
        }
        else
        {
            foreach (GameObject go in blueTeamPlayer)
            {
                if (go.GetPhotonView().owner.Equals(PhotonNetwork.player))
                {
                    mainCam.SetTarget(go.transform);
                    break;
                }
            }
        }

        yield return new WaitForSeconds(2.0f);
        mainCam.ResetTarget();

        yield return null;
    }

    private void Annoucement()
    {
        sysmsg.Annoucement(1, true);
    }

    private void Update()
    {
        if (PhotonNetwork.isMasterClient)
        {
            if (loadedPlayer == PhotonNetwork.playerList.Length && !runOnce)
            {
                runOnce = true;
                Event_SendViewID();
                Event_StartingWall_Off();
            }
            if (!timerOnce && runOnce)
            {
                if (ingameTimer.temp >= 28f)
                {
                    timerOnce = true;
                    sysmsg.Annoucement(2, true);
                }
            }
        }
    }

    private void SetChampionMove()
    {
        if (redTeamPlayer.Count >= 1)
        {
            for (int i = 0; i < redTeamPlayer.Count; i++)
            {
                redTeamPlayer[i].transform.GetComponent<Pathfinding.AIBase>().enabled = true;
                redTeamPlayer[i].transform.GetComponent<Pathfinding.RVO.RVOController>().radius = 0.5f;
                redTeamPlayer[i].transform.GetComponent<Pathfinding.RVO.RVOController>().enabled = true;
            }
        }
        if (blueTeamPlayer.Count >= 1)
        {
            for (int i = 0; i < blueTeamPlayer.Count; i++)
            {
                blueTeamPlayer[i].transform.GetComponent<Pathfinding.AIBase>().enabled = true;
                blueTeamPlayer[i].transform.GetComponent<Pathfinding.RVO.RVOController>().radius = 0.5f;
                blueTeamPlayer[i].transform.GetComponent<Pathfinding.RVO.RVOController>().enabled = true;
            }
        }
    }

    // 마스터가 겜 나가면 플레이어들 로비로 보내버림
    public override void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
    {
        if (otherPlayer.IsMasterClient)
        {
            PhotonNetwork.LeaveRoom();
        }
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("Lobby");
    }
}