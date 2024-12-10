/********************************************************************
	created:	2013/12/25
	created:	25:12:2013   10:50
	filename: 	ServerListItem.cs
	author:		王迪
	
	purpose:	服务器列表项
*********************************************************************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GCGame.Table;

public class ServerListItem : MonoBehaviour {

    public UILabel labelServerName;
    public UISprite sprState;
    public UISprite sprStateLabel;
    public UISprite[] serverRolesHead;
    public UILabel[] serverRolesLevel;
    public GameObject sprHeightLight;
    public GameObject sprNormal;
    public UISprite sprRecommand;
    public enum State
    {
        NEW,
        NORAML,
        HOT,
        STOP,
    }

    public enum Type
    {
        NEW,
        NORMAL,
        HOT,
    }

	void Start () {
	
	}

    public void SetState(string name, State state, Type type, List<LoginData.PlayerRoleData> serverRoleList)
    {
        labelServerName.text = name;
        switch(state)
        {
            case ServerListItem.State.HOT:
                sprState.spriteName = "YanChi03";
                break;
            case ServerListItem.State.NEW:
                sprState.spriteName = "YanChi01";
                break;
            case ServerListItem.State.NORAML:
                sprState.spriteName = "YanChi02";
                break;
            case ServerListItem.State.STOP:
                sprState.spriteName = "YanChi04";
                break;
            default:
                sprState.spriteName = "YanChi04";
                break;
        }

        //sprRecommand.gameObject.SetActive((int)type > 0);
        /*
        switch(type)
        {
            case Type.NEW:
                sprStateLabel.spriteName = "xinqu";
                break;
            case Type.NORMAL:
                sprStateLabel.spriteName = "shunchang";
                break;
            case Type.HOT:
                sprStateLabel.spriteName = "tuijian";
                break;
            default:
                sprStateLabel.spriteName = "xinqu";
                break;
        }
        */

        if( null != serverRoleList)
        {
            for (int i = 0, max = serverRoleList.Count; i < max; i++)
            {
                if (i < serverRolesHead.Length)
                {
                    Tab_CharModel curCharModel = TableManager.GetCharModelByID(serverRoleList[i].type, 0);
                    if (null != curCharModel)
                    {
                        serverRolesHead[i].spriteName = curCharModel.HeadPic;
                        serverRolesHead[i].gameObject.SetActive(true);
                    }
                    serverRolesLevel[i].text = serverRoleList[i].level.ToString();
                    serverRolesLevel[i].gameObject.SetActive(true);
                }
            }
        }
    }

    void OnSelectServerClick()
    {
        UIControllerBase<ServerListWindow>.Instance().ServerSelected(gameObject.name);
    }

    public void EnableHeightLight(bool bEnable)
    {
        sprHeightLight.SetActive(bEnable);
        sprNormal.SetActive(!bEnable);
        /*
        if (bEnable)
        {
            labelServerName.color = Color.yellow;
        }
        else
        {
            labelServerName.color = Color.white;
        }
         */
    }
}
