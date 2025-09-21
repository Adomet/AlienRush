using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Level", menuName = "Level")]
public class Level : ScriptableObject
{
    [Header("General Level Info")]
    public float levelDuration = 180f;      
    public float minSpawnDistance;
    public float maxSpawnDistance;
    
    [Header("Difficulty Modifiers")]
    public float enemyHealth = 100f;
    public float enemySpeed = 3.5f;
    public float enemyDamage = 10f;
    public float enemyHitRate = 1.5f;
    
    [Header("Waves")]
    public List<WaveData> waves;              
}

[System.Serializable]
public class WaveData
{
    [Header("Wave Info")]
    public int spawnCount = 3; 
    public float enemyPerSpawn = 1;
    public float spawnDelay = 1f;
    
    [Header("Timing")]
    public float waveDelay = 3f;            
}