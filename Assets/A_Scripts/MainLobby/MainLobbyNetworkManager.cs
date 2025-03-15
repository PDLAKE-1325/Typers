using UnityEngine;
using UnityEngine.UI;
using Unity.Mathematics;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System.Text.RegularExpressions;
using Photon.Pun.Demo.Cockpit.Forms;
using System.Linq;

public class MainLobbyNetworkManager : MonoBehaviourPunCallbacks
{
    [SerializeField] string mode;
    [SerializeField] int room_maxPlayer;
    [SerializeField] bool room_isPrivate;
    [SerializeField] string username;
    public Action<string, int?, bool?> room_option_changed;

    [Header("요소들")]
    [SerializeField] Text modeText;
    [SerializeField] Transform container;
    [SerializeField] GameObject roomObject;
    string[] modes = { "Speed", "Accuracy", "Solo" };
    List<RoomInfo> pre_roomlist = null;

    #region Unity Methods
    private void Start()
    {
        username = PlayerPrefs.GetString("USERNAME");
        mode = modes[0];
        room_isPrivate = false;
        room_maxPlayer = 10;
        if (PhotonNetwork.IsConnectedAndReady) return;
        ConnectToPhoton();
    }//Speed Mode Accuracy Mode Glitch Mode Solo Mode
    #endregion

    #region Private Methods
    void ConnectToPhoton()
    {
        Debug.Log($"[+] 포톤 접속 : {username}");
        PhotonNetwork.AuthValues = new AuthenticationValues(username);
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.NickName = username;
        PhotonNetwork.ConnectUsingSettings();
    }

    void UpdateRoom(List<RoomInfo> rooms, bool changedmode = false)
    {
        if (pre_roomlist != null && rooms.Count == pre_roomlist.Count && !changedmode)
        {
            bool same = true;
            for (int i = 0; i < rooms.Count; i++)
            {
                if (rooms[i].Name != pre_roomlist[i].Name ||
                    rooms[i].PlayerCount != pre_roomlist[i].PlayerCount)
                {
                    same = false;
                    break;
                }
            }
            if (same) return;
        }

        pre_roomlist = new List<RoomInfo>(rooms);

        foreach (Transform child in container)
        {
            Destroy(child.gameObject);
        }

        Debug.Log($"룸 갯수 : {rooms.Count}");

        foreach (RoomInfo ri in rooms)
        {
            var cp = ri.CustomProperties;

            if (ri.RemovedFromList)
                continue;

            Debug.Log($"방이름 : {ri.Name}");
            if (cp.ContainsKey("MODE") && cp["MODE"].ToString() == mode)
            {
                Debug.Log($"방모드 : {cp["MODE"]}");
                Debug.Log("[+] 프리펩 생성");
                GameObject ro = Instantiate(roomObject, container);
                ro.transform.GetChild(0).GetComponent<Text>().text = ri.Name;
                ro.transform.GetChild(1).GetComponent<Text>().text = $"{ri.PlayerCount}/{ri.MaxPlayers}";
                ro.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(() => JoinRoom(ri.Name));
            }
        }
    }

    void CreateRoom(int maxplayers, bool open, bool visible)
    {
        if (string.IsNullOrEmpty(username)) return;
        if (string.IsNullOrEmpty(mode)) return;

        string roomName = $"{username}";

        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = maxplayers,
            IsOpen = open,
            IsVisible = visible,
            PublishUserId = true,
            CustomRoomProperties = new Hashtable
            {
                { "MODE", mode }
            },
            CustomRoomPropertiesForLobby = new string[] { "MODE" }
        };

        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }

    void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }
    void UpdateMode()
    {
        UpdateRoom(pre_roomlist, true);
    }
    void RoomUIUpdate(string mode = null, int? max_player = null, bool? is_private = null)
    {
        room_option_changed?.Invoke(mode, max_player, is_private);
    }
    #endregion

    #region Public Methods
    public void CreateRoom_()
    {
        CreateRoom(room_maxPlayer, true, room_isPrivate);
    }
    public void ChangeMode(bool is_right)
    {
        int next = is_right ? 1 : -1;
        for (int i = 0; i < modes.Length; i++)
            if (mode == modes[i])
            {
                mode = modes[(i + next) % modes.Length];
                break;
            }
        RoomUIUpdate(mode);
        UpdateMode();
    }
    public void ChangeMaxPlayer(bool is_right)
    {
        int next = room_maxPlayer + (is_right ? 1 : -1);
        if (next == 0) room_maxPlayer = 10;
        else if (next == 11) room_maxPlayer = 2;
        else room_maxPlayer = next;
        RoomUIUpdate(max_player: room_maxPlayer);
    }
    public void ChangeRoomVisible()
    {
        room_isPrivate = !room_isPrivate;
        RoomUIUpdate(is_private: room_isPrivate);
    }
    #endregion
    #region Photon Callbacks
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        UpdateRoom(roomList);
    }
    public override void OnConnectedToMaster()
    {
        Debug.Log($"[+] 마스터 서버 접속 ({PhotonNetwork.CloudRegion})");
        if (!PhotonNetwork.InLobby) PhotonNetwork.JoinLobby(TypedLobby.Default);
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("[+] 로비 접속");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log($"[+] 방 접속 : {PhotonNetwork.CurrentRoom.Name}");
    }

    public override void OnCreatedRoom()
    {
        Debug.Log($"[+] 방 생성 : {PhotonNetwork.CurrentRoom.Name}");
    }

    public override void OnLeftRoom()
    {
        Debug.LogError($"[!] 방 퇴장");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogError($"[!] 방 생성 실패 ({message})");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogError($"[!] 방 접속 실패 ({message})");
    }
    #endregion
}
