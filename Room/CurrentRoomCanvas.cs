using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//Room 씬 안에 시작, 나가기버튼 ,강퇴버튼
public class CurrentRoomCanvas : Photon.PunBehaviour
{
    [SerializeField]
    private Text roomNameText;
    [SerializeField]
    private Text roomSettingText;
    [SerializeField]
    private GameObject kickConfirmBox;
    [SerializeField]
    private Text kickConfirmText;
    [SerializeField]
    private GameObject gameStartButton;

    private PhotonPlayer selectedPlayer;

    private void Start()
    {
        kickConfirmBox.SetActive(false);

        roomNameText.text = PhotonNetwork.room.Name;
        int teamcount = PhotonNetwork.room.MaxPlayers / 2;
        roomSettingText.text = "방장 - " + (string)PhotonNetwork.room.CustomProperties["MasterName"]
            + "\n" + teamcount.ToString() + "대" + teamcount.ToString() + " 게임";

        StartButtonActive();
    }

    public void StartButtonActive()
    {
        if (!PhotonNetwork.isMasterClient)
        {
            gameStartButton.GetComponent<Button>().interactable = false;
            gameStartButton.GetComponent<Image>().color = Color.gray;
        }
        else
        {
            gameStartButton.GetComponent<Button>().interactable = true;
            gameStartButton.GetComponent<Image>().color = new Color(16f / 255f, 22f / 255f, 30f / 255f, 1);
        }
    }

    public void StartButton()
    {
        if (PhotonNetwork.isMasterClient)
        {
            //게임 시작하면 참가 못하게 방을 닫음
            PhotonNetwork.room.IsOpen = false;
            Debug.Log("캐릭터 선택으로 이동");

            //챔피언 선택 씬 로드
            PhotonNetwork.LoadLevelAsync("Selection");
        }

        PlayButtonClickSound();
    }

    public void LeaveRoomButton()
    {
        PhotonNetwork.LeaveRoom();
        Debug.Log("방에서 나감");

        PlayButtonClickSound();
    }

    /// <summary>
    /// Room 씬에서 Player 프리팹을 클릭하면 PlayerListing.cs 에서 Photonplayer를 넘겨줌
    /// 강퇴메세지 확인 창이 생성. 예, 아니오로 구분
    /// </summary>
    public void PlayerKick(PhotonPlayer other)
    {
        selectedPlayer = other;
        if (PhotonNetwork.isMasterClient)
        {
            kickConfirmBox.SetActive(true);
            kickConfirmText.text = other.NickName + "님을 추방하시겠습니까?";

            PlayButtonClickSound();
        }
    }

    public void KickOKButton()
    {
        kickConfirmBox.SetActive(false);
        PhotonNetwork.CloseConnection(selectedPlayer);
        Debug.Log(selectedPlayer.NickName + " 강퇴함");
        selectedPlayer = null;

        PlayButtonClickSound();
    }

    public void KickCancelButton()
    {
        kickConfirmBox.SetActive(false);
        selectedPlayer = null;

        PlayButtonClickSound();
    }

    private void PlayButtonClickSound()
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.Button_Click_Sound();
    }
}
