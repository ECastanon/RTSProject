using UnityEngine;

public class MouseController : MonoBehaviour
{
    public GameObject Hero;
    public GameObject recentlyClickedOnCharacter;
    public GameObject hoveredCharacter;

    public GameObject cursorMark;
    private Vector3 shrink = new Vector3(.001f, .001f, 1);

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Click();
        }
        ShrinkObject();
    }

    private Vector2 mousePos()
    {
        var mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;
        return mouseWorldPos;
    }

    public void Click()
    {
        Hero.GetComponent<Player_Movement>().SetDestinationData(mousePos(), hoveredCharacter);
        cursorMark.transform.localScale = new Vector3(.5f, .5f, 1);
        cursorMark.transform.position = mousePos();
        cursorMark.transform.GetChild(0).GetComponent<Animator>().Play("clickanim", -1, 0f);
    }

    private void ShrinkObject()
    {
        if (cursorMark.transform.localScale != shrink)
        {
            cursorMark.transform.localScale = Vector3.MoveTowards(cursorMark.transform.localScale, shrink, 0.5f * Time.deltaTime);
        }
    }
}
