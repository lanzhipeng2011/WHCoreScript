using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Module.Log;
using Games.LogicObj;

public class RechargeData 
{
	private const string key_goods_describe = "\"goods_describe\"";
	private const string key_goods_icon = "\"goods_icon\"";
	private const string key_goods_id = "\"goods_id\"";
	private const string key_goods_name = "\"goods_name\"";
	private const string key_goods_number = "\"goods_number\"";
	private const string key_goods_price = "\"goods_price\"";
	private const string key_register_id = "\"goods_register_id\"";

    static public void PayUI()
    {
        CG_ASK_ISRECHARGE_ENABLE packet = (CG_ASK_ISRECHARGE_ENABLE)PacketDistributed.CreatePacket(MessageID.PACKET_CG_ASK_ISRECHARGE_ENABLE);
        packet.SetNone(0);
        packet.SendPacket();
    }

    public static void RechareStateUpdate(bool bEnable)
    {
        if (!bEnable)
        {
            Obj_MainPlayer mainPlayer = Singleton<ObjManager>.Instance.MainPlayer;
            if (null != mainPlayer)
            {
                mainPlayer.SendNoticMsg(false, "#{2136}");
            }
            return;
        }
        //if (Application.platform == RuntimePlatform.Android)
        //{
        //    PlatformHelper.MakePay();
        //}
        //else
        //{
            UIManager.ShowUI(UIInfo.Recharge);
        //}
    }

	public class GoodInfo
	{
       
		public GoodInfo(string info)
		{
			goods_describe = GetValue(key_goods_describe, ref info);
			goods_icon = GetValue(key_goods_icon, ref info);
			goods_id = GetValue(key_goods_id, ref info);
			goods_name = GetValue(key_goods_name, ref info);
			goods_number = GetValue(key_goods_number, ref info);
			goods_price = GetValue(key_goods_price, ref info);
			goods_register_id = GetValue(key_register_id, ref info);
		}


        public GoodInfo()
        {

        }

		public string goods_describe;
		public string goods_icon;
		public string goods_id;
		public string goods_name;
		public string goods_number;
		public string goods_price;
		public string goods_register_id;

        // 设置倍数
        public GoodInfo GetGoodInfoWithTimes(int times)
        {
            int newGoodNum = -1;
            int newGoodPrice = -1;
            if (!int.TryParse(goods_number, out newGoodNum) || !int.TryParse(goods_price, out newGoodPrice))
            {
                LogModule.ErrorLog("good_num or good price error:" + goods_number + " " + goods_price);
                return null;
            }
            
            GoodInfo newGood = new GoodInfo();
            newGood.goods_describe = goods_describe;
            newGood.goods_icon = goods_icon;
            newGood.goods_id = goods_id;
            newGood.goods_name = goods_name;
            newGood.goods_number = (newGoodNum).ToString();
            newGood.goods_price = (newGoodPrice * times).ToString();
            newGood.goods_register_id = goods_register_id;

            return newGood;
        }

		private string GetValue(string curKey, ref string info)
		{
			int startPos = info.IndexOf(curKey) + curKey.Length+1;
			int valueStartPos = info.IndexOf("=", startPos) + 2;
			int valueEndPos = info.IndexOf(";", valueStartPos);
			return info.Substring(valueStartPos,  valueEndPos - valueStartPos); 
		}

        public static GoodInfo ValueOf(JsonData data)
        {
            GoodInfo good = new GoodInfo();
            string goodsId = data["goodsId"].ToString();
            string goodsIcon = data["goodsIcon"].ToString();
            string goodsNumber = data["goodsNumber"].ToString();
            string goodsDescribe = data["goodsDescribe"].ToString();
            string goodsPrice = data["goodsPrice"].ToString();
            if (goodsPrice != null)
            { 
                int dotPos = goodsPrice.IndexOf(".");
                if (dotPos > 0)
                {
                    goodsPrice = goodsPrice.Substring(0, dotPos);
                }
            }
            string type = data["type"].ToString();
            string goodsRegisterId = data["goodsRegisterId"].ToString();
            string goodsName = data["goodsName"].ToString();
            good.goods_id = goodsId;
            good.goods_icon = goodsIcon;
            good.goods_number = goodsNumber;
            good.goods_describe = goodsDescribe;
            good.goods_price = goodsPrice;
            good.goods_register_id = goodsRegisterId;
            good.goods_name = goodsName;
            return good;
        }

		public void WriteString()
		{
			LogModule.DebugLog("des:" + goods_describe + " icon:" + goods_icon + " id:" + goods_id + " name:" + goods_name + " number:" + goods_number + " price" + goods_price + " regid" + goods_register_id);
		}
	}

	public static Dictionary<string, GoodInfo> m_dicGoodInfos = new Dictionary<string, GoodInfo>();

	public static void InitGoodInfo(string goodInfoList)
	{
		m_dicGoodInfos.Clear();
#if !UNITY_EDITOR && UNITY_ANDROID 
        JsonData data = JsonMapper.ToObject(goodInfoList);
        for (int i = 0; i < data.Count; i++)
        {
            GoodInfo newGood = GoodInfo.ValueOf(data[i]);
            if (!m_dicGoodInfos.ContainsKey(newGood.goods_register_id))
            {
                m_dicGoodInfos.Add(newGood.goods_register_id, newGood);
            }
            else
            {
                LogModule.ErrorLog("key exist" + newGood.goods_register_id);
                break;
            }
        }
#else
        int curPos = goodInfoList.IndexOf("{") + 1;
		while(curPos > 0 && curPos < goodInfoList.Length)
		{
			int endPos = goodInfoList.IndexOf("}", curPos);
			Debug.Log( " endPos"+endPos);
			if(endPos < 0 && endPos <= curPos)
			{
				LogModule.ErrorLog(" parse good info list fail");
				break;
			}

			GoodInfo newGood = new GoodInfo(goodInfoList.Substring(curPos, endPos - curPos));
			Debug.Log("add goodInfo" + newGood.goods_register_id);
			if(!m_dicGoodInfos.ContainsKey(newGood.goods_register_id))
			{
				m_dicGoodInfos.Add(newGood.goods_register_id, newGood);
			}
			else
			{
				LogModule.ErrorLog("key exist" + newGood.goods_register_id);
				break;
			}
			curPos = goodInfoList.IndexOf("{", endPos) + 1;

        }
#endif
        
        foreach (KeyValuePair<string, GoodInfo> curPair in m_dicGoodInfos)
		{
			curPair.Value.WriteString();
        }

    }

    public static GoodInfo GetGoodInfo(string registerID)
    {
        if (m_dicGoodInfos.ContainsKey(registerID))
        {
            return m_dicGoodInfos[registerID];
        }

        return null;
    }

}
