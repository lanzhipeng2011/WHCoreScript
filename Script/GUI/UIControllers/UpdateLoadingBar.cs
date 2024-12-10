using UnityEngine;
using System.Collections;

public class UpdateLoadingBar : MonoBehaviour {

    public UILabel LabelState;
    public ClipSlider SliderLoading;
	// Use this for initialization


    void Start() 
    {
     
    }
	public void SetStateString(string strState)
    {
        LabelState.text = strState;
    }

    public void SetLoadingPrecent(float percent)
    {
        SliderLoading.SetNextProgress(percent);
    }


}
