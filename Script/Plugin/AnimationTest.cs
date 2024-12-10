/********************************************************************
	created:	2013/12/24
	created:	24:12:2013   14:11
	filename    AnimationTest.cs
	author:		王迪
	
	purpose:	动作测试脚本，给美术调动作用
*********************************************************************/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("MLDJ/Plugin/AnimationTest")]
[RequireComponent(typeof(AnimationTestData))]
public class AnimationTest : MonoBehaviour
{

    private List<GameObject> curEffectPoints;
    private List<Object> curEffectObjs;
    private List<float> delays;
    private List<Vector3> curPos;
    private List<Vector3> curRot;

    private List<float> animDelays;
    private List<GameObject> animObjs;
    private List<string> animNames;
    // Use this for initialization
    //void Start()
    //{

    //}

    // Update is called once per frame
    void Update()
    {
        Time.timeScale = 1.0f;
        if (delays != null)
        {
            for (int i = 0; i < delays.Count; i++)
            {
                if (delays[i] > 0)
                {
                    delays[i] -= Time.deltaTime;
                    if (delays[i] <= 0)
                    {
                        DoPlay(i);
                    }
                }
            }
        }

        if (animDelays != null)
        {
            for (int i = 0; i < animDelays.Count; i++)
            {
                if (animDelays[i] > 0)
                {
                    animDelays[i] -= Time.deltaTime;
                    if (animDelays[i] <= 0)
                    {
                        PlayOtherAnim(i);
                    }
                }
            }
        }
    }

	public void Play()
    {
        AnimationTestData data = gameObject.GetComponent<AnimationTestData>();
        if (null == data)
            return;

        Animation animation = gameObject.animation;
        if (null != animation && !string.IsNullOrEmpty(data.AnimationName))
        {
            animation.CrossFade(data.AnimationName);
        }


        curEffectPoints = new List<GameObject>();
        curEffectObjs = new List<Object>();
        delays = new List<float>();
        curPos = new List<Vector3>();
        curRot = new List<Vector3>();


        for (int i = 0; i < data.EffectObject.Length; i++)
        {
            curEffectPoints.Add(data.EffectBindPoint[i]);
            curEffectObjs.Add(data.EffectObject[i]);
            delays.Add(data.effectDelay[i]);
            curPos.Add(data.pos[i]);
            curRot.Add(data.rotate[i]);

            if (data.effectDelay[i] == 0)
            {
                DoPlay(i);
            }
        }

        animDelays = new List<float>();
        animObjs = new List<GameObject>();
        animNames = new List<string>();

        for (int i = 0; i < data.OtherAnimtionObj.Length; i++)
        {
            animObjs.Add(data.OtherAnimtionObj[i]);
            animDelays.Add(data.OtherAnimDelay[i]);
            animNames.Add(data.OtherAnimName[i]);

            if (data.OtherAnimDelay[i] == 0)
            {
                PlayOtherAnim(i);
            }
        }

    }

	void DoPlay(int index)
    {
        if (index < 0 || index >= curEffectObjs.Count)
            return;

        GameObject newEffect = GameObject.Instantiate(curEffectObjs[index]) as GameObject;
        if (null != newEffect)
        {
            if (index < curEffectPoints.Count)
                newEffect.transform.parent = curEffectPoints[index].transform;
            if (index < curPos.Count)
                newEffect.transform.localPosition = curPos[index];
            if (index < curRot.Count)
                newEffect.transform.localRotation = Quaternion.Euler(curRot[index]);
        }
    }

	void PlayOtherAnim(int index)
    {
        if (animObjs.Count <= index || index < 0)
        {
            return;
        }

        if (animObjs[index] == null || animObjs[index].animation == null)
        {
            return;
        }

        if (null != animObjs[index].animation && index < animNames[index].Length)
            animObjs[index].animation.Play(animNames[index]);
    }
}
