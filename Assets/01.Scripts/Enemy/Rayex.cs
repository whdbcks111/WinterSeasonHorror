using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class Rayex : MonoBehaviour
{
    public float _rayLength;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // ����ĳ��Ʈ�� �����Ͽ� �浹 ���� ���
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, _rayLength))
        {
            // �浹 ������ ǥ���� ���� ����
            Debug.Log("���̰� " + hit.collider.gameObject.name + "��(��) �浹�߽��ϴ�.");

            // ���⼭ �ʿ��� �߰� ������ ������ �� �ֽ��ϴ�.
        }

        // ���� �׸��� (����׿�)
        Debug.DrawRay(transform.position, transform.forward * _rayLength, Color.red);
    }
}
