using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private readonly System.Random rnd = new();

    [SerializeField] private Transform[] EnemySpawnPoints;
    [SerializeField] private GameObject EnemyObject;
    [SerializeField] private float TimeToSpawn;
    [SerializeField] private float EnemyCount;

    private float tmtspwn;
    private int TransformChoose;

    private void Update()
    {
        if (EnemyCount > 0 && tmtspwn <= 0)
        {
            SpawnEnemyGameObject();
            tmtspwn = TimeToSpawn;
        }
        else tmtspwn -= Time.deltaTime;
    }

    private void SpawnEnemyGameObject()
    {
        TransformChoose = rnd.Next(0, EnemySpawnPoints.Length);
        _ = Instantiate(EnemyObject, new Vector3(EnemySpawnPoints[TransformChoose].position.x,
            EnemySpawnPoints[TransformChoose].position.y, EnemySpawnPoints[TransformChoose].position.z), Quaternion.identity);
        EnemyCount--;
    }
}