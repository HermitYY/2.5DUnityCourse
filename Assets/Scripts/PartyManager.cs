using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyManager : MonoBehaviour
{
    [SerializeField] private PartyMemberInfo[] allMembers;
    [SerializeField] private List<PartyMember> currentParty;
    [SerializeField] private PartyMemberInfo defaultPartyMember;

    [SerializeField] private Vector3 PlayerPosition;
    private static PartyManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            AddMemberToPartyByName(defaultPartyMember.MemberName);
        }
        DontDestroyOnLoad(gameObject);
    }
    public void AddMemberToPartyByName(string memberName)
    {
        for (int i = 0; i < allMembers.Length; i++)
        {
            if (allMembers[i].MemberName == memberName)
            {
                PartyMember newPartyMember = new PartyMember();
                newPartyMember.MemberName = allMembers[i].MemberName;
                newPartyMember.Level = allMembers[i].staringLevel;
                newPartyMember.CurHealth = allMembers[i].BaseHealth;
                newPartyMember.MaxHealth = newPartyMember.CurHealth;
                newPartyMember.Strength = allMembers[i].BaseStrength;
                newPartyMember.Initiative = allMembers[i].BaseInitiative;
                newPartyMember.CurExp = 0;
                newPartyMember.MaxExp = 100;
                newPartyMember.MemberBattleVisualPrefab = allMembers[i].MemberBattleVisualPrefab;
                newPartyMember.MemberOverworldVisualPrefab = allMembers[i].MemberOverworldVisualPrefab;
                currentParty.Add(newPartyMember);
            }
        }
    }

    public List<PartyMember> GetCurrentAlivePartyMembers()
    {
        // 生命值大于0的才能出战
        List<PartyMember> aliveParty = new List<PartyMember>(currentParty);
        aliveParty.RemoveAll(member => member.CurHealth <= 0);
        return aliveParty;
    }

    public List<PartyMember> GetCurrentPartyMembers()
    {
        return currentParty;
    }

    public void SaveHealthByNameList(List<(string name, int health)> healthDataList)
    {
        // 转为 Dictionary 提高效率
        Dictionary<string, int> healthMap = new Dictionary<string, int>();
        foreach (var data in healthDataList)
        {
            healthMap[data.name] = data.health;
        }

        foreach (PartyMember member in currentParty)
        {
            if (healthMap.TryGetValue(member.MemberName, out int health))
            {
                member.CurHealth = health;
            }
            else
            {
                member.CurHealth = 0;
            }
        }
    }

    public void SavePosition(Vector3 position)
    {
        PlayerPosition = position;
    }

    public Vector3 GetSavedPosition()
    {
        return PlayerPosition;
    }
}

[System.Serializable]
public class PartyMember
{
    public string MemberName;
    public int Level;
    public int CurHealth;
    public int MaxHealth;
    public int Strength;
    public int Initiative;
    public int CurExp;
    public int MaxExp;
    public GameObject MemberBattleVisualPrefab;
    public GameObject MemberOverworldVisualPrefab;
}
