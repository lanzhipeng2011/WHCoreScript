//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from: LoginDefine.proto
namespace ProtoCmd
{
    [global::ProtoBuf.ProtoContract(Name=@"LoginReturn")]
    public enum LoginReturn
    {
            
      [global::ProtoBuf.ProtoEnum(Name=@"LOGIN_RETURN_UNKNOWN", Value=0)]
      LOGIN_RETURN_UNKNOWN = 0,
            
      [global::ProtoBuf.ProtoEnum(Name=@"LOGIN_RETURN_VERSIONERROR", Value=1)]
      LOGIN_RETURN_VERSIONERROR = 1,
            
      [global::ProtoBuf.ProtoEnum(Name=@"LOGIN_RETURN_DBERROR", Value=2)]
      LOGIN_RETURN_DBERROR = 2,
            
      [global::ProtoBuf.ProtoEnum(Name=@"LOGIN_RETURN_NOGAMESERVER", Value=3)]
      LOGIN_RETURN_NOGAMESERVER = 3,
            
      [global::ProtoBuf.ProtoEnum(Name=@"LOGIN_RETURN_PASSWORDERROR", Value=4)]
      LOGIN_RETURN_PASSWORDERROR = 4,
            
      [global::ProtoBuf.ProtoEnum(Name=@"LOGIN_RETURN_CHANGEPASSWORD", Value=5)]
      LOGIN_RETURN_CHANGEPASSWORD = 5,
            
      [global::ProtoBuf.ProtoEnum(Name=@"LOGIN_RETURN_IDINUSE", Value=6)]
      LOGIN_RETURN_IDINUSE = 6,
            
      [global::ProtoBuf.ProtoEnum(Name=@"LOGIN_RETURN_IDINCLOSE", Value=7)]
      LOGIN_RETURN_IDINCLOSE = 7,
            
      [global::ProtoBuf.ProtoEnum(Name=@"LOGIN_RETURN_ACCESSNOSTART", Value=8)]
      LOGIN_RETURN_ACCESSNOSTART = 8,
            
      [global::ProtoBuf.ProtoEnum(Name=@"LOGIN_RETURN_USERMAX", Value=9)]
      LOGIN_RETURN_USERMAX = 9,
            
      [global::ProtoBuf.ProtoEnum(Name=@"LOGIN_RETURN_ACCOUNTEXIST", Value=10)]
      LOGIN_RETURN_ACCOUNTEXIST = 10,
            
      [global::ProtoBuf.ProtoEnum(Name=@"LOGON_RETURN_ACCOUNTSUCCESS", Value=11)]
      LOGON_RETURN_ACCOUNTSUCCESS = 11,
            
      [global::ProtoBuf.ProtoEnum(Name=@"LOGIN_RETURN_CHARNAMEREPEAT", Value=12)]
      LOGIN_RETURN_CHARNAMEREPEAT = 12,
            
      [global::ProtoBuf.ProtoEnum(Name=@"LOGIN_RETURN_USERDATANOEXIST", Value=13)]
      LOGIN_RETURN_USERDATANOEXIST = 13,
            
      [global::ProtoBuf.ProtoEnum(Name=@"LOGIN_RETURN_USERNAMEREPEAT", Value=14)]
      LOGIN_RETURN_USERNAMEREPEAT = 14,
            
      [global::ProtoBuf.ProtoEnum(Name=@"LOGIN_RETURN_TIMEOUT", Value=15)]
      LOGIN_RETURN_TIMEOUT = 15,
            
      [global::ProtoBuf.ProtoEnum(Name=@"LOGIN_RETURN_JPEG_PASSPORT", Value=16)]
      LOGIN_RETURN_JPEG_PASSPORT = 16,
            
      [global::ProtoBuf.ProtoEnum(Name=@"LOGIN_RETURN_LOCK", Value=17)]
      LOGIN_RETURN_LOCK = 17,
            
      [global::ProtoBuf.ProtoEnum(Name=@"LOGIN_RETURN_WAITACTIVE", Value=18)]
      LOGIN_RETURN_WAITACTIVE = 18,
            
      [global::ProtoBuf.ProtoEnum(Name=@"LOGIN_RETURN_IMG_LOCK", Value=19)]
      LOGIN_RETURN_IMG_LOCK = 19,
            
      [global::ProtoBuf.ProtoEnum(Name=@"LOGIN_RETURN_DISCONNECT", Value=20)]
      LOGIN_RETURN_DISCONNECT = 20,
            
      [global::ProtoBuf.ProtoEnum(Name=@"LOGIN_RETURN_PLATFORMSUCCESS", Value=21)]
      LOGIN_RETURN_PLATFORMSUCCESS = 21,
            
      [global::ProtoBuf.ProtoEnum(Name=@"LOGIN_RETURN_RECONNECTING", Value=22)]
      LOGIN_RETURN_RECONNECTING = 22,
            
      [global::ProtoBuf.ProtoEnum(Name=@"LOGIN_RETURN_RECONNECT_OK", Value=23)]
      LOGIN_RETURN_RECONNECT_OK = 23,
            
      [global::ProtoBuf.ProtoEnum(Name=@"LOGIN_RETURN_RECONNECT_ERROR", Value=24)]
      LOGIN_RETURN_RECONNECT_ERROR = 24,
            
      [global::ProtoBuf.ProtoEnum(Name=@"LOGIN_RETURN_UNREG_LOGIN", Value=25)]
      LOGIN_RETURN_UNREG_LOGIN = 25
    }
  
    [global::ProtoBuf.ProtoContract(Name=@"ClientMachineType")]
    public enum ClientMachineType
    {
            
      [global::ProtoBuf.ProtoEnum(Name=@"ClientMachineType_Unknow", Value=0)]
      ClientMachineType_Unknow = 0,
            
      [global::ProtoBuf.ProtoEnum(Name=@"ClientMachineType_Android", Value=1)]
      ClientMachineType_Android = 1,
            
      [global::ProtoBuf.ProtoEnum(Name=@"ClientMachineType_Iphone", Value=2)]
      ClientMachineType_Iphone = 2,
            
      [global::ProtoBuf.ProtoEnum(Name=@"ClientMachineType_Ipad", Value=3)]
      ClientMachineType_Ipad = 3,
            
      [global::ProtoBuf.ProtoEnum(Name=@"ClientMachineType_IosPc", Value=4)]
      ClientMachineType_IosPc = 4,
            
      [global::ProtoBuf.ProtoEnum(Name=@"ClientMachineType_WinPc", Value=5)]
      ClientMachineType_WinPc = 5,
            
      [global::ProtoBuf.ProtoEnum(Name=@"ClientMachineType_Win8", Value=6)]
      ClientMachineType_Win8 = 6,
            
      [global::ProtoBuf.ProtoEnum(Name=@"ClientMachineType_WinPad", Value=7)]
      ClientMachineType_WinPad = 7,
            
      [global::ProtoBuf.ProtoEnum(Name=@"ClientMachineType_Max", Value=8)]
      ClientMachineType_Max = 8
    }
  
    [global::ProtoBuf.ProtoContract(Name=@"ClientPlatformType")]
    public enum ClientPlatformType
    {
            
      [global::ProtoBuf.ProtoEnum(Name=@"ClientPlatformType_Unknow", Value=0)]
      ClientPlatformType_Unknow = 0,
            
      [global::ProtoBuf.ProtoEnum(Name=@"ClientPlatformType_ZQGame", Value=1)]
      ClientPlatformType_ZQGame = 1,
            
      [global::ProtoBuf.ProtoEnum(Name=@"ClientPlatformType_IOS", Value=2)]
      ClientPlatformType_IOS = 2,
            
      [global::ProtoBuf.ProtoEnum(Name=@"ClientPlatformType_360", Value=3)]
      ClientPlatformType_360 = 3,
            
      [global::ProtoBuf.ProtoEnum(Name=@"ClientPlatformType_UC", Value=4)]
      ClientPlatformType_UC = 4,
            
      [global::ProtoBuf.ProtoEnum(Name=@"ClientPlatformType_BD", Value=5)]
      ClientPlatformType_BD = 5,
            
      [global::ProtoBuf.ProtoEnum(Name=@"ClientPlatformType_XM", Value=6)]
      ClientPlatformType_XM = 6,
            
      [global::ProtoBuf.ProtoEnum(Name=@"ClientPlatformType_WDJ", Value=7)]
      ClientPlatformType_WDJ = 7,
            
      [global::ProtoBuf.ProtoEnum(Name=@"ClientPlatformType_AZ", Value=8)]
      ClientPlatformType_AZ = 8,
            
      [global::ProtoBuf.ProtoEnum(Name=@"ClientPlatformType_DL", Value=9)]
      ClientPlatformType_DL = 9,
            
      [global::ProtoBuf.ProtoEnum(Name=@"ClientPlatformType_YM", Value=10)]
      ClientPlatformType_YM = 10,
            
      [global::ProtoBuf.ProtoEnum(Name=@"ClientPlatformType_KY", Value=11)]
      ClientPlatformType_KY = 11,
            
      [global::ProtoBuf.ProtoEnum(Name=@"ClientPlatformType_PP", Value=12)]
      ClientPlatformType_PP = 12,
            
      [global::ProtoBuf.ProtoEnum(Name=@"ClientPlatformType_91IOS", Value=13)]
      ClientPlatformType_91IOS = 13,
            
      [global::ProtoBuf.ProtoEnum(Name=@"ClientPlatformType_TBT", Value=14)]
      ClientPlatformType_TBT = 14,
            
      [global::ProtoBuf.ProtoEnum(Name=@"ClientPlatformType_ITOOLS", Value=15)]
      ClientPlatformType_ITOOLS = 15,
            
      [global::ProtoBuf.ProtoEnum(Name=@"ClientPlatformType_PPS", Value=16)]
      ClientPlatformType_PPS = 16,
            
      [global::ProtoBuf.ProtoEnum(Name=@"ClientPlatformType_AppStore", Value=17)]
      ClientPlatformType_AppStore = 17,
            
      [global::ProtoBuf.ProtoEnum(Name=@"ClientPlatformType_x", Value=18)]
      ClientPlatformType_x = 18,
            
      [global::ProtoBuf.ProtoEnum(Name=@"ClientPlatformType_xx", Value=19)]
      ClientPlatformType_xx = 19,
            
      [global::ProtoBuf.ProtoEnum(Name=@"ClientPlatformType_xxx", Value=20)]
      ClientPlatformType_xxx = 20,
            
      [global::ProtoBuf.ProtoEnum(Name=@"ClientPlatformType_IOS_Visitor", Value=1002)]
      ClientPlatformType_IOS_Visitor = 1002,
            
      [global::ProtoBuf.ProtoEnum(Name=@"ClientPlatformType_AZ_Visitor", Value=1008)]
      ClientPlatformType_AZ_Visitor = 1008,
            
      [global::ProtoBuf.ProtoEnum(Name=@"ClientPlatformType_AppStore_Visitor", Value=1017)]
      ClientPlatformType_AppStore_Visitor = 1017,
            
      [global::ProtoBuf.ProtoEnum(Name=@"ClientPlatformType_Max", Value=2000)]
      ClientPlatformType_Max = 2000
    }
  
    [global::ProtoBuf.ProtoContract(Name=@"GameType")]
    public enum GameType
    {
            
      [global::ProtoBuf.ProtoEnum(Name=@"GameType_HY", Value=3)]
      GameType_HY = 3
    }
  
}