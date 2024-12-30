using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;

public class HittableProp : MonoBehaviour
{
    [SerializeField] private GameObject part;
    [SerializeField] private int MinPartCount = 6;
    [SerializeField] private int MaxPartCount = 10;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void PropHit(Vector3 hitDirection)
    {
        int count = Random.Range(MinPartCount, MaxPartCount + 1);

        for (int i = 0; i < count; ++i)
        {
            Vector3 randomDiretion = Random.insideUnitSphere;
            Vector3 lastDiretion = Quaternion.LookRotation(randomDiretion) 
                                   * hitDirection;
        
            GameObject instance = Instantiate(part, transform.position,
                Quaternion.LookRotation(lastDiretion));

            instance.GetComponent<Rigidbody>().AddForce(lastDiretion, ForceMode.Impulse);
            Destroy(this.gameObject);
        }
    }
}
