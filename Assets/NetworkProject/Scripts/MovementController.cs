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
    private Vector3 networkPosition;
    private Quaternion networkRotation;
    private Vector3 networkLinearVelocity;
    private Vector3 networkAngularVelocity;

    private void Start()
    {
        m_rb = m_Cube.GetComponent<Rigidbody>();
    }
    
    void FixedUpdate()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            InterpolateFromMaster();
            Debug.Log("Not Master Client");
            return;
        }
        Debug.Log("Master Client");
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
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            // stream.SendNext(m_rb.linearVelocity);
            // stream.SendNext(m_rb.angularVelocity);
        }else
        {
            networkPosition = (Vector3)stream.ReceiveNext();
            networkRotation = (Quaternion)stream.ReceiveNext();
            // networkLinearVelocity = (Vector3)stream.ReceiveNext();
            // networkAngularVelocity = (Vector3)stream.ReceiveNext();
        }
    }

    private void InterpolateFromMaster()
    {
        transform.position = Vector3.Lerp(transform.position, networkPosition, Time.deltaTime * PhotonNetwork.SerializationRate);
        transform.rotation = Quaternion.Lerp(transform.rotation, networkRotation, Time.deltaTime * PhotonNetwork.SerializationRate);
    }
}
