using UnityEngine;
using System.Collections;
using GCGame;

public class LevelUpController : UIControllerBase<LevelUpController> {

    public UILabel LabelTips;
    public TweenAlpha curTween;

    void Awake()
    {
        SetInstance(this);
    }
	// Use this for initialization
	void Start () {
	
	}

    public static void ShowTipByID(int dicID)
    {
        UIManager.ShowUI(UIInfo.LevelupTip, LevelUpController.OnShowTip, dicID);
        
    }

    static void OnShowTip(bool bSuccess, object dicID)
    {
        LevelUpController.Instance().SetLabelTextByID((int)dicID);
    }
    public void SetLabelText(string text)
    {
        LabelTips.text = text;
        curTween.Reset();
    }

    public void SetLabelTextByID(int dicID)
    {
        LabelTips.text = Utils.GetDicByID(dicID);
        curTween.Reset();
    }

    public void OnTweenFinish()
    {
        UIManager.CloseUI(UIInfo.LevelupTip);
    }
}
