/********************************************************************
	日期:	2014/02/25
	文件: 	Obj_DropItem.cs
	路径:	D:\work\code\mldj\Version\Main\Project\Client\Assets\MLDJ\Script\Obj\Obj_DropItem.cs
	作者:	YangXin
	描述:	掉落物品
	修改:	
*********************************************************************/
using UnityEngine;
using Games.GlobeDefine;
using System;
using GCGame.Table;
using Games.Item;
using Games.Scene;
using GCGame;
using Module.Log;
namespace Games.LogicObj
{
    public class Obj_DropItem : Obj
    {
        public float fActiveRadius = 5; //拾取范围
        private bool bIsDrop = true; //是否可以捡取
        private float m_fDropTimeSecond = Time.realtimeSinceStartup;

        private bool bIsSendDrop = true; //延迟发送
        private float m_fSendDropTimeSecond = Time.realtimeSinceStartup;

        private float m_fUpdateTimeSecond = Time.realtimeSinceStartup + 0.3f;    //1秒钟执行一次掉落捡取,以防止不停刷背包已满.第一次没有限制

        public float m_fMoveSecond = 0.2f; //移动速度

        public float m_fSpeed = 4; //移动时长

        public float m_fStop = 0.2f; //停顿时间

        public float m_fScaling = 2; //放大陪数
        private Vector3 m_localScale;
		private float m_lifetime=30.0f;//如果30s内不捡，删除
		private bool  m_isbackfull=false;//背包是否已满
		private float m_firstdroptime=0.0f;//包掉落时间
        // private float m_fSpeed = 0.0f;
        public Obj_DropItem()
        {
            m_ObjType = GameDefine_Globe.OBJ_TYPE.OBJ_DROP_ITEM;
        }

        public virtual bool Init(Obj_DroopItemData initData)
        {
            if (null == m_ObjTransform)
            {
                m_ObjTransform = transform;
            }
            if (null != GameManager.gameManager.ActiveScene &&
                null != GameManager.gameManager.ActiveScene.DropItemBoardRoot)
            {
                m_ObjTransform.parent = GameManager.gameManager.ActiveScene.DropItemBoardRoot.transform;
            }
            else
            {
                LogModule.ErrorLog("GameManager.gameManager.ActiveScene.DropItemBoardRoot is null");
            }

            DropType = initData.m_nType;
            ItemId = initData.m_nItemId;
            ItemCount = initData.m_nItemCount;
            ServerID = initData.m_nServerID;
            OwnerGuid = initData.m_OwnerGuid;

            float fScaling = 0.005f;

            Vector3 pos = new Vector3(initData.m_fX, 0, initData.m_fZ);
            if (null != Singleton<ObjManager>.Instance.MainPlayer)
            {
                pos.y = Singleton<ObjManager>.Instance.MainPlayer.Position.y;
            }
            else
            {
                pos = ActiveScene.GetTerrainPosition(pos);
            }
            pos.y += 0.2f;

            if (GameManager.gameManager.RunningScene == (int)GameDefine_Globe.SCENE_DEFINE.SCENE_WUSHENTA)
            {
                pos.y = 15.1f;
            }

            m_ObjTransform.position = pos;
            //暂时 写死了 后面再用服务器
            //m_ObjTransform.Rotate(Vector3.up * 135);
			m_ObjTransform.LookAt (Camera.main.transform.position);

            //初始化特效
            if (ObjEffectLogic == null)
            {
                ObjEffectLogic = gameObject.AddComponent<EffectLogic>();
            }

            InitEffect();

            if (DropType == (int)GameDefine_Globe.DROP_TYPE.DROP_ITEM)
            {
                Tab_CommonItem line = TableManager.GetCommonItemByID(m_nItemId, 0);
                if (line != null)
                {
                    fScaling = line.DropScaling;

                    int nEffectId = line.DropEffectId;
                    if (nEffectId != -1)
                    {
                        PlayEffect(nEffectId);
                    }
                }
            }
            Vector3 posScale = m_ObjTransform.localScale;
            m_ObjTransform.localScale = posScale * fScaling;
            m_localScale = m_ObjTransform.localScale;
            return true;
        }

        void OnTriggerEnter(Collider other)
        {
            Obj_MainPlayer mainPlayer = other.gameObject.GetComponent<Obj_MainPlayer>();
            if (null != mainPlayer)
            {
                if (mainPlayer.GUID == OwnerGuid)
                {
                    CG_ASK_PICKUP_ITEM packet = (CG_ASK_PICKUP_ITEM)PacketDistributed.CreatePacket(MessageID.PACKET_CG_ASK_PICKUP_ITEM);
                    packet.SetObjId(1);
                    packet.SendPacket();
                }
            }
        }
        //更新Obj_DropItem逻辑数据
        void FixedUpdate()
        {
			if (m_isbackfull == true) 
			{
				if (Time.realtimeSinceStartup - m_firstdroptime > m_lifetime)
				{
					gameObject.SetActive(false);
					DestroyImmediate(gameObject);
					//ObjManager.GetInstance().ReomoveObjInScene(gameObject,true);
					//UIManager.CloseUI(UIInfo.DropItemHeadInfo);
					return;
				}
			}
            //不要频繁发包,5秒发一次
            if (bIsDrop == false)
            {
                if (Time.realtimeSinceStartup - m_fDropTimeSecond > 5)
                {
                    m_fDropTimeSecond = Time.realtimeSinceStartup;
                    bIsDrop = true;
                }
                else
                {
                    return;
                }
            }

            //延迟发送
            if (bIsSendDrop == false)
            {
                if (Time.realtimeSinceStartup - m_fSendDropTimeSecond > m_fMoveSecond)
                {
                    SendDropItem();
                    m_fSendDropTimeSecond = Time.realtimeSinceStartup;
                    bIsSendDrop = true;
                    gameObject.SetActive(false);
                    // UpdateMove(100);
                }
                else
                {
                    UpdateMove(m_fSpeed);
                }

                return;
            }

            if (Time.realtimeSinceStartup - m_fUpdateTimeSecond > m_fStop)
            {
                m_fUpdateTimeSecond = Time.realtimeSinceStartup;
                //更新拾取
                Obj_MainPlayer OwnerUser = Singleton<ObjManager>.Instance.MainPlayer;
                if (null != OwnerUser && OwnerUser.GUID == OwnerGuid && OwnerUser.IsDie() == false)
                {
                    if (OwnerUser.GetAutoCombatState() == true)// 挂机中自动拾取
                    {
                        if (DropType == (int)GameDefine_Globe.DROP_TYPE.DROP_ITEM)
                        {
                            Tab_CommonItem line = TableManager.GetCommonItemByID(m_nItemId, 0);
                            if (line != null)
                            {
                                //计算品级及拾取规则
//                                 if ((line.ClassID == (int)ItemClass.EQUIP && OwnerUser.GetAutoPickUpFlag(line.Quality))
//                                    || (line.ClassID == (int)ItemClass.STRENGTHEN && OwnerUser.GetAutoPickUpFlag((int)GameDefine_Globe.AUTOCOMBAT_PICKUP_TYPE.PICKUP_STUFF))
//                                    || (line.ClassID != (int)ItemClass.EQUIP && line.ClassID != (int)ItemClass.STRENGTHEN && OwnerUser.GetAutoPickUpFlag((int)GameDefine_Globe.AUTOCOMBAT_PICKUP_TYPE.PICKUP_OTHER)))
                                {
                                    GameItemContainer BackPack = GameManager.gameManager.PlayerDataPool.BackPack;
                                    if (BackPack.GetCanContainerSize() <= 0)
                                    {
                                        OwnerUser.SendNoticMsg(true, "#{1039}");
                                        bIsDrop = false;
										m_isbackfull=true;
                                        return;
                                    }
                                    //SendDropItem();
                                    bIsSendDrop = false;
                                    m_fSendDropTimeSecond = Time.realtimeSinceStartup;
                                }
                            }
                        }
                        else if (DropType == (int)GameDefine_Globe.DROP_TYPE.DROP_COIN)
                        {
                            //SendDropItem();
                            bIsSendDrop = false;
                            m_fSendDropTimeSecond = Time.realtimeSinceStartup;
                            StopEffect(175, true);
                            PlayEffect(176);
                        }

                    }
                    else if (Vector3.Distance(OwnerUser.transform.position, this.gameObject.transform.position) <= fActiveRadius)
                    {
                        GameItemContainer BackPack = GameManager.gameManager.PlayerDataPool.BackPack;
                        if (BackPack.GetCanContainerSize() <= 0 && DropType == (int)GameDefine_Globe.DROP_TYPE.DROP_ITEM)
                        {
                            OwnerUser.SendNoticMsg(true, "#{1039}");
							m_isbackfull=true;
                            bIsDrop = false;
                            return;
                        }
                        //SendDropItem();                   
                        bIsSendDrop = false;
                        m_fSendDropTimeSecond = Time.realtimeSinceStartup;
                        if (DropType == (int)GameDefine_Globe.DROP_TYPE.DROP_COIN)
                        {
                            StopEffect(175, true);
                            PlayEffect(176);
                        }
                    }
                }
            }
        }

        void Start()
        {
            m_fDropTimeSecond = Time.realtimeSinceStartup;
            m_fSendDropTimeSecond = Time.realtimeSinceStartup;
			m_firstdroptime = Time.realtimeSinceStartup;
            ShowNameBoard();
            ShowItemSprite();
        }
        public void SendDropItem()
        {
            CG_ASK_PICKUP_ITEM packet = (CG_ASK_PICKUP_ITEM)PacketDistributed.CreatePacket(MessageID.PACKET_CG_ASK_PICKUP_ITEM);
			packet.SetObjId((uint)ServerID);
            packet.SendPacket();
            //播放拾取声音
            if (null != GameManager.gameManager.SoundManager)
            {
                if (DropType == (int)GameDefine_Globe.DROP_TYPE.DROP_COIN)
                {
                    GameManager.gameManager.SoundManager.PlaySoundEffect(31);   //pickup_coin
                }
                else if (DropType == (int)GameDefine_Globe.DROP_TYPE.DROP_ITEM)
                {
                    GameManager.gameManager.SoundManager.PlaySoundEffect(32);   //pickup_goods
                }
            }

			//=========通知人物头顶Pop拾取文字
			Obj_MainPlayer OwnerUser = Singleton<ObjManager>.Instance.MainPlayer;
			if (null != OwnerUser && OwnerUser.GUID == OwnerGuid && OwnerUser.IsDie() == false)
			{
				Tab_CommonItem line = TableManager.GetCommonItemByID(m_nItemId, 0);
				if (line != null)
				{
					OwnerUser.m_playerHeadInfo.ShowDropPop( m_nItemId);
				}

			}

            bIsDrop = false;
        }

        protected int m_nDropType;        //掉落类型
        public int DropType
        {
            get { return m_nDropType; }
            set { m_nDropType = value; }
        }

        protected int m_nItemId;        //物品Id
        public int ItemId
        {
            get { return m_nItemId; }
            set { m_nItemId = value; }
        }
        protected int m_nItemCount;     //物品数量
        public int ItemCount
        {
            get { return m_nItemCount; }
            set { m_nItemCount = value; }
        }
        protected UInt64 m_OwnerGuid;   //归属者Guild
        public UInt64 OwnerGuid
        {
            get { return m_OwnerGuid; }
            set { m_OwnerGuid = value; }
        }

        public UILabel m_NameBoard;            // 物品名字
        public UISprite m_ItemSprite;           // 物品图片

        public void ShowNameBoard()
        {
            Tab_CommonItem line = TableManager.GetCommonItemByID(m_nItemId, 0);
            if (line != null)
            {
                SetNameBoardColor();

                if (null != m_NameBoard)
                {
                    m_NameBoard.text = line.Name;
                }
            }
        }
        public void SetNameBoardColor()
        {
            string strColor = "FFFFFF";
            Tab_CommonItem line = TableManager.GetCommonItemByID(m_nItemId, 0);
            if (line != null)
            {
                switch ((Item.ItemClass)line.ClassID)
                {
                    case Item.ItemClass.EQUIP:
                    case Item.ItemClass.MATERIAL:
                    case Item.ItemClass.STRENGTHEN:
                    case Item.ItemClass.PRIZE:
                    case Item.ItemClass.MEDIC:
                    case Item.ItemClass.MISSION:
                    case Item.ItemClass.MOUNT:
                    case Item.ItemClass.FASHION:
                    case Item.ItemClass.FELLOW:
                        {
                            strColor = "70E718";
                        }
                        break;
                    default:
                        break;
                }
            }
            if (m_NameBoard)
            {
                m_NameBoard.color = Utils.GetColorByString(strColor);
            }
        }
        //public UISpriteAnimation m_SpriteAnimation;
        public void ShowItemSprite()
        {
            if (DropType == (int)GameDefine_Globe.DROP_TYPE.DROP_ITEM)
            {
                Tab_CommonItem line = TableManager.GetCommonItemByID(m_nItemId, 0);
                if (line != null)
                {
                    m_ItemSprite.spriteName = line.DropIcon;
                    m_ItemSprite.MakePixelPerfect();
                }
            }
            else if (DropType == (int)GameDefine_Globe.DROP_TYPE.DROP_COIN)
            {
                //                 m_ItemSprite.spriteName = "DiaoLuo_Coin_1";
                //                 m_SpriteAnimation.namePrefix = "DiaoLuo_Coin_";
                //                 if (!m_SpriteAnimation.enabled)
                //                 {
                //                     m_SpriteAnimation.enabled = true;
                //                 }
                //                 else
                //                 {
                //                     m_SpriteAnimation.Reset();
                //                 }

                //                 if (ItemCount <100 )
                //                 {
                //                     m_ItemSprite.spriteName = "DiaoLuo_JinBiXiao";
                //                 }
                //                 else if (ItemCount < 300)
                //                 {
                //                     m_ItemSprite.spriteName = "DiaoLuo_JinBiZhong";
                //                 }
                //                 else
                //                {
                m_ItemSprite.spriteName = "DiaoLuo_Coin_5";
                //                }

                PlayEffect(175);
                m_ItemSprite.MakePixelPerfect();
            }
        }
        public void UpdateMove(float fSpeed)
        {
            Obj_MainPlayer OwnerUser = Singleton<ObjManager>.Instance.MainPlayer;
            if (null != OwnerUser && OwnerUser.GUID == OwnerGuid && OwnerUser.IsDie() == false && m_ObjTransform != null)
            {
                Vector3 OwnerPos = OwnerUser.transform.position;
                OwnerPos.y += 1;
                Vector3 pos = OwnerPos - m_ObjTransform.position;
                Vector3 newPos = pos * fSpeed * Time.deltaTime;
                m_ObjTransform.position += newPos;
                m_ObjTransform.localScale = m_localScale * m_fScaling;
                m_fScaling = 1;

            }
        }
    }
}
