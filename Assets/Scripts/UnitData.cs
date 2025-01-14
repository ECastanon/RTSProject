using HighlightPlus;
using Pathfinding;
using System.Collections;
using UnityEngine;

public class UnitData : MonoBehaviour
{
    private AIHeroMovement aiHero;

    public float maxHP;
    public float HP;

    public bool isHero;

    public int deathTimer = 3;

    public bool isDefeated;

    public int defeatValue;

    private void Start()
    {
        StartUnit();
    }

    private void StartUnit()
    {
        HP = maxHP;
        isDefeated = false;
        if (GetComponent<AIHeroMovement>()) { aiHero = GetComponent<AIHeroMovement>(); }
        
    }

    private void Update()
    {
        if(aiHero && HP <= maxHP / 4)
        {
            aiHero.state = AIHeroMovement.State.Fleeing;
        }

        if(HP <= 0)
        {
            if (isHero)
            {
                Respawn();
            }
            else
            {
                DeathEvent();
            }
        }
    }

    private void Respawn()
    {
        HP = maxHP;
        deathTimer += 3;
        if(deathTimer > 15) 
        {
            deathTimer = 15;
        }
    }

    public void RecieveDamage(float damage, bool display = true)
    {
        HP -= damage;

        if (HP > maxHP)
        {
            HP = maxHP;
            if (GetComponent<AIHeroMovement>()) { GetComponent<AIHeroMovement>().state = AIHeroMovement.State.Default; }
            return;
        }

        if (display)
        {
            HighlightEffect effect = transform.GetChild(0).GetChild(0).GetComponent<HighlightEffect>();
            effect.HitFX();
        }
    }

    void OnMouseOver()
    {
        GameObject.Find("GameManager").GetComponent<MouseController>().hoveredCharacter = gameObject;
    }
    void OnMouseExit()
    {
        GameObject.Find("GameManager").GetComponent<MouseController>().hoveredCharacter = null;
    }

    private IEnumerator DeathStun()
    {
        transform.GetChild(0).GetComponent<Animator>().Play("DEATH");
        DisableAllCircleColliders();
        if (!isHero)
        {
            GetComponent<AIAgent>().moveSpeed = 0;
        }
        yield return new WaitForSeconds(deathTimer);
        //StartUnit();
        gameObject.SetActive(false);
    }

    public void DisableAllCircleColliders()
    {
        CircleCollider2D[] colliders = GetComponents<CircleCollider2D>();
        foreach (CircleCollider2D collider in colliders)
        {
            collider.enabled = false;
        }
    }

    private void DeathEvent()
    {
        if (!isDefeated)
        {
            isDefeated = true;
            GameObject.Find("GameManager").GetComponent<GoldManager>().AddSubtractGold(defeatValue);
            StartCoroutine(DeathStun());
        }
    }
}
