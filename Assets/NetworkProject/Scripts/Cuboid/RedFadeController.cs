using Photon.Pun;
using UnityEngine;

public class RedFadeController : MonoBehaviourPun, IPunObservable
{
    public Renderer targetRenderer;
    public float fadeSpeed = 1.0f;

    private float redValue = 0f;
    private MaterialPropertyBlock mpb;

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
        if (redValue > 0f)
        {
            redValue -= fadeSpeed * Time.deltaTime;
            if (redValue < 0f) redValue = 0f;

            UpdateColor();
        }
    }

    private void OnCollisionStay(Collision other)
    {
        if (other.gameObject.CompareTag("Character"))
        {
            redValue = 1f;
            UpdateColor();
        }

        if (other.gameObject.CompareTag("Cuboid"))
        {
            var otherCtrl = other.gameObject.GetComponent<RedFadeController>();
            if (otherCtrl != null && otherCtrl.GetRedValue() > redValue)
            {
                redValue = otherCtrl.GetRedValue() / 2.0f;
                UpdateColor();
            }
        }
    }

    private void UpdateColor()
    {
        // Interpola tra bianco e rosso
        Color c = Color.Lerp(Color.white, Color.red, redValue);

        // Aggiorna il MaterialPropertyBlock
        targetRenderer.GetPropertyBlock(mpb);
        mpb.SetColor("_BaseColor", c);
        targetRenderer.SetPropertyBlock(mpb);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(redValue);
        }
        else
        {
            redValue = (float)stream.ReceiveNext();
        }
    }
}