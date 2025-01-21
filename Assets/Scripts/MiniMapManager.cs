using System.Collections.Generic;
using UnityEngine;

public class MiniMapManager : MonoBehaviour
{
    private Transform PlayerParent;
    private Transform EnemyParent;

    private GameObject PlayerIcon;
    private GameObject PlayerUnitIcon;
    private GameObject EnemyIcon;
    private GameObject EnemyUnitIcon;

    public List<GameObject> PlayerIconList;
    public List<GameObject> EnemyIconList;

    public Vector2 mapOffset;

    private void Start()
    {
        PlayerParent = transform.GetChild(0);
        EnemyParent = transform.GetChild(1);

        PlayerIcon = transform.GetChild(2).gameObject;
        PlayerUnitIcon = transform.GetChild(3).gameObject;
        EnemyIcon = transform.GetChild(4).gameObject;
        EnemyUnitIcon = transform.GetChild(5).gameObject;
    }

    public void CreateIconOnMap(string type)
    {
        if (type == "PlayerUnit")
        {
            GameObject Icon = Instantiate(PlayerUnitIcon);
            Icon.transform.SetParent(PlayerParent);
            PlayerIconList.Add(Icon);
        }
        else if (type == "EnemyUnit")
        {
            GameObject Icon = Instantiate(EnemyUnitIcon);
            Icon.transform.SetParent(EnemyParent);
            EnemyIconList.Add(Icon);
        } 
        else
        {
            Debug.Log("Incorrect parameter given: " + type.ToString());
        }
    }

    public void RemoveIconFromMap(int ID, string type)
    {
        if (type == "PlayerUnit")
        {
            GameObject icon = PlayerIconList[ID];
            PlayerIconList[ID] = null;
            Destroy(icon);
        }
        else if (type == "EnemyUnit")
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

    public void MoveOnMap(Transform self, int ID, string type)
    {
        Vector2 MapOffsetPos = new Vector2(self.position.x * mapOffset.x, self.position.y * mapOffset.y);
        switch (type)
        {
            case "Player":
                PlayerIcon.GetComponent<RectTransform>().anchoredPosition = MapOffsetPos;
                break;
            case "Enemy":
                EnemyIcon.GetComponent<RectTransform>().anchoredPosition = MapOffsetPos;
                break;
            case "PlayerUnit":
                PlayerIconList[ID].GetComponent<RectTransform>().anchoredPosition = MapOffsetPos;
                break;
            case "EnemyUnit":
                EnemyIconList[ID].GetComponent<RectTransform>().anchoredPosition = MapOffsetPos;
                break;
            default:
                Debug.Log("Incorrect parameter given: " + type.ToString());
                break;
        }
    }
}
