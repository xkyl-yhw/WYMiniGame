using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum TeamTag { red, blue, green }
public class TeamSetup : MonoBehaviour
{
    public TeamTag teamTag;
    public Color teamColor;

    private void Start()
    {
        teamColor = returnColor(teamTag);
    }

    public static Color returnColor(TeamTag tag)
    {
        switch (tag)
        {
            case TeamTag.red: return Color.red; break;
            case TeamTag.blue: return Color.blue; break;
            case TeamTag.green: return Color.green; break;
            default: return Color.white;
        }
    }

    public static TeamTag returnTeam(Color color)
    {
        if (color == Color.red)
            return TeamTag.red;
        else if (color == Color.blue)
            return TeamTag.blue;
        else return TeamTag.green;
    }
}
