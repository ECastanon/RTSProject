using UnityEngine;

public class UnitCombat_Data : MonoBehaviour
{
    public enum UnitType { Archer, SwordFighter, Mage }
    public UnitType unitType;

    public float aggressionRange;
    public float attackRange;
    public LayerMask targetLayer;

    public float damage;
    public float attackSpeed;
    private float timer;

    public GameObject arrowPrefab; // Only used for Archer
    public Transform target;

    private float updateInterval = 0.25f;
    private void Start()
    {
        InvokeRepeating("UpdateInterval", updateInterval, updateInterval);
    }

    private void UpdateInterval()
    {
        //use this as the secondary update.
        CheckForTargetsInRange();
    }

    private void Update()
    {
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

        //Units cannot attack while defeated
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
                    case UnitType.Mage:
                        PerformSwordMageAttack();
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

    private void PerformSwordMageAttack()
    {
        transform.GetChild(0).GetComponent<Animator>().Play("Attack_Mage");
    }

    private void CheckTargetIsAlive()
    {
        if (target && target.GetComponent<UnitSpawner>())
        {
            if (target.GetComponent<UnitSpawner>().hp <= 0)
            {
                GetComponent<AIAgent>().target = null;
                target = null;
                return;
            }
        } else if (target && target.GetComponent<UnitData>().HP <= 0)
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
