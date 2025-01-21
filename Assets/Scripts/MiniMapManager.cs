using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MiniMapManager : MonoBehaviour
{
    [HideInInspector] public enum IconType { Player, Enemy }
    [HideInInspector] public IconType iconType;

    private Transform PlayerParent;
    private Transform EnemyParent;

    private GameObject PlayerIcon;
    private GameObject PlayerUnitIcon;
    private GameObject EnemyIcon;
    private GameObject EnemyUnitIcon;

    public List<GameObject> PlayerIconList;
    public List<GameObject> EnemyIconList;

    private void Start()
    {
        PlayerParent = transform.GetChild(0);
        EnemyParent = transform.GetChild(1);

        PlayerIcon = transform.GetChild(2).gameObject;
        PlayerUnitIcon = transform.GetChild(3).gameObject;
        EnemyIcon = transform.GetChild(4).gameObject;
        EnemyUnitIcon = transform.GetChild(5).gameObject;
    }

    public void CreateIconOnMap(IconType type)
    {
        if(type == IconType.Player)
        {
            GameObject Icon = Instantiate(PlayerUnitIcon);
            Icon.transform.SetParent(transform);
            PlayerIconList.Add(Icon);
        }
        else if (type == IconType.Enemy)
        {
            GameObject Icon = Instantiate(EnemyUnitIcon);
            Icon.transform.SetParent(transform);
            EnemyIconList.Add(Icon);
        } 
        else
        {
            Debug.Log("Incorrect parameter given: " + type.ToString());
        }
    }

    public void RemoveIconFromMap(int ID, IconType type)
    {
        if (type == IconType.Player)
        {
            GameObject icon = PlayerIconList[ID];
            PlayerIconList[ID] = null;
            Destroy(icon);
        }
        else if (type == IconType.Enemy)
        {
            GameObject icon = EnemyIconList[ID];
            EnemyIconList[ID] = null;
            Destroy(icon);
        }
        else
        {
            Debug.Log("Incorrect parameter given: " + type.ToString());
        }
    }

    public void ClearIconList()
    {
        PlayerIconList.Clear();
        EnemyIconList.Clear();
    }
}
