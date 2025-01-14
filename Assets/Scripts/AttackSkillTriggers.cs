using System.Collections;
using UnityEngine;

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
        if (target == null)
        {
            Debug.LogWarning("No ally found for Sacrifice.");
            yield break;
        }

        GameObject flame = InstantiateFlame(target);
        if (flame == null) yield break;

        yield return new WaitForSeconds(0.5f);

        UnitData targetData = target.GetComponent<UnitData>();
        if (targetData != null)
        {
            targetData.HP = 0;
        }
        else
        {
            Debug.LogWarning("Target does not have a UnitData component.");
        }

        StartCoroutine(ActiveBuffs());
        flame.SetActive(false);
    }

    private GameObject InstantiateFlame(GameObject target)
    {
        GameObject flame = null;
        var parent = transform.parent;
        if (parent.TryGetComponent(out Hero_Data heroData) && heroData.bloodFlame != null)
        {
            flame = Instantiate(heroData.bloodFlame, target.transform.position, Quaternion.identity);
        }
        else if (parent.TryGetComponent(out AICombatData aiCombatData) && aiCombatData.bloodFlame != null)
        {
            flame = Instantiate(aiCombatData.bloodFlame, target.transform.position, Quaternion.identity);
        }

        if (flame == null)
        {
            Debug.LogWarning("Flame object could not be instantiated.");
            return null;
        }

        var followCharacter = flame.GetComponent<FollowCharacter>();
        if (followCharacter != null)
        {
            followCharacter.myParent = target.transform;
        }
        else
        {
            Debug.LogWarning("Flame does not have a FollowCharacter component.");
        }

        return flame;
    }

    private GameObject GetNearestAlly()
    {
        var parent = transform.parent;
        if (parent == null) return null;

        var layerMask = 1 << parent.gameObject.layer;
        var skillRange = parent.GetComponent<Hero_Data>()?.skillRange ?? parent.GetComponent<AICombatData>()?.skillRange ?? 0;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(parent.position, skillRange, layerMask);
        GameObject nearestAlly = null;
        float shortestDistance = Mathf.Infinity;

        foreach (var collider in colliders)
        {
            if (collider.gameObject == parent.gameObject) continue;

            float distance = Vector2.Distance(parent.position, collider.transform.position);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                nearestAlly = collider.gameObject;
            }
        }

        return nearestAlly;
    }

    private IEnumerator ActiveBuffs()
    {
        var parent = transform.parent;
        var unitData = parent.GetComponent<UnitData>();
        var movement = parent.GetComponent<Player_Movement>();

        // Try to retrieve either Hero_Data or AICombatData
        var combatData = (Component)parent.GetComponent<Hero_Data>() ?? parent.GetComponent<AICombatData>();

        if (unitData == null || movement == null || combatData == null) yield break;

        // Heal 50% of max HP
        unitData.HP = Mathf.Min(unitData.HP + (int)(unitData.maxHP / 2), unitData.maxHP);

        // Retrieve shared variables using reflection
        var damageField = combatData.GetType().GetField("damage");
        var attackSpeedField = combatData.GetType().GetField("attackSpeed");
        var skillDurationField = combatData.GetType().GetField("skillDuration");

        if (damageField == null || attackSpeedField == null || skillDurationField == null) yield break;

        float oldDamage = (float)damageField.GetValue(combatData);
        float oldAttackSpeed = (float)attackSpeedField.GetValue(combatData);
        float skillDuration = (float)skillDurationField.GetValue(combatData);

        // Modify stats
        damageField.SetValue(combatData, oldDamage * 1.2f);
        attackSpeedField.SetValue(combatData, oldAttackSpeed * 0.5f);
        float oldMoveSpeed = movement.moveSpeed;
        movement.moveSpeed *= 1.5f;

        // Wait for the skill duration
        yield return new WaitForSeconds(skillDuration);

        // Revert stats
        damageField.SetValue(combatData, oldDamage);
        attackSpeedField.SetValue(combatData, oldAttackSpeed);
        movement.moveSpeed = oldMoveSpeed;
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
    //  LUCAS
    //-------------------------------------------------------------------------

    public void SpawnLucasProjectile()
    {
        if (TryGetComponentFromParent<Hero_Data>(out var hero))
        {
            var proj = Instantiate(hero.lucasArrow, transform.position, Quaternion.identity);
            proj.GetComponent<Projectile>().GetData(hero.damage, hero.target.transform, hero.targetLayer);
        }
    }

    public void SpawnLucasSkill()
    {
        if (TryGetComponentFromParent<Hero_Data>(out var hero))
        {
            var proj = Instantiate(hero.stunArrow, transform.position, Quaternion.identity);
            proj.GetComponent<StunArrow>().GetData(hero.stunLocation, hero.skillDuration, hero.targetLayer);
        }
    }

    //=========================================================================
    //  UNITS
    //=========================================================================

    public void SpawnProjectile()
    {
        if (TryGetComponentFromParent<UnitCombat_Data>(out var combatData))
        {
            var proj = Instantiate(combatData.arrowPrefab, transform.parent.position, Quaternion.identity);
            proj.GetComponent<Projectile>().GetData(combatData.damage, combatData.target, combatData.targetLayer);
        }
    }

    public void DealDamage()
    {
        if (TryGetComponentFromParent<UnitCombat_Data>(out var combatData))
        {
            combatData.DealDamage();
        }

        if (TryGetComponentFromParent<Hero_Data>(out var hero))
        {
            hero.DealDamage();
        }
    }

    private bool TryGetComponentFromParent<T>(out T component) where T : Component
    {
        component = transform.parent?.GetComponent<T>();
        return component != null;
    }
}