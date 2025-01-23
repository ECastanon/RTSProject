using UnityEngine;

[ExecuteInEditMode]
public class SpritePos : MonoBehaviour
{
    //Created some lag when played in the editor
    //Can uncomment if needed later

    /*
    #if UNITY_EDITOR
    // Update is called once per frame
    void Update()
    {
        float tX = transform.localPosition.x;
        float tY = transform.localPosition.y;
        tX = tX- (tX % 0.015625f);
        tY = tY- (tY % 0.015625f);
        transform.localPosition = new Vector3(tX,tY,0);
    }
    #endif
    */
}