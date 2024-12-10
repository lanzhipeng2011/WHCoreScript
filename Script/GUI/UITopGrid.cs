/********************************************************************
	created:	2014/01/15
	created:	15:1:2014   16:02
	filename: 	UITopGrid.cs
	author:		王迪
	
	purpose:	改造了一下CenterOnChild，改成在列表总长度小于剪裁长度时
                可以左右拖动，拖动后拉回初始位置
*********************************************************************/
using UnityEngine;
using Module.Log;
/// <summary>
/// Ever wanted to be able to auto-center on an object within a draggable panel?
/// Attach this script to the container that has the objects to top on as its children.
/// </summary>

[AddComponentMenu("NGUI/Interaction/Top On Grid")]
public class UITopGrid : MonoBehaviour
{
	/// <summary>
	/// The strength of the spring.
	/// </summary>

	public float springStrength = 8f;
    public float cellSize = 50.0f;
	public bool IsResetOnEnable = false;
    public bool IsRecenterOnDragFinished = true;

    public bool recenterTopNow = false;

    private bool IsTopOnDragWhenCellNoFull = false;
    public bool TopOnDragWhenCellNoFull
    {
        set
        {
            IsTopOnDragWhenCellNoFull = value;
        }
    }
	/// <summary>
	/// Callback to be triggered when the centering operation completes.
	/// </summary>

	public SpringPanel.OnFinished onFinished;

	UIDraggablePanel mDrag;
	GameObject mCenteredObject;
	Vector3 mPanelStartPos ;
	Vector2 mClipStartCenter;

	/// <summary>
	/// Game object that the draggable panel is currently centered on.
	/// </summary>

	public GameObject centeredObject { get { return mCenteredObject; } }

    void Start() 
    { 
        Recenter(IsResetOnEnable); 
    }
    void FixedUpdate() 
    {
        if (recenterTopNow)
        {
            Recenter(true);
            recenterTopNow = false;
        }
    }
    void OnDragFinished() 
    {
        if (enabled && IsRecenterOnDragFinished) 
        {
            if (!IsTopOnDragWhenCellNoFull)
            {
                Recenter(false);
                return;
            }
            if (IsAllChildRestrictInPanel())
            {
                Recenter(true);
            }
            else
            {
                Recenter(false);
            }    
            Recenter(false);
        }     
    }

	/// <summary>
	/// Recenter the draggable list on the center-most child.
	/// </summary>

	public void Recenter (bool bResetToTop)
	{
		if (mDrag == null)
		{
			mDrag = NGUITools.FindInParents<UIDraggablePanel>(gameObject);

			if (mDrag == null)
			{
				LogModule.WarningLog(GetType() + " requires " + typeof(UIDraggablePanel) + " on a parent object in order to work", this);
				enabled = false;
				return;
			}
			else
			{
				mDrag.onDragFinished = OnDragFinished;
				mPanelStartPos = mDrag.transform.localPosition;
				UIPanel curPanel = mDrag.gameObject.GetComponent<UIPanel>();
				if(null != curPanel)
				{
					mClipStartCenter = new Vector2(curPanel.clipRange.x, curPanel.clipRange.y);
				}
				if (mDrag.horizontalScrollBar != null)
					mDrag.horizontalScrollBar.onDragFinished = OnDragFinished;

				if (mDrag.verticalScrollBar != null)
					mDrag.verticalScrollBar.onDragFinished = OnDragFinished;
			}
		}
		if (mDrag.panel == null) return;

		if(bResetToTop)
		{
            mDrag.DisableSpring();
			mDrag.transform.localPosition = mPanelStartPos;
			Vector4 curRange = mDrag.panel.clipRange;
			curRange.x = mClipStartCenter.x;
			curRange.y = mClipStartCenter.y;
			mDrag.panel.clipRange = curRange;
			mDrag.repositionClipping = true;
			return;
		}
		// Calculate the panel's center in world coordinates
		Vector4 clip = mDrag.panel.clipRange;
		Transform dt = mDrag.panel.cachedTransform;

        Vector3 panelPos = dt.localPosition;
        Vector3 top = panelPos;
        Vector3 bottom = panelPos;

        if (mDrag.scale.y > 0)
        {
            top.y = panelPos.y + clip.w * 0.5f - cellSize * 0.5f;
            top.x += clip.x;
            top.y += clip.y;
            top = dt.parent.TransformPoint(top);

            if (transform.childCount * cellSize > clip.w)
            {
                bottom.y = panelPos.y - clip.w * 0.5f + cellSize * 0.5f;
            }
            else
            {
                bottom.y = panelPos.y + clip.w * 0.5f - cellSize * (transform.childCount - 0.5f);
            }

            bottom.x += clip.x;
            bottom.y += clip.y;
            bottom = dt.parent.TransformPoint(bottom);

            // 
            if (transform.childCount > 0)
            {
                if (transform.GetChild(transform.childCount - 1).position.y > bottom.y)
                {
                    top = bottom;
                }
            }
       
        }
        else
        {
            top.x = panelPos.x - clip.z * 0.5f + cellSize * 0.5f;
            top.x += clip.x;
            top.y += clip.y;
            top = dt.parent.TransformPoint(top);

            if (transform.childCount * cellSize > clip.z)
            {
                bottom.x = panelPos.x + clip.z * 0.5f - cellSize * 0.5f;
            }
            else
            {
                bottom.x = panelPos.x - clip.z * 0.5f + cellSize * (transform.childCount - 0.5f);
            }

            bottom.x += clip.x;
            bottom.y += clip.y;
            bottom = dt.parent.TransformPoint(bottom);

            // 
            if (transform.childCount > 0)
            {
                if (transform.GetChild(transform.childCount - 1).position.x < bottom.x)
                {
                    top = bottom;
                }
            }
        }
       
		// Offset this value by the momentum
        Vector3 offsetCenter = top - mDrag.currentMomentum * (mDrag.momentumAmount * 0.1f);
		mDrag.currentMomentum = Vector3.zero;

		float min = float.MaxValue;
		Transform closest = null;
		Transform trans = transform;

		// Determine the closest child
		for (int i = 0, imax = trans.childCount; i < imax; ++i)
		{
			Transform t = trans.GetChild(i);
			float sqrDist = Vector3.SqrMagnitude(t.position - offsetCenter);

			if (sqrDist < min)
			{
				min = sqrDist;
				closest = t;
			}
		}

		if (closest != null)
		{
			mCenteredObject = closest.gameObject;

			// Figure out the difference between the chosen child and the panel's center in local coordinates
			Vector3 cp = dt.InverseTransformPoint(closest.position);
            Vector3 cc = dt.InverseTransformPoint(top);
			Vector3 offset = cp - cc;

			// Offset shouldn't occur if blocked by a zeroed-out scale
			if (mDrag.scale.x == 0f) offset.x = 0f;
			if (mDrag.scale.y == 0f) offset.y = 0f;
			if (mDrag.scale.z == 0f) offset.z = 0f;

			// Spring the panel to this calculated position
			SpringPanel.Begin(mDrag.gameObject, dt.localPosition - offset, springStrength).onFinished = onFinished;
		}
		else mCenteredObject = null;
	}
   
    bool IsAllChildRestrictInPanel()
    {
        if (transform.childCount <= 0)
        {
            return false;
        }
        UIGrid grid = gameObject.GetComponent<UIGrid>();
        if (grid == null)
        {
            return false;
        }
        if (mDrag == null)
        {
            mDrag = NGUITools.FindInParents<UIDraggablePanel>(gameObject);
            if (mDrag == null)
            {
                return false;
            }
        }
        int nCellcountPerLine = grid.maxPerLine;
        if (mDrag.scale.y <= 0)
        {
            return false;
        }
        if (nCellcountPerLine <= 0)
        {
            nCellcountPerLine = transform.childCount;
        }
        if (nCellcountPerLine <= 0)
        {
            return false;
        }
        int nLine = transform.childCount / nCellcountPerLine;
        if (transform.childCount % nCellcountPerLine > 0)
        {
            nLine += 1;
        }
        if (cellSize * nLine >= mDrag.panel.clipRange.w)
        {
            return false;
        }
        return true;
    }
}