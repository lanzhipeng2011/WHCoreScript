using UnityEngine;
using System.Collections;
using GCGame.Table;
public class AutoDrugLogic : UIControllerBase<AutoDrugLogic>
{
    //private AutoFightLogic m_FightLogic;
    public AutoDrugWindow m_AutoWindow;
    public UILabel m_ItemName;
    void Awake()
    {
        UIControllerBase<AutoDrugLogic>.SetInstance(this);
    }
	// Use this for initialization
	void Start () {
	
	}
	
    public void setData(int nType, AutoFightLogic parent)
    {
        m_AutoWindow.m_nType = nType;
        //m_FightLogic = parent;
        if (nType == 3)
        {
            m_ItemName.text = StrDictionary.GetClientDictionaryString("#{3147}");
        }
        else
        {
            m_ItemName.text = StrDictionary.GetClientDictionaryString("#{1202}");
        }
        m_AutoWindow.UpdateData(nType, parent);
    }
   
    public void CloseWindow()
    {
        UIManager.CloseUI(UIInfo.AutoDrug);
    }
}
