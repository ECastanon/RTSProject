using System.Collections.Generic;
using UnityEngine;

public class HealZone : MonoBehaviour
{
    public LayerMask layerToHeal;
    public int HpPerSecond;

    private float timer;
    public List<UnitData> unitsInside;
    

    private void Update()
    {
        if(timer > 1)
        {
            timer = 0;
            foreach (UnitData unit in unitsInside)
            {
                unit.RecieveDamage(-HpPerSecond, false);
            }
        }
        timer += Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D obj)
    {
        if (((1 << obj.gameObject.layer) & layerToHeal) != 0)
        {
            unitsInside.Add(obj.GetComponent<UnitData>());
        }
    }
    void OnTriggerExit2D(Collider2D obj)
    {
        if (unitsInside.Contains(obj.GetComponent<UnitData>()))
        {
            unitsInside.Remove(obj.GetComponent<UnitData>());
        }
    }
}
