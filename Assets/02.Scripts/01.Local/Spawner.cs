using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject[] tetrominoes;

    private void Start()
    {
        SpawnNewTetromino();
    }

    // ���ο� Tetromino ����
    public void SpawnNewTetromino()
    {
        int randomIndex = Random.Range(0, tetrominoes.Length);
        Instantiate(tetrominoes[randomIndex], transform.position, Quaternion.identity);
    }
}
