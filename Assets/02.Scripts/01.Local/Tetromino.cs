using UnityEngine;

public class Tetromino : MonoBehaviour
{
    public float fallSpeed = 1.0f;
    private float previousTime;

    private GameObject ghostBlock;  // Ghost Block (���� ���� ��ġ ���)
    public Material ghostMaterial;  // Ghost Block�� ������ ������ Material

    private Board board;
    private Camera mainCamera;  // ī�޶� ����

    void Start()
    {
        board = FindObjectOfType<Board>();
        mainCamera = Camera.main;  // ���� ī�޶� ����
        CreateGhostBlock();  // ���� ���� �� Ghost Block ����
    }

    void Update()
    {
        HandleInput();  // ��� �̵� �� ȸ�� �Է� ó��
        HandleFall();   // ��� �ڵ� ���� ó��
        UpdateGhostBlock();  // �ǽð����� Ghost Block ������Ʈ
    }

    // ����� �Է� ó�� (��� �̵� �� ȸ��)
    void HandleInput()
    {
        Vector3 moveDirection = Vector3.zero;  // �̵� ���� �ʱ�ȭ

        // ī�޶��� forward�� right ���͸� ������ (�̵� ���� ����)
        Vector3 cameraForward = new Vector3(mainCamera.transform.forward.x, 0, mainCamera.transform.forward.z).normalized;
        Vector3 cameraRight = new Vector3(mainCamera.transform.right.x, 0, mainCamera.transform.right.z).normalized;

        // WŰ�� ������ ī�޶��� ���� �������� �̵�
        if (Input.GetKeyDown(KeyCode.W))
        {
            moveDirection = GetClosestRightAngleDirection(cameraForward);  // ���� �������� �̵�
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            moveDirection = GetClosestRightAngleDirection(-cameraForward);  // �ݴ� ���� �������� �̵�
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            moveDirection = GetClosestRightAngleDirection(-cameraRight);  // ī�޶��� ���� ���� �������� �̵�
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            moveDirection = GetClosestRightAngleDirection(cameraRight);  // ī�޶��� ������ ���� �������� �̵�
        }

        // ����� �̵���Ŵ
        if (moveDirection != Vector3.zero)
        {
            transform.position += moveDirection;
            if (!IsValidMove())
            {
                transform.position -= moveDirection;  // ��ȿ���� ������ ���� ��ġ�� ����
            }
        }
        // ��� ȸ�� (Q, E)
        if (Input.GetKeyDown(KeyCode.Q))
        {
            transform.Rotate(0, 90, 0);  // y�� ȸ��
            if (!IsValidMove()) transform.Rotate(0, -90, 0);  // ��ȿ���� ������ ����
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            transform.Rotate(-90, 0, 0);  // x�� ȸ��
            if (!IsValidMove()) transform.Rotate(90, 0, 0);  // ��ȿ���� ������ ����
        }
    }

    // ����� �����ϴ� ó��
    void HandleFall()
    {
        //if (Time.time - previousTime > fallSpeed)
        //{
            transform.position += new Vector3(0, -0.01f, 0);  // y������ ����
            if (!IsValidMove())
            {
                transform.position += new Vector3(0, 0.5f, 0);  // �浹�ϸ� ���� ��ġ�� ����
                enabled = false;  // �� �̻� �������� ����
                board.AddToGrid(this);
                board.CheckForLines();  // �� �ϼ� ���� Ȯ��
                FindObjectOfType<Spawner>().SpawnNewTetromino();  // ���ο� Tetromino ����
                Destroy(ghostBlock);  // Ghost Block ����


                transform.SetParent(board.transform);
            }
            previousTime = Time.time;
        //}
    }

    // ���� ����� ���� ������ ��ȯ�ϴ� �Լ�
    Vector3 GetClosestRightAngleDirection(Vector3 direction)
    {
        // X���̳� Z�� �� ���� ����� ������ ����
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.z))
        {
            // X�� ������ �� ũ�� X���� �������� ���� �̵�
            return new Vector3(Mathf.Sign(direction.x), 0, 0);  // X������ �̵�
        }
        else
        {
            // Z�� ������ �� ũ�� Z���� �������� ���� �̵�
            return new Vector3(0, 0, Mathf.Sign(direction.z));  // Z������ �̵�
        }
    }

    // ����� ��ȿ�� ��ġ�� �ִ��� Ȯ��
    bool IsValidMove()
    {
        foreach (Transform child in transform)
        {
            Vector3 pos = board.RoundPosition(child.position);
            if (!board.IsInsideGrid(pos))
                return false;
            if (board.IsPositionOccupied(pos))
                return false;
        }
        return true;
    }

    #region Ghost Block ����
    // Ghost Block ����
    void CreateGhostBlock()
    {
        ghostBlock = Instantiate(gameObject, transform.position, transform.rotation);  // ���� ����� ����
        Destroy(ghostBlock.GetComponent<Tetromino>());  // Ghost Block������ Tetromino ��ũ��Ʈ ��Ȱ��ȭ
        
        foreach (Transform child in ghostBlock.transform)
        {
            Destroy(child.GetComponent<Collider>());
        }

        ApplyGhostMaterial();  // Ghost Block�� ������ Material ����
    }

    // Ghost Block�� ������ Material ����
    void ApplyGhostMaterial()
    {
        foreach (Transform child in ghostBlock.transform)
        {
            child.GetComponent<Renderer>().material = ghostMaterial;  // ������ Material ����
        }
    }

    // Ghost Block ������Ʈ
    void UpdateGhostBlock()
    {
        ghostBlock.transform.position = transform.position;
        ghostBlock.transform.rotation = transform.rotation;

        while (IsValidGhostMove())
        {
            ghostBlock.transform.position += new Vector3(0, -0.01f, 0);
        }

        // �߸��� ��ġ�� �̵������� �ٽ� �� ĭ ���� ����
        ghostBlock.transform.position += new Vector3(0, 0.5f, 0);
    }

    // Ghost Block�� ��ȿ�� ��ġ�� �ִ��� Ȯ�� (�浹 �˻�)
    bool IsValidGhostMove()
    {
        foreach (Transform child in ghostBlock.transform)
        {
            Vector3 pos = board.RoundPosition(child.position);
            if (!board.IsInsideGrid(pos))
                return false;
            if (board.IsPositionOccupied(pos))
                return false;
        }
        return true;
    }
    #endregion
}
