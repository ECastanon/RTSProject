using System.Collections.Generic;
using UnityEngine;

public class UnitPooler : MonoBehaviour
{
    [Header("Blue Team")]
    public List<UnitPooling> bluePool = new List<UnitPooling>();
    private Dictionary<GameObject, List<GameObject>> blueUnits = new Dictionary<GameObject, List<GameObject>>();

    [Header("Red Team")]
    public List<UnitPooling> redPool = new List<UnitPooling>();
    private Dictionary<GameObject, List<GameObject>> redUnits = new Dictionary<GameObject, List<GameObject>>();

    void Start()
    {
        InitializePool(bluePool, blueUnits);
        InitializePool(redPool, redUnits);
    }

    private void InitializePool(List<UnitPooling> pool, Dictionary<GameObject, List<GameObject>> unitDictionary)
    {
        foreach (UnitPooling unit in pool)
        {
            if (!unitDictionary.ContainsKey(unit.UnitType))
            {
                unitDictionary[unit.UnitType] = new List<GameObject>();
            }

            for (int i = 0; i < unit.numberToInitialize; i++)
            {
                GameObject newUnit = Instantiate(unit.UnitType, transform.position, Quaternion.identity);
                newUnit.SetActive(false); // Start inactive
                unitDictionary[unit.UnitType].Add(newUnit);
            }
        }
    }

    public GameObject RequestUnit(GameObject unitPrefab, bool isBlue)
    {
        var unitDictionary = isBlue ? blueUnits : redUnits;

        if (!unitDictionary.ContainsKey(unitPrefab))
        {
            Debug.LogError($"No pool exists for the unit prefab: {unitPrefab.name}");
            return null;
        }

        // Find the first inactive unit in the pool
        foreach (var unit in unitDictionary[unitPrefab])
        {
            if (!unit.activeSelf)
            {
                unit.SetActive(true); // Activate the unit
                return unit;
            }
        }

        // If no inactive unit is available, log a warning or add a new unit
        Debug.LogWarning($"No inactive unit available for prefab: {unitPrefab.name}. Instantiating a new one.");
        GameObject newUnit = Instantiate(unitPrefab, transform.position, Quaternion.identity);
        unitDictionary[unitPrefab].Add(newUnit);
        return newUnit;
    }
}

[System.Serializable]
public class UnitPooling
{
    public GameObject UnitType;
    public int numberToInitialize;
}

