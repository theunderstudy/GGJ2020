using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public struct TileKey
{
    public TileKey(int x, int z)
    {
        this.X = x;
        this.Z = z;
    }

    public TileKey(Vector3 vec)
    {
        this.X = Mathf.RoundToInt(vec.x);
        this.Z = Mathf.RoundToInt(vec.z);
    }
    public int X;
    public int Z;

    public static TileKey north = new TileKey(0, 1);
    public static TileKey northEast = new TileKey(1, 1);
    public static TileKey east = new TileKey(1, 0);
    public static TileKey southEast = new TileKey(1, -1);
    public static TileKey south = new TileKey(0, -1);
    public static TileKey southWest = new TileKey(-1, -1);
    public static TileKey west = new TileKey(-1, 0);
    public static TileKey northWest = new TileKey(-1, 1);

    public static TileKey GetDirection(TileKey from, TileKey to)
    {
        TileKey _return = to -= from;
        // needs to be abs?
        _return /= TileKey.Abs(to);
        return _return;
    }

    public static TileKey Abs(TileKey value)
    {
        if (value.X < 0)
        {
            value.X *= -1;
        }
        if (value.Z < 0)
        {
            value.Z *= -1;
        }

        return value;
    }
    public static int GetDirectionIndex(TileKey from, TileKey to)
    {
        TileKey _directionKey = GetDirection(from, to);
        TileKey[] Surroundingkeys = Types.SurroundingKeys;
        for (int i = 0; i < Surroundingkeys.Length; i++)
        {
            if (Surroundingkeys[i] == _directionKey)
            {
                return i;
            }
        }
        return -1;
    }
    public static TileKey operator +(TileKey first, TileKey second)
    {
        first.X += second.X;
        first.Z += second.Z;
        return first;
    }
    public static TileKey operator -(TileKey first, TileKey second)
    {
        first.X -= second.X;
        first.Z -= second.Z;
        return first;
    }
    public static TileKey operator /(TileKey first, TileKey second)
    {
        if (second.X != 0)
            first.X /= second.X;
        if (second.Z != 0)
            first.Z /= second.Z;
        return first;
    }
    public static TileKey operator *(TileKey first, int second)
    {

        first.X *= second;

        first.Z *= second;
        return first;
    }


    public static bool operator ==(TileKey first, TileKey second)
    {
        if (first.X == second.X && second.Z == first.Z)
        {
            return true;
        }
        return false;
    }
    public static bool operator !=(TileKey first, TileKey second)
    {
        if (first.X != second.X || second.Z != first.Z)
        {
            return true;
        }
        return false;
    }


}



public enum TileMovementTypes
{
    nil,
    Regular,
    Blocked
}

public enum UpgradeTypes
{
    nil,

    dirt,

    grass,

    tree,

    home,

    windmill,

    water,

    sprinkler,

}



[System.Serializable]
public struct UpgradeDescriptions
{
    public UpgradeTypes Upgrade;
    public string Description;
}




public class Types : Singleton<Types>
{
    public static TileKey[] CardinalKeys = new TileKey[4] { TileKey.north, TileKey.east, TileKey.south, TileKey.west };
    public static TileKey[] SurroundingKeys = new TileKey[8] { TileKey.north, TileKey.northEast, TileKey.east, TileKey.southEast, TileKey.south, TileKey.southWest, TileKey.west, TileKey.northWest };

    public UpgradeDescriptions[] UpgradeDescriptions;


    public string GetUpgradeDescription(UpgradeTypes type)
    {
        string _return = "no description for upgrade " + type;

        for (int i = 0; i < UpgradeDescriptions.Length; i++)
        {
            if (type == UpgradeDescriptions[i].Upgrade)
            {
                _return = UpgradeDescriptions[i].Description;
                break;
            }
        }

        return _return;
    }
}