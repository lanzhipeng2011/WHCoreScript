using UnityEngine;
using System.Collections;
using GCGame;
using GCGame.Table;
using Games.GlobeDefine;

public class HuaShanPvPOpponentViewWindow : MonoBehaviour
{
    public UILabel Countdown;
    public UISprite HeadIcon;
    public UILabel Name;
    public UILabel CombatNum;
    public UILabel HP;
    public UILabel MP;
    public UILabel Attack;
    public UILabel Defense;
    public UILabel Critical;
    public UILabel Speed;
    public UILabel dodge;
    public UILabel Profession;
    public UILabel progress;
    public UILabel searchTip;
    
    private int UIStage { set; get; }
    private int SearchSecond { set; get; }

	// Use this for initialization
	void Start () 
	{
		InvokeRepeating ("CountdownSrearch", 1.0f, 1.0f);
	}

    void OnEnable()
    {
        SearchSecond = 0;
        UIStage = 0;
        HuaShanPVPData.delegateShowOpponentViewInfo += ShowOpponentView;
        HuaShanPVPData.delegateShowSearchOpponent += ShowSearchOppoent;
        HuaShanPVPData.delegateShowWaitForOpponet += ShowWaitForOpponent;
    }

    void OnDisable()
    {
        HuaShanPVPData.delegateShowOpponentViewInfo -= ShowOpponentView;
        HuaShanPVPData.delegateShowSearchOpponent -= ShowSearchOppoent;
        HuaShanPVPData.delegateShowWaitForOpponet -= ShowWaitForOpponent;
    }


	void CountdownSrearch () {
	    if( SearchSecond > 0 )
        {
       		Countdown.text = StrDictionary.GetClientDictionaryString("#{1824}", SearchSecond);
        	SearchSecond--;
        }
	}

    void ShowWaitForOpponent( )
    {
        string text = StrDictionary.GetClientDictionaryString("#{2344}");
        //searchTip.text = text;
        Countdown.text = text;
    }

	void ShowOpponentView()
	{
        UIStage = 1;
        SearchSecond = 0;
        progress.text = HuaShanPVPData.HSRounderTip();
        if (HuaShanPVPData.OppoViewInfo.guid == Games.GlobeDefine.GlobeVar.INVALID_GUID)
        {
            string text = "";
            if (HuaShanPVPData.Rounder == 1)
            {
                text = StrDictionary.GetClientDictionaryString("#{2346}");
            }
            else
            {
                string roundTips = HuaShanPVPData.HSRoundTipPrefix();
                text = StrDictionary.GetClientDictionaryString("#{1663}", roundTips);
            }

            searchTip.text = text;
            Countdown.text = text;

        }
        else
        {
            searchTip.text = Utils.GetDicByID(1845);
            Countdown.text = Utils.GetDicByID(1845);
            int id = (int)HuaShanPVPData.OppoViewInfo.profession;
            Tab_RoleBaseAttr roleBaseAttr = TableManager.GetRoleBaseAttrByID(id, 0);
            if (roleBaseAttr != null)
            {
                Tab_CharModel charModel = TableManager.GetCharModelByID(roleBaseAttr.CharModelID, 0);
                if (charModel != null)
                {
                    HeadIcon.spriteName = charModel.HeadPic;
                }
            }
            if( id >= 0 && id <  CharacterDefine.PROFESSION_DICNUM.Length)
            {
                Profession.text = Utils.GetDicByID(CharacterDefine.PROFESSION_DICNUM[id]);
            }
        }

        Name.text = HuaShanPVPData.OppoViewInfo.name;
        CombatNum.text = HuaShanPVPData.OppoViewInfo.combat.ToString();
        HP.text = HuaShanPVPData.OppoViewInfo.hp.ToString();
        MP.text = HuaShanPVPData.OppoViewInfo.mp.ToString();
        Attack.text = HuaShanPVPData.OppoViewInfo.atk.ToString();
        Defense.text = HuaShanPVPData.OppoViewInfo.def.ToString();
        Critical.text = HuaShanPVPData.OppoViewInfo.cri.ToString();
        Speed.text = HuaShanPVPData.OppoViewInfo.spd.ToString();
        dodge.text = HuaShanPVPData.OppoViewInfo.dge.ToString();
	}

    void ShowSearchOppoent(int continueSecond, int Progress)
    {
        UIStage = 0;
        SearchSecond = continueSecond;
        Countdown.text = StrDictionary.GetClientDictionaryString("#{1824}", SearchSecond);
        searchTip.text = Utils.GetDicByID(1846);
        progress.text = StrDictionary.GetClientDictionaryString("#{1844}", Progress);
    }

    void OnClickPkInfo()
    {
        CG_REQ_HUASHAN_PKINFO packet = (CG_REQ_HUASHAN_PKINFO)PacketDistributed.CreatePacket(MessageID.PACKET_CG_REQ_HUASHAN_PKINFO);
        packet.None = 0;
        packet.SendPacket();
    }
}
