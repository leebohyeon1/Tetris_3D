using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RefreshBtn : MonoBehaviour
{
    private NetworkManager networkManager;
    private Button refreshButton;

    private void Awake()
    {
        if (refreshButton == null)
        {
            refreshButton = GetComponent<Button>();
        }

        refreshButton.onClick.AddListener(() => {Refresh();});
    }
    private void Start()
    {
        networkManager = FindFirstObjectByType<NetworkManager>();
    }

    private void Refresh()
    {
        StartCoroutine(RefreshWait());
    }

    private IEnumerator RefreshWait()
    {
        refreshButton.interactable = false;
        networkManager.RefreshSessionListUI();
        yield return new WaitForSeconds(3f);
        refreshButton.interactable = true;
    }
}
