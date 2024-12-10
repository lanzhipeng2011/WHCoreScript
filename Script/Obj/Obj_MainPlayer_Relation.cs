/********************************************************************************
 *	文件名：	Obj_MainPlayer_Relation.cs
 *	全路径：	\Script\Obj\Obj_MainPlayer_Relation.cs
 *	创建人：	李嘉
 *	创建时间：2013-02-14_情人节
 *
 *	功能说明：游戏主角Obj的社交关系逻辑部分
 *	修改记录：
*********************************************************************************/
using UnityEngine;
using System.Collections;
using System;
using Games.GlobeDefine;

namespace Games.LogicObj
{
    public partial class Obj_MainPlayer : Obj_OtherPlayer
    {
        //向服务器发起添加好友请求
        public void ReqAddFriend(UInt64 guid)
        {
            //判断guid合法性
            if (guid == GlobeVar.INVALID_GUID)
            {
                return;
            }

            //超过上限，不进行添加操作
            if (GameManager.gameManager.PlayerDataPool.FriendList.GetRelationNum() >= GlobeVar.MAX_FRIEND_NUM)
            {
                //您当前的好友列表已满。
                SendNoticMsg(false, "#{1080}");
                return;
            }

            //是否已经是好友，已经是则不添加
            if (GameManager.gameManager.PlayerDataPool.FriendList.RelationDataList.ContainsKey(guid))
            {
                return;
            }

            //自己不能加自己
            if (Singleton<ObjManager>.GetInstance().MainPlayer.GUID == guid)
            {
                return;
            }

            //向服务器发送添加好友包
            CG_ADDFRIEND msg = (CG_ADDFRIEND)PacketDistributed.CreatePacket(MessageID.PACKET_CG_ADDFRIEND);
            msg.Guid = guid;
            msg.SendPacket();

            // Singleton<ObjManager>.Instance.MainPlayer.SendNoticMsg(false, "#{2906}");
        }

        //向服务器发起删除好友请求
        public void ReqDelFriend(UInt64 guid)
        {
            //判断guid合法性
            if (guid == GlobeVar.INVALID_GUID)
            {
                return;
            }

            //是否已经是好友，不是则不删除
            if (false == GameManager.gameManager.PlayerDataPool.FriendList.RelationDataList.ContainsKey(guid))
            {
                return;
            }

            //向服务器发送删除好友包
            CG_DELFRIEND msg = (CG_DELFRIEND)PacketDistributed.CreatePacket(MessageID.PACKET_CG_DELFRIEND);
            msg.Guid = guid;
            msg.SendPacket();
        }

        //向服务器发起添加黑名单请求
        public void ReqAddBlack(UInt64 guid)
        {
            //判断guid合法性
            if (guid == GlobeVar.INVALID_GUID)
            {
                return;
            }

            //超过上限，不进行添加操作
            if (GameManager.gameManager.PlayerDataPool.BlackList.GetRelationNum() >= GlobeVar.MAX_BLACK_NUM)
            {
                return;
            }

            //向服务器发送添加黑名单包
            CG_ADDBLACKLIST msg = (CG_ADDBLACKLIST)PacketDistributed.CreatePacket(MessageID.PACKET_CG_ADDBLACKLIST);
            msg.Guid = guid;
            msg.SendPacket();
        }

        //向服务器发起删除黑名单请求
        public void ReqDelBlack(UInt64 guid)
        {
            //判断guid合法性
            if (guid == GlobeVar.INVALID_GUID)
            {
                return;
            }

            //向服务器发送删除黑名单包
            CG_DELBLACKLIST msg = (CG_DELBLACKLIST)PacketDistributed.CreatePacket(MessageID.PACKET_CG_DELBLACKLIST);
            msg.Guid = guid;
            msg.SendPacket();
        }

        //向服务器发起删除仇人请求
        public void ReqDelHate(UInt64 guid)
        {
            //判断guid合法性
            if (guid == GlobeVar.INVALID_GUID)
            {
                return;
            }

            //向服务器发送删除黑名单包
            CG_DELHATELIST msg = (CG_DELHATELIST)PacketDistributed.CreatePacket(MessageID.PACKET_CG_DELHATELIST);
            msg.Guid = guid;
            msg.SendPacket();
        }

        public void ReqTrailPlayer(UInt64 guid)
        {
            //判断guid合法性
            if (guid == GlobeVar.INVALID_GUID)
            {
                return;
            }

            if (GameManager.gameManager.PlayerDataPool.BackPack.GetItemCountByDataId(GlobeVar.WuYingXunZongDataID) <= 0)
            {
                SendNoticMsg(false, "#{3035}");
                return;
            }

            CG_ASK_TRAIL msg = (CG_ASK_TRAIL)PacketDistributed.CreatePacket(MessageID.PACKET_CG_ASK_TRAIL);
            msg.Guid = guid;
            msg.SendPacket();
        }
    }
}

