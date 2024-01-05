using UnityEngine;


public class ZoomArea : MonoBehaviour
{
    public float ZoomSize;
    public float ZoomTime;

    private float _beforeZoom = -1;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Player _) && _beforeZoom < 0)
        {
            _beforeZoom = CameraController.Instance.ZoomSize;
            CameraController.Instance.Zoom(ZoomSize, ZoomTime);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Player _) && _beforeZoom > 0)
        {
            CameraController.Instance.Zoom(_beforeZoom, ZoomTime);
            _beforeZoom = -1;
        }
    }
}