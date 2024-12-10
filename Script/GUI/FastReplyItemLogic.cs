using UnityEngine;
using System.Collections;

public class FastReplyItemLogic : MonoBehaviour {

    public GameObject m_DeleteButton;
    public GameObject m_DetailButton;
    public GameObject m_EditButton;
    public GameObject m_ReplyButton;
    public UILabel m_ReplyContent;

    private int m_ItemIndex = -1;

    private string m_ItemStrContent = "";
    public string ItemStrContent
    {
        get { return m_ItemStrContent; }
        set 
        { 
            m_ItemStrContent = value;
            if (m_ReplyContent != null)
            {
                m_ReplyContent.text = m_ItemStrContent;
            }
        }
    }

    private FastReplyLogic m_FastReplyLogic = null;

	// Use this for initialization
	void Start () {
        m_EditButton.SetActive(false);
        m_DeleteButton.SetActive(false);
        m_DetailButton.SetActive(true);
	}

    public void ChangeToEdit()
    {
        m_EditButton.SetActive(true);
        m_DeleteButton.SetActive(true);
        m_DetailButton.SetActive(false);
    }

    public void QuitEdit()
    {
        m_EditButton.SetActive(false);
        m_DeleteButton.SetActive(false);
        m_DetailButton.SetActive(true);
    }

    public void Init(int index, string text, FastReplyLogic parentLogic)
    {
        m_ItemIndex = index;
        m_ItemStrContent = text;

        if (m_ReplyContent != null)
        {
            m_ReplyContent.text = text;
        }

        m_FastReplyLogic = parentLogic;
    }

    void DeleteReply()
    {
        m_ItemStrContent = "";
        if (m_ReplyContent != null)
        {
            m_ReplyContent.text = "";
        }

        if (m_FastReplyLogic != null)
        {
            m_FastReplyLogic.WriteAllFastReply();
        }
    }

    void ShowReplyDetail()
    {
        if (m_FastReplyLogic != null)
        {
            m_FastReplyLogic.ShowReplyDetail(m_ItemIndex, m_ItemStrContent);
        }
    }

    void ShowReplyEdit()
    {
        if (m_FastReplyLogic != null)
        {
            m_FastReplyLogic.ShowReplyEdit(m_ItemIndex, m_ItemStrContent);
        }
    }

    void SubmitReply()
    {
        if (m_FastReplyLogic != null)
        {
            m_FastReplyLogic.FastReplySubmit(m_ItemIndex);
        }
    }
}
