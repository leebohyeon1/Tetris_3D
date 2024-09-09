using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform boardTransform;  // 보드의 중심 Transform
    public float rotationSpeed = 20f;  // 카메라 회전 속도
    public float distanceFromBoard = 10f;  // 보드로부터의 거리
    public float heightOffset = 5f;  // 카메라가 보드보다 위로 얼마나 떨어져 있는지

    private float currentAngle = 0f;  // 카메라가 회전하는 각도

    private int count = 0;
    private bool isRight = false;

    void Update()
    {
        RotateCameraAroundBoard();  // 보드를 중심으로 카메라 회전
    }

    // 카메라가 보드를 중심으로 일정한 속도로 회전하는 함수
    void RotateCameraAroundBoard()
    {
        if(!isRight)
        {
            // 각도 갱신 (y축 기준으로 회전)
            currentAngle += rotationSpeed * Time.deltaTime;
            if (currentAngle >= 360f)
            {
                currentAngle -= 360f;  // 각도가 360도 넘으면 0도로 리셋
                count++;
            }
        }
        else
        {
            // 각도 갱신 (y축 기준으로 회전)
            currentAngle -= rotationSpeed * Time.deltaTime;
            if (currentAngle <= 0)
            {
                currentAngle += 360f;  // 각도가 360도 넘으면 0도로 리셋
                count++;
            }

        }

        //if(count > 1)
        //{
        //    isRight = !isRight;
        //    count = 0;
        //}
        // 회전할 위치 계산 (보드 중심을 기준으로 원형 궤도)
        Vector3 offset = new Vector3(Mathf.Sin(currentAngle * Mathf.Deg2Rad), 0, Mathf.Cos(currentAngle * Mathf.Deg2Rad)) * distanceFromBoard;

        // 카메라의 위치를 보드 중심 기준으로 설정
        transform.position = boardTransform.position + offset + new Vector3(0, heightOffset, 0);

        // 카메라가 항상 보드를 바라보도록 설정
        transform.LookAt(boardTransform);
    }
}
