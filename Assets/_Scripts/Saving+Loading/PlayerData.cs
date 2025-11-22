using UnityEngine;
using System.Xml.Serialization;

[System.Serializable]
public class PlayerData
{
    public TransformData Transform;
    public int Health;
    public bool IsWeaponEquipped;
    public Vector3Data WeaponPosition;
    public Vector3Data WeaponRotation;
}

[System.Serializable]
public class TransformData
{
    public Vector3Data Position;
    public Vector3Data Rotation;
    public Vector3Data Scale;
}

[System.Serializable]
public class Vector3Data
{
    public float X;
    public float Y;
    public float Z;

    public Vector3Data() { }

    public Vector3Data(Vector3 v)
    {
        X = v.x;
        Y = v.y;
        Z = v.z;
    }

    public Vector3 ToVector3()
    {
        return new Vector3(X, Y, Z);
    }
}

