using UnityEngine;

public class AICombatData : MonoBehaviour
{
    public enum HeroType { Morgan, Grace, Lucas }
    public HeroType heroType;

    private AIHeroMovement aiHero;

    // Shared properties
    public float attackRange;
    public float maxAttackRange;
    public float skillRange;
    public LayerMask targetLayer;
    public GameObject target;

    public float damage;
    public float attackSpeed;

    public bool skillConditionMet;
    public float skillCooldown;
    public float skillDuration;

    private float timer;
    private float skillTimer;

    [Header("Morgan Data")]
    public GameObject bloodBolt;
    public GameObject bloodFlame;

    [Header("Grace Data")]
    public int nearbySoldiers;
    public bool BuffFieldIsActive;

    [Header("Lucas Data")]
    public GameObject lucasArrow;
    public GameObject stunArrow;
    [HideInInspector] public Vector2 stunLocation;

    private void Start()
    {
        aiHero = GetComponent<AIHeroMovement>();
        skillTimer = skillCooldown;
        target = aiHero.target.gameObject;
    }

    private void Update()
    {
        CheckForTargetsInRange();
        Attack();
        CheckTargetIsAlive();

        //Grace Buff Field
        if (BuffFieldIsActive && transform.GetChild(1).localScale != new Vector3(4, 4, 1))
        {
            transform.GetChild(1).localScale = Vector3.MoveTowards(transform.GetChild(1).transform.localScale, new Vector3(4, 4, 1), 4f * Time.deltaTime);
        }
    }

    private void OnDrawGizmos()
    {
        UnityEditor.Handles.color = Color.red;
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.back, attackRange);
        UnityEditor.Handles.color = Color.blue;
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.back, skillRange);
        UnityEditor.Handles.color = Color.gray;
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.back, maxAttackRange);
    }

    private void Attack()
    {
        if (target && target.layer != gameObject.layer && aiHero.state == AIHeroMovement.State.Attacking)
        {
            float dist = Vector3.Distance(target.transform.position, transform.position);
            if (dist <= attackRange && timer >= attackSpeed)
            {
                aiHero.anim.SetBool("isMoving", false);

                switch (heroType)
                {
                    case HeroType.Morgan:
                        PerformMorganAttack();
                        break;
                    case HeroType.Grace:
                        PerformGraceAttack();
                        break;
                    case HeroType.Lucas:
                        PerformLucasAttack();
                        break;
                }

                timer = 0;
            }
        }

        if (skillTimer >= skillCooldown)
        {
            bool conditionMet = false;
            switch (heroType)
            {
                case HeroType.Morgan:
                    if(GetNearestAlly())
                    {
                        conditionMet = true;
                    }
                    break;
                case HeroType.Grace:
                    if (nearbySoldiers > 2)
                    {
                        conditionMet = true;
                    }
                    break;
                case HeroType.Lucas:
                    if (true)
                    {
                        conditionMet = true;
                    }
                    break;
            }
            if (conditionMet)
            {
                PerformSkill();
                skillTimer = 0;
            }
        }

        timer += Time.deltaTime;
        skillTimer += Time.deltaTime;
    }

    private void PerformMorganAttack()
    {
        if (bloodBolt)
        {
            GameObject proj = Instantiate(bloodBolt, transform.position, Quaternion.identity);
            proj.GetComponent<Projectile>().GetData(damage, target.transform, targetLayer);
            aiHero.anim.Play("Attack_Magic");
        }
    }

    private void PerformGraceAttack()
    {
        aiHero.anim.Play("Attack_Normal");
    }

    private void PerformLucasAttack()
    {
        if (lucasArrow)
        {
            aiHero.anim.Play("Attack_Bow_Lucas");
        }
    }

    private void PerformSkill()
    {
        switch (heroType)
        {
            case HeroType.Morgan:
                aiHero.anim.Play("Skill_Magic");
                break;

        }
    }

    private void CheckTargetIsAlive()
    {
        if (target)
        {
            UnitData targetData = target.GetComponent<UnitData>();
            if (targetData && targetData.HP <= 0)
            {
                target = null;
                aiHero.target = null;
                aiHero.state = AIHeroMovement.State.Default;
            }
        }
    }


    public void DealDamage()
    {
        if (target)
        {
            target.gameObject.GetComponent<UnitData>().RecieveDamage(damage);
        }
    }

    private void CheckForTargetsInRange()
    {
        if (aiHero.state == AIHeroMovement.State.Default)
        {
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, attackRange, targetLayer);
            foreach (var hit in hitColliders)
            {
                aiHero.state = AIHeroMovement.State.Attacking;
                aiHero.target = hit.gameObject.transform;
                target = aiHero.target.gameObject;
                break; // Assign only the first target found
            }
        }
    }

    private GameObject GetNearestAlly()
    {
        int layerMask = 1 << gameObject.layer; // Convert the layer number to a layer mask
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, skillRange, layerMask);
        GameObject nearestAlly = null;
        float shortestDistance = Mathf.Infinity;

        foreach (var collider in colliders)
        {
            if (collider.gameObject != gameObject) // Ensure it's not the object itself
            {
                float distance = Vector2.Distance(transform.position, collider.transform.position);
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    nearestAlly = collider.gameObject;
                }
            }
        }
        return nearestAlly;
    }
}