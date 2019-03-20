using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectListing : Photon.MonoBehaviour
{
    //프리팹에 포톤플레이어를 할당해주는 변수
    public PhotonPlayer PhotonPlayer { get; set; }

    public Text championNameText;
    [SerializeField]
    private Text playerNameText;
    public Image championIcon;
    public Text timerText;

    [HideInInspector]
    public bool isSelect = false;
    [HideInInspector]
    public string selectedChampionName = string.Empty;

    private SelectionManager selectionManager;
    private SelectionLayoutGroup selectionLayoutGroup;

    private void Start()
    {
        if (PhotonPlayer.IsLocal)
        {
            GetComponent<Image>().color = new Color(0f / 255f, 200f / 255f, 255f / 255f, 0.7f);
            timerText.gameObject.SetActive(true);
        }

        gameObject.name = PhotonPlayer.NickName;
        championNameText.text = "선택중...";
        selectionManager = GameObject.FindGameObjectWithTag("SelectionManager").GetComponent<SelectionManager>();
        selectionLayoutGroup = GameObject.FindGameObjectWithTag("RedTeamLayout").GetComponent<SelectionLayoutGroup>();
    }

    private void Update()
    {
        if(!isSelect)
            timerText.text = selectionManager.timerText.text;
    }

    // 텍스트에 플레이어 닉네임 할당
    public void ApplyPhotonPlayer(PhotonPlayer photonPlayer)
    {
        playerNameText.text = photonPlayer.NickName;
    }

    //현재 플레이어 리스트에 챔피언을 할당.
    public void ApplyChampion(string name)
    {
        // 이미지는 버튼에서 바꿔줌

        // 선택된 챔피언이 없다면 할당
        if (selectedChampionName.Equals(string.Empty))
        {
            selectedChampionName = name;
        }
        // 선택된 챔피언이 변경되었으면
        else if(!selectedChampionName.Equals(name))
        {
            // 챔피언리스트에서 이전에 선택된 챔피언을 찾아 RPC를 보내 다른사람이 선택가능하게 함.
            GameObject.FindGameObjectWithTag("ChampList").transform.Find(selectedChampionName)
                .GetComponent<ChampionButton>().SendRPC("SwitchRPC");

            // 이후 챔피언 바꿔줌
            selectedChampionName = name;
        }
    }

    public void SendRPC(string method)
    {
        this.photonView.RPC(method, PhotonTargets.AllViaServer);
    }
}
