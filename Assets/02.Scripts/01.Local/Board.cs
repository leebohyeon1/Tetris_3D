using UnityEngine;

public class Board : MonoBehaviour
{
    public Vector3 boardSize;

    public static int width = 10;   // 보드의 너비 (x축)
    public static int height = 20;  // 보드의 높이 (y축)
    public static int depth = 10;   // 보드의 깊이 (z축)

    public Transform[,,] grid = new Transform[width, height, depth];  // 3D 그리드

    // 보드 중앙 오프셋 (그리드 중심을 보드 중앙에 맞추기 위해)
    private Vector3 boardCenterOffset;

    public Material lineMaterial;  // LineRenderer에 사용할 Material
    public float rotationSpeed = 10f;  // 보드가 회전하는 속도

    void Start()
    {
        width = (int)boardSize.x;
        height = (int)boardSize.y;
        depth = (int)boardSize.z;

        boardCenterOffset = new Vector3(width / 2f, 0, depth / 2f);  // 보드 중앙 계산
        DrawGrid();  // 그리드 시각화
    }


    // 블록이 떨어질 때 보드 상에 업데이트하는 함수
    public void UpdateBoard(Transform block)
    {
        Vector3 pos = RoundPosition(block.position);  // 블록의 위치 반올림
        grid[(int)pos.x, (int)pos.y, (int)pos.z] = block;  // 보드에 블록 위치 업데이트
    }

    // 좌표를 반올림하여 반환 (블록의 위치를 그리드에 맞추기 위해)
    public Vector3 RoundPosition(Vector3 pos)
    {
        return new Vector3(Mathf.Round(pos.x), Mathf.Round(pos.y), Mathf.Round(pos.z));
    }

    // 좌표가 보드 내부에 있는지 확인 (x, y, z 축 범위 검사)
    public bool IsInsideGrid(Vector3 pos)
    {
        Vector3 offsetPos = pos + boardCenterOffset;  // 중앙 오프셋 적용
        return (offsetPos.x >= 0 && offsetPos.x < width && offsetPos.y >= 0 && offsetPos.y < height && offsetPos.z >= 0 && offsetPos.z < depth);
    }

    // 좌표에 블록이 이미 있는지 확인
    public bool IsPositionOccupied(Vector3 pos)
    {
        Vector3 offsetPos = pos + boardCenterOffset;  // 중앙 오프셋 적용
        return grid[(int)offsetPos.x, (int)offsetPos.y, (int)offsetPos.z] != null;
    }

    // 블록을 그리드에 추가
    public void AddToGrid(Tetromino tetromino)
    {
        foreach (Transform child in tetromino.transform)
        {
            Vector3 pos = RoundPosition(child.position + boardCenterOffset);  // 중앙 오프셋 적용
            grid[(int)pos.x, (int)pos.y, (int)pos.z] = child;

        }
    }

    // 블록을 그리드에서 제거
    public void RemoveFromGrid(Tetromino tetromino)
    {
        foreach (Transform child in tetromino.transform)
        {
            Vector3 pos = RoundPosition(child.position + boardCenterOffset);  // 중앙 오프셋 적용
            if (grid[(int)pos.x, (int)pos.y, (int)pos.z] == child)
            {
                grid[(int)pos.x, (int)pos.y, (int)pos.z] = null;
            }
        }
    }

    // 블록이 움직이기 전에 그리드에서 제거한 후, 움직인 후 다시 등록
    public void UpdateGrid(Tetromino tetromino)
    {
        RemoveFromGrid(tetromino);  // 이전 위치에서 블록 제거
        AddToGrid(tetromino);       // 새로운 위치에 블록 추가
    }

    // 줄을 확인하고 가득 찬 줄을 삭제
    public void CheckForLines()
    {
        for (int y = 0; y < height; y++)
        {
            if (IsLineFull(y))
            {
                DeleteLine(y);
                MoveAllLinesDown(y);
            }
        }
    }

    // y축에서 한 줄이 꽉 찼는지 확인
    bool IsLineFull(int y)
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                if (grid[x, y, z] == null)
                {
                    return false;
                }
            }
        }
        return true;
    }

    // 가득 찬 줄을 삭제
    void DeleteLine(int y)
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                if (grid[x, y, z] != null)
                {
                    Destroy(grid[x, y, z].gameObject);
                    grid[x, y, z] = null;
                }
            }
        }
    }

    // 모든 줄을 아래로 이동
    void MoveAllLinesDown(int y)
    {
        for (int i = y; i < height; i++)
        {
            MoveLineDown(i);
        }
    }

    // y축에서 한 줄씩 아래로 이동
    void MoveLineDown(int y)
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                if (grid[x, y, z] != null)
                {
                    grid[x, y - 1, z] = grid[x, y, z];
                    grid[x, y, z] = null;
                    grid[x, y - 1, z].position += new Vector3(0, -1, 0);
                }
            }
        }
    }

    // 그리드를 시각적으로 표현하는 함수
    void DrawGrid()
    {
        for (int y = 0; y <= height; y++)
        {
            for (int z = 0; z <= depth; z++)
            {
                DrawLine(new Vector3(0, y, z) - boardCenterOffset, new Vector3(width, y, z) - boardCenterOffset);
            }
            for (int x = 0; x <= width; x++)
            {
                DrawLine(new Vector3(x, y, 0) - boardCenterOffset, new Vector3(x, y, depth) - boardCenterOffset);
            }
        }
    }

    // LineRenderer로 선을 그리는 함수
    void DrawLine(Vector3 start, Vector3 end)
    {
        GameObject lineObj = new GameObject("GridLine");

        LineRenderer lineRenderer = lineObj.AddComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.material = lineMaterial;
        lineRenderer.startColor = Color.gray;
        lineRenderer.endColor = Color.gray;
    }
}
