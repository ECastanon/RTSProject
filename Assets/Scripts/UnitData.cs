using HighlightPlus;
using System.Collections;
using UnityEngine;

public class UnitData : MonoBehaviour
{
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
    }

    private void Update()
    {
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

    public void RecieveDamage(float damage)
    {
        HP -= damage;

        HighlightEffect effect = transform.GetChild(0).GetChild(0).GetComponent<HighlightEffect>();
        effect.HitFX();
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
