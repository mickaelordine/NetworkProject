using Photon.Pun;
using UnityEngine;

public class RedFadeController : MonoBehaviour
{
    public Renderer targetRenderer;
    public float fadeSpeed = 1.0f;
    private Material mat;
    private float redValue = 0f;

    public float GetRedValue()
    {
        return redValue;
    }

    void Start()
    {
        mat = targetRenderer.material;
        mat.SetFloat("_RedAmount", redValue);
    }

    void Update()
    {
        if (redValue > 0f)
        {
            redValue -= fadeSpeed * Time.deltaTime;
            if (redValue < 0f) redValue = 0f;
            mat.SetFloat("_RedAmount", redValue);
        }
    }

    private void OnCollisionStay(Collision other)
    {
        if (other.gameObject.CompareTag("Character"))
        {
            redValue = 1f;
            mat.SetFloat("_RedAmount", redValue);
        }
        if (other.gameObject.CompareTag("Cuboid"))
        {
            if (other.gameObject.GetComponent<RedFadeController>().GetRedValue() > redValue)
            {
                redValue = other.gameObject.GetComponent<RedFadeController>().GetRedValue() / 2.0f;
                mat.SetFloat("_RedAmount", redValue);
            }
        }
    } 
}