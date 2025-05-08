using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterSystem : MonoBehaviour
{
    [SerializeField] private Encounter[] enemiesInScene;
    [SerializeField] private int maxNumEnemies;
    [SerializeField] private EnemyManager enemyManager;
    void Start()
    {
        enemyManager = GameObject.FindFirstObjectByType<EnemyManager>();
        enemyManager.GenerateEnemyByEncounter(enemiesInScene, maxNumEnemies);
    }
}


[System.Serializable]
public class Encounter
{
    public EnemyInfo[] enemy;
    public int LevelMin;
    public int LevelMax;
}