using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;
    List<GameObject> selectedHexList = new List<GameObject>();
    Vector2 startTouchPosition, endTouchPosition, swipeDiff;

    HexValues firstHex, secondHex, thirdHex;

    public bool isRotating, isExploding = false;

    GameManager gameManager;

    void Awake()
    {
        instance = this;

    }

    void Start()
    {
        gameManager = GameManager.instance;
    }


    void Update()
    {
        SwipeControl();
    }



    public void addSelectedHexagonToList(GameObject hex)
    {
        selectedHexList.Add(hex);
    }

    public int getSelectedHexagonCount()
    {
        return selectedHexList.Count;
    }

    public void UnSelectHexagons()
    {
        foreach (GameObject hex in selectedHexList)
        {
            if (hex != null)
            {
                hex.GetComponent<BaseTile>().UnSelect();
            }

        }
        selectedHexList.Clear();

    }

    // Yapılan swipe uzunluğuna göre Clockwise veya CounterClockWise metotları çağırılır.
    void SwipeControl()
    {
        if (Input.GetMouseButtonDown(0))
        {
            swipeDiff = Vector2.zero;
            startTouchPosition = Input.mousePosition;

        }

        if (Input.GetMouseButtonUp(0))
        {
            endTouchPosition = Input.mousePosition;
            swipeDiff = endTouchPosition - startTouchPosition;
            //Debug.Log(endTouchPosition - startTouchPosition);
        }

        if (swipeDiff != Vector2.zero && gameManager.CurrentState == GameManager.GameState.Ready)
        {
            Move();
        }
        else
        {
            swipeDiff = Vector2.zero;
        }
    }


    void Move()
    {
        if (swipeDiff.y > 20)
        {
            StartCoroutine(RotateCounterClockWise());
            swipeDiff = Vector2.zero;
        }
        else if (swipeDiff.y < -20)
        {
            StartCoroutine(RotateClockWise());
            swipeDiff = Vector2.zero;
        }
    }

    // Clockwise döndürmek için seçilen hexagonun "Relocate" metodu çalıştırılır. 
    // Eğer explode var ise metot break edilir ve seçilmiş olan hexagonlar array'den çıkartılır.

    IEnumerator RotateClockWise()
    {
        if (selectedHexList.Count > 0)
        {
            isRotating = true;
            for (int i = 0; i < selectedHexList.Count; i++)
            {
                AssignItems();
                selectedHexList[0].GetComponent<BaseTile>().Relocate(thirdHex.arrayPosX, thirdHex.arrayPosY, thirdHex.position);
                selectedHexList[1].GetComponent<BaseTile>().Relocate(firstHex.arrayPosX, firstHex.arrayPosY, firstHex.position);
                selectedHexList[2].GetComponent<BaseTile>().Relocate(secondHex.arrayPosX, secondHex.arrayPosY, secondHex.position);

                HexagonManager.instance.CheckForMatches();

                yield return new WaitForSeconds(.3f);

                if (isExploding)
                {
                    GameManager.instance.RoundStarted = true;
                    UnSelectHexagons();
                    break;
                }
            }
            isRotating = false;

        }

    }


    IEnumerator RotateCounterClockWise()
    {
        if (selectedHexList.Count > 0)
        {
            isRotating = true;

            for (int i = 0; i < selectedHexList.Count; i++)
            {
                AssignItems();
                selectedHexList[0].GetComponent<BaseTile>().Relocate(secondHex.arrayPosX, secondHex.arrayPosY, secondHex.position);
                selectedHexList[1].GetComponent<BaseTile>().Relocate(thirdHex.arrayPosX, thirdHex.arrayPosY, thirdHex.position);
                selectedHexList[2].GetComponent<BaseTile>().Relocate(firstHex.arrayPosX, firstHex.arrayPosY, firstHex.position);

                HexagonManager.instance.CheckForMatches();

                yield return new WaitForSeconds(.3f);

                if (isExploding)
                {
                    GameManager.instance.RoundStarted = true;
                    UnSelectHexagons();
                    break;
                }

            }
            isRotating = false;
        }

    }

    // Helper Method
    void AssignItems()
    {

        firstHex = new HexValues(selectedHexList[0].GetComponent<BaseTile>().arrayPosX, selectedHexList[0].GetComponent<BaseTile>().arrayPosY, selectedHexList[0].transform.position);
        secondHex = new HexValues(selectedHexList[1].GetComponent<BaseTile>().arrayPosX, selectedHexList[1].GetComponent<BaseTile>().arrayPosY, selectedHexList[1].transform.position);
        thirdHex = new HexValues(selectedHexList[2].GetComponent<BaseTile>().arrayPosX, selectedHexList[2].GetComponent<BaseTile>().arrayPosY, selectedHexList[2].transform.position);
    }


}

struct HexValues
{
    public int arrayPosX;
    public int arrayPosY;
    public Vector2 position;

    public HexValues(int arrayPosX, int arrayPosY, Vector2 position)
    {
        this.arrayPosX = arrayPosX;
        this.arrayPosY = arrayPosY;
        this.position = position;
    }
}
