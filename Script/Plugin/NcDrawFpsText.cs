// ----------------------------------------------------------------------------------
//
//
// ----------------------------------------------------------------------------------

using UnityEngine;
using Module.Log;
public class NcDrawFpsText : MonoBehaviour 
{
	// -------------------------------------------------------------------------------------------
	public  float updateInterval = 0.5F;
	 
	private float accum   = 0;
	private int   frames  = 0;
	private float timeleft;

    private bool m_bUpdate = false;
	// -------------------------------------------------------------------------------------------
	void Start()
	{
        if (!PlatformHelper.IsEnableDebugMode())
        {
            return;
        }

        m_bUpdate = true;
		if (!guiText)
		{
			LogModule.DebugLog("UtilityFramesPerSecond needs a GUIText component!");
			enabled = false;
			return;
		}
		timeleft = updateInterval;  
	}

	void Update()
	{
        if (!m_bUpdate)
        {
            return;
        }
		timeleft -= Time.deltaTime;
		accum += Time.timeScale/Time.deltaTime;
		++frames;
	    
		// Interval ended - update GUI text and start new interval
		if (timeleft <= 0.0)
		{
			// display two fractional digits (f2 format)
			float fps = accum/frames;
			string format = System.String.Format("{0:F2} FPS",fps);
			guiText.text = format;
            
			if (fps < 10)
				 guiText.material.color = Color.red;
			else if (fps < 30)
				guiText.material.color = Color.yellow;
			else 
                guiText.material.color = Color.green;

			timeleft = updateInterval;
			accum = 0.0F;
			frames = 0;
		}
	}
}
