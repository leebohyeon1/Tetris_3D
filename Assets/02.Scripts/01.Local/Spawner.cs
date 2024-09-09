using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject[] tetrominoes;

    private void Start()
    {
        SpawnNewTetromino();
    }

    // 货肺款 Tetromino 积己
    public void SpawnNewTetromino()
    {
        int randomIndex = Random.Range(0, tetrominoes.Length);
        Instantiate(tetrominoes[randomIndex], transform.position, Quaternion.identity);
    }
}
