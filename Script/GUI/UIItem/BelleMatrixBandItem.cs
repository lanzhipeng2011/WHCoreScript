using UnityEngine;
using System.Collections;

public class BelleMatrixBandItem : MonoBehaviour {

    public UISprite m_SprIcon;
    public UILabel m_LabelName;
    public UITexture m_TexHeadInfo;

    public void SetIconName(string iconName, int belleNature)
    {
        if (null == iconName)
        {
            m_TexHeadInfo.gameObject.SetActive(false);
        }
        else
        {
            Texture belleTextureObj = ResourceManager.LoadResource(iconName, typeof(Texture)) as Texture;
            if (null != belleTextureObj)
            {
                m_TexHeadInfo.mainTexture = belleTextureObj;
                m_TexHeadInfo.gameObject.SetActive(true);
            }
            else
            {
                m_TexHeadInfo.gameObject.SetActive(false);
            }
            
        }

        BelleData.SetLabelNature(m_LabelName, belleNature);
    }
}
