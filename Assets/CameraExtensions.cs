using System.Collections.Generic;
using UnityEngine;

public class CameraExtensions : MonoBehaviour
{
    private Camera camera;
    private void Start()
    {
        camera = GetComponent<Camera>();
    }
    public List<float> GetCameraBorders()
    {
        float verticalSize = camera.orthographicSize; // Half of the height in world units
        float horizontalSize = verticalSize * camera.aspect; // Half of the width in world units

        Vector2 cameraPosition = camera.transform.position;

        // Calculate the borders
        float left = cameraPosition.x - horizontalSize;
        float right = cameraPosition.x + horizontalSize;
        float top = cameraPosition.y + verticalSize;
        float bottom = cameraPosition.y - verticalSize;

        List<float> borders = new List<float>
        {
            left,
            right,
            bottom,
            top
        };

        //Returns (X LEFT, X RIGHT, Y BOTTOM, Y TOP)
        return borders;
    }
}
