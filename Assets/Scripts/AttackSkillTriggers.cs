using System.Collections;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class AttackSkillTriggers : MonoBehaviour
{
    //=========================================================================
    //  HEROES
    //=========================================================================

    //-------------------------------------------------------------------------
    //  MORGAN
    //-------------------------------------------------------------------------
    public void SacrificeAlly() //Morgan Skill
    {
        StartCoroutine(Sacrifice());
    }
    private IEnumerator Sacrifice()
    {
        GameObject target = GetNearestAlly();
        GameObject flame = Instantiate(transform.parent.GetComponent<Hero_Data>().bloodFlame, target.transform.transform.position, Quaternion.identity);
        flame.GetComponent<FollowCharacter>().myParent = target.transform;
        yield return new WaitForSeconds(0.5f);
        target.transform.GetComponent<UnitData>().HP = 0;
        StartCoroutine(ActiveBuffs());
        flame.SetActive(false);
    }
    private GameObject GetNearestAlly()
    {
        int layerMask = 1 << transform.parent.gameObject.layer; // Convert the layer number to a layer mask
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.parent.position, transform.parent.GetComponent<Hero_Data>().skillRange, layerMask);
        GameObject nearestAlly = null;
        float shortestDistance = Mathf.Infinity;

        foreach (var collider in colliders)
        {
            if (collider.gameObject != transform.parent.gameObject) // Ensure it's not the object itself
            {
                float distance = Vector2.Distance(transform.parent.position, collider.transform.position);
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    nearestAlly = collider.gameObject;
                }
            }
        }

        Debug.Log(nearestAlly);
        return nearestAlly;
    }
    private IEnumerator ActiveBuffs()
    {
        //Heals 50% of their HP
        transform.parent.GetComponent<UnitData>().HP += (int)(transform.parent.GetComponent<UnitData>().maxHP / 2);
        if (transform.parent.GetComponent<UnitData>().HP > transform.parent.GetComponent<UnitData>().maxHP) { transform.parent.GetComponent<UnitData>().HP = transform.parent.GetComponent<UnitData>().maxHP; }

        float oldDamage = transform.parent.GetComponent<Hero_Data>().damage;
        float oldAS = transform.parent.GetComponent<Hero_Data>().attackSpeed;
        float oldMS = transform.parent.GetComponent<Player_Movement>().moveSpeed;

        //Increase Stats
        transform.parent.GetComponent<Hero_Data>().damage *= 1.2f;
        transform.parent.GetComponent<Hero_Data>().attackSpeed *= 0.5f;
        transform.parent.GetComponent<Player_Movement>().moveSpeed *= 1.5f;

        yield return new WaitForSeconds(transform.parent.GetComponent<Hero_Data>().skillDuration);

        transform.parent.GetComponent<Hero_Data>().damage = oldDamage;
        transform.parent.GetComponent<Hero_Data>().attackSpeed = oldAS;
        transform.parent.GetComponent<Player_Movement>().moveSpeed = oldMS;
    }

    //-------------------------------------------------------------------------
    //  GRACE
    //-------------------------------------------------------------------------

    public void ResetBuffAnim()
    {
        GetComponent<SpriteRenderer>().sortingOrder = -2;
    }

    public void StartGraceSkill()
    {
        StartCoroutine(StartBuffSkill());
    }
    public IEnumerator StartBuffSkill()
    {
        transform.localScale = new Vector2(0.5f, 0.5f);
        yield return new WaitForSeconds(transform.parent.GetComponent<Hero_Data>().skillDuration);
        gameObject.SetActive(false);
    }

    //-------------------------------------------------------------------------
    //  Lucas
    //-------------------------------------------------------------------------

    public void SpawnLucasProjectile()
    {
        Hero_Data hero = transform.parent.GetComponent<Hero_Data>();
        GameObject proj = Instantiate(hero.lucasArrow, transform.position, Quaternion.identity);
        proj.GetComponent<Projectile>().GetData(hero.damage, hero.target.transform, hero.targetLayer);
    }

    public void SpawnLucasSkill()
    {
        Hero_Data hero = transform.parent.GetComponent<Hero_Data>();
        GameObject proj = Instantiate(hero.stunArrow, transform.position, Quaternion.identity);
        proj.GetComponent<StunArrow>().GetData(hero.stunLocation, hero.skillDuration, hero.targetLayer);
    }

    //=========================================================================
    //  UNITS
    //=========================================================================

    //-------------------------------------------------------------------------
    //  Archer
    //-------------------------------------------------------------------------
    public void SpawnProjectile()
    {
        GameObject proj = Instantiate(transform.parent.GetComponent<UnitCombat_Data>().arrowPrefab, transform.parent.position, Quaternion.identity);
        proj.GetComponent<Projectile>().GetData(transform.parent.GetComponent<UnitCombat_Data>().damage, transform.parent.GetComponent<UnitCombat_Data>().target, transform.parent.GetComponent<UnitCombat_Data>().targetLayer);
    }

    //-------------------------------------------------------------------------
    //  SwordFighter
    //-------------------------------------------------------------------------
    public void DealDamage()
    {
        if (transform.parent.GetComponent<UnitCombat_Data>())
        {
            transform.parent.GetComponent<UnitCombat_Data>().DealDamage();

        }
        if (transform.parent.GetComponent<Hero_Data>())
        {
            transform.parent.GetComponent<Hero_Data>().DealDamage();
        }
    }
}
