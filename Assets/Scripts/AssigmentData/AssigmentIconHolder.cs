using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssigmentIconHolder : MonoBehaviour
{
    [SerializeField] private List<AssignmentIcon> _assignmentIcons = new List<AssignmentIcon>();
    
    public Sprite GetSpriteByType(IconType type)
    {
        foreach (var icon in _assignmentIcons)
        {
            if (icon.Type == type)
            {
                return icon.Sprite;
            }
        }
        
        return null;
    }
}

[Serializable]
public class AssignmentIcon
{
    public Sprite Sprite;
    public IconType Type;
}

public enum IconType
{
    First,
    Second,
    Third,
    Fourth,
    Fifth,
    Sixth,
    Seventh,
    Eighth,
    Ninth,
    Tenth,
    Eleventh,
    Twelfth,
    Thirteenth,
    Fourteenth,
    Fifteenth,
    Sixteenth,
    Seventeenth,
    Eighteenth,
    None
}