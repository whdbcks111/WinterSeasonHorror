using UnityEngine;


public class GlitchTrigger : BaseTrigger
{
    [Header("글리치 강도")]
    [Range(4f, 15f)]
    public float GlitchStrength;
    public float SmoothTime;

    private float _beforeGlitch = -1;

    public override void Enter()
    {
        _beforeGlitch = CameraController.Instance.GlitchStrength;
        CameraController.Instance.SetGlitch(GlitchStrength, SmoothTime);
    }

    public override void Exit()
    {
        CameraController.Instance.SetGlitch(_beforeGlitch, SmoothTime);
        _beforeGlitch = -1;
    }

}