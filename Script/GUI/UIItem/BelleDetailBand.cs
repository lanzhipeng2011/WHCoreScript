/********************************************************************
	created:	2014/02/17
	created:	17:2:2014   9:51
	filename: 	BelleDetailBand.cs
	author:		王迪
	
	purpose:	美人详细信息版，根据模板ID设置显示内容
*********************************************************************/
using UnityEngine;
using System.Collections;
using GCGame.Table;
using Module.Log;
using GCGame;
using Games.GlobeDefine;
using System;
public class BelleDetailBand : MonoBehaviour {

    public UILabel labelName;
    public UILabel labelDesc;
    public UILabel labelGrade;
    public UILabel labelLevel;
    public UILabel labelCloseValue;
    public UILabel labelMatrixInfo;
    public UILabel labelNature;
    public UILabel labelNextAttrValue;
    public UILabel[] labelAtris; 
    public UILabel[] labelAffections;
    public UILabel[] labelAffectionDesc;
    public GameObject[] activeState;

    private Color m_ColorHighlight = new Color(255.0f / 255.0f, 244f/255f ,212f/255f, 1.0f);
    private Color m_ColorDisable = new Color(200f / 255.0f, 200f / 255.0f, 200f / 255.0f, 1.0f);
	public void SetBelle(int belleID)
    {
        Tab_Belle curTabBelle = TableManager.GetBelleByID(belleID, 0);
        Belle curBelleData = null;
        bool bOwned = false;
        if (BelleData.OwnedBelleMap.ContainsKey(belleID))
        {
            curBelleData = BelleData.OwnedBelleMap[belleID];
            bOwned = true;
        }
        if(null == curTabBelle)
        {
            LogModule.ErrorLog("can not read belle from table id: " + belleID.ToString());
            return;
        }

        if (null != labelName)
        {
            labelName.text = curTabBelle.Name;
            if (null != curBelleData)
            {
                labelName.color = BelleData.GetBelleColorByColorLevel(curBelleData.colorLevel);
			}else{
				//=======如果未获得则使用白色文本
				labelName.color = BelleData.GetBelleColorByColorLevel(0);
			}
            
        }
        if(null != labelDesc) labelDesc.text = curTabBelle.Introduction;
        BelleData.SetLabelNature(labelNature, curTabBelle.Nature);
        
        if (null != labelGrade)
        {
            if (null != curBelleData)
            {
                curBelleData.UpdateAttrMap();
                labelGrade.text = Utils.GetDicByID(1595) + ":" + BelleData.GetPowerNum(curBelleData.attrMap).ToString();
            }
            else
            {
                labelGrade.text = Utils.GetDicByID(1595) + ":0";
            }
            
        }

        
        labelLevel.gameObject.SetActive(bOwned);
        labelCloseValue.gameObject.SetActive(bOwned);
        labelMatrixInfo.gameObject.SetActive(bOwned);
        
        if (null != curBelleData)
        {
            labelLevel.text = StrDictionary.GetClientDictionaryString("#{1308}", BelleData.GetBelleDescByColorLevel(curBelleData.colorLevel), curBelleData.subLevel);
            labelLevel.color = BelleData.GetBelleColorByColorLevel(curBelleData.colorLevel);
            if (null != labelCloseValue) labelCloseValue.text = StrDictionary.GetClientDictionaryString("#{1307}",curBelleData.favourValue);
            if (null != labelMatrixInfo)
            {
                Tab_BelleMatrix curMatrix = TableManager.GetBelleMatrixByID(curBelleData.matrixID, 0);
                if (null != curMatrix)
                {
                    labelMatrixInfo.text = StrDictionary.GetClientDictionaryString("#{1309}") + curMatrix.Name;
                }
                else
                {
                    labelMatrixInfo.text = StrDictionary.GetClientDictionaryString("#{1309}") + StrDictionary.GetClientDictionaryString("#{1310}");
                }
            }
        }

        Tab_BelleAddition tabAdditonInfo = null;
        for (int curBelleAdditionIndex = 0; curBelleAdditionIndex < TableManager.GetBelleAddition().Count; curBelleAdditionIndex++)
        {
            Tab_BelleAddition curBelleAddition = TableManager.GetBelleAdditionByID(curBelleAdditionIndex, 0);
            if (null != curBelleAddition)
            {
                if (curBelleAddition.BelleCount == BelleData.OwnedBelleMap.Count && curBelleAddition.BelleID == belleID)
                {
                    tabAdditonInfo = curBelleAddition;
                    break;
                }
            }
        }

        labelNextAttrValue.text = "";
        if(null != labelAtris)
        {
            for (int i = 0; i < labelAtris.Length; i++)
            {
                if (i < curTabBelle.getAttrTypeCount())
                {
                    // 属性X增加N
                    int curValueAdd = curTabBelle.GetAttrValuebyIndex(i);
                    int nextValueAdd = 0;
                    string strNextValueAdd = "";
                    string strAddValue = "";
                    if (null != curBelleData && bOwned)
                    {
                        if (i == curBelleData.colorLevel - 1)
                        {
                            curValueAdd = (int)(curValueAdd * curBelleData.subLevel);
                            if (curBelleData.subLevel < 9)
                            {
                                nextValueAdd = curTabBelle.GetAttrValuebyIndex(i);
                                labelNextAttrValue.text = StrDictionary.GetClientDictionaryString("#{3084}", nextValueAdd, Utils.GetAttrTypeString(curTabBelle.GetAttrTypebyIndex(i)));
                            }
                        }
                        else
                        {
                            curValueAdd = (int)(curValueAdd * 9);
                        }

                        if (i < curBelleData.colorLevel && null != tabAdditonInfo && i < tabAdditonInfo.getAdditionValueCount())
                        {
                            strAddValue = " +" + tabAdditonInfo.GetAdditionValuebyIndex(i).ToString();
                        }
                        
                    }
					//==========
					if(i == 0)
					{
						labelAtris[i].text = Utils.GetDicByID(1592) + (i + 1).ToString() + " : " + Utils.GetAttrTypeString(curTabBelle.GetAttrTypebyIndex(i)) + Utils.GetDicByID(1593) + curValueAdd.ToString() + strNextValueAdd + strAddValue;
					}else{
						labelAtris[i].text = Utils.GetDicByID(1592) + (i + 1).ToString() + ": " + Utils.GetAttrTypeString(curTabBelle.GetAttrTypebyIndex(i)) + Utils.GetDicByID(1593) + curValueAdd.ToString() + strNextValueAdd + strAddValue;
					}

                   
                    if (nextValueAdd > 0)
                    {
                        Vector3 curAttrPos = labelAtris[i].transform.localPosition;
                        labelNextAttrValue.transform.localPosition = new UnityEngine.Vector3(curAttrPos.x + labelAtris[i].printedSize.x + 10, curAttrPos.y, curAttrPos.z);
                    }
                }

                if (null != curBelleData && bOwned)
                {
                    labelAtris[i].color = i < curBelleData.colorLevel ? m_ColorHighlight : m_ColorDisable;
                }
                else
                {
                    labelAtris[i].color = m_ColorDisable;
                }

            }
        }

        if(null != activeState)
        {
            for(int i=0; i<activeState.Length; i++)
            {
                if (null != curBelleData && bOwned)
                {
                    activeState[i].SetActive(i >= curBelleData.colorLevel);
                }
                else
                {
                    activeState[i].SetActive(false);
                }
               
                
            }
        }

        if(null != labelAffections)
        {
            for (int i = 0; i < labelAffections.Length; i++)
            {
                if (i < curTabBelle.getAffectionGainsTypeCount())
                {
                    // 情缘N
					//===========
					if(i == 0)
					{
						labelAffections[i].text = Utils.GetDicByID(1594) + (i + 1).ToString() + " : " + curTabBelle.GetAffectionDescbyIndex(i);
					}else{
						labelAffections[i].text = Utils.GetDicByID(1594) + (i + 1).ToString() + ": " + curTabBelle.GetAffectionDescbyIndex(i);
					}
                   
                    labelAffectionDesc[i].text = "";// ": " + curTabBelle.GetAffectionDescbyIndex(i);
                    labelAffections[i].color = m_ColorDisable;
                    labelAffectionDesc[i].color = m_ColorDisable;
                    if (null != curBelleData && bOwned)
                    {
                        int curBelleMatrixID = curBelleData.matrixID;
                        if (curBelleMatrixID < 0)
                        {
                            continue;
                        }

                        if (!BelleData.OwnedMatrixMap.ContainsKey(curBelleMatrixID))
                        {
                            continue;
                        }

                        if (!BelleData.OwnedMatrixMap[curBelleMatrixID].isActive)
                        {
                            continue;
                        }

                        bool bActive = false;
                        int type = curTabBelle.GetAffectionConditonTypebyIndex(i);
                        switch (type)
                        {
                            case 1:
                                {
                                    // 同上阵
                                    int targetBelleID = curTabBelle.GetAffectionConditonValuebyIndex(i);
                                    if (BelleData.OwnedBelleMap.ContainsKey(targetBelleID))
                                    {
                                        if (BelleData.OwnedBelleMap[targetBelleID].matrixID == curBelleData.matrixID)
                                        {
                                            bActive = true;
                                        }
                                    }
                                }
                                break;
                            case 2:
                                {
                                    int targetLevel = curTabBelle.GetAffectionConditonValuebyIndex(i);
                                    if (curBelleData.colorLevel > targetLevel || (curBelleData.colorLevel == targetLevel && curBelleData.subLevel == 9))
                                    {
                                        bActive = true;
                                    }
                                }
                                break;
                        }
                        labelAffections[i].color = bActive ? m_ColorHighlight : m_ColorDisable;
                        labelAffectionDesc[i].color = bActive ? m_ColorHighlight : m_ColorDisable;
                    }
                }
            }
        }
    }

}
