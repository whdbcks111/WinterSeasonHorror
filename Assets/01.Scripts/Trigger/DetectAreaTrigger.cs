using UnityEngine;

[RequireComponent (typeof(Collider2D))]
public abstract class DetectAreaTrigger : BaseTrigger
{
    public bool TriggerOnce = false;

    [SerializeField] public int EnterCount = 0;
    
    private bool _isEntered = false;

    public BaseTrigger[] TargetTriggers;

    protected abstract bool CheckCollision(Collider2D collider);

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (_isEntered) return;
        if (CheckCollision(collision))
        {
            if (EnterCount > 0 && TriggerOnce) return;

            EnterCount++;
            _isEntered = true;

            Enter();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (CheckCollision(collision))
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