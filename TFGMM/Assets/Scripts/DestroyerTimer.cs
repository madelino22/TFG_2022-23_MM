using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyerTimer : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField]
    float time = 2;

    private float startTime = 0;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        startTime += Time.deltaTime;

        if (startTime >= time) Destroy(this.gameObject);

    }
}
