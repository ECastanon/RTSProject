using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildManager : MonoBehaviour
{
    public GameObject buildCanvas;

    public bool buildModeEnabled;

    private void Start()
    {
        buildCanvas = GameObject.Find("BuildCanvas");
        buildCanvas.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            buildModeEnabled = !buildModeEnabled;
        }

        if (buildModeEnabled)
        {
            buildCanvas.SetActive(true);
            Time.timeScale = 0.0f;
        }
        else
        {
            buildCanvas.SetActive(false);
            Time.timeScale = 1.0f;
        }
    }
}
