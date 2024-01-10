

using UnityEngine;

public class ActiveTrigger : MonoBehaviour
{
    public GameObject[] TargetObjects;
    public ActiveType Type = ActiveType.ActiveOnTrigger;

    private bool _alreadyTriggered = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Player _) && !_alreadyTriggered)
        {
            _alreadyTriggered = true;

            foreach (var obj in TargetObjects)
            {
                obj.SetActive(Type == ActiveType.ActiveOnTrigger);
            }
        }
    }

    public enum ActiveType { ActiveOnTrigger, DeactiveOnTrigger }
}