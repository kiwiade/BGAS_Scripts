using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class ChatFunction : Photon.MonoBehaviour
{
    //채팅을 저장할 리스트
    [HideInInspector]
    private List<string> chatList = new List<string>();

    [SerializeField]
    private Text chatBox;
    [SerializeField]
    private ScrollRect chatScroll;
    [SerializeField]
    private Scrollbar scrollbar;
    public InputField chatInput;
    // sendTypeDisplay는 인게임씬에서만 할당
    [SerializeField]
    private Text sendTypeDisplay;

    private InGameTimer inGameTimer;
    private bool isTeamSend = false;

    private string redColorCode = "<color=#9D0F29>";
    private string blueColorCode = "<color=#1BA1CF>";
    private string colorCodeEnd = "</color>";
    private string myChampionName;
    private string teamString = "";
    private float enabledTime;
    private bool isInGame = false;

    private SelectionManager selection;

    private void OnLevelWasLoaded(int level)
    {
        string sceneName = SceneManager.GetSceneByBuildIndex(level).name;
        if (sceneName.Equals("InGame"))
        {
            sendTypeDisplay.text = "[전체]";
            inGameTimer = GameObject.FindGameObjectWithTag("InGameManager").GetComponent<InGameTimer>();
            isInGame = true;
            chatBox.fontSize = 22;
        }
        else if (sceneName.Equals("Selection"))
        {
            isInGame = false;
            selection = GameObject.FindGameObjectWithTag("SelectionManager").GetComponent<SelectionManager>();
        }
        else
        {
            isInGame = false;
        }
        myChampionName = PlayerData.Instance.championName;
        chatInput.text = string.Empty;
    }

    public void ChatValueChanged()
    {
        //엔터키로 전송
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if(isInGame)
                chatBox.enabled = true;

            Send();
            chatInput.ActivateInputField();
            if (SoundManager.Instance != null)
                SoundManager.Instance.Chat_Sound();
        }
    }

    private void Update()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        if (currentSceneName.Equals("InGame"))
        {
            if (chatInput.gameObject.activeInHierarchy)
            {
                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    isTeamSend = !isTeamSend;
                    SendTypeChange();
                }
            }
            if(chatBox.enabled)
            {
                enabledTime += Time.deltaTime;
                if(enabledTime >= 10.0f)
                {
                    chatBox.enabled = false;
                    enabledTime = 0f;
                }
            }
        }
        else if (currentSceneName.Equals("Result"))
        {
            isTeamSend = false;
        }
    }

    private void SendTypeChange()
    {
        if (!isTeamSend)
            sendTypeDisplay.text = "[전체]";
        else
            sendTypeDisplay.text = "[팀에게만]";
    }

    public void HideScroll()
    {
        chatScroll.verticalScrollbar = null;
        scrollbar.gameObject.SetActive(false);
    }

    public void RevealScroll()
    {
        scrollbar.gameObject.SetActive(true);
        chatScroll.verticalScrollbar = scrollbar;
    }

    // 인게임씬에서는 UICanvas.cs에서 불러줌. 그 외에는 InputField에 ChatValueChanged함수를 할당하여 호출
    public void Send()
    {
        if (SceneManager.GetActiveScene().name.Equals("Selection"))
        {
            if (selection.timer <= 3.0f)
            {
                return;
            }
        }

        string currentMsg = chatInput.text;
        if (string.IsNullOrEmpty(currentMsg))
            return;

        if (!isTeamSend)
            SendRPC(PhotonTargets.All, currentMsg, false, myChampionName);
        else
            SendRPC(PhotonTargets.All, currentMsg, true, myChampionName);
        chatInput.text = string.Empty;
    }

    //RPC를 사용하여 메세지를 주고받음 
    //RPC함수 'SendMSG 함수'를 가지고 있으면 모두 호출함
    public void SendRPC(PhotonTargets _target, string _msg, bool isTeamChat, string championName)
    {
        photonView.RPC("SendMsg", _target, _msg, isTeamChat, championName);
    }

    [PunRPC]
    private void SendMsg(string _msg, bool isteamchat, string championName, PhotonMessageInfo _info)
    {
        string sendPlayer = _info.sender.ToString().Split("\'".ToCharArray())[1];

        if (isInGame) // 인게임에서만
        {
            if(championName.Contains("Ashe") || championName.Contains("ashe"))
            {
                championName = "제애쉬";
            }
            else if (championName.Contains("Mundo") || championName.Contains("mundo"))
            {
                championName = "거문도";
            }
            else if (championName.Contains("Alistar") || championName.Contains("alistar"))
            {
                championName = "하영수";
            }

            if (isteamchat) // 팀챗 활성화면 같은 팀만 받기
            {
                teamString = "(팀에게만)";
                if (PhotonNetwork.player.GetTeam().ToString().Equals(_info.sender.GetTeam().ToString())) // 팀따라 색주기.
                    sendPlayer = blueColorCode + teamString + sendPlayer + " (" + championName.ToString() + ")" + colorCodeEnd;
                else
                    sendPlayer = redColorCode + teamString + sendPlayer + " (" + championName.ToString() + ")" + colorCodeEnd;

                if (PhotonNetwork.player.GetTeam().ToString().Equals(_info.sender.GetTeam().ToString()))
                    AddChatToChatBox(string.Format("[{0}] {1} : {2}", inGameTimer.text.text, sendPlayer, _msg)); // 타이머, <컬러>sender, 챔프 : 내용
                else
                    return;
            }
            else
            {
                teamString = "(전체에게)";
                if (PhotonNetwork.player.GetTeam().ToString().Equals(_info.sender.GetTeam().ToString())) // 팀따라 색주기.
                    sendPlayer = blueColorCode + teamString + sendPlayer + " (" + championName.ToString() + ")" + colorCodeEnd;
                else
                    sendPlayer = redColorCode + teamString + sendPlayer + " (" + championName.ToString() + ")" + colorCodeEnd;
                AddChatToChatBox(string.Format("[{0}] {1} : {2}", inGameTimer.text.text, sendPlayer, _msg)); // 타이머, <컬러>sender, 챔프 : 내용
            }
        }
        else
        {
            if (PhotonNetwork.player.GetTeam().ToString().Equals(_info.sender.GetTeam().ToString())) // 팀따라 색주기.
                sendPlayer = blueColorCode + sendPlayer + colorCodeEnd;
            else
                sendPlayer = redColorCode + sendPlayer + colorCodeEnd;
            AddChatToChatBox(string.Format("{0} : {1}", sendPlayer, _msg));
        }
    }

    public void SendSystemMessage(PhotonTargets _target, string _msg)
    {
        photonView.RPC("SendSystemMsg", _target, _msg);
    }

    [PunRPC]
    private void SendSystemMsg(string msg)
    {
        string systemMsg = "<color=#ffe963>" + msg + "</color>";
        AddChatToChatBox(systemMsg);
    }

    //메세지를 받아서 메세지박스에 출력 및 리스트에 저장
    private void AddChatToChatBox(string _msg)
    {
        if(isInGame)
            chatBox.enabled = true;
        if (SoundManager.Instance != null)
            SoundManager.Instance.Chat_Sound();
        string chat = chatBox.text;
        chat += string.Format("\n{0}", _msg);
        chatBox.text = chat;
        chatList.Add(_msg);
    }
}
