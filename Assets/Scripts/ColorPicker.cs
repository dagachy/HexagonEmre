using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorPicker : MonoBehaviour
{

    public Color[] hexagonColors;

    public static ColorPicker instance;

    void Awake()
    {
        instance = this;
    }

    public Color RandomColor()
    {
        return hexagonColors[Random.Range(0, hexagonColors.Length)];
    }


}
