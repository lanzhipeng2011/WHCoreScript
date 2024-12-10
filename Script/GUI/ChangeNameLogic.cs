using UnityEngine;
using System.Collections;
using Games.LogicObj;
using GCGame;
using Module.Log;
using GCGame.Table;
using Games.GlobeDefine;
public class ChangeNameLogic : MonoBehaviour {

    public enum ChangeNameType
    {
        ChangeNameType_Invalid      = 0,
        ChangeNameType_PlayerName   = 1,      //
        ChangeNameType_GuildName    = 2,           //
        ChangeNameType_MasterName   = 3,
    }

    public UILabel m_ChangeNameLable;
    public UILabel m_ChangeNameTitleLable;

    private ChangeNameType m_NameType;

    public static void ShowChangeName(ChangeNameType NameType)
    {
        if (NameType < ChangeNameType.ChangeNameType_PlayerName || NameType > ChangeNameType.ChangeNameType_MasterName)
        {
            LogModule.ErrorLog("ShowChangeName:NnameType is invalid");
            return;
        }
        UIManager.ShowUI(UIInfo.ChangeName, ChangeNameLogic.OnShowChangeName, NameType);

    }
    private static void OnShowChangeName(bool bSuccess, object param)
    {
        if (!bSuccess)
        {
            LogModule.ErrorLog("load ChangeName error");
            return;
        }
        ChangeNameType NameType = (ChangeNameType)param;
        if (null != ChangeNameLogic.Instance())
            ChangeNameLogic.Instance().OnShowChangeName(NameType);
    }

    private void OnShowChangeName(ChangeNameType NameType)
    {
        if (m_ChangeNameTitleLable != null)
        {
            m_ChangeNameTitleLable.text = "";
        }
        if (NameType < ChangeNameType.ChangeNameType_PlayerName || NameType > ChangeNameType.ChangeNameType_MasterName)
        {
            LogModule.ErrorLog("OnShowChangeName::NameType is invalid");
            return;
        }
        m_NameType = NameType;
        if (ChangeNameType.ChangeNameType_PlayerName == m_NameType)
        {
            m_ChangeNameTitleLable.text = StrDictionary.GetClientDictionaryString("#{3054}");
        }
        else if (ChangeNameType.ChangeNameType_GuildName == m_NameType)
        {
            m_ChangeNameTitleLable.text = StrDictionary.GetClientDictionaryString("#{3055}");
        }
        else if (ChangeNameType.ChangeNameType_MasterName == m_NameType)
        {
            m_ChangeNameTitleLable.text = StrDictionary.GetClientDictionaryString("#{3056}");
        }
    }

    private static ChangeNameLogic m_Instance;
    public static ChangeNameLogic Instance()
    {
        return m_Instance;
    }

    void Awake()
    {
        m_Instance = this;
    }

    void OnDestroy()
    {
        m_Instance = null;
    }

    void OnChangeName()
    {
        if (m_NameType < ChangeNameType.ChangeNameType_PlayerName || m_NameType > ChangeNameType.ChangeNameType_MasterName)
        {
            return;
        }

        //判断名称控件是否存在
        if (null == m_ChangeNameLable)
        {
            LogModule.ErrorLog("OnChangeName m_ChangeNameLable is null");
            return;
        }

        Obj_MainPlayer mainPlayer = Singleton<ObjManager>.GetInstance().MainPlayer;
        if (null == mainPlayer)
        {
            return;
        }

        //判断名称是否合法
        string szCurName = m_ChangeNameLable.text;

        //过短
        if (szCurName.Length <= 0)
        {
            mainPlayer.SendNoticMsg(false, "#{6023}");     //名字过长   
            return;
        }

        //过长
        int nTextCount = 0;
        for (int i = 0; i < szCurName.Length; i++)
        {
            if (szCurName[i] >= 128 )
            {
                nTextCount += 2;
            }
            else if( szCurName[i] >= 65 && szCurName[i] <= 90)
            {
                nTextCount += 2;
            }
            else
            {
                nTextCount++;
            }
            if (char.IsWhiteSpace(szCurName[i]))
            {

                mainPlayer.SendNoticMsg(false, StrDictionary.GetClientDictionaryString("#{2797}"));
                return;
            }
        }
        
        if (nTextCount > GlobeVar.MAX_GUILD_NAME)
        {
            mainPlayer.SendNoticMsg(false, "#{2943}");     //名字过长
            return;
        }

        if (szCurName.Contains("*"))
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1278}");
            return;
        }

        if (Utils.GetStrFilter(szCurName, (int)Games.GlobeDefine.GameDefine_Globe.STRFILTER_TYPE.STRFILTER_NAME) != null)
        {
            mainPlayer.SendNoticMsg(false, "#{2932}");     //非法字符
            return;
        }

        CG_REQ_CHANGENAME packet = (CG_REQ_CHANGENAME)PacketDistributed.CreatePacket(MessageID.PACKET_CG_REQ_CHANGENAME);
        packet.Changename = szCurName;
        packet.Nametype = (int)m_NameType;
        packet.SendPacket();

        CloseWindow();   
    }
    void CloseWindow()
    {
        m_NameType = ChangeNameType.ChangeNameType_Invalid;
        UIManager.CloseUI(UIInfo.ChangeName);
    }
    void OnClose()
    {
        CloseWindow();
    }
}
