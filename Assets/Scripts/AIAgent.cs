using Pathfinding;
using System.Collections.Generic;
using UnityEngine;

public class AIAgent : MonoBehaviour
{
    private AIPath aipath;
    public List<Transform> path = new List<Transform>();
    public int pathCounter;
    public float moveSpeed;
    public Transform primaryTarget;
    public Transform target;

    private Animator anim;

    void Start()
    {
        anim = transform.GetChild(0).GetComponent<Animator>();
        aipath = GetComponent<AIPath>();
    }

    void Update()
    {
        if (!GetComponent<UnitData>().isDefeated)
        {
            if (target)
            {
                FollowTarget();
            }
            else if (path.Count > 0)
            {
                FollowPath();
            }

            CheckMoveDirection();
        } else
        {
            aipath.canMove = false;
        }
    }

    private void FollowTarget()
    {
        float targetDist = Vector3.Distance(transform.position, target.position);
        var combatData = GetComponent<UnitCombat_Data>();

        if (combatData && targetDist > combatData.attackRange)
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

    private void FollowPath()
    {
        if (pathCounter < path.Count)
        {
            float dist = Vector3.Distance(transform.position, path[pathCounter].position);
            if (dist <= 1f && pathCounter < path.Count - 1)
            {
                pathCounter++;
                MoveTo(transform.position);
                anim?.SetBool("isMoving", false);
            }
            MoveTo(path[pathCounter].position);
            anim?.SetBool("isMoving", true);
        }
    }

    private void CheckMoveDirection()
    {
        Transform targetTransform = target ?? (path.Count > 0 ? path[pathCounter] : null);

        if (targetTransform != null)
        {
            float direction = targetTransform.position.x - transform.position.x;
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

