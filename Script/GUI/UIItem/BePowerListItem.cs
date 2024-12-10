using UnityEngine;
using System.Collections;
using GCGame;
using GCGame.Table;
using Games.GlobeDefine;

public class BePowerListItem : MonoBehaviour
{
    private BePowerData.BePowerListItemData m_data;

    public UILabel m_labelTitle;
    public UILabel m_labelDesc;
    public UILabel m_labelFuncBtn1;
    public GameObject m_FunctionButton1;
    public UILabel m_labelFuncBtn2;
    public GameObject m_FunctionButton2;
    
    public static BePowerListItem CreateItem(GameObject grid, GameObject resItem, string name, BePowerWindow parent, BePowerData.BePowerListItemData data)
    {
        GameObject curItem = Utils.BindObjToParent(resItem, grid, name);
        if (null != curItem)
        {
            BePowerListItem curItemComponent = curItem.GetComponent<BePowerListItem>();
            if (null != curItemComponent)
                curItemComponent.SetData(parent, data);

            return curItemComponent;
        }

        return null;
    }

    void SetFunctionBtn(int idx)
    {
        if (idx == 1)
        {
            if (m_data.btnDict > 0 && null != m_labelFuncBtn1)
            {
                m_labelFuncBtn1.text = Utils.GetDicByID(m_data.btnDict);
            }
            else if (null != m_labelFuncBtn1)
            {
                m_labelFuncBtn1.text = "";
            }


            if (null != m_FunctionButton1)
            {
                m_FunctionButton1.SetActive((bool)(m_data.btnDict > 0));
            }

        }

        if (idx == 2)
        {
            if (m_data.btnDict2 > 0 && null != m_labelFuncBtn2)
            {
                m_labelFuncBtn2.text = Utils.GetDicByID(m_data.btnDict2);
            }

            if (null != m_FunctionButton2)
            {
                m_FunctionButton2.SetActive((bool)(m_data.btnDict2 > 0));
            }

        }
        
    }

    public void SetData(BePowerWindow parent, BePowerData.BePowerListItemData data)
    {
        m_data = data;
        m_labelDesc.text = Utils.GetDicByID(m_data.descDict);
        m_labelTitle.text = Utils.GetDicByID(m_data.titleDict);

        SetFunctionBtn(1);
        SetFunctionBtn(2);
        
        if (Singleton<ObjManager>.Instance.MainPlayer != null)
        {
            if (Singleton<ObjManager>.Instance.MainPlayer.IsDie())
            {
                if (null != m_FunctionButton1)
                {
                    m_FunctionButton1.SetActive(false);
                }

                if (null != m_FunctionButton2)
                {
                    m_FunctionButton2.SetActive(false);
                }
            }
        }
    }

    void OnFunctionButton1Click()
    {
        OnFunctionButtonClick(m_data.type, m_data.function);
    }

     void OnFunctionButton2Click()
    {
        OnFunctionButtonClick(m_data.type, m_data.function2);
    }
    

    void OnFunctionButtonClick(int type, int function)
    {
        if (type > (int)BePowerData.BePowerWayDefine.EBPWD_Null &&
            type < (int)BePowerData.BePowerWayDefine.EBPWD_WAY_MAX)
        {
            BePowerData.BePowerWayDefine wayDefine = (BePowerData.BePowerWayDefine)function;
            switch (wayDefine)
            {
                case BePowerData.BePowerWayDefine.EBPWD_COPYSCENE_JUXIANZHUANG:
                    UIManager.ShowUI(UIInfo.Activity, ChangeToCopyScene, (int)GameDefine_Globe.SCENE_DEFINE.SCENE_HULAOGUAN);
                    break;
                case BePowerData.BePowerWayDefine.EBPWD_COPYSCENE_CANGJINGE:
                    UIManager.ShowUI(UIInfo.Activity, ChangeToCopyScene, (int)GameDefine_Globe.SCENE_DEFINE.SCENE_WUSHENTA);
                    break;
                case BePowerData.BePowerWayDefine.EBPWD_COPYSCENE_SHAOSHISHAN:
                    UIManager.ShowUI(UIInfo.Activity, ChangeToCopyScene, (int)GameDefine_Globe.SCENE_DEFINE.SCENE_SHAOSHISHAN);
                    break;
                case BePowerData.BePowerWayDefine.EBPWD_COPYSCENE_ZHENLONGQIJU:
                    UIManager.ShowUI(UIInfo.Activity, ChangeToCopyScene, (int)GameDefine_Globe.SCENE_DEFINE.SCENE_QIXINGXUANZHEN);
                    break;
                case BePowerData.BePowerWayDefine.EBPWD_COPYSCENE_NUHAICHUJIAN:
                    UIManager.ShowUI(UIInfo.Activity, ChangeToCopyScene, (int)GameDefine_Globe.SCENE_DEFINE.SCENE_GUOGUANZHANJIANG);
                    break;
                case BePowerData.BePowerWayDefine.EBPWD_COPYSCENE_YANZIWU:
                    UIManager.ShowUI(UIInfo.Activity, ChangeToCopyScene, (int)GameDefine_Globe.SCENE_DEFINE.SCENE_HUSONGMEIREN);
                    break;
                case BePowerData.BePowerWayDefine.EBPWD_COPYSCENE_YANWANGGUMU:
                    UIManager.ShowUI(UIInfo.Activity, ChangeToCopyScene, (int)GameDefine_Globe.SCENE_DEFINE.SCENE_YEXIDAYING);
                    break;  
                case BePowerData.BePowerWayDefine.EBPWD_EQUIP_UPGRADE:
                    UIManager.ShowUI(UIInfo.EquipStren, LoadEquipStrength, 1);
                    break;
                case BePowerData.BePowerWayDefine.EBPWD_EQUIP_STAR:
                    UIManager.ShowUI(UIInfo.EquipStren, LoadEquipStrength, 2);
                    break;
                case BePowerData.BePowerWayDefine.EBPWD_BELLE_TUJIAN:
                    UIManager.ShowUI(UIInfo.Belle, LoadBelleUI, 1);
                    break;
                case BePowerData.BePowerWayDefine.EBPWD_BELLE_ZHENFA:
                    UIManager.ShowUI(UIInfo.Belle, LoadBelleUI, 2);
                    break;
                case BePowerData.BePowerWayDefine.EBPWD_SHOP_QIANGHUA:
                    CG_ASK_YUANBAOSHOP_OPEN packet = (CG_ASK_YUANBAOSHOP_OPEN)PacketDistributed.CreatePacket(MessageID.PACKET_CG_ASK_YUANBAOSHOP_OPEN);
                    packet.NoParam = 2;
                    packet.SendPacket();
                    break;
                case BePowerData.BePowerWayDefine.EBPWD_SKILL_LEVELUP:
                    UIManager.ShowUI(UIInfo.SkillInfo);
                    break;
                case BePowerData.BePowerWayDefine.EBPWD_FELLOW_GET:
                    UIManager.ShowUI(UIInfo.PartnerAndMountRoot, LoadPartnerAndMountRoot, 1);
                    break;
                case BePowerData.BePowerWayDefine.EBPWD_FELLOW_UPGRADE:
                    UIManager.ShowUI(UIInfo.PartnerAndMountRoot, LoadPartnerAndMountRoot, 2);
                    break;
                case BePowerData.BePowerWayDefine.EBPWD_FELLOW_STAR:
                    UIManager.ShowUI(UIInfo.PartnerAndMountRoot, LoadPartnerAndMountRoot, 3);
                    break;
                case BePowerData.BePowerWayDefine.EBPWD_FELLOW_SKILL:
                    UIManager.ShowUI(UIInfo.PartnerAndMountRoot, LoadPartnerAndMountRoot, 4);
                    break;
                case BePowerData.BePowerWayDefine.EBPWD_XIAKE:
                    UIManager.ShowUI(UIInfo.SwordsManRoot);
                    break;
            }
        }
    }

    void ChangeToCopyScene(bool bSuccess, object param)
    {
        if (ActivityController.Instance())
        {
            int sceneID = (int)param;
            switch (sceneID)
            {
                case (int)GameDefine_Globe.SCENE_DEFINE.SCENE_HULAOGUAN:
                    ActivityController.Instance().m_TabDungeon.ChangeTab("Tab3");
                    break;
                case (int)GameDefine_Globe.SCENE_DEFINE.SCENE_WUSHENTA:
                    ActivityController.Instance().m_TabDungeon.ChangeTab("Tab5");
                    break;
                case (int)GameDefine_Globe.SCENE_DEFINE.SCENE_SHAOSHISHAN:
                    ActivityController.Instance().m_TabDungeon.ChangeTab("Tab10");
                    break;
                case (int)GameDefine_Globe.SCENE_DEFINE.SCENE_QIXINGXUANZHEN:
                    ActivityController.Instance().m_TabDungeon.ChangeTab("Tab7");
                    break;
                case (int)GameDefine_Globe.SCENE_DEFINE.SCENE_GUOGUANZHANJIANG:
                    ActivityController.Instance().m_TabDungeon.ChangeTab("Tab13");
                    break;
                case (int)GameDefine_Globe.SCENE_DEFINE.SCENE_HUSONGMEIREN:
                    ActivityController.Instance().m_TabDungeon.ChangeTab("Tab6");
                    break;
                case (int)GameDefine_Globe.SCENE_DEFINE.SCENE_YEXIDAYING:
                    ActivityController.Instance().m_TabDungeon.ChangeTab("Tab8");
                    break;
            }
        }
    }

    void LoadEquipStrength(bool bSuccess, object param)
    {
        if (EquipStrengthenLogic.Instance() != null)
        {
            int type = (int)param;
            //EquipStrengthenLogic.Instance().SetAfterDeldegate(type);
            switch (type)
            {
                case 1:
                    EquipStrengthenLogic.Instance().m_TabController.ChangeTab("Button1-Strengthen");
                    break;
                case 2:
                    EquipStrengthenLogic.Instance().m_TabController.ChangeTab("Button2-Star");
                    break;
            }
        }
    }

    void LoadPartnerAndMountRoot(bool bSuccess, object param)
    {
        if( PartnerAndMountLogic.Instance() == null)
            return;

        PartnerAndMountLogic.Instance().ShowPartnerRoot();

        if (PartnerFrameLogic.Instance() == null)
            return;

        int type = (int)param;
        //PartnerFrameLogic.Instance().SetStartDelegate(type);
        switch (type)
        {
            case 1:

                PartnerFrameLogic.Instance().ShowPartnerGamble();
                break;
            case 2:
                PartnerFrameLogic.Instance().ShowZiZhiStren();
                break;
            case 3:
                PartnerFrameLogic.Instance().ShowRaiseStar();
                break;
            case 4:
                PartnerFrameLogic.Instance().ShowPartnerSkill();
                break;
        }
    }

    void LoadBelleUI(bool bSuccess, object param)
    {
        //BelleController
        if (BelleController.Instance() == null)
        {
            return;
        }

        int type = (int)param;
        switch (type)
        {
            case 1:
                BelleController.Instance().tabController.ChangeTab("Tab1");
                break;
            case 2:
                BelleController.Instance().tabController.ChangeTab("Tab2");
                break;
            case 3:
                break;
        }
    }

    public BePowerData.BePowerListItemData GetData()
    {
        return m_data;
    }

}
