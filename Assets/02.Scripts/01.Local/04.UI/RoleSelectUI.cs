using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class RoleSelectUI : MonoBehaviour
{
    // UI 커서 이미지
    public RectTransform[] cursorImage;

    // 각 역할의 위치들
    public RectTransform[] rolePositions;

    // 현재 선택된 역할 인덱스
    private int currentRoleIndex_1P = 1;
    private int currentRoleIndex_2P = 1;

    // 이동 속도 설정 (부드럽게 움직일 수 있음)
    public float moveSpeed = 10f;

    void Start()
    {
        // 커서를 처음 위치로 이동
        MoveCursorToPosition(1,currentRoleIndex_1P);
        MoveCursorToPosition(2,currentRoleIndex_2P);
    }

    void Update()
    {
        ContorlCursor_1P();
        ContorlCursor_2P();
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            ConfirmRole();
        }

        // 커서 이동 애니메이션 (부드럽게 이동)
        cursorImage[0].position = new Vector3(Mathf.Lerp(cursorImage[0].position.x, rolePositions[currentRoleIndex_1P].position.x, Time.deltaTime * moveSpeed), cursorImage[0].position.y);
        cursorImage[1].position = new Vector3(Mathf.Lerp(cursorImage[1].position.x, rolePositions[currentRoleIndex_2P].position.x, Time.deltaTime * moveSpeed), cursorImage[1].position.y);
    }

    // 왼쪽으로 이동 (A 또는 방향키 좌)
    void ContorlCursor_1P()
    {
        if(!LocalGameManager.instance.GetIsPlayerReady(0)) // 준비 상태일 때는 조작 X
        {
            if (LocalPlayerInputManager.instance.moveInput_1P.x == -1)
            {
                if (currentRoleIndex_1P > 0)
                {
                    currentRoleIndex_1P--;
                }
            }
            else if (LocalPlayerInputManager.instance.moveInput_1P.x == 1)
            {
                if (currentRoleIndex_1P < rolePositions.Length - 1)
                {
                    currentRoleIndex_1P++;
                }
            }
        }
    }

    // 오른쪽으로 이동 (D 또는 방향키 우)
    void ContorlCursor_2P()
    {
        if (!LocalGameManager.instance.GetIsPlayerReady(1)) // 준비 상태일 때는 조작 X
        {
            if (LocalPlayerInputManager.instance.moveInput_2P.x == -1)
            {
                if (currentRoleIndex_2P > 0)
                {
                    currentRoleIndex_2P--;
                }
            }
            else if (LocalPlayerInputManager.instance.moveInput_2P.x == 1)
            {
                if (currentRoleIndex_2P < rolePositions.Length - 1)
                {
                    currentRoleIndex_2P++;
                }
            }
        }
    }

    // 커서를 지정된 위치로 이동
    void MoveCursorToPosition(int playerIndex,int index)
    {
        if (playerIndex == 1)
        {
            cursorImage[0].position = new Vector3(rolePositions[index].position.x, cursorImage[0].position.y);
        }
        else if (playerIndex == 2)
        {
            cursorImage[1].position = new Vector3(rolePositions[index].position.x, cursorImage[1].position.y);
        }
    }

    // 선택된 역할을 가져오는 함수
    //public string GetSelectedRole()
    //{
    //    // 예시: 역할 이름을 반환
    //    string[] roles = { "Tank", "DPS", "Healer" };
    //    return roles[currentRoleIndex];
    //}

    void ConfirmRole()
    {
        LocalGameManager.instance.SetReady(0);
        LocalGameManager.instance.SetReady(1);

        //string selectedRole = GetSelectedRole();
        //Debug.Log($"Player selected: {selectedRole}");
        // 이후 선택된 역할에 따라 게임 로직 처리
    }
}
