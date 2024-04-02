using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;


[CreateAssetMenu(fileName = "ColorTemplate", menuName = "ScriptableObjects/ColorTemplate", order = 1)]
public class ColorTemplate : ScriptableObject
{
    [SerializeField] private List<Color> colorValues;
    [SerializeField] public Color Background;
    
    public Color GetColorForValue(int value)
    {
        var index = ((int)Mathf.Log(value, 2) % colorValues.Count) - 1;
        if (index < 0) 
        {
            var lastIndex = colorValues.Count - 1;
            index = lastIndex;
        }
        
        return colorValues[index];
    }
}
