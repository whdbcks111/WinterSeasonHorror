using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PrtPlayer : MonoBehaviour
{
    float Xdir;
    float Ydir;
    [SerializeField] float Speed;
    [SerializeField] float OffSpeed;
    Rigidbody rb;

    Light2D ligh;
    bool ThunderOff;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        ligh = GetComponent<Light2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Xdir = Input.GetAxisRaw("Horizontal");// < 0 ? -1 : 1;
        Ydir = Input.GetAxisRaw("Vertical"); //< 0 ? -1 : 1;

        Debug.Log(Xdir + ", "+ Ydir);
        transform.Translate(new Vector3(Xdir, Ydir) *Speed * Time.deltaTime);

        if(Input.GetKeyDown(KeyCode.J)) {
            ligh.intensity = 100;
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            ligh.intensity = 0;
        }

        if(Input.GetKeyDown (KeyCode.N)) {
            StartCoroutine(Thunder());
        }

        if(ThunderOff)
        {

            Mathf.Lerp(1, 100, 0.5f);
            
        }
    }
    IEnumerator Thunder()
    {
        ThunderOff = false;
        ligh.intensity = 10;
        yield return new WaitForSeconds(0.5f);
        for(int i = 0; i < 20; i++) {
            ligh.intensity -= i * 0.5f;
            yield return new WaitForSeconds(0.025f + i * 0.01f);
        }

        ThunderOff = true;

    }
}
