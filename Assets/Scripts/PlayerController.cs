using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] public int speed;
    [SerializeField] public Animator anim;
    [SerializeField] public SpriteRenderer playerSprite;
    [SerializeField] public LayerMask grassLayer;
    public Rigidbody rb;
    public PartyManager partyManager;

    #region States
    public PlayerStateMachine stateMachine { get; private set; }
    public PlayerIdleState idleState { get; private set; }
    public PlayerMoveState moveState { get; private set; }
    #endregion

    private void Awake()
    {
        stateMachine = new PlayerStateMachine();
        idleState = new PlayerIdleState(this, stateMachine, "Idle");
        moveState = new PlayerMoveState(this, stateMachine, "IsWalk");
    }

    private void OnEnable()
    {
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        stateMachine.Initialize(idleState);
        partyManager = FindAnyObjectByType<PartyManager>();
        if (partyManager.GetSavedPosition() != Vector3.zero)
        {
            transform.position = partyManager.GetSavedPosition();
        }
    }

    private void Update()
    {
        stateMachine.currentState.Update();
    }

    private void FixedUpdate()
    {
        stateMachine.currentState.FixedUpdate();
    }
}
