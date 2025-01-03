using UnityEngine;

public class StunArrow : MonoBehaviour
{
    public float speed;
    private float skillTime;
    private Vector3 target;
    private LayerMask layer;
    public GameObject StunField;

    private void Update()
    {
        MoveTowardsTarget();
        LookAtTarget();
    }

    private void MoveTowardsTarget()
    {
        transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);

        float distance = Vector2.Distance(transform.position, target);
        if (distance <= 0.1f)
        {
            GameObject field = Instantiate(StunField, transform.position, Quaternion.identity);
            field.GetComponent<StunField>().ReceiveData(layer, skillTime);
            gameObject.SetActive(false);
        }
    }

    public void GetData(Vector2 target_, float duration, LayerMask layers)
    {
        target = target_;
        skillTime = duration;
        layer = layers;
    }

    private void LookAtTarget()
    {
        Vector3 diff = target - transform.position;
        diff.Normalize();
        float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rot_z);
    }
}
