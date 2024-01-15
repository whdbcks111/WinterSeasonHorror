

using UnityEngine;

public class MoveTrigger : BaseTrigger
{
    public float MoveSpeed = 10f;
    public Transform MoveTarget;
    public Transform[] MoveDestinations;

    private int _index = 0;
    private bool _isMoving = false;

    public override void Enter()
    {
        if (_isMoving) return;

        _index = 0;
        _isMoving = true;
    }

    private void Update()
    {
        if(_isMoving)
        {
            if(_index >= MoveDestinations.Length)
            {
                _index = 0;
                _isMoving = false;
                return;
            }

            if(MoveTarget.position == MoveDestinations[_index].position)
            {
                _index++;
            }
            else
            {
                MoveTarget.position = Vector3.MoveTowards(MoveTarget.position, MoveDestinations[_index].position, Time.deltaTime * MoveSpeed);
            }
        }
    }

    public override void Exit()
    {
    }
}