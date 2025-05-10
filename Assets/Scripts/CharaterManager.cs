using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharaterManager : MonoBehaviour
{
    private bool infrontOfpartyMember;
    private GameObject joinableMember;
    private PlayerControls playerControls;
    private List<GameObject> overworldCharacters = new List<GameObject>();

    private const string COMSTR_NPC_JOIN_TAG = "NPCJoinable";

    private void Awake()
    {
        playerControls = new PlayerControls();
    }

    void Start()
    {
        playerControls.Player.Interact.performed += _ => Interact();
        SpawnOverworldMembers();
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    void Update()
    {

    }

    private void Interact()
    {
        if (infrontOfpartyMember == true && joinableMember != null)
        {
            Companion companion = joinableMember.GetComponent<Companion>();
            if (companion != null)
            {
                MemberJoined(companion.MemberTojoin);
                infrontOfpartyMember = false;
                joinableMember = null;
            }
        }
    }

    private void MemberJoined(PartyMemberInfo partyMember)
    {
        FindAnyObjectByType<PartyManager>().AddMemberToPartyByName(partyMember.MemberName);
        joinableMember.GetComponent<Companion>().CheckIfJoinEd();
        SpawnOverworldMembers();
    }

    private void SpawnOverworldMembers()
    {
        for (int i = 1; i < overworldCharacters.Count; i++)
        {
            Destroy(overworldCharacters[i]);
        }
        overworldCharacters.Clear();
        List<PartyMember> currentParty = FindAnyObjectByType<PartyManager>().GetCurrentPartyMembers();
        for (int i = 0; i < currentParty.Count; i++)
        {
            if (i != 0)
            // {
            //     // GameObject player = gameObject;
            //     // GameObject playerVisual = Instantiate(currentParty[i].MemberOverworldVisualPrefab, player.transform.position, Quaternion.identity);
            //     // playerVisual.transform.SetParent(player.transform);

            //     // player.GetComponent<PlayerController>().SetOverworldVisuals(playerVisual.GetComponent<Animator>(), playerVisual.GetComponent<SpriteRenderer>());
            //     // playerVisual.GetComponent<MemberFollowAI>().enabled = false;
            //     // overworldCharacters.Add(playerVisual);
            // }
            // else
            {
                Vector3 positionToSpawn = transform.position;
                positionToSpawn.x -= 1;
                GameObject tempOverworldMember = Instantiate(currentParty[i].MemberOverworldVisualPrefab, positionToSpawn, Quaternion.identity);
                tempOverworldMember.GetComponent<MemberFollowAI>().SetFollowDistance(i);
                overworldCharacters.Add(tempOverworldMember);
            }

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(COMSTR_NPC_JOIN_TAG))
        {
            infrontOfpartyMember = true;
            joinableMember = other.gameObject;
            joinableMember.GetComponent<Companion>().ShowInteractPrompt(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(COMSTR_NPC_JOIN_TAG))
        {
            infrontOfpartyMember = false;
            Companion companion = other.GetComponent<Companion>();
            if (companion != null)
                companion.ShowInteractPrompt(false);
            else
                Debug.LogWarning("Companion component not found on the exiting object.");
            joinableMember = null;
        }
    }
}
