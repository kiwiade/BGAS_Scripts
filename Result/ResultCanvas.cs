using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResultCanvas : Photon.PunBehaviour
{
    [SerializeField]
    private Text gameResultText;
    [SerializeField]
    private Image myChampionImage;
    [SerializeField]
    private Text myGradeText;

    [SerializeField]
    private GameObject[] blueTeamResultInfo;
    [SerializeField]
    private GameObject[] redTeamResultInfo;

    private void Awake()
    {
        Screen.SetResolution(1280, 720, false);
        Cursor.lockState = CursorLockMode.None;

        TabCharacterInfo characterInfo = null;
        ResultManager.ResultData resultData = null;

        gameResultText.text = ResultManager.Instance.result;
        
        for (int i = 0; i < 5; i++)
        {
            // 데이터가 저장되있지않으면 해당 줄의 액티브 꺼버림
            if (string.IsNullOrEmpty(ResultManager.Instance.blueTeamResults[i].championName))
            {
                blueTeamResultInfo[i].SetActive(false);
            }
            // 저장되있으면 저장데이터 불러와서 보여줌
            else
            {
                characterInfo = blueTeamResultInfo[i].GetComponent<TabCharacterInfo>();
                resultData = ResultManager.Instance.blueTeamResults[i];

                DataApply(characterInfo, resultData);

                if (resultData.isMine)
                {
                    myChampionImage.sprite = Resources.Load<Sprite>("Champion/ChampionIcon/" + resultData.championName + "_Big");
                    myGradeText.text = GradeCalculate(resultData.kill, resultData.death, resultData.assist);
                }
            }

            if (string.IsNullOrEmpty(ResultManager.Instance.redTeamResults[i].championName))
            {
                redTeamResultInfo[i].SetActive(false);
            }
            else
            {
                characterInfo = redTeamResultInfo[i].GetComponent<TabCharacterInfo>();
                resultData = ResultManager.Instance.redTeamResults[i];

                DataApply(characterInfo, resultData);

                if (resultData.isMine)
                {
                    myChampionImage.sprite = Resources.Load<Sprite>("Champion/ChampionIcon/" + resultData.championName + "_Big");
                    myGradeText.text = GradeCalculate(resultData.kill, resultData.death, resultData.assist);
                }
            }
        }
    }

    public string GradeCalculate(int kill, int death, int assist)
    {
        float kda = (float)(kill + assist) / (float)death;
        string grade;

        if (kda >= 3.0f)
            grade = "S+";
        else if (kda >= 2.7f)
            grade = "S";
        else if (kda >= 2.4f)
            grade = "S-";
        else if (kda >= 2.1f)
            grade = "A+";
        else if (kda >= 1.8f)
            grade = "A";
        else if (kda >= 1.5f)
            grade = "A-";
        else if (kda >= 1.2f)
            grade = "B+";
        else if (kda >= 0.9f)
            grade = "B";
        else if (kda >= 0.6f)
            grade = "B-";
        else
            grade = "C";

        return grade;
    }

    public void DataApply(TabCharacterInfo characterInfo, ResultManager.ResultData resultData)
    {
        characterInfo.level_Text.text = resultData.level.ToString();
        characterInfo.championIcon_Image.sprite = Resources.Load<Sprite>("Champion/ChampionIcon/" + resultData.championName + "_Big");

        characterInfo.nickname_Text.text = resultData.nickName;

        characterInfo.kda_Text.text = resultData.kill.ToString() + "/" + resultData.death.ToString() + "/" + resultData.assist.ToString();
        characterInfo.cs_Text.text = resultData.cs.ToString();

        for (int i = 0; i < 6; i++)
        {
            if (resultData.items[i] != 0)
            {
                ShopItem.Item it = ShopItem.Instance.itemlist[resultData.items[i]];
                // myItem은 복사본이 아니므로 변경해서는 안됨
                characterInfo.items[i].gameObject.GetComponent<ItemInfo>().myItem = it;
                characterInfo.items[i].sprite = Resources.Load<Sprite>("Item_Image/" + it.iconName);
                characterInfo.items[i].color = Color.white;
            }
            else
            {
                characterInfo.items[i].gameObject.GetComponent<ItemInfo>().myItem = null;
                characterInfo.items[i].sprite = null;
                characterInfo.items[i].color = new Color(1, 1, 1, 0);
            }
        }

        if (resultData.accessoryItem != 0)
        {
            ShopItem.Item it = ShopItem.Instance.itemlist[resultData.accessoryItem];
            characterInfo.accessoryItem.gameObject.GetComponent<ItemInfo>().myItem = it;
            characterInfo.accessoryItem.sprite = Resources.Load<Sprite>("Item_Image/" + it.iconName);
            characterInfo.accessoryItem.color = Color.white;
        }
        else
        {
            characterInfo.accessoryItem.gameObject.GetComponent<ItemInfo>().myItem = null;
            characterInfo.accessoryItem.sprite = null;
            characterInfo.accessoryItem.color = new Color(1, 1, 1, 0);
        }
    }

    public void LeaveButton()
    {
        ResultManager.Instance.ListReset();
        PhotonNetwork.player.SetTeam(PunTeams.Team.none);

        PhotonNetwork.Disconnect();
        SceneManager.LoadScene("Lobby");
    }
}
