using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SessionEntryPrefab : MonoBehaviour
{
    private NetworkManager networkManager;

    public TextMeshProUGUI sesisonName;
    public TextMeshProUGUI playerCount;
    public Button joinButton;

    private void Awake()
    {
        joinButton.onClick.AddListener(() => { JoinSession(); });
    }

    private void Start()
    {
        networkManager = FindFirstObjectByType<NetworkManager>();
    }

    private void JoinSession()
    {
        
        networkManager.StartGame(Fusion.GameMode.Client,sesisonName.text);
    }
}
