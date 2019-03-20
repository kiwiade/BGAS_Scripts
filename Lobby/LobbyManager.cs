using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    private string _gameVersion = "1";

    [SerializeField]
    private Text playerNameField;
    [SerializeField]
    private GameObject roomCreateWindow;

    [HideInInspector]
    public string selectedRoomName;
    [HideInInspector]
    public GameObject selectedRoomObject;

    private readonly WaitForSeconds connectCheckInterval = new WaitForSeconds(5.0f);

    void Awake()
    {
        Screen.SetResolution(1280, 720, false);

        PhotonNetwork.autoJoinLobby = true;
        PhotonNetwork.automaticallySyncScene = true;
    }

    private void OnEnable()
    {
        Connect();
        NicknameSet();
        StartCoroutine("ConnectCheck");
    }

    IEnumerator ConnectCheck()
    {
        while (true)
        {
            yield return connectCheckInterval;
            if (!PhotonNetwork.connected)
            {
                Connect();
            }
        }
    }

    public void Connect()
    {
        if (PhotonNetwork.connected)
        {
            Debug.Log("포톤서버 연결되있음");
            PhotonNetwork.JoinLobby();
        }
        else
        {
            print("포톤서버에 연결시도");
            PhotonNetwork.ConnectUsingSettings(_gameVersion);
        }
    }

    public void NicknameSet()
    {
        if (PlayerPrefs.HasKey("Nickname"))
        {
            playerNameField.text = PlayerPrefs.GetString("Nickname");
            PhotonNetwork.playerName = PlayerPrefs.GetString("Nickname");
        }
    }

    public void CreateRoomButton()
    {
        if (roomCreateWindow != null)
            roomCreateWindow.SetActive(true);
        PlayLobbyButtonClickSound();
    }

    public void QuickJoinButton()
    {
        PhotonNetwork.JoinRandomRoom();
        PlayLobbyButtonClickSound();
    }

    public void JoinButton()
    {
        if (!string.IsNullOrEmpty(selectedRoomName))
            PhotonNetwork.JoinRoom(selectedRoomName);
        PlayButtonClickSound();
    }

    public void PlayButtonClickSound()
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.Button_Click_Sound();
    }

    private void PlayLobbyButtonClickSound()
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.Button_Lobby_Sound();
    }
}