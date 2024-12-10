using UnityEngine;
using System.Collections;

public class RankItem : MonoBehaviour {

    //..
    public UILabel m_Label1;
    public UILabel m_Label2;
    public UILabel m_Label3;
    public UILabel m_Label4;
    public UILabel m_Label5;
    public UILabel m_Label6;

    //..
    public UIGrid m_LabelGrid;


    public void SetMaxLabel( int n)
    {
        if(m_LabelGrid != null)
        {
            m_LabelGrid.cellWidth = 600/n;
            m_LabelGrid.repositionNow = true;
        }
    }

    public void SendData(string str1, string str2, string str3 = "", string str4 = "", string str5 = "", string str6 = "")
    {
        string colorStr = "";
        if( str1 == "1")
        {
            colorStr = "[ff9933]"; 
        }
        else if( str1 == "2")
        {
            colorStr = "[cc66ff]";
        }
        else if( str1 == "3")
        {
            colorStr = "[33cc66]";
        }
        m_Label1.text = colorStr + str1;
        m_Label2.text = colorStr + str2;
        m_Label3.text = colorStr + str3;
        m_Label4.text = colorStr + str4;
        m_Label5.text = colorStr + str5;
        m_Label6.text = colorStr + str6;
    }

    public void Cleanup()
    {
        m_Label1.text = "";
        m_Label2.text = "";
        m_Label3.text = "";
        m_Label4.text = "";
        m_Label5.text = "";
        m_Label6.text = "";
    }
}
