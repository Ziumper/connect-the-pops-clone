using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;


[CreateAssetMenu(fileName = "ColorTemplate", menuName = "ScriptableObjects/ColorTemplate", order = 1)]
public class ColorTemplate : ScriptableObject
{
    [Serializable]
    public class ColorValue
    {
        public Color Color;
        public int Value;
    }

    [SerializeField] private List<ColorValue> colorValues;
    [SerializeField] public Color Background;
    
    public Color GetColorForValue(int value)
    {
        var color = colorValues.FirstOrDefault(v => v.Value == value);
        if(color != null)
        {
            return color.Color;
        }

        return Color.white;
    }
}
