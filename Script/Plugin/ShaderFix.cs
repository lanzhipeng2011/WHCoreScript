using UnityEngine;
using System.Collections;

public class ShaderFix : MonoBehaviour {

    private void Awake()
    {
#if UNITY_EDITOR
        foreach (Transform t in gameObject.GetComponentsInChildren<Transform>())
        {
            if (t.gameObject.renderer != null)
            {
                t.gameObject.renderer.material.shader = Shader.Find(t.gameObject.renderer.material.shader.name);
            }
        }
#endif
    }
}
