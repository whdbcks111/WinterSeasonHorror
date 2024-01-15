using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class Statue : MonoBehaviour
{
    public float radius = 1.0f;
    public float maxDistance = 5.0f;


    Believer _believer;
    Player _player;
    [SerializeField] private Vector2 _boxSize;
    [SerializeField] private Transform _boxPosition;

    [SerializeField] private GameObject _eyeSphere;
    [SerializeField] private GameObject _pupil;


    [Header("¼ö³à»ó Á¾·ù")]
    [SerializeField] GameObject[] _statuePresets;
    private GameObject _selectedStatue;
    [SerializeField] GameObject _statuePreview;

    private void Start()
    {
        _player = Player.Instance;

        _selectedStatue = SelectPreset();
        _statuePreview.SetActive(false);
        
        _pupil.SetActive(false);
        
    }
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.V))
        {
            AttackOrder();
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            ActivateEye();
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            DeactivateEye();
        }



    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(_boxPosition.position, _boxSize);
    }

    private void ActivateEye()
    {
        _pupil.SetActive(true);
    }
    private void DeactivateEye()
    {
        _pupil.SetActive(false);
    }
    void AttackOrder()
    {
        RaycastHit2D[] hits = Physics2D.BoxCastAll(_boxPosition.position, _boxSize,0,Vector2.zero);
        foreach(RaycastHit2D hit in hits)
        {
            if(hit.collider.gameObject.TryGetComponent<Believer>(out Believer BelieverScript) && BelieverScript._fsm.CurrentState != BelieverScript._threteningHide && BelieverScript._fsm.CurrentState != BelieverScript._chaseState)
            {
                BelieverScript._fsm.CurrentState = BelieverScript._chaseState;
            }
        }
    }
    private GameObject SelectPreset()
    {
        int Selecter = Random.Range(0, _statuePresets.Length);
        GameObject SelectedPreset = Instantiate(_statuePresets[Selecter],transform.position,Quaternion.identity);
        SelectedPreset.transform.parent = gameObject.transform;

        switch (Selecter) {
            case 0:
                Instantiate(_pupil, transform.position, Quaternion.identity);
                Instantiate(_eyeSphere, transform.position, Quaternion.identity);
                break;
            case 1:
                Instantiate(_pupil, transform.position, Quaternion.identity);
                Instantiate(_eyeSphere, transform.position, Quaternion.identity);
                break;
            case 2:
                Instantiate(_pupil, transform.position, Quaternion.identity);
                Instantiate(_eyeSphere, transform.position, Quaternion.identity);
                break;
            case 3:
                Instantiate(_pupil, transform.position, Quaternion.identity);
                Instantiate(_eyeSphere, transform.position, Quaternion.identity);
                break;
            default:
                break;
        }


        return SelectedPreset;
    }
}
