using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class RoleSelectUI : MonoBehaviour
{
    // UI Ŀ�� �̹���
    public RectTransform[] cursorImage;

    // �� ������ ��ġ��
    public RectTransform[] rolePositions;

    // ���� ���õ� ���� �ε���
    private int currentRoleIndex_1P = 1;
    private int currentRoleIndex_2P = 1;

    // �̵� �ӵ� ���� (�ε巴�� ������ �� ����)
    public float moveSpeed = 10f;

    void Start()
    {
        // Ŀ���� ó�� ��ġ�� �̵�
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

        // Ŀ�� �̵� �ִϸ��̼� (�ε巴�� �̵�)
        cursorImage[0].position = new Vector3(Mathf.Lerp(cursorImage[0].position.x, rolePositions[currentRoleIndex_1P].position.x, Time.deltaTime * moveSpeed), cursorImage[0].position.y);
        cursorImage[1].position = new Vector3(Mathf.Lerp(cursorImage[1].position.x, rolePositions[currentRoleIndex_2P].position.x, Time.deltaTime * moveSpeed), cursorImage[1].position.y);
    }

    // �������� �̵� (A �Ǵ� ����Ű ��)
    void ContorlCursor_1P()
    {
        if(!LocalGameManager.instance.GetIsPlayerReady(0)) // �غ� ������ ���� ���� X
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

    // ���������� �̵� (D �Ǵ� ����Ű ��)
    void ContorlCursor_2P()
    {
        if (!LocalGameManager.instance.GetIsPlayerReady(1)) // �غ� ������ ���� ���� X
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

    // Ŀ���� ������ ��ġ�� �̵�
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

    // ���õ� ������ �������� �Լ�
    //public string GetSelectedRole()
    //{
    //    // ����: ���� �̸��� ��ȯ
    //    string[] roles = { "Tank", "DPS", "Healer" };
    //    return roles[currentRoleIndex];
    //}

    void ConfirmRole()
    {
        LocalGameManager.instance.SetReady(0);
        LocalGameManager.instance.SetReady(1);

        //string selectedRole = GetSelectedRole();
        //Debug.Log($"Player selected: {selectedRole}");
        // ���� ���õ� ���ҿ� ���� ���� ���� ó��
    }
}
