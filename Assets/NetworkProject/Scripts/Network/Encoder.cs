using System;
using UnityEngine;
using Photon.Pun;
using Unity.Mathematics;


// CUSTOM STRUCTURES

//VECTOR3
public static class Vector3Compression
{
    // Valore massimo assoluto atteso (range di normalizzazione)
    public const float valueRange = 1f;

    public static byte[] Encode(Vector3 v)
    {
        byte[] bytes = new byte[6];

        short x = FloatToShort(v.x);
        short y = FloatToShort(v.y);
        short z = FloatToShort(v.z);

        // Copio short nei byte (Big Endian)
        Buffer.BlockCopy(BitConverter.GetBytes(x), 0, bytes, 0, 2);
        Buffer.BlockCopy(BitConverter.GetBytes(y), 0, bytes, 2, 2);
        Buffer.BlockCopy(BitConverter.GetBytes(z), 0, bytes, 4, 2);

        return bytes;
    }

    public static Vector3 Decode(byte[] bytes)
    {
        if (bytes == null || bytes.Length != 6)
            throw new ArgumentException("Byte array deve avere lunghezza 6");

        short x = BitConverter.ToInt16(bytes, 0);
        short y = BitConverter.ToInt16(bytes, 2);
        short z = BitConverter.ToInt16(bytes, 4);

        return new Vector3(ShortToFloat(x), ShortToFloat(y), ShortToFloat(z));
    }

    private static short FloatToShort(float f)
    {
        f = Mathf.Clamp(f, -valueRange, valueRange);
        return (short)(f / valueRange * short.MaxValue);
    }

    private static float ShortToFloat(short s)
    {
        return (float)s / short.MaxValue * valueRange;
    }
}


//QUTERNIONS
public static class QuaternionCompression
{
    // Valore massimo assoluto atteso (range di normalizzazione)
    public const float valueRange = 1f;

    public static byte[] Encode(Quaternion q)
    {
        byte[] bytes = new byte[8];

        short x = FloatToShort(q.x);
        short y = FloatToShort(q.y);
        short z = FloatToShort(q.z);
        short w = FloatToShort(q.w);

        // Copio short nei byte (Big Endian)
        Buffer.BlockCopy(BitConverter.GetBytes(x), 0, bytes, 0, 2);
        Buffer.BlockCopy(BitConverter.GetBytes(y), 0, bytes, 2, 2);
        Buffer.BlockCopy(BitConverter.GetBytes(z), 0, bytes, 4, 2);
        Buffer.BlockCopy(BitConverter.GetBytes(w), 0, bytes, 6, 2);

        return bytes;
    }

    public static Quaternion Decode(byte[] bytes)
    {
        if (bytes == null || bytes.Length != 8)
            throw new ArgumentException("Byte array deve avere lunghezza 8");

        short x = BitConverter.ToInt16(bytes, 0);
        short y = BitConverter.ToInt16(bytes, 2);
        short z = BitConverter.ToInt16(bytes, 4);
        short w = BitConverter.ToInt16(bytes, 6);

        return new Quaternion(ShortToFloat(x), ShortToFloat(y), ShortToFloat(z), ShortToFloat(w));
    }

    private static short FloatToShort(float f)
    {
        f = Mathf.Clamp(f, -valueRange, valueRange);
        return (short)(f / valueRange * short.MaxValue);
    }

    private static float ShortToFloat(short s)
    {
        return (float)s / short.MaxValue * valueRange;
    }
}