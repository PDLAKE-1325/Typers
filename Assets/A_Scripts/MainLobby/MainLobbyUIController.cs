using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Unity.Mathematics;
using UnityEngine.EventSystems;

public class MainLobbyUIController : MonoBehaviour
{
    [Header("사운드")]
    [SerializeField] AudioClip bgm;

    [Header("오브젝트")]
    [SerializeField] GameObject MidUIBase;
    [SerializeField] GameObject[] MidUI;

    [Header("텍스트 컴포넌트")]
    [SerializeField] Text username_text;
    [SerializeField] Text room_mode_text;
    [SerializeField] Text room_maxPlayer_text;
    [SerializeField] Text room_isVisible_text;

    [Header("애니메이터")]
    [SerializeField] Animator transitionAnimator;

    [Header("파라미터")]
    [SerializeField] float speed_transition;
    [SerializeField] string username;

    Vector3 targetPosTransition = new Vector3(-2000, 0, 0);

    #region Unity Method
    private void Awake()
    {
        try
        {
            SoundManager.Instance.PlayBGM(bgm);
            SoundManager.Instance.OnSceneLoaded();
        }
        catch
        {
            SceneManager.LoadScene("Login");
        }
    }

    private void Start()
    {
        transitionAnimator.SetTrigger("out");
        username = PlayerPrefs.GetString("USERNAME");
        username_text.text = username;
        MidUIBase.SetActive(false);
        for (int i = 0; i < MidUI.Length; i++)
        {
            MidUI[i].SetActive(false);
        }
    }
    #endregion
    #region Sound
    public void PlaySfx(AudioClip clip)
    {
        SoundManager.Instance.PlaySFX(clip);
    }
    #endregion
    #region UIButtonPointerEnterEvents
    readonly Color buttonEnterColor = new Color(0.3f, 0.3f, 0.3f, 0.8f);
    readonly Color buttonExitColor = new Color(0, 0, 0, 0.9f);
    readonly Color buttonExitInvisibleColor = new Color(0, 0, 0, 0);
    readonly Color arrowEnterColor = Color.white;
    readonly Color arrowExitColor = new Color(0.5f, 0.5f, 0.5f);
    public void OnArrowPointerEnter(Text text)
    {
        text.color = arrowEnterColor;
    }
    public void OnArrowPointerExit(Text text)
    {
        text.color = arrowExitColor;
    }
    public void OnButtonPointerEnter(Image img)
    {
        img.color = buttonEnterColor;
    }
    public void OnButtonPointerExit(Image img)
    {
        img.color = buttonExitColor;
    }
    public void OnButtonPointerExitInvisible(Image img)
    {
        img.color = buttonExitInvisibleColor;
    }
    #endregion
    #region MenuUIActiveControl
    public void MidUIOpen(int num)
    {
        for (int i = 0; i < MidUI.Length; i++)
        {
            if (i == num)
            {
                bool isActive = MidUI[i].activeSelf;
                MidUI[i].SetActive(!isActive);
                MidUIBase.SetActive(!isActive);
            }
            else
            {
                MidUI[i].SetActive(false);
            }
        }
    }
    #endregion
    #region Public Methods
    public void OnRoomOptionChanged(string mode, int? max_player, bool? is_private)
    {
        if (mode != null)
            room_mode_text.text = mode;
        else if (max_player != null)
            room_maxPlayer_text.text = max_player.ToString();
        else if (is_private != null)
            room_isVisible_text.text = (bool)is_private ? "True" : "False";
    }
    #endregion
}
