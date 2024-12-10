using UnityEngine;
using System.Collections;
using GCGame.Table;
using Module.Log;

public class BelleDetailWindow : MonoBehaviour {

   
    public BelleDetailBand belleDetailBand;
    public BelleTickBand belleTickBand;
    public GameObject btnBattle;
    public GameObject btnRest;
    public UITexture texCard;
    private int m_belleID;

	public GameObject m_DetailBand;
	public UISprite m_SprPower;

    public void ShowBelle(int id)
    {
        Tab_Belle curBelle = TableManager.GetBelleByID(id, 0);
        if (null == curBelle)
        {
            LogModule.ErrorLog("can not find cur belle id:" + id);
            return;
        }

        SetData(id, curBelle);
    }

  
    void OnEnable()
    {
        BelleData.delRest += Ret_Rest;
        BelleData.delEvolutionRapid += OnEvolution;

		StartCoroutine (SetActiveSecond (0.01f));
    }
	//======

	IEnumerator SetActiveSecond(float numbers) 
	{
		m_DetailBand.SetActive (false);
		yield return new WaitForSeconds(numbers);
		m_DetailBand.SetActive (true);
	}
	//========

    void OnDisable()
    {
        BelleData.delRest -= Ret_Rest;
        BelleData.delEvolutionRapid -= OnEvolution;
    }

    void SetData(int belleID, Tab_Belle tabBelle)
    {
        texCard.mainTexture = ResourceManager.LoadResource(BelleData.GetBelleBigTextureName(tabBelle), typeof(Texture)) as Texture;
        belleDetailBand.SetBelle(belleID);

		Tab_BelleCamp tbC = TableManager.GetBelleCampByID (tabBelle.Camp,0);
		m_SprPower.spriteName = tbC.IconName;

        btnBattle.SetActive(false);
        btnRest.SetActive(false);
        if (BelleData.OwnedBelleMap.ContainsKey(belleID))
        {
            belleTickBand.ShowBelleState(belleID);
            belleTickBand.gameObject.SetActive(true);
            if (BelleData.OwnedBelleMap[belleID].matrixID >= 0)
            {
                btnRest.SetActive(true);
            }
            else
            {
                btnBattle.SetActive(true);
            }
        }
        else
        {
            belleTickBand.gameObject.SetActive(false);
        }

        m_belleID = belleID;
    }

    void OnEvolution()
    {
        ShowBelle(m_belleID);
    }

   

    void OnBattleClick()
    {
        if (null != BelleController.Instance())
        {
            BelleController.Instance().ChangeMatrixWindow();
            //BelleController.Instance().SelectMatrix(m_belleID, BelleMatrixWindow.SelectFromType.TYPE_BELLEDETAIL);
        }
    }

    void OnRestClick()
    {
        CG_BELLE_REST restRequest = (CG_BELLE_REST)PacketDistributed.CreatePacket(MessageID.PACKET_CG_BELLE_REST);
        restRequest.SetBelleID(m_belleID);
        restRequest.SendPacket();
    }

    void Ret_Rest()
    {
        if (BelleData.OwnedBelleMap.ContainsKey(m_belleID))
        {
            if (BelleData.OwnedBelleMap[m_belleID].matrixID >= 0)
            {
                btnRest.SetActive(true);
                btnBattle.SetActive(false);
            }
            else
            {
                btnBattle.SetActive(true);
                btnRest.SetActive(false);
            }
        }
       
    }
}
