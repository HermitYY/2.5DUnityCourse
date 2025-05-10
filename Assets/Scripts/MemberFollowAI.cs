using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemberFollowAI : MonoBehaviour
{
    [SerializeField] private Transform followTarget;
    [SerializeField] private int speed;

    private float followDist;
    private Animator anim;
    private SpriteRenderer spriteRenderer;
    void Start()
    {
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        followTarget = FindAnyObjectByType<PlayerController>().transform;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Vector3.Distance(transform.position, followTarget.position) > followDist)
        {
            Vector3 direction = (followTarget.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;

            if (direction.x < 0)
            {
                spriteRenderer.flipX = true;
            }
            else
            {
                spriteRenderer.flipX = false;
            }

            anim.SetBool("IsWalk", true);
        }
        else
        {
            anim.SetBool("IsWalk", false);
        }
    }

    public void SetFollowDistance(float distance)
    {
        followDist = distance;
    }
}
