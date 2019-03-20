using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class ChampionButton : Photon.MonoBehaviour
{
    [SerializeField]
    private Image iconImage;

    private bool isSelect = false;
    private Button championButton;
    private GameObject selectRoom;
    private SelectionLayoutGroup selectionLayoutGroup;

    private void Start()
    {
        championButton = GetComponent<Button>();
        selectRoom = GameObject.FindGameObjectWithTag("SelectRoom");
        selectionLayoutGroup = GameObject.FindGameObjectWithTag("RedTeamLayout").GetComponent<SelectionLayoutGroup>();
    }

    public void ButtonClick()
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.Button_Click_Sound();

        // 챔피언 고르면 누른 유저의 프리팹을 찾아 이름과 아이콘 세팅
        foreach (SelectListing Prefab in selectionLayoutGroup.selectListings)
        {
            if(Prefab.PhotonPlayer == PhotonNetwork.player)
            {
                // 유저가 선택완료면 챔피언을 변경하지않음.
                if (Prefab.isSelect)
                    return;

                Prefab.ApplyChampion(gameObject.name);
                Prefab.championIcon.sprite = iconImage.sprite;
                break;
            }
        }

        //RPC
        this.photonView.RPC("SwitchRPC", PhotonTargets.AllViaServer);
        this.photonView.RPC("Sync", PhotonTargets.AllViaServer);
    }

    public void SendRPC(string method)
    {
        this.photonView.RPC(method, PhotonTargets.AllViaServer);
    }

    [PunRPC]
    public void SwitchRPC(PhotonMessageInfo info) // 버튼의 스위치 온/오프
    {
        //RPC 를 보낸 sender와 팀을 비교해서 다른 팀이면 RPC를 받지않음
        if (PhotonNetwork.player.GetTeam().Equals(info.sender.GetTeam()))
        {
            // 내가 선택하면 다른사람들은 RPC 받아서 비활성화
            Switch();
        }
    }

    [PunRPC]
    public void Sync(PhotonMessageInfo info) // 챔피언 이름 할당, 이미지 할당
    {
        // 같은편만 받음
        if (PhotonNetwork.player.GetTeam().Equals(info.sender.GetTeam()))
        {            
            foreach (SelectListing Prefab in selectionLayoutGroup.selectListings)
            {
                if (Prefab.PhotonPlayer == info.sender)
                {
                    Prefab.selectedChampionName = gameObject.name;
                    Prefab.championIcon.sprite = iconImage.sprite;
                    break;
                }
            }
        }
    }

    public void Switch()
    {
        isSelect = !isSelect;
        if (isSelect)
        {
            championButton.interactable = false;
            iconImage.color = Color.gray;
        }
        else
        {
            championButton.interactable = true;
            iconImage.color = Color.white;
        }
    }
}
