using System.Collections.Generic;
using UnityEngine;

public class AIBuildManager : MonoBehaviour
{
    public enum AIType { Rush, Conserve, Evolve }
    public AIType aiType;

    //Jungle stage is 16.5, -4.5
    public Vector2Int gridSize;
    private Structures[,] spawnGrid;
    private Vector2Int[] ThreeXTwo = new Vector2Int[]
    {
        new Vector2Int(-1, 0), // left-bottom
        new Vector2Int(0, 0),  // bottom-middle (origin is here)
        new Vector2Int(1, 0),  // right-bottom
        new Vector2Int(-1, 1), // left-top
        new Vector2Int(0, 1),  // middle-top
        new Vector2Int(1, 1)   // right-top
    };
    private Vector2Int[] FiveXThree = new Vector2Int[]
    {
        new Vector2Int(1, 0),
        new Vector2Int(1, 1),
        new Vector2Int(0, 1),
        new Vector2Int(-1, 1),
        new Vector2Int(-1, 0),
        new Vector2Int(0, 0),
        new Vector2Int(1, -1),
        new Vector2Int(0, -1),
        new Vector2Int(-1, -1),
        new Vector2Int(2, 1),
        new Vector2Int(2, 0),
        new Vector2Int(2, -1),
        new Vector2Int(-2, 1),
        new Vector2Int(-2, 0),
        new Vector2Int(-2, -1)
    };

    // Gold Management
    private GoldManager gm;
    public int gold; // Only for inspector reference; use gm.enemyGold for calculations
    private int startingGold;
    private bool startingGoldSpent;

    public List<Structures> structureList = new List<Structures>();
    public List<GameObject> structuresOnField;

    public Vector2 buildPositionOffset;

    private void Start()
    {
        spawnGrid = new Structures[gridSize.x, gridSize.y];

        gm = GameObject.Find("GameManager").GetComponent<GoldManager>();
        startingGold = gm.enemyGold;
        gold = gm.enemyGold;

        InvokeRepeating("UpdateInterval", SetUpdateSpeed(), SetUpdateSpeed());
    }

    private float SetUpdateSpeed()
    {
        // Sets the update speed (in seconds) based on the AI type
        float updatespeed = 1f;

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

    //===========================================================================================================
    // Supporting Functions
    //===========================================================================================================

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
        // If there isn't enough gold to build the structure, exit.
        if (structure == null || gm.enemyGold < structure.cost)
        {
            return;
        }

        // Start the recursive placement process.
        BuildStructureRecursive(structure, GetAllEmptyGridPositions());
    }

    private void BuildStructureRecursive(Structures structure, List<Vector2Int> candidates)
    {
        // If no more candidates remain, then exit.
        if (candidates.Count == 0)
        {
            Debug.Log("No More Valid Positons Avaiable");

            //If no valid spot is available, sell the lowest quality structure currently owned and clear the space,
            SellLowestQualityStructure();

            return;
        }

        // Pick a random candidate index.
        int index = Random.Range(0, candidates.Count);
        Vector2Int buildPos = candidates[index];
        // Remove it from the candidate list so we don't try it again.
        candidates.RemoveAt(index);

        //Set the tile offsets based on structure size
        Vector2Int[] tileOffsets = new Vector2Int[] {new Vector2Int(0, 0)};
        if(structure.size == new Vector2(3, 2)) //Use 3x2 Size
        {
            tileOffsets = ThreeXTwo;
        }
        if (structure.size == new Vector2(5, 3)) //Use 5x3 Size
        {
            tileOffsets = FiveXThree;
        }


        // Check that every required cell is within bounds and unoccupied.
        bool canPlace = true;
        foreach (Vector2Int offset in tileOffsets)
        {
            Vector2Int checkPos = buildPos + offset;
            if (!IsInBounds(checkPos) || spawnGrid[checkPos.x, checkPos.y] != null)
            {
                canPlace = false;
                break;
            }
        }

        if (canPlace)
        {
            // Mark all the cells as occupied.
            foreach (Vector2Int offset in tileOffsets)
            {
                Vector2Int posToMark = buildPos + offset;
                spawnGrid[posToMark.x, posToMark.y] = structure;
            }

            // Instantiate the structure at the desired world position.
            // (Assuming buildPositionOffset is used to convert from grid space to world space.)
            Vector2 worldPos = buildPos + buildPositionOffset;
            GameObject building = Instantiate(structure.structToSpawn, worldPos, Quaternion.identity);

            //Sets the building's quality/size & adds to a list of other built structures
            building.GetComponent<UnitSpawner>().quality = structure.quality;
            building.GetComponent<UnitSpawner>().size = structure.size;
            structuresOnField.Add(building);

            // Deduct the cost.
            gm.enemyGold -= structure.cost;
            startingGold -= structure.cost;
        }
        else
        {
            // Candidate position was invalid.
            // Recurse and try again with the remaining candidates.
            BuildStructureRecursive(structure, candidates);
        }
    }


    private Structures FindMostExpensiveStructure()
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

    private GameObject FindLowestQualityStructure()
    {
        //Loops through each built structure and return the first lowest quality one
        int q = int.MaxValue;
        GameObject g = null;
        foreach (GameObject s in structuresOnField)
        {
            int x = s.GetComponent<UnitSpawner>().quality;
            if (q > x)
            {
                q = x;
                g = s;
            }
        }
        return g;
    }

    private void SellLowestQualityStructure()
    {
        GameObject lowesetQualityStruct = FindLowestQualityStructure();
        Vector2Int[] offsets = new Vector2Int[] { new Vector2Int(0, 0) };
        if (lowesetQualityStruct.GetComponent<UnitSpawner>().size == new Vector2(3, 2)) //Use 3x2 Size
        {
            offsets = ThreeXTwo;
        }
        if (lowesetQualityStruct.GetComponent<UnitSpawner>().size == new Vector2(5, 3)) //Use 5x3 Size
        {
            offsets = FiveXThree;
        }

        // UNMark all the cells as empty.
        foreach (Vector2Int offset in offsets)
        {
            Vector2 location = new Vector2(lowesetQualityStruct.transform.position.x, lowesetQualityStruct.transform.position.y) - buildPositionOffset;
            Vector2Int locationInt = new Vector2Int((int)location.x, (int)location.y);
            Vector2Int posToMark = locationInt + offset;
            spawnGrid[posToMark.x, posToMark.y] = null;
        }
        structuresOnField.Remove(lowesetQualityStruct);
        //Refunds a % of the original cost
        gm.enemyGold += lowesetQualityStruct.GetComponent<UnitSpawner>().cost / 10;
        Destroy(lowesetQualityStruct);
    }

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
        Structures structure = FindMostExpensiveStructure();
        if (structure == null)
        {
            return;
        }
        else if(gm.enemyGold >= structure.cost)
        {
            BuildStructure(structure);
        }
    }
}

[System.Serializable]
public class Structures
{
    public GameObject structToSpawn;
    public int cost;
    [Tooltip("Width,Height")] public Vector2Int size;
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
