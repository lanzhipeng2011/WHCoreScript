using UnityEngine;
using System.Collections.Generic;
using System;

public class ShowLable : MonoBehaviour {

    public UILabel[] m_Lables;

    public void ChooseShowLable(int index)
    {
        if (index >= m_Lables.Length)
        {
            foreach (UILabel lable in m_Lables)
            {
                lable.gameObject.SetActive(false);
            }
            return;
        }

        for (int i = 0; i < m_Lables.Length; i++)
        {
            m_Lables[i].gameObject.SetActive(i == index);
        }
    }  

}
