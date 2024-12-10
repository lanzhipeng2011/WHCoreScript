using UnityEngine;
using System.Collections;

public class SGUIEventMask : MonoBehaviour 
{
    public static SGUIEventMask Instance;

    public BoxCollider colloderTop;
    public BoxCollider colloderBottom;
    public BoxCollider colloderLeft;
    public BoxCollider colloderRight; 
    void Awake()
    {
        
        gameObject.layer = 8;
      
        Instance = this;
    }

    public void Create(Vector3 center,Vector3 size)
    {
        float w = ((float)UIRoot.list[0].manualHeight / Screen.height) * Screen.width;
        float h = UIRoot.list[0].manualHeight;
        float aspt = w/h;
        float asptNormal = 16.0f / 9.0f;
        if (System.Math.Abs(aspt - asptNormal) > 0.01f) 
        {
            float scale  =w /  1920.0f;
            center.x *= scale;
        }

        center.x *= -1;
        colloderTop.size = new Vector3(w, h - (center.y + h / 2) - (size.y /2), 1);
        colloderTop.transform.localPosition = new Vector3(0,(h / 2)-(colloderTop.size.y / 2),0);

        this.colloderBottom.size = new Vector3(w,h - colloderTop.size.y - size.y);
        this.colloderBottom.transform.localPosition = new Vector3(0,this.colloderBottom.size.y/2 - h/2,0);

        float subY = h - colloderTop.size.y - colloderBottom.size.y;
        float posY = this.colloderBottom.size.y + size.y/2-(h/2);
        this.colloderLeft.size = new Vector3(w - center.x - (size.x/2) - w / 2,subY,0);
        this.colloderLeft.transform.localPosition = new Vector3(this.colloderLeft.size.x / 2 - w / 2, posY, 0);

        this.colloderRight.size = new Vector3(w - size.x-this.colloderLeft.size.x   , subY, 0);
        this.colloderRight.transform.localPosition = new Vector3(w - colloderRight.size.x/2 - w/2, posY, 0);

    }
   
}
 