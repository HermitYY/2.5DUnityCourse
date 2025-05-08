using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private EnemyInfo[] allEnemies;
    [SerializeField] private List<Enemy> currentEnemies;

    private static EnemyManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }

    public void GenerateEnemyByEncounter(Encounter[] encounters, int maxNumEnemies)
    {
        currentEnemies.Clear();
        int numEnemies = Random.Range(1, maxNumEnemies + 1);
        for (int i = 0; i < numEnemies; i++)
        {
            int randomEncounterIndex = Random.Range(0, encounters.Length);
            Encounter encounter = encounters[randomEncounterIndex];
            int randomEnemyIndex = Random.Range(0, encounter.enemy.Length);
            EnemyInfo enemyInfo = encounter.enemy[randomEnemyIndex];
            int level = Random.Range(encounter.LevelMin, encounter.LevelMax + 1);
            GenerateEnemyByName(enemyInfo.EnemyName, level);
        }
    }

    private void GenerateEnemyByName(string enemyName, int level)
    {
        for (int i = 0; i < allEnemies.Length; i++)
        {
            if (allEnemies[i].EnemyName == enemyName)
            {
                Enemy newEnemy = new Enemy();
                newEnemy.EnemyName = allEnemies[i].EnemyName;
                newEnemy.Level = level;
                newEnemy.CurHealth = allEnemies[i].BaseHealth;
                newEnemy.MaxHealth = newEnemy.CurHealth;
                newEnemy.Strength = allEnemies[i].BaseStrength;
                newEnemy.Initiative = allEnemies[i].BaseInitiative;
                newEnemy.EnemyBattleVisualPrefab = allEnemies[i].EnemyBattleVisualPrefab;
                currentEnemies.Add(newEnemy);
            }
        }
    }

    public List<Enemy> GetCurrentEnemies()
    {
        return currentEnemies;
    }
}

[System.Serializable]
public class Enemy
{
    public string EnemyName;
    public int Level;
    public int CurHealth;
    public int MaxHealth;
    public int Strength;
    public int Initiative;
    public GameObject EnemyBattleVisualPrefab;
}
