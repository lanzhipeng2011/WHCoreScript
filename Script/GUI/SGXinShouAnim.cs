using UnityEngine;
using System.Collections;

public class SGXinShouAnim : MonoBehaviour 
{
    public static SGXinShouAnim Instance;

    public UITexture text1;
    public UITexture text2;
    public UITexture text3;

    public GameObject TextRoot;
    public UISprite blackMask;

    
    void Awake()
    {
        Instance = this;
    }

    void Start() 
    {
        text1.gameObject.SetActive(false);
        text2.gameObject.SetActive(false);
        text3.gameObject.SetActive(false);
        blackMask.color = new Color(1,1,1,1);
        TweenAlpha.Begin(blackMask.gameObject, 2f, 0);
        StartCoroutine(BlackShowEnd());
    }

    //自适应宽度 这里原始比例为16:9
    void AutoScale() 
    {
        float wsub = (float)Screen.width / 16f;
    }
    IEnumerator BlackShowEnd()
    {
        yield return new WaitForSeconds(1.8f);

        text1.gameObject.SetActive(true);
        text1.color = new Color(1, 1, 1, 0);
        text1.transform.localScale = new Vector3(2, 1, 1);
        TweenAlpha.Begin(text1.gameObject, 1.7f, 1f);
        iTween.ScaleTo(text1.gameObject, iTween.Hash("scale", Vector3.one, "time", 1.7f, "oncomplete", "text1EndShow", "oncompletetarget", gameObject, "easetype", iTween.EaseType.linear));
    }

    void text1EndShow() 
    {
        StartCoroutine(_text1EndShow());
    }

    IEnumerator _text1EndShow() 
    {
        yield return new WaitForSeconds(0.7f);
        text2.gameObject.SetActive(true);
        text2.color = new Color(1, 1, 1, 0);
        text2.transform.localScale = new Vector3(2, 1, 1);
        TweenAlpha.Begin(text2.gameObject, 1.7f, 1f);
        iTween.ScaleTo(text2.gameObject, iTween.Hash("scale", Vector3.one, "time", 1.7f, "oncomplete", "text2EndShow", "oncompletetarget", gameObject, "easetype", iTween.EaseType.linear));
    } 
    void text2EndShow()
    {
        StartCoroutine(_text2EndShow());
    }

    IEnumerator _text2EndShow()
    {
        yield return new WaitForSeconds(0.7f);
        text3.gameObject.SetActive(true);
        text3.color = new Color(1, 1, 1, 0);
        text3.transform.localScale = new Vector3(2, 1, 1);
        TweenAlpha.Begin(text3.gameObject, 1.7f, 1f);
        iTween.ScaleTo(text3.gameObject, iTween.Hash("scale", Vector3.one, "time", 1.7f, "easetype", iTween.EaseType.linear));
    }

}
