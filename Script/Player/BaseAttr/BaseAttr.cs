/********************************************************************************
 *	文件名：	BaseAttr.cs
 *	全路径：	\Script\Player\BaseAttr.cs
 *	创建人：	李嘉
 *	创建时间：2013-11-20
 *
 *	功能说明：Obj基础战斗属性，以及基础属性换算公式
 *	修改记录：
*********************************************************************************/
using UnityEngine;
using System.Collections;
using Games.LogicObj;
using Games.GlobeDefine;
using System;

public enum SKILLDELANDGAINTYPE //技能的消耗和收益
{
    HPTYPE_INVAILID =-1,
    HPTYPE_VALUE =101,//血值
    HPTYPE_RATE  =102,//血百分百
    MPTYPE_VALUE = 201,//蓝值
    MPTYPE_RATE = 202,//蓝百分百
    XPTYPE_VALUE = 301,//战意值
    XPTYPE_RATE = 302,//战意百分百
    COINTYPE =401,//金币
}
public enum COMBATATTE
{
    MAXHP           =0,     //血上限
    MAXMP           =1,     //蓝上限
    MAXXP           =2,     //战意上限
    PYSATTACK       =3,     //物理攻击
    MAGATTACK       =4,     //魔法攻击
    PYSDEF          =5,     //物理防御
    MAGDEF          =6,     //魔法防御
    HIT             =7,     //命中
    DODGE           =8,     //闪避
    CRITICAL        =9,     //暴击
    DECRITICAL      =10,    //暴抗
    STRIKE          =11,    //穿透
    DUCTICAL        =12,    //韧性
    CRITIADD        =13,    //暴击伤害加成
    CRITIMIS        =14,    //暴击伤害减免
    MOVESPEED       =15,    //移动速度
    ATTACKSPEED     =16,    //攻击速度
    COMBATATTE_MAXNUM,
}

public enum  MIXBATATTR
{
    MIXATTR_BEGIN = 999,
    MIXATTR_ALLATTACK = 1000,
    MIXATTR_ALLDEF = 1001,
}

public class MaxBatAttrUtil
{
    public static bool IsContainType(MIXBATATTR nMixType, COMBATATTE nJudgeAttrType)
    {
        switch (nMixType)
        {
            case MIXBATATTR.MIXATTR_ALLDEF:
                {
                    if (nJudgeAttrType == COMBATATTE.PYSDEF || nJudgeAttrType == COMBATATTE.MAGDEF)
                    {
                        return true;
                    }
                }
                break;
            case MIXBATATTR.MIXATTR_ALLATTACK:
                {
                    if (nJudgeAttrType == COMBATATTE.PYSATTACK || nJudgeAttrType == COMBATATTE.MAGATTACK)
                    {
                        return true;
                    }
                }
                break;
            default:
                break;
        }
        return false;
    }
}

public class BaseAttr
{
    //生命值
    private int m_nHP = 0;
    public int HP 
    {
        get { return m_nHP; } 
        set { m_nHP = value; } 
    }
    private int m_nMaxHP = 0;
    public int MaxHP
    {
        get { return m_nMaxHP; }
        set { m_nMaxHP = value; }
    }
    //法力值
    private int m_nMp = 0;
    public int MP 
    { 
        get { return m_nMp; } 
        set { m_nMp = value; } 
    }

    private int m_nMaxMp = 0;
    public int MaxMP
    {
        get { return m_nMaxMp; }
        set { m_nMaxMp = value; }
    }
    //战意
    private int m_nXp = 0;
    public int XP 
    { 
        get { return m_nXp; } 
        set { m_nXp = value; } 
    }
    
    private int m_nMaxXp = 0;
    public int MaxXP
    {
        get { return m_nMaxXp; }
        set { m_nMaxXp = value; }
    }

    public int Exp { set; get; }
    //名字
    private string m_RoleName = "";
    public string RoleName
    {
        get { return m_RoleName; }
        set { m_RoleName = value; }
    }
  
    //等级
    private int m_nLevel = 0;
    public int Level
    {
        get { return m_nLevel; }
        set { m_nLevel = value; }
    }

    // 头像
    private string m_strHeadPic = "";
    public string HeadPic
    {
        get { return m_strHeadPic; }
        set { m_strHeadPic = value; }
    }

    //数据表ID,既RoleBase中的ID
    private int m_nRoleBaseID = -1;
    public int RoleBaseID
    {
        get { return m_nRoleBaseID; }
        set { m_nRoleBaseID = value; }
    }
    
    private int m_nForce = -1;
    public int Force
    {
        get { return m_nForce; }
        set { m_nForce = value; }
    }

    private float m_fMoveSpeed = 5.0f;
    public float MoveSpeed
    {
        get { return m_fMoveSpeed; }
        set { m_fMoveSpeed = value; }
    }

    //死亡状态
    private bool m_bDie =false;
    public bool Die
    {
        get { return m_bDie; }
        set { m_bDie = value; }
    }

    //朝向
    private float m_fDirection = 0.0f;
    public float Direction
    {
        get { return m_fDirection; }
        set { m_fDirection = value; }
    }

    //战斗力
    private int m_nCombatValue = 0;
    public int CombatValue
    {
        get { return m_nCombatValue; }
        set { m_nCombatValue = value; }
    }
    //体能
    private int m_nCurStamina = 0;
    public int CurStamina
    {
        get { return m_nCurStamina; }
        set { m_nCurStamina = value; }
    }
    private int m_nOffLineExp = 0;
    public int OffLineExp
    {
        get { return m_nOffLineExp;  }
        set { m_nOffLineExp = value;  }
    }
//    //物理攻击
//    private int m_nPAttack = 0;
//    public int PAttack
//    {
//        get { return m_nPAttack; }
//        set { m_nPAttack = value; }
//    }

//    //法术攻击
//    private int m_nMAttack = 0;
//    public int MAttack
//    {
//        get { return m_nMAttack; }
//        set { m_nMAttack = value; }
//    }
//    //物理防御
//    private int m_nPDefend = 0;
//    public int PDefend
//    {
//        get { return m_nPDefend; }
//        set { m_nPDefend = value; }
//    }

//    //法术防御
//    private int m_nMDefend = 0;
//    public int MDefend
//    {
//        get { return m_nMDefend; }
//        set { m_nMDefend = value; }
//    }

//    //物理命中
//    private int m_nPHit = 0;
//    public int PHit
//    {
//        get { return m_nPHit; }
//        set { m_nPHit = value; }
//    }

//    //物理闪避
//    private int m_nPDodge = 0;
//    public int PDodge
//    {
//        get { return m_nPDodge; }
//        set { m_nPDodge = value; }
//    }

//    //暴击
//    private int m_nCritical = 0;
//    public int Critical
//    {
//        get { return m_nCritical; }
//        set { m_nCritical = value; }
//    }

//    //暴抗
//    private int m_nDeCritical = 0;
//    public int DeCritical
//    {
//        get { return m_nDeCritical; }
//        set { m_nDeCritical = value; }
//    }

//    //攻击速度
//    private float m_nAttackSpeed = 0;
//    public float AttackSpeed
//    {
//        get { return m_nAttackSpeed; }
//        set { m_nAttackSpeed = value; }
//    }

//    //警戒范围
//    private float m_fAlertRadius = 0;
//    public float AlertRadius
//    {
//        get { return m_fAlertRadius; }
//        set { m_fAlertRadius = value; }
//    }
//    //巡逻范围
//    private float m_fPathRadius = 0;
//    public float PathRadius
//    {
//        get { return m_fPathRadius; }
//        set { m_fPathRadius = value; }
//    }
//    //攻击范围
//    private float m_fAttrRadius = 0;
//    public float AttrRadius
//    {
//        get { return m_fAttrRadius; }
//        set { m_fAttrRadius = value; }
//    }


//    //部分固定参数
//    public static float fHitMin = 0.0f;                  //命中下限
//    public static float fHitMax = 1.0f;                  //命中上限
//    public static float fCriMin = 0.0f;                  //暴击下限
//    public static float fCriMax = 1.0f;                  //暴击上限
//    public static float fPDamageRatio = 0.1f;            //物理攻击伤害下限系数
//    public static float fMDamageRatio = 0.08f;           //法术攻击伤害下限系数
//    public static float fPCriAddition = 1.2f;            //物理攻击暴击加成
//    public static float fMCriAddition = 1.2f;            //法术攻击暴击加成
//    public static float fPCriDamageRatio = 0.12f;        //物理攻击暴击伤害下限系数
//    public static float fMCriDamageRatio = 0.096f;       //法术攻击暴击伤害下限系数

//    public static float fDamageRangeMin = 0.98f;         //伤害随机范围波动下限
//    public static float fDamageRangeMax = 1.02f;         //伤害随机范围波动上限

//    //////////////////////////////////////////////////////////////////////////
//    //下面为战斗属性相关计算公式
//    //全部采用Static接口
//    //////////////////////////////////////////////////////////////////////////
//    //获得物理伤害，attacker为攻击者，defender为防守者，之后以此类推
//    public static int GetPDamage(BaseAttr attacker, BaseAttr defender,out bool isHit,out bool isCritical)
//    {
//        int nPDamage = 0;
//        isHit = false;
//        isCritical = false;
//        if (null == attacker || null == defender)
//        {
//            return nPDamage;
//        }

//        if (false == IsHit(attacker, defender))
//        {
//            return nPDamage;
//        }
//        isHit = true;
//        isCritical = IsCritical(attacker, defender);
//        if (isCritical)
//        {
//            //命中且暴击	
//            //物理伤害=max(攻击方物理攻击*(d+max((攻击方暴击伤害加成-被攻击方暴击伤害减免),0))-(1-(攻击方穿透-被攻击方韧性)/10000)*被攻击方物理防御,e*攻击方物理攻击)
//            //目前无攻击方暴击伤害加成和被攻击方暴击伤害减免以及攻击方穿透和被攻击方韧性，按0处理
//            //公式简化为：max(攻击方物理攻击*d-被攻击方物理防御, e*攻击方物理攻击)
//            //d为暴击加成基数，e为伤害下限系数			d=1.2	e=12%
//            nPDamage = (int)Math.Max((attacker.PAttack * BaseAttr.fPCriAddition - defender.PDefend), BaseAttr.fPCriDamageRatio * attacker.PAttack);
//        }
//        else
//        {
//            //命中未暴击	
//            //物理伤害=max(攻击方物理攻击-(1-(攻击方穿透-被攻击方韧性)/10000)*被攻击方物理防御,c*攻击方物理攻击)
//            //目前无攻击方穿透和被攻击方韧性，按照0处理，则公式简化为
//            //max(攻击方物理攻击-被攻击方物理防御， c*攻击方物理攻击)
//            //c为伤害下限系数		c=10%
//            nPDamage = (int)Math.Max((attacker.PAttack - defender.PDefend), BaseAttr.fPDamageRatio * attacker.PAttack);
//        }

//        //乘以伤害波动方位
//        nPDamage = (int)(nPDamage * UnityEngine.Random.Range(fDamageRangeMin, fDamageRangeMax));

//        return nPDamage;
//    }

//    //获得法术伤害
//    public static int GetMDamage(BaseAttr attacker, BaseAttr defender)
//    {
//        int nMDamage = 0;
//        if (null == attacker || null == defender)
//        {
//            return nMDamage;
//        }

//        if (false == IsHit(attacker, defender))
//        {
//            return nMDamage;
//        }

//        bool bIsCri = IsCritical(attacker, defender);
//        if (bIsCri)
//        {
//            //命中且暴击	
//            //法术伤害=max(攻击方法术攻击*(d+max((攻击方暴击伤害加成-被攻击方暴击伤害减免),0))-(1-(攻击方穿透-被攻击方韧性)/10000)*被攻击方法术防御,e*攻击方法术攻击)
//            //目前无攻击方暴击伤害加成和被攻击方暴击伤害减免以及攻击方穿透和被攻击方韧性，按0处理
//            //公式简化为：max(攻击方法术攻击*d-被攻击方法术防御, e*攻击方物理攻击)
//            //d为暴击加成基数，e为伤害下限系数			d=1.2	e=9.6%
//            nMDamage = (int)Math.Max((attacker.MAttack * BaseAttr.fMCriAddition - defender.MDefend), BaseAttr.fMCriDamageRatio * attacker.MAttack);

//        }
//        else
//        {
//            //命中未暴击	
//            //法术伤害=max(攻击方法术攻击-(1-(攻击方穿透-被攻击方韧性)/10000)*被攻击方法术防御,c*攻击方法术攻击)     
//            //目前无攻击方穿透和被攻击方韧性，按照0处理，则公式简化为
//            //max(攻击方法术攻击-被攻击方法术防御， c*攻击方法术攻击)  
//            //c为伤害下限系数		c=8%
//            nMDamage = (int)Math.Max((attacker.MAttack - defender.MDefend), BaseAttr.fMDamageRatio * attacker.MAttack);
//        }

//        nMDamage = (int)(nMDamage * UnityEngine.Random.Range(fDamageRangeMin, fDamageRangeMax));
//        return nMDamage;
//    }

//    //是否命中
//    private static bool IsHit(BaseAttr attacker, BaseAttr defender)
//    {
//        if (null == attacker || null == defender)
//        {
//            return false;
//        }

//        float fAttackerPHit = (float)attacker.PHit;
//        float fDefenderPDog = (float)defender.PDodge;
//        if (0 == 10000 * (1 - fDefenderPDog / 10000))
//        {
//            return false;
//        }

//        float fPHitRate = Math.Min(Math.Max(fAttackerPHit / 10000 * (1 - fDefenderPDog / 10000), 0), 1);

//        float fRandom = UnityEngine.Random.Range(0.0f, 1.0f);
//        if (fRandom <= fPHitRate)
//        {
//            return true;
//        }

//        return false;
//    }

//    //是否暴击
//    private static bool IsCritical(BaseAttr attacker, BaseAttr defender)
//    {
//        if (null == attacker || null == defender)
//        {
//            return false;
//        }

//        float fAttackerPCri = (float)attacker.Critical;
//        float fDefenderPDeCri = (float)defender.DeCritical;

//        if (0 == 10000 * (1 - fDefenderPDeCri / 10000))
//        {
//            return false;
//        }

//        float fPCri = Math.Min(Math.Max(fAttackerPCri / 10000 * (1 - fDefenderPDeCri / 10000), 0), 1);

//        float fRandom = UnityEngine.Random.Range(0.0f, 1.0f);
//        if (fRandom <= fPCri)
//        {
//            return true;
//        }
//        return false;
//    }


}
