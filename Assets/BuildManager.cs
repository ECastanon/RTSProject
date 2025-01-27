using UnityEngine;

public class BuildManager : MonoBehaviour
{
    public GameObject buildCanvas;
    public GameObject buildGridView;

    private MouseController mouseController;

    public bool buildModeEnabled;

    public GameObject structurePlaceHolder;
    public GameObject structureToPlace;

    private Vector2 tilePos;

    private void Start()
    {
        mouseController = GetComponent<MouseController>();

        buildCanvas = GameObject.Find("BuildCanvas");
        buildCanvas.SetActive(false);
        buildGridView = GameObject.Find("BuildGrid");
        buildGridView.SetActive(false);

        structurePlaceHolder.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            buildModeEnabled = !buildModeEnabled;

            if (buildModeEnabled)
            {
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

            if (Input.GetMouseButtonDown(0))
            {
                CheckStructurePlacement("2x3");
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
        RaycastHit2D hit = Physics2D.Raycast(tile, -Vector2.up, Mathf.Infinity, LayerMask.GetMask("Structure"));

        // If it hits something on the specified layer...
        if (hit.collider != null)
        {
            isAvailable = false; // Mark placement as taken
        }

        return isAvailable;
    }

    private void CheckStructurePlacement(string size)
    {
        //Checks if the structure can fit within the given sizes.
        //If it cannot the function will end before placement is called
        if(size == "2x3")
        {
            Vector2[] tileOffsets = {
                new Vector2(1, 0),
                new Vector2(1, 1),
                new Vector2(0, 1),
                new Vector2(-1, 1),
                new Vector2(-1, 0),
                new Vector2(0, 0)
            };

            //Returns if any tile is not available
            foreach (Vector2 offset in tileOffsets)
            {
                if (!CheckIfTileIsAvailable(tilePos + offset))
                {
                    Debug.Log("OBSTRUCTION IN THE WAY!");
                    return;
                }
            }
        }
        if (size == "3x5")
        {
            Vector2[] tileOffsets = {
                new Vector2(1, 0),
                new Vector2(1, 1),
                new Vector2(0, 1),
                new Vector2(-1, 1),
                new Vector2(-1, 0),
                new Vector2(0, 0),
                new Vector2(1, -1),
                new Vector2(0, -1),
                new Vector2(-1, -1),
                new Vector2(2, 1),
                new Vector2(2, 0),
                new Vector2(2, -1),
                new Vector2(-2, 1),
                new Vector2(-2, 0),
                new Vector2(-2, -1),
            };

            //Returns if any tile is not available
            foreach (Vector2 offset in tileOffsets)
            {
                if (!CheckIfTileIsAvailable(tilePos + offset))
                {
                    Debug.Log("OBSTRUCTION IN THE WAY!");
                    return;
                }
            }
        }

        Debug.Log("STRUCTURE PLACED!");
        PlaceStructure();
    }

    private void PlaceStructure()
    {
        Instantiate(structureToPlace, tilePos, Quaternion.identity);
        GameObject.Find("PathFinder").GetComponent<AstarPath>().Scan();
    }
}