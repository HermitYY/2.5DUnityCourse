using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Runtime.CompilerServices;

public class BattleSystem : MonoBehaviour
{
    [Serializable]
    private enum BattleState
    {
        Start,
        Selection,
        Battle,
        Won,
        Lost,
        Run
    }
    [Header("Battle state")]
    [SerializeField] private BattleState state;


    [Header("Spawn Points")]
    [SerializeField] private Transform[] partySpawnPoints;
    [SerializeField] private Transform[] enemySpawnPoints;

    [Header("Battle Entities")]
    [SerializeField] private List<BattleEntities> allBattlers = new List<BattleEntities>();
    [SerializeField] private List<BattleEntities> playerBattlers = new List<BattleEntities>();
    [SerializeField] private List<BattleEntities> enemyBattlers = new List<BattleEntities>();

    [Header("UI")]
    [SerializeField] private GameObject[] enemySelectionButtons;
    [SerializeField] private GameObject battleMenu;
    [SerializeField] private GameObject enemySelectionMenu;
    [SerializeField] private TextMeshProUGUI actionText;
    [SerializeField] private GameObject bottomTextPopUp;
    [SerializeField] private TextMeshProUGUI bottomTextPopUpText;

    private PartyManager _partyManager;
    private EnemyManager _enemyManager;
    private int _currentPlayerIndex = 0;

    private const string COMSTR_ACTION_MESSAGE = "'s Action!";
    private const string COMSTR_ATTACK_BATTLE_TIP = "{0} attacked {1} for {2} damage!";

    void Start()
    {
        _partyManager = GameObject.FindFirstObjectByType<PartyManager>();
        _enemyManager = GameObject.FindFirstObjectByType<EnemyManager>();

        CreatePartyEntities();
        CreateEnemyEntities();
        ShowBattkeMenu();
    }

    private IEnumerator BattleRoutine()
    {
        enemySelectionMenu.SetActive(false);
        state = BattleState.Battle;
        bottomTextPopUp.SetActive(true);
        bottomTextPopUpText.text = "Battle Start!";

        for (int i = 0; i < allBattlers.Count; i++)
        {
            switch (allBattlers[i].BattleAction)
            {
                case BattleEntities.Action.Attack:
                    AttackAction(allBattlers[i], allBattlers[allBattlers[i].TargetIndex]);
                    Debug.Log(allBattlers[i].Name + " attacked " + allBattlers[allBattlers[i].TargetIndex].Name + " for " + allBattlers[i].Strength + " damage!");
                    break;
                case BattleEntities.Action.Run:
                    // Implement run logic here
                    break;
            }
        }

        if (state == BattleState.Battle)
        {
            bottomTextPopUp.SetActive(false);
            _currentPlayerIndex = 0;
            ShowBattkeMenu();
        }

        yield return null;
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

    public void ShowBattkeMenu()
    {
        actionText.text = playerBattlers[_currentPlayerIndex].Name + COMSTR_ACTION_MESSAGE;
        battleMenu.SetActive(true);
    }

    public void ShowEnemySelectionMenu()
    {
        battleMenu.SetActive(false);
        SetEnemySelectionButtons();
        enemySelectionMenu.SetActive(true);
    }

    public void SetEnemySelectionButtons()
    {
        for (int i = 0; i < enemySelectionButtons.Length; i++)
        {
            if (i < enemyBattlers.Count)
            {
                enemySelectionButtons[i].SetActive(true);
                enemySelectionButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = enemyBattlers[i].Name;
            }
            else
            {
                enemySelectionButtons[i].SetActive(false);
            }
        }
    }

    public void SelectEnemy(int enemyIndex)
    {
        BattleEntities currentPlayerEntity = playerBattlers[_currentPlayerIndex];
        currentPlayerEntity.SetTarget(allBattlers.IndexOf(enemyBattlers[enemyIndex]));
        currentPlayerEntity.BattleAction = BattleEntities.Action.Attack;
        _currentPlayerIndex++;

        if (_currentPlayerIndex >= playerBattlers.Count)
        {
            _currentPlayerIndex = 0;
            enemySelectionMenu.SetActive(false);
            StartCoroutine(BattleRoutine());
            Debug.Log("Start Battle!");
        }
        else
        {
            enemySelectionMenu.SetActive(false);
            ShowBattkeMenu();
        }

    }

    private void AttackAction(BattleEntities attacker, BattleEntities target)
    {
        target.CurrentHealth -= attacker.Strength;
        attacker.BattleVisuals.PlayAttackAnimation();
        target.BattleVisuals.PlayHitAnimation();
        target.UpdateUI();
        bottomTextPopUpText.text = String.Format(COMSTR_ATTACK_BATTLE_TIP, attacker.Name, target.Name, attacker.Strength);
    }

}


[System.Serializable]
public class BattleEntities
{
    public enum Action
    {
        Attack,
        Run
    }
    public Action BattleAction;

    public string Name;
    public int MaxHealth;
    public int CurrentHealth;
    public int Initiative;
    public int Strength;
    public int Level;
    public bool IsPlayer;
    public BattleVisuals BattleVisuals;
    public int TargetIndex;

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

    public void SetTarget(int targetIndex)
    {
        TargetIndex = targetIndex;
    }

    public void UpdateUI()
    {
        BattleVisuals.ChangeHealth(CurrentHealth);
        BattleVisuals.UpdateHealthBar();
    }
}