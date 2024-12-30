using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Tank : MonoBehaviour
{
    
    private struct TrajectoryInformStruct
    {
        public int idx;
        public Vector3 position;
        public bool bIsActive;

        private TrajectoryInformStruct(int idx, Vector3 position, bool bIsActive)
        {
            this.idx = idx;
            this.position = position;
            this.bIsActive = bIsActive;
        }
    }
    
    //조작 관련 프로퍼티
    [Header("[Command]")]
    private Rigidbody _rigidbody;
    
    //발사체 발사 관련  프로퍼티
    [Header("[Projectile]")]
    [SerializeField] private GameObject _tankCannon;
    [SerializeField] private GameObject _cannonBall;
    [SerializeField] private GameObject _targeting;
    [SerializeField] private GameObject _trajectory;
    [SerializeField] private GameObject VFX_CannonSmoke;
    [SerializeField] private MissileData _defaultMissile;
    [SerializeField] private MissileData _bombMissile;
    [SerializeField] private UIManager _uiManager;
    public float Mass = 10.0f;
    public int maxStep = 20;
    public float timeStep = 0.1f;
    private bool bIsTrajectoryInitialized = false;
    public List<GameObject> Objects = new List<GameObject>();
    private float currentPower;
    public bool isPowerOver;
    public List<MissileData> missiles;


    void Awake()
    {
        missiles = new List<MissileData>()
        {
            _defaultMissile, _defaultMissile, _bombMissile, _defaultMissile, _defaultMissile, _bombMissile,
            _defaultMissile, _defaultMissile, _bombMissile, _defaultMissile
        };
    }
    void start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }
    
    //탄환 궤적 예측 표시선
    public void PredictTrajectory(float CurrentPower)
    {
        //예측 궤적의 정보 구조체를 담아둘 리스트
        List<TrajectoryInformStruct> trajectorys = new List<TrajectoryInformStruct>();
        
        Vector3 position = _tankCannon.transform.position;  //canon의 위치 값
        Vector3 velocity = _tankCannon.transform.forward * CurrentPower / Mass;        //뉴턴의 운동 법칙 f = m * a(힘 = 질량 X 속도)

        if (!_targeting.activeSelf)
        {
            _targeting.SetActive(true);
        }
        TrajectoryInformStruct initTrajectoryInform = new TrajectoryInformStruct();
        initTrajectoryInform.idx = 0;
        initTrajectoryInform.position = position;
        initTrajectoryInform.bIsActive = true;
        trajectorys.Add(initTrajectoryInform);              //리스트에 초기 위치 저장

        //시간이 지날때 마다 적용되는 궤적의 위치를 계산하여 리스트에 추가
        for (int i = 1; i < maxStep; i++)
        {
            float timeElapsed = timeStep * i;
            TrajectoryInformStruct trajectoryInform = new TrajectoryInformStruct();
            
            trajectoryInform.idx = i;
            // 등가속도 운동 법칙 s = vt + 0.5 * a * t^2
            trajectoryInform.position = position + velocity * timeElapsed + Physics.gravity * (0.5f * timeElapsed * timeElapsed);
            
            //Active시켜놓을지 저장
            if (!trajectorys[i - 1].bIsActive)
            {
                trajectoryInform.bIsActive = false;
            }
            else
            {
                //궤적에 걸리는 방해물이 있는지 확인.
                if (CheckCollision(trajectorys[i - 1].position, trajectoryInform.position, out Vector3 hitPoint))
                {
                    trajectoryInform.bIsActive = false;
                }
                else
                {
                    trajectoryInform.bIsActive = true;
                }
            }
            trajectorys.Add(trajectoryInform);
        }
        
        bool bIsTargetingPointSet = false;
        if (!bIsTrajectoryInitialized)
        {
            //궤적 표시 오브젝트 생성
            foreach (var trajectory in trajectorys)
            {
                var missile = Instantiate(_trajectory, trajectory.position, Quaternion.identity);
                missile.transform.parent = this.transform.GetChild(1).transform;
                Objects.Add(missile);
                if (trajectory.idx == 0)
                {
                    missile.SetActive(false);
                }
                else
                {
                    missile.SetActive(trajectory.bIsActive);
                    if (!missile.activeSelf && !bIsTargetingPointSet)
                    {
                        bIsTargetingPointSet = true;
                        _targeting = Instantiate(_targeting,new Vector3(trajectory.position.x, 0.0075f, trajectory.position.z), Quaternion.Euler(-90.0f, 0.0f, 0.0f)); 
                    }
                }
            }
            bIsTrajectoryInitialized = true;
            
        }
        else
        {
            for(int idx = 1; idx < trajectorys.Count; idx++)
            {
                Objects[idx].transform.position = trajectorys[idx].position;
                Objects[idx].SetActive(trajectorys[idx].bIsActive);
                if (idx != 0 && !Objects[idx].activeSelf && !bIsTargetingPointSet)
                {
                    bIsTargetingPointSet = true;
                    _targeting.transform.position = new Vector3(trajectorys[idx].position.x, 0.0075f, trajectorys[idx].position.z);
                }
            }   
        }
    }

    private void ResetPredictTrajectory()
    {
        foreach (var obj in Objects)
        {
            obj.SetActive(false);
            _targeting.SetActive(false);
        }
    }
    
    private bool CheckCollision(Vector3 start, Vector3 end, out Vector3 hitPoint)
    {
        hitPoint = end;
        Vector3 direction = end - start;        //점과 점사이의 방향 벡터
        float distance = direction.magnitude;   //점과 점 사이의 거리
        
        if (Physics.Raycast(start, direction.normalized, out RaycastHit hit, distance, 1 << LayerMask.NameToLayer("Ground")))
        {
            hitPoint = hit.point;
            return true;
        }
        return false;
    }

    
    public void Fire(float power)
    {
        if (missiles.Count > 0)
        {
            currentPower = power;
            // 폭발 이펙트 생성
            if (VFX_CannonSmoke != null)
            {
                Instantiate(VFX_CannonSmoke, _tankCannon.transform.position, _tankCannon.transform.rotation);
            }
            GameObject missile = Instantiate(_cannonBall, _tankCannon.transform.position, _tankCannon.transform.rotation);
            missile.GetComponent<Projectile>().bIsBombActivate = missiles[0].isBoomActivate;
            missiles.RemoveAt(0);
            this.gameObject.GetComponent<ScoreManager>().DecreaseRemainMissile();
            _uiManager.SetMissileImage();
            missile.transform.parent = this.transform.GetChild(1).transform;
            missile.GetComponent<Projectile>().LaunchMissile(Mass, _tankCannon.transform.forward * currentPower);
            ResetPredictTrajectory();
        }
        else
        {
            Debug.Log("No missiles found");
            missiles.RemoveAt(0);
            _uiManager.SetMissileImage();
            this.gameObject.GetComponent<ScoreManager>().DecreaseRemainMissile();
        }
    }
}