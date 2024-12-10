using UnityEngine;
using System.Collections;

public class PopMenuItemLogic : MonoBehaviour {

    public UILabel m_MenuItemLabel;
    public delegate void MenuItemOnClicked();
    private MenuItemOnClicked deleMenuItemOnClicked;        // 响应函数托管

	// Use this for initialization
	void Start () {
	
	}

    /// <summary>
    /// 菜单项点击事件
    /// </summary>
    void OnClicked()
    {
        MenuItemOnClicked temp = deleMenuItemOnClicked;
        deleMenuItemOnClicked = null;
        if (temp != null)
        {
            temp();
            UIManager.CloseUI(UIInfo.PopMenuRoot);
        }
    }

    /// <summary>
    /// 初始化菜单项信息
    /// </summary>
    /// <param name="strLabel">文字</param>
    /// <param name="funcItemOnClicked">响应函数</param>
    public void InitMenuItem(string strLabel, MenuItemOnClicked funcItemOnClicked)
    {
        m_MenuItemLabel.text = strLabel;
        deleMenuItemOnClicked = funcItemOnClicked;
    }
}
