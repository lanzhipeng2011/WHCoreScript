using UnityEngine;
using System.Collections;

public class SGDyingBlood : MonoBehaviour 
{

    UITexture bloodTexture;

    public float MinAlpha = 100.0f / 255f;
    public float MaxAlpha = 150f / 255f;
    public float step = 0.002f;

    float addstep;

    public static SGDyingBlood Instance;
    public static bool IsShow =  false;
    void Awake() 
    {
        SGDyingBlood.Instance = this;
    }
    void Start ()
    {
        bloodTexture = gameObject.GetComponent<UITexture>();
        Hide();
	}

    public void Show() 
    {
        this.enabled = true;
        this.bloodTexture.alpha = MinAlpha;
        addstep = step;
        IsShow = true;
    }

    public void Hide() 
    {
        this.enabled = false;
        this.bloodTexture.alpha = 0;
        IsShow = false;
    }


  
	void Update () 
    {
        bloodTexture.width =(int)(1080f/Screen.height * Screen.width);
        if (this.bloodTexture.color.a < MinAlpha)
        {
            addstep = step;
        }
        if (this.bloodTexture.alpha>MaxAlpha)
        {
            addstep = -step;
        }
        this.bloodTexture.alpha += addstep;
	}
}
