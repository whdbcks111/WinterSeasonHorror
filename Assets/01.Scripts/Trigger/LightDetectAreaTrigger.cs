using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class LightDetectAreaTrigger : BaseTrigger
{
    public bool TriggerOnce = false;

    private int _enterCount = 0;
    private bool _isEntered = false;

    public BaseTrigger[] TargetTriggers;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("HandLight"))
        {
            if (_enterCount > 0 && TriggerOnce) return;
            if (_isEntered) return;

            _enterCount++;
            _isEntered = true;

            Enter();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("HandLight"))
        {
            _isEntered = false;

            Exit();
        }
    }

    public override void Enter()
    {
        foreach (var trigger in TargetTriggers)
        {
            trigger.Enter();
        }

    }

    public override void Exit()
    {
        foreach (var trigger in TargetTriggers)
        {
            trigger.Exit();
        }

    }
}