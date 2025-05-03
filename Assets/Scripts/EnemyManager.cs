using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private EnemyInfo[] allEnemies;
    [SerializeField] private List<Enemy> curEnemies;

    private void Awake()
    {
        GenerateEnemyByName("Slime", 1);
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
                curEnemies.Add(newEnemy);
            }
        }
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
