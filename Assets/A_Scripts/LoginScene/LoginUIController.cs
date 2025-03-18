using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Unity.Mathematics;

public class LoginUIController : MonoBehaviour
{
    [Header("사운드")]
    [SerializeField] AudioClip bgm;
    [SerializeField] AudioClip enter;

    [Header("오브젝트")]
    [SerializeField] Text errorText;

    [Header("애니메이터")]
    [SerializeField] Animator animator;
    [SerializeField] Animator transitionAnimator;

    [Header("파라미터")]
    [SerializeField] float speed_transition;

    bool errorTextOpend = false;
    bool loginOpened = false;

    private void Start()
    {
        SoundManager.Instance.PlayBGM(bgm);
        animator.enabled = false;
    }
    private void Update()
    {
        if (!loginOpened && Input.GetMouseButtonUp(0))
        {
            loginOpened = true;
            animator.enabled = true;
            animator.SetTrigger("open");
            SoundManager.Instance.PlaySFX(enter);
        }
    }
    public void OpenErrorText(string text)
    {
        if (errorTextOpend) return;
        errorTextOpend = true;
        errorText.gameObject.SetActive(true);
        errorText.text = text;
        Invoke("CloseErrorText", 1.5f);
    }
    void CloseErrorText()
    {
        errorTextOpend = false;
        errorText.gameObject.SetActive(false);
    }
    public void EnterGame()
    {
        Debug.Log("EnterGame");
        transitionAnimator.SetTrigger("in");
    }
}
