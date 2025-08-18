using Photon.Pun;
using UnityEngine;

public class RedFadeController : MonoBehaviourPun, IPunObservable
{
    public Renderer targetRenderer;
    public float fadeSpeed = 1.0f;
    

    private float redValue = 0f;
    private MaterialPropertyBlock mpb;
    private bool m_IsTouchingSomething = false;
    private bool m_IsTouchingSomethingNetwork = false;

    public float GetRedValue()
    {
        return redValue;
    }

    void Awake()
    {
        mpb = new MaterialPropertyBlock();
    }

    void Update()
    {
        if (PhotonNetwork.IsMasterClient || !m_IsTouchingSomethingNetwork)
        {
            if (redValue > 0f)
            {
                redValue -= fadeSpeed * Time.deltaTime;
                if (redValue < 0f) redValue = 0f;
            }
        }
        UpdateColor();
    }

    private void OnCollisionStay(Collision other)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        if (other.gameObject.CompareTag("Character"))
        {
            redValue = 1f;
            m_IsTouchingSomething = true;
            return;
        }
        else
        {
            m_IsTouchingSomething = false;
        }

        if (other.gameObject.CompareTag("Cuboid"))
        {
            var otherCtrl = other.gameObject.GetComponent<RedFadeController>();
            if (otherCtrl != null && otherCtrl.GetRedValue() > redValue)
            {
                redValue = otherCtrl.GetRedValue() / 2.0f;
            }
            m_IsTouchingSomething = true;
        }
        else
        {
            m_IsTouchingSomething = false;
        }
    }

    private void UpdateColor()
    {
        Color c = Color.Lerp(Color.white, Color.red, redValue);

        targetRenderer.GetPropertyBlock(mpb);
        mpb.SetColor("_BaseColor", c);
        targetRenderer.SetPropertyBlock(mpb);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        
        if (stream.IsWriting)
        {
            stream.SendNext(m_IsTouchingSomething);
            if (m_IsTouchingSomething)
            {
                stream.SendNext(redValue);
            }
        }
        else
        {
            m_IsTouchingSomethingNetwork = (bool)stream.ReceiveNext();
            if(m_IsTouchingSomethingNetwork)
                redValue = (float)stream.ReceiveNext();
        }
    }
}