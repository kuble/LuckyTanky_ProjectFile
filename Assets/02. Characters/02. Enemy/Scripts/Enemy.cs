using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class Enemy : MonoBehaviour
{
    public GameObject part;
    public int MinPartCount = 6;
    public int MaxPartCount = 10;
    private GameObject _player;
    [SerializeField] private int score;
    private bool bIsDead = false;
    
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void EnemyHit(Vector3 hitDirection)
    {
        if (!bIsDead)
        {
            int count = Random.Range(MinPartCount, MaxPartCount + 1);
            _player = GameObject.Find("Tank");
            _player.GetComponent<ScoreManager>().AddScore(score);
            _player.GetComponent<ScoreManager>().DecreaseRemainEnemy();
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
        bIsDead = true;
    }
}
