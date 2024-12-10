using UnityEngine;
using System.Collections;
using Module.Log;

public class BonusItemGetOpenBox : UIControllerBase<BonusItemGetOpenBox>
{
    void Awake()
    {
        SetInstance(this);
    }
    //// Use this for initialization
    //void Start () {
	
    //}
    void ShowOpenBoxAnimation()
    {
        if (BonusItemGetOpenBox.Instance() != null)
        {
            if (BonusItemGetLogic.Instance() != null)
            {
				BonusItemGetLogic.Instance().ShowOpenBoxAnimation();               
            }
        }
    }
}
