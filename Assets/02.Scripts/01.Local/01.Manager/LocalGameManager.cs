using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalGameManager : MonoBehaviour
{
    public static LocalGameManager instance;

    #region 캐릭터 선택

    [SerializeField] private GameObject RoleSelectionUI;

    //플레이어 준비 상태
    private bool []isPlayeReady = new bool[2];

    #endregion

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            DestroyImmediate(gameObject);
        }
    }

    public void SetReady(int playerIndex)
    {
        isPlayeReady[playerIndex] = !isPlayeReady[playerIndex];

        foreach (bool isReady in isPlayeReady)
        {
            if (!isReady)
            {
                return;
            }
        }

        GameStart();
    }

    public bool GetIsPlayerReady(int playerIndex)
    {
        return isPlayeReady[playerIndex];   
    }

    public void GameStart()
    {
        RoleSelectionUI.SetActive(false);
    }
}
