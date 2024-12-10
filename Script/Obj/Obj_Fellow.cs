/********************************************************************************
 *	文件名：	Obj_Fellow.cs
 *	全路径：	\Script\Obj\Obj_Fellow.cs
 *	创建人：	李嘉
 *	创建时间：2013-10-25
 *
 *	功能说明：游戏伙伴Obj逻辑类
 *	修改记录：
*********************************************************************************/

using GCGame;
using UnityEngine;
using System.Collections;
using Games.GlobeDefine;
using Games.Scene;
using GCGame.Table;
using Games.Animation_Modle;
using System;

namespace Games.LogicObj
{
    public class Obj_Fellow : Obj_Character
    {
        public Obj_Fellow()
        {
            m_ObjType = GameDefine_Globe.OBJ_TYPE.OBJ_FELLOW;
            m_OwnerObjId = -1;
        }

        public override bool Init(Obj_Init_Data initData)
        {
            if (null == m_ObjTransform)
            {
                m_ObjTransform = gameObject.transform;
            }

            m_ObjTransform.position = ActiveScene.GetTerrainPosition(new Vector3(initData.m_fX, 0, initData.m_fZ));

            //暂时 写死了 后面再用服务器
            m_ObjTransform.Rotate(Vector3.up * 135);

            //服务器发过来的信息               
            this.ServerID = initData.m_ServerID;
            BaseAttr.RoleBaseID = initData.m_RoleBaseID;
            BaseAttr.RoleName = initData.m_StrName;
            m_OwnerObjId = initData.m_OwnerObjId;
            StealthLev = initData.m_StealthLev;
            m_Quality = initData.m_FellowQuality;
            OptStealthLevChange();
            //防止伙伴追上人物导致动作不流畅 把客户端主角伙伴速度修改为和人物一样
            if (IsOwnedByMainPlayer() == true)
            {
                Singleton<ObjManager>.GetInstance().MainPlayer.CurFellowObjId = ServerID;
                BaseAttr.MoveSpeed = Singleton<ObjManager>.GetInstance().MainPlayer.BaseAttr.MoveSpeed;
            }
            else
            {
                Obj_OtherPlayer otherPlayer = Singleton<ObjManager>.Instance.FindObjInScene(m_OwnerObjId) as Obj_OtherPlayer;
                if (null != otherPlayer)
                {
                    otherPlayer.FellowID = ServerID;
                    m_bVisible = otherPlayer.IsVisibleChar();
                }

                BaseAttr.MoveSpeed = initData.m_MoveSpeed;
            }
            Tab_FellowAttr fellowAttrTab = TableManager.GetFellowAttrByID(BaseAttr.RoleBaseID, 0);
            if (fellowAttrTab != null)
            {
                //初始化CharModelID，并读取部分客户端信息
                ModelID = fellowAttrTab.ModelId;
                Tab_CharModel charModel = TableManager.GetCharModelByID(ModelID, 0);
                if (null != charModel)
                {
                    BaseAttr.HeadPic = charModel.HeadPic;
                    //设置动作路径
                    AnimationFilePath = charModel.AnimPath;
                    //设置名字版高度
                    DeltaHeight = charModel.HeadInfoHeight;

                    m_ObjTransform.localScale = new Vector3(charModel.Scale, charModel.Scale, charModel.Scale);
                    m_ObjTransform.localRotation = Utils.DirServerToClient(initData.m_fDir);
                }
            }
            

            //初始化寻路代理
            InitNavAgent();

            //初始化AutoMove功能模块
            //if (gameObject.GetComponent<AutoMove>() == null)
            //{
            gameObject.AddComponent<AutoMove>();
            //}

            //初始化动画，需要在AnimationFilePath被赋值后进行
            if (AnimLogic == null)
            {
                AnimLogic = gameObject.AddComponent<AnimationLogic>();
            }
            //AnimLogic = gameObject.GetComponent<AnimationLogic>();
            //初始化特效
            if (ObjEffectLogic == null)
            {
                ObjEffectLogic = gameObject.AddComponent<EffectLogic>();
            }
            //ObjEffectLogic = gameObject.GetComponent<EffectLogic>();
            InitEffect();

            //召出播放特效
            if (Singleton<ObjManager>.GetInstance().MainPlayer != null)
            {
                if (m_OwnerObjId == Singleton<ObjManager>.GetInstance().MainPlayer.ServerID)
                {
                    if (GameManager.gameManager.PlayerDataPool.FellowPlayerEffect == true)
                    {
                        PlayEffect(52);
                        GameManager.gameManager.PlayerDataPool.FellowPlayerEffect = false;
                        //播放音效 改为UI召唤按钮播放
                        //GameManager.gameManager.SoundManager.PlaySoundEffect("pet_call");
                    }
                }
            }

            if (IsDie())
            {
                OnCorpse();
            }
            else
            {
                if (Objanimation != null)
                {
                    Objanimation.Stop();
                }
                CurObjAnimState = GameDefine_Globe.OBJ_ANIMSTATE.STATE_NORMOR;
            }

            InitNameBoard();

            return base.Init(initData);
        }

        //进入可视区域
        void OnBecameVisible()
        {
            //设置是否在视口内标记位，为其他系统优化判断标识
            ModelInViewPort = true;
           
            //显示名字版
            if (null != m_HeadInfoBoard)
            {
                m_HeadInfoBoard.SetActive(true);
                OnSwithObjAnimState(CurObjAnimState);
            }

            //显示模型
            if (null != ModelNode && IsVisibleChar())
            {
                ModelNode.SetActive(true);
            }
        }

        //离开可视区域
        void OnBecameInvisible()
        {
            //设置是否在视口内标记位，为其他系统优化判断标识
            ModelInViewPort = false;

            //隐藏名字版
            if (null != m_HeadInfoBoard)
            {
                m_HeadInfoBoard.SetActive(false);
            }

            //隐藏模型
            if (null != ModelNode)
            {
                ModelNode.SetActive(false);
            }
        }

        //更新Obj_Fellow逻辑数据
        void FixedUpdate()
        {
            UpdateTargetMove();
            UpdateAnimation();
            //主角伙伴移动
            if (IsOwnedByMainPlayer())
            {
                UpdateFellowMove();
                //Obj_MainPlayer _mainPlayer = Singleton<ObjManager>.GetInstance().MainPlayer;
                ////隐身半透
                //if (_mainPlayer)
                //{
                //    if (_mainPlayer.IsHaveStealthBuff() && m_bIsSteathShader == false)
                //    {
                //        SetStealthState();
                //    }
                //    else if (_mainPlayer.IsHaveStealthBuff() == false && m_bIsSteathShader)
                //    {
                //        CancelStealthState();
                //    }
                //}
            }
          
            //技能结束检测
            if (m_SkillCore != null)
            {
                m_SkillCore.CheckSkillShouldFinish();
            }

        }
                        
        void InitNameBoard()
        {
            ResourceManager.LoadHeadInfoPrefab(UIInfo.FellowHeadInfo, gameObject, "FellowHeadInfo", OnLoadNameBoard);
        }

        void OnLoadNameBoard(GameObject objNameBoard)
        {
            m_HeadInfoBoard = objNameBoard;
            if (null != m_HeadInfoBoard)
            {
                m_NameBoard = m_HeadInfoBoard.transform.FindChild("NameBoardOffset").FindChild("NameBoard").GetComponent<UILabel>();
                //m_DamageBoard = m_HeadInfoBoard.transform.FindChild("DamageBoard").GetComponent<DamageBoardManager>();
                //if (null != m_DamageBoard)
                //{
                //    m_DamageBoard.gameObject.SetActive(false);
                //}

                ShowNameBoard();
            }
        }
        
        public override Color GetNameBoardColor()
        {
            //伙伴名字根据品质决定
            string strColor = Utils.GetFellowNameColor(m_Quality);

            return GCGame.Utils.GetColorByString(strColor);
        }

        /// <summary>
        /// 伙伴品质
        /// </summary>
        private int m_Quality;
        public int Quality
        {
            get { return m_Quality; }
            set { m_Quality = value; }
        }

        private int m_OwnerObjId;
        public int OwnerObjId
        {
            get { return m_OwnerObjId; }
            set { m_OwnerObjId = value; }
        }

        /// <summary>
        /// 是否主角的伙伴
        /// </summary>
        public bool IsOwnedByMainPlayer()
        {
            if (Singleton<ObjManager>.GetInstance().MainPlayer != null)
            {
                return (m_OwnerObjId == Singleton<ObjManager>.GetInstance().MainPlayer.ServerID);
            }
            return false;
        }

        /// <summary>
        /// 把主角伙伴客户端速度设置为与主角一致
        /// </summary>
        public void SetMoveSpeedAsMainPlayer()
        {
            if (Singleton<ObjManager>.GetInstance().MainPlayer != null)
            { 
                if (IsOwnedByMainPlayer())
                {
                    BaseAttr.MoveSpeed = Singleton<ObjManager>.GetInstance().MainPlayer.BaseAttr.MoveSpeed;
                    if (NavAgent != null)
                    {
                        NavAgent.speed = BaseAttr.MoveSpeed;
                    }
                }
            }
        }

        private void UpdateFellowMove()
        {
            SetMoveSpeedAsMainPlayer();
            Obj_MainPlayer mainPlayer = Singleton<ObjManager>.GetInstance().MainPlayer;
            //TODO 判读是否在战斗
            Vector3 targetPos = GetFellowPos(mainPlayer);
            if (Vector3.Distance(Position, targetPos) >= 6)
            {
                //距离太远 直接拉过去
                if (NavAgent != null)
                {
                    UnityEngine.GameObject.DestroyImmediate(NavAgent);
                }
                m_ObjTransform.position = ActiveScene.GetTerrainPosition(new Vector3(targetPos.x, 0f, targetPos.z));
                if (NavAgent == null)
                {
                    InitNavAgent();
                }
            }
            else if (Vector3.Distance(Position, targetPos) >= 2 || (IsMoving && Vector3.Distance(Position, targetPos) >= 1))
            {
                MoveTo(targetPos, null, 0f);
            }
        }

        private Vector3 GetFellowPos(Obj_MainPlayer mainPlayer)
        {
            float fAngle = (float)(Math.PI * (5.0f / 4.0f));
            int nDist = 1;

            Vector3 retPos = Position;
            //主角位置
            Vector3 mainPos = mainPlayer.Position;
            //主角朝向
            float mainDir = Utils.DirClientToServer(mainPlayer.Rotation);
            mainDir = Utils.NormaliseDirection(mainDir);

            retPos.x = (float)(nDist * Math.Cos(fAngle));
            retPos.z = (float)(nDist * Math.Sin(fAngle));
            //旋转
            float x = (float)(retPos.x * Math.Cos(mainDir) - retPos.z * Math.Sin(mainDir));
            float z = (float)(retPos.z * Math.Cos(mainDir) + retPos.x * Math.Sin(mainDir));
            retPos.x = x;
            retPos.z = z;
            //平移
            retPos.x += mainPos.x;
            retPos.z += mainPos.z;

            return retPos;
        }

    }
}
