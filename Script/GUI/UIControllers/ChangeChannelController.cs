using UnityEngine;
using System.Collections;
using GCGame.Table;
using Module.Log;
using Games.LogicObj;

public class ChangeChannelController : MonoBehaviour {

	public GameObject ChannelListGrid;
    public GameObject FirstChild;
    public UIInput    InputChannel;
    public UILabel    m_CurLineLable;
	// Use this for initialization
	void Start () {
        UIManager.LoadItem(UIInfo.ChannelListItem, OnLoadChannelListItem);
        FirstChild.SetActive(true);
	}

    void OnLoadChannelListItem(GameObject resItem, object param)
    {
        for (int i = 0; i < SceneData.SceneInstList.Count; i++)
        {
            ChannelListItem.CreateItem(ChannelListGrid, resItem, SceneData.SceneInstList[i].ToString(), this, StrDictionary.GetClientDictionaryString("#{1177}", (SceneData.SceneInstList[i] + 1).ToString()));
        }

        ChannelListGrid.GetComponent<UIGrid>().repositionNow = true;
        int nLine = SceneData.SceneInst + 1;
        m_CurLineLable.text = StrDictionary.GetClientDictionaryString("#{3192}", nLine);

    }

	public void OnChangeChannel(ChannelListItem item)
	{
        int channelID;
        if(int.TryParse(item.name, out channelID))
        {
            if (channelID != SceneData.SceneInst && SceneData.SceneInstList.Contains(channelID))
            {
                CG_SCENE_CHANGEINST packet = (CG_SCENE_CHANGEINST)PacketDistributed.CreatePacket(MessageID.PACKET_CG_SCENE_CHANGEINST);
                packet.SetSceneInst(channelID);
                packet.SendPacket();
            }
        }
        
		UIManager.CloseUI(UIInfo.ChannelChange);
	}

    void OnCloseClick()
    {
        UIManager.CloseUI(UIInfo.ChannelChange);
        UIManager.ShowUI(UIInfo.SceneMapRoot);
    }

    // 回车时响应
    public void OnInputSubmit()
    {
        Obj_MainPlayer mainPlayer = Singleton<ObjManager>.GetInstance().MainPlayer;
        if (null == mainPlayer)
        {
            return;
        }
        string szCurInput = InputChannel.value;
        for (int i = 0; i < szCurInput.Length; i++)
        {
            if (szCurInput[i] < '0' || szCurInput[i] > '9')
            {
                InputChannel.value = "";
                mainPlayer.SendNoticMsg(false, "#{2174}");     
                break;
            }
        }
    }

    void OnClickChangeChannel()
    {
        if (null == InputChannel)
        {
            LogModule.ErrorLog( "InputChannel  can't find ");
            return;
        }
        if (string.IsNullOrEmpty(InputChannel.value))
        {
            //MessageBoxLogic.OpenOKBox(1288, 1000);
            MessageBoxLogic.OpenOKBox(2174, 1000);
            return;
        }
        int nChannelID = -1;
        if (!int.TryParse(InputChannel.value, out nChannelID))
        {
            MessageBoxLogic.OpenOKBox(2174, 1000);
            return;
        }
        nChannelID = nChannelID - 1;
        if (nChannelID >= 0 && nChannelID != SceneData.SceneInst && SceneData.SceneInstList.Contains(nChannelID))
        {

            CG_SCENE_CHANGEINST packet = (CG_SCENE_CHANGEINST)PacketDistributed.CreatePacket(MessageID.PACKET_CG_SCENE_CHANGEINST);
            packet.SetSceneInst(nChannelID);
            packet.SendPacket();
            UIManager.CloseUI(UIInfo.ChannelChange);
        }
        else
        {
            MessageBoxLogic.OpenOKBox(2174, 1000);
        }
    }
}
