using System;
using UnityEngine;
using Photon.Pun;
using Unity.Mathematics;

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
        }
        else
        {
            bool isNotMoving = (bool)stream.ReceiveNext();
            if (!isNotMoving)
            {
                byte[] receivePosition = (byte[])stream.ReceiveNext();
                m_NetworkPosition = Vector3Compression.Decode(receivePosition);
                
                byte[] receiveRotation = (byte[])stream.ReceiveNext();
                m_NetworkRotation = QuaternionCompression.Decode(receiveRotation);
            }
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
