/********************************************************************
	created:	2014/02/13
	created:	13:2:2014   19:12
	filename: 	UserConfigData.cs
	file ext:	cs
	author:		王迪
	
	purpose:	客户端配置文件类
*********************************************************************/

using System;
using Games.GlobeDefine;
using Games.SkillModle;
using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Xml;
using Module.Log;

#if UNITY_WP8
using UnityPort;
using System.Text;
using System.Xml.Serialization;
#endif

public class UserConfigData
{
    public static string DataPath { get { return Application.persistentDataPath + "/UserData/"; } }

	private static int AccountInfoMax = 5;		// role info max

    private static string AccountFileName = "Account.data";
    private static Dictionary<string, string> m_accountMap = null;

    private static string RoleInfoFileLast = ".RoleInfo";
    private static Dictionary<string, List<LoginData.PlayerRoleData>> m_roleInfoMap;
    private static string CurRoleInfoAccount = "";

    //private static string ClientResVersionFileName = "ClientResVersion.data";

    private static string FastReplyFileName = "FastReply.data";
    private static List<string> m_FastReplyList = null;

    private static string MissionClientName = "MissionClient.data";
    private static Dictionary<string, List<Games.Mission.CurOwnMission>> m_MissionClientData;

    private static string AutoConfigFileName = "AutoConfig.data";
    private static Dictionary<string, PlayerAuto> m_AutoConfigMap;

	private static string RoleSpecialDataFileName = "SpecialData.data";
	private static Dictionary<string, PlayerSpecialData> m_SpecialDataMap;

    private static string SkillBarSetFileName = "SkillBarSet.data";
    private static Dictionary<string, SkillBarInfo[]> m_SkillBarSetMap = null;

    private static string RestaurantConfigFileName = "RestaurantConfig.data";
    private static Dictionary<string, RestaurantConfigData> m_RestaurantConfigMap;
    
    public static int ClientResVersion
    {
        get
        {
            string strVersionFilePath = UpdateHelper.LocalVersionPath + "/" + UpdateHelper.VersionFileName;
            if (!File.Exists(strVersionFilePath))
            {
                return 0;
            }

            FileStream fs = new FileStream(strVersionFilePath, FileMode.Open, FileAccess.Read);
            StreamReader sr = new StreamReader(fs);

            string strLine = sr.ReadLine();

            int retValue = 0;
            if (!int.TryParse(strLine, out retValue))
            {
                LogModule.ErrorLog("res version file format error :" + strLine);
                retValue = 0;
            }

            fs.Close();
            
            return retValue;
        }
    }


    // 将账号信息保存在TXT文件中
    public static void AddAccountInfo(string account, string psw)
    {
        Dictionary<string, string> accountMapOrg = GetAccountList();
        Dictionary<string, string> newAccountMap = new Dictionary<string, string>();
        foreach (KeyValuePair<string, string> oldPair in accountMapOrg)
        {
            if(oldPair.Key != account)
            {
                newAccountMap.Add(oldPair.Key, oldPair.Value);
            }
        }
 
        newAccountMap.Add(account, psw);
        if (newAccountMap.Count > AccountInfoMax)
		{
			string delKey = null;
            foreach (string curKey in newAccountMap.Keys)
			{
				delKey = curKey;
				break;
			}

			if(null != delKey)
			{
                newAccountMap.Remove(delKey);
				RemoveRoleInfo(delKey);
			}
		}
        if (!Directory.Exists(DataPath))
        {
            Directory.CreateDirectory(DataPath);
        }
        FileStream fs = new FileStream(DataPath + AccountFileName, FileMode.Create);
        StreamWriter sw = new StreamWriter(fs);
        foreach (KeyValuePair<string, string> curPair in newAccountMap)
        {
            sw.WriteLine(curPair.Key + '\t' + curPair.Value);
        }

        sw.Close();
        fs.Close();

        m_accountMap = newAccountMap;
    }

    // 从配置文件读取账号数据，保存在Map中
    public static Dictionary<string, string> GetAccountList()
    {
        if (m_accountMap != null)
        {
            return m_accountMap;
        }

        m_accountMap = new Dictionary<string, string>();
        string filePath = DataPath + AccountFileName;
        if (!File.Exists(filePath))
        {
            return m_accountMap;
        }
        FileStream fs = new FileStream(filePath, FileMode.Open);
        StreamReader sr = new StreamReader(fs);

        string strLine = sr.ReadLine();
        while (strLine != null)
        {
            string[] codes = strLine.Split('\t');
            if (codes.Length == 2)
            {
                if (m_accountMap.ContainsKey(codes[0]))
                {
                    m_accountMap[codes[0]] = codes[1];
                }
                else
                {
                    m_accountMap.Add(codes[0], codes[1]);
                }
            }
            strLine = sr.ReadLine();
        }

        sr.Close();
        fs.Close();
        return m_accountMap;
    }

#if UNITY_WP8
    public class RoleElement : IXmlSerializable
    {
        public string type { get; set; }
        public string level { get; set; }

        public System.Xml.Schema.XmlSchema GetSchema() { return null; }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            if (null == reader)
            {
                return;
            }

            reader.Read();
            reader.ReadStartElement("Type");
            type = reader.ReadContentAsString();
            reader.ReadEndElement();

            reader.ReadStartElement("Level");
            level = reader.ReadContentAsString();
            reader.ReadEndElement();
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteElementString("Type", type);
            writer.WriteElementString("Level", level);
        }
    }

    public class ServerElement : IXmlSerializable
    {
        public List<RoleElement> list { get; set; }

        public System.Xml.Schema.XmlSchema GetSchema() { return null; }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            list = new List<RoleElement>();
            while (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "Role")
            {
                RoleElement element = new RoleElement();
                element.ReadXml(reader);
                list.Add(element);
                reader.Read();
            }
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            foreach (RoleElement element in list)
            {
                writer.WriteStartElement("Role");
                element.WriteXml(writer);
                writer.WriteEndElement();
            }
        }

    }

    public class RoleInfo : IXmlSerializable
    {
        public Dictionary<string, ServerElement> dic { get; set; }

        public System.Xml.Schema.XmlSchema GetSchema() { return null; }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            dic = new Dictionary<string, ServerElement>();

            if (reader.MoveToContent() == XmlNodeType.Element && "RoleInfo" == reader.LocalName)
            {
                reader.Read();
                while (reader.MoveToContent() == XmlNodeType.Element)
                {
                    string name = reader.LocalName;
                    ServerElement elem = new ServerElement();

                    reader.Read();
                    elem.ReadXml(reader);
                    dic.Add(name, elem);
                    reader.Read();
                }
            }
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            foreach (KeyValuePair<string, ServerElement> elem in dic)
            {
                writer.WriteStartElement(elem.Key);
                elem.Value.WriteXml(writer);
                writer.WriteEndElement();
            }
        }

    }

#endif

    // 将角色信息保存在XML文件中
    public static void AddRoleInfo()
    {
        string account = PlayerPreferenceData.LastAccount;
        string serverID = PlayerPreferenceData.LastServer.ToString();

        m_roleInfoMap = GetRoleInfoList(account);
        if(m_roleInfoMap.ContainsKey(serverID))
        {
            m_roleInfoMap.Remove(serverID);
        }

        m_roleInfoMap.Add(serverID, LoginData.loginRoleList);

        if (!Directory.Exists(DataPath))
        {
            Directory.CreateDirectory(DataPath);
        }

#if UNITY_WP8

        RoleInfo roleInfo = new RoleInfo();
        roleInfo.dic = new Dictionary<string, ServerElement>();

        foreach (KeyValuePair<string, List<LoginData.PlayerRoleData>> iter in m_roleInfoMap)
        {
            ServerElement serverElem = new ServerElement();
            serverElem.list = new List<RoleElement>();

            string name = "Server" + iter.Key;
            List<LoginData.PlayerRoleData> list = iter.Value;
            foreach (LoginData.PlayerRoleData curRoleData in list)
            {
                RoleElement roleElem = new RoleElement();
                roleElem.type = curRoleData.type.ToString();
                roleElem.level = curRoleData.level.ToString();

                serverElem.list.Add(roleElem);
            }

            roleInfo.dic.Add(name, serverElem);
        }

        XmlHelper.XmlSerializeToFile(roleInfo, DataPath + account + RoleInfoFileLast, Encoding.UTF8);

#else
        XmlDocument xml = new XmlDocument();
        XmlElement root = xml.CreateElement("RoleInfo");
        xml.AppendChild(root);
        foreach(string keys in m_roleInfoMap.Keys)
        {
            XmlElement curServerElement = xml.CreateElement("Server"+keys);
            root.AppendChild(curServerElement);
            List<LoginData.PlayerRoleData> curList = m_roleInfoMap[keys];
            if(null != curList)
            {
                foreach (LoginData.PlayerRoleData curRoleData in curList)
                {
                    XmlElement curRoleElement = xml.CreateElement("Role");
                    curServerElement.AppendChild(curRoleElement);
                    XmlElement curTypeElement = xml.CreateElement("Type");
                    curTypeElement.InnerText = curRoleData.type.ToString();
                    curRoleElement.AppendChild(curTypeElement);
                    curTypeElement = xml.CreateElement("Level");
                    curTypeElement.InnerText = curRoleData.level.ToString();
                    curRoleElement.AppendChild(curTypeElement);
                }
            }
        }
        xml.Save(DataPath + account + RoleInfoFileLast);
#endif
    }

    // 从配置文件读取角色数据，保存在Map中
    public static Dictionary<string, List<LoginData.PlayerRoleData>> GetRoleInfoList(string account)
    {
        if (null != m_roleInfoMap && CurRoleInfoAccount == account)
        {
            return m_roleInfoMap;
        }

        m_roleInfoMap = new Dictionary<string, List<LoginData.PlayerRoleData>>();

#if UNITY_WP8
        RoleInfo roleInfo = XmlHelper.XmlDeserializeFromFile<RoleInfo>(DataPath + account + RoleInfoFileLast, Encoding.UTF8);
        if (null == roleInfo)
        {
            return m_roleInfoMap;
        }

        foreach (KeyValuePair<string, ServerElement> iter in roleInfo.dic)
        {
            List<LoginData.PlayerRoleData> curList = new List<LoginData.PlayerRoleData>();
            string serverID = iter.Key.Substring(6);
            m_roleInfoMap.Add(serverID, curList);

            foreach (RoleElement elem in iter.Value.list)
            {
                LoginData.PlayerRoleData curRoleData = new LoginData.PlayerRoleData(0, 0, null, 0, -1, -1, -1);
                string typeValue = elem.type;
                int.TryParse(typeValue, out curRoleData.type);
                string roleLevel = elem.level;
                int.TryParse(roleLevel, out curRoleData.level);

                curList.Add(curRoleData);
            }
        }
#else
        XmlDocument xml = new XmlDocument();
        try
        {
            xml.Load(DataPath +account + RoleInfoFileLast);
        }
        catch (System.Exception ex)
        {
            return m_roleInfoMap;
        }

        foreach (XmlElement elemServer in xml.FirstChild.ChildNodes)
        {
            List<LoginData.PlayerRoleData> curList = new List<LoginData.PlayerRoleData>();
            string serverID = elemServer.Name.Substring(6);
            m_roleInfoMap.Add(serverID, curList);
            
            foreach (XmlElement roleInfo in elemServer)
            {
                if (roleInfo.Name == "Role")
                {

                    LoginData.PlayerRoleData curRoleData = new LoginData.PlayerRoleData(0, 0, null, 0, -1, -1, -1,0);
                    foreach (XmlElement elemDetail in roleInfo)
                    {
                        if (elemDetail.Name == "Type")
                        {
                            string typeValue = elemDetail.InnerText;
                            int.TryParse(typeValue, out curRoleData.type);
                        }
                        else if (elemDetail.Name == "Level")
                        {
                            string roleLevel = elemDetail.InnerText;
                            int.TryParse(roleLevel, out curRoleData.level);
                        }
                    }

                    curList.Add(curRoleData);
                }
            }
        }
#endif

        return m_roleInfoMap;
    }

	public static void RemoveRoleInfo(string account)
	{
		if(File.Exists(DataPath + account + RoleInfoFileLast))
		{
			File.Delete(DataPath + account + RoleInfoFileLast);
		}
	}
    // 写入快速回复
    public static void AddFastReplyInfo(List<string> textList)
    {
        List<string> curList = GetFastReplyList();
        curList.Clear();
        foreach (string str in textList)
        {
            curList.Add(str);
        }

        if (!Directory.Exists(DataPath))
        {
            Directory.CreateDirectory(DataPath);
        }
        FileStream fs = new FileStream(DataPath + FastReplyFileName, FileMode.OpenOrCreate);
        StreamWriter sw = new StreamWriter(fs);

        for (int i = 0; i < textList.Count; i++ )
        {
            sw.WriteLine(textList[i]);
        }

        sw.Close();
    }

    // 从配置文件读取快速回复
    public static List<string> GetFastReplyList()
    {
        if (m_FastReplyList != null)
        {
            return m_FastReplyList;
        }

        m_FastReplyList = new List<string>();
        string filePath = DataPath + FastReplyFileName;
        if (!File.Exists(filePath))
        {
            return m_FastReplyList;
        }
        FileStream fs = new FileStream(filePath, FileMode.Open);
        StreamReader sr = new StreamReader(fs);

        string strLine = sr.ReadLine();        
        while (strLine != null)
        {
            m_FastReplyList.Add(strLine);
            strLine = sr.ReadLine();
        }
        fs.Close();
        return m_FastReplyList;
    }

#if UNITY_WP8
    public class MissionElement : IXmlSerializable
    {
        public string state { get; set; }
        public string param0 { get; set; }

        public System.Xml.Schema.XmlSchema GetSchema() { return null; }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            reader.Read();
            reader.ReadStartElement("State");
            state = reader.ReadContentAsString();
            reader.ReadEndElement();

            reader.ReadStartElement("Param0");
            param0 = reader.ReadContentAsString();
            reader.ReadEndElement();
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteElementString("State", state);
            writer.WriteElementString("Param0", param0);
        }
    }

    public class PlayerGuidElement : IXmlSerializable
    {
        public Dictionary<string, MissionElement> dic { get; set; }


        public System.Xml.Schema.XmlSchema GetSchema() { return null; }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            dic = new Dictionary<string, MissionElement>();
            while (reader.MoveToContent() == XmlNodeType.Element)
            {
                string name = reader.LocalName;
                MissionElement element = new MissionElement();
                element.ReadXml(reader);
                dic.Add(name, element);
                reader.Read();
            }
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            foreach (KeyValuePair<string, MissionElement> iter in dic)
            {
                writer.WriteStartElement(iter.Key);
                iter.Value.WriteXml(writer);
                writer.WriteEndElement();
            }
        }
    }

    public class ClientMissionData : IXmlSerializable
    {
        public Dictionary<string, PlayerGuidElement> dic { get; set; }

        public System.Xml.Schema.XmlSchema GetSchema() { return null; }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            dic = new Dictionary<string, PlayerGuidElement>();

            if (reader.MoveToContent() == XmlNodeType.Element && "ClientMissionData" == reader.LocalName)
            {
                reader.Read();
                while (reader.MoveToContent() == XmlNodeType.Element)
                {
                    string name = reader.LocalName;
                    PlayerGuidElement elem = new PlayerGuidElement();

                    reader.Read();
                    elem.ReadXml(reader);
                    dic.Add(name, elem);
                    reader.Read();
                }
            }
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            foreach (KeyValuePair<string, PlayerGuidElement> iter in dic)
            {
                writer.WriteStartElement(iter.Key);
                iter.Value.WriteXml(writer);
                writer.WriteEndElement();
            }
        }
    }

#endif

    // 将客户端任务 信息保存
    public static void AddClientMission(string strGuid, Games.Mission.CurOwnMission oClientMission)
    {
        m_MissionClientData = GetClientMissionData();

        if (m_MissionClientData.ContainsKey(strGuid))
        {
            bool bIsHaveCurMission = false;
            for (int i = 0; i < m_MissionClientData[strGuid].Count;i++ )
            {
                if (m_MissionClientData[strGuid][i].m_nMissionID == oClientMission.m_nMissionID)
                {
                    bIsHaveCurMission = true;
                    m_MissionClientData[strGuid][i] = oClientMission;
                    break;
                }
            }

            if (false == bIsHaveCurMission)
            {
                m_MissionClientData[strGuid].Add(oClientMission);
            }
        }
        else
        {
            List<Games.Mission.CurOwnMission> ClientMissionList = new List<Games.Mission.CurOwnMission>();
            ClientMissionList.Add(oClientMission);
            m_MissionClientData.Add(strGuid, ClientMissionList);
        }

        if (!Directory.Exists(DataPath))
        {
            Directory.CreateDirectory(DataPath);
        }

#if UNITY_WP8
        ClientMissionData clientMissionData = new ClientMissionData();
        clientMissionData.dic = new Dictionary<string, PlayerGuidElement>();

        foreach (string keys in m_MissionClientData.Keys)
        {
            PlayerGuidElement playerGuidElem = new PlayerGuidElement();
            playerGuidElem.dic = new Dictionary<string, MissionElement>();
            string playerGuidName = "PlayerGuid" + keys;

            foreach (Games.Mission.CurOwnMission oMission in m_MissionClientData[keys])
            {
                MissionElement missionElem = new MissionElement();
                string missionName = "Mission" + oMission.m_nMissionID;
                missionElem.state = oMission.m_yStatus.ToString();
                missionElem.param0 = oMission.m_nParam[0].ToString();

                playerGuidElem.dic.Add(missionName, missionElem);
            }

            clientMissionData.dic.Add(playerGuidName, playerGuidElem);
        }

        XmlHelper.XmlSerializeToFile(clientMissionData, DataPath + MissionClientName, Encoding.UTF8);

#else

        XmlDocument xml = new XmlDocument();
        XmlElement root = xml.CreateElement("ClientMissionData");
        xml.AppendChild(root);
        foreach (string keys in m_MissionClientData.Keys)
        {
            XmlElement curUserElement = xml.CreateElement("PlayerGuid" + keys);
            root.AppendChild(curUserElement);

            foreach (Games.Mission.CurOwnMission oMission in m_MissionClientData[keys])
            {
                XmlElement curMissionElement = xml.CreateElement("Mission" + oMission.m_nMissionID);
                curUserElement.AppendChild(curMissionElement);
                XmlElement curStateElement = xml.CreateElement("State");
                curStateElement.InnerText = oMission.m_yStatus.ToString();
                curMissionElement.AppendChild(curStateElement);
                XmlElement curParam0Element = xml.CreateElement("Param0");
                curParam0Element.InnerText = oMission.m_nParam[0].ToString();
                curMissionElement.AppendChild(curParam0Element);
            }
        }
        xml.Save(DataPath + MissionClientName);
#endif
    }

    // 删除 客户端任务
    public static void DelClientMission(string strGuid, int nMissionID)
    {
        m_MissionClientData = GetClientMissionData();

        if (!m_MissionClientData.ContainsKey(strGuid))
        {
            return;
        }

        int nIndex = -1;
        for (int i = 0; i < m_MissionClientData[strGuid].Count;i++ )
        {
            if (nMissionID == m_MissionClientData[strGuid][i].m_nMissionID)
            {
                nIndex = i;
                break;
            }
        }
        if (nIndex < 0 || nIndex >= m_MissionClientData[strGuid].Count)
        {
            return;
        }

        // 删除
        m_MissionClientData[strGuid].RemoveAt(nIndex);

        if (m_MissionClientData[strGuid].Count <= 0)
        {
            m_MissionClientData.Remove(strGuid);
        }

        if (!Directory.Exists(DataPath))
        {
            Directory.CreateDirectory(DataPath);
        }

#if UNITY_WP8
        ClientMissionData clientMissionData = new ClientMissionData();
        clientMissionData.dic = new Dictionary<string, PlayerGuidElement>();

        foreach (string keys in m_MissionClientData.Keys)
        {
            PlayerGuidElement playerGuidElem = new PlayerGuidElement();
            playerGuidElem.dic = new Dictionary<string, MissionElement>();
            string playerGuidName = "PlayerGuid" + keys;

            foreach (Games.Mission.CurOwnMission oMission in m_MissionClientData[keys])
            {
                MissionElement missionElem = new MissionElement();
                string missionName = "Mission" + oMission.m_nMissionID;
                missionElem.state = oMission.m_yStatus.ToString();
                missionElem.param0 = oMission.m_nParam[0].ToString();

                playerGuidElem.dic.Add(missionName, missionElem);
            }

            clientMissionData.dic.Add(playerGuidName, playerGuidElem);
        }

        XmlHelper.XmlSerializeToFile(clientMissionData, DataPath + MissionClientName, Encoding.UTF8);

#else

        XmlDocument xml = new XmlDocument();
        XmlElement root = xml.CreateElement("ClientMissionData");
        xml.AppendChild(root);
        foreach (string keys in m_MissionClientData.Keys)
        {
            XmlElement curUserElement = xml.CreateElement("PlayerGuid" + keys);
            root.AppendChild(curUserElement);

            foreach (Games.Mission.CurOwnMission oMission in m_MissionClientData[keys])
            {
                XmlElement curMissionElement = xml.CreateElement("Mission" + oMission.m_nMissionID);
                curUserElement.AppendChild(curMissionElement);
                XmlElement curStateElement = xml.CreateElement("State");
                curStateElement.InnerText = oMission.m_yStatus.ToString();
                curMissionElement.AppendChild(curStateElement);
                XmlElement curParam0Element = xml.CreateElement("Param0");
                curParam0Element.InnerText = oMission.m_nParam[0].ToString();
                curMissionElement.AppendChild(curParam0Element);
            }
        }
        xml.Save(DataPath + MissionClientName);
#endif
    }

    // 从配置文件读取账号数据，保存在Map中
    public static Dictionary<string, List<Games.Mission.CurOwnMission>> GetClientMissionData()
    {
        if (null != m_MissionClientData)
        {
            return m_MissionClientData;
        }

        m_MissionClientData = new Dictionary<string, List<Games.Mission.CurOwnMission>>();

#if UNITY_WP8
        ClientMissionData clientMissionData = XmlHelper.XmlDeserializeFromFile<ClientMissionData>(DataPath + MissionClientName, Encoding.UTF8);

        if (null == clientMissionData)
        {
            return m_MissionClientData;
        }

        foreach (KeyValuePair<string, PlayerGuidElement> playerGuidIter in clientMissionData.dic)
        {
            List<Games.Mission.CurOwnMission> curMissionList = new List<Games.Mission.CurOwnMission>();
            m_MissionClientData.Add(playerGuidIter.Key.Substring(10), curMissionList);

            foreach (KeyValuePair<string, MissionElement> missionIter in playerGuidIter.Value.dic)
            {
                Games.Mission.CurOwnMission oMission = new Games.Mission.CurOwnMission();
                oMission.CleanUp();

                curMissionList.Add(oMission);

                string strMissionID = missionIter.Key.Substring(7);
                oMission.m_nMissionID = int.Parse(strMissionID);

                string strState = missionIter.Value.state;
                oMission.m_yStatus = byte.Parse(strState);

                string strParam0 = missionIter.Value.param0;
                oMission.m_nParam[0] = int.Parse(strParam0);
            }
        }

#else

        XmlDocument xml = new XmlDocument();
        try
        {
            xml.Load(DataPath + MissionClientName);
        }
        catch (System.Exception ex)
        {
            return m_MissionClientData;
        }

        foreach (XmlElement curUserElement in xml.FirstChild.ChildNodes)
        {
            List<Games.Mission.CurOwnMission> curMissionList = new List<Games.Mission.CurOwnMission>();
            m_MissionClientData.Add(curUserElement.Name.Substring(10), curMissionList);

            foreach (XmlElement curMissionElement in curUserElement)
            {
                Games.Mission.CurOwnMission oMission = new Games.Mission.CurOwnMission();
                oMission.CleanUp();
                curMissionList.Add(oMission);
                string strMissionID = curMissionElement.Name.Substring(7);
                oMission.m_nMissionID = int.Parse(strMissionID);

                foreach (XmlElement curElement in curMissionElement)
                {
                    if (curElement.Name == "State")
                    {
                        string strState = curElement.InnerText;
                        oMission.m_yStatus = byte.Parse(strState);
                    }
                    else if (curElement.Name == "Param0")
                    {
                        string strParam0 = curElement.InnerText;
                        oMission.m_nParam[0] = int.Parse(strParam0);
                    }
                }
            }
        }
#endif

        return m_MissionClientData;
    }
    
#if UNITY_WP8
    public class AutoElement : IXmlSerializable
    {
        public string AutoPickUp { get; set; }
        public string AutoTaem { get; set; }
        public string AutoHpPercent { get; set; }
        public string AutoHpID { get; set; }
        public string AutoMpPercent { get; set; }
        public string AutoMpID { get; set; }
        public string AutoBuyDrug { get; set; }
        public string AutoNotice { get; set; }
        public string AutoIsSelectDrug { get; set; }
        public string AutoEquipGuid { get; set; }

        public System.Xml.Schema.XmlSchema GetSchema() { return null; }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            reader.Read();
            reader.ReadStartElement("AutoPickUp");
            AutoPickUp = reader.ReadContentAsString();
            reader.ReadEndElement();

            reader.ReadStartElement("AutoTaem");
            AutoTaem = reader.ReadContentAsString();
            reader.ReadEndElement();

            reader.ReadStartElement("AutoHpPercent");
            AutoHpPercent = reader.ReadContentAsString();
            reader.ReadEndElement();

            reader.ReadStartElement("AutoHpID");
            AutoHpID = reader.ReadContentAsString();
            reader.ReadEndElement();

            reader.ReadStartElement("AutoMpPercent");
            AutoMpPercent = reader.ReadContentAsString();
            reader.ReadEndElement();

            reader.ReadStartElement("AutoMpID");
            AutoMpID = reader.ReadContentAsString();
            reader.ReadEndElement();

            reader.ReadStartElement("AutoBuyDrug");
            AutoBuyDrug = reader.ReadContentAsString();
            reader.ReadEndElement();

            reader.ReadStartElement("AutoNotice");
            AutoNotice = reader.ReadContentAsString();
            reader.ReadEndElement();

            reader.ReadStartElement("AutoIsSelectDrug");
            AutoIsSelectDrug = reader.ReadContentAsString();
            reader.ReadEndElement();

            reader.ReadStartElement("AutoEquipGuid");
            AutoEquipGuid = reader.ReadContentAsString();
            reader.ReadEndElement();
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteElementString("AutoPickUp", AutoPickUp);
            writer.WriteElementString("AutoTaem", AutoTaem);
            writer.WriteElementString("AutoHpPercent", AutoHpPercent);
            writer.WriteElementString("AutoHpID", AutoHpID);
            writer.WriteElementString("AutoMpPercent", AutoMpPercent);
            writer.WriteElementString("AutoMpID", AutoMpID);
            writer.WriteElementString("AutoBuyDrug", AutoBuyDrug);
            writer.WriteElementString("AutoNotice", AutoNotice);
            writer.WriteElementString("AutoIsSelectDrug", AutoIsSelectDrug);
            writer.WriteElementString("AutoEquipGuid", AutoEquipGuid);

        }
    }


    public class AutoInfo : IXmlSerializable
    {
        public Dictionary<string, AutoElement> dic { get; set; }

        public System.Xml.Schema.XmlSchema GetSchema() { return null; }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            dic = new Dictionary<string, AutoElement>();

            if (reader.MoveToContent() == XmlNodeType.Element && "AutoInfo" == reader.LocalName)
            {
                reader.Read();                                                      
                while (reader.MoveToContent() == XmlNodeType.Element)
                {
                    string name = reader.LocalName;

                    reader.Read();             
                    if (reader.MoveToContent() == XmlNodeType.Element && "Auto" == reader.LocalName)
                    {
                        AutoElement elem = new AutoElement();
                        elem.ReadXml(reader);
                        dic.Add(name, elem);
                        reader.Read();
                    }

                    reader.Read();
                }
            }
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            foreach (KeyValuePair<string, AutoElement> elem in dic)
            {
                writer.WriteStartElement(elem.Key);
                writer.WriteStartElement("Auto");
                elem.Value.WriteXml(writer);
                writer.WriteEndElement();
                writer.WriteEndElement();
            }
        }

    }


#endif


	// 将角色特殊设置信息保存在XML文件中
	public static void AddPlayerSpecial(string sGUID, PlayerSpecialData curData)
	{
		m_SpecialDataMap = GetPlayerSpecialList();
		
		if (m_SpecialDataMap.ContainsKey(sGUID))
		{
			m_SpecialDataMap.Remove(sGUID);
		}
		
		m_SpecialDataMap.Add(sGUID, curData);
		
		if (!Directory.Exists(DataPath))
		{
			Directory.CreateDirectory(DataPath);
		}
		//AutoInfo
		#if UNITY_WP8
		AutoInfo autoInfo = new AutoInfo();
		autoInfo.dic = new Dictionary<string, AutoElement>();
		
		foreach (string keys in m_AutoConfigMap.Keys)
		{
			string name = "GUID" + keys;
			PlayerAuto curList = m_AutoConfigMap[keys];
			
			if (curList != null)
			{
				AutoElement elem = new AutoElement();
				elem.AutoPickUp = curList.AutoPickUp.ToString();
				elem.AutoTaem = curList.AutoTaem.ToString();
				elem.AutoHpPercent = curList.AutoHpPercent.ToString();
				elem.AutoHpID = curList.AutoHpID.ToString();
				elem.AutoMpPercent = curList.AutoMpPercent.ToString();
				elem.AutoMpID = curList.AutoMpID.ToString();
				elem.AutoBuyDrug = curList.AutoBuyDrug.ToString();
				elem.AutoNotice = curList.AutoNotice.ToString();
				elem.AutoIsSelectDrug = curList.AutoIsSelectDrug.ToString();
				elem.AutoEquipGuid = curList.AutoEquipGuid.ToString();
				
				autoInfo.dic.Add(name, elem);
			}
		}
		
		XmlHelper.XmlSerializeToFile(autoInfo, DataPath + AutoConfigFileName, Encoding.UTF8);
		#else
		XmlDocument xml = new XmlDocument();
		XmlElement root = xml.CreateElement("SpecialInfo");
		xml.AppendChild(root);
		foreach (string keys in m_SpecialDataMap.Keys)
		{
			XmlElement curGUIDElement = xml.CreateElement("GUID" + keys);
			root.AppendChild(curGUIDElement);
			PlayerAuto curList = m_AutoConfigMap[keys];
			if (null != curList)
			{
				//foreach (PlayerAuto curAutoData in curList)
				{
					XmlElement curAutoElement = xml.CreateElement("Special");
					curGUIDElement.AppendChild(curAutoElement);
					
					//                     XmlElement curAutoOpenElement = xml.CreateElement("AutoOpen");
					//                     curAutoOpenElement.InnerText = curList.AutoFightOpenToggle.ToString();
					//                     curAutoElement.AppendChild(curAutoOpenElement);
					
					XmlElement curAutoPickUpElement = xml.CreateElement("IsFirstYeXiDaYing");
					curAutoPickUpElement.InnerText = curList.AutoPickUp.ToString();
					curAutoElement.AppendChild(curAutoPickUpElement);
					

				}
			}
		}
		xml.Save(DataPath + RoleSpecialDataFileName);
		#endif
	}
	// 从配置文件读取角色个人设置数据，保存在Map中
	public static Dictionary<string, PlayerSpecialData> GetPlayerSpecialList()
	{
		if (null != m_SpecialDataMap)
		{
			return m_SpecialDataMap;
		}
		
		m_SpecialDataMap = new Dictionary<string, PlayerSpecialData>();
		
		#if UNITY_WP8
		AutoInfo autoInfo = XmlHelper.XmlDeserializeFromFile<AutoInfo>(DataPath + AutoConfigFileName, Encoding.UTF8);
		if (null == autoInfo)
		{
			return m_AutoConfigMap;
		}
		
		foreach (KeyValuePair<string, AutoElement> iter in autoInfo.dic)
		{
			string guid = iter.Key.Substring(4);
			AutoElement elem = iter.Value;
			
			PlayerAuto curAutoData = new PlayerAuto(0, false, 0, 0, false, 0, -1, -1, false, GlobeVar.INVALID_GUID);
			int.TryParse(elem.AutoPickUp, out curAutoData.AutoPickUp);
			int.TryParse(elem.AutoTaem, out curAutoData.AutoTaem);
			int.TryParse(elem.AutoHpPercent, out curAutoData.AutoHpPercent);
			int.TryParse(elem.AutoHpID, out curAutoData.AutoHpID);
			int.TryParse(elem.AutoMpPercent, out curAutoData.AutoMpPercent);
			int.TryParse(elem.AutoMpID, out curAutoData.AutoMpID);
			int.TryParse(elem.AutoBuyDrug, out curAutoData.AutoBuyDrug);
			int.TryParse(elem.AutoNotice, out curAutoData.AutoNotice);
			int.TryParse(elem.AutoIsSelectDrug, out curAutoData.AutoIsSelectDrug);
			ulong.TryParse(elem.AutoEquipGuid, out curAutoData.AutoEquipGuid);
			
			m_AutoConfigMap.Add(guid, curAutoData);
		}
		
		#else
		XmlDocument xml = new XmlDocument();
		try
		{
			xml.Load(DataPath + RoleSpecialDataFileName);
		}
		catch (System.Exception ex)
		{
			return m_SpecialDataMap;
		}
		
		foreach (XmlElement elemGUID in xml.FirstChild.ChildNodes)
		{
			//PlayerAuto curList;
			string guid = elemGUID.Name.Substring(4);
			//m_AutoConfigMap.Add(guid, curList);
			
			foreach (XmlElement autoInfo in elemGUID)
			{
				if (autoInfo.Name == "Special")
				{
					PlayerSpecialData curData = new PlayerSpecialData(false);
					foreach (XmlElement elemDetail in autoInfo)
					{
						if (elemDetail.Name == "IsFirstYeXiDaYing")
						{
							string typeValue = elemDetail.InnerText;
							bool.TryParse(typeValue, out curData.m_bisFirstYeXiDaYing);
						}
				                   
					}
					m_SpecialDataMap.Add(guid, curData);
				}
			}
		}
		#endif
		return m_SpecialDataMap;
	}
	// 将角色个人设置信息保存在XML文件中
	public static void AddPlayerAuto(string sGUID, PlayerAuto curData)
	{
		m_AutoConfigMap = GetPlayerAutoList();
		
		if (m_AutoConfigMap.ContainsKey(sGUID))
		{
			m_AutoConfigMap.Remove(sGUID);
		}
		
		m_AutoConfigMap.Add(sGUID, curData);
		
		if (!Directory.Exists(DataPath))
		{
			Directory.CreateDirectory(DataPath);
		}
		
		#if UNITY_WP8
		AutoInfo autoInfo = new AutoInfo();
		autoInfo.dic = new Dictionary<string, AutoElement>();
		
		foreach (string keys in m_AutoConfigMap.Keys)
		{
			string name = "GUID" + keys;
			PlayerAuto curList = m_AutoConfigMap[keys];
			
			if (curList != null)
			{
				AutoElement elem = new AutoElement();
				elem.AutoPickUp = curList.AutoPickUp.ToString();
				elem.AutoTaem = curList.AutoTaem.ToString();
				elem.AutoHpPercent = curList.AutoHpPercent.ToString();
				elem.AutoHpID = curList.AutoHpID.ToString();
				elem.AutoMpPercent = curList.AutoMpPercent.ToString();
				elem.AutoMpID = curList.AutoMpID.ToString();
				elem.AutoBuyDrug = curList.AutoBuyDrug.ToString();
				elem.AutoNotice = curList.AutoNotice.ToString();
				elem.AutoIsSelectDrug = curList.AutoIsSelectDrug.ToString();
				elem.AutoEquipGuid = curList.AutoEquipGuid.ToString();
				
				autoInfo.dic.Add(name, elem);
			}
		}
		
		XmlHelper.XmlSerializeToFile(autoInfo, DataPath + AutoConfigFileName, Encoding.UTF8);
		#else
		XmlDocument xml = new XmlDocument();
        XmlElement root = xml.CreateElement("AutoInfo");
        xml.AppendChild(root);
        foreach (string keys in m_AutoConfigMap.Keys)
        {
            XmlElement curGUIDElement = xml.CreateElement("GUID" + keys);
            root.AppendChild(curGUIDElement);
            PlayerAuto curList = m_AutoConfigMap[keys];
            if (null != curList)
            {
                //foreach (PlayerAuto curAutoData in curList)
                {
                    XmlElement curAutoElement = xml.CreateElement("Auto");
                    curGUIDElement.AppendChild(curAutoElement);

//                     XmlElement curAutoOpenElement = xml.CreateElement("AutoOpen");
//                     curAutoOpenElement.InnerText = curList.AutoFightOpenToggle.ToString();
//                     curAutoElement.AppendChild(curAutoOpenElement);

                    XmlElement curAutoPickUpElement = xml.CreateElement("AutoPickUp");
                    curAutoPickUpElement.InnerText = curList.AutoPickUp.ToString();
                    curAutoElement.AppendChild(curAutoPickUpElement);

                    XmlElement curAutoTaemElement = xml.CreateElement("AutoTaem");
                    curAutoTaemElement.InnerText = curList.AutoTaem.ToString();
                    curAutoElement.AppendChild(curAutoTaemElement);

					XmlElement curAutoAcceptTeamElement = xml.CreateElement("AutoAcceptTeam");
					curAutoAcceptTeamElement.InnerText = curList.AutoAcceptTeam.ToString();
					curAutoElement.AppendChild(curAutoAcceptTeamElement);

                    XmlElement curAutoHpPercentElement = xml.CreateElement("AutoHpPercent");
                    curAutoHpPercentElement.InnerText = curList.AutoHpPercent.ToString();
                    curAutoElement.AppendChild(curAutoHpPercentElement);

                    XmlElement curAutoHpIDElement = xml.CreateElement("AutoHpID");
                    curAutoHpIDElement.InnerText = curList.AutoHpID.ToString();
                    curAutoElement.AppendChild(curAutoHpIDElement);

                    XmlElement curAutoMpPercentElement = xml.CreateElement("AutoMpPercent");
                    curAutoMpPercentElement.InnerText = curList.AutoMpPercent.ToString();
                    curAutoElement.AppendChild(curAutoMpPercentElement);

                    XmlElement curAutoMpIDElement = xml.CreateElement("AutoMpID");
                    curAutoMpIDElement.InnerText = curList.AutoMpID.ToString();
                    curAutoElement.AppendChild(curAutoMpIDElement);

                    XmlElement curAutoBuyDrugElement = xml.CreateElement("AutoBuyDrug");
                    curAutoBuyDrugElement.InnerText = curList.AutoBuyDrug.ToString();
                    curAutoElement.AppendChild(curAutoBuyDrugElement);

//                     XmlElement curAutoRadiusElement = xml.CreateElement("AutoRadius");
//                     curAutoRadiusElement.InnerText = curList.AutoRadius.ToString();
//                     curAutoElement.AppendChild(curAutoRadiusElement);

                    XmlElement curAutoNoticeElement = xml.CreateElement("AutoNotice");
                    curAutoNoticeElement.InnerText = curList.AutoNotice.ToString();
                    curAutoElement.AppendChild(curAutoNoticeElement);

                    XmlElement curAutoIsSelectDrug = xml.CreateElement("AutoIsSelectDrug");
                    curAutoIsSelectDrug.InnerText = curList.AutoIsSelectDrug.ToString();
                    curAutoElement.AppendChild(curAutoIsSelectDrug);

                    XmlElement curAutoEquipGuid = xml.CreateElement("AutoEquipGuid");
                    curAutoEquipGuid.InnerText = curList.AutoEquipGuid.ToString();
                    curAutoElement.AppendChild(curAutoEquipGuid);
					
					XmlElement curAutoSkill = xml.CreateElement("AutoSkill");
					for(int i=0;i<10;i++)
					{
						XmlElement curAutoSkillindex = xml.CreateElement("AutoSkillIndex");
						curAutoSkillindex.InnerText = curData.AutoSkills[i].ToString();
						curAutoSkill.AppendChild(curAutoSkillindex);
					}
					curAutoElement.AppendChild(curAutoSkill);
                }
            }
        }
        xml.Save(DataPath + AutoConfigFileName);
#endif
    }

    // 从配置文件读取角色个人设置数据，保存在Map中
    public static Dictionary<string, PlayerAuto> GetPlayerAutoList()
    {
        if (null != m_AutoConfigMap)
        {
            return m_AutoConfigMap;
        }

        m_AutoConfigMap = new Dictionary<string, PlayerAuto>();

#if UNITY_WP8
        AutoInfo autoInfo = XmlHelper.XmlDeserializeFromFile<AutoInfo>(DataPath + AutoConfigFileName, Encoding.UTF8);
        if (null == autoInfo)
        {
            return m_AutoConfigMap;
        }

        foreach (KeyValuePair<string, AutoElement> iter in autoInfo.dic)
        {
            string guid = iter.Key.Substring(4);
            AutoElement elem = iter.Value;

            PlayerAuto curAutoData = new PlayerAuto(0, false, 0, 0, false, 0, -1, -1, false, GlobeVar.INVALID_GUID);
            int.TryParse(elem.AutoPickUp, out curAutoData.AutoPickUp);
            int.TryParse(elem.AutoTaem, out curAutoData.AutoTaem);
            int.TryParse(elem.AutoHpPercent, out curAutoData.AutoHpPercent);
            int.TryParse(elem.AutoHpID, out curAutoData.AutoHpID);
            int.TryParse(elem.AutoMpPercent, out curAutoData.AutoMpPercent);
            int.TryParse(elem.AutoMpID, out curAutoData.AutoMpID);
            int.TryParse(elem.AutoBuyDrug, out curAutoData.AutoBuyDrug);
            int.TryParse(elem.AutoNotice, out curAutoData.AutoNotice);
            int.TryParse(elem.AutoIsSelectDrug, out curAutoData.AutoIsSelectDrug);
            ulong.TryParse(elem.AutoEquipGuid, out curAutoData.AutoEquipGuid);

            m_AutoConfigMap.Add(guid, curAutoData);
        }

#else
        XmlDocument xml = new XmlDocument();
        try
        {
            xml.Load(DataPath + AutoConfigFileName);
        }
        catch (System.Exception ex)
        {
            return m_AutoConfigMap;
        }

        foreach (XmlElement elemGUID in xml.FirstChild.ChildNodes)
        {
            //PlayerAuto curList;
            string guid = elemGUID.Name.Substring(4);
            //m_AutoConfigMap.Add(guid, curList);

            foreach (XmlElement autoInfo in elemGUID)
            {
                if (autoInfo.Name == "Auto")
                {
					bool [] autoskill=new bool[10];
					PlayerAuto curAutoData = new PlayerAuto(0, false, false,0, 0, false, 0, -1, -1, false, GlobeVar.INVALID_GUID,autoskill);
                    foreach (XmlElement elemDetail in autoInfo)
                    {
                        if (elemDetail.Name == "AutoPickUp")
                        {
                            string typeValue = elemDetail.InnerText;
                            int.TryParse(typeValue, out curAutoData.AutoPickUp);
                        }
                        else if (elemDetail.Name == "AutoTaem")
                        {
                            string typeValue = elemDetail.InnerText;
                            int.TryParse(typeValue, out curAutoData.AutoTaem);
                        }
						else if (elemDetail.Name == "AutoAcceptTeam")
						{
							string typeValue = elemDetail.InnerText;
							int.TryParse(typeValue, out curAutoData.AutoAcceptTeam);
						}
                        else if (elemDetail.Name == "AutoHpPercent")
                        {
                            string typeValue = elemDetail.InnerText;
                            int.TryParse(typeValue, out curAutoData.AutoHpPercent);
                        }
                        else if (elemDetail.Name == "AutoHpID")
                        {
                            string typeValue = elemDetail.InnerText;
                            int.TryParse(typeValue, out curAutoData.AutoHpID);
                        }
                        else if (elemDetail.Name == "AutoMpPercent")
                        {
                            string typeValue = elemDetail.InnerText;
                            int.TryParse(typeValue, out curAutoData.AutoMpPercent);
                        }
                        else if (elemDetail.Name == "AutoMpID")
                        {
                            string typeValue = elemDetail.InnerText;
                            int.TryParse(typeValue, out curAutoData.AutoMpID);
                        }
                        else if (elemDetail.Name == "AutoBuyDrug")
                        {
                            string typeValue = elemDetail.InnerText;
                            int.TryParse(typeValue, out curAutoData.AutoBuyDrug);
                        }
//                         else if (elemDetail.Name == "AutoRadius")
//                         {
//                             string typeValue = elemDetail.InnerText;
//                             int.TryParse(typeValue, out curAutoData.AutoRadius);
//                         }
                        else if (elemDetail.Name == "AutoNotice")
                        {
                            string typeValue = elemDetail.InnerText;
                            int.TryParse(typeValue, out curAutoData.AutoNotice);
                        }
                        else if (elemDetail.Name == "AutoIsSelectDrug")
                        {
                            string typeValue = elemDetail.InnerText;
                            int.TryParse(typeValue, out curAutoData.AutoIsSelectDrug);
                        }
                        else if (elemDetail.Name == "AutoEquipGuid")
                        {
                            string typeValue = elemDetail.InnerText;
                            ulong.TryParse(typeValue, out curAutoData.AutoEquipGuid);
                        }
						else if (elemDetail.Name == "AutoSkill")
						{
							int i=0;
							foreach (XmlElement elemskill in elemDetail)
							{
								string typeValue = elemskill.InnerText;
								int.TryParse(typeValue, out curAutoData.AutoSkills[i++]);
							}

						}
                        // curList.Add(curAutoData);                       
                    }
                    m_AutoConfigMap.Add(guid, curAutoData);
                }
            }
        }
#endif
        return m_AutoConfigMap;
    }

    // 将角色个人设置信息保存在XML文件中
    public static void AddRestaurantConfig(string sGUID, RestaurantConfigData curData)
    {
        m_RestaurantConfigMap = GetRestaurantConfigList();
        if (null == m_RestaurantConfigMap)
        {
            return;
        }
        if (m_RestaurantConfigMap.ContainsKey(sGUID))
        {
            m_RestaurantConfigMap.Remove(sGUID);
        }

        m_RestaurantConfigMap.Add(sGUID, curData);

        if (!Directory.Exists(DataPath))
        {
            Directory.CreateDirectory(DataPath);
        }

#if UNITY_WP8
        
#else
        XmlDocument xml = new XmlDocument();
        XmlElement root = xml.CreateElement("RestaurantInfo");
        xml.AppendChild(root);
        foreach (string keys in m_RestaurantConfigMap.Keys)
        {
            XmlElement curGUIDElement = xml.CreateElement("GUID" + keys);
            root.AppendChild(curGUIDElement);
            RestaurantConfigData curList = m_RestaurantConfigMap[keys];
            if (null != curList)
            {

                XmlElement curRestaurantElement = xml.CreateElement("Restaurant");
                curGUIDElement.AppendChild(curRestaurantElement);
               

                XmlElement curRestaurantFilterExpElement = xml.CreateElement("keyRestaurantFilterExp");
                curRestaurantFilterExpElement.InnerText = curList.RestaurantFilterExp.ToString();
                curRestaurantElement.AppendChild(curRestaurantFilterExpElement);

                XmlElement curRestaurantFilterCoinElement = xml.CreateElement("keyRestaurantFilterCoin");
                curRestaurantFilterCoinElement.InnerText = curList.RestaurantFilterCoin.ToString();
                curRestaurantElement.AppendChild(curRestaurantFilterCoinElement);

                XmlElement curRestaurantFilterMeterialElement = xml.CreateElement("keyRestaurantFilterMeterial");
                curRestaurantFilterMeterialElement.InnerText = curList.RestaurantFilterMeterial.ToString();
                curRestaurantElement.AppendChild(curRestaurantFilterMeterialElement);

                for ( int i = 1 ; i <= RestaurantData.FoodLevelMax; i++)
                {
                    string Elementkeystr = string.Format("keyRestaurantFilterLv{0}", i);
                    XmlElement curRestaurantFilterLvElement = xml.CreateElement(Elementkeystr);
                    curRestaurantFilterLvElement.InnerText = curList.GetRestaurantFilterLv(i).ToString();
                    curRestaurantElement.AppendChild(curRestaurantFilterLvElement);        
                }                                                   
            }
        }
        xml.Save(DataPath + RestaurantConfigFileName);
#endif
    }

    // 从配置文件读取角色个人设置数据，保存在Map中
    public static Dictionary<string, RestaurantConfigData> GetRestaurantConfigList()
    {
        if (m_RestaurantConfigMap != null)
        {
            return m_RestaurantConfigMap;
        }

        m_RestaurantConfigMap = new Dictionary<string, RestaurantConfigData>();

#if UNITY_WP8
       
#else
        XmlDocument xml = new XmlDocument();
        try
        {
            xml.Load(DataPath + RestaurantConfigFileName);
        }
        catch (System.Exception ex)
        {
            return m_RestaurantConfigMap;
        }

        foreach (XmlElement elemGUID in xml.FirstChild.ChildNodes)
        {
            string guid = elemGUID.Name.Substring(4);
            foreach (XmlElement RestaurantElem in elemGUID)
            {
                if (RestaurantElem.Name == "Restaurant")
                {
                    RestaurantConfigData curRrestaurantData = new RestaurantConfigData();
                    foreach (XmlElement elemDetail in RestaurantElem)
                    {
                        if (elemDetail.Name == "keyRestaurantFilterExp")
                        {
                            int nValue = 1;
                            string typeValue = elemDetail.InnerText;
                            int.TryParse(typeValue, out nValue);
                            curRrestaurantData.RestaurantFilterExp = nValue;
                        }
                        else if (elemDetail.Name == "keyRestaurantFilterCoin")
                        {
                            int nValue = 1;
                            string typeValue = elemDetail.InnerText;
                            int.TryParse(typeValue, out nValue);
                            curRrestaurantData.RestaurantFilterCoin = nValue;
                        }
                        else if (elemDetail.Name == "keyRestaurantFilterMeterial")
                        {
                            int nValue = 1;
                            string typeValue = elemDetail.InnerText;
                            int.TryParse(typeValue, out nValue);
                            curRrestaurantData.RestaurantFilterMeterial = nValue;
                        }
                        else                        
                        {
                            for ( int i = 1 ; i <= RestaurantData.FoodLevelMax; i++)
                            {
                                string Elementkeystr = string.Format("keyRestaurantFilterLv{0}", i);
                                if (elemDetail.Name == Elementkeystr)
                                {
                                    int nValue = 1;
                                    string typeValue = elemDetail.InnerText;
                                    int.TryParse(typeValue, out nValue);
                                    curRrestaurantData.SetRestaurantFilterLv(i, nValue);
                                }
                            }                          
                        }                              
                    }
                    m_RestaurantConfigMap.Add(guid, curRrestaurantData);
                }
            }
        }
#endif
        return m_RestaurantConfigMap;
    }



#if UNITY_WP8
    public class SkillIndexElement : IXmlSerializable
    {
        public string SkillIndex { get; set; }

        public System.Xml.Schema.XmlSchema GetSchema() { return null; }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            reader.Read();
            SkillIndex = reader.ReadContentAsString();
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteElementString("SkillIndex", SkillIndex);
        }
    }

    public class GUIDElement : IXmlSerializable
    {
        public List<SkillIndexElement> list { get; set; }

        public System.Xml.Schema.XmlSchema GetSchema() { return null; }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            list = new List<SkillIndexElement>();
            while (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "SkillIndex")
            {
                SkillIndexElement elem = new SkillIndexElement();
                elem.ReadXml(reader);
                list.Add(elem);
                reader.Read();
            }
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            foreach (SkillIndexElement elem in list)
            {
                elem.WriteXml(writer);
            }
        }
    }

    public class SkillBarSetInfo : IXmlSerializable
    {
        public Dictionary<string, GUIDElement> dic { get; set; }

        public System.Xml.Schema.XmlSchema GetSchema() { return null; }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            dic = new Dictionary<string, GUIDElement>();

            if (reader.MoveToContent() == XmlNodeType.Element && "SkillBarSetInfo" == reader.LocalName)
            {
                reader.Read();
                while (reader.MoveToContent() == XmlNodeType.Element)
                {
                    string name = reader.LocalName;
                    GUIDElement elem = new GUIDElement();
                    reader.Read();
                    elem.ReadXml(reader);
                    dic.Add(name, elem);
                    reader.Read();
                }
            }
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            foreach (KeyValuePair<string, GUIDElement> elem in dic)
            {
                writer.WriteStartElement(elem.Key);
                elem.Value.WriteXml(writer);
                writer.WriteEndElement();
            }
        }
    }

#endif

    // 将技能栏设置信息保存在XML文件中
    public static void AddSkillBarSetInfo(string guid,SkillBarInfo[] curData)
    {
        m_SkillBarSetMap = GetSkillBarSetInfo();
        if (m_SkillBarSetMap.ContainsKey(guid))
        {
            m_SkillBarSetMap.Remove(guid);
        }
        m_SkillBarSetMap.Add(guid, curData);
        if (!Directory.Exists(DataPath))
        {
            Directory.CreateDirectory(DataPath);
        }
#if UNITY_WP8
        SkillBarSetInfo skillBarSetInfo = new SkillBarSetInfo();
        skillBarSetInfo.dic = new Dictionary<string, GUIDElement>();

        foreach (string keys in m_SkillBarSetMap.Keys)
        {
            string name = "GUID" + keys;
            SkillBarInfo[] _skillBarInfo = m_SkillBarSetMap[keys];

            if (null != _skillBarInfo)
            {
                GUIDElement guidElem = new GUIDElement();
                guidElem.list = new List<SkillIndexElement>();
                
                for (int i = 0; i < _skillBarInfo.Length; ++i)
                {
                    SkillIndexElement skillIndexElem = new SkillIndexElement();
                    skillIndexElem.SkillIndex = _skillBarInfo[i].SkillIndex.ToString();
                    guidElem.list.Add(skillIndexElem);
                }

                skillBarSetInfo.dic.Add(name, guidElem);
            }
        }

        XmlHelper.XmlSerializeToFile(skillBarSetInfo, DataPath + SkillBarSetFileName, Encoding.UTF8);

#else
        XmlDocument xml = new XmlDocument();
        XmlElement root = xml.CreateElement("SkillBarSetInfo");
        xml.AppendChild(root);
        foreach (string keys in m_SkillBarSetMap.Keys)
        {
            XmlElement curGUIDElement = xml.CreateElement("GUID" + keys);
            root.AppendChild(curGUIDElement);
            SkillBarInfo[] _skillBarInfo = m_SkillBarSetMap[keys];
            if (null != _skillBarInfo)
            {
                for (int i = 0; i < _skillBarInfo.Length; i++)
                {
                    XmlElement curAutoElement = xml.CreateElement("SkillIndex");
                    curAutoElement.InnerText = _skillBarInfo[i].SkillIndex.ToString();
                    curGUIDElement.AppendChild(curAutoElement);
                }
            }
        }
        xml.Save(DataPath + SkillBarSetFileName);
#endif
    }

    // 从配置文件读取角色数据，保存在Map中
    public static Dictionary<string,SkillBarInfo[]> GetSkillBarSetInfo()
    {
        if (null != m_SkillBarSetMap)
        {
            return m_SkillBarSetMap;
        }

        m_SkillBarSetMap = new Dictionary<string, SkillBarInfo[]>();
#if UNITY_WP8
        SkillBarSetInfo skillBarSetInfo = XmlHelper.XmlDeserializeFromFile<SkillBarSetInfo>(DataPath + SkillBarSetFileName, Encoding.UTF8);
        if (null == skillBarSetInfo)
        {
            return m_SkillBarSetMap;
        }

        foreach (KeyValuePair<string, GUIDElement> iter in skillBarSetInfo.dic)
        {
            string guid = iter.Key.Substring(4);
            GUIDElement guidElem = iter.Value;

            SkillBarInfo[] _skillBarInfo = new SkillBarInfo[(int)SKILLBAR.MAXSKILLBARNUM];
            for (int i = 0; i < (int)SKILLBAR.MAXSKILLBARNUM; i++)
            {
                _skillBarInfo[i] = new SkillBarInfo();
            }

            int nCount = 0;
            foreach (SkillIndexElement skillIndexElem in guidElem.list)
            {
                if (nCount >= 0 && nCount < _skillBarInfo.Length)
                {
                    string typeValue = skillIndexElem.SkillIndex;
                    _skillBarInfo[nCount].SkillIndex = Convert.ToInt32(typeValue);
                    nCount++;
                }
            }
            m_SkillBarSetMap.Add(guid, _skillBarInfo);

        }    

#else
        XmlDocument xml = new XmlDocument();
        try
        {
            xml.Load(DataPath + SkillBarSetFileName);
        }
        catch (System.Exception ex)
        {
            return m_SkillBarSetMap;
        }

        foreach (XmlElement elemGUID in xml.FirstChild.ChildNodes)
        {
            string guid = elemGUID.Name.Substring(4);
            SkillBarInfo[] _skillBarInfo =new SkillBarInfo[(int)SKILLBAR.MAXSKILLBARNUM];
            for (int i = 0; i < (int)SKILLBAR.MAXSKILLBARNUM; i++)
            {
                _skillBarInfo[i] = new SkillBarInfo();
                _skillBarInfo[i].CleanUp();
            }
            int nCount = 0;
            foreach (XmlElement _ownSkillElement in elemGUID)
            {
                if (_ownSkillElement.Name == "SkillIndex")
                {
                    if (nCount >= 0 && nCount < _skillBarInfo.Length)
                    {
                       string typeValue = _ownSkillElement.InnerText;
                       _skillBarInfo[nCount].SkillIndex = Convert.ToInt32(typeValue);
                       nCount++;
                    }
                }
            }
            m_SkillBarSetMap.Add(guid, _skillBarInfo);
        }
#endif
        return m_SkillBarSetMap;
    }
    public static void SetSystemDefault()
	{
        // 经过测试始终Android下始终关闭不掉FastBloom，所以在这里进行一次处理
#if UNITY_ANDROID && !UNITY_EDITOR
            if (PlayerPreferenceData.SystemTableau == 0)    //安卓第一次进入
            {
                PlayerPreferenceData.SystemFloodlight = 0;
                PlayerPreferenceData.SystemWallVisionEnable = false;

                string ret = AndroidHelper.platformHelper("shouldShowWarnning");
				if(ret.EndsWith("1")) //低端机型,单核CPU或者512内存，设置为低
				{
                    PlayerPreferenceData.SystemShowOtherPlayerCount = 4;
					PlayerPreferenceData.SystemTableau = 3;
					PlayerPreferenceData.SystemNameBoard = 0;
				}
				else if(ret.EndsWith("2")) //双核CPU，默认设置为中，并且把头顶信息,这个选择关闭
				{
                    PlayerPreferenceData.SystemShowOtherPlayerCount = 15;
					PlayerPreferenceData.SystemTableau = 2;
					PlayerPreferenceData.SystemNameBoard = 0;
				}
				else  //其它默认为中
				{
                    PlayerPreferenceData.SystemShowOtherPlayerCount = 15;
					PlayerPreferenceData.SystemTableau = 2;
            	}
			}
        //ios处理
#elif !UNITY_EDITOR && UNITY_IPHONE 
            if (PlayerPreferenceData.SystemTableau == 0)
            {
                 if (
                     iPhoneGeneration.iPhone3GS == iPhone.generation
                     || iPhoneGeneration.iPhone3G == iPhone.generation
                     || iPhoneGeneration.iPhone4 == iPhone.generation                     
                     )
                {
                    PlayerPreferenceData.SystemFloodlight = 0;
                    PlayerPreferenceData.SystemWallVisionEnable = false;
                    PlayerPreferenceData.SystemMusic = 0;
                    PlayerPreferenceData.SystemSoundEffect = 0;
                    PlayerPreferenceData.SystemScreenMove = 1;
                    PlayerPreferenceData.NewPlayerGuideClose  = false;
                    PlayerPreferenceData.DeathPushDisable = false;
                    PlayerPreferenceData.KillNpcExp = true;
                    PlayerPreferenceData.SystemSkillEffectEnable = false;
                    PlayerPreferenceData.SystemDamageBoardEnable = false;
                    PlayerPreferenceData.SystemShowOtherPlayerCount = 4;
                    PlayerPreferenceData.SystemTableau = 3;
                }
                else if (
                    iPhoneGeneration.iPadMini1Gen == iPhone.generation
                     || iPhoneGeneration.iPad2Gen == iPhone.generation
                     || iPhoneGeneration.iPhone4S == iPhone.generation
                     || iPhoneGeneration.iPodTouch5Gen == iPhone.generation)
                {
                    PlayerPreferenceData.SystemFloodlight = 0;
                    PlayerPreferenceData.SystemWallVisionEnable = false;
                    PlayerPreferenceData.SystemMusic = 1;
                    PlayerPreferenceData.SystemSoundEffect = 1;
                    PlayerPreferenceData.SystemScreenMove = 1;
                    PlayerPreferenceData.NewPlayerGuideClose  = false;
                    PlayerPreferenceData.DeathPushDisable = true;
                    PlayerPreferenceData.KillNpcExp = true;
                    PlayerPreferenceData.SystemSkillEffectEnable = true;
                    PlayerPreferenceData.SystemDamageBoardEnable = true;
                    PlayerPreferenceData.SystemShowOtherPlayerCount = 15;
                    PlayerPreferenceData.SystemTableau = 2;
                }
                 else
                 {
                     PlayerPreferenceData.SystemTableau = 1;
                 }
            }
#endif
    }
}
