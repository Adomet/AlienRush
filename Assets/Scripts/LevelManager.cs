using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.Pool;

public class LevelManager : MonoBehaviour
{
    public IObjectPool<AlienController> alienPool;
    public List<AlienController> aliens;
    private Level currentLevel;
    private int currentWaveIndex = 0;
    private bool collectionCheck = true;
    private int defaultCapacity = 50;
    private int maxCapacity = 500;
    public List<Level> levels;
    public bool isLevelFinished = false;
    public int timeRemaining = 0;

    public AlienController enemyPrefab;


    private void Awake()
    {
        alienPool = new ObjectPool<AlienController>(CreateAlien, OnGetFromPool, OnReleaseToPool, OnDestroyPooledAlien, collectionCheck, defaultCapacity, maxCapacity);
    }

    public AlienController CreateAlien()
    {
        AlienController alien = Instantiate(enemyPrefab, Vector3.zero, Quaternion.identity);
        alien.Pool = alienPool;
        return alien;
    }


    public void OnGetFromPool(AlienController alien)
    {
        alien.gameObject.SetActive(true);
    }


    public void OnReleaseToPool(AlienController alien)
    {
        alien.gameObject.SetActive(false);
    }


    public void OnDestroyPooledAlien(AlienController alien)
    {
        Destroy(alien.gameObject);
    }


    public void play(int levelIndex = 0)
    {
        if (levelIndex < 0 || levelIndex >= levels.Count)
            levelIndex = levels.Count - 1;

        isLevelFinished = false;
        currentLevel = levels[levelIndex];
        clean();
        StopAllCoroutines();
        StartCoroutine(nameof(StartLevel));
    }

    public void clean()
    {
        foreach (AlienController alien in aliens)
        {
            alienPool.Release(alien);
        }

        aliens.Clear();
    }

    public IEnumerator StartLevel()
    {
        timeRemaining = (int)currentLevel.levelDuration;
        StartCoroutine(nameof(updateTimer));

        Debug.Log($"{currentLevel} start!");
        for (currentWaveIndex = 0; currentWaveIndex < currentLevel.waves.Count; currentWaveIndex++)
        {
            WaveData wave = currentLevel.waves[currentWaveIndex];
            Debug.Log($"{wave} start...");

            yield return StartCoroutine(SpawnWave(wave));
            yield return new WaitForSeconds(wave.waveDelay);
        }

        yield return new WaitUntil(() => aliens.Count == 0);
        isLevelFinished = true;
        Debug.Log($"{currentLevel} end!");
    }

    public IEnumerator updateTimer()
    {
        while (timeRemaining > 0)
        {
            yield return new WaitForSeconds(1);
            timeRemaining--;
        }

        // times up
        Debug.Log("Times up!");
        isLevelFinished = true;
    }

    private IEnumerator SpawnWave(WaveData wave)
    {
        for (int i = 0; i < wave.spawnCount; i++)
        {
            Vector3 spawnPoint = RandomPointAroundPlayer(PlayerController.instance.transform, currentLevel.minSpawnDistance, currentLevel.maxSpawnDistance);

            for (int j = 0; j < wave.enemyPerSpawn; j++)
            {
                AlienController alien = alienPool.Get();
                alien.transform.position = spawnPoint;
                alien.init(currentLevel.enemyHealth, currentLevel.enemySpeed, currentLevel.enemyDamage, currentLevel.enemyHitRate);
                aliens.Add(alien);
                yield return new WaitForSeconds(0.1f);
            }
            
            yield return new WaitForSeconds(wave.spawnDelay);
        }
    }

    public Vector3 RandomPointAroundPlayer(Transform playerTrans, float minDistance, float maxDistance)
    {
        if (minDistance < 0f) minDistance = 0f;
        if (maxDistance < minDistance) maxDistance = minDistance;

        var randomDirection = Random.insideUnitCircle.normalized;
        var randDist = Random.Range(minDistance, maxDistance);

        Vector3 offset = new Vector3(randomDirection.x, 0, randomDirection.y) * randDist;
        Vector3 finalPosition = playerTrans.position + offset;

        return finalPosition;
    }
}