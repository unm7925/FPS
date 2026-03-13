using UnityEngine;
public class EnemySight:MonoBehaviour
{
        float sightRange = 200;
        float sightAngle = 90;

        public bool CanSeeTarget(GameObject target)
        {
                Vector3 direction = (target.transform.position - transform.position).normalized;
                float angle = Vector3.Angle(transform.forward, direction);
                if ((sightAngle / 2) < angle) 
                {
                        return false;
                }
                
                RaycastHit hit;
                
                return Physics.Raycast(transform.position, direction, out hit, sightRange)&&
                       hit.collider.gameObject.layer == LayerMask.NameToLayer("Player");
        }
}

