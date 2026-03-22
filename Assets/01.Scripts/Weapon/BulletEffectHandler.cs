using System;
using System.Collections;
using UnityEngine;
public class BulletEffectHandler:MonoBehaviour
{
    [SerializeField]private Transform muzzle;
    [SerializeField]private GameObject impactEffectPrefab;

    private float waitFadeEffect = 0.005f;
    private float fadeSpeed = 20f;
    private LineRenderer lineRenderer;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    public void ShowTracer( Vector3 to)
    {
        if (!gameObject.activeInHierarchy) return;
        lineRenderer.material.color = new Color
            (lineRenderer.material.color.r, lineRenderer.material.color.g, lineRenderer.material.color.b, 1f);
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, muzzle.position);
        lineRenderer.SetPosition(1, to);
        StartCoroutine(DeleteTracer());
    }
    private IEnumerator DeleteTracer()
    {
        yield return new WaitForSeconds(waitFadeEffect);

        while (true) 
        {
            Color color = lineRenderer.material.color;
            color.a -= fadeSpeed*Time.deltaTime;
            lineRenderer.material.color = color;
            if (color.a <= 0) 
            {
                break;
            }
            yield return null;
        }
        
        lineRenderer.enabled = false;
    }

    public void ShowImpact(RaycastHit hit)
    {
        if (impactEffectPrefab == null) return;
        IDamageable damageable = hit.collider.GetComponent<IDamageable>();
        if (damageable == null) 
        {
             Instantiate(impactEffectPrefab, hit.point, Quaternion.LookRotation(hit.normal));
        }
    }
}

