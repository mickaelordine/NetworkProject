using System;
using UnityEngine;
using Photon.Pun;
using Unity.Mathematics;

// CUSTOM STRUCTURES
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



public class CuboidMovement : MonoBehaviourPun, IPunObservable
{
    public static int amountOfCubes = 0;
    
    private Rigidbody m_rb;
    private Vector3 m_NetworkPosition;
    private Quaternion m_NetworkRotation;
    private Vector3 m_NetworkLinearVelocity;
    private Quaternion m_NetworkAngularVelocity;

    private void Start()
    {
        m_rb = GetComponent<Rigidbody>();
        m_NetworkPosition = gameObject.transform.position;
        m_NetworkRotation = gameObject.transform.rotation;
    }
    
    private void Update()
    {
        
    }
    
    private void FixedUpdate()
    {
        bool isNotMoving = NearZeroFloat(m_rb.linearVelocity.sqrMagnitude);
        if (!PhotonNetwork.IsMasterClient && !isNotMoving)
        {
            InterpolateFromMaster();
        }
        
        if (!PhotonNetwork.IsMasterClient)
        {
            m_rb.isKinematic = true;
            return;
        }
        m_rb.isKinematic = false;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            bool isNotMoving = NearZeroFloat(m_rb.linearVelocity.sqrMagnitude);
            stream.SendNext(isNotMoving);
            
            if (!isNotMoving)
            {
                byte[] positionEncoded = Vector3Compression.Encode(transform.position);
                stream.SendNext(positionEncoded);
                byte[] rotationEncoded = QuaternionCompression.Encode(transform.rotation);
                stream.SendNext(rotationEncoded);
            }
        }else
        {
            bool isNotMoving = (bool)stream.ReceiveNext();
            
            if (!isNotMoving)
            {
                byte[] receivePosition = (byte[])stream.ReceiveNext();
                m_NetworkPosition = Vector3Compression.Decode(receivePosition);
                
                byte[] receiveRotation = (byte[])stream.ReceiveNext();
                m_NetworkRotation = QuaternionCompression.Decode(receiveRotation);
            }
            // else
            // {
            //     PhotonNetwork.CleanRpcBufferIfMine(PhotonView.Get(gameObject));
            // }
        }
    }
    
    private void InterpolateFromMaster()
    {
        //snap se la distanza è troppo grande
        if ((transform.position - m_NetworkPosition).sqrMagnitude > 1f) {
            transform.position = m_NetworkPosition;
            transform.rotation = m_NetworkRotation;
        } 
        else 
        {
            //interpola altrimenti
            float lerpFactor = Time.deltaTime * PhotonNetwork.SerializationRate;
            transform.position = Vector3.Lerp(transform.position, m_NetworkPosition, lerpFactor);
            transform.rotation = Quaternion.Slerp(transform.rotation, m_NetworkRotation, lerpFactor);
        }
    }



    private bool NearZeroFloat(float value, float tollerance = 0.05f)
    {
        if (value > tollerance || value < -tollerance)
        {
            return false;
        }
        return true;
    }
    
    private bool NearZero(Vector3 velocity, float tollerance = 0.05f)
    {
        if (NearZeroFloat(velocity.x, tollerance) && NearZeroFloat(velocity.y, tollerance) &&
            NearZeroFloat(velocity.z, tollerance))
        {
            return true;
        }
        return false;
    }
}
