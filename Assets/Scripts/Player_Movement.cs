using UnityEngine;
using static UnitCombat_Data;

public class Player_Movement : MonoBehaviour
{

    [HideInInspector] public Animator anim;

    public GameObject target;
    public Vector2 location;

    //Velocity
    public float moveSpeed;

    //Direction
    private Vector2 posLastFrame;
    private Vector2 posThisFrame;

    [Header("MiniMap Data")]
    private MiniMapManager mmapManager;
    private string unitType = "Player";

    void Start()
    {
        anim = transform.GetChild(0).GetComponent<Animator>();
        mmapManager = GameObject.Find("MMap").GetComponent<MiniMapManager>();
        UpdateMiniMapPos();

        location = transform.position;
    }

    void Update()
    {
        MoveTowardsTarget();
        CheckMoveDirection();
    }

    public void SetDestinationData(Vector2 loc, GameObject character)
    {
        location = loc;
        if (character != null && character.layer != gameObject.layer)
        {
            target = character;
        } else { target = null; }
    }

    private void MoveTowardsTarget()
    {
        if(target != null)
        {
            //Walk directly towards the target
            float dist = Vector3.Distance(target.transform.position, transform.position);
            if (GetComponent<Hero_Data>())
            {
                if (dist > GetComponent<Hero_Data>().attackRange)
                {
                    transform.position = Vector2.MoveTowards(transform.position, target.transform.position, moveSpeed * Time.deltaTime);
                    anim.SetBool("isMoving", true);
                }
                else { anim.SetBool("isMoving", false); }
            }

        }
        else
        {
            float dist = Vector3.Distance(location, transform.position);
            if (dist > 0.1f)
            {
                transform.position = Vector2.MoveTowards(transform.position, location, moveSpeed * Time.deltaTime);
                anim.SetBool("isMoving", true);
            }
            else
            {
                anim.SetBool("isMoving", false);
            }
        }

        if (anim.GetBool("isMoving") == true)
        {
            UpdateMiniMapPos();
        }
    }

    private void CheckMoveDirection()
    {
        posLastFrame = posThisFrame;
        posThisFrame = transform.position;

        if (posThisFrame.x > posLastFrame.x)
        {
            // Moving Right
            transform.GetChild(0).localRotation = Quaternion.Euler(0, 180, 0);
        }
        else if (posThisFrame.x < posLastFrame.x)
        {
            // Moving Left
            transform.GetChild(0).localRotation = Quaternion.Euler(0, 0, 0);
        }
    }

    private void UpdateMiniMapPos()
    {
        mmapManager.MoveOnMap(transform, 0, unitType);
    }
}
