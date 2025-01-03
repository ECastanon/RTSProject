using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DebuffedOriginalStats
{
    public GameObject unit;
    public float oldSpeed;
    public float oldAttackSpeed;
}

public class StunField : MonoBehaviour
{
    public List<DebuffedOriginalStats> debuffedEnemies = new List<DebuffedOriginalStats>();
    public LayerMask targetLayer;
    public float duration = 5;
    private float timer;

    private void OnEnable()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, GetComponent<CircleCollider2D>().radius, targetLayer);
        foreach (var hit in hitColliders)
        {
            AddToDebuffList(hit.gameObject);
        }
    }

    private void Update()
    {
        if (timer >= duration)
        {
            RestoreOriginalStats();
        }
        timer += Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & targetLayer) != 0)
        {
            AddToDebuffList(other.gameObject);
        }
    }

    private void AddToDebuffList(GameObject unit)
    {
        if (unit == null || debuffedEnemies.Exists(x => x.unit == unit))
        {
            return;
        }

        DebuffedOriginalStats originalStats = new DebuffedOriginalStats { unit = unit };

        if (unit.GetComponent<Player_Movement>())
        {
            originalStats.oldSpeed = unit.GetComponent<Player_Movement>().moveSpeed;
            originalStats.oldAttackSpeed = unit.GetComponent<Hero_Data>().attackSpeed;
        }
        else if (unit.GetComponent<AIAgent>())
        {
            originalStats.oldSpeed = unit.GetComponent<AIAgent>().moveSpeed;
            originalStats.oldAttackSpeed = unit.GetComponent<UnitCombat_Data>().attackSpeed;
        }

        debuffedEnemies.Add(originalStats);
        ApplyDebuff(unit);
    }

    private void ApplyDebuff(GameObject unit)
    {
        if (unit.GetComponent<Player_Movement>())
        {
            unit.GetComponent<Player_Movement>().moveSpeed /= 4;
            unit.GetComponent<Hero_Data>().attackSpeed *= 2;
        }
        else if (unit.GetComponent<AIAgent>())
        {
            unit.GetComponent<AIAgent>().moveSpeed /= 4;
            unit.GetComponent<UnitCombat_Data>().attackSpeed *= 2;
        }
    }

    private void RestoreOriginalStats()
    {
        foreach (DebuffedOriginalStats debuffed in debuffedEnemies)
        {
            if (debuffed.unit == null) continue;

            if (debuffed.unit.GetComponent<Player_Movement>())
            {
                debuffed.unit.GetComponent<Player_Movement>().moveSpeed = debuffed.oldSpeed;
                debuffed.unit.GetComponent<Hero_Data>().attackSpeed = debuffed.oldAttackSpeed;
            }
            else if (debuffed.unit.GetComponent<AIAgent>())
            {
                debuffed.unit.GetComponent<AIAgent>().moveSpeed = debuffed.oldSpeed;
                debuffed.unit.GetComponent<UnitCombat_Data>().attackSpeed = debuffed.oldAttackSpeed;
            }
        }

        debuffedEnemies.Clear();
        gameObject.SetActive(false);
    }

    public void ReceiveData(LayerMask layers, float skillTime)
    {
        targetLayer = layers;
        duration = skillTime;
    }
}
