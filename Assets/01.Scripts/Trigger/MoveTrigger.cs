

using UnityEngine;

public class MoveTrigger : BaseTrigger
{
    public bool DrawGizmos = false;
    public float MoveSpeed = 10f;
    public Transform MoveTarget;
    public Transform MoveDestinationsParent;

    private int _index = 0;
    private bool _isMoving = false;

    public override void Enter()
    {
        if (_isMoving) return;

        _index = 0;
        _isMoving = true;
    }

    private void OnDrawGizmos()
    {
        if (!DrawGizmos) return;
        if(MoveDestinationsParent == null) return;
        Transform before = null;

        int index = 0;
        foreach (Transform t in MoveDestinationsParent)
        {
            Gizmos.color = Color.Lerp(Color.red, Color.white, (float)index / (MoveDestinationsParent.childCount - 1));
            Gizmos.DrawSphere(t.position, 0.3f);

            if (before != null)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawLine(before.position, t.position);
            }

            before = t;
            index++;
        }
    }

    private void Update()
    {
        if(_isMoving)
        {
            if(_index >= MoveDestinationsParent.childCount)
            {
                _index = 0;
                _isMoving = false;
                return;
            }

            if(MoveTarget.position == MoveDestinationsParent.GetChild(_index).position)
            {
                _index++;
            }
            else
            {
                MoveTarget.position = Vector3.MoveTowards(MoveTarget.position, 
                    MoveDestinationsParent.GetChild(_index).position, Time.deltaTime * MoveSpeed);
            }
        }
    }

    public override void Exit()
    {
    }
}