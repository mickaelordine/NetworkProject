using System;
using UnityEngine;
using Photon.Pun;

public class MovementController : MonoBehaviourPun, IPunObservable
{
    [Header("Movement Settings")]
    public float moveSpeed = 8f;
    public float jumpForce = 8f;
    
    [SerializeField]
    private GameObject m_Cube;  
    
    [Header("JumpImpulse Settings")]
    [SerializeField]
    private float shockwaveRadius = 5f;
    [SerializeField]
    private float shockwaveForce = 300f;
    
    private bool isGrounded = true;
    private Rigidbody m_rb;
    
    // NETWORK MEMBERS
    private Vector3 m_NetworkPosition;
    private Quaternion m_NetworkRotation;
    private Vector3 m_NetworkLinearVelocity;
    private Vector3 m_NetworkAngularVelocity;

    private void Start()
    {
        m_rb = m_Cube.GetComponent<Rigidbody>();
        m_NetworkPosition = transform.position;
        m_NetworkRotation = transform.rotation;
    }

    void FixedUpdate()
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
        Movement();
        Jump();
    }

    // MOVEMENT FUNCTIONS
    private void Movement()
    {
        if (Input.GetKey(KeyCode.W))
        {
            Vector3 point = m_Cube.transform.position + transform.up * 0.5f;
            m_rb.AddForceAtPosition(transform.forward * moveSpeed, point, ForceMode.Force);
        }
        
        if (Input.GetKey(KeyCode.S))
        {
            Vector3 point = m_Cube.transform.position + transform.up * 0.5f;
            m_rb.AddForceAtPosition(-transform.forward * moveSpeed, point, ForceMode.Force);
        }
        
        if (Input.GetKey(KeyCode.A))
        {
            Vector3 point = m_Cube.transform.position + transform.up * 0.5f;
            m_rb.AddForceAtPosition(-transform.right * moveSpeed, point, ForceMode.Force);
        }
        
        if (Input.GetKey(KeyCode.D))
        {
            Vector3 point = m_Cube.transform.position + transform.up * 0.5f;
            m_rb.AddForceAtPosition(transform.right * moveSpeed, point, ForceMode.Force);
        }
    }

    // JUMP FUNCTIONS
    private void Jump()
    {
        if (Input.GetKey(KeyCode.Space) && isGrounded)
        {
            m_rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            GenerateJumpImpulse();
            isGrounded = false;
        }
    }
    
    private void GenerateJumpImpulse()
    {
        Collider[] hitColliders = Physics.OverlapSphere(m_Cube.transform.position, shockwaveRadius);
        foreach (Collider hit in hitColliders)
        {
            Rigidbody rbHit = hit.attachedRigidbody;
            if (rbHit != null && rbHit != m_rb)
            {
                rbHit.AddExplosionForce(shockwaveForce, m_Cube.transform.position, shockwaveRadius, 1f, ForceMode.Impulse);
            }
        }
    }

    public void SetGrounded(bool isGrounded)
    {
        this.isGrounded = isGrounded;
    }
    
    
    // NETWORKFUNCTIONS

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
