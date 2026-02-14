using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EditorAttributes;
using SerializeReferenceEditor;
using UnityEngine;
using UnityEngine.Serialization;
using Utilities;
using Random = System.Random;
using Void = EditorAttributes.Void;

public class EnemyManager : MonoBehaviour, ILoadable
{
    [Header("References")]
    [SerializeField] private EnemyIndicator enemyIndicatorPrefab;
    
    [Header("Settings")]
    [SerializeField] private List<EnemySpawnSettings> _enemyPerDifficulty = new();
    [SerializeField, Required] private Transform enemyParent;
    [SerializeField, Range(0f, 10f)] private float enemyLifeSpan = 3f;

    public void LoadWithScene()
    {
        Registry<EnemyManager>.RegisterSingletonOrLogError(this);
    }

    public void UnLoadWithScene()
    {
        Registry<EnemyManager>.TryRemove(this);
    }

    public Vector3 GetSpawnPosition(int spawnIndex)
    {
        var shipSetup = Registry<ShipSetup>.GetFirst();
        if (!shipSetup)
        {
            Debug.LogError("No ship setup found");
            return Vector3.zero;
        }

        return shipSetup.ShipRows[spawnIndex].SpawnPosition;
    }
    
    public Vector3 GetIndicatorSpawnPosition(int spawnIndex)
    {
        var shipSetup = Registry<ShipSetup>.GetFirst();
        if (!shipSetup)
        {
            Debug.LogError("No ship setup found");
            return Vector3.zero;
        }

        return shipSetup.ShipRows[spawnIndex].IndicatorPosition;
    }

    public EnemySpawnSettings GenerateRandomEnemy(int round)
    {
        int totalWeight = 0;
        List<EnemySpawnSettings> enemiesForThisRound = new();

        foreach (var enemyPerDifficulty in _enemyPerDifficulty)
        {
            if (enemyPerDifficulty.AppearOnRound > round) continue;
            enemiesForThisRound.Add(enemyPerDifficulty);
            totalWeight += enemyPerDifficulty.SpawnWeight;
        }

        if (enemiesForThisRound.Count == 0)
        {
            Debug.LogError("No enemies to spawn");
            return null;
        }

        var random = new Random();
        var randomWeight = random.Next(0, totalWeight);
        var currentWeight = 0;
        foreach (var enemyPerDifficulty in enemiesForThisRound)
        {
            currentWeight += enemyPerDifficulty.SpawnWeight;
            if (randomWeight < currentWeight)
            {
                return enemyPerDifficulty;
            }
        }

        Debug.LogError("Bug in random selection of enemies..");
        throw new Exception("Bug in random selection of enemies..");
    }

    [Button]
    public void SpawnRandomEnemy(int round)
    {
        var enemyInfo = GenerateRandomEnemy(round);
        if (enemyInfo == null) return;
        StartCoroutine(SpawnEnemies(enemyInfo));
    }

    private IEnumerator SpawnEnemies(EnemySpawnSettings enemyInfo)
    {
        int nbOfRow = Registry<ShipSetup>.GetFirst().ShipRows.Length;
        List<int> spawnIndicies = GetUniqueRandomNumbers(0, nbOfRow, enemyInfo.AmountToSpawn);
        List<Vector3> indicatorSpawnPosition = spawnIndicies.ConvertAll(GetIndicatorSpawnPosition).ToList();

        var mainUICanvas = Registry<MainUICanvas>.GetFirst();
        if (mainUICanvas == null)
        {
            Debug.LogError("No Main UI Canvas found");
            yield break;
        }
        
        for (var index = 0; index < spawnIndicies.Count; index++)
        {
            var spawnPos = indicatorSpawnPosition[index];
            var indicator = Instantiate(enemyIndicatorPrefab, spawnPos, Quaternion.identity, mainUICanvas.transform);
            indicator.SetTimer(enemyInfo.TimeToSpawn, startTimer: true);
        }

        yield return new WaitForSeconds(enemyInfo.TimeToSpawn);

        List<Vector3> spawnPositions = spawnIndicies.ConvertAll(GetSpawnPosition).ToList();
        for (var index = 0; index < spawnIndicies.Count; index++)
        {
            var spawnPos = spawnPositions[index];
            Enemy enemy = Instantiate<Enemy>(enemyInfo.EnemyPrefab, spawnPos, Quaternion.identity, enemyParent);
            enemy.SetHealth(enemyInfo.Health)
                .SetSpeed(enemyInfo.Speed)
                .SetEnemyBehaviour(enemyInfo.EnemyBehaviour)
                .SetLifeSpan(enemyLifeSpan)
                .Spawn();
        }
    }

    /**
     * Get n unique random numbers between a and b
     */
    public List<int> GetUniqueRandomNumbers(int a, int b, int n)
    {
        List<int> numbers = new List<int>();
        for (int i = a; i < b; i++) numbers.Add(i);

        if (n > (b - a))
        {
            Debug.LogWarning("n is greater than the number of possible values.");
            return numbers;
        }

        numbers = numbers.Shuffle();
        return numbers.GetRange(0, n);
    }
}


[Serializable]
public class EnemySpawnSettings
{
    [Header("Spawn Settings")]
    [SerializeField, HorizontalGroup(nameof(appearOnRound), nameof(spawnWeight))]
    private Void groupHolder1;

    [Header("Prefab")]
    [SerializeField] private Enemy enemyPrefab;

    [Header("Behaviour")]
    [SerializeField, SerializeReference, SR] private EnemyBehaviour enemyBehaviour;

    [Header("Stats")]
    [SerializeField, HorizontalGroup(nameof(amountToSpawn), nameof(health))]
    private Void groupHolder2;

    [SerializeField, HorizontalGroup(nameof(speed), nameof(timeToSpawn))]
    private Void groupHolder3;

    [SerializeField, HideProperty] private int appearOnRound = 1;
    [SerializeField, HideProperty] private int spawnWeight = 1;

    [SerializeField, HideProperty] private int amountToSpawn = 1;
    [SerializeField, HideProperty] private int health = 1;
    [SerializeField, HideProperty] private int speed = 1;
    [SerializeField, HideProperty] private float timeToSpawn = 2;

    public Enemy EnemyPrefab => enemyPrefab;
    public EnemyBehaviour EnemyBehaviour => enemyBehaviour;
    public int AmountToSpawn => amountToSpawn;
    public int Health => health;
    public int Speed => speed;
    public float TimeToSpawn => timeToSpawn;
    public int SpawnWeight => spawnWeight;
    public int AppearOnRound => appearOnRound;
}

[Serializable]
public abstract class EnemyBehaviour
{
    public abstract void Update(Transform enemy, int speed);
}

[Serializable]
public class DefaultEnemyBehaviour : EnemyBehaviour
{
    public override void Update(Transform enemy, int speed)
    {
        enemy.position += Vector3.left * (speed * .1f);
    }
}