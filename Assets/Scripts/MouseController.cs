using UnityEngine;
using UnityEngine.TextCore.Text;

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
        //Debug.Log("Mouse Clicked on: " +  mouseWorldPos);
        return mouseWorldPos;
    }
    public Vector2 mouseTilePos()
    {
        var mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.x = Mathf.Floor(mouseWorldPos.x) + 0.5f;
        mouseWorldPos.y = Mathf.Floor(mouseWorldPos.y) + 0.5f;
        mouseWorldPos.z = 0f;
        //Debug.Log("Mouse Clicked on: " + mouseWorldPos);
        return mouseWorldPos;
    }

    public void Click()
    {
        Hero.GetComponent<Player_Movement>().SetDestinationData(mousePos(), hoveredCharacter);
        cursorMark.transform.localScale = new Vector3(.5f, .5f, 1);
        cursorMark.transform.position = mousePos();

        if(hoveredCharacter != null && hoveredCharacter.layer != Hero.layer)
        {
            cursorMark.transform.GetChild(0).gameObject.SetActive(false);
            cursorMark.transform.GetChild(1).gameObject.SetActive(true);

            cursorMark.transform.GetChild(1).GetComponent<Animator>().Play("orangeclick", -1, 0f);
        }
        else
        {
            cursorMark.transform.GetChild(1).gameObject.SetActive(false);
            cursorMark.transform.GetChild(0).gameObject.SetActive(true);

            cursorMark.transform.GetChild(0).GetComponent<Animator>().Play("clickanim", -1, 0f);
        }
    }

    private void ShrinkObject()
    {
        if (cursorMark.transform.localScale != shrink)
        {
            cursorMark.transform.localScale = Vector3.MoveTowards(cursorMark.transform.localScale, shrink, 0.5f * Time.deltaTime);
        }
    }
}
