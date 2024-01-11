using UnityEngine;


public class CameraFocusTrigger : BaseTrigger
{
    public Transform FocusTarget;
    public float FocusMoveTime;

    private Transform _beforeFocus = null;

    public override void Enter()
    {
        _beforeFocus = CameraController.Instance.FocusTarget;
        CameraController.Instance.SetFocus(FocusTarget, FocusMoveTime);
    }

    public override void Exit()
    {
        CameraController.Instance.SetFocus(_beforeFocus, FocusMoveTime);
        _beforeFocus = null;
    }
}