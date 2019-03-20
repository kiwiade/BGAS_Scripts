using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIWinLose : MonoBehaviour
{
    [SerializeField]
    private TabUI tabUI;
    [SerializeField]
    private GameObject winText;
    [SerializeField]
    private GameObject loseText;

    public void ExitButton()
    {
        string resultText = "";
        if (winText.activeSelf)
            resultText = winText.GetComponent<Text>().text;
        else if (loseText.activeSelf)
            resultText = loseText.GetComponent<Text>().text;

        // 탭창의 Result데이터를 ResultManager에 저장
        tabUI.ResultManagerSave(resultText);

        // 아이템 리스트 리셋해줌.
        // 아이템 리셋은 result 저장후 ResultManagerSave에서 실행함
        SceneManager.LoadScene("Result");
    }
}
