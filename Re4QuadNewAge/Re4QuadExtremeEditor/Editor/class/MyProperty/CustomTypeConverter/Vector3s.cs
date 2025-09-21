using System;
using System.ComponentModel;

// A simple struct to hold three short values, representing a 3D vector.
[TypeConverter(typeof(Vector3sConverter))]
public struct Vector3s
{
    public short X { get; set; }
    public short Y { get; set; }
    public short Z { get; set; }

    public Vector3s(short x, short y, short z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public override string ToString()
    {
        return $"{X}, {Y}, {Z}";
    }
}