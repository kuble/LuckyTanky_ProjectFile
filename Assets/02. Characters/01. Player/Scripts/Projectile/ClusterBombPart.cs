using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ClusterBombPart : MonoBehaviour
{
    private Rigidbody rb;
    [SerializeField] private GameObject VFX_Explode;
    [SerializeField] private GameObject ExplodeDamageField;
    private GameObject _soundManager;
    [SerializeField] private AudioClip _explosionSound;

    private string explosionSoundID;
    // Start is called before the first frame update
    IEnumerator Start()
    {
        _soundManager = GameObject.Find("SoundManager");
        explosionSoundID = "ExplodeSound"+ Guid.NewGuid().ToString();
        rb = this.GetComponent<Rigidbody>();
        yield return new WaitForSeconds(5f);
        Destroy(this.gameObject); 
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground") || other.gameObject.layer == LayerMask.NameToLayer("Enemy") || other.gameObject.layer == LayerMask.NameToLayer("Prop"))
        {
            if (VFX_Explode != null)
            {
                Instantiate(VFX_Explode, transform.position, Quaternion.identity);
            }
            _soundManager.GetComponent<SoundManager>().SetAndPlaySound(explosionSoundID, transform, _explosionSound);
            Destroy(gameObject); // 즉시 미사일 파괴
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
