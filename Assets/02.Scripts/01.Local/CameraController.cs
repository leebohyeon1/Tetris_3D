using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform boardTransform;  // ������ �߽� Transform
    public float rotationSpeed = 20f;  // ī�޶� ȸ�� �ӵ�
    public float distanceFromBoard = 10f;  // ����κ����� �Ÿ�
    public float heightOffset = 5f;  // ī�޶� ���庸�� ���� �󸶳� ������ �ִ���

    private float currentAngle = 0f;  // ī�޶� ȸ���ϴ� ����

    private int count = 0;
    private bool isRight = false;

    void Update()
    {
        RotateCameraAroundBoard();  // ���带 �߽����� ī�޶� ȸ��
    }

    // ī�޶� ���带 �߽����� ������ �ӵ��� ȸ���ϴ� �Լ�
    void RotateCameraAroundBoard()
    {
        if(!isRight)
        {
            // ���� ���� (y�� �������� ȸ��)
            currentAngle += rotationSpeed * Time.deltaTime;
            if (currentAngle >= 360f)
            {
                currentAngle -= 360f;  // ������ 360�� ������ 0���� ����
                count++;
            }
        }
        else
        {
            // ���� ���� (y�� �������� ȸ��)
            currentAngle -= rotationSpeed * Time.deltaTime;
            if (currentAngle <= 0)
            {
                currentAngle += 360f;  // ������ 360�� ������ 0���� ����
                count++;
            }

        }

        //if(count > 1)
        //{
        //    isRight = !isRight;
        //    count = 0;
        //}
        // ȸ���� ��ġ ��� (���� �߽��� �������� ���� �˵�)
        Vector3 offset = new Vector3(Mathf.Sin(currentAngle * Mathf.Deg2Rad), 0, Mathf.Cos(currentAngle * Mathf.Deg2Rad)) * distanceFromBoard;

        // ī�޶��� ��ġ�� ���� �߽� �������� ����
        transform.position = boardTransform.position + offset + new Vector3(0, heightOffset, 0);

        // ī�޶� �׻� ���带 �ٶ󺸵��� ����
        transform.LookAt(boardTransform);
    }
}
