// ----------------------------------------------------------------------------------
//
//
// ----------------------------------------------------------------------------------

using System;
using UnityEngine;
using System.Collections;

public class NcDrawFpsRect : MonoBehaviour
{
	public	bool		centerTop		= true;
	public	Rect		startRect		= new Rect( 0, 0, 100, 100 );	// The rect the window is initially displayed at.
	public	bool		updateColor		= true;							// Do you want the color to change if the FPS gets low
	public	bool		allowDrag		= true;							// Do you want to allow the dragging of the FPS window
	public  float		frequency		= 0.5F;							// The update frequency of the fps
	public	int			nbDecimal		= 1;							// How many decimal do you want to display
	 
	private float		accum			= 0f;							// FPS accumulated over the interval
	private int			frames			= 0;							// Frames drawn over the interval
	private Color		color			= Color.white;					// The color of the GUI, depending of the FPS ( R < 10, Y < 30, G >= 30 )
	private string		sFPS			= "";							// The fps formatted into a string.
	private GUIStyle	style;											// The style the text will be displayed at, based en defaultSkin.label.

    private bool m_bUpdate = false;
    private float m_AndroidFPS = 0f;

	void Start()
	{
        if (!PlatformHelper.IsEnableDebugMode())
        {
            return;
        }

        m_AndroidFPS = Time.time + 1f;
        m_bUpdate = true;
		StartCoroutine(FPS());
	}

	void Update()
	{
        if(!m_bUpdate)
        {
            return;
        }
		accum += Time.timeScale / Time.deltaTime;
		++frames;

	}

	IEnumerator FPS()
	{
		while (true)
		{
			// Update the FPS
			float fps = accum/frames;
			sFPS = fps.ToString( "f" + Mathf.Clamp( nbDecimal, 0, 10 ) );
	    
			//Update the color
			color = (fps >= 30) ? Color.green : ((fps > 10) ? Color.yellow : Color.red);
	        
			accum = 0.0F;
			frames = 0;
	        
			yield return new WaitForSeconds( frequency );
		}
	}

	void OnGUI()
	{
        if (!m_bUpdate)
        {
            return;
        }
		// Copy the default label skin, change the color and the alignement
		if (style == null)
		{
			style = new GUIStyle( GUI.skin.label );
			style.normal.textColor = Color.white;
			style.alignment = TextAnchor.MiddleCenter;
		}
	    
		GUI.color = updateColor ? color : Color.white;
		Rect rect = startRect;
		if (centerTop)
			rect.x += Screen.width/2 - rect.width/2;
		startRect = GUI.Window(0, rect, DoMyWindow, "");
		if (centerTop)
			startRect.x -= Screen.width/2 - rect.width/2;

#if UNITY_ANDROID && !UNITY_EDITOR
    if(m_AndroidFPS <= Time.time)
            {
            Debug.Log("CTCFPS:" + sFPS);
            m_AndroidFPS = Time.time + 1f;
}
#endif
    }

    private int m_NPCNum = 0;
	void DoMyWindow(int windowID)
    {
        GUI.Label(new Rect(0, -15, startRect.width, startRect.height), sFPS + " FPS", style);
        //GUI.Label(new Rect(0, -3, startRect.width, startRect.height), "M: " + GC.GetTotalMemory(false)/(1024*1024), style);
        //GUI.Label(new Rect(0, 9, startRect.width, startRect.height), "U: " + Profiler.usedHeapSize/(1024*1024), style);
        GUI.Label(new Rect(0, -3, startRect.width, startRect.height), "R: " + NetWorkLogic.s_nReceiveCount, style);
        GUI.Label(new Rect(0, 9, startRect.width, startRect.height), "S: " + NetWorkLogic.s_nSendCount, style);

		if (allowDrag)
			GUI.DragWindow(new Rect(0, 0, Screen.width, Screen.height));

        
	}
}