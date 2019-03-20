using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 룸 씬에서 플레이어리스트 프리팹을 생성, 팀에따라 나누고, 플레이어가 나가면 삭제.
/// </summary>
public class PlayerLayoutGroup : Photon.PunBehaviour
{
    [SerializeField]
    private GameObject playerListingPrefab;
    [SerializeField]
    private GameObject[] team1ViewPort;
    [SerializeField]
    private GameObject[] team2ViewPort;
    [SerializeField]
    private GameObject[] team1JoinButton;
    [SerializeField]
    private GameObject[] team2JoinButton;

    [HideInInspector]
    public List<PlayerListing> playerListings = new List<PlayerListing>();

    [HideInInspector]
    public int team1Count = 0;
    [HideInInspector]
    public int team2Count = 0;

    private bool[] team1Check = new bool[5] { false, false, false, false, false };
    private bool[] team2Check = new bool[5] { false, false, false, false, false };

    private GameObject currentPlayer;

    private void Start()
    {
        PlayersTeamSet();
        RoomLayoutUpdate();
    }

    // 룸에 입장하면 최대 인원수에 맞춰서 안쓰는 레이아웃을 꺼버림
    public void RoomLayoutUpdate()
    {
        // 0~5번 중 안쓰는 레이아웃 끔
        // 2명이면 0만 사용, 4명이면 0,1만사용, ...
        for (int i = PhotonNetwork.room.MaxPlayers/2; i<=4; i++)
        {
            foreach(Transform tr in team1ViewPort[i].transform)
            {
                tr.gameObject.SetActive(false);
            }
            foreach (Transform tr in team2ViewPort[i].transform)
            {
                tr.gameObject.SetActive(false);
            }
        }
    }

    // 룸에 들어오면 기본세팅 1번 실행
    public void PlayersTeamSet()
    {
        print("룸 플레이어 목록 불러옴");

        //룸안에 모든 플레이어를 불러와 팀별로 정렬
        PhotonPlayer[] photonPlayers = PhotonNetwork.playerList;
        for (int i = 0; i < photonPlayers.Length; i++)
        {
            if(photonPlayers[i].GetTeam().Equals(PunTeams.Team.red))
            {
                RedTeamJoin(photonPlayers[i]);
            }
            else if(photonPlayers[i].GetTeam().Equals(PunTeams.Team.blue))
            {
                BlueTeamJoin(photonPlayers[i]);
            }
            // 팀이 없으면 (나) 기본 입장방식으로 처리
            else
            {
                PlayerJoinedRoom(photonPlayers[i]);
            }
        }
        JoinButtonUpdate();
    }

    // 플레이어가 룸에 들어오면
    public void PlayerJoinedRoom(PhotonPlayer photonPlayer)
    {
        if (photonPlayer.Equals(null))
            return;
        
        //팀카운트에 따라 플레이어가 속한 뷰포트 및 팀 할당
        if (team1Count == team2Count)
        {
            RedTeamJoin(photonPlayer);
        }
        else if (team1Count > team2Count)
        {
            BlueTeamJoin(photonPlayer);
        }
        else if (team1Count < team2Count)
        {
            RedTeamJoin(photonPlayer);
        }

        // 참가버튼 재설정
        JoinButtonUpdate();
    }

    // 레드팀(1팀) 조인
    public void RedTeamJoin(PhotonPlayer photonPlayer)
    {
        //플레이어가 이미 들어왔는데, 프리팹이 다시 생성되는걸 방지
        PlayerLeftRoom(photonPlayer);

        //플레이어 프리팹 생성, 리스트에 추가
        currentPlayer = Instantiate(playerListingPrefab);
        PlayerListing playerListing = currentPlayer.GetComponent<PlayerListing>();
        playerListing.PlayerNameTextChange(photonPlayer.NickName);
        playerListing.PhotonPlayer = photonPlayer;
        playerListings.Add(playerListing);

        // 플레이어 프리팹이 자리에 들어가므로 '비어있음'텍스트와 '참가'버튼을 끔
        foreach (Transform tr in team1ViewPort[team1Count].transform)
        {
            if (tr.name != "Line")
                tr.gameObject.SetActive(false);
        }

        // 프리팹 위치 설정
        playerListing.transform.SetParent(team1ViewPort[team1Count].transform, false);
        playerListing.transform.localPosition = Vector3.zero;

        // bool 변수 체크. 스크립트에 팀과 번호 기록
        team1Check[team1Count] = true;
        playerListing.viewnum = team1Count;
        playerListing.Team = 'r';

        // 팀인원수 증가 및 팀 설정
        team1Count++;
        photonPlayer.SetTeam(PunTeams.Team.red);
    }

    // 블루팀(2팀) 조인
    public void BlueTeamJoin(PhotonPlayer photonPlayer)
    {
        //플레이어가 이미 들어왔는데, 프리팹이 다시 생성되는걸 방지
        PlayerLeftRoom(photonPlayer);

        //플레이어 프리팹 생성, 리스트에 추가
        currentPlayer = Instantiate(playerListingPrefab);
        PlayerListing playerListing = currentPlayer.GetComponent<PlayerListing>();
        playerListing.PlayerNameTextChange(photonPlayer.NickName);
        playerListing.PhotonPlayer = photonPlayer;
        playerListings.Add(playerListing);

        // 플레이어 프리팹이 자리에 들어가므로 '비어있음'텍스트와 '참가'버튼을 끔
        foreach (Transform tr in team2ViewPort[team2Count].transform)
        {
            if (tr.name != "Line")
                tr.gameObject.SetActive(false);
        }

        playerListing.transform.SetParent(team2ViewPort[team2Count].transform, false);
        playerListing.transform.localPosition = Vector3.zero;

        team2Check[team2Count] = true;
        playerListing.viewnum = team2Count;
        playerListing.Team = 'b';

        team2Count++;
        photonPlayer.SetTeam(PunTeams.Team.blue);
    }

    // 플레이어가 룸에서 나가면
    public void PlayerLeftRoom(PhotonPlayer photonPlayer)
    {
        // 나간 플레이어의 인덱스를 검색
        int index = playerListings.FindIndex(x => x.PhotonPlayer == photonPlayer);

        //똑같은 플레이어의 인덱스를 찾으면
        if (index != -1)
        {   //프리팹 삭제
            PlayerListing leftPlayer = playerListings[index];
            Destroy(playerListings[index].gameObject);
            playerListings.RemoveAt(index);

            // 삭제할 플레이어의 위치 받아옴.
            int viewnum = leftPlayer.viewnum;

            // 팀카운트 조정
            if (photonPlayer.GetTeam().Equals(PunTeams.Team.red))
            {
                // 팀 인원수 감소
                team1Count--;

                // 나간위치 뒤에 유저가 있으면 1칸씩 당김
                for(int i= viewnum + 1; i < PhotonNetwork.room.MaxPlayers / 2; i++)
                {
                    foreach (PlayerListing playerPrefab in playerListings)
                    {
                        if(playerPrefab.Team == 'r' && playerPrefab.viewnum == i)
                        {
                            playerPrefab.transform.SetParent(team1ViewPort[i - 1].transform);
                            playerPrefab.transform.localPosition = Vector3.zero;
                            playerPrefab.viewnum = i-1;
                            break;
                        }
                    }
                }

                // 마지막자리는 false로 만들고 참가버튼과 비어있음 텍스트를 다시 켬
                team1Check[team1Count] = false;
                foreach (Transform tr in team1ViewPort[team1Count].transform)
                    tr.gameObject.SetActive(true);
            }

            if (photonPlayer.GetTeam().Equals(PunTeams.Team.blue))
            {
                team2Count--;

                for (int i = viewnum + 1; i < PhotonNetwork.room.MaxPlayers / 2; i++)
                {
                    foreach (PlayerListing playerPrefab in playerListings)
                    {
                        if (playerPrefab.Team == 'b' && playerPrefab.viewnum == i)
                        {
                            playerPrefab.transform.SetParent(team2ViewPort[i - 1].transform);
                            playerPrefab.transform.localPosition = Vector3.zero;
                            playerPrefab.viewnum = i-1;
                            break;
                        }
                    }
                }

                team2Check[team2Count] = false;
                foreach (Transform tr in team2ViewPort[team2Count].transform)
                    tr.gameObject.SetActive(true);
            }
        }
        JoinButtonUpdate();
    }

    public void JoinButtonUpdate()
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.Button_Click_Sound();

        // 참가버튼 다끄고
        for (int i=0; i<PhotonNetwork.room.MaxPlayers/2; i++)
        {
            team1JoinButton[i].SetActive(false);
            team2JoinButton[i].SetActive(false);
        }
        // 처음 사람이 없는구간의 참가버튼만 켬
        if (PhotonNetwork.player.GetTeam().Equals(PunTeams.Team.blue))
        {
            for (int i = 0; i < PhotonNetwork.room.MaxPlayers / 2; i++)
            {
                if (team1Check[i] == false)
                {
                    team1JoinButton[i].SetActive(true);
                    break;
                }
            }
        }
        if (PhotonNetwork.player.GetTeam().Equals(PunTeams.Team.red))
        {
            for (int i = 0; i < PhotonNetwork.room.MaxPlayers / 2; i++)
            {
                if (team2Check[i] == false)
                {
                    team2JoinButton[i].SetActive(true);
                    break;
                }
            }
        }
    }

    public void TeamChange()
    {
        this.photonView.RPC("PlayerTeamChange", PhotonTargets.All, PhotonNetwork.player);
    }

    [PunRPC]
    public void PlayerTeamChange(PhotonPlayer player)
    {
        foreach (var playerPrefab in playerListings)
        {
            if (playerPrefab.PhotonPlayer.Equals(player))
            {
                if (player.GetTeam().Equals(PunTeams.Team.red) && team2Count < PhotonNetwork.room.MaxPlayers / 2)
                {
                    BlueTeamJoin(player);
                }
                else if (player.GetTeam().Equals(PunTeams.Team.blue) && team1Count < PhotonNetwork.room.MaxPlayers / 2)
                {
                    RedTeamJoin(player);
                }
                break;
            }
        }
        JoinButtonUpdate();
    }
}
