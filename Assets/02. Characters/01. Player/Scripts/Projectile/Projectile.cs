using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class Projectile : MonoBehaviour
{
    private bool bIsMissileLaunched = false;
    [SerializeField] private Rigidbody rb; // Rigidbody를 명시적으로 참조
    [SerializeField] private GameObject VFX_Explode;
    [SerializeField] private GameObject ExplodeDamageField;
    private Transform missileTransform;
    [SerializeField] private Camera _camera;
    public GameObject part;
    public int MinPartCount = 6;
    public int MaxPartCount = 10;
    private Vector3 Force;
    public bool bIsBombActivate = false;
    private GameObject _soundManager;
    [SerializeField] private AudioClip _launchSound;
    [SerializeField] private AudioClip _explosionSound;
    private string launchSoundID;
    private string explosionSoundID;
    [SerializeField] private Mesh bombMesh;

    private void Awake()
    {
        _soundManager = GameObject.Find("SoundManager");
        launchSoundID = "LaunchSound" + Guid.NewGuid().ToString();
        explosionSoundID = "ExplodeSound"+ Guid.NewGuid().ToString();
        if (bIsBombActivate)
        {
            transform.GetChild(0).gameObject.GetComponent<MeshFilter>().mesh = bombMesh;
        }
    }
    IEnumerator Start()
    {
        GameObject.Find("Tank").GetComponent<TankController>().SetControllable(false);
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }
        missileTransform = transform; // 미사일의 Transform 참조
        _camera = Camera.main;
        _camera.GetComponent<CameraManager>().SetValidCameraID(2);
        
        yield return new WaitForSeconds(2.0f);
        _soundManager.GetComponent<SoundManager>().SetAndPlaySound(explosionSoundID, transform, _explosionSound);
        Destroy(gameObject);
    }

    void Update()
    {
        if (bIsMissileLaunched)
        {
            UpdateMissileRotation();
        }

        if (_camera.GetComponent<CameraManager>().validCameraID == 2)
        {
            _camera.GetComponent<CameraManager>().MoveCameraWithMissile(this.transform);
        }
        
    }

    // 미사일 회전을 이동 방향에 맞게 업데이트
    private void UpdateMissileRotation()
    {
        if (rb.velocity.sqrMagnitude > 0.1f) // 속도가 거의 0인 경우 회전 업데이트 방지
        {
            Quaternion targetRotation = Quaternion.LookRotation(rb.velocity);
            missileTransform.rotation = Quaternion.Lerp(
                missileTransform.rotation, 
                targetRotation, 
                Time.deltaTime * 5f // 회전 속도
            );
        }
    }

    public void LaunchMissile(float mass, Vector3 force)
    {
        bIsMissileLaunched = true;
        rb.mass = mass; // Rigidbody 질량 설정
        Force = force;
        rb.AddForce(Force, ForceMode.Impulse); // 힘을 적용
        _soundManager.GetComponent<SoundManager>().SetAndPlaySound(launchSoundID, transform, _launchSound);
    }
    
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground") || other.gameObject.layer == LayerMask.NameToLayer("Enemy") || other.gameObject.layer == LayerMask.NameToLayer("Prop"))
        {
            //적의 태그가 Enemy인 경우 EnemyHit함수를 호출
            if (other.gameObject.GetComponent<Enemy>() != null)
            {
                Vector3 hitDirection = rb.velocity * -1;
                other.gameObject.GetComponent<Enemy>().EnemyHit(hitDirection);
            }
            //적의 태그가 Prop인 경우 PropHit함수를 호출
            if (other.gameObject.layer == LayerMask.NameToLayer("Prop"))
            {
                Vector3 hitDirection = rb.velocity * -1;
                other.gameObject.GetComponent<HittableProp>().PropHit(hitDirection);
            }
            _soundManager.GetComponent<SoundManager>().SetAndPlaySound(explosionSoundID, this.transform, _explosionSound);
            Destroy(gameObject); // 즉시 미사일 파괴
        }
    }

    private void OnDestroy()
    {
        if (_soundManager.GetComponent<SoundManager>().SoundExists(launchSoundID))
        {
           _soundManager.GetComponent<SoundManager>().StopSound(launchSoundID);   
        }
        _camera.GetComponent<CameraManager>().SetValidCameraID(1);
        if (bIsBombActivate)
        {
            ClusterMissileBomb();   
        }
        
        //폭발 이펙트 호출
        if (VFX_Explode != null)
        {
            Instantiate(VFX_Explode, transform.position, Quaternion.identity);
        }
        GameObject.Find("Tank").GetComponent<TankController>().SetControllable(true);
    }
    
    public void ClusterMissileBomb()
    {
        Vector3 hitDirection = this.transform.up * -1;
        int count = Random.Range(MinPartCount, MaxPartCount + 1);

        for (int i = 0; i < count; ++i)
        {
            Vector3 randomDiretion = Random.insideUnitSphere;
            Vector3 lastDiretion = Quaternion.LookRotation(randomDiretion) 
                                   * hitDirection;
        
            GameObject instance = Instantiate(part, transform.position, Random.rotation);
            instance.GetComponent<Rigidbody>().AddForce(Random.onUnitSphere * 0.1f, ForceMode.Impulse); // 힘을 적용
            
        }
        Destroy(this.gameObject);
    }
    
}
