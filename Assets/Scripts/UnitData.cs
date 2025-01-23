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

        if (display && transform.GetChild(0).GetChild(0).GetComponent<HighlightEffect>())
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
        ResetUnit();
        gameObject.SetActive(false);
    }

    private void DisableAllCircleColliders()
    {
        CircleCollider2D[] colliders = GetComponents<CircleCollider2D>();
        foreach (CircleCollider2D collider in colliders)
        {
            collider.enabled = false;
        }
    }
    private void EnableAllCircleColliders()
    {
        CircleCollider2D[] colliders = GetComponents<CircleCollider2D>();
        foreach (CircleCollider2D collider in colliders)
        {
            collider.enabled = true;
        }
    }

    private void DeathEvent()
    {
        if (!isDefeated)
        {
            isDefeated = true;
            GetComponent<AIAgent>().mmapManager.RemoveIconFromMap(GetComponent<AIAgent>().minimapID, GetComponent<AIAgent>().unitType);

            GameObject.Find("GameManager").GetComponent<GoldManager>().AddSubtractGold(defeatValue);
            StartCoroutine(DeathStun());
        }
    }

    private void ResetUnit()
    {
        AIAgent ai = GetComponent<AIAgent>();

        ai.path.Clear();
        ai.pathCounter = 0;
        ai.target = null;
        ai.minimapID = 0;
        ai.unitType = "";
        ai.moveSpeed = 2;

        HP = maxHP;
        isDefeated = false;

        GetComponent<AIPath>().canMove = true;
        transform.GetChild(0).GetComponent<Animator>().Play("MOVE");

        //Fix closed eyes from being enabled when reseting the unit
        transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(2).GetChild(0).GetChild(3).GetChild(2).gameObject.SetActive(false);
        transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(2).GetChild(0).GetChild(3).GetChild(3).gameObject.SetActive(false);

        EnableAllCircleColliders();

        gameObject.SetActive(false);
    }
}
