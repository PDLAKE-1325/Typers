using UnityEngine;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections;

public class AccountManager : MonoBehaviour
{
    private bool isLoggingOut = false; // 로그아웃 진행 여부 플래그
    private bool logoutComplete = false; // 로그아웃 완료 여부 플래그

    private void Start()
    {
        Application.wantsToQuit += OnWantsToQuit;
    }

    #region LogOut
    bool OnWantsToQuit()
    {
        Debug.Log("[!] 애플리케이션 종료 요청 감지");
        TriggerLogout();

        // 로그아웃 완료 시 종료 허용
        return logoutComplete;
    }

    private void OnApplicationPause(bool isPaused)
    {
        if (isPaused)
        {
            Debug.Log("[!] 애플리케이션 백그라운드 감지");
            TriggerLogout();
        }
    }

    private void OnApplicationQuit()
    {
        Debug.Log("[!] 애플리케이션 종료 감지");
        TriggerLogout();
    }

    private void TriggerLogout()
    {
        if (isLoggingOut || logoutComplete) return;

        isLoggingOut = true;
        Debug.Log("[*] 로그아웃 시작");

        LogOut();
    }

    private void LogOut()
    {
        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                { "IsLoggedIn", "false" }
            }
        };

        try
        {
            // PlayFab API 호출
            PlayFabClientAPI.UpdateUserData(request,
                result =>
                {
                    Debug.Log("[+] 로그아웃 상태 업데이트 성공");
                    CompleteLogout();
                },
                error =>
                {
                    Debug.LogError("[!] 로그아웃 상태 업데이트 실패: " + error.GenerateErrorReport());
                    CompleteLogout();
                });
        }
        catch
        {
            print("[!] 플레이팹에 로그인되어있지 않음.");
        }
        // 타임아웃 처리 시작
        StartCoroutine(LogoutTimeout(5f));
    }

    private IEnumerator LogoutTimeout(float timeout)
    {
        yield return new WaitForSeconds(timeout);

        if (isLoggingOut) // 여전히 로그아웃 진행 중이라면
        {
            Debug.LogWarning("[!] 로그아웃 타임아웃 발생. 강제 종료");
            CompleteLogout();
        }
    }

    private void CompleteLogout()
    {
        if (logoutComplete) return;

        try
        {
            PlayFabClientAPI.ForgetAllCredentials();
            Debug.Log("[*] 로그아웃 완료");
        }
        catch
        {
            print("[!] 플레이팹에 로그인되어있지 않아 로그아웃 실패.");
        }

        isLoggingOut = false;
        logoutComplete = true;

        // 애플리케이션 종료 허용
        Application.Quit();
    }
    #endregion
}
