using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GCGame;
using Module.Log;
using Games.ChatHistory;

public class FastReplyLogic : MonoBehaviour {

    private static FastReplyLogic m_Instance = null;
    public static FastReplyLogic Instance()
    {
        return m_Instance;
    }

    private enum CUR_REPLYMODE
    {
        FAST_REPLY,
        HISTORY_REPLY,
    }

    public GameObject m_ModeButton;
    public GameObject m_EditButton;
    public GameObject m_QuitEditButton;
    public GameObject m_FastReplyGrid;
    public GameObject m_FastReplyEdit;
    public GameUIInput m_FastReplyEditInput;
    public GameObject m_FastReplyDetail;
    public UILabel m_FastReplyDetailLabel;
    public ChatInfoLogic m_ChatInfoLogic;

    private List<FastReplyItemLogic> m_FastReplyItemList = new List<FastReplyItemLogic>();
    private const int FastReplyMaxNum = 6;
    private int m_CurFastReplyItemIndex = -1;

    private CUR_REPLYMODE m_CurReplyMode = CUR_REPLYMODE.FAST_REPLY;

    void Awake()
    {
        m_Instance = this;
        InitFastReply();
    }
	// Use this for initialization
	void Start () {
        
	}

    void OnEnable()
    {
        
    }

    public void OnCloseChat()
    {
        m_Instance = null;
    }

    public void Init()
    {
        m_Instance = this;
    }

    void InitFastReply()
    {
        UIManager.LoadItem(UIInfo.FastReplyItem, OnLoadFastReplayItem);
    }

    void OnLoadFastReplayItem(GameObject resItem, object param)
    {
        List<string> curFastReply = UserConfigData.GetFastReplyList();
        for (int i = 0; i < FastReplyMaxNum; i++)
        {
            GameObject FastReplyItem = Utils.BindObjToParent(resItem,
                        m_FastReplyGrid, i < 10 ? ("FastReplyItem" + "0" + i.ToString()) : ("FastReplyItem" + i.ToString()));
            if (FastReplyItem != null)
            {
                FastReplyItemLogic itemLogic = FastReplyItem.GetComponent<FastReplyItemLogic>();
                if (itemLogic != null)
                {
                    if (i + 1 > curFastReply.Count)
                    {
                        itemLogic.Init(i, "", this);
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(curFastReply[i]))
                        {
                            itemLogic.Init(i, "", this);
                        }
                        else
                        {
                            itemLogic.Init(i, curFastReply[i], this);
                        }
                    }
                    m_FastReplyItemList.Add(itemLogic);
                }
            }
        }
        m_FastReplyGrid.GetComponent<UIGrid>().Reposition();
        ChangeMode();
    }

    void EditFastReply()
    {
        foreach (FastReplyItemLogic item in m_FastReplyGrid.GetComponentsInChildren<FastReplyItemLogic>())
        {
            item.ChangeToEdit();
        }
        m_ModeButton.SetActive(false);
        m_EditButton.SetActive(false);
        m_QuitEditButton.SetActive(true);
    }

    void QuitEditFastReply()
    {
        foreach (FastReplyItemLogic item in m_FastReplyGrid.GetComponentsInChildren<FastReplyItemLogic>())
        {
            item.QuitEdit();
        }
        m_ModeButton.SetActive(true);
        m_EditButton.SetActive(true);
        m_QuitEditButton.SetActive(false);
    }

    public void ShowReplyEdit(int index, string strContent)
    {
        m_FastReplyEdit.SetActive(true);

        m_CurFastReplyItemIndex = index;
        m_FastReplyEditInput.value = strContent;
    }

    void ReplyEditCancel()
    {
        m_FastReplyEdit.SetActive(false);
    }

    void ReplyEditComplete()
    {
        string strVal = m_FastReplyEditInput.value;
        int nTextCount = 0;
        for (int i = 0; i < strVal.Length; i++)
        {
            if (strVal[i] > 128)
            {
                nTextCount += 2;
            }
            else
            {
                nTextCount += 1;
            }
        }
        if (nTextCount > 256)
        {
            Singleton<ObjManager>.Instance.MainPlayer.SendNoticMsg(false, "#{2213}");
            return;
        }

        m_FastReplyItemList[m_CurFastReplyItemIndex].ItemStrContent = m_FastReplyEditInput.value;

        WriteAllFastReply();
        m_FastReplyEdit.SetActive(false);
    }

    void ReplyEditSubmit()
    {
        ReplyEditComplete();
        FastReplySubmit(m_CurFastReplyItemIndex);
    }

    public void FastReplySubmit(int nItemClickIndex)
    {
        string text = "";
        text = m_FastReplyItemList[nItemClickIndex].ItemStrContent;
        if (!string.IsNullOrEmpty(text))
        {
            if (text.Length > 3 && PlatformHelper.IsEnableGM() && text.Substring(0, 2) == GameDefines.GMCMD_BEGINORDER)
            {
                Utils.SendGMCommand(text.Substring(2, text.Length - 2));
            }
            else
            {
                text = Utils.StrFilter_Chat(text);
                if (m_CurReplyMode == CUR_REPLYMODE.FAST_REPLY)
                {
                    ChatHistoryItem item = new ChatHistoryItem();
                    item.CleanUp();
                    Utils.SendCGChatPak(text, item);
                }
                else
                {
                    List<ChatHistoryItem> listPlayerReplyHistory = GameManager.gameManager.PlayerDataPool.ChatHistory.ReplyHistoryList;
                    Utils.SendCGChatPak(text, listPlayerReplyHistory[listPlayerReplyHistory.Count - 1 - nItemClickIndex]);
                }
            }
        }
        m_ChatInfoLogic.RecoverFastReply();
    }

    public void WriteAllFastReply()
    {
        List<string> listFastReplyContent = new List<string>();
        foreach (FastReplyItemLogic item in m_FastReplyGrid.GetComponentsInChildren<FastReplyItemLogic>())
        {
            listFastReplyContent.Add(item.ItemStrContent);
        }
        UserConfigData.AddFastReplyInfo(listFastReplyContent);
    }

    public void ShowReplyDetail(int index, string strContent)
    {
        m_FastReplyDetail.SetActive(true);

        m_CurFastReplyItemIndex = index;
        m_FastReplyDetailLabel.text = strContent;
    }

    void ReplyDetailCancel()
    {
        m_FastReplyDetail.SetActive(false);
    }

    void ReplyDetailSubmit()
    {
        FastReplySubmit(m_CurFastReplyItemIndex);
        m_FastReplyDetail.SetActive(false);
    }

    void ChangeMode()
    {
        if (m_CurReplyMode == CUR_REPLYMODE.FAST_REPLY)
        {
            m_CurReplyMode = CUR_REPLYMODE.HISTORY_REPLY;
            UpdateHistoryReply();
            return;
        }
        else
        {
            m_CurReplyMode = CUR_REPLYMODE.FAST_REPLY;
            UpdateFastReply();
            return;
        }
    }

    public void UpdateHistoryReply()
    {
        if (m_CurReplyMode == CUR_REPLYMODE.HISTORY_REPLY)
        {
            m_EditButton.SetActive(false);
            for (int i = 0; i < m_FastReplyItemList.Count; ++i)
            {
                m_FastReplyItemList[i].ItemStrContent = "快捷回复";
            }
            //foreach (FastReplyItemLogic item in m_FastReplyItemList)
            //{
            //    item.ItemStrContent = "";
            //}
            List<ChatHistoryItem> listPlayerReplyHistory = GameManager.gameManager.PlayerDataPool.ChatHistory.ReplyHistoryList;
            for (int i = listPlayerReplyHistory.Count - 1, j = 0; i >= 0 && j < FastReplyMaxNum; i--, j++)
            {
                m_FastReplyItemList[j].ItemStrContent = listPlayerReplyHistory[i].ChatInfo;
            }
        }        
    }

    void UpdateFastReply()
    {
        if (m_CurReplyMode == CUR_REPLYMODE.FAST_REPLY)
        {
            m_EditButton.SetActive(true);
            List<string> curFastReply = UserConfigData.GetFastReplyList();
            for (int i = 0; i < FastReplyMaxNum; i++)
            {
                m_FastReplyItemList[i].ItemStrContent = curFastReply[i];
            }
        }
    }
}
