using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.SceneManagement;

public class LogInManager : MonoBehaviour
{
    [SerializeField]
    private Toggle videoPlayOnOffTogle = null;
    [SerializeField]
    private Toggle soundOnOffToggle = null;
    [SerializeField]
    private VideoPlayer videoPlayer = null;
    [SerializeField]
    private AudioSource loginBGM = null;

    [SerializeField]
    private InputField inputID;
    [SerializeField]
    private InputField inputPass;
    [SerializeField]
    private Text errorText;

    [SerializeField]
    private GameObject registerWindow;
    [SerializeField]
    private InputField registerInputID;
    [SerializeField]
    private InputField registerInputPass;

    private string username;
    private string password;

    private void Awake()
    {
        PlayFabSettings.TitleId = "5C01";
        videoPlayer.Play();

        Screen.SetResolution(1280, 720, false);
    }

    public void IdValueChanged()
    {
        username = inputID.text.ToString();

        if (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter))
        {
            Login();
        }
    }

    public void PasswordValueChanged()
    {
        password = inputPass.text.ToString();

        if (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter))
        {
            Login();
        }
    }

    public void RegisterEnterCheck()
    {
        if (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter))
        {
            Register();
        }
    }

    public void Login()
    {
        var request = new LoginWithPlayFabRequest { Username = username, Password = password };
        PlayFabClientAPI.LoginWithPlayFab(request, OnLoginSuccess, OnLoginFailure);
        errorText.text = "<color=#ffffff>로그인 중...</color>";
        SoundManager.Instance.Button_Login_Sound();

        if (SoundManager.Instance != null)
            SoundManager.Instance.Button_Login_Sound();
    }

    private void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("로그인 성공");
        PlayerPrefs.SetString("Username", username);

        // 계정정보 받아옴
        var request = new GetAccountInfoRequest { Username = username };
        PlayFabClientAPI.GetAccountInfo(request, GetAccountSuccess, GetAccountFailure);
    }

    private void OnLoginFailure(PlayFabError error)
    {
        Debug.LogWarning("로그인 실패");
        Debug.LogWarning(error.GenerateErrorReport());
        errorText.text = "계정이름과 비밀번호가 일치하지 않습니다. 다시 시도해 주세요.";
    }

    private void GetAccountSuccess(GetAccountInfoResult result)
    {
        print("Accout를 정상적으로 받아옴");

        string nickname = result.AccountInfo.TitleInfo.DisplayName;
        if (nickname == null)
        {
            SceneManager.LoadScene("NicknameSet");
        }
        else
        {
            PlayerPrefs.SetString("Nickname", nickname);
            SceneManager.LoadScene("Lobby");
        }
    }

    private void GetAccountFailure(PlayFabError error)
    {
        print("Accout를 받아오지 못함");
        print(error.GenerateErrorReport());
        errorText.text = "계정정보를 받아오지 못했습니다.";
    }

    public void RegisterButton()
    {
        registerWindow.SetActive(true);
        PlayButtonClickSound();
    }

    public void Register()
    {
        var request = new RegisterPlayFabUserRequest { Username = registerInputID.text, Password = registerInputPass.text, RequireBothUsernameAndEmail = false };
        PlayFabClientAPI.RegisterPlayFabUser(request, RegisterSuccess, RegisterFailure);

        PlayButtonClickSound();
    }

    public void RegisterWindowClose()
    {
        registerInputID.text = "";
        registerInputPass.text = "";
        registerWindow.SetActive(false);

        PlayButtonClickSound();
    }

    private void RegisterSuccess(RegisterPlayFabUserResult result)
    {
        Debug.Log("가입 성공");
        errorText.text = "계정 생성에 성공했습니다.";
        RegisterWindowClose();
    }

    private void RegisterFailure(PlayFabError error)
    {
        Debug.LogWarning("가입 실패");
        Debug.LogWarning(error.GenerateErrorReport());
        errorText.text = "계정 생성에 실패했습니다. 계정이름은 3자리, 비밀번호는 6자리 이상으로 설정해주세요.";
    }

    public void VideoToggle()
    {
        if (videoPlayOnOffTogle.isOn)
        {
            videoPlayer.playbackSpeed = 0;
        }
        else
        {
            videoPlayer.playbackSpeed = 1;
        }

        PlayUIButtonClickSound();
    }

    public void AudioToggle()
    {
        if (soundOnOffToggle.isOn)
        {
            loginBGM.mute = true;
        }
        else
        {
            loginBGM.mute = false;
        }

        PlayUIButtonClickSound();
    }

    private void PlayButtonClickSound()
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.Button_Click_Sound();
    }

    private void PlayUIButtonClickSound()
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.Button_UI_Sound();
    }
}
