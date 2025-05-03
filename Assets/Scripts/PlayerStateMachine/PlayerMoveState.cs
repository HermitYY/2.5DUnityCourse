using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveState : PlayerState
{

    private int stepsInGrass = 0;
    private bool movingInGrass;
    private float stepTimer;
    private const float timePerStep = .5f;

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

        if (xInput != 0 && xInput < 0)
        {
            player.playerSprite.flipX = true;
        }
        else if (xInput != 0 && xInput > 0)
        {
            player.playerSprite.flipX = false;
        }
    }

    public override void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * player.speed * Time.fixedDeltaTime);

        Collider[] collideds = Physics.OverlapSphere(player.transform.position, 1, player.grassLayer);
        movingInGrass = collideds.Length > 0 && movement != Vector3.zero;
        if (movingInGrass)
        {
            stepTimer += Time.fixedDeltaTime;
            if (stepTimer >= timePerStep)
            {
                stepsInGrass++;
                stepTimer = 0;
                Debug.Log("Steps in grass: " + stepsInGrass);
                if (stepsInGrass >= 5)
                {
                    Debug.Log("Player has walked 5 steps in grass!");
                }
            }
        }
        else
        {
            stepsInGrass = 0;
            stepTimer = 0;
        }
    }
}
