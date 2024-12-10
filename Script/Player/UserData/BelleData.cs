/********************************************************************
	created:	2014/01/16
	created:	16:1:2014   14:06
	filename: 	BelleData.cs
	author:		王迪
	
	purpose:	美人系统相关数据
*********************************************************************/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GCGame.Table;
using Module.Log;
using Games.LogicObj;
using GCGame;
public class Belle
{
    public int id;                      // ID
    public int colorLevel;                   // 品阶
    public int subLevel;
    public int orgLevel;
    public int level
    {
        set
        {
            orgLevel = value;
            colorLevel = (orgLevel - 1) / 9 + 1;
            subLevel = (value-1) % 9 + 1;
        }
    }
    public int matrixID;                // 所在美人阵ID，无阵为-1
    public int matrixIndex;             // 所在美人阵位置，默认0
    //public int nextEvolutionTime;       // 下次可进化时间，暂时不用了
    public int favourValue;             // 亲密度

    public Dictionary<int, int> attrMap = new Dictionary<int, int>();
    public void UpdateAttrMap()
    {
        attrMap.Clear();

        Tab_Belle curTabBelle = TableManager.GetBelleByID(id, 0);
        if (null == curTabBelle)
        {
            return;
        }
        Belle curBelleData = null;
        if (BelleData.OwnedBelleMap.ContainsKey(id))
        {
            curBelleData = BelleData.OwnedBelleMap[id];
        }

        if (null == curTabBelle)
        {
            return;
        }
        // 计算面板数值

        int belleAdditonTableCount = TableManager.GetBelleAddition().Count;
        for (int curLevel = 0; curLevel < colorLevel; curLevel++)
        {
            if (curLevel < curTabBelle.getAttrTypeCount())
            {
                // 属性X增加N
                int curType = curTabBelle.GetAttrTypebyIndex(curLevel);
                int curValueAdd = curTabBelle.GetAttrValuebyIndex(curLevel);
                if (curLevel < curBelleData.colorLevel)
                {
                    if (curLevel == curBelleData.colorLevel - 1)
                    {
                        curValueAdd = (int)(curValueAdd * curBelleData.subLevel);
                    }
                    else
                    {
                        curValueAdd = (int)(curValueAdd * 9);
                    }

                    if (attrMap.ContainsKey(curType))
                    {
                        attrMap[curType] += curValueAdd;
                    }
                    else
                    {
                        attrMap.Add(curType, curValueAdd);
                    }

                    for (int additonIndex = 0; additonIndex < belleAdditonTableCount; additonIndex++)
                    {
                        Tab_BelleAddition curTabBelleAdditon = TableManager.GetBelleAdditionByID(additonIndex, 0);
                        if (null == curTabBelleAdditon)
                        {
                            continue;
                        }

                        if (BelleData.OwnedBelleMap.Count == curTabBelleAdditon.BelleCount && id == curTabBelleAdditon.BelleID && curLevel < curTabBelleAdditon.getAdditionValueCount())
                        {
                            attrMap[curType] += curTabBelleAdditon.GetAdditionValuebyIndex(curLevel);
                        }
                    }
                }
            }
        }
    }
}

public class BelleMatrix
{
    public BelleMatrix(int _id, bool _isActive, int _belleCount)
    {
        id = _id;
        isActive = _isActive;
        belleIDs = new int[_belleCount];
        for (int i = 0; i < _belleCount; i++)
        {
            belleIDs[i] = -1;
        }
    }
    public int      id;                 // id
    public bool     isActive;           // 是否激活
    public int[]    belleIDs;           // 阵上美人列表

    public Dictionary<int, int> attrMap = new Dictionary<int, int>();
    public Dictionary<int, int> attrAffectionMap = new Dictionary<int, int>();
    public Dictionary<int, int> attrBelleMap = new Dictionary<int, int>();

    public bool m_bAffectionActive = false;
    private static float m_lastTotalValue = 0;     // 记录上次激活的时候情缘总值，以此来判断情缘是否发生变化

    public void UpdateAttrMap(bool bCalAffection = false)
    {
        attrMap.Clear();
        attrBelleMap.Clear();
        attrAffectionMap.Clear();
        m_bAffectionActive = false;

        int belleMinLevel = 0;
        int curValidBelleCount = 0;
        float curTotalValue = 0;

        for (int i = 0; i < belleIDs.Length; i++)
        {
            // 收集面板加成
            Belle curBelleData = null;
            if (BelleData.OwnedBelleMap.ContainsKey(belleIDs[i]))
            {
                curBelleData = BelleData.OwnedBelleMap[belleIDs[i]];
            }

            if(null == curBelleData)
            {
                continue;
            }

            curValidBelleCount++;
            if(curBelleData.colorLevel < belleMinLevel)
            {
                belleMinLevel = curBelleData.colorLevel;
            }
            curBelleData.UpdateAttrMap();
            foreach (KeyValuePair<int, int> curPair in curBelleData.attrMap)
            {
                if (attrMap.ContainsKey(curPair.Key))
                {
                    attrMap[curPair.Key] += curPair.Value;
                }
                else
                {
                    attrMap.Add(curPair.Key, curPair.Value);
                }

                if (attrBelleMap.ContainsKey(curPair.Key))
                {
                    attrBelleMap[curPair.Key] += curPair.Value;
                }
                else
                {
                    attrBelleMap.Add(curPair.Key, curPair.Value);
                }
            }

            // 收集情缘加成

            if (!isActive)
            {
                continue;
            }

            Tab_Belle curTabBelle = TableManager.GetBelleByID(belleIDs[i], 0);
            if (null == curBelleData)
            {
                continue;
            }

            for (int curAffectoinIndex = 0; curAffectoinIndex < curTabBelle.getAffectionConditonTypeCount(); curAffectoinIndex++)
            {
                int type = curTabBelle.GetAffectionConditonTypebyIndex(curAffectoinIndex);
                bool bAddValue = false;
                switch (type)
                {
                    case 1:
                        {
                            // 同上阵
                            int targetBelleID = curTabBelle.GetAffectionConditonValuebyIndex(curAffectoinIndex);
                            if (BelleData.OwnedBelleMap.ContainsKey(targetBelleID) && null != BelleData.OwnedBelleMap[targetBelleID])
                            {
                                if (BelleData.OwnedBelleMap[targetBelleID].matrixID == curBelleData.matrixID)
                                {
                                    bAddValue = true;
                                }
                            }
                        }
                        break;
                    case 2:
                        {
                            int targetLevel = curTabBelle.GetAffectionConditonValuebyIndex(curAffectoinIndex);
                            if (curBelleData.colorLevel > targetLevel || (curBelleData.colorLevel == targetLevel && curBelleData.subLevel == 9))
                            {
                                bAddValue = true;
                            }
                        }
                        break;
                }

                if (bAddValue)
                {

                    int attrType = curTabBelle.GetAffectionGainsTypebyIndex(curAffectoinIndex);
                    int attrValue = curTabBelle.GetAffectionGainsValuebyIndex(curAffectoinIndex);
                    curTotalValue += attrValue;

                    m_bAffectionActive = true;

                    if (attrMap.ContainsKey(attrType))
                    {
                        attrMap[attrType] += attrValue;
                    }
                    else
                    {
                        attrMap.Add(attrType, attrValue);
                    }

                    if (attrAffectionMap.ContainsKey(attrType))
                    {
                        attrAffectionMap[attrType] += attrValue;
                    }
                    else
                    {
                        attrAffectionMap.Add(attrType, attrValue);
                    }
                }
            }
           
        }

        if (bCalAffection)
        {
            if (m_bAffectionActive)
            {
                m_bAffectionActive = curTotalValue != m_lastTotalValue;
            }
            m_lastTotalValue = curTotalValue;
        }
        else
        {
            m_bAffectionActive = false;
        }

        // 收集阵法加成 
        Tab_BelleMatrix curBelleMatrix = TableManager.GetBelleMatrixByID(id, 0);
        if (null == curBelleMatrix)
        {
            return;
        }

        if(curBelleMatrix.BelleCount > curValidBelleCount)
        {
            return;
        }
        belleMinLevel -= 1;
        if (belleMinLevel >= 0 && belleMinLevel < curBelleMatrix.getAttrGainsCount())
        {
            int type = curBelleMatrix.AttrType;
            float precent = curBelleMatrix.GetAttrGainsbyIndex(belleMinLevel);

            foreach (KeyValuePair<int, int> curPair in attrMap)
            {
                if (curPair.Key == type)
                {
                    attrMap[curPair.Key] = (int)(curPair.Value + curPair.Value * precent);
                }
            }
        }

       
    }
}

public class BelleData
{
    public const int MatrixBelleMax = 9;                                 //阵法上最多有9个美人
    public static int dayCloseTime = 0;                                 // 当日已亲密次数
    public static int dayMaxCloseTime = 0;                              // 当日可亲密最大次数
    public static int nextCloseTime = 0;                                // 下次亲密时间 realtimeSinceStartup
    public static int belleMatrixActiveMax = 1;                         // 可以激活的美人阵最大数量                  
    public static Dictionary<int, Belle> OwnedBelleMap = new Dictionary<int, Belle>();        // 玩家拥有的美人表
    public static Dictionary<int, BelleMatrix> OwnedMatrixMap = new Dictionary<int, BelleMatrix>();        // 玩家拥有的美人阵

    public static int m_belleActiveCount = 0;
    private static float m_lastEffectTime = 0;
    // 接收服务器推送的美人信息，更新数据
    public static void UpdateBelleData(GC_BELLE_DATA data)
    {
        OwnedBelleMap.Clear();
        OwnedMatrixMap.Clear();

        // 全局信息
        dayCloseTime = data.DayCloseTime;
        dayMaxCloseTime = data.DayMaxCloseTime;
        belleMatrixActiveMax = data.BelleMatrixActiveMax;
        nextCloseTime = (int)Time.realtimeSinceStartup + data.NextCloseTimer;
      
        // 美人信息
        int idCount = data.belleIDCount;
        for (int i = 0; i < idCount; i++)
        {
            Belle curBelle = new Belle();
            curBelle.id = data.belleIDList[i];

            curBelle.level = data.belleLevelList[i];
            curBelle.matrixID = data.belleMatrixIDList[i];
            curBelle.matrixIndex = data.belleMatrixIndexList[i];
            //curBelle.nextEvolutionTime = (int)Time.realtimeSinceStartup + data.belleNextEvolutionTimerList[i];
            curBelle.favourValue = data.belleFavourValueList[i];
            OwnedBelleMap.Add(curBelle.id, curBelle);
        }

        // 阵型信息
        int matrixCount = data.matrixIDCount;
        //matrixBelleMax = data.MatrixBelleCountMax;
        foreach (KeyValuePair<int, List<Tab_BelleMatrix>> curTabMatrix in TableManager.GetBelleMatrix())
        {
            BelleMatrix curMatrix = new BelleMatrix(curTabMatrix.Value[0].Id, false, curTabMatrix.Value[0].BelleCount);
            OwnedMatrixMap.Add(curMatrix.id, curMatrix);
        }

		int indexNum = 0;

        for (int i = 0; i < matrixCount; i++)
        {
            if (!OwnedMatrixMap.ContainsKey(data.matrixIDList[i]))
            {
                LogModule.ErrorLog("can not find cur matrix in matrix table id:" + data.matrixIDList[i]);
                continue;
            }

            // 服务器传来的长度是9，只截取表中定义的长度
             BelleMatrix curMatrix = OwnedMatrixMap[data.matrixIDList[i]];
             curMatrix.isActive = data.matrixIsActiveList[i] > 0;
            for (int j = 0; j < MatrixBelleMax; j++)
            {
                if (j < curMatrix.belleIDs.Length)
                {
					curMatrix.belleIDs[j] = (data.matrixBellesList[indexNum+j]);//(data.matrixBellesList[i * MatrixBelleMax + j]);
                }
            }
			indexNum += curMatrix.belleIDs.Length;
        }

        m_belleActiveCount = PlayerPreferenceData.BelleActiveTipCount;

        if (MenuBarLogic.Instance() != null)
        {
            MenuBarLogic.Instance().UpdateBelleTip();
        }
    }

    // 美人亲密消息
    public delegate void CloseDataDelegate();
    public static CloseDataDelegate delClose;
    public static void UpdateCloseData(GC_BELLE_CLOSE_RET packet)
    {
        dayCloseTime = packet.DayCloseTime;
        dayMaxCloseTime = packet.DayMaxCloseTime;
        nextCloseTime = (int)Time.realtimeSinceStartup + packet.NextCloserTimer;
        int belleID = packet.BelleID;
        if (OwnedBelleMap.ContainsKey(belleID))
        {
            if (packet.BelleFavourValue <= OwnedBelleMap[belleID].favourValue)
            {
                // 用此来判断是否达到亲密上限

                if (BackCamerControll.Instance() != null)
                {
                    BackCamerControll.Instance().PlaySceneEffect(208);
                }

                Obj_MainPlayer mainPlayer = Singleton<ObjManager>.Instance.MainPlayer;
                if (null != mainPlayer)
                {
                    mainPlayer.SendNoticMsg(false, "#{3046}");
                }
            }
            OwnedBelleMap[belleID].favourValue = packet.BelleFavourValue;
        }

        if (BackCamerControll.Instance() != null)
        {
            BackCamerControll.Instance().PlaySceneEffect(61);
        }

        if (null != GameManager.gameManager && null != GameManager.gameManager.SoundManager)
        {
            GameManager.gameManager.SoundManager.PlaySoundEffect(138);   //kiss
        }

        if (MenuBarLogic.Instance() != null)
        {
            MenuBarLogic.Instance().UpdateBelleTip();
        }
        //LevelUpController.ShowTipByID(1434);
        if (null != delClose) delClose();
    }

    // 美人进化消息
    public delegate void EvolutionDataDelegate();
    public static EvolutionDataDelegate delEvolution;
    public static void UpdateEvolutionData(GC_BELLE_EVOLUTION_RET packet)
    {
        int belleID = packet.BelleID;
        if (!OwnedBelleMap.ContainsKey(belleID))
        {
            LogModule.ErrorLog("receive an invalided bell id " + belleID.ToString());
            return;
        }

        Belle curBelle = OwnedBelleMap[belleID];
        //curBelle.nextEvolutionTime = (int)Time.realtimeSinceStartup + packet.NextEvolutionTimer;

        if (BackCamerControll.Instance() != null)
        {
            if (Time.time - m_lastEffectTime > 3)
            {
                BackCamerControll.Instance().PlaySceneEffect(61);
                m_lastEffectTime = Time.time;
            }
        }

        if (null != GameManager.gameManager && null != GameManager.gameManager.SoundManager)
        {
            GameManager.gameManager.SoundManager.PlaySoundEffect(138);   //kiss
        }
       
        //LevelUpController.ShowTipByID(1435);
        if (null != delEvolution) delEvolution();
    }

    // 美人加速进化消息
    public delegate void EvolutionRapidDataDelegate();
    public static EvolutionRapidDataDelegate delEvolutionRapid;
    public static void UpdateEvolutionRapidData(GC_BELLE_EVOLUTIONRAPID_RET packet)
    {
        int belleID = packet.BelleID;
        if (!OwnedBelleMap.ContainsKey(belleID))
        {
            LogModule.ErrorLog("receive an invalided bell id " + belleID.ToString());
            return;
        }

        Belle curBelle = OwnedBelleMap[belleID];
        curBelle.level = packet.Level;
        //curBelle.nextEvolutionTime = 0;
        if (null != delEvolution) delEvolutionRapid();
    }


    // 美人出阵消息
    public delegate void BattleDataDelegate();
    public static BattleDataDelegate delBattle;
    public static void UpdateBattleData(GC_BELLE_BATTLE_RET packet)
    {
        int belleID = packet.BelleID;
        int oldMatrixID = packet.OldMatrixID;
        int oldMatrixIndex = packet.OldMatrixIndex;
        int newMatrixID = packet.NewMatrixID;
        int newMatrixIndex = packet.NewMatrixIndex;
        Tab_BelleMatrix oldTabMatrix = TableManager.GetBelleMatrixByID(oldMatrixID, 0);
        Tab_BelleMatrix newTabMatrix = TableManager.GetBelleMatrixByID(newMatrixID, 0);
        if (null == newTabMatrix)
        {
            return;
        }

        int nOldBelleID = packet.RestBelleID;
        if (OwnedBelleMap.ContainsKey(nOldBelleID) && null != OwnedBelleMap[nOldBelleID])
        {
            OwnedBelleMap[nOldBelleID].matrixID = -1;
            OwnedBelleMap[nOldBelleID].matrixIndex = -1;
        }
        if (null != oldTabMatrix &&  OwnedMatrixMap.ContainsKey(oldMatrixID) && oldMatrixIndex >= 0 && oldMatrixIndex < oldTabMatrix.BelleCount)
        {
            OwnedMatrixMap[oldMatrixID].belleIDs[oldMatrixIndex] = -1;
            //OwnedMatrixMap[oldMatrixID].isActive = packet.OldMatrixIsActive > 0;
        }

        if (OwnedMatrixMap.ContainsKey(newMatrixID) && newMatrixIndex >= 0 && newMatrixIndex < newTabMatrix.BelleCount)
        {
            OwnedMatrixMap[newMatrixID].belleIDs[newMatrixIndex] = belleID;
            
            //OwnedMatrixMap[newMatrixID].isActive = packet.NewMatrixIsActive > 0;
        }

        if (OwnedBelleMap.ContainsKey(belleID))
        {
            OwnedBelleMap[belleID].matrixID = packet.NewMatrixID;
            OwnedBelleMap[belleID].matrixIndex = newMatrixIndex;
        }


        if (OwnedMatrixMap.ContainsKey(newMatrixID) && OwnedMatrixMap[newMatrixID].isActive)
        {
            // 判断情缘变化
            OwnedMatrixMap[newMatrixID].UpdateAttrMap(true);
            if (OwnedMatrixMap[newMatrixID].m_bAffectionActive)
            {
                if (BackCamerControll.Instance() != null)
                {
                    BackCamerControll.Instance().PlaySceneEffect(195);
                    GameManager.gameManager.SoundManager.PlaySoundEffect(142);
                }
				GUIData.AddNotifyData2Client(false,"#{2771}");
            }

            // 判断五行相克
            Tab_Belle curTabBelle = TableManager.GetBelleByID(belleID, 0);
            if (null != newTabMatrix && newMatrixIndex < newTabMatrix.getNatureIndexCount() && OwnedBelleMap.ContainsKey(belleID) && null != curTabBelle)
            {
               
                int natureDiff = Mathf.Abs(curTabBelle.Nature - newTabMatrix.GetNatureIndexbyIndex(newMatrixIndex));
                if (natureDiff == 0 || natureDiff == 3)
                {
                    // 增益
                }
                else if(natureDiff == 1)
                {
                    // 减益
                }
                
            }
        }


        if (null != delBattle) delBattle();
    }


    // 美人休息消息
    public delegate void RestDataDelegate();
    public static RestDataDelegate delRest;
    public static void UpdateRestData(GC_BELLE_REST_RET packet)
    {
        int belleId = packet.BelleID;
        if (OwnedBelleMap.ContainsKey(belleId))
        {
            int matrixID = OwnedBelleMap[belleId].matrixID;
            OwnedBelleMap[belleId].matrixID = -1;
            OwnedBelleMap[belleId].matrixIndex = -1;
            Tab_BelleMatrix curTabMatrix = TableManager.GetBelleMatrixByID(matrixID, 0);
            if (OwnedMatrixMap.ContainsKey(matrixID) && packet.MatrixIndex < curTabMatrix.BelleCount)
            {
                OwnedMatrixMap[matrixID].belleIDs[packet.MatrixIndex] = -1;
                //OwnedMatrixMap[matrixID].isActive = false;
            }
        }

        if (null != delRest) delRest();
    }

    // 激活阵型消息
    public delegate void ActiveMatrixDelegate();
    public static ActiveMatrixDelegate delActiveMatrix;
    public static void UpdateActiveMatrixData(GC_BELLE_ACTIVEMATRIX_RET packet)
    {
        if (OwnedMatrixMap.ContainsKey(packet.MatrixID))
        {
            // 只有一个阵型可以激活
//            foreach (KeyValuePair<int, BelleMatrix> curPair in OwnedMatrixMap)
//            {
//                curPair.Value.isActive = false;
//            }

            OwnedMatrixMap[packet.MatrixID].isActive = true;
            OwnedMatrixMap[packet.MatrixID].UpdateAttrMap(true);
            if (OwnedMatrixMap[packet.MatrixID].m_bAffectionActive)
            {
                if (BackCamerControll.Instance() != null)
                {
                    BackCamerControll.Instance().PlaySceneEffect(195);
                    GameManager.gameManager.SoundManager.PlaySoundEffect(142);
                }
				GUIData.AddNotifyData2Client(false,"#{2771}");
            }

        }
        if (null != delActiveMatrix) delActiveMatrix();
    }

	// 取消激活阵型消息
	public delegate void UnActiveMatrixDelegate();
	public static UnActiveMatrixDelegate delUnActiveMatrix;
	public static void UpdateUnActiveMatrixData(GC_BELLE_CANCELMATRIX_RET packet)
	{
		if (OwnedMatrixMap.ContainsKey(packet.MatrixID))
		{
			
			OwnedMatrixMap[packet.MatrixID].isActive = false;
			OwnedMatrixMap[packet.MatrixID].UpdateAttrMap(true);
//			if (OwnedMatrixMap[packet.MatrixID].m_bAffectionActive)
//			{
//				if (BackCamerControll.Instance() != null)
//				{
//					BackCamerControll.Instance().PlaySceneEffect(195);
//					GameManager.gameManager.SoundManager.PlaySoundEffect(142);
//				}
//				GUIData.AddNotifyData("#{2771}");
//			}
			
		}
		if (null != delUnActiveMatrix) delUnActiveMatrix();
	}

    public static void AddBelle(int belleID)
    {
        if (OwnedBelleMap.ContainsKey(belleID))
        {
            return;
        }

        Belle curBelle = new Belle();
        curBelle.id = belleID;

        curBelle.level = 1;
        curBelle.matrixID = -1;
        curBelle.matrixIndex = -1;
        //curBelle.nextEvolutionTime = 0;
        curBelle.favourValue = 0;
        OwnedBelleMap.Add(curBelle.id, curBelle);
        m_belleActiveCount = PlayerPreferenceData.BelleActiveTipCount + 1;
        PlayerPreferenceData.BelleActiveTipCount = m_belleActiveCount;

        if (MenuBarLogic.Instance() != null)
        {
            MenuBarLogic.Instance().UpdateBelleTip();
        }
    }

    public static void CleanBelleTip()
    {
        PlayerPreferenceData.BelleActiveTipCount = 0;
        m_belleActiveCount = 0;
        if (MenuBarLogic.Instance() != null)
        {
            MenuBarLogic.Instance().UpdateBelleTip();
        }
    }

    public static string GetBelleBigTextureName(Tab_Belle curTabBelle)
    {
        if (null == curTabBelle)
        {
            return "";
        }
        int textureIndex = 0;
        if (BelleData.OwnedBelleMap.ContainsKey(curTabBelle.Id))
        {
            if (BelleData.OwnedBelleMap[curTabBelle.Id].colorLevel > 3)
            {
                textureIndex = BelleData.OwnedBelleMap[curTabBelle.Id].colorLevel - 3;
            }
        }

        if (textureIndex < 0 || textureIndex > 3)
        {
            LogModule.ErrorLog("textureIndex out of range");
        }

        return curTabBelle.GetBigTexNamebyIndex(textureIndex);
    }

    private static Color m_colorOrange = new Color(253f/255f, 123f/255f, 16f/255f);
    private static Color m_colorPurple = new Color(139, 0, 255);
    public static Color GetBelleColorByColorLevel(int colorLevel)
    {
        if (colorLevel == 5)
        {
            return m_colorOrange;
        }
        else if (colorLevel == 4)
        {
            return m_colorPurple;
        }
        else if (colorLevel == 3)
        {
            return Color.blue;
        }
        else if (colorLevel == 2)
        {
            return Color.green;
        }
        else
        {
            return Color.white;
        }
    }

    public static string GetBelleDescByColorLevel(int colorLevel)
    {
        if (colorLevel == 5)
        {
            return Utils.GetDicByID(3241);
        }
        else if (colorLevel == 4)
        {
            return Utils.GetDicByID(3240);
        }
        else if (colorLevel == 3)
        {
            return Utils.GetDicByID(3239);
        }
        else if (colorLevel == 2)
        {
            return Utils.GetDicByID(3238);
        }
        else
        {
            return Utils.GetDicByID(3237);
        }
    }

    // 根据属性字典获取战斗力数值
    public static int GetPowerNum(Dictionary<int, int> attrMap)
    {
        if (null == attrMap)
        {
            return 0;
        }

        float retValue = 0;
        foreach (KeyValuePair<int, int> curPair in attrMap)
        {
            switch (curPair.Key)
            {
                case 0:
                    //生命
                    retValue += curPair.Value * 0.2f;
                    break;
                case 3:
                case 4:
                case 1000:
                    // 攻击
                    retValue += curPair.Value * 5.1f;
                    break;
                case 5:
                case 6:
                case 1001:
                    // 防御
                    retValue += curPair.Value * 3f;
                    break;
                case 7:
                    retValue += curPair.Value * 3f;
                    // 命中
                    break;
                case 8:
                    retValue += curPair.Value * 4.2f;
                    // 命中
                    break;
                case 9:
                    retValue += curPair.Value;
                    // 暴击
                    break;
                case 10:
                    retValue += curPair.Value * 0.8f;
                    // 暴抗
                    break;
                case 11:
                    retValue += curPair.Value * 8.3f;
                    // 穿透
                    break;
                case 12:
                    retValue += curPair.Value * 8.3f;
                    // 韧性
                    break;
                case 13:
                    retValue += curPair.Value * 6.5f;
                    // 爆伤加成
                    break;
                case 14:
                    retValue += curPair.Value * 6.5f;
                    // 爆伤减免
                    break;
                case 5000:
                    retValue += curPair.Value * 6.5f;
                    // 爆伤减免
                    break;

            }
        }

        return (int)retValue;   
    }

    // 距离下次亲密还有多少秒
    public static int GetBelleCloseTimeDiff()
    {
        return BelleData.nextCloseTime - (int)Time.realtimeSinceStartup;
    }

    // 获取美人功能当前通知数字数量
    public static int GetBelleTipCount()
    {
        int retBelleCloseCount = 0;
        if (IsCanCloseFree()|| m_belleActiveCount > 0)
        {
            retBelleCloseCount = 1;
        }

        return retBelleCloseCount;
    }

    public static bool IsCanCloseFree()
    {
        return (GetBelleCloseTimeDiff() <= 0 && BelleData.dayCloseTime < 3 && BelleData.OwnedBelleMap.Count > 0);
    }

    public static int GetActiveMatrixPowerNum()
    {
        foreach (KeyValuePair<int, BelleMatrix> curPair in OwnedMatrixMap)
        {
            if (curPair.Value.isActive)
            {
                curPair.Value.UpdateAttrMap(false);
                return GetPowerNum(curPair.Value.attrMap);
            }
        }
        return 0;
    }

    public static void SetLabelNature(UILabel curLabel, int natureValue)
    {
        if (null == curLabel)
        {
            return;
        }
        string strNature = "";

        switch (natureValue)
        {
            case 1:
                curLabel.color = Color.yellow;
                strNature = "金";
                break;
            case 2:
                curLabel.color = Color.green;
                strNature = "木";
                break;
            case 3:
                curLabel.color = new Color(60.0f / 255.0f, 82.0f / 255.0f, 45.0f / 255.0f, 1);
                strNature = "土";
                break;
            case 4:
                curLabel.color = Color.blue;
                strNature = "水";
                break;
            case 5:
                curLabel.color = Color.red;
                strNature = "火";
                break;
        }

        curLabel.text = strNature;
    }
    
}
