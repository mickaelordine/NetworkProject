using System;
using UnityEngine;
using UnityEngine.UIElements;

public class FollowCamera : MonoBehaviour
{
    [SerializeField]
    private GameObject m_CubeCharacter;

    private Vector3 m_DistanceToKeep;
    
    //to use for the correct distance for the cube instantiation with photon
    private Vector3 m_StartPosition = new Vector3(0, 0, 0);

    private void Start()
    {
        m_DistanceToKeep = m_CubeCharacter.transform.position - transform.position;
        //m_DistanceToKeep.z = m_CubeCharacter.transform.position.z;
    }

    private void Update()
    {
        FollowPlayerWithSmoothEffect();
    }

    void FollowPlayerWithSmoothEffect()
    {
        Vector3 targetPos = m_CubeCharacter.transform.position - m_DistanceToKeep;
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * 2f);
    }
    
}
