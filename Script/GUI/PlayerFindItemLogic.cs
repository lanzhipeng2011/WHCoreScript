using UnityEngine;
using System.Collections;
using System;
using Games.GlobeDefine;
using GCGame.Table;
using GCGame;

public class PlayerFindItemLogic : MonoBehaviour 
{
    public UILabel m_PlayerProLabel;                // 职业
    public UILabel m_PlayerNameLabel;               // 名字
    public UILabel m_PlayerLevelLabel;              // 等级
    public UILabel m_PlayerCombatNumLabel;          // 战斗力
    public GameObject m_SelectSprite;
    public GameObject m_BkSprite;
    //按钮
    public UIImageButton m_AddFriendImgBtn;         // 添加好友按钮

    private UInt64 m_Guid = GlobeVar.INVALID_GUID;  //玩家GUID
    public UInt64 Guid
    {
        get { return m_Guid; }
    }

    //选中状态
    //private bool m_bSelected = false;               //选中状态

    private PlayerFindWindow m_Parent = null;

    public static PlayerFindItemLogic CreateItem(GameObject grid, GameObject resItem, string name, PlayerFindWindow parent)
    {
        if (null == resItem || null == grid || null == parent)
        {
            return null;
        }

        GameObject curItem = Utils.BindObjToParent(resItem, grid, name);
        if (null != curItem)
        {
            PlayerFindItemLogic curItemComponent = curItem.GetComponent<PlayerFindItemLogic>();
            if (null != curItemComponent)
                curItemComponent.SetParent(parent);

            return curItemComponent;
        }

        return null;
    }

    void SetParent(PlayerFindWindow parent)
    {
        m_Parent = parent;
    }

    //显示玩家查找结果
    public void InitPlayerFindItem(Relation player)
    {
        if (null != player)
        {
            m_Guid = player.Guid;
            SetName(player.Name);
            SetLevel(player.Level);
            SetPro(player.Profession);
            SetCombatNum(player.CombatNum);
        }
    }

    public void OnClickAddFriend()
    {
        //检查GUID合法性
        if (m_Guid == GlobeVar.INVALID_GUID)
        {
            return;
        }

        //是否已经是好友
        if (true == GameManager.gameManager.PlayerDataPool.FriendList.IsExist(m_Guid))
        {
            return;
        }
        
        //添加好友
        Singleton<ObjManager>.GetInstance().MainPlayer.ReqAddFriend(m_Guid);
    }

    void SetPro(int nProfession)
    {
        //判断职业是否合法
        if (nProfession < 0 || nProfession >= (int)CharacterDefine.PROFESSION.MAX)
        {
            //非法职业ID，则强制为0
            nProfession = 0;
        }

        // 玩家职业
        string ProName = StrDictionary.GetClientDictionaryString("#{" + CharacterDefine.PROFESSION_DICNUM[nProfession].ToString() + "}");
        m_PlayerProLabel.text = StrDictionary.GetClientDictionaryString("#{1062}", ProName);
    }

    void SetName(string szName)
    {
        m_PlayerNameLabel.text = szName;
    }

    void SetLevel(int nLevel)
    {
        m_PlayerLevelLabel.text = StrDictionary.GetClientDictionaryString("#{1063}", nLevel);
    }

    void SetCombatNum(int nCombatNum)
    {
        m_PlayerCombatNumLabel.text = StrDictionary.GetClientDictionaryString("#{1064}", nCombatNum);
    }

    public void OnSelectItem()
    {
        if (m_SelectSprite != null)
        {
            m_SelectSprite.SetActive(true);
        }
        if (m_BkSprite != null)
        {
            m_BkSprite.SetActive(false);
        }
    }

    public void OnCancelSelectItem()
    {
        if (m_SelectSprite != null)
        {
            m_SelectSprite.SetActive(false);
        }
        if (m_BkSprite != null)
        {
            m_BkSprite.SetActive(true);
        }
    }

    void OnClickItem()
    {
        if (m_Parent != null)
        {
            m_Parent.OnClickPlayerItem(this);
        }
    
    }
}
