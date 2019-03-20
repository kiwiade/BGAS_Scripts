using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 방생성, 랜덤룸 입장,
/// </summary>
public class CreateRoom : Photon.PunBehaviour, IPunCallbacks
{
    private byte maxPlayersPerRoom = 10;
    private string roomName = "";
    [SerializeField]
    private Text basicRoomNameText;
    [SerializeField]
    private Dropdown teamCountDropdown;

    private void Start()
    {
        basicRoomNameText.text = PhotonNetwork.playerName + "님의 게임";
        maxPlayersPerRoom = (byte)((teamCountDropdown.value + 1) * 2);
    }

    public void MaxPlayerCountChange(int value)
    {
        maxPlayersPerRoom = (byte)((value+1) * 2);
    }

    public void RoomNameChange(string value)
    {
        roomName = value;
    }

    public void RoomCreateButton()
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.Button_Click_Sound();

        RoomOptions roomOptions = new RoomOptions()
        {
            IsVisible = true,
            IsOpen = true,
            MaxPlayers = maxPlayersPerRoom,
            CustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "MasterName", PhotonNetwork.playerName } },
            CustomRoomPropertiesForLobby = new string[] { "MasterName" }
        };


        if (string.IsNullOrEmpty(roomName))
        {
            // 방이름이 없을경우 ~~님의 게임으로 자동설정.
            roomName = PhotonNetwork.playerName + "님의 게임";
        }

        if (PhotonNetwork.CreateRoom(roomName, roomOptions, TypedLobby.Default))
        {
            Debug.Log("룸 생성 성공. RoomName : " + roomName + " MaxPlayers : " + maxPlayersPerRoom.ToString());
        }
        else
        {
            Debug.Log("룸 생성실패");
        }
    }

    public void CancelButton()
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.Button_Click_Sound();
        gameObject.SetActive(false);
    }
}
