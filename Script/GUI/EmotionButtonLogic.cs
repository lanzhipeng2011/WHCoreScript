using UnityEngine;
using System.Collections;

public class EmotionButtonLogic : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

    void InsertEmotion()
    {
        // 如果小喇叭窗口正打开 说明是小喇叭的表情窗口 插入表情代码后直接return
        if (LoudSpeakerLogic.Instance() != null && LoudSpeakerLogic.Instance().IsLoudSpeakerWindowShow())
        {
            LoudSpeakerLogic.Instance().InsertEmotion(gameObject);
            return;
        }
        if (ChatInfoLogic.Instance() != null)
        {
            ChatInfoLogic.Instance().InsertEmotion(gameObject);
        }
    }
}
