using UnityEngine;
using System.Collections;

public enum BlackType
{
    ImmediatelyBlack,
    PingPong,
    EditCurve
}

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Image Effects/Black/BlackScreenCSharp")]

public class BlackScreenEffect : MonoBehaviour {

#region DataDeclaration

    public Shader _blackScreenShader;
    public float _TransTime = 1.0f;
    static Material s_blackMaterial = null;
    public bool _IsLoop = false;  
    public BlackType _BlackType = BlackType.ImmediatelyBlack;
    public AnimationCurve _AnimationCurve = new AnimationCurve(new Keyframe(0.0f, 0.0f, 0.0f, 1.0f), new Keyframe(1.0f, 1.0f, 1.0f, 1.0f));

    private float timeCounter ;
    private float timeCounterAnim;
    private bool isPingPongSecRange = false;
    private bool isEnd = false;
#endregion

#region DataCheck

    private BlackType BlackTypeFuc
    {
        get
        {
            return _BlackType;
        }
    }

    protected Material material
    {
        get
        {
            if (s_blackMaterial == null)
            {
                s_blackMaterial = new Material(_blackScreenShader);
                s_blackMaterial.hideFlags = HideFlags.DontSave;
            }
            return s_blackMaterial;
        }
    }

    private void InitData()
    {
        if (BlackTypeFuc == BlackType.PingPong)
        {
            timeCounter = _TransTime;
        }
        else if (BlackTypeFuc == BlackType.ImmediatelyBlack)
        {
            timeCounter = 0.0f;
        }
        else
        {
            timeCounter = 0.0f;
            timeCounterAnim = 0.0f;
        }
    }

#endregion

#region MainFunc
    //黑屏淡入淡出计算时间。
    void BlackScreenTimeEvaluate()
    {
        //当type设置为pingpong时，屏幕从亮到黑再到亮，过程总时间为2*_TransTime。
        if (BlackTypeFuc == BlackType.PingPong)
        {
            //从亮到暗
            if (!isPingPongSecRange)
            {
                timeCounter -= Time.deltaTime;
                if (timeCounter < 0.0f)
                {
                    isPingPongSecRange = true;
                }
            }
            //从暗到亮
            else
            {
                timeCounter += Time.deltaTime;
                if (timeCounter > _TransTime)
                {
                    isPingPongSecRange = false;
                    timeCounter = _TransTime;
                    if (!_IsLoop)
                    {
                        enabled = false;
                        //isEnd = true;
                    }

                }
            }
        }
        //ImmediatelyBlack，瞬间黑屏，经过_TransTime的时间后变亮。
        else if (BlackTypeFuc == BlackType.ImmediatelyBlack)
        {
            timeCounter += Time.deltaTime;
            if (timeCounter > _TransTime)
            {
                if (!_IsLoop)
                {
                    enabled = false;
                    isEnd = true;
                }
                else
                {
                    timeCounter = 0.0f;
                }
            }
        }
        //EditCurve，解决参数化问题，时间的曲线变化随AnimationCurve编辑的曲线变动。
        else
        {
            timeCounterAnim += Time.deltaTime;
            timeCounter = _AnimationCurve.Evaluate(timeCounterAnim / _TransTime) * _TransTime;
            if (timeCounterAnim > _TransTime)
            {
                if (!_IsLoop)
                {
                    enabled = false;
                    isEnd = true;
                }
                else
                {
                    timeCounterAnim = 0;
                    timeCounter = _AnimationCurve.Evaluate(0) * _TransTime;
                }
            }
        }

        material.SetFloat("_blackCount", timeCounter / _TransTime);

        //无loop的时候，若在上段代码把以下参数设置为0会导致闪屏问题。
        if (isEnd)
        {
            timeCounterAnim = 0.0f;
            timeCounter = 0.0f;
            isEnd = false;
        }

    }    
#endregion

 #region MonoBehaviourField 

    // Use this for initialization
    void Start()
    {
        // Disable if we don't support image effects
        if (!SystemInfo.supportsImageEffects)
        {
            enabled = false;
            return;
        }
        // Disable if the shader can't run on the users graphics card
        if (!_blackScreenShader || !material.shader.isSupported)
        {
            enabled = false;
            return;
        }

        InitData();
    }

    protected void OnDisable()
    {
        if (s_blackMaterial)
        {
            Destroy(s_blackMaterial);
        }
    }


    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        //source.filterMode = FilterMode.Bilinear;

        BlackScreenTimeEvaluate();

        int rtW = source.width;
        int rtH = source.height;
        //当前屏幕（满分辨率）作为buffer使用shader渲染后到目标屏幕，并释放当前buffer。
        RenderTexture buffer = RenderTexture.GetTemporary(rtW, rtH, 0);
        Graphics.Blit(source, buffer, material, 0);
        Graphics.Blit(buffer, destination);
        RenderTexture.ReleaseTemporary(buffer);
    }
 #endregion
  
}
