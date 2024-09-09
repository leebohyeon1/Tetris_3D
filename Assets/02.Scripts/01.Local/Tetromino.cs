using UnityEngine;

public class Tetromino : MonoBehaviour
{
    public float fallSpeed = 1.0f;
    private float previousTime;

    private GameObject ghostBlock;  // Ghost Block (예상 낙하 위치 블록)
    public Material ghostMaterial;  // Ghost Block에 적용할 투명한 Material

    private Board board;
    private Camera mainCamera;  // 카메라 참조

    void Start()
    {
        board = FindObjectOfType<Board>();
        mainCamera = Camera.main;  // 메인 카메라 참조
        CreateGhostBlock();  // 게임 시작 시 Ghost Block 생성
    }

    void Update()
    {
        HandleInput();  // 블록 이동 및 회전 입력 처리
        HandleFall();   // 블록 자동 낙하 처리
        UpdateGhostBlock();  // 실시간으로 Ghost Block 업데이트
    }

    // 사용자 입력 처리 (블록 이동 및 회전)
    void HandleInput()
    {
        Vector3 moveDirection = Vector3.zero;  // 이동 방향 초기화

        // 카메라의 forward와 right 벡터를 가져옴 (이동 방향 결정)
        Vector3 cameraForward = new Vector3(mainCamera.transform.forward.x, 0, mainCamera.transform.forward.z).normalized;
        Vector3 cameraRight = new Vector3(mainCamera.transform.right.x, 0, mainCamera.transform.right.z).normalized;

        // W키를 누르면 카메라의 직각 방향으로 이동
        if (Input.GetKeyDown(KeyCode.W))
        {
            moveDirection = GetClosestRightAngleDirection(cameraForward);  // 직각 방향으로 이동
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            moveDirection = GetClosestRightAngleDirection(-cameraForward);  // 반대 직각 방향으로 이동
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            moveDirection = GetClosestRightAngleDirection(-cameraRight);  // 카메라의 왼쪽 직각 방향으로 이동
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            moveDirection = GetClosestRightAngleDirection(cameraRight);  // 카메라의 오른쪽 직각 방향으로 이동
        }

        // 블록을 이동시킴
        if (moveDirection != Vector3.zero)
        {
            transform.position += moveDirection;
            if (!IsValidMove())
            {
                transform.position -= moveDirection;  // 유효하지 않으면 원래 위치로 복구
            }
        }
        // 블록 회전 (Q, E)
        if (Input.GetKeyDown(KeyCode.Q))
        {
            transform.Rotate(0, 90, 0);  // y축 회전
            if (!IsValidMove()) transform.Rotate(0, -90, 0);  // 유효하지 않으면 복구
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            transform.Rotate(-90, 0, 0);  // x축 회전
            if (!IsValidMove()) transform.Rotate(90, 0, 0);  // 유효하지 않으면 복구
        }
    }

    // 블록이 낙하하는 처리
    void HandleFall()
    {
        //if (Time.time - previousTime > fallSpeed)
        //{
            transform.position += new Vector3(0, -0.01f, 0);  // y축으로 낙하
            if (!IsValidMove())
            {
                transform.position += new Vector3(0, 0.5f, 0);  // 충돌하면 원래 위치로 복구
                enabled = false;  // 더 이상 움직이지 않음
                board.AddToGrid(this);
                board.CheckForLines();  // 줄 완성 여부 확인
                FindObjectOfType<Spawner>().SpawnNewTetromino();  // 새로운 Tetromino 생성
                Destroy(ghostBlock);  // Ghost Block 삭제


                transform.SetParent(board.transform);
            }
            previousTime = Time.time;
        //}
    }

    // 가장 가까운 직각 방향을 반환하는 함수
    Vector3 GetClosestRightAngleDirection(Vector3 direction)
    {
        // X축이나 Z축 중 가장 가까운 방향을 선택
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.z))
        {
            // X축 방향이 더 크면 X축을 기준으로 직각 이동
            return new Vector3(Mathf.Sign(direction.x), 0, 0);  // X축으로 이동
        }
        else
        {
            // Z축 방향이 더 크면 Z축을 기준으로 직각 이동
            return new Vector3(0, 0, Mathf.Sign(direction.z));  // Z축으로 이동
        }
    }

    // 블록이 유효한 위치에 있는지 확인
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

    #region Ghost Block 관련
    // Ghost Block 생성
    void CreateGhostBlock()
    {
        ghostBlock = Instantiate(gameObject, transform.position, transform.rotation);  // 현재 블록을 복제
        Destroy(ghostBlock.GetComponent<Tetromino>());  // Ghost Block에서는 Tetromino 스크립트 비활성화
        
        foreach (Transform child in ghostBlock.transform)
        {
            Destroy(child.GetComponent<Collider>());
        }

        ApplyGhostMaterial();  // Ghost Block에 투명한 Material 적용
    }

    // Ghost Block에 투명한 Material 적용
    void ApplyGhostMaterial()
    {
        foreach (Transform child in ghostBlock.transform)
        {
            child.GetComponent<Renderer>().material = ghostMaterial;  // 투명한 Material 적용
        }
    }

    // Ghost Block 업데이트
    void UpdateGhostBlock()
    {
        ghostBlock.transform.position = transform.position;
        ghostBlock.transform.rotation = transform.rotation;

        while (IsValidGhostMove())
        {
            ghostBlock.transform.position += new Vector3(0, -0.01f, 0);
        }

        // 잘못된 위치로 이동했으면 다시 한 칸 위로 복구
        ghostBlock.transform.position += new Vector3(0, 0.5f, 0);
    }

    // Ghost Block이 유효한 위치에 있는지 확인 (충돌 검사)
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
