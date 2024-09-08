using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class NetworkManager : MonoBehaviour, INetworkRunnerCallbacks
{
    private NetworkRunner _runner;

    [SerializeField] private NetworkPrefabRef _playerPrefab;
    private Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = new Dictionary<PlayerRef, NetworkObject>();

    [Header("Session List")]
    private List<SessionInfo> _sessions = new List<SessionInfo>(); // 현재 세션들
    [HideInInspector] public Transform sessionListContent; 
    public GameObject sessionEntryPrefab; // 세션 프리팹

    [Header("Loading")]
    public GameObject LoadingUI; // 로딩 UI
    
    [Header("Connect")]
    public GameObject ConnectUI; // 로딩 UI



    // 재시도 관련 변수들
    private int maxRetryCount = 5; // 최대 재시도 횟수
    private int currentRetryCount = 0; // 현재 재시도 횟수
    private float retryDelay = 3f; // 재시도 사이의 지연 시간 (초)

    private void Start()
    {
        if (_runner == null)
        {
            _runner = gameObject.AddComponent<NetworkRunner>();
 
        }

        StartCoroutine(TryJoinLobby());  // 첫 번째 로비 접속 시도
    }
    public void OnConnectedToServer(NetworkRunner runner)
    {
        
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        var data = new NetworkInputData();

        if (Input.GetKey(KeyCode.W))
            data.direction += Vector3.forward;

        if (Input.GetKey(KeyCode.S))
            data.direction += Vector3.back;

        if (Input.GetKey(KeyCode.A))
            data.direction += Vector3.left;

        if (Input.GetKey(KeyCode.D))
            data.direction += Vector3.right;

        input.Set(data);
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
    }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer)
        {
            // 예시: 플레이어 수에 따라 역할을 할당
            PlayerRole role = (player.RawEncoded % 2 == 0) ? PlayerRole.Tetris : PlayerRole.Avoider;

            // Create a unique position for the player
            Vector3 spawnPosition = new Vector3((player.RawEncoded % runner.Config.Simulation.PlayerCount) * 3, 1, 0);
            NetworkObject networkPlayerObject = runner.Spawn(_playerPrefab, spawnPosition, Quaternion.identity, player);
            // Keep track of the player avatars for easy access
            _spawnedCharacters.Add(player, networkPlayerObject);
        }
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (_spawnedCharacters.TryGetValue(player, out NetworkObject networkObject))
        {
            runner.Despawn(networkObject);
            _spawnedCharacters.Remove(player);
        }
    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        LoadingUI.SetActive(false);
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
        LoadingUI.SetActive(true);
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        _sessions.Clear();
        _sessions = sessionList;

        RefreshSessionListUI();
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
    }

    public async void StartGame(GameMode mode, string sessionName)
    {
        if (_runner == null)
        {
            _runner = gameObject.AddComponent<NetworkRunner>();
        }
        LoadingUI.SetActive(true);
        // Create the Fusion runner and let it know that we will be providing user input
        _runner.ProvideInput = true;

        // Create the NetworkSceneInfo from the current scene
        var scene = SceneRef.FromIndex(3);
        var sceneInfo = new NetworkSceneInfo();
        if (scene.IsValid)
        {
            sceneInfo.AddSceneRef(scene, LoadSceneMode.Additive);
        }

        string roomName = sessionName;
        // 방 이름이 입력되지 않았으면 랜덤한 이름으로 설정
        if (string.IsNullOrEmpty(roomName))
        {
            roomName = GenerateRandomRoomName();
        }

        // Start or join (depends on gamemode) a session with a specific name
        await _runner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = roomName,
            Scene = scene,
            PlayerCount = 2,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });
    }


    // 랜덤 방 이름 생성 메소드
    private string GenerateRandomRoomName()
    {
        // 시간 기반의 랜덤 이름 생성 예시 (고유 이름 보장)
        return "Room_" + System.Guid.NewGuid().ToString("N").Substring(0, 8);
    }

    // 선택한 세션에 입장하는 메소드
    public async void JoinSession(string sessionName)
    {
        if (_runner != null)
        {
            await _runner.StartGame(new StartGameArgs()
            {
                GameMode = GameMode.Client,
                SessionName = sessionName,
                SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
            });
        }
    }

    public void RefreshSessionListUI()
    {
        // clear out Session List UI so we don't create duplicates
        foreach (Transform child in sessionListContent)
        {
            Destroy(child.gameObject);
        }

        foreach (SessionInfo session in _sessions)
        {
            if (session.IsVisible)
            {
                GameObject entry = GameObject.Instantiate(sessionEntryPrefab, sessionListContent);
                SessionEntryPrefab script = entry.GetComponent<SessionEntryPrefab>();
                script.sesisonName.text = session.Name;
                script.playerCount.text = session.PlayerCount + "/" + session.MaxPlayers;

                if (session.IsOpen == false || session.PlayerCount >= session.MaxPlayers)
                {
                    script.joinButton.interactable = false;
                }
                else
                {
                    script.joinButton.interactable = true;
                }
            }
        }
    }

    // 접속을 시도하는 코루틴
    private IEnumerator TryJoinLobby()
    {
        while (currentRetryCount < maxRetryCount)
        {
            Debug.Log("Trying to join lobby... Attempt " + (currentRetryCount + 1));

            var task = _runner.JoinSessionLobby(SessionLobby.Shared);
            yield return new WaitUntil(() => task.IsCompleted);  // JoinSessionLobby가 완료될 때까지 대기

            if (task.IsCompleted && !task.IsFaulted) // 성공한 경우
            {
                Debug.Log("Successfully joined lobby.");
                currentRetryCount = 0;  // 재시도 카운트 리셋
                ConnectUI.SetActive(false);
                yield break; // 성공했으므로 반복문 종료
            }
            else // 실패한 경우
            {
                Debug.LogWarning("Failed to join lobby. Retrying in " + retryDelay + " seconds...");
                currentRetryCount++;
                yield return new WaitForSeconds(retryDelay); // 지연 시간 후 재시도
            }
        }

        ConnectUI.transform.GetChild(0).GetComponent<TMP_Text>().text = "Connect Failed";
        // 최대 재시도 횟수를 초과한 경우 실패 처리
        Debug.LogError("Max retries reached. Failed to join lobby.");
        // 여기서 추가적인 실패 처리 로직을 넣을 수 있습니다 (예: UI 경고 표시)
    }

}