using System;
using UnityEngine;
using Photon.Pun;

public class Levitation : MonoBehaviourPun, IPunObservable
{
    [SerializeField] 
    private GameObject m_Cube;
    [SerializeField]
    private Rigidbody m_rb;
    
    [Header("Levitation Settings")]
    [SerializeField]
    private float distanceToKeep = 0.0f;
    [SerializeField]
    private float damping = 5f;
    public float levitationForce = 0.5f;
    
    [Header("Levitation Impulse Settings")]
    [SerializeField]
    private float levitationImpulseRadius = 1.0f;
    [SerializeField]
    private float levitationImpulseForce = 0.1f;
    
    

    void FixedUpdate()
    {
        if(!PhotonNetwork.IsMasterClient)
            return;
        Levitate();
    }

    void Levitate()
    {
        if(m_Cube == null)
            return;
        if (Input.GetKey(KeyCode.Mouse1))
        {
            if (m_rb == null) return;

            if (m_Cube.transform.position.y <= distanceToKeep)
            {
                m_rb.freezeRotation = true;
                float currentHeight = m_rb.position.y;
                
                float heightError = distanceToKeep - currentHeight;
                
                float forceUp = levitationForce * heightError;
                
                float verticalVelocity = m_rb.linearVelocity.y;
                float dampingForce = damping * verticalVelocity;
                
                float totalForce = forceUp - dampingForce;
                
                m_rb.AddForce(Vector3.up * totalForce, ForceMode.Force);
                GenerateLevitateImpulse();
            }
        }
        m_rb.freezeRotation = false;
    }

    void GenerateLevitateImpulse()
    {
        Vector3 position = new Vector3(m_Cube.transform.position.x, m_Cube.transform.position.y - 0.5f, m_Cube.transform.position.z);
        Collider[] hitColliders = Physics.OverlapSphere(position, levitationImpulseRadius);
        foreach (Collider hit in hitColliders)
        {
            Rigidbody rbHit = hit.attachedRigidbody;
            if (rbHit != null && rbHit != m_rb)
            {
                rbHit.AddExplosionForce(levitationImpulseForce, position, levitationImpulseRadius, 1f, ForceMode.Impulse);
            }
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        
    }
}
