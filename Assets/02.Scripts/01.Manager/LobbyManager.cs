using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    public NetworkManager networkManager;

    [SerializeField] private Button createRoomBtn, joinRoomBtn;
    [SerializeField] private TMP_InputField roomNameInput;

    public Transform sessionListContent;

    private void Start()
    {
        createRoomBtn.onClick.AddListener(() => { networkManager.StartGame(GameMode.Host,roomNameInput.text); });
        joinRoomBtn.onClick.AddListener(() => { networkManager.StartGame(GameMode.Client, roomNameInput.text); });
    
        networkManager.sessionListContent = sessionListContent;
    }

 
}
