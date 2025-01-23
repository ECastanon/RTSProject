using System.Collections.Generic;
using UnityEngine;
using static MiniMapManager;

public class UnitSpawner : MonoBehaviour
{
    public GameObject UnitToSpawn;
    public bool ownedByPlayer;
    public float timeToSpawn;
    private float timer;

    public List<Transform> paths_Blue = new List<Transform>();
    public List<Transform> paths_Red = new List<Transform>();
    private int pathCounter;

    private MiniMapManager mmapManager;
    private UnitPooler unitPooler;

    private void Start()
    {
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
            timer = 0;
            GameObject unit = unitPooler.RequestUnit(UnitToSpawn, ownedByPlayer);
            unit.transform.position = transform.position;
            unit.SetActive(true);
            if(ownedByPlayer)
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
                    unit.GetComponent<UnitCombat_Data>().targetLayer = 1 << 7;
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
        }
        timer += Time.deltaTime;
    }
}
