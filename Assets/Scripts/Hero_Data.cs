using UnityEngine;

public class Hero_Data : MonoBehaviour
{
    public enum HeroType { Morgan, Grace, Lucas }
    public HeroType heroType;

    // Shared properties
    public float attackRange;
    public float maxAttackRange;
    public float skillRange;
    public LayerMask targetLayer;
    public GameObject target;

    public float damage;
    public float attackSpeed;
    public float skillCooldown;
    public float skillDuration;

    private float timer;
    private float skillTimer;

    [Header("Morgan Data")]
    public GameObject bloodBolt;
    public GameObject bloodFlame;

    [Header("Grace Data")]
    public bool BuffFieldIsActive;

    [Header("Lucas Data")]
    public GameObject lucasArrow;
    public GameObject stunArrow;
    [HideInInspector] public Vector2 stunLocation;

    private void Start()
    {
        skillTimer = skillCooldown;
    }

    private void Update()
    {
        target = GetComponent<Player_Movement>().target;
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
        if (target && target.layer != gameObject.layer)
        {
            float dist = Vector3.Distance(target.transform.position, transform.position);
            if (dist <= attackRange && timer >= attackSpeed)
            {
                GetComponent<Player_Movement>().anim.SetBool("isMoving", false);

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

        if (skillTimer >= skillCooldown && Input.GetMouseButtonDown(1))
        {
            PerformSkill();
            skillTimer = 0;
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
            GetComponent<Player_Movement>().anim.Play("Attack_Magic");
        }
    }

    private void PerformGraceAttack()
    {
        GetComponent<Player_Movement>().anim.Play("Attack_Normal");
    }

    private void PerformLucasAttack()
    {
        if (lucasArrow)
        {
            GetComponent<Player_Movement>().anim.Play("Attack_Bow_Lucas");
        }
    }

    private void PerformSkill()
    {
        switch (heroType)
        {
            case HeroType.Morgan:
                GetComponent<Player_Movement>().anim.Play("Skill_Magic");
                break;
            case HeroType.Grace:
                BuffFieldIsActive = true;
                transform.GetChild(2).GetComponent<SpriteRenderer>().sortingOrder = 6;
                transform.GetChild(2).GetComponent<Animator>().Play("graceskillanim");

                transform.GetChild(1).gameObject.SetActive(true);
                transform.GetChild(1).GetComponent<AttackSkillTriggers>().StartGraceSkill();
                break;
            case HeroType.Lucas:
                stunLocation = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                float distance = Vector2.Distance(transform.position, stunLocation);
                if(distance <= skillRange)
                {
                    GetComponent<Player_Movement>().anim.Play("Skill_Bow");
                }
                else
                {
                    skillTimer = skillCooldown;
                }
                break;
        }
    }

    private void CheckTargetIsAlive()
    {
        if (target)
        {
            if (target.GetComponent<UnitData>().HP <= 0)
            {
                target = null;
                GetComponent<Player_Movement>().target = null;
                GetComponent<Player_Movement>().location = transform.position;
                GetNewTargetInRange();
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

    private void GetNewTargetInRange()
    {
        float searchRange = maxAttackRange;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, searchRange, targetLayer);
        GameObject nearestTarget = null;
        float shortestDistance = Mathf.Infinity;

        foreach (var collider in colliders)
        {
            float distance = Vector2.Distance(transform.position, collider.transform.position);
            if (distance < shortestDistance && !collider.GetComponent<UnitData>().isDefeated)
            {
                shortestDistance = distance;
                nearestTarget = collider.gameObject;
            }
        }

        GetComponent<Player_Movement>().target = nearestTarget;
        target = nearestTarget;
    }
}

