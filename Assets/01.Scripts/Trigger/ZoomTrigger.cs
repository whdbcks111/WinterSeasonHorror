using UnityEngine;


public class ZoomTrigger : BaseTrigger
{
    [Header("줌 사이즈 (작을 수록 클로즈업)")]
    public float ZoomSize;
    [Header("줌에 걸리는 시간")]
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