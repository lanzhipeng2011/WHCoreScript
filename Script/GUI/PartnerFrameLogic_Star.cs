using UnityEngine;
using System.Collections;
using Games.Fellow;
using System.Collections.Generic;
using GCGame.Table;
using Games.GlobeDefine;
using Games.Item;

public class PartnerFrameLogic_Star : MonoBehaviour {

    public UISprite m_PartnerHeadIcon;
    public UISprite m_PartnerQualityFrame;
    public ItemSlotLogic m_MaterialItem;
    public UISprite[] m_PartnerStarLevelSprite;
    public UILabel m_NeedItemNumLabel;
    public UILabel m_HaveItemNumLabel;

	public UILabel m_MaxStarLv;

    private Fellow m_Fellow = null;

    private float m_Delay_Time = 0f;
    private ulong m_Delay_FellowGuid = 0;

    private static PartnerFrameLogic_Star m_Instance = null;
    public static PartnerFrameLogic_Star Instance()
    {
        return m_Instance;
    }
 
    void OnEnable()
    {
        m_Instance = this;

        InvokeRepeating("DelaySendPacket", 0f, 0.3f);

        if (m_Fellow == null)
        {
            UpdateEmpty_Star();
        }
    }

    void OnDisable()
    {
        //关闭界面时 如果有延迟发包 则直接发包
        if (m_Delay_Time > 0)
        {
            CG_FELLOW_STAR fellowPacket = (CG_FELLOW_STAR)PacketDistributed.CreatePacket(MessageID.PACKET_CG_FELLOW_STAR);
            fellowPacket.SetFellowguid(m_Delay_FellowGuid);
            fellowPacket.SendPacket();

            m_Delay_FellowGuid = 0;
        }

        //关闭界面时 如果有UI特效播放 则关闭特效
        if (BackCamerControll.Instance())
        {
            BackCamerControll.Instance().StopSceneEffect(132, true);
        }

        CancelInvoke("DelaySendPacket");
        m_Instance = null;
    }

    void DelaySendPacket()
    {
        if (m_Delay_Time > 0)
        {
            m_Delay_Time -= 0.3f;
            if (m_Delay_Time <= 0)
            {
                CG_FELLOW_STAR fellowPacket = (CG_FELLOW_STAR)PacketDistributed.CreatePacket(MessageID.PACKET_CG_FELLOW_STAR);
                fellowPacket.SetFellowguid(m_Delay_FellowGuid);
                fellowPacket.SendPacket();

                m_Delay_FellowGuid = 0;
            }
        }
    }

    void UpdateEmpty_Star()
    {
        m_PartnerHeadIcon.gameObject.SetActive(false);
        m_PartnerQualityFrame.gameObject.SetActive(false);
        m_MaterialItem.ClearInfo();
        for (int i = 0; i < 12; i++)
        {
            m_PartnerStarLevelSprite[i].gameObject.SetActive(true);
            m_PartnerStarLevelSprite[i].spriteName = "ui_pub_053";
        }
    }

    public void UpdateFellow_Star(Fellow fellow)
    {
        UpdateCurPartner(fellow);
        m_Fellow = fellow;
		m_MaxStarLv.text =m_Fellow.GetFellowQualityColorName(m_Fellow.Quality) +" "+ m_Fellow.GetMaxStarLevel ().ToString () + " 星";
    }

    void UpdateCurPartner(Fellow fellow)
    {
        //头像
        m_PartnerHeadIcon.spriteName = fellow.GetIcon();
        m_PartnerHeadIcon.gameObject.SetActive(true);
        m_PartnerQualityFrame.spriteName = FellowTool.GetFellowSkillQualityFrame(fellow.Quality);
        m_PartnerQualityFrame.gameObject.SetActive(true);

        int starLevel = fellow.StarLevel;
        string nowStarSpriteName = NowStarSpriteName(starLevel);
        string perStarSpriteName = PerStarSpriteName(starLevel);
        int nowStarNum = NowStarNum(starLevel);

        //当前星级
        for (int i = 0; i < 12; i++)
        {
            if (i < nowStarNum)
            {
                m_PartnerStarLevelSprite[i].spriteName = nowStarSpriteName;
            }
            else
            {
                m_PartnerStarLevelSprite[i].spriteName = perStarSpriteName;
            }
        }
        //材料
        Tab_FellowStar line = TableManager.GetFellowStarByID(fellow.StarLevel, 0);

        if (line != null)
        {
            int itemId = line.ConsumeSubType;
            int itemNum = line.ConsumeNum;
            int curitemNum = GameManager.gameManager.PlayerDataPool.BackPack.GetItemCountByDataId(itemId);

            string curNumColor = "[D60031]"; //红色
            if (curitemNum >= itemNum)
            {
                curNumColor = "[33CC66]"; //绿色
            }
            m_HaveItemNumLabel.text = string.Format("{0}{1}", curNumColor, curitemNum);
            m_NeedItemNumLabel.text = string.Format("[FFFF69]{0}", itemNum);

            m_MaterialItem.InitInfo(ItemSlotLogic.SLOT_TYPE.TYPE_ITEM, itemId, OnClickStarItem, "", false);
        }

		if (fellow.StarLevel >= fellow.GetMaxStarLevel() + ((int) fellow.Quality * 12))
		{
			m_NeedItemNumLabel.text = string.Format("[FFFF69]{0}", StrDictionary.GetClientDictionaryString("#{3992}", ""));
		}

    }

    public void OnClickStar()
    {
        if (m_Delay_Time > 0)
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{2215}");
            return;
        }

        if (m_Fellow == null || m_Fellow.IsValid() == false)
        {
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1836}");
            return;
        }

        if (m_Fellow.StarLevel >= m_Fellow.GetMaxStarLevel() + ((int) m_Fellow.Quality * 12))
        {
            //已经达到星级上限
            Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{1828}");
            return;
        }

        Tab_FellowStar line = TableManager.GetFellowStarByID(m_Fellow.StarLevel, 0);
        if (line != null)
        {
            int itemNum = GameManager.gameManager.PlayerDataPool.BackPack.GetItemCountByDataId(line.ConsumeSubType);
            if (itemNum < line.ConsumeNum)
            {
                //道具不足
                Singleton<ObjManager>.GetInstance().MainPlayer.SendNoticMsg(false, "#{2368}");
                return;
            }
        }

        if (null != GameManager.gameManager)
        {
            GameManager.gameManager.SoundManager.PlaySoundEffect(28);   //intensify
        }
        //播放特效 延迟发包
        if (BackCamerControll.Instance() != null)
        {
            BackCamerControll.Instance().PlaySceneEffect(132);
        }
        m_Delay_Time = 2.0f;
        m_Delay_FellowGuid = m_Fellow.Guid;
    }

    public void OnClickStarItem(int nItemID, ItemSlotLogic.SLOT_TYPE eItemType, string strSlotName)
    {
        if (nItemID >= 0)
        {
            GameItem item = new GameItem();
            item.DataID = nItemID;
            ItemTooltipsLogic.ShowItemTooltip(item, ItemTooltipsLogic.ShowType.Info);
        }
    }

    string NowStarSpriteName(int starLevel)
    {
        int starQuality = starLevel / 12;
        switch (starQuality)
        {
            case 0:
			return "ui_pub_048";
            case 1:
			return "ui_pub_049";
            case 2:
			return "ui_pub_050";
            case 3:
			return "ui_pub_051";
            case 4:
			return "ui_pub_052";
            default:
            return "ui_pub_053";
        };
    }

    string PerStarSpriteName(int starLevel)
    {
        int starQuality = starLevel / 12;
        switch (starQuality - 1)
        {
            case -1:
			return "ui_pub_053";
            case 0:
			return "ui_pub_048";
            case 1:
			return "ui_pub_049";
            case 2:
			return "ui_pub_050";
            case 3:
			return "ui_pub_051";
            case 4:
			return "ui_pub_052";
            default:
			return "ui_pub_053";
        };
    }

    int NowStarNum(int starLevel)
    {
        return starLevel % 12;
    }
}

