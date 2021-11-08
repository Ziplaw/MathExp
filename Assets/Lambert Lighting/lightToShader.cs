using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[ExecuteInEditMode]
public class lightToShader : MonoBehaviour
{
    List<Renderer> renderers;

    // Update is called once per frame
    void Update()
    {
        renderers = (from r in FindObjectsOfType<Renderer>() where r.gameObject.CompareTag("lightable") select r).ToList();

        foreach (var r in renderers)
        {
            MaterialPropertyBlock p = new MaterialPropertyBlock();
            r.GetPropertyBlock(p);
            p.SetVector("_LambertPosition",transform.position);
            r.SetPropertyBlock(p);
        }
    }
}
