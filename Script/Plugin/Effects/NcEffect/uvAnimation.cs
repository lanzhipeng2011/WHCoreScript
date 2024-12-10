using UnityEngine;
using System.Collections;

//Decompile by Si Borokokok

public class uvAnimation : MonoBehaviour
{
    public int materialIndex;
    public string textureName = "_MainTex";
    public Vector2 uvAnimationRate = new Vector2(1f, 0f);
    private Vector2 uvOffset = Vector2.zero;

	private Renderer r;
    private Material m_IndexMaterial;

	void Start(){
		r = renderer;
        m_IndexMaterial = r.sharedMaterials[materialIndex]; 
	}

    private void Update()
    {
		uvOffset += (uvAnimationRate * Time.deltaTime);
		if (uvOffset.x > 1)
		{
			uvOffset.x -= 1;
		}
		if (uvOffset.y > 1)
		{
			uvOffset.y -= 1;
		}
		if (r.enabled)
		{
			m_IndexMaterial.SetTextureOffset(textureName, uvOffset);
		}
	}
}


 

