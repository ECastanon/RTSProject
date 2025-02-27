using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GoldManager : MonoBehaviour
{
    public int gold;
    public TextMeshProUGUI goldtext;

    public int enemyGold;

    public void AddSubtractGold(int value, bool goesToPlayer)
    {
        if(goesToPlayer)
        {
            gold += value;
            goldtext.text = "Gold: " + gold.ToString();
        }
        else
        {
            enemyGold += value;
        }
    }
}
