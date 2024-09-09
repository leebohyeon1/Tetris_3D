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
        // 카메라의 forward, right 벡터를 가져옴
        Vector3 cameraForward = new Vector3(mainCamera.transform.forward.x, 0.0f, mainCamera.transform.forward.z).normalized;
        Vector3 cameraRight = new Vector3(mainCamera.transform.right.x, 0.0f, mainCamera.transform.right.z).normalized;

        // 입력된 방향을 카메라의 좌우/앞뒤 기준으로 변환
        float moveHorizontal = direction.x;
        float moveVertical = direction.y;

        // 카메라의 forward와 right를 기준으로 움직임 방향 설정
        movement = cameraRight * moveHorizontal + cameraForward * moveVertical;

        return movement * moveSpeed; // 이동 속도 설정
    }

}
