using UnityEngine;
using System.Collections;

public class UISkipStory : MonoBehaviour {

    public void OnClick_SkipStory(GameObject go) 
    {
        GameTrigger.Instance.CallEvent("ClickSkipStory");
    }
}
