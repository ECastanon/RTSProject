using UnityEngine;

public class UnitCombat_Data : MonoBehaviour
{
    public enum UnitType { Archer, SwordFighter }
    public UnitType unitType;

    public float aggressionRange;
    public float attackRange;
    public LayerMask targetLayer;

    public float damage;
    public float attackSpeed;
    private float timer;

    public GameObject arrowPrefab; // Only used for Archer
    [HideInInspector] public Transform target;

    private void Update()
    {
        CheckForTargetsInRange();
        Attack();
        CheckTargetIsAlive();
    }

    private void OnDrawGizmos()
    {
        UnityEditor.Handles.color = Color.yellow;
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.back, aggressionRange);
        UnityEditor.Handles.color = Color.red;
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.back, attackRange);
    }

    private void CheckForTargetsInRange()
    {
        if (!GetComponent<AIAgent>().target)
        {
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, aggressionRange, targetLayer);
            foreach (var hit in hitColliders)
            {
                GetComponent<AIAgent>().target = hit.gameObject.transform;
                break; // Assign only the first target found
            }
        }
    }

    private void Attack()
    {
        target = GetComponent<AIAgent>().target;

        if (target && !GetComponent<UnitData>().isDefeated)
        {
            float dist = Vector3.Distance(target.position, transform.position);
            if (timer >= attackSpeed && dist <= attackRange)
            {
                switch (unitType)
                {
                    case UnitType.Archer:
                        PerformArcherAttack();
                        break;

                    case UnitType.SwordFighter:
                        PerformSwordFighterAttack();
                        break;
                }
                timer = 0;
            }
        }
        timer += Time.deltaTime;
    }

    private void PerformArcherAttack()
    {
        if (arrowPrefab)
        {
            transform.GetChild(0).GetComponent<Animator>().Play("Attack_Bow");
        }
    }

    private void PerformSwordFighterAttack()
    {
        transform.GetChild(0).GetComponent<Animator>().Play("Attack_Normal");
        //Deals Damage during the animation
    }

    private void CheckTargetIsAlive()
    {
        if (target && target.GetComponent<UnitData>().HP <= 0)
        {
            GetComponent<AIAgent>().target = null;
            target = null;
        }
    }

    public void DealDamage()
    {
        if (target)
        {
            target.gameObject.GetComponent<UnitData>().RecieveDamage(damage);
        }
    }
}
