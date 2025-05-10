using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Companion : MonoBehaviour
{
    public PartyMemberInfo MemberTojoin;
    [SerializeField] private GameObject interactPrompt;

    // Start is called before the first frame update
    void Start()
    {
        CheckIfJoinEd();
        interactPrompt.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ShowInteractPrompt(bool show)
    {
        if (show)
        {
            interactPrompt.SetActive(true);
        }
        else
        {
            interactPrompt.SetActive(false);
        }
    }

    public void CheckIfJoinEd()
    {
        List<PartyMember> currentParty = FindAnyObjectByType<PartyManager>().GetCurrentPartyMembers();
        for (int i = 0; i < currentParty.Count; i++)
        {
            if (currentParty[i].MemberName == MemberTojoin.MemberName)
            {
                ShowInteractPrompt(false);
                Destroy(this.gameObject);
                return;
            }
        }
    }
}
