using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveState : PlayerState
{
    public PlayerMoveState(PlayerController _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (movement == Vector3.zero)
        {
            stateMachine.ChangeState(player.idleState);
        }
    }

    public override void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * player.speed * Time.fixedDeltaTime);

        if (xInput != 0 && xInput < 0)
        {
            player.playerSprite.flipX = true;
        }
        else if (xInput != 0 && xInput > 0)
        {
            player.playerSprite.flipX = false;
        }
    }
}
