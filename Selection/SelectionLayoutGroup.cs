using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Selection 씬에서 각 플레이어 프리팹을 팀별로, 뷰포트에 생성해줌
public class SelectionLayoutGroup : Photon.PunBehaviour
{
    [HideInInspector]
    public List<SelectListing> selectListings;

    //플레이어 리스트 프리팹
    [SerializeField]
    private GameObject ourTeamPlayerPrefab;
    [SerializeField]
    private GameObject enemyTeamPlayerPrefab;
    [SerializeField]
    private GameObject ourTeamViewPort;
    [SerializeField]
    private GameObject enemyTeamViewPort;

    //현재 로컬플레이어의 프리팹 과 팀
    private GameObject currentSelectingPrefab;
    private PhotonPlayer me;

    private void Start()
    {
        selectListings = new List<SelectListing>();
        selectListings.Clear();

        //로컬플레이어 프리팹 먼저 생성
        me = PhotonNetwork.player;
        if (me.IsLocal)
        {
            // 프리팹 생성후 리스트 등록. 세팅
            currentSelectingPrefab = Instantiate(ourTeamPlayerPrefab, ourTeamViewPort.transform, false);
            SelectListing selectListing = currentSelectingPrefab.GetComponent<SelectListing>();
            selectListings.Add(selectListing);

            selectListing.PhotonPlayer = me;
            selectListing.ApplyPhotonPlayer(me);
        }
        //다른플레이어 리스트를 받아와서 플레이어 리스트 만큼 프리팹을 만듬
        PhotonPlayer[] photonPlayers = PhotonNetwork.otherPlayers;
        for (int i = 0; i < photonPlayers.Length; i++)
        {
            PlayerJoinedRoom(photonPlayers[i]);
        }
    }

    // 나 제외
    public void PlayerJoinedRoom(PhotonPlayer photonPlayer)
    {
        if (photonPlayer.Equals(null))
        {
            return;
        }
        GameObject otherPlayer;

        //로컬플레이어가 아니면 로컬플레이어가 속한 팀에따라 좌우를 나눔
        if (me.GetTeam() != photonPlayer.GetTeam())
        {
            otherPlayer = Instantiate(enemyTeamPlayerPrefab);
            otherPlayer.transform.SetParent(enemyTeamViewPort.transform, false);
        }
        else
        {
            otherPlayer = Instantiate(ourTeamPlayerPrefab);
            otherPlayer.transform.SetParent(ourTeamViewPort.transform, false);
        }

        SelectListing selectListing = otherPlayer.GetComponent<SelectListing>();
        selectListings.Add(selectListing);

        selectListing.PhotonPlayer = photonPlayer;
        selectListing.ApplyPhotonPlayer(photonPlayer);
    }
}
