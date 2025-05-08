using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Runtime.CompilerServices;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

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

    private const int TURN_DURATION = 1;
    private const string COMSTR_OVERWORLD_SCENE = "OverworldScene";
    private const string COMSTR_ACTION_MESSAGE = "'s Action!";
    private const string COMSTR_ATTACK_BATTLE_TIP = "{0} attacked {1} for {2} damage!";
    private const string COMSTR_BATTLE_START = "Battle Start!";
    private const string COMSTR_BATTLE_WON = "You won the battle!";
    private const string COMSTR_BATTLE_LOST = "You lost the battle!";
    private const string COMSTR_DEFEATED = "{0} defeated {1}";



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
        bottomTextPopUpText.text = COMSTR_BATTLE_START;

        for (int i = 0; i < allBattlers.Count; i++)
        {
            if (state != BattleState.Battle)
            {
                yield break;
            }
            switch (allBattlers[i].BattleAction)
            {
                case BattleEntities.Action.Attack:
                    yield return StartCoroutine(AttackRoutine(i));
                    break;
                case BattleEntities.Action.Run:
                    yield return StartCoroutine(EnemyAttackRoutine(i));
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

    private IEnumerator AttackRoutine(int i)
    {
        if (allBattlers[i].IsPlayer)
        {
            yield return StartCoroutine(PartyAttackRoutine(i));
        }
        else
        {
            yield return StartCoroutine(EnemyAttackRoutine(i));
        }
    }

    private IEnumerator PartyAttackRoutine(int i)
    {
        if (allBattlers[i].IsPlayer)
        {
            BattleEntities attacker = allBattlers[i];
            if (attacker.TargetIndex >= allBattlers.Count)
            {
                attacker.SetTarget(GetRandomEnemyIndexInAllBattlers());
            }
            BattleEntities target = allBattlers[attacker.TargetIndex];
            if (target.CurrentHealth > 0)
            {
                AttackAction(attacker, target);
                yield return new WaitForSeconds(TURN_DURATION);
            }

            if (target.CurrentHealth <= 0)
            {
                bottomTextPopUpText.text = String.Format(COMSTR_DEFEATED, target.Name, attacker.Name);
                yield return new WaitForSeconds(TURN_DURATION);
                enemyBattlers.Remove(target);
                allBattlers.Remove(target);
            }

            if (enemyBattlers.Count <= 0)
            {
                state = BattleState.Won;
                bottomTextPopUpText.text = COMSTR_BATTLE_WON;
                bottomTextPopUp.SetActive(true);
                yield return new WaitForSeconds(TURN_DURATION);
                bottomTextPopUp.SetActive(false);
                SceneManager.LoadScene(COMSTR_OVERWORLD_SCENE);
            }
        }
    }

    private IEnumerator EnemyAttackRoutine(int i)
    {
        if (!allBattlers[i].IsPlayer)
        {
            BattleEntities attacker = allBattlers[i];
            attacker.SetTarget(GetRandomPartyMemberIndexInAllBattlers());
            BattleEntities target = allBattlers[attacker.TargetIndex];
            if (target.CurrentHealth > 0)
            {
                AttackAction(attacker, target);
                yield return new WaitForSeconds(TURN_DURATION);
            }

            if (target.CurrentHealth <= 0)
            {
                bottomTextPopUpText.text = String.Format(COMSTR_DEFEATED, target.Name, attacker.Name);
                yield return new WaitForSeconds(TURN_DURATION);
                playerBattlers.Remove(target);
                allBattlers.Remove(target);
            }

            if (playerBattlers.Count <= 0)
            {
                state = BattleState.Lost;
                bottomTextPopUpText.text = COMSTR_BATTLE_LOST;
                bottomTextPopUp.SetActive(true);
                battleMenu.SetActive(false);
                enemySelectionMenu.SetActive(false);
                yield return new WaitForSeconds(TURN_DURATION);
                bottomTextPopUp.SetActive(false);
            }
        }
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
        }
        else
        {
            enemySelectionMenu.SetActive(false);
            ShowBattkeMenu();
        }
    }

    public void SelectRun()
    {
        BattleEntities currentPlayerEntity = playerBattlers[_currentPlayerIndex];
        currentPlayerEntity.BattleAction = BattleEntities.Action.Run;
        _currentPlayerIndex++;

        if (_currentPlayerIndex >= playerBattlers.Count)
        {
            _currentPlayerIndex = 0;
            StartCoroutine(BattleRoutine());
        }
        else
        {
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

    private int GetRandomPartyMemberIndexInAllBattlers()
    {
        int randomIndex = UnityEngine.Random.Range(0, playerBattlers.Count);
        return allBattlers.IndexOf(playerBattlers[randomIndex]);
    }

    private int GetRandomEnemyIndexInAllBattlers()
    {
        int randomIndex = UnityEngine.Random.Range(0, enemyBattlers.Count);
        return allBattlers.IndexOf(enemyBattlers[randomIndex]);
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