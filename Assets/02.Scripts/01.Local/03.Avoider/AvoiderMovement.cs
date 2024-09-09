using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvoiderMovement : MonoBehaviour
{

    Vector3 movement;

    private Camera mainCamera;
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector3 HandleMovement(Vector2 direction, float moveSpeed)
    {
        // ī�޶��� forward, right ���͸� ������
        Vector3 cameraForward = new Vector3(mainCamera.transform.forward.x, 0.0f, mainCamera.transform.forward.z).normalized;
        Vector3 cameraRight = new Vector3(mainCamera.transform.right.x, 0.0f, mainCamera.transform.right.z).normalized;

        // �Էµ� ������ ī�޶��� �¿�/�յ� �������� ��ȯ
        float moveHorizontal = direction.x;
        float moveVertical = direction.y;

        // ī�޶��� forward�� right�� �������� ������ ���� ����
        movement = cameraRight * moveHorizontal + cameraForward * moveVertical;

        return movement * moveSpeed; // �̵� �ӵ� ����
    }

}
