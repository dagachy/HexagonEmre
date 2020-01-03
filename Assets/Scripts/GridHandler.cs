using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridHandler : MonoBehaviour
{
    public static GridHandler instance;
    public GameObject hexPrefab, bombPrefab;
    public int gridWidth;
    public int gridHeight;
    public float gap = 0f;

    private float hexHeight = 0.626f;
    private float hexWidth = 0.695f;//0.545

    Vector3 startPos;

    public bool isBombReady = false;

    int nullCount = 0;

    List<GameObject> refillList = new List<GameObject>();

    void Awake()
    {
        instance = this;
    }
    void Start()
    {
        HexagonManager.instance.InstantiateArray(gridWidth, gridHeight);
        CreateBoard();
    }



    void CreateBoard()
    {
        AddGap();
        CalcStartPos();
        CreateGrid();
    }

    void AddGap()
    {
        hexWidth += hexWidth * gap;
        hexHeight += hexHeight * gap;
    }

    // Oluşturulan board'un ekranın ortasına gelmesi için hesaplama metot'u.
    // Board boyutuna göre 0,0 noktasını en ortadaki item'a (hexagona) göre ayarlar.
    void CalcStartPos()
    {
        float x = 0;
        float y = 0;

        if (gridWidth % 2 == 0)
        {
            float offset = 0;
            if (gridWidth / 2 % 2 == 0)
                offset = hexWidth / 4;
            x = ((gridWidth / 2) * (hexWidth / 2)) + (hexWidth * 0.375f) + offset;
        }

        else
        {
            x = ((gridWidth / 2) * (hexWidth * 0.75f));
        }

        y = ((gridHeight - 1) * (hexHeight / 2));
        startPos = new Vector3(x, y, 0);
    }

    // Hexagonal grid için gerekli position hesaplaması yapılır
    public Vector2 CalcWordPos(int x, int y)
    {
        float offset = 0;
        if (x % 2 != 0)
            offset = hexHeight / 2;

        float a = -startPos.x + (x * hexWidth * 0.75f);
        float b = -startPos.y + (y * hexHeight) + offset;
        return new Vector2(a, b);
    }

    // Belirlenen boyutta board oluşturur
    void CreateGrid()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                GameObject hex = Instantiate(hexPrefab, CalcWordPos(x, y), Quaternion.identity, this.transform);
                HexagonManager.instance.AddToArray(hex, x, y);
                hex.GetComponent<BaseTile>().SetArrayPosition(x, y);
                hex.name = x + "," + y;
            }
        }

        BoardChecker.instance.CheckInitialMatches();
    }

    public void RefillBoard()
    {
        SetNullCount();
        SetRefillList();
        InstantiateList();
        nullCount = 0;
        HexagonManager.instance.CheckForMatches();
        
    }

    // Board'da bulunan bütün boşlukları hesaplar
    public void SetNullCount()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                if (HexagonManager.instance.GetFromArray(x, y) == null)
                {
                    nullCount++;
                }
            }
        }
    }

    // Boşluk sayısına göre bir refill array'i oluşturulur.
    // İleride eklenecek başka bir special tile kolaylıkla buradan eklenebilir.
    public void SetRefillList()
    {
        for (int i = 0; i < nullCount; i++)
        {
            if (isBombReady)
            {
                refillList.Add(bombPrefab);
                isBombReady = false;
            }
            else
            {
                refillList.Add(hexPrefab);

            }
        }
    }

    // Refiil Array'i burada insantiate edilir
    void InstantiateList()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                if (HexagonManager.instance.GetFromArray(x, y) == null)
                {
                    GameObject hex = Instantiate(refillList[0], CalcWordPos(x, y), Quaternion.identity, this.transform);
                    HexagonManager.instance.AddToArray(hex, x, y);
                    hex.GetComponent<BaseTile>().SetArrayPosition(x, y);
                    hex.name = x + "," + y;

                    refillList.RemoveAt(0);
                }
            }
        }
    }

 
}
