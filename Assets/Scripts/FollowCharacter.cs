using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCharacter : MonoBehaviour
{
    public Transform myParent;
    public bool isCamera;
    public float cameraSize;

    private void Start()
    {
        if (isCamera)
        {
            transform.GetComponent<Camera>().orthographicSize = cameraSize;
            transform.parent = myParent;
            transform.localPosition = new Vector3(0, 0, -10);
        }
        else
        {
            transform.parent = myParent;
            transform.localPosition = new Vector2(0, 0);
        }
    }
}
