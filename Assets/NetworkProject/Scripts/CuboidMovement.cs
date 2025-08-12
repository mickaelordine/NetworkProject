using System;
using UnityEngine;
using Photon.Pun;

public class CuboidMovement : MonoBehaviourPun, IPunObservable
{
    
    private Rigidbody m_rb;
    private Vector3 m_NetworkPosition;
    private Quaternion m_NetworkRotation;

    private void Start()
    {
        m_rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            m_rb.isKinematic = true;
            //InterpolateFromMaster();
        }
        m_rb.isKinematic = false;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting && !NearZero(m_rb.linearVelocity))
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            // stream.SendNext(m_rb.linearVelocity);
            // stream.SendNext(m_rb.angularVelocity);
        }else
        {
            m_NetworkPosition = (Vector3)stream.ReceiveNext();
            m_NetworkRotation = (Quaternion)stream.ReceiveNext();
            // networkLinearVelocity = (Vector3)stream.ReceiveNext();
            // networkAngularVelocity = (Vector3)stream.ReceiveNext();
        }
    }
    
    private void InterpolateFromMaster()
    {
        transform.position = Vector3.Lerp(transform.position, m_NetworkPosition, Time.deltaTime * PhotonNetwork.SerializationRate);
        transform.rotation = Quaternion.Lerp(transform.rotation, m_NetworkRotation, Time.deltaTime * PhotonNetwork.SerializationRate);
    }



    private bool NearZeroFloat(float value, float tollerance = 0.001f)
    {
        if (value > tollerance || value < -tollerance)
        {
            return false;
        }
        return true;
    }
    
    private bool NearZero(Vector3 velocity, float tollerance = 0.001f)
    {
        if (NearZeroFloat(velocity.x, tollerance) && NearZeroFloat(velocity.y, tollerance) &&
            NearZeroFloat(velocity.z, tollerance))
        {
            return true;
        }
        return false;
    }
    
}
