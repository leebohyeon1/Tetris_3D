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
    private List<SessionInfo> _sessions = new List<SessionInfo>(); // ���� ���ǵ�
    [HideInInspector] public Transform sessionListContent; 
    public GameObject sessionEntryPrefab; // ���� ������

    [Header("Loading")]
    public GameObject LoadingUI; // �ε� UI
    
    [Header("Connect")]
    public GameObject ConnectUI; // �ε� UI



    // ��õ� ���� ������
    private int maxRetryCount = 5; // �ִ� ��õ� Ƚ��
    private int currentRetryCount = 0; // ���� ��õ� Ƚ��
    private float retryDelay = 3f; // ��õ� ������ ���� �ð� (��)

    private void Start()
    {
        if (_runner == null)
        {
            _runner = gameObject.AddComponent<NetworkRunner>();
 
        }

        StartCoroutine(TryJoinLobby());  // ù ��° �κ� ���� �õ�
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
            // ����: �÷��̾� ���� ���� ������ �Ҵ�
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
        // �� �̸��� �Էµ��� �ʾ����� ������ �̸����� ����
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


    // ���� �� �̸� ���� �޼ҵ�
    private string GenerateRandomRoomName()
    {
        // �ð� ����� ���� �̸� ���� ���� (���� �̸� ����)
        return "Room_" + System.Guid.NewGuid().ToString("N").Substring(0, 8);
    }

    // ������ ���ǿ� �����ϴ� �޼ҵ�
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

    // ������ �õ��ϴ� �ڷ�ƾ
    private IEnumerator TryJoinLobby()
    {
        while (currentRetryCount < maxRetryCount)
        {
            Debug.Log("Trying to join lobby... Attempt " + (currentRetryCount + 1));

            var task = _runner.JoinSessionLobby(SessionLobby.Shared);
            yield return new WaitUntil(() => task.IsCompleted);  // JoinSessionLobby�� �Ϸ�� ������ ���

            if (task.IsCompleted && !task.IsFaulted) // ������ ���
            {
                Debug.Log("Successfully joined lobby.");
                currentRetryCount = 0;  // ��õ� ī��Ʈ ����
                ConnectUI.SetActive(false);
                yield break; // ���������Ƿ� �ݺ��� ����
            }
            else // ������ ���
            {
                Debug.LogWarning("Failed to join lobby. Retrying in " + retryDelay + " seconds...");
                currentRetryCount++;
                yield return new WaitForSeconds(retryDelay); // ���� �ð� �� ��õ�
            }
        }

        ConnectUI.transform.GetChild(0).GetComponent<TMP_Text>().text = "Connect Failed";
        // �ִ� ��õ� Ƚ���� �ʰ��� ��� ���� ó��
        Debug.LogError("Max retries reached. Failed to join lobby.");
        // ���⼭ �߰����� ���� ó�� ������ ���� �� �ֽ��ϴ� (��: UI ��� ǥ��)
    }

}