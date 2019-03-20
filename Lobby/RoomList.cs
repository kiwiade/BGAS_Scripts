using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RoomList : MonoBehaviour
{
    [SerializeField]
    private GameObject selectedImage;
    [SerializeField]
    private Text roomNameText;
    [SerializeField]
    private Text roomMasterText;
    [SerializeField]
    private Text playerCountText;
    [SerializeField]
    private Text roomStatusText;

    public string RoomName { get; private set; }
    public bool Updated { get; set; }

    [HideInInspector]
    public LobbyManager lobbyManager;

    private void Awake()
    {
        lobbyManager = GameObject.FindGameObjectWithTag("LobbyManager").GetComponent<LobbyManager>();
    }

    public void RoomClick()
    {
        if (lobbyManager.selectedRoomObject != gameObject)
        {
            // 기존에 선택된게 있으면 끄기
            if (lobbyManager.selectedRoomObject != null)
                lobbyManager.selectedRoomObject.GetComponent<RoomList>().selectedImage.SetActive(false);

            selectedImage.SetActive(true);
            lobbyManager.selectedRoomName = roomNameText.text;
            lobbyManager.selectedRoomObject = gameObject;
        }
        else
        {
            selectedImage.SetActive(false);
            lobbyManager.selectedRoomName = "";
            lobbyManager.selectedRoomObject = null;
        }

        if (SoundManager.Instance != null)
            SoundManager.Instance.Button_UI_Sound();
    }

    //룸 텍스트 갱신
    public void SetRoomText(RoomInfo room)
    {
        RoomName = room.Name;
        roomNameText.text = room.Name;

        ExitGames.Client.Photon.Hashtable cp = room.CustomProperties;
        roomMasterText.text = (string)cp["MasterName"];
        playerCountText.text = room.PlayerCount.ToString() + "/" + room.MaxPlayers.ToString();

        if (room.IsOpen)
            roomStatusText.text = "대기 중";
        else
            roomStatusText.text = "게임 중";
    }
}