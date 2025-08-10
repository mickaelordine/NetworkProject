using System;
using UnityEngine;
using Photon.Pun;

public class NotifyCollision : MonoBehaviour
{
    [SerializeField]
    private GameObject m_Parent;

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Floor"))
        {
            if(m_Parent == null)
                return;
            m_Parent.GetComponent<MovementController>().SetGrounded(true);
        }
    }
}