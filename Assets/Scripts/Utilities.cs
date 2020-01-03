using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utilities
{

    // Çoğu class'ın ihtiyacı olacağı metotlar burada yer alıyor.

    public static float CalculateAngle(Vector3 pos1, Vector3 pos2)
    {
        Vector2 targetdir = pos1 - pos2;
        float angle = Vector2.Angle(Vector2.right, targetdir);
        if (pos1.y < pos2.y)
            angle = 360 - angle;
        return angle;
    }

    public static bool inBounds2D(int x, int y, int firstDim, int secondDim)
    {
        return x < firstDim && y < secondDim && x >= 0 && y >= 0;
    }

    // Hexagon'un basıldığı açısına göre seçilecek area döndürülür.
    public static int SelectedArea(float angle)
    {
        if (angle <= 45 || angle > 315)
        {
            return 1;
        }
        else if (angle > 45 && angle <= 90)
        {
            return 2;
        }
        else if (angle > 90 && angle <= 135)
        {
            return 3;
        }
        else if (angle > 135 && angle <= 225)
        {
            return 4;
        }
        else if (angle > 225 && angle <= 270)
        {
            return 5;
        }
        else if (angle > 270 && angle <= 315)
        {
            return 6;
        }

        return 0;
    }

    public static Tuple<int, int, int, int> ReturnAreaHexagons(int area, int arrayPosX, int arrayPosY)
    {

        int firstSelectionPosX = -1;
        int firstSelectionPosY = -1;
        int secondSelectionPosX = -1;
        int secondSelectionPosY = -1;

        bool isDivisable = arrayPosX % 2 != 0;

        /*
            Hexagonal Grid yapılırken her iki sütunda bir oluşan boşluk yüzünden uzunca bir switch case oluşturuldu.
            Her hexagon 6'ya bölünüp basılan veya seçilen tarafa göre hexagon pozisyonu döndürür.
            Her seçimde 3 tane hexagon seçeceği için bu metot sadece seçtiği iki hexagon'un pozisyonlarını döndürür.
        */
        #region Endless Switch Case


        switch (area)
        {
            case 1:
                firstSelectionPosX = arrayPosX + 1;
                secondSelectionPosX = arrayPosX + 1;

                if (isDivisable)
                {
                    firstSelectionPosY = arrayPosY;
                    secondSelectionPosY = arrayPosY + 1;
                }
                else
                {
                    firstSelectionPosY = arrayPosY - 1;
                    secondSelectionPosY = arrayPosY;
                }
                break;
            case 2:
                firstSelectionPosX = arrayPosX + 1;
                secondSelectionPosX = arrayPosX;

                if (isDivisable)
                {
                    firstSelectionPosY = arrayPosY + 1;
                    secondSelectionPosY = arrayPosY + 1;
                }
                else
                {
                    firstSelectionPosY = arrayPosY;
                    secondSelectionPosY = arrayPosY + 1;
                }
                break;
            case 3:
                firstSelectionPosX = arrayPosX;
                secondSelectionPosX = arrayPosX - 1;
                if (isDivisable)
                {
                    firstSelectionPosY = arrayPosY + 1;
                    secondSelectionPosY = arrayPosY + 1;
                }
                else
                {
                    firstSelectionPosY = arrayPosY + 1;
                    secondSelectionPosY = arrayPosY;
                }

                break;
            case 4:
                firstSelectionPosX = arrayPosX - 1;
                secondSelectionPosX = arrayPosX - 1;


                if (isDivisable)
                {
                    firstSelectionPosY = arrayPosY + 1;
                    secondSelectionPosY = arrayPosY;
                }
                else
                {
                    firstSelectionPosY = arrayPosY;
                    secondSelectionPosY = arrayPosY - 1;
                }
                break;
            case 5:
                firstSelectionPosX = arrayPosX - 1;
                secondSelectionPosX = arrayPosX;
                if (isDivisable)
                {
                    firstSelectionPosY = arrayPosY;
                    secondSelectionPosY = arrayPosY - 1;
                }
                else
                {
                    firstSelectionPosY = arrayPosY - 1;
                    secondSelectionPosY = arrayPosY - 1;
                }
                break;
            case 6:
                firstSelectionPosX = arrayPosX;
                secondSelectionPosX = arrayPosX + 1;
                if (isDivisable)
                {
                    firstSelectionPosY = arrayPosY - 1;
                    secondSelectionPosY = arrayPosY;
                }
                else
                {
                    firstSelectionPosY = arrayPosY - 1;
                    secondSelectionPosY = arrayPosY - 1;
                }

                break;
        }
        #endregion


        return Tuple.Create<int, int, int, int>(firstSelectionPosX, firstSelectionPosY, secondSelectionPosX, secondSelectionPosY);
    }
}
