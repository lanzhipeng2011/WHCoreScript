using UnityEngine;
using System.Collections;

using GCGame;
using Module.Log;
using Games.ChatHistory;

public class GMPanelObjectManager : MonoBehaviour {

	public GameObject m_GMPanelObject;

	public UILabel expLabel;
	public UILabel lvlLabel;
	public UILabel hpLabel;
	public UILabel mpLabel;
	public UILabel goldLabel;
	public UILabel goodsIdLabel;
	public UILabel goodsNumLabel;
	public UILabel VipNumLabel;

	public UILabel ShimenxinhuoLabel;
	public UILabel GerenxinhuoLabel;
	public UILabel BanghuicaifuLabel;

	// Use this for initialization
	void Start () {
		m_GMPanelObject.SetActive (false);
		initEvnet ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	private void initEvnet()
	{
		EventManager.instance.addEventListener ("ShowGMPanel", this.gameObject, "showGMPanel");
	}
	private void showGMPanel()
	{
		m_GMPanelObject.SetActive (true);
	}
	private void OnCloseGMBtn()
	{
		m_GMPanelObject.SetActive (false);
		//EventManager.instance.dispatchEvent(new Hashtable(),"ChangeIsOpenGM");
	}

	private string text;
	private void OnAddExp()
	{
		if(expLabel.text == "You can type here"|| expLabel.text =="")
			return;
		
		text = "#exp "+ expLabel.text;
		//text = Utils.StrFilter_Chat(text);
		ChatHistoryItem item = new ChatHistoryItem();
		item.CleanUp();
		Utils.SendCGChatPak(text, item);
	}
	private void OnAddLvl()
	{
		if(lvlLabel.text == "You can type here" || lvlLabel.text =="")
			return;
		
		text = "#lvl "+ lvlLabel.text;
		ChatHistoryItem item = new ChatHistoryItem();
		item.CleanUp();
		Utils.SendCGChatPak(text, item);
	}
	private void OnAddHp()
	{
		if(hpLabel.text == "You can type here"|| hpLabel.text =="")
			return;
		
		text = "#hp "+ hpLabel.text;
		ChatHistoryItem item = new ChatHistoryItem();
		item.CleanUp();
		Utils.SendCGChatPak(text, item);
	}
	private void OnAddMp()
	{
		if(mpLabel.text == "You can type here"|| mpLabel.text =="")
			return;
		
		text = "#mp "+ mpLabel.text;
		ChatHistoryItem item = new ChatHistoryItem();
		item.CleanUp();
		Utils.SendCGChatPak(text, item);
	}
	private void OnAddYuanB()
	{
		if(goldLabel.text == "You can type here"|| goldLabel.text =="")
			return;
		
		text = "#gold "+ goldLabel.text;
		ChatHistoryItem item = new ChatHistoryItem();
		item.CleanUp();
		Utils.SendCGChatPak(text, item);
	}
	private void OnAddBindYuanB()
	{
		if(goldLabel.text == "You can type here"|| goldLabel.text =="")
			return;
		
		text = "#gold_bind "+ goldLabel.text;
		ChatHistoryItem item = new ChatHistoryItem();
		item.CleanUp();
		Utils.SendCGChatPak(text, item);
	}
	private void OnAddSilver()
	{
		if(goldLabel.text == "You can type here"|| goldLabel.text =="")
			return;
		
		text = "#silver "+ goldLabel.text;
		ChatHistoryItem item = new ChatHistoryItem();
		item.CleanUp();
		Utils.SendCGChatPak(text, item);
	}
	private void OnAddGoods()
	{
		
		text = "#goods "+ goodsIdLabel.text + " "+goodsNumLabel.text;
		ChatHistoryItem item = new ChatHistoryItem();
		item.CleanUp();
		Utils.SendCGChatPak(text, item);
	}

	private void OnAddVip()
	{
		if(VipNumLabel.text == "You can type here"|| VipNumLabel.text =="")
			return;
		
		text = "#vip "+ VipNumLabel.text;
		ChatHistoryItem item = new ChatHistoryItem();
		item.CleanUp();
		Utils.SendCGChatPak(text, item);
	}

	private void OnAddShiMen()
	{
		text = "#mtorch "+ ShimenxinhuoLabel.text;
		ChatHistoryItem item = new ChatHistoryItem();
		item.CleanUp();
		Utils.SendCGChatPak(text, item);
	}

	private void OnAddGeRen()
	{
		text = "#ptorch "+ GerenxinhuoLabel.text;
		ChatHistoryItem item = new ChatHistoryItem();
		item.CleanUp();
		Utils.SendCGChatPak(text, item);
	}

	private void OnAddBanghuiCaiFu()
	{
		text = "#wealth "+ BanghuicaifuLabel.text;
		ChatHistoryItem item = new ChatHistoryItem();
		item.CleanUp();
		Utils.SendCGChatPak(text, item);
	}

}
