using Pathfinding;
using Unity.VisualScripting;
using UnityEngine;

public class AIHeroMovement: MonoBehaviour
{
    private AIPath aipath;
    [HideInInspector] public Animator anim;

    public enum State { Default, Attacking, Fleeing }
    //Default: AI is running towards the main objective
    //Attacking: AI is attacking
    //Fleeing: AI is running away
    public State state;

    public Transform primaryTarget;
    public Transform recoveryPoint;
    public Transform target;
    public float moveSpeed;

    void Start()
    {
        anim = transform.GetChild(0).GetComponent<Animator>();
        aipath = GetComponent<AIPath>();

        target = primaryTarget;
    }

    void Update()
    {
        switch (state)
        {
            case State.Default:
                target = primaryTarget;
                break;
            case State.Attacking:
                break;
            case State.Fleeing:
                target = recoveryPoint;
                break;
        }

        if (!GetComponent<UnitData>().isDefeated)
        {
            if (target)
            {
                FollowTarget();
            }

            CheckMoveDirection();
        }
        else
        {
            aipath.canMove = false;
        }
    }

    private void FollowTarget()
    {
        float targetDist = Vector3.Distance(transform.position, target.position);
        //var combatData = GetComponent<UnitCombat_Data>();

        //If fleeing move directly onto the targetted object
        if(state == State.Fleeing)
        {
            MoveTo(target.position);
            if (targetDist > 0.1f)
            {
                anim?.SetBool("isMoving", true);
            }
            else
            {
                anim?.SetBool("isMoving", false);
            }
            return;
        }

        if (targetDist > GetComponent<AICombatData>().attackRange)
        {
            MoveTo(target.position);
            anim?.SetBool("isMoving", true);
        }
        else
        {
            MoveTo(transform.position);
            anim?.SetBool("isMoving", false);
        }
    }

    private void CheckMoveDirection()
    {
        if (target != null)
        {
            float direction = target.position.x - transform.position.x;
            transform.localRotation = Quaternion.Euler(0, direction > 0 ? 180 : 0, 0);
        }
        else
        {
            float direction = aipath.destination.x - transform.position.x;
            transform.localRotation = Quaternion.Euler(0, direction > 0 ? 180 : 0, 0);
        }
    }

    private void MoveTo(Vector2 pos)
    {
        aipath.maxSpeed = moveSpeed;
        aipath.destination = pos;
    }
}
