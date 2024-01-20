using UnityEngine;


public class ZoomTrigger : BaseTrigger
{
    [Header("�� ������ (���� ���� Ŭ�����)")]
    public float ZoomSize;
    [Header("�ܿ� �ɸ��� �ð�")]
    public float ZoomTime;

    private float _beforeZoom = -1;

    public override void Enter()
    {
        _beforeZoom = CameraController.Instance.ZoomSize;
        CameraController.Instance.Zoom(ZoomSize, ZoomTime);
    }

    public override void Exit()
    {
        if(_beforeZoom > 0f)
            CameraController.Instance.Zoom(_beforeZoom, ZoomTime);
    }
}