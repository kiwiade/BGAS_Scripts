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

        // ��â�� Result�����͸� ResultManager�� ����
        tabUI.ResultManagerSave(resultText);

        // ������ ����Ʈ ��������.
        // ������ ������ result ������ ResultManagerSave���� ������
        SceneManager.LoadScene("Result");
    }
}
