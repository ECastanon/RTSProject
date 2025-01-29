using UnityEngine;

public class Projectile : MonoBehaviour
{
    private float damage;
    public float speed;
    public float lifeTime;
    private float timer;

    public Transform target;
    private LayerMask targetLayer;
    private Vector2 dir;

    public bool lookAtTarget;
    public bool canDamageStructures;

    private void Update()
    {
        if (!target)
        {
            gameObject.SetActive(false);
        }
        else
        {
            TimerDeath();
            MoveTowardsTarget();
        }
    }

    private void MoveTowardsTarget()
    {
        // Calculate direction towards the target
        dir = (target.position - transform.position).normalized;

        // Move the projectile in the calculated direction
        transform.position += (Vector3)(dir * speed * Time.deltaTime);

        if (lookAtTarget)
        {
            LookAtTarget();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the collided object's layer matches the target layer
        if (((1 << other.gameObject.layer) & targetLayer) != 0)
        {
            UnitData unitData = other.gameObject.GetComponent<UnitData>();
            UnitSpawner unitSpawner = other.gameObject.GetComponent<UnitSpawner>();

            if (unitData != null)
            {
                unitData.RecieveDamage(damage);
            }
            else if (canDamageStructures && unitSpawner != null)
            {
                unitSpawner.UpdateHP(damage);
            }

            gameObject.SetActive(false);
        }
    }


    private void TimerDeath()
    {
        timer += Time.deltaTime;

        if (timer > lifeTime || target == null || !target.gameObject.activeSelf)
        {
            gameObject.SetActive(false);
            return;
        }

        UnitData unitData = target.GetComponent<UnitData>();
        UnitSpawner unitSpawner = target.GetComponent<UnitSpawner>();

        if ((unitData != null && unitData.HP <= 0) || (unitSpawner != null && unitSpawner.hp <= 0))
        {
            gameObject.SetActive(false);
        }
    }


    public void GetData(float damage_, Transform target_, LayerMask layer_)
    {
        damage = damage_;
        target = target_;
        targetLayer = layer_;
    }

    private void LookAtTarget()
    {
        Vector3 diff = target.position - transform.position;
        diff.Normalize();
        float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rot_z);
    }
}
