using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssigmentColorHolder : MonoBehaviour
{
    [SerializeField] private List<AssignmentColor> _assignmentColors = new List<AssignmentColor>();
    
    public Color GetColorByType(ColorType type)
    {
        foreach (var icon in _assignmentColors)
        {
            if (icon.Type == type)
            {
                return icon.Color;
            }
        }
        
        return Color.clear;
    }
}

[Serializable]
public class AssignmentColor
{
    public Color Color;
    public ColorType Type;
}

public enum ColorType
{
    Purple,
    Teal,
    Green,
    Red,
    Orange,
    Brown,
    Black,
    None
}
