using UnityEngine;

public class Projectile : MonoBehaviour
{
    private float damage;
    public float speed;
    public float lifeTime;
    private float timer;

    private Transform target;
    private LayerMask targetLayer;
    private Vector2 dir;

    public bool lookAtTarget;

    private void Update()
    {
        TimerDeath();
        MoveTowardsTarget();
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
            if (other.gameObject.GetComponent<UnitData>())
            {
                other.gameObject.GetComponent<UnitData>().RecieveDamage(damage);
            }
            gameObject.SetActive(false);
        }
    }

    private void TimerDeath()
    {
        timer += Time.deltaTime;
        if (timer > lifeTime || !target.gameObject.activeSelf || target.GetComponent<UnitData>().HP <= 0)
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
