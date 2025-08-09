using System;
using UnityEngine;
using Photon.Pun;

public class Attraction : MonoBehaviour
{
    public float attractionRadius = 5f;
    public float attractionForce = 10f;
    
    [SerializeField]
    private GameObject m_Cube;
    
    private Rigidbody m_rb;

    private void Start()
    {
        m_rb = m_Cube.GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // if(!PhotonNetwork.IsMasterClient)
        //     return;
        Attract();
    }

    private void Attract()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            Collider[] colliders = Physics.OverlapSphere(m_Cube.transform.position, attractionRadius);
            foreach (Collider col in colliders)
            {
                Rigidbody rb = col.attachedRigidbody;
                if (rb != null && rb.gameObject != gameObject)
                {
                    Vector3 direction = (m_Cube.transform.position - rb.position).normalized;
                    rb.AddForce(direction * attractionForce);
                }
            }
        }
    }
}
