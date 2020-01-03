using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseTile : MonoBehaviour, ITile
{
    Color hexColor;
    GameObject firstSelection;
    GameObject secondSelection;
    public int arrayPosX;
    public int arrayPosY;
    Vector2 startTouchPosition, endTouchPosition, swipeDiff;
    public bool isReadyToMove, isMatched = false;
    public Vector2 targetPos;
    GameManager gameManager;
    HexagonManager hexagonManager;
    PlayerController playerController;
    public Color HexColor { get => hexColor; set => hexColor = value; }

    void Awake()
    {
        GetRandomColor();
        gameManager = GameManager.instance;
        hexagonManager = HexagonManager.instance;
        playerController = PlayerController.instance;
    }



    void Update()
    {
        // Daha smooth bir hareket için MoveTowards. Boolean MoveTile tarafından değiştiriliyor.
        if (isReadyToMove)
        {
            float step = 7 * Time.deltaTime;


            transform.position = Vector2.MoveTowards(transform.position, targetPos, step);

            if ((Vector2)transform.position == targetPos)
                isReadyToMove = false;
        }

        if (Input.GetMouseButtonDown(0))
            OnMouseDown();

        if (Input.GetMouseButtonUp(0))
            OnMouseUp();
    }

    public void GetRandomColor()
    {
        Color c = ColorPicker.instance.RandomColor();
        this.GetComponent<SpriteRenderer>().color = c;
        HexColor = c;

    }

    public void SetArrayPosition(int x, int y)
    {
        arrayPosX = x;
        arrayPosY = y;
    }

    public void OnMouseDown()
    {
        Vector3 wp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 touchPos = new Vector2(wp.x, wp.y);
        if (this.GetComponent<PolygonCollider2D>() == Physics2D.OverlapPoint(touchPos))
        {

            swipeDiff = Vector2.zero;
            startTouchPosition = Input.mousePosition;
        }

    }


    //Swipe mı yoksa hexagon mu seçildiğini anladığımız kısım
    // Eğer seçilen hexagon ise pozisyona göre area bulunup, o area'ya göre diğer hexagonlar seçilir.
    public void OnMouseUp()
    {
        endTouchPosition = Input.mousePosition;
        swipeDiff = endTouchPosition - startTouchPosition;

        if (swipeDiff.magnitude < 20 && gameManager.CurrentState == GameManager.GameState.Ready)
        {
            float angle = Utilities.CalculateAngle(Camera.main.ScreenToWorldPoint(Input.mousePosition), this.transform.position);
            FindAndSelectHexagonsInArea(angle, null);
        }
    }

    public void MoveTile(Vector2 targetPos)
    {
        this.targetPos = targetPos;
        isReadyToMove = true;
    }

    // Area'ya göre hexagon getiren helper class
    public Tuple<SelectedHexagon, SelectedHexagon> GetClosestItems(int area)
    {
        Tuple<int, int, int, int> result = Utilities.ReturnAreaHexagons((int)area, arrayPosX, arrayPosY);

        SelectedHexagon h1 = new SelectedHexagon(result.Item1, result.Item2);
        SelectedHexagon h2 = new SelectedHexagon(result.Item3, result.Item4);

        if (isAvailable(h1, h2))
        {
            return Tuple.Create<SelectedHexagon, SelectedHexagon>(h1, h2);
        }
        else
        {
            return null;
        }

    }

    // Recursive function
    // En köşedeki hexagonlar için yazıldı.
    // Kenarları boşluk olduğu için bazı area'larda hexagon bulunmuyor.
    // En yakın hexagon'u bulana kadar dönen fonksiyon.
    public void FindAndSelectHexagonsInArea(float angle, int? area)
    {
        if (area == null)
            area = Utilities.SelectedArea(angle);
        Debug.Log(angle + " " + area);

        Tuple<SelectedHexagon, SelectedHexagon> result = GetClosestItems((int)area);

        if (result != null)
        {
            SelectedHexagon h1 = result.Item1;
            SelectedHexagon h2 = result.Item2;

            Select(hexagonManager.GetFromArray(h1.PosX, h1.PosY), hexagonManager.GetFromArray(h2.PosX, h2.PosY));

        }
        else
        {
            area = (area % 6) + 1;
            FindAndSelectHexagonsInArea(angle, area);
        }
    }


    public void Select(GameObject selectionOne, GameObject selectionTwo)
    {
        playerController.UnSelectHexagons();
        firstSelection = selectionOne;
        secondSelection = selectionTwo;

        transform.GetChild(0).gameObject.SetActive(true);
        firstSelection.transform.GetChild(0).gameObject.SetActive(true);
        secondSelection.transform.GetChild(0).gameObject.SetActive(true);

        playerController.addSelectedHexagonToList(this.gameObject);
        playerController.addSelectedHexagonToList(firstSelection);
        playerController.addSelectedHexagonToList(secondSelection);
    }


    public void UnSelect()
    {
        this.transform.GetChild(0).gameObject.SetActive(false);
    }


    // Seçilen hexagon'lar array'de mi diye kontrol etmek için helper class
    public bool isAvailable(SelectedHexagon h1, SelectedHexagon h2)
    {
        if (hexagonManager.CheckItems(h1.PosX, h1.PosY, h2.PosX, h2.PosY))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // Array ve position olarak iki hexagon arasında yer değiştirmeye yarayan helper class.
    // Swipe için kullanılır.
    public void Relocate(int arrayPosX, int arrayPosY, Vector2 pos)
    {
        hexagonManager.RemoveFromArray(arrayPosX, arrayPosY);
        this.arrayPosX = arrayPosX;
        this.arrayPosY = arrayPosY;
        hexagonManager.AddToArray(this.gameObject, arrayPosX, arrayPosY);
        MoveTile(pos);
        name = arrayPosX + "," + arrayPosY;

    }



    public void CheckNearbyHexagons(bool explode)
    {

        for (int area = 1; area < 7; area++)
        {
            Tuple<SelectedHexagon, SelectedHexagon> result = GetClosestItems(area);

            if (result != null)
            {
                SelectedHexagon h1 = result.Item1;
                SelectedHexagon h2 = result.Item2;
                CheckForMatch(hexagonManager.GetFromArray(h1.PosX, h1.PosY), hexagonManager.GetFromArray(h2.PosX, h2.PosY), explode);

            }

        }
    }

    // Match var mı diye renkleri kontrol edilir.
    // Eğer "explode" bool'u true gelirse patlatılır, false gelirse sadece bilgi verilir.
    public void CheckForMatch(GameObject firstHexagon, GameObject secondHexagon, bool explode)
    {
        Color firstColor = firstHexagon.GetComponent<BaseTile>().HexColor;
        Color secondColor = secondHexagon.GetComponent<BaseTile>().HexColor;

        if (this.HexColor.Equals(firstColor) && this.HexColor.Equals(secondColor) && firstColor.Equals(secondColor))
        {
            //            Debug.Log("There is a Match on " + arrayPosX + "," + arrayPosY + ". Because first color is " + firstColor + " and the second color is " + secondColor);

            if (explode)
            {

                isMatched = true;
                firstHexagon.GetComponent<BaseTile>().isMatched = true;
                secondHexagon.GetComponent<BaseTile>().isMatched = true;
                hexagonManager.AddToExplodeArray(firstHexagon.GetComponent<BaseTile>().arrayPosX, firstHexagon.GetComponent<BaseTile>().arrayPosY);
                hexagonManager.AddToExplodeArray(secondHexagon.GetComponent<BaseTile>().arrayPosX, secondHexagon.GetComponent<BaseTile>().arrayPosY);
                hexagonManager.AddToExplodeArray(arrayPosX, arrayPosY);
            }
            else
            {
                BoardChecker.instance.MatchFound = true;

            }
        }
    }

}


public struct SelectedHexagon
{
    public int PosX;
    public int PosY;

    public SelectedHexagon(int PosX, int PosY)
    {
        this.PosX = PosX;
        this.PosY = PosY;
    }
}