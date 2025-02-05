using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class AIBuildManager : MonoBehaviour
{
    public enum AIType { Rush, Conserve, Evolve }
    public AIType aiType;

    public Vector2Int gridSize;
    private Structures[,] spawnGrid;

    // Gold Management
    private GoldManager gm;
    public int gold; // Only for inspector reference; use gm.enemyGold for calculations
    private int startingGold;
    public bool startingGoldSpent; //

    public List<Structures> structureList = new List<Structures>();

    public Vector2 buildPositonOffset;

    private float SetUpdateSpeed()
    {
        // Sets the update speed (in seconds) based on the AI type
        float updatespeed = 1f;

        switch (aiType)
        {
            case AIType.Rush:
                updatespeed = .1f;
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
            case AIType.Rush:
                RushAiLogic();
                break;
            case AIType.Conserve:
                // Conserve logic here...
                break;
            case AIType.Evolve:
                // Evolve logic here...
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
            // Choose the structure with the lowest quality value
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
        int highestPrice = int.MinValue; // Use int.MinValue to look for the highest cost

        foreach (Structures s in structureList)
        {
            // Choose the structure with the highest cost that can still be built
            if (s.cost > highestPrice)
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
        float highestScore = float.MinValue; // start with a very low score

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
                // If the cell is empty (no structure present), add it to the list
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
        // First ensure the position is within bounds
        if (!IsInBounds(pos))
        {
            return false;
        }

        // Return true if the cell is not empty
        return spawnGrid[pos.x, pos.y] != null;
    }

    private bool IsInBounds(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < gridSize.x && pos.y >= 0 && pos.y < gridSize.y;
    }

    private void BuildStructure(Structures structure)
    {
        // Base condition: if there isn't enough gold to build the structure, exit function.
        if (structure == null || gm.enemyGold < structure.cost)
        {
            return;
        }

        Vector2Int buildPos = FindBestBuildLocation();
        if (buildPos == Vector2Int.one * -1)
        {
            return; // No valid build location was found.
        }

        // Instantiate the structure at the specified world position (plus an offset)
        Vector2 worldPos = buildPos + buildPositonOffset;
        Instantiate(structure.structToSpawn, worldPos, Quaternion.identity);

        //Update currencies and grid locations
        gm.enemyGold -= structure.cost;
        startingGold -= structure.cost;
        spawnGrid[buildPos.x, buildPos.y] = structure;
    }

    //===========================================================================================================
    // Rush AI
    //===========================================================================================================
    private void RushAiLogic()
    {
        // When starting gold is still available, build the cheapest (lowest quality) structures.
        if (!startingGoldSpent)
        {
            Structures rushStructure = GetLowestQualityStructure();

            if (startingGold < rushStructure.cost)
            {
                startingGoldSpent = true;
            }
            else
            {
                BuildStructure(rushStructure);
            }
            return;
        }

        // When starting gold is spent, build the most expensive buildable structure.
        Structures structure = FindMostExpensiveANDBuildableStructure();
        if (structure == null)
        {
            return;
        }
        else if(gm.enemyGold >= structure.cost)
        {
            BuildStructure(structure);
        }
    }

    private Structures FindMostExpensiveANDBuildableStructure()
    {
        Structures chosen = null;
        int highestCost = -1;

        List<Structures> tempList = new List<Structures>(structureList);
        foreach (Structures s in tempList)
        {
            // Only consider structures that can be afforded.
            if (s.cost <= gm.enemyGold)
            {
                if (s.cost > highestCost)
                {
                    highestCost = s.cost;
                    chosen = s;
                }
            }
        }

        return chosen;
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
