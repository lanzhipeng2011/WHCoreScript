using UnityEngine;
using System.Collections;
using Games.LogicObj;
using Games.GlobeDefine;

public class Obj_ZombieUser : Obj_OtherPlayer 
{
    public override bool Init(Obj_Init_Data initData)
    {
        BaseAttr.MoveSpeed = m_fMoveSpeed;
        //服务器发过来的信息    
        m_ObjType = GameDefine_Globe.OBJ_TYPE.OBJ_ZOMBIE_PLAYER;
        StealthLev = initData.m_StealthLev;
        OptStealthLevChange();
        if (null == transform)
        {
            m_ObjTransform = transform;
        }

        return base.Init(initData);
    }
    
    void FixedUpdate()
    {
        UpdateTargetMove();
        UpdateAnimation();

        //多次冒血的
        UpdateShowMultiShowDamageBoard();
        //技能结束检测
        if (m_SkillCore != null)
        {
            m_SkillCore.CheckSkillShouldFinish();
        }
    }

    void OnBecameVisible()
    {
        //设置是否在视口内标记位，为其他系统优化判断标识
        ModelInViewPort = true;

        //显示名字版
        if (null != m_HeadInfoBoard)
        {
            m_HeadInfoBoard.SetActive(true);
        }

        //显示模型
        if (null != ModelNode && IsVisibleChar())
        {
            ModelNode.SetActive(true);
            OnSwithObjAnimState(CurObjAnimState);
        }

        //如果已经死亡，则切换到死亡状态
        if (IsDie())
        {
            OnCorpse();
        }
    }

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
}
