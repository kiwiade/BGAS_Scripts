using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RoomClickCheck : MonoBehaviour, IPointerClickHandler
{
    private bool oneClicked = false;
    private float firstClickTime = 0;
    private readonly float doubleClickCheckInterval = 0.4f;

    private RoomList roomList;

    private void Awake()
    {
        roomList = GetComponent<RoomList>();
    }

    private void Update()
    {
        DoubleClickCheck();
    }

    // 원클릭 후 일정시간이 지나면 원클릭상태를 풀어버림(그전에 한번더 클릭시 더블클릭으로 처리)
    private void DoubleClickCheck()
    {
        if (oneClicked)
        {
            if ((Time.time - firstClickTime) > doubleClickCheckInterval)
                oneClicked = false;
        }
    }

    // 더블클릭 체크
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            // 첫클릭하면 변수 true, 첫클릭한 시간체크
            if (!oneClicked)
            {
                oneClicked = true;
                firstClickTime = Time.time;
                roomList.RoomClick();
            }
            // 더블클릭이면
            else
            {
                oneClicked = false;
                roomList.lobbyManager.JoinButton();
                if (SoundManager.Instance != null)
                    SoundManager.Instance.Button_UI_Sound();
            }
        }
    }
}
