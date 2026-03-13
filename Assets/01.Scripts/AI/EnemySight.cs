using UnityEngine;
public class EnemySight:MonoBehaviour
{
        float sightRange = 20;
        float sightAngle = 90;
        Transform target;

        public bool CanSeeTarget()
        {
                float distance = Vector3.Distance(transform.position, target.position);
                if (distance > sightRange) 
                {
                        return false;
                }
                Vector3 direction = (target.position - transform.position).normalized;
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

