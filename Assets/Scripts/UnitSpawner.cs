using System.Collections.Generic;
using UnityEngine;

public class UnitSpawner : MonoBehaviour
{
    public GameObject UnitToSpawn;
    public bool ownedByPlayer;
    public float timeToSpawn;
    private float timer;

    public List<Transform> paths_Blue = new List<Transform>();
    public List<Transform> paths_Red = new List<Transform>();
    private int pathCounter;

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

        pathCounter = Random.Range(0, paths_Blue.Count);
    }

    private void Update()
    {
        if (timer >= timeToSpawn)
        {
            timer = 0;
            GameObject unit = Instantiate(UnitToSpawn, transform.position, Quaternion.identity);
            if(ownedByPlayer)
            {
                unit.layer = LayerMask.NameToLayer("Player");
                unit.GetComponent<AIAgent>().primaryTarget = GameObject.Find("Totem_Enemy").transform;
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
