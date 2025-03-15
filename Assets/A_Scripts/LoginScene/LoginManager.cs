using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections;
using System.Collections.Generic;


public class LoginManager : MonoBehaviour
{
    public string titleId = "87348";
    public bool LoggedIn = false;
    [SerializeField] string username;
    [SerializeField] InputField username_text;
    [SerializeField] LoginUIController uiController;

    #region Unity Methods
    void Start()
    {
        if (string.IsNullOrEmpty(PlayFabSettings.TitleId))
        {
            PlayFabSettings.TitleId = titleId;
        }
    }
    private void Update()
    {
        username = username_text.text;
    }
    #endregion
    #region Private Methods
    void LoginWithCustomId()
    {
        var request = new LoginWithCustomIDRequest { CustomId = username, CreateAccount = true };
        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnFailure);
    }
    void UpdateDisplayName(string displayname)
    {
        var request = new UpdateUserTitleDisplayNameRequest { DisplayName = displayname };
        PlayFabClientAPI.UpdateUserTitleDisplayName(request, result => Debug.Log("DisplayName변경완료"), OnFailure);
    }
    void OnLoginSuccess(LoginResult result)
    {
        UpdateDisplayName(username);
        Debug.Log($"로그인 완료 [{username}]");
        PlayerPrefs.SetString("USERNAME", username);
        CheckIfAlreadyLoggedIn(result.PlayFabId);
    }
    void CheckIfAlreadyLoggedIn(string playFabId)
    {
        var request = new GetUserDataRequest
        {
            PlayFabId = playFabId,
            Keys = new List<string> { "IsLoggedIn" }
        };

        PlayFabClientAPI.GetUserData(request,
            result =>
            {
                if (result.Data != null && result.Data.ContainsKey("IsLoggedIn") && result.Data["IsLoggedIn"].Value == "true")
                {
                    Debug.Log($"데이터 : {result.Data["IsLoggedIn"].Value}");
                    Debug.LogWarning("이미 로그인된 계정입니다. 새 로그인을 차단합니다.");
                    uiController.OpenErrorText("잠시 후 시도하거나 다른 이름으로 설정해주세요.");
                    PlayFabClientAPI.ForgetAllCredentials(); // 기존 세션 무효화
                }
                else
                {
                    Debug.Log("데이터 : " + result.Data != null && result.Data.ContainsKey("IsLoggedIn") ? result.Data["IsLoggedIn"].Value : "null");
                    Debug.Log("새로운 로그인 허용.");
                    UpdateLoginStatus();
                    uiController.EnterGame();
                }
            },
            error =>
            {
                Debug.LogError("로그인 상태 확인 실패: " + error.GenerateErrorReport());
            });
    }
    void UpdateLoginStatus()
    {
        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                { "IsLoggedIn", "true"}
            }
        };

        PlayFabClientAPI.UpdateUserData(request,
            result => Debug.Log("로그인 상태 업데이트 완료."),
            error => Debug.LogError("로그인 상태 업데이트 실패: " + error.GenerateErrorReport()));
    }
    void OnFailure(PlayFabError error)
    {
        Debug.Log($"에러 발생 잠시 후 다시 실행바람 : {error.GenerateErrorReport()}");
        if (error.GenerateErrorReport() != "/Client/UpdateUserTitleDisplayName: Name not available")
            uiController.OpenErrorText("잠시 후 시도하거나 다른 이름으로 설정해주세요.");
    }
    bool IsValidUsername()
    {
        bool error = false;
        if (username.Length >= 3 && username.Length <= 24)
            error = true;
        return error;
    }
    void UnValidUsername()
    {
        uiController.OpenErrorText("3자 이상 24자 이하로 설정해주세요.");
    }
    #endregion
    #region Public Methods
    public void Enter()
    {
        if (IsValidUsername())
        {
            LoginWithCustomId();
        }
        else
        {
            UnValidUsername();
        }
    }
    #endregion
}
