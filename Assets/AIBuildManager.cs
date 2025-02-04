using System.Collections.Generic;
using UnityEngine;

public class AIBuildManager : MonoBehaviour
{
    public enum AIType { Rush, Conserve, Evolve }
    public AIType aiType;

    public Vector2Int gridSize;
    private Structures[,] spawnGrid;

    //Gold Management
    private GoldManager gm;
    public int gold; //Only to be used in the inspector, use gm.enemyGold for other calculations
    private int startingGold;
    private bool startingGoldSpent;

    public List<Structures> structureList = new List<Structures>();

    private GameObject structureToBuild;

    public Vector2 buildPositonOffset;

    private float SetUpdateSpeed()
    {
        //Sets the update speed (in seconds) to be based on the ai type
        float updatespeed = 1;

        switch (aiType)
        {
            case AIType.Rush:
                updatespeed = 1f;
                break;
            case AIType.Conserve:
                updatespeed = 10f;
                break;
            case AIType.Evolve:
                updatespeed = 10f;
                break;
        }

        return updatespeed;
    }

    private void Start()
    {
        spawnGrid = new Structures[gridSize.x, gridSize.y];

        gm = GameObject.Find("GameManager").GetComponent<GoldManager>();
        startingGold = gm.enemyGold;
        gold = gm.enemyGold;

        InvokeRepeating("UpdateInterval", SetUpdateSpeed(), SetUpdateSpeed());
    }

    private void UpdateInterval()
    {
        switch (aiType)
        {
            //Create a new script housing the rush ai logic
            case AIType.Rush:
                RushAiLogic();
                break;
            //Create a new script housing the conserve ai logic
            case AIType.Conserve:
                //
                break;
            //Create a new script housing the evolve ai logic
            case AIType.Evolve:
                //
                break;
        }
        gold = gm.enemyGold;
    }

    private Structures GetLowestQualityStructure()
    {
        Structures chosen = null;
        int lowestQuality = int.MaxValue;

        foreach (Structures s in structureList)
        {
            // Check if this structure's quality is lower than the current lowest.
            if (s.quality < lowestQuality)
            {
                lowestQuality = s.quality;
                chosen = s;
            }
        }

        return chosen;
    }
    private Structures GetHighestCostStructure()
    {
        Structures chosen = null;
        int highestPrice = int.MaxValue;

        foreach (Structures s in structureList)
        {
            // Check if this structure's quality is lower than the current lowest.
            if (s.cost < highestPrice)
            {
                highestPrice = s.cost;
                chosen = s;
            }
        }

        return chosen;
    }

    Vector2Int FindBestBuildLocation()
    {
        Vector2Int bestPosition = Vector2Int.one * -1;
        float highestScore = 0;

        foreach (Vector2Int pos in GetAllEmptyGridPositions())
        {
            float score = EvaluatePlacement(pos);
            if (score > highestScore)
            {
                highestScore = score;
                bestPosition = pos;
            }
        }

        return bestPosition;
    }

    float EvaluatePlacement(Vector2Int position)
    {
        float score = 0;

        List<Vector2Int> directions = new List<Vector2Int>
        {
            new Vector2Int(0, 1),  // Up
            new Vector2Int(1, 0),  // Right
            new Vector2Int(0, -1), // Down
            new Vector2Int(-1, 0)  // Left
        };

        foreach (Vector2Int dir in directions)
        {
            Vector2Int neighbor = position + dir;
            if (IsInBounds(neighbor) && IsOccupied(neighbor))
            {
                score += 5;
            }
        }

        if (position.x == 0 || position.x == gridSize.x - 1 ||
            position.y == 0 || position.y == gridSize.y - 1)
        {
            score -= 3;
        }

        return score;
    }

    private List<Vector2Int> GetAllEmptyGridPositions()
    {
        List<Vector2Int> emptyPositions = new List<Vector2Int>();

        // Loop through every cell in the grid
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                // Check if the cell is empty (no structure present)
                if (spawnGrid[x, y] == null)
                {
                    emptyPositions.Add(new Vector2Int(x, y));
                }
            }
        }

        return emptyPositions;
    }

    private bool IsOccupied(Vector2Int pos)
    {
        bool occupied = false;
        if (spawnGrid[pos.x, pos.y] == null)
        {
            occupied = true;
        }

        return occupied;
    }

    private bool IsInBounds(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < gridSize.x && pos.y >= 0 && pos.y < gridSize.y;
    }

    //===========================================================================================================
    // Rush AI
    //===========================================================================================================
    private void RushAiLogic()
    {
        //When starting gold still remains
        if (!startingGoldSpent)
        {
            SpendMoneyOnCheapestStructuresImmediately();
            return;
        }

        //When starting gold is empty
        BuildMostExpensiveStructure();

    }

    private void SpendMoneyOnCheapestStructuresImmediately()
    {
        Structures rushStructure = GetLowestQualityStructure();

        //Continues this each update cycle until the initial starting gold has been spent
        if (startingGold < rushStructure.cost)
        {
            startingGoldSpent = true;
        }
        if (!startingGoldSpent)
        {
            // If we have a valid structure and enough money...
            if (gm.enemyGold >= rushStructure.cost)
            {
                // Try to find a valid build location.
                Vector2Int buildPos = FindBestBuildLocation();

                // If a valid position is found...
                if (buildPos != Vector2Int.one * -1)
                {
                    // Instantiate the structure at the position. Adjust the position conversion as needed.
                    Vector2 worldPos = new Vector2(buildPos.x, buildPos.y);
                    Instantiate(rushStructure.structToSpawn, worldPos + buildPositonOffset, Quaternion.identity);

                    // Deduct the cost
                    gm.enemyGold -= rushStructure.cost;
                    startingGold -= rushStructure.cost;

                    // Mark the grid as occupied with this structure.
                    spawnGrid[buildPos.x, buildPos.y] = rushStructure;
                }
            }
        }
    }

    private void BuildMostExpensiveStructure()
    {
        Structures mostExpStructure = GetHighestCostStructure();

        if (!startingGoldSpent)
        {
            // If we have a valid structure and enough money...
            if (gm.enemyGold >= mostExpStructure.cost)
            {
                // Try to find a valid build location.
                Vector2Int buildPos = FindBestBuildLocation();

                // If a valid position is found...
                if (buildPos != Vector2Int.one * -1)
                {
                    // Instantiate the structure at the position. Adjust the position conversion as needed.
                    Vector2 worldPos = new Vector2(buildPos.x, buildPos.y);
                    Instantiate(mostExpStructure.structToSpawn, worldPos + buildPositonOffset, Quaternion.identity);

                    // Deduct the cost
                    gm.enemyGold -= mostExpStructure.cost;

                    // Mark the grid as occupied with this structure.
                    spawnGrid[buildPos.x, buildPos.y] = mostExpStructure;
                }
            }
        }
    }
}

[System.Serializable]
public class Structures
{
    public GameObject structToSpawn;
    public int cost;
    [Tooltip("X,Y")] public string size;
    public int quality;
}

/*Idea to have several AI types
-----------------------------------
Rush AI: Builds low quality structures as early as possible
    Goal: Spam units as early as possible to overwhelm the opponent in the early game.
Conserve AI: Saves money to build higher quality structures, will still mix some early structures to not be overwhelmed quickly
    Goal: Win the mid-late game with high quality structures and units.
Evolve AI: Focus on building Hero upgrades as early as possible, will still mix some early structures to not be overwhelmed quickly
    Goal: Evolve the Hero as much as possible to create a single unstoppable unit.
*/
