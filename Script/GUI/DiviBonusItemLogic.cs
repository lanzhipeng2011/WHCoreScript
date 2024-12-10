using UnityEngine;
using System.Collections;
using Module.Log;
using GCGame;

public class DiviBonusItemLogic : MonoBehaviour {

	public UISprite m_ItemIcon;
	public UILabel m_ItemName;
	public UISprite m_ItemImpact;
	public UISprite m_ItemQuality;
	private TweenPosition m_ItemObjTweenPosition;
	private TweenRotation m_ItemObjTweenRotation;
	private TweenScale m_ItemObjTweenScale;

	private bool m_ImpactShow = false;

	void Awake()
	{
		m_ItemObjTweenPosition = this.gameObject.GetComponent<TweenPosition> ();
		m_ItemObjTweenRotation = this.gameObject.GetComponent<TweenRotation> ();
		m_ItemObjTweenScale = this.gameObject.GetComponent<TweenScale> ();
	}

	// Use this for initialization
	void Start () {
	
	}

	public void InitItem(string ImageName,string ItemName,string ItemQuality,bool ShowImpact)
	{
		if ("" == ImageName || "" == ItemName)
		{
			return;
		}

		m_ItemIcon.spriteName = ImageName;
		m_ItemName.text = ItemName;
		m_ItemQuality.spriteName = ItemQuality;
		m_ImpactShow = ShowImpact;
		m_ItemName.gameObject.SetActive (false);
		this.gameObject.SetActive (false);
	}

	//显示物品图标动画
	public void ShowItemObjTween(Vector3 InitPos, Vector3 ToPos, Vector3 InitRotation, Vector3 ToRotation, Vector3 InitScale, Vector3 ToScale, float TweenTime)
	{
		this.gameObject.SetActive(true);

		if (null != m_ItemObjTweenPosition)
		{
			EventDelegate.Add(m_ItemObjTweenPosition.onFinished, OnTweenPositionFinished);
			m_ItemObjTweenPosition.from = InitPos;
			m_ItemObjTweenPosition.to = ToPos;
			m_ItemObjTweenPosition.duration = TweenTime;
			m_ItemObjTweenPosition.Reset();
			m_ItemObjTweenPosition.Play(true);
		}

		if (null != m_ItemObjTweenRotation)
		{
			m_ItemObjTweenRotation.from = InitRotation;
			m_ItemObjTweenRotation.to = ToRotation;
			m_ItemObjTweenRotation.duration = TweenTime;
			m_ItemObjTweenRotation.Reset();
			m_ItemObjTweenRotation.Play(true);
		}

		if (null != m_ItemObjTweenScale)
		{
			m_ItemObjTweenScale.from = InitScale;
			m_ItemObjTweenScale.to = ToScale;
			m_ItemObjTweenScale.duration = TweenTime;
			m_ItemObjTweenScale.Reset();
			m_ItemObjTweenScale.Play(true);
		}
	}

	void OnTweenPositionFinished()
	{
		m_ItemName.gameObject.SetActive (true);
//		if (m_ImpactShow)
//		{
//			m_ItemImpact.gameObject.SetActive(m_ImpactShow);
//			GameManager.gameManager.SoundManager.PlaySoundEffect(133);
//		}
	}
}
