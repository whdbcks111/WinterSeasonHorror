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
        print(_beforeZoom);
        CameraController.Instance.Zoom(ZoomSize, ZoomTime);
    }

    public override void Exit()
    {
        CameraController.Instance.Zoom(_beforeZoom, ZoomTime);
    }
}