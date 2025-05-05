using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{
    [Header("Spawn Points")]
    [SerializeField] private Transform[] partySpawnPoints;
    [SerializeField] private Transform[] enemySpawnPoints;

    [Header("Battle Entities")]
    [SerializeField] private List<BattleEntities> allBattlers = new List<BattleEntities>();
    [SerializeField] private List<BattleEntities> playerBattlers = new List<BattleEntities>();
    [SerializeField] private List<BattleEntities> enemyBattlers = new List<BattleEntities>();

    private PartyManager _partyManager;
    private EnemyManager _enemyManager;
    void Start()
    {
        _partyManager = GameObject.FindFirstObjectByType<PartyManager>();
        _enemyManager = GameObject.FindFirstObjectByType<EnemyManager>();

        CreatePartyEntities();
        CreateEnemyEntities();
    }

    private void CreatePartyEntities()
    {
        List<PartyMember> currentParty = _partyManager.GetCurrentPartyMembers();
        for (int i = 0; i < currentParty.Count; i++)
        {
            BattleEntities newEntity = new BattleEntities();
            newEntity.SetEntityValues(currentParty[i].MemberName, currentParty[i].MaxHealth, currentParty[i].CurHealth, currentParty[i].Initiative, currentParty[i].Strength, currentParty[i].Level, true);

            BattleVisuals tempBattleVisual = Instantiate(currentParty[i].MemberBattleVisualPrefab, partySpawnPoints[i].position, Quaternion.identity).GetComponent<BattleVisuals>();
            tempBattleVisual.SetStaringValues(currentParty[i].MaxHealth, currentParty[i].CurHealth, currentParty[i].Level);
            newEntity.BattleVisuals = tempBattleVisual;

            allBattlers.Add(newEntity);
            playerBattlers.Add(newEntity);
        }
    }

    private void CreateEnemyEntities()
    {
        List<Enemy> currentEnemies = _enemyManager.GetCurrentEnemies();
        for (int i = 0; i < currentEnemies.Count; i++)
        {
            BattleEntities newEntity = new BattleEntities();
            newEntity.SetEntityValues(currentEnemies[i].EnemyName, currentEnemies[i].MaxHealth, currentEnemies[i].CurHealth, currentEnemies[i].Initiative, currentEnemies[i].Strength, currentEnemies[i].Level, false);

            BattleVisuals tempBattleVisual = Instantiate(currentEnemies[i].EnemyBattleVisualPrefab, enemySpawnPoints[i].position, Quaternion.identity).GetComponent<BattleVisuals>();
            tempBattleVisual.SetStaringValues(currentEnemies[i].MaxHealth, currentEnemies[i].CurHealth, currentEnemies[i].Level);
            newEntity.BattleVisuals = tempBattleVisual;

            allBattlers.Add(newEntity);
            enemyBattlers.Add(newEntity);
        }
    }

}


[System.Serializable]
public class BattleEntities
{
    public string Name;
    public int MaxHealth;
    public int CurrentHealth;
    public int Initiative;
    public int Strength;
    public int Level;
    public bool IsPlayer;
    public BattleVisuals BattleVisuals;

    public void SetEntityValues(string name, int maxHealth, int currentHealth, int initiative, int strength, int level, bool isPlayer)
    {
        Name = name;
        MaxHealth = maxHealth;
        CurrentHealth = currentHealth;
        Initiative = initiative;
        Strength = strength;
        Level = level;
        IsPlayer = isPlayer;
    }
}