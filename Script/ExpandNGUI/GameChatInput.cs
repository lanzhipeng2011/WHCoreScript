//********************************************************************
// 文件名: GameChatInpput.cs
// 描述: 聊天输入框 在NGUI自带的ChatInput基础上修改
// 作者: WangZhe
//********************************************************************

using UnityEngine;
using System.Collections;
using GCGame;
using Module.Log;
using Games.ChatHistory;
public class GameChatInput : MonoBehaviour
{
    public GameTextList textList;

    GameUIInput mInput;
    bool mIgnoreNextEnter = false;
    void Start()
    {
        mInput = GetComponent<GameUIInput>();
        //         for (int i = 0; i < 30; ++i)
        //         {
        //             textList.Add(((i % 2 == 0) ? "[FFFFFF]" : "[AAAAAA]") +
        //                 "This is an example paragraph for the text list, testing line " + i + "[-]");
        //         }
    }

#if UNITY_EDITOR
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Return))
        {
            if (!mIgnoreNextEnter && !mInput.selected)
            {
                mInput.label.maxLineCount = 1;
                mInput.selected = true;
            }
            mIgnoreNextEnter = false;
        }
    } 
#endif

    public void OnSubmit()
    {
        if (textList != null)
        {
            string text = NGUITools.StripSymbols(mInput.value);
            text = ChatInfoLogic.Instance().InsertLinkSymbols(text);

            string textCopy = text;
            if (ChatInfoLogic.Instance().GetCurChannelType() == (int)CG_CHAT.CHATTYPE.CHAT_TYPE_TELL)
            {
                textCopy = textCopy.Replace("/" + ChatInfoLogic.Instance().TellChatReceiverName + " ", "");
            }
            else if (ChatInfoLogic.Instance().GetCurChannelType() == (int)CG_CHAT.CHATTYPE.CHAT_TYPE_FRIEND)
            {
                textCopy = textCopy.Replace("/" + ChatInfoLogic.Instance().FriendChatReceiverName + " ", "");
            }

            if (!string.IsNullOrEmpty(textCopy))
            {
                //int line;
                //textList.Add(text, out line);

                if (text.Length > 3 && PlatformHelper.IsEnableGM() && text.Substring(0, 2) == GameDefines.GMCMD_BEGINORDER)
                {
                    Utils.SendGMCommand(text.Substring(2, text.Length-2));
                }
                else
                {
                    text = Utils.StrFilter_Chat(text);
                    ChatHistoryItem item = new ChatHistoryItem();
                    item.CleanUp();
                    Utils.SendCGChatPak(text, item);
                }
            }

            ChatInfoLogic.Instance().ClearCurInput();
            mInput.selected = false;            
        }
    }

    public void ShowNewChat(string strChatFull)
    {
        int linesCount = 0;
        textList.Add(strChatFull, out linesCount);
        ChatInfoLogic.Instance().MoveLinkPos(ChatInfoLogic.EMOTIONLINK_MOVE_DIRECTION.EMOTIONLINK_MOVE_UP, linesCount);
        ChatInfoLogic.Instance().MoveEmotionPos(ChatInfoLogic.EMOTIONLINK_MOVE_DIRECTION.EMOTIONLINK_MOVE_UP, linesCount);
    }

    public void ClearChatHistory()
    {
        textList.Clear();
    }
}
