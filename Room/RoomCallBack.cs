using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class RoomCallBack : Photon.PunBehaviour
{
    private PlayerLayoutGroup playerLayoutGroup;

    [SerializeField]
    private ChatFunction chatManager;

    private void Start()
    {
        playerLayoutGroup = GameObject.FindGameObjectWithTag("RedTeamLayout").GetComponent<PlayerLayoutGroup>();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("현재 지역:" + PhotonNetwork.networkingPeer.CloudRegion);

        if (PhotonNetwork.connected)
        {
            PhotonNetwork.JoinLobby(TypedLobby.Default);
        }
    }

    public override void OnDisconnectedFromPhoton()
    {
        Debug.LogError("OnDisconnectedFromPhoton() called by CallbackManager.cs");
    }

    public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        Debug.Log(newPlayer + "들어옴");
        playerLayoutGroup.PlayerJoinedRoom(newPlayer);

        if (PhotonNetwork.isMasterClient)
            chatManager.SendSystemMessage(PhotonTargets.All, newPlayer.NickName + "님이 룸에 참가했습니다.");

        if (SoundManager.Instance != null)
            SoundManager.Instance.Enter_User_Sound();
    }

    public override void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
    {
        Debug.Log(otherPlayer + "나감");
        playerLayoutGroup.PlayerLeftRoom(otherPlayer);

        if (SoundManager.Instance != null)
            SoundManager.Instance.ExitRoom_Sound();

        // 사람이 나가면 방장체크하여 룸설정 바꿔줌
        if (PhotonNetwork.isMasterClient)
        {
            // 채팅창에 시스템메세지 출력
            chatManager.SendSystemMessage(PhotonTargets.All, otherPlayer.NickName + "님이 룸에서 나갔습니다.");

            // 룸 설정에서 마스터 변경
            ExitGames.Client.Photon.Hashtable cp = PhotonNetwork.room.CustomProperties;
            cp["MasterName"] = PhotonNetwork.playerName;
            PhotonNetwork.room.SetCustomProperties(cp);

            // 해당 플레이어 프리팹 찾아서 방장으로 바꿔주고 강퇴클릭 리스너 없앰
            foreach (PlayerListing playerListing in playerLayoutGroup.playerListings)
            {
                if (playerListing.playerName.text == PhotonNetwork.playerName)
                {
                    playerListing.transform.Find("KickButton").GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/crown");
                    playerListing.transform.Find("KickButton").GetComponent<Button>().onClick.RemoveAllListeners();
                    break;
                }
            }

            // 스타트 버튼 활성화
            GameObject.FindGameObjectWithTag("CurrentRoom").GetComponent<CurrentRoomCanvas>().StartButtonActive();
        }

        if (PhotonNetwork.room.PlayerCount == 1)
        {
            PhotonPlayer leftPlayer = PhotonNetwork.playerList[0];
            PhotonNetwork.SetMasterClient(leftPlayer);
            Debug.Log(leftPlayer + "이(가) 방장이 되었습니다.");
        }
    }

    public override void OnLeftRoom()
    {
        if (PhotonNetwork.player.GetTeam().Equals(PunTeams.Team.red))
        {
            playerLayoutGroup.team1Count--;
        }
        else if (PhotonNetwork.player.GetTeam().Equals(PunTeams.Team.blue))
        {
            playerLayoutGroup.team2Count--;
        }
        PhotonNetwork.player.SetTeam(PunTeams.Team.none);
    }

    public override void OnJoinedLobby()
    {
        if (SceneManager.GetActiveScene().name != "Lobby")
        {
            SceneManager.LoadScene("Lobby");
        }
    }
}
