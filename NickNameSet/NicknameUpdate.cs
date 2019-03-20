using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.SceneManagement;

public class NicknameUpdate : MonoBehaviour
{
    [SerializeField]
    private InputField nicknameInputField;
    [SerializeField]
    private Text errorText;

    void Start()
    {
        nicknameInputField.ActivateInputField();
    }

    public void EnterCheck()
    {
        if (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter))
        {
            AcceptButton();
        }
    }

    public void AcceptButton()
    {
        if (nicknameInputField.text.Contains("BGA") || nicknameInputField.text.Contains("bga"))
        {
            errorText.text = "BGA라는 단어는 직원용 계정에 한정되므로 포함될 수 없습니다.";
            nicknameInputField.ActivateInputField();
            return;
        }

        var request = new UpdateUserTitleDisplayNameRequest { DisplayName = nicknameInputField.text };
        PlayFabClientAPI.UpdateUserTitleDisplayName(request, DisplayNameUpdateSuccess, DisplayNameUpdateFailure);
    }

    private void DisplayNameUpdateSuccess(UpdateUserTitleDisplayNameResult result2)
    {
        PlayerPrefs.SetString("Nickname", nicknameInputField.text);
        SceneManager.LoadScene("Lobby");
    }

    private void DisplayNameUpdateFailure(PlayFabError error)
    {
        print(error.GenerateErrorReport());

        if (error.ErrorMessage == "Invalid input parameters")
            errorText.text = "소환사 이름을 다시 확인해주세요.";
        else if (error.ErrorMessage == "Name not available")
            errorText.text = "소환사 이름이 중복되었습니다.";

        nicknameInputField.ActivateInputField();
    }
}
