using UnityEngine;

public class Board : MonoBehaviour
{
    public Vector3 boardSize;

    public static int width = 10;   // ������ �ʺ� (x��)
    public static int height = 20;  // ������ ���� (y��)
    public static int depth = 10;   // ������ ���� (z��)

    public Transform[,,] grid = new Transform[width, height, depth];  // 3D �׸���

    // ���� �߾� ������ (�׸��� �߽��� ���� �߾ӿ� ���߱� ����)
    private Vector3 boardCenterOffset;

    public Material lineMaterial;  // LineRenderer�� ����� Material
    public float rotationSpeed = 10f;  // ���尡 ȸ���ϴ� �ӵ�

    void Start()
    {
        width = (int)boardSize.x;
        height = (int)boardSize.y;
        depth = (int)boardSize.z;

        boardCenterOffset = new Vector3(width / 2f, 0, depth / 2f);  // ���� �߾� ���
        DrawGrid();  // �׸��� �ð�ȭ
    }


    // ����� ������ �� ���� �� ������Ʈ�ϴ� �Լ�
    public void UpdateBoard(Transform block)
    {
        Vector3 pos = RoundPosition(block.position);  // ����� ��ġ �ݿø�
        grid[(int)pos.x, (int)pos.y, (int)pos.z] = block;  // ���忡 ��� ��ġ ������Ʈ
    }

    // ��ǥ�� �ݿø��Ͽ� ��ȯ (����� ��ġ�� �׸��忡 ���߱� ����)
    public Vector3 RoundPosition(Vector3 pos)
    {
        return new Vector3(Mathf.Round(pos.x), Mathf.Round(pos.y), Mathf.Round(pos.z));
    }

    // ��ǥ�� ���� ���ο� �ִ��� Ȯ�� (x, y, z �� ���� �˻�)
    public bool IsInsideGrid(Vector3 pos)
    {
        Vector3 offsetPos = pos + boardCenterOffset;  // �߾� ������ ����
        return (offsetPos.x >= 0 && offsetPos.x < width && offsetPos.y >= 0 && offsetPos.y < height && offsetPos.z >= 0 && offsetPos.z < depth);
    }

    // ��ǥ�� ����� �̹� �ִ��� Ȯ��
    public bool IsPositionOccupied(Vector3 pos)
    {
        Vector3 offsetPos = pos + boardCenterOffset;  // �߾� ������ ����
        return grid[(int)offsetPos.x, (int)offsetPos.y, (int)offsetPos.z] != null;
    }

    // ����� �׸��忡 �߰�
    public void AddToGrid(Tetromino tetromino)
    {
        foreach (Transform child in tetromino.transform)
        {
            Vector3 pos = RoundPosition(child.position + boardCenterOffset);  // �߾� ������ ����
            grid[(int)pos.x, (int)pos.y, (int)pos.z] = child;

        }
    }

    // ����� �׸��忡�� ����
    public void RemoveFromGrid(Tetromino tetromino)
    {
        foreach (Transform child in tetromino.transform)
        {
            Vector3 pos = RoundPosition(child.position + boardCenterOffset);  // �߾� ������ ����
            if (grid[(int)pos.x, (int)pos.y, (int)pos.z] == child)
            {
                grid[(int)pos.x, (int)pos.y, (int)pos.z] = null;
            }
        }
    }

    // ����� �����̱� ���� �׸��忡�� ������ ��, ������ �� �ٽ� ���
    public void UpdateGrid(Tetromino tetromino)
    {
        RemoveFromGrid(tetromino);  // ���� ��ġ���� ��� ����
        AddToGrid(tetromino);       // ���ο� ��ġ�� ��� �߰�
    }

    // ���� Ȯ���ϰ� ���� �� ���� ����
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

    // y�࿡�� �� ���� �� á���� Ȯ��
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

    // ���� �� ���� ����
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

    // ��� ���� �Ʒ��� �̵�
    void MoveAllLinesDown(int y)
    {
        for (int i = y; i < height; i++)
        {
            MoveLineDown(i);
        }
    }

    // y�࿡�� �� �پ� �Ʒ��� �̵�
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

    // �׸��带 �ð������� ǥ���ϴ� �Լ�
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

    // LineRenderer�� ���� �׸��� �Լ�
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
