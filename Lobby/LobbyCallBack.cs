using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class LobbyCallBack : Photon.PunBehaviour
{
    [SerializeField]
    private GameObject roomJoinErrorWindow;
    
    public override void OnConnectedToMaster()
    {
        Debug.Log("현재 지역:" + PhotonNetwork.networkingPeer.CloudRegion);

        if (PhotonNetwork.connected)
        {
            PhotonNetwork.JoinLobby(TypedLobby.Default);
        }
    }

    public override void OnPhotonRandomJoinFailed(object[] codeAndMsg)
    {
        Debug.Log("랜덤 입장 실패");
        roomJoinErrorWindow.SetActive(true);
        roomJoinErrorWindow.transform.GetChild(0).GetComponent<Text>().text = "현재 입장 가능한 방이 없습니다.";
    }

    public override void OnPhotonCreateRoomFailed(object[] codeAndMsg)
    {
        print("룸 생성 실패" + codeAndMsg[1]);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("룸 생성 완료");
    }

    public override void OnDisconnectedFromPhoton()
    {
        Debug.LogError("포톤 연결 끊김");
    }

    // Lobby 씬에서만 동작함. Room 씬에서는 PlayerLayoutGroup.cs를 참조
    public override void OnJoinedRoom()
    {
        Debug.Log("룸에 입장 완료");
        // 방으로 입장
        PhotonNetwork.LoadLevel("Room");
    }

    public override void OnJoinedLobby()
    {
        print("로비 입장 완료");
        if(SceneManager.GetActiveScene().name != "Lobby")
            SceneManager.LoadScene("Lobby");
    }

    public override void OnPhotonJoinRoomFailed(object[] codeAndMsg)
    {
        print("룸 입장 실패" + codeAndMsg[1]);
        roomJoinErrorWindow.SetActive(true);

        if ((string)codeAndMsg[1] == "Game full")
            roomJoinErrorWindow.transform.GetChild(0).GetComponent<Text>().text = "방 인원이 가득찼습니다.";
        else if ((string)codeAndMsg[1] == "Game closed")
            roomJoinErrorWindow.transform.GetChild(0).GetComponent<Text>().text = "이미 시작된 게임입니다.";
    }
}
