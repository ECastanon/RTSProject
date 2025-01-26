using UnityEngine;

public class BuildManager : MonoBehaviour
{
    public GameObject buildCanvas;
    public GameObject buildGridView;

    public bool buildModeEnabled;

    private void Start()
    {
        buildCanvas = GameObject.Find("BuildCanvas");
        buildCanvas.SetActive(false);
        buildGridView = GameObject.Find("BuildGrid");
        buildGridView.SetActive(false);
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
            }
            else
            {
                buildCanvas.SetActive(false);
                buildGridView.SetActive(false);
            }
        }

        if (buildModeEnabled)
        {
            Time.timeScale = 0.0f;
        }
        else
        {
            Time.timeScale = 1.0f;
        }
    }
}
