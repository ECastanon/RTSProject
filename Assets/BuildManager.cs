using System.Collections.Generic;
using UnityEngine;

public class BuildManager : MonoBehaviour
{
    private GameObject buildCanvas;
    private GameObject buildGridView;
    private MouseController mouseController;

    public Vector2Int gridSize;
    public Vector2 playerGridLocationOffest;

    public bool buildModeEnabled;

    public GameObject structurePlaceHolder;

    public GameObject[] placeHolders;

    public GameObject structureToPlace;

    public GameObject[] structures;

    private string buildSize = "";

    private Vector2 tilePos;

    private Vector2Int[] ThreeXTwo = new Vector2Int[]
    {
        new Vector2Int(0, 0),  // bottom-middle (origin is here)
        new Vector2Int(-1, 0), // left-bottom
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
    public List<Vector2> playerGrid = new List<Vector2>();

    private void Start()
    {
        mouseController = GetComponent<MouseController>();

        buildCanvas = GameObject.Find("BuildCanvas");
        buildCanvas.SetActive(false);
        buildGridView = GameObject.Find("BuildGrid");
        buildGridView.SetActive(false);

        structurePlaceHolder.SetActive(false);

        CreatePlayerGridLocations();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            buildModeEnabled = !buildModeEnabled;

            if (buildModeEnabled)
            {
                buildSize = "";
                buildCanvas.SetActive(true);
                buildGridView.SetActive(true);
                structurePlaceHolder.SetActive(true);
            }
            else
            {
                buildCanvas.SetActive(false);
                buildGridView.SetActive(false);
                structurePlaceHolder.SetActive(false);
            }
        }

        if (buildModeEnabled)
        {
            Time.timeScale = 0.0f;
            tilePos = mouseController.mouseTilePos();
            structurePlaceHolder.transform.position = tilePos;

            if (Input.GetMouseButtonDown(0) && buildSize != "")
            {
                CheckStructurePlacement();
            }
        }
        else
        {
            Time.timeScale = 1.0f;
        }
    }

    private bool CheckIfTileIsAvailable(Vector2 tile)
    {
        bool isAvailable = true;

        // Cast a ray straight down
        RaycastHit2D hit = Physics2D.Raycast(tile, -Vector2.up, Mathf.Infinity, LayerMask.GetMask("Structure_Player"));

        // If it hits something on the specified layer...
        if (hit.collider != null)
        {
            Debug.Log("I HIT: " +  hit.collider.gameObject.name + " @ " + tile);
            isAvailable = false; // Mark placement as taken
        }

        return isAvailable;
    }

    private void CheckStructurePlacement()
    {
        //Checks if the structure can fit within the given sizes.
        //If it cannot the function will end before placement is called
        if(buildSize == "3x2")
        {
            if (IfStructureIsOnGrid())
            {
                //Returns if any tile is not available
                foreach (Vector2 offset in ThreeXTwo)
                {
                    if (!CheckIfTileIsAvailable(tilePos + offset))
                    {
                        Debug.Log("2 I HIT @ " + (tilePos + offset));
                        Debug.Log("OBSTRUCTION IN THE WAY!");
                        return;
                    }
                }
            }
            else { return; }
        }
        if (buildSize == "5x3")
        {
            if(IfStructureIsOnGrid())
            {
                //Returns if any tile is not available
                foreach (Vector2 offset in FiveXThree)
                {
                    if (!CheckIfTileIsAvailable(tilePos + offset))
                    {
                        Debug.Log("OBSTRUCTION IN THE WAY!");
                        return;
                    }
                }
            }
            else { return; }
        }
        else
        {
            Debug.Log("INCORRECT SIZE: " + buildSize);
        }
        Debug.Log("STRUCTURE PLACED!");
        PlaceStructure();
    }

    private void PlaceStructure()
    {
        Instantiate(structureToPlace, tilePos, Quaternion.identity);
        GameObject.Find("PathFinder").GetComponent<AstarPath>().Scan();
    }

    //BuildCavas Button Actions
    public void ArcherSpawnerLevel1()
    {
        buildSize = "3x2";
        placeHolders[1].SetActive(false);
        placeHolders[0].SetActive(true);
        structurePlaceHolder = placeHolders[0];
        structureToPlace = structures[0];

    }
    public void ArcherSpawnerLevel2()
    {
        buildSize = "5x3";
        placeHolders[0].SetActive(false);
        placeHolders[1].SetActive(true);
        structurePlaceHolder = placeHolders[1];
        structureToPlace = structures[1];
    }

    private void CreatePlayerGridLocations()
    {
        List<Vector2> grid = new List<Vector2>();
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                playerGrid.Add(new Vector2(x + playerGridLocationOffest.x, y + playerGridLocationOffest.y));
            }
        }
    }

    private bool IfStructureIsOnGrid()
    {
        //Checks each size and checks if each structure's tile can fit within the playerGrid
        if (buildSize == "3x2")
        {
            foreach (Vector2 offset in ThreeXTwo)
            {
                //Checks if any tile of the structure is NOT on the grid
                if (!playerGrid.Contains(offset + tilePos))
                {
                    Debug.Log(offset + tilePos);
                    Debug.Log("IS NOT ALL ON THE GRID");
                    return false;
                }
            }
        }

        if (buildSize == "5x3")
        {
            foreach (Vector2 offset in FiveXThree)
            {
                //Checks if any tile of the structure is NOT on the grid
                if (!playerGrid.Contains(offset + tilePos))
                {
                    Debug.Log("IS NOT ALL ON THE GRID");
                    return false;
                }
            }
        }
        //Returns true if all pieces are on the grid
        return true;
    }
}