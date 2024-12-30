using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionDamage : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground") || other.gameObject.layer == LayerMask.NameToLayer("Enemy") || other.gameObject.layer == LayerMask.NameToLayer("Prop"))
        {
            //적의 태그가 Enemy인 경우 EnemyHit함수를 호출
            if (other.gameObject.GetComponent<Enemy>() != null)
            {
                Vector3 hitDirection = this.transform.forward;
                other.gameObject.GetComponent<Enemy>().EnemyHit(hitDirection);
            }
            //적의 태그가 Prop인 경우 PropHit함수를 호출
            if (other.gameObject.layer == LayerMask.NameToLayer("Prop"))
            {
                Vector3 hitDirection = this.transform.forward;
                other.gameObject.GetComponent<HittableProp>().PropHit(hitDirection);
            }
            Destroy(this.gameObject);
        }
    }
}
