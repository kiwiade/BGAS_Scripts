using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 방리스트 새로고침, 방리스트 생성 
public class RoomListLayoutGroup : Photon.PunBehaviour,IPunCallbacks
{ 
    [SerializeField]
    private GameObject roomListingPrefab;
    private List<RoomList> roomListingButtons = new List<RoomList>();

    void Start()
    {
        RefreshRoomList();
    }

    public void RefreshRoomList()
    {   
        OnReceivedRoomListUpdate();
    }

    public override void OnReceivedRoomListUpdate()
    {
        RoomInfo[] rooms = PhotonNetwork.GetRoomList();
        foreach (RoomInfo room in rooms)
        {
            RoomReceived(room);
        }
        RemoveOldRooms();
    }

    private void RoomReceived(RoomInfo room)
    {
        // 룸네임을 체크하여 리스트에 있는 방인지 확인.
        int index = roomListingButtons.FindIndex(x => x.RoomName == room.Name);

        // 리스트에 방이 없다면 화면에 생성하고 리스트에 등록
        if (index == -1)
        {
            if (room.IsVisible)
            {
                GameObject roomListingObj = Instantiate(roomListingPrefab);
                roomListingObj.transform.SetParent(transform, false);

                RoomList roomListing = roomListingObj.GetComponent<RoomList>();
                roomListingButtons.Add(roomListing);

                index = (roomListingButtons.Count - 1);
            }
        }

        // 리스트에 방이 있다면 방 이름을 갱신, 업데이트를 체크해줌
        if (index != -1)
        {
            RoomList roomListing = roomListingButtons[index];
            roomListing.SetRoomText(room);
            roomListing.Updated = true;
        }
    }

    private void RemoveOldRooms()
    {
        List<RoomList> removeRooms = new List<RoomList>();

        // 업데이트가 안된 방은 삭제해줌
        foreach (RoomList roomListing in roomListingButtons)
        {
            if (!roomListing.Updated)
                removeRooms.Add(roomListing);
            else
                roomListing.Updated = false;
        }

        foreach (RoomList roomListing in removeRooms)
        {
            roomListingButtons.Remove(roomListing);
            Destroy(roomListing.gameObject);
        }
    }
}