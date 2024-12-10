//********************************************************************
// 文件名: CollectItemSliderLogic.cs
// 描述: 采集滚动条
// 作者: HeWenpeng
// 创建时间: 2014-1-13
//
// 修改历史:
//********************************************************************
using UnityEngine;
using System.Collections;

public class CollectItemSliderLogic : MonoBehaviour
{
    public UISlider loadingSlider;

    private static CollectItemSliderLogic m_Instance = null;
    public static CollectItemSliderLogic Instance()
    {
        return m_Instance;
    }

    void Awake()
    {
        m_Instance = this;
    }

	// Use this for initialization
	void Start () {
        loadingSlider.value = 0;
	}
	
	// Update is called once per frame
	void Update () {
        loadingSlider.value += Time.deltaTime;
        if (loadingSlider.value >= 1)
        {
            Singleton<CollectItem>.GetInstance().SafeDeleteItem();
            loadingSlider.value = 0;
            UIManager.CloseUI(UIInfo.CollectItemSlider);
			//恢复武器显示
			if(Singleton<ObjManager>.GetInstance().MainPlayer.isHideWeapon==true)
			Singleton<ObjManager>.GetInstance().MainPlayer.HideOrShowWeanpon();

			//====In MissionCollect
			//Singleton<ObjManager>.GetInstance().MainPlayer.isMissionCollect = false;
			//Singleton<ObjManager>.GetInstance().MainPlayer.RideOrUnMount();
            return;
        }
	}

    public void CloseCollect()
    {
        if (loadingSlider.value > 0)
        {
            Singleton<CollectItem>.GetInstance().SafeDeleteItem();
            loadingSlider.value = 0;
            UIManager.CloseUI(UIInfo.CollectItemSlider);
		
        }
    }
    
    void OnDestroy()
    {
        m_Instance = null;
    }

}
