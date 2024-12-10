using UnityEngine;
using System.Collections;

public class SGOneMonthAgo : MonoBehaviour {

    public UISprite blackBG;
    public UITexture TextPic;
	// Use this for initialization
	void Start ()
    {
        TweenAlpha.Begin(blackBG.gameObject, 1f, 1);
        StartCoroutine(_ShowBGEnd());
	}

    IEnumerator _ShowBGEnd() 
    {
        yield return new WaitForSeconds(1f);
        TweenAlpha.Begin(TextPic.gameObject, 1f, 1);
    }
}
