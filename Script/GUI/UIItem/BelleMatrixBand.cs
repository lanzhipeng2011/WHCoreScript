using UnityEngine;
using System.Collections;
using Module.Log;
using GCGame.Table;

public class BelleMatrixBand : MonoBehaviour {

    public BelleMatrixBandItem[]    m_BelleBandBtns;

    public bool SetMatrixInfo(int matrixID)
    {
        
        BelleMatrix curMatrix = BelleData.OwnedMatrixMap[matrixID];
        Tab_BelleMatrix curBelleMatrix = TableManager.GetBelleMatrixByID(curMatrix.id, 0);
        if (null == curBelleMatrix)
        {
            LogModule.ErrorLog("BelleMatrix not defined");
            return false;
        }
        for (int i = 0; i < m_BelleBandBtns.Length; ++i)
        {
            int curNatrue = 0;
            if (i < curBelleMatrix.getNatureIndexCount())
            {
                curNatrue = curBelleMatrix.GetNatureIndexbyIndex(i);
            }
            m_BelleBandBtns[i].SetIconName(null, curNatrue);
        }

        if (!BelleData.OwnedMatrixMap.ContainsKey(matrixID))
        {
            return false;
        }

        for (int i = 0; i < curMatrix.belleIDs.Length; ++i)
        {
            if (BelleData.OwnedBelleMap.ContainsKey(curMatrix.belleIDs[i]))
            {
                Belle curBelle = BelleData.OwnedBelleMap[curMatrix.belleIDs[i]];
                if (curBelle.matrixID != matrixID)
                {
                    LogModule.WarningLog("matrix ID data error: matrixID :" + matrixID.ToString() + "belleMatrixID: " + curBelle.matrixID.ToString());
                    continue;
                }

                int curMatrixIndex = curBelle.matrixIndex;
                if (curMatrixIndex >= m_BelleBandBtns.Length)
                {
                    LogModule.WarningLog("curMatrixIndex is big than button array " + curMatrixIndex.ToString());
                    continue;
                }

                Tab_Belle curTabBelle = TableManager.GetBelleByID(curMatrix.belleIDs[i], 0);
                if (null == curTabBelle)
                {
                    continue;
                }
//                m_BelleBandBtns[curMatrixIndex].SetIconName(curTabBelle.IconTexName, 0);
				m_BelleBandBtns[curBelle.matrixIndex].SetIconName(curTabBelle.IconTexName, 0);
            }
        }

        return true;
    }
}
