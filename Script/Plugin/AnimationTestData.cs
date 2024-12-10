/********************************************************************
	created:	2013/12/24
	created:	24:12:2013   14:12
	filename: 	AnimationTestData.cs
	file ext:	cs
	author:		王迪
	
	purpose:	AnimationTest类所需要暴露的数据
*********************************************************************/
using UnityEngine;
using System.Collections;

public class AnimationTestData : MonoBehaviour
{
    public string AnimationName;
    public GameObject[] OtherAnimtionObj;
    public string[] OtherAnimName;
    public float[] OtherAnimDelay;
    public Object[] EffectObject;
    public GameObject[] EffectBindPoint;
    public float[] effectDelay;
    public Vector3[] pos;
    public Vector3[] rotate;
}
