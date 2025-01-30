using System.Collections.Generic;
using UnityEngine;

public class AIBuildManager : MonoBehaviour
{
    public Vector2Int gridSize;
    private Vector2Int[,] spawnGrid;

    public List<Structures> structureList = new List<Structures>();

    private GoldManager gm;

    private void Start()
    {
        spawnGrid = new Vector2Int[gridSize.x, gridSize.y];

        gm = GameObject.Find("GameManager").GetComponent<GoldManager>();
    }
}

[System.Serializable]
public class Structures
{
    public GameObject structToSpawn;
    public string size; //(Length, Height)
}

/*Idea to have several AI types
-----------------------------------
Rushdown AI: Builds low quality structures as early as possible
    Goal: Spam units as early as possible to overwhelm the opponent in the early game.
Conservative AI: Saves money to build higher quality structures, will still mix some early structures to not be overwhelmed quickly
    Goal: Win the mid-late game with high quality structures and units.
Evolution AI: Focus on building Hero upgrades as early as possible, will still mix some early structures to not be overwhelmed quickly
    Goal: Evolve the Hero as much as possible to create a single unstoppable unit.
*/
