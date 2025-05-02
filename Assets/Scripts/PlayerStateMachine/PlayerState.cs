using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState
{
    protected PlayerStateMachine stateMachine;
    protected PlayerController player;
    protected Rigidbody rb;
    private string animBoolName;

    private PlayerControls playerControls;
    protected float xInput;
    protected float zInput;
    protected Vector3 movement;


    public PlayerState(PlayerController _player, PlayerStateMachine _stateMachine, string _animBoolName)
    {
        this.player = _player;
        this.stateMachine = _stateMachine;
        this.animBoolName = _animBoolName;

        playerControls = new PlayerControls();
        playerControls.Enable();
    }

    public virtual void Enter()
    {
        player.anim.SetBool(animBoolName, true);
        rb = player.rb;
    }

    public virtual void Update()
    {
        xInput = playerControls.Player.Move.ReadValue<Vector2>().x;
        zInput = playerControls.Player.Move.ReadValue<Vector2>().y;
        movement = new Vector3(xInput, 0, zInput).normalized;
    }

    public virtual void FixedUpdate()
    {
    }

    public virtual void Exit()
    {
        player.anim.SetBool(animBoolName, false);
    }
}
