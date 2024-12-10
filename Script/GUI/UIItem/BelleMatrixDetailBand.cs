/********************************************************************
	created:	2014/02/17
	created:	17:2:2014   9:52
	filename: 	BelleMatrixDetailBand.cs
	author:		王迪
	
	purpose:	美人阵法界面，根据阵法ID，显示相应信息
*********************************************************************/
using UnityEngine;
using System.Collections;
using GCGame.Table;
using GCGame;

public class BelleMatrixDetailBand : MonoBehaviour {

    public UILabel labelName;
    public UILabel labelMatrixGrade;
    public UILabel labelBelleGrade;
    public UILabel labelAffectionValue;
    public UILabel labelBelleCountGrade;
    public UILabel[] labelEffects;
    public UILabel[] labelAttrs;
    public UILabel[] labelAttrValues;

	public void SetMatrix(int matrixID)
    {
        if (!BelleData.OwnedMatrixMap.ContainsKey(matrixID))
        {
            return;
        }
        Tab_BelleMatrix curMatrix = TableManager.GetBelleMatrixByID(matrixID, 0);
        if (null == curMatrix)
        {
            return;
        }

        if (null != labelName) labelName.text = curMatrix.Name;

        BelleMatrix curMatrixData = BelleData.OwnedMatrixMap[matrixID];

        int minLevel = 10000;

        curMatrixData.UpdateAttrMap();
		labelMatrixGrade.text = BelleData.GetPowerNum(curMatrixData.attrMap).ToString();//Utils.GetDicByID(1595) + ":" + 
		labelBelleGrade.text = BelleData.GetPowerNum(curMatrixData.attrBelleMap).ToString();//Utils.GetDicByID(1595) + ":" + 
		labelAffectionValue.text = BelleData.GetPowerNum(curMatrixData.attrAffectionMap).ToString();//Utils.GetDicByID(1595) + ":" + 
        int belleCountAddValue = 0;
        if (BelleData.OwnedBelleMap.Count > 0)
        {
            belleCountAddValue = (BelleData.OwnedBelleMap.Count - 1) * 5;
        }
        labelBelleCountGrade.text = belleCountAddValue + "%";
        for (int i = 0; i < curMatrixData.belleIDs.Length; ++i)
        {
            if (BelleData.OwnedBelleMap.ContainsKey(curMatrixData.belleIDs[i]))
            {
                int curLevel = BelleData.OwnedBelleMap[curMatrixData.belleIDs[i]].colorLevel;
                if (curLevel < minLevel)
                {
                    minLevel = curLevel;
                }
            }
            else
            {
                minLevel = 0;
            }
        }

        for (int i = 0; i < labelEffects.Length; i++)
        {
            if (labelAttrValues.Length > i && labelAttrs.Length > i)
            {
                if ((i + 1) == minLevel)
                {
                    if (BelleData.OwnedMatrixMap[matrixID].isActive)
                    {
                        labelEffects[i].color = labelAttrs[i].color = labelAttrValues[i].color = Utils.GetColorByString("fff100");
                    }
                    else
                    {
                        labelEffects[i].color = labelAttrs[i].color = labelAttrValues[i].color = Color.black;
                    }
                }
                else if ((i + 1) < minLevel)
                {
                    labelEffects[i].color = labelAttrs[i].color = labelAttrValues[i].color = Color.black;
                }
                else
                {
                    labelEffects[i].color = labelAttrs[i].color = labelAttrValues[i].color = Color.gray;
                }

                labelAttrValues[i].text = curMatrix.GetAttrGainsbyIndex(i).ToString();
                labelAttrs[i].text = Utils.GetAttrTypeString(curMatrix.AttrType);
            }
        }


    }
}
