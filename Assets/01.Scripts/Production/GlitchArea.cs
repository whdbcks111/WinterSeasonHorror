using UnityEngine;


public class GlitchArea : MonoBehaviour
{
    public float GlitchStrength;
    public float SmoothTime;

    private float _beforeGlitch = -1;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Player _) && _beforeGlitch < 0)
        {
            _beforeGlitch = CameraController.Instance.GlitchStrength;
            CameraController.Instance.SetGlitch(GlitchStrength, SmoothTime);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Player _) && _beforeGlitch >= 0)
        {
            CameraController.Instance.SetGlitch(_beforeGlitch, SmoothTime); 
            _beforeGlitch = -1;
        }
    }
}