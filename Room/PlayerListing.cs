using UnityEngine;
using UnityEngine.UI;

//room 씬 안 생성되는 플레이어 프리팹 안에 있는 텍스트에 플레이어 이름을 할당
public class PlayerListing : Photon.PunBehaviour
{
    public Text playerName;
    private Button playerPrefabButton;

    public PhotonPlayer PhotonPlayer { get; set; }
    [HideInInspector]
    public char Team;
    [HideInInspector]
    public int viewnum;

    private void Start()
    {
        if (!PhotonPlayer.IsMasterClient)
            ButtonAddListener();
        else
            MasterClientImage();
    }

    public void PlayerNameTextChange(string nickname)
    {
        playerName.text = nickname;
    }

    //생성된 버튼 클릭하면, CurrentRoomCanvas.cs 에 PlayerKick() 로 PhotonPlayer를 넘겨줌
    public void ButtonAddListener()
    {
        CurrentRoomCanvas currentRoomCanvas = GetComponentInParent<CurrentRoomCanvas>();

        playerPrefabButton = transform.Find("KickButton").GetComponent<Button>();
        playerPrefabButton.onClick.AddListener(() => currentRoomCanvas.PlayerKick(PhotonPlayer));
    }

    public void MasterClientImage()
    {
        // KickButton의 이미지를 방장 이미지로 바꿔주고 버튼리스너 안붙여줌
        transform.Find("KickButton").GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/crown");
    }
}
