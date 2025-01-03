using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UnitBuffsFromGrace
{
    public float oldDamage;
    public float oldSpeed;
    public float attackSpeed;
}

public class GraceBuffs : MonoBehaviour
{
    public GameObject grace;
    public List<GameObject> allies = new List<GameObject>();
    public UnitBuffsFromGrace[] buffs = new UnitBuffsFromGrace[50];
    private Dictionary<GameObject, int> unitToIndex = new Dictionary<GameObject, int>();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject unit = collision.gameObject;
        if ((unit.GetComponent<UnitCombat_Data>()) && unit.layer == grace.layer)
        {
            if (!allies.Contains(unit))
            {
                allies.Add(unit);
                int index = allies.IndexOf(unit); // Use the index in allies list
                unitToIndex[unit] = index;

                buffs[index] = new UnitBuffsFromGrace(); // Initialize the buff
                EnableBuffs(unit);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (allies.Contains(collision.gameObject))
        {
            DisableBuffs(collision.gameObject);
            unitToIndex.Remove(collision.gameObject);
            allies.Remove(collision.gameObject);
        }
    }

    private void EnableBuffs(GameObject unit)
    {
        int index = unitToIndex[unit];

        unit.transform.GetChild(1).gameObject.SetActive(true);

        buffs[index].oldSpeed = unit.GetComponent<AIAgent>().moveSpeed;
        unit.GetComponent<AIAgent>().moveSpeed *= 1.5f;

        if (unit.GetComponent<UnitCombat_Data>())
        {
            buffs[index].oldDamage = unit.GetComponent<UnitCombat_Data>().damage;
            unit.GetComponent<UnitCombat_Data>().damage = Mathf.Ceil(unit.GetComponent<UnitCombat_Data>().damage * 1.3f);

            buffs[index].attackSpeed = unit.GetComponent<UnitCombat_Data>().attackSpeed;
            unit.GetComponent<UnitCombat_Data>().attackSpeed -= 1;
        }
    }

    private void DisableBuffs(GameObject unit)
    {
        int index = unitToIndex[unit];

        unit.transform.GetChild(1).gameObject.SetActive(false);

        unit.GetComponent<AIAgent>().moveSpeed = buffs[index].oldSpeed;

        if (unit.GetComponent<UnitCombat_Data>())
        {
            unit.GetComponent<UnitCombat_Data>().damage = buffs[index].oldDamage;
            unit.GetComponent<UnitCombat_Data>().attackSpeed = buffs[index].attackSpeed;
        }

        buffs[index] = null; // Clear the buff
    }
}
