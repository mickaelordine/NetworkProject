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


// public class Instantiator : MonoBehaviour, IPunPrefabPool
// {
//     public GameObject Instantiate(string prefabId, Vector3 position, Quaternion rotation)
//     {
//         throw new NotImplementedException();
//     }
//
//     public void Destroy(GameObject gameObject)
//     {
//         throw new NotImplementedException();
//     }
// }
