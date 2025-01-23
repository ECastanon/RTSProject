using HighlightPlus;
using System.Collections.Generic;
using UnityEngine;

public class DisableWhenOffScreen : MonoBehaviour
{
    private HighlightEffect he;
    private GameObject rootChild_1, rootChild_2, rootChild_3;
    private CameraExtensions ce;
    private float updateInterval = 0.25f;
    private void Start()
    {
        ce = GameObject.Find("Main Camera").GetComponent<CameraExtensions>();
        he = GetComponent<HighlightEffect>();
        rootChild_1 = transform.GetChild(0).gameObject;
        rootChild_2 = transform.GetChild(1).gameObject;
        rootChild_3 = transform.GetChild(2).gameObject;

        InvokeRepeating("UpdateInterval", updateInterval, updateInterval);
    }
    private void UpdateInterval()
    {
        //use this as the secondary update.
        if(CheckIfOffScreen())
        {
            SetInvisible();
        } else { SetVisible(); }
    }

    private bool CheckIfOffScreen()
    {
        bool isOffScreen = false;
        //Left, Right, Bottom, Top
        List<float> borders = ce.GetCameraBorders();

        //Check if object is outside the X borders
        if(transform.position.x < borders[0] || transform.position.x > borders[1])
        {
            isOffScreen = true;
        }
        //Check if object is outside the Y borders
        if (transform.position.y < borders[2] || transform.position.y > borders[3])
        {
            isOffScreen = true;
        }
        return isOffScreen;
    }

    private void SetInvisible()
    {
        he.enabled = false;
        rootChild_1.SetActive(false);
        rootChild_2.SetActive(false);
        rootChild_3.SetActive(false);
    }
    private void SetVisible()
    {
        he.enabled = true;
        rootChild_1.SetActive(true);
        rootChild_2.SetActive(true);
        rootChild_3.SetActive(true);
    }
}
