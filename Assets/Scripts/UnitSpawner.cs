using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitSpawner : MonoBehaviour
{
    [Header("HP Values")]
    public float totalHP;
    public float hp;

    [Header("Spawn Data")]
    public GameObject UnitToSpawn;
    public bool ownedByPlayer;
    public int numberToSpawn;
    public float timeToSpawn;
    private float timer;
    public List<Transform> spawnLocations = new List<Transform>();

    [Header("Pathing Data")]
    public List<Transform> paths_Blue = new List<Transform>();
    public List<Transform> paths_Red = new List<Transform>();
    private int pathCounter;

    [Header("Canvas Bars")]
    public Image buildBar;
    public Transform buildBarDamagedPos;
    public Image hpBar;

    //References
    private MiniMapManager mmapManager;
    private UnitPooler unitPooler;

    private void Start()
    {
        hp = totalHP;
        hpBar.fillAmount = 0;

        foreach (Transform path in GameObject.Find("WayPoints_Blue").transform)
        {
            paths_Blue.Add(path);
        }
        foreach (Transform path in GameObject.Find("WayPoints_Red").transform)
        {
            paths_Red.Add(path);
        }

        mmapManager = GameObject.Find("MMap").GetComponent<MiniMapManager>();
        unitPooler = GameObject.Find("UnitPooler").GetComponent<UnitPooler>();
        pathCounter = Random.Range(0, paths_Blue.Count);
    }

    private void Update()
    {
        if (timer >= timeToSpawn)
        {
            int spawnCount = 0;
            while(spawnCount < numberToSpawn)
            {
                timer = 0;
                GameObject unit = unitPooler.RequestUnit(UnitToSpawn, ownedByPlayer);
                unit.transform.position = spawnLocations[spawnCount].position;
                unit.SetActive(true);
                if (ownedByPlayer)
                {
                    unit.layer = LayerMask.NameToLayer("Player");
                    unit.GetComponent<AIAgent>().primaryTarget = GameObject.Find("Totem_Enemy").transform;

                    unit.GetComponent<AIAgent>().minimapID = mmapManager.PlayerIconList.Count;
                    unit.GetComponent<AIAgent>().unitType = "PlayerUnit";
                    mmapManager.CreateIconOnMap("PlayerUnit");

                    foreach (Transform path in paths_Blue[pathCounter])
                    {
                        unit.GetComponent<AIAgent>().path.Add(path);
                    }
                    unit.GetComponent<AIAgent>().path.Add(GameObject.Find("Totem_Enemy").transform);
                    //=====================================================================================
                    if (unit.GetComponent<UnitCombat_Data>())
                    {
                        unit.GetComponent<UnitCombat_Data>().targetLayer += 1 << 7;
                        if(unit.GetComponent<UnitCombat_Data>().unitType == UnitCombat_Data.UnitType.Mage)
                        {
                            unit.GetComponent<UnitCombat_Data>().targetLayer += 1 << 10;
                        }
                    }
                }
                else
                {
                    unit.layer = LayerMask.NameToLayer("Enemy");
                    unit.GetComponent<AIAgent>().primaryTarget = GameObject.Find("Totem_Player").transform;

                    unit.GetComponent<AIAgent>().minimapID = mmapManager.EnemyIconList.Count;
                    unit.GetComponent<AIAgent>().unitType = "EnemyUnit";
                    mmapManager.CreateIconOnMap("EnemyUnit");

                    foreach (Transform path in paths_Red[pathCounter])
                    {
                        unit.GetComponent<AIAgent>().path.Add(path);
                    }
                    unit.GetComponent<AIAgent>().path.Add(GameObject.Find("Totem_Player").transform);
                    //=====================================================================================
                    if (unit.GetComponent<UnitCombat_Data>())
                    {
                        unit.GetComponent<UnitCombat_Data>().targetLayer = 1 << 8;
                    }
                }
                pathCounter++;
                if (pathCounter > 2) { pathCounter = 0; }
                spawnCount++;
            }
        }



        buildBar.fillAmount = timer / timeToSpawn;
        
        timer += Time.deltaTime;
    }

    public void UpdateHP(float value)
    {
        hp -= value;
        if(hp <= 0)
        {
            gameObject.SetActive(false);
        }

        if (hp < totalHP)
        {
            hpBar.fillAmount = hp / totalHP;
            buildBar.transform.parent.position = buildBarDamagedPos.position;
        }
        else
        {
            hpBar.fillAmount = 0;
            buildBar.transform.parent.position = hpBar.transform.parent.transform.position;
        }
    }
}
