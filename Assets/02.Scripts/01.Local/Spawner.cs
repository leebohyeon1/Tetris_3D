using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject[] tetrominoes;

    private void Start()
    {
        SpawnNewTetromino();
    }

    // 새로운 Tetromino 생성
    public void SpawnNewTetromino()
    {
        int randomIndex = Random.Range(0, tetrominoes.Length);
        Instantiate(tetrominoes[randomIndex], transform.position, Quaternion.identity);
    }
}
