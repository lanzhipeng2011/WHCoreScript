//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from: PlayerDataDefine.proto
// Note: requires additional types generated from: map_screen_define.proto
namespace ProtoCmd
{
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"PlayerMainData")]
  public partial class PlayerMainData : global::ProtoBuf.IExtensible
  {
    public PlayerMainData() {}
    
    private uint _role = default(uint);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"role", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(uint))]
    public uint role
    {
      get { return _role; }
      set { _role = value; }
    }
    private uint _sex = default(uint);
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"sex", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(uint))]
    public uint sex
    {
      get { return _sex; }
      set { _sex = value; }
    }
    private uint _level = default(uint);
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"level", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(uint))]
    public uint level
    {
      get { return _level; }
      set { _level = value; }
    }
    private uint _exp = default(uint);
    [global::ProtoBuf.ProtoMember(4, IsRequired = false, Name=@"exp", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(uint))]
    public uint exp
    {
      get { return _exp; }
      set { _exp = value; }
    }
    private uint _titleid = default(uint);
    [global::ProtoBuf.ProtoMember(5, IsRequired = false, Name=@"titleid", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(uint))]
    public uint titleid
    {
      get { return _titleid; }
      set { _titleid = value; }
    }
    private float _speed = default(float);
    [global::ProtoBuf.ProtoMember(6, IsRequired = false, Name=@"speed", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    [global::System.ComponentModel.DefaultValue(default(float))]
    public float speed
    {
      get { return _speed; }
      set { _speed = value; }
    }
    private float _att_speed = default(float);
    [global::ProtoBuf.ProtoMember(7, IsRequired = false, Name=@"att_speed", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
    [global::System.ComponentModel.DefaultValue(default(float))]
    public float att_speed
    {
      get { return _att_speed; }
      set { _att_speed = value; }
    }
    private uint _fight_capacity = default(uint);
    [global::ProtoBuf.ProtoMember(8, IsRequired = false, Name=@"fight_capacity", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(uint))]
    public uint fight_capacity
    {
      get { return _fight_capacity; }
      set { _fight_capacity = value; }
    }
    private string _name = "";
    [global::ProtoBuf.ProtoMember(9, IsRequired = false, Name=@"name", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string name
    {
      get { return _name; }
      set { _name = value; }
    }
    private uint _player_id = default(uint);
    [global::ProtoBuf.ProtoMember(10, IsRequired = false, Name=@"player_id", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(uint))]
    public uint player_id
    {
      get { return _player_id; }
      set { _player_id = value; }
    }
    private uint _physical = default(uint);
    [global::ProtoBuf.ProtoMember(11, IsRequired = false, Name=@"physical", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(uint))]
    public uint physical
    {
      get { return _physical; }
      set { _physical = value; }
    }
    private ProtoCmd.FightProp _fightProp = null;
    [global::ProtoBuf.ProtoMember(12, IsRequired = false, Name=@"fightProp", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public ProtoCmd.FightProp fightProp
    {
      get { return _fightProp; }
      set { _fightProp = value; }
    }
    private ProtoCmd.ElementProp _elementProp = null;
    [global::ProtoBuf.ProtoMember(13, IsRequired = false, Name=@"elementProp", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public ProtoCmd.ElementProp elementProp
    {
      get { return _elementProp; }
      set { _elementProp = value; }
    }
    private uint _arena_credits = default(uint);
    [global::ProtoBuf.ProtoMember(14, IsRequired = false, Name=@"arena_credits", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(uint))]
    public uint arena_credits
    {
      get { return _arena_credits; }
      set { _arena_credits = value; }
    }
    private uint _arena_num = default(uint);
    [global::ProtoBuf.ProtoMember(15, IsRequired = false, Name=@"arena_num", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(uint))]
    public uint arena_num
    {
      get { return _arena_num; }
      set { _arena_num = value; }
    }
    private uint _arena_num_friend = default(uint);
    [global::ProtoBuf.ProtoMember(16, IsRequired = false, Name=@"arena_num_friend", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(uint))]
    public uint arena_num_friend
    {
      get { return _arena_num_friend; }
      set { _arena_num_friend = value; }
    }
    private uint _vip_level = default(uint);
    [global::ProtoBuf.ProtoMember(17, IsRequired = false, Name=@"vip_level", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(uint))]
    public uint vip_level
    {
      get { return _vip_level; }
      set { _vip_level = value; }
    }
    private uint _vip_point = default(uint);
    [global::ProtoBuf.ProtoMember(18, IsRequired = false, Name=@"vip_point", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(uint))]
    public uint vip_point
    {
      get { return _vip_point; }
      set { _vip_point = value; }
    }
    private uint _vip_next_point = default(uint);
    [global::ProtoBuf.ProtoMember(19, IsRequired = false, Name=@"vip_next_point", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(uint))]
    public uint vip_next_point
    {
      get { return _vip_next_point; }
      set { _vip_next_point = value; }
    }
    private uint _physical_max = default(uint);
    [global::ProtoBuf.ProtoMember(20, IsRequired = false, Name=@"physical_max", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(uint))]
    public uint physical_max
    {
      get { return _physical_max; }
      set { _physical_max = value; }
    }
    private uint _courage = default(uint);
    [global::ProtoBuf.ProtoMember(21, IsRequired = false, Name=@"courage", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(uint))]
    public uint courage
    {
      get { return _courage; }
      set { _courage = value; }
    }
    private uint _pvp_power = default(uint);
    [global::ProtoBuf.ProtoMember(22, IsRequired = false, Name=@"pvp_power", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(uint))]
    public uint pvp_power
    {
      get { return _pvp_power; }
      set { _pvp_power = value; }
    }
    private uint _guild_skill_level = default(uint);
    [global::ProtoBuf.ProtoMember(23, IsRequired = false, Name=@"guild_skill_level", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(uint))]
    public uint guild_skill_level
    {
      get { return _guild_skill_level; }
      set { _guild_skill_level = value; }
    }
    private uint _contrib = default(uint);
    [global::ProtoBuf.ProtoMember(24, IsRequired = false, Name=@"contrib", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(uint))]
    public uint contrib
    {
      get { return _contrib; }
      set { _contrib = value; }
    }
    private ProtoCmd.FightProp _rune_fightProp = null;
    [global::ProtoBuf.ProtoMember(25, IsRequired = false, Name=@"rune_fightProp", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public ProtoCmd.FightProp rune_fightProp
    {
      get { return _rune_fightProp; }
      set { _rune_fightProp = value; }
    }
    private ProtoCmd.FightProp _pet_fightProp = null;
    [global::ProtoBuf.ProtoMember(26, IsRequired = false, Name=@"pet_fightProp", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public ProtoCmd.FightProp pet_fightProp
    {
      get { return _pet_fightProp; }
      set { _pet_fightProp = value; }
    }
    private uint _battle_score = default(uint);
    [global::ProtoBuf.ProtoMember(27, IsRequired = false, Name=@"battle_score", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(uint))]
    public uint battle_score
    {
      get { return _battle_score; }
      set { _battle_score = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
    [global::ProtoBuf.ProtoContract(Name=@"RideVehicleAction")]
    public enum RideVehicleAction
    {
            
      [global::ProtoBuf.ProtoEnum(Name=@"RideVehicleAction_TakeDown", Value=0)]
      RideVehicleAction_TakeDown = 0,
            
      [global::ProtoBuf.ProtoEnum(Name=@"RideVehicleAction_TakeUp", Value=1)]
      RideVehicleAction_TakeUp = 1
    }
  
    [global::ProtoBuf.ProtoContract(Name=@"RideVehicleResult")]
    public enum RideVehicleResult
    {
            
      [global::ProtoBuf.ProtoEnum(Name=@"RideVehicleResult_Success", Value=0)]
      RideVehicleResult_Success = 0,
            
      [global::ProtoBuf.ProtoEnum(Name=@"RideVehicleResult_Failed", Value=1)]
      RideVehicleResult_Failed = 1
    }
  
    [global::ProtoBuf.ProtoContract(Name=@"SetPkModeResult")]
    public enum SetPkModeResult
    {
            
      [global::ProtoBuf.ProtoEnum(Name=@"SetPkModeResult_Succ", Value=0)]
      SetPkModeResult_Succ = 0,
            
      [global::ProtoBuf.ProtoEnum(Name=@"SetPkModeResult_Fail_Unknown", Value=1)]
      SetPkModeResult_Fail_Unknown = 1,
            
      [global::ProtoBuf.ProtoEnum(Name=@"SetPkModeResult_Fail_ModeTypeWrong", Value=2)]
      SetPkModeResult_Fail_ModeTypeWrong = 2,
            
      [global::ProtoBuf.ProtoEnum(Name=@"SetPkModeResult_Fail_LevelTooLow", Value=3)]
      SetPkModeResult_Fail_LevelTooLow = 3,
            
      [global::ProtoBuf.ProtoEnum(Name=@"SetPkModeResult_Fail_MapConfigNotAllow", Value=4)]
      SetPkModeResult_Fail_MapConfigNotAllow = 4,
            
      [global::ProtoBuf.ProtoEnum(Name=@"SetPkModeResult_MaxV", Value=5)]
      SetPkModeResult_MaxV = 5
    }
  
    [global::ProtoBuf.ProtoContract(Name=@"MoneyType")]
    public enum MoneyType
    {
            
      [global::ProtoBuf.ProtoEnum(Name=@"MoneyType_Money", Value=0)]
      MoneyType_Money = 0,
            
      [global::ProtoBuf.ProtoEnum(Name=@"MoneyType_Diamond", Value=1)]
      MoneyType_Diamond = 1,
            
      [global::ProtoBuf.ProtoEnum(Name=@"MoneyType_End", Value=2)]
      MoneyType_End = 2
    }
  
    [global::ProtoBuf.ProtoContract(Name=@"AddMoneyAction")]
    public enum AddMoneyAction
    {
            
      [global::ProtoBuf.ProtoEnum(Name=@"AddMoneyAction_None", Value=400)]
      AddMoneyAction_None = 400,
            
      [global::ProtoBuf.ProtoEnum(Name=@"AddMoneyAction_FB", Value=401)]
      AddMoneyAction_FB = 401,
            
      [global::ProtoBuf.ProtoEnum(Name=@"AddMoneyAction_Trade", Value=402)]
      AddMoneyAction_Trade = 402,
            
      [global::ProtoBuf.ProtoEnum(Name=@"AddMoneyAction_Booth", Value=403)]
      AddMoneyAction_Booth = 403,
            
      [global::ProtoBuf.ProtoEnum(Name=@"AddMoneyAction_Shop", Value=404)]
      AddMoneyAction_Shop = 404,
            
      [global::ProtoBuf.ProtoEnum(Name=@"AddMoneyAction_Mentor", Value=405)]
      AddMoneyAction_Mentor = 405,
            
      [global::ProtoBuf.ProtoEnum(Name=@"AddMoneyAction_GM", Value=406)]
      AddMoneyAction_GM = 406,
            
      [global::ProtoBuf.ProtoEnum(Name=@"AddMoneyAction_StorMove", Value=407)]
      AddMoneyAction_StorMove = 407,
            
      [global::ProtoBuf.ProtoEnum(Name=@"AddMoneyAction_PlayerPick", Value=408)]
      AddMoneyAction_PlayerPick = 408,
            
      [global::ProtoBuf.ProtoEnum(Name=@"AddMoneyAction_SubalternPick", Value=409)]
      AddMoneyAction_SubalternPick = 409,
            
      [global::ProtoBuf.ProtoEnum(Name=@"AddMoneyAction_Script", Value=410)]
      AddMoneyAction_Script = 410,
            
      [global::ProtoBuf.ProtoEnum(Name=@"AddMoneyAction_Quest", Value=411)]
      AddMoneyAction_Quest = 411,
            
      [global::ProtoBuf.ProtoEnum(Name=@"AddMoneyAction_MailPick", Value=412)]
      AddMoneyAction_MailPick = 412,
            
      [global::ProtoBuf.ProtoEnum(Name=@"AddMoneyAction_Hunt", Value=413)]
      AddMoneyAction_Hunt = 413,
            
      [global::ProtoBuf.ProtoEnum(Name=@"AddMoneyAction_Trust", Value=414)]
      AddMoneyAction_Trust = 414,
            
      [global::ProtoBuf.ProtoEnum(Name=@"AddMoneyAction_SellStar", Value=415)]
      AddMoneyAction_SellStar = 415,
            
      [global::ProtoBuf.ProtoEnum(Name=@"AddMoneyAction_Bless", Value=416)]
      AddMoneyAction_Bless = 416,
            
      [global::ProtoBuf.ProtoEnum(Name=@"AddMoneyAction_Stagedrop", Value=417)]
      AddMoneyAction_Stagedrop = 417,
            
      [global::ProtoBuf.ProtoEnum(Name=@"AddMoneyAction_Monster", Value=418)]
      AddMoneyAction_Monster = 418,
            
      [global::ProtoBuf.ProtoEnum(Name=@"AddMoneyAction_BattleAwary", Value=419)]
      AddMoneyAction_BattleAwary = 419,
            
      [global::ProtoBuf.ProtoEnum(Name=@"AddMoneyAction_ArenaRanking", Value=420)]
      AddMoneyAction_ArenaRanking = 420,
            
      [global::ProtoBuf.ProtoEnum(Name=@"AddMoneyAction_Soldiers", Value=421)]
      AddMoneyAction_Soldiers = 421,
            
      [global::ProtoBuf.ProtoEnum(Name=@"AddMoneyAction_MarketMoneyBack", Value=422)]
      AddMoneyAction_MarketMoneyBack = 422,
            
      [global::ProtoBuf.ProtoEnum(Name=@"AddMoneyAction_Achievement", Value=423)]
      AddMoneyAction_Achievement = 423,
            
      [global::ProtoBuf.ProtoEnum(Name=@"AddMoneyAction_Gift", Value=424)]
      AddMoneyAction_Gift = 424,
            
      [global::ProtoBuf.ProtoEnum(Name=@"AddMoneyAction_Recharge", Value=425)]
      AddMoneyAction_Recharge = 425,
            
      [global::ProtoBuf.ProtoEnum(Name=@"AddMoneyAction_SignIn", Value=426)]
      AddMoneyAction_SignIn = 426,
            
      [global::ProtoBuf.ProtoEnum(Name=@"AddMoneyAction_SellRune", Value=427)]
      AddMoneyAction_SellRune = 427,
            
      [global::ProtoBuf.ProtoEnum(Name=@"AddMoneyAction_Compass", Value=428)]
      AddMoneyAction_Compass = 428,
            
      [global::ProtoBuf.ProtoEnum(Name=@"AddMoneyAction_Lianjin", Value=429)]
      AddMoneyAction_Lianjin = 429,
            
      [global::ProtoBuf.ProtoEnum(Name=@"AddMoneyAction_ArenaCredits", Value=430)]
      AddMoneyAction_ArenaCredits = 430,
            
      [global::ProtoBuf.ProtoEnum(Name=@"AddMoneyAction_Daily", Value=431)]
      AddMoneyAction_Daily = 431,
            
      [global::ProtoBuf.ProtoEnum(Name=@"AddMoneyAction_Augury", Value=432)]
      AddMoneyAction_Augury = 432,
            
      [global::ProtoBuf.ProtoEnum(Name=@"AddMoneyAction_GuildMine", Value=433)]
      AddMoneyAction_GuildMine = 433,
            
      [global::ProtoBuf.ProtoEnum(Name=@"AddMoneyAction_Activity", Value=434)]
      AddMoneyAction_Activity = 434,
            
      [global::ProtoBuf.ProtoEnum(Name=@"AddMoneyAction_RechargeGift", Value=435)]
      AddMoneyAction_RechargeGift = 435,
            
      [global::ProtoBuf.ProtoEnum(Name=@"AddMoneyAction_Gift_Media", Value=436)]
      AddMoneyAction_Gift_Media = 436,
            
      [global::ProtoBuf.ProtoEnum(Name=@"AddMoneyAction_Gift_Recharge", Value=437)]
      AddMoneyAction_Gift_Recharge = 437,
            
      [global::ProtoBuf.ProtoEnum(Name=@"AddMoneyAction_Gift_LevelUp", Value=438)]
      AddMoneyAction_Gift_LevelUp = 438,
            
      [global::ProtoBuf.ProtoEnum(Name=@"AddMoneyAction_Gift_Vip", Value=439)]
      AddMoneyAction_Gift_Vip = 439,
            
      [global::ProtoBuf.ProtoEnum(Name=@"AddMoneyAction_Gift_SignIn", Value=440)]
      AddMoneyAction_Gift_SignIn = 440,
            
      [global::ProtoBuf.ProtoEnum(Name=@"AddMoneyAction_Gift_FreshMan", Value=441)]
      AddMoneyAction_Gift_FreshMan = 441,
            
      [global::ProtoBuf.ProtoEnum(Name=@"AddMoneyAction_Gift_VipDaily", Value=442)]
      AddMoneyAction_Gift_VipDaily = 442,
            
      [global::ProtoBuf.ProtoEnum(Name=@"AddMoneyAction_Gift_Online", Value=443)]
      AddMoneyAction_Gift_Online = 443,
            
      [global::ProtoBuf.ProtoEnum(Name=@"AddMoneyAction_Gift_Physical", Value=444)]
      AddMoneyAction_Gift_Physical = 444,
            
      [global::ProtoBuf.ProtoEnum(Name=@"AddMoneyAction_Gift_FirstRecharge", Value=445)]
      AddMoneyAction_Gift_FirstRecharge = 445,
            
      [global::ProtoBuf.ProtoEnum(Name=@"AddMoneyAction_Gift_FBLevel", Value=446)]
      AddMoneyAction_Gift_FBLevel = 446,
            
      [global::ProtoBuf.ProtoEnum(Name=@"AddMoneyAction_Gift_Activity", Value=447)]
      AddMoneyAction_Gift_Activity = 447,
            
      [global::ProtoBuf.ProtoEnum(Name=@"AddMoneyAction_Gift_Login", Value=448)]
      AddMoneyAction_Gift_Login = 448,
            
      [global::ProtoBuf.ProtoEnum(Name=@"AddMoneyAction_Gift_Fund", Value=449)]
      AddMoneyAction_Gift_Fund = 449,
            
      [global::ProtoBuf.ProtoEnum(Name=@"AddMoneyAction_Gift_MonthCard", Value=450)]
      AddMoneyAction_Gift_MonthCard = 450,
            
      [global::ProtoBuf.ProtoEnum(Name=@"AddMoneyAction_Gift_ShareBlog", Value=451)]
      AddMoneyAction_Gift_ShareBlog = 451,
            
      [global::ProtoBuf.ProtoEnum(Name=@"AddMoneyAction_WBOSS_REWARD", Value=452)]
      AddMoneyAction_WBOSS_REWARD = 452,
            
      [global::ProtoBuf.ProtoEnum(Name=@"AddMoneyAction_PVAI_REWARD", Value=453)]
      AddMoneyAction_PVAI_REWARD = 453,
            
      [global::ProtoBuf.ProtoEnum(Name=@"AddMoneyAction_GBOSS_REWARD_GUILD", Value=454)]
      AddMoneyAction_GBOSS_REWARD_GUILD = 454,
            
      [global::ProtoBuf.ProtoEnum(Name=@"AddMoneyAction_GBOSS_REWARD_PLAYER", Value=455)]
      AddMoneyAction_GBOSS_REWARD_PLAYER = 455,
            
      [global::ProtoBuf.ProtoEnum(Name=@"AddMoneyAction_GWAR_REWARD_GUILD", Value=456)]
      AddMoneyAction_GWAR_REWARD_GUILD = 456,
            
      [global::ProtoBuf.ProtoEnum(Name=@"AddMoneyAction_GWAR_REWARD_PLAYER", Value=457)]
      AddMoneyAction_GWAR_REWARD_PLAYER = 457,
            
      [global::ProtoBuf.ProtoEnum(Name=@"AddMoneyAction_GWAR_REWARD_POINT", Value=458)]
      AddMoneyAction_GWAR_REWARD_POINT = 458,
            
      [global::ProtoBuf.ProtoEnum(Name=@"AddMoneyAction_MONTH_CARD", Value=459)]
      AddMoneyAction_MONTH_CARD = 459,
            
      [global::ProtoBuf.ProtoEnum(Name=@"AddMoneyAction_QUARTER_CARD", Value=460)]
      AddMoneyAction_QUARTER_CARD = 460,
            
      [global::ProtoBuf.ProtoEnum(Name=@"AddMoneyAction_FOREVER_CARD", Value=461)]
      AddMoneyAction_FOREVER_CARD = 461,
            
      [global::ProtoBuf.ProtoEnum(Name=@"AddMoneyAction_Lottery", Value=462)]
      AddMoneyAction_Lottery = 462,
            
      [global::ProtoBuf.ProtoEnum(Name=@"AddMoneyAction_SacrificeRank", Value=463)]
      AddMoneyAction_SacrificeRank = 463,
            
      [global::ProtoBuf.ProtoEnum(Name=@"AddMoneyAction_Gift_WeekFirstRecharge", Value=464)]
      AddMoneyAction_Gift_WeekFirstRecharge = 464,
            
      [global::ProtoBuf.ProtoEnum(Name=@"AddMoneyAction_Gift_SignVIP", Value=465)]
      AddMoneyAction_Gift_SignVIP = 465,
            
      [global::ProtoBuf.ProtoEnum(Name=@"AddMoneyAction_Gift_VIP_Double", Value=466)]
      AddMoneyAction_Gift_VIP_Double = 466,
            
      [global::ProtoBuf.ProtoEnum(Name=@"AddMoneyAction_Business", Value=467)]
      AddMoneyAction_Business = 467,
            
      [global::ProtoBuf.ProtoEnum(Name=@"AddMoneyAction_Max", Value=468)]
      AddMoneyAction_Max = 468
    }
  
    [global::ProtoBuf.ProtoContract(Name=@"DelMoneyAction")]
    public enum DelMoneyAction
    {
            
      [global::ProtoBuf.ProtoEnum(Name=@"DelMoneyAction_None", Value=600)]
      DelMoneyAction_None = 600,
            
      [global::ProtoBuf.ProtoEnum(Name=@"DelMoneyAction_ExtendPackage", Value=601)]
      DelMoneyAction_ExtendPackage = 601,
            
      [global::ProtoBuf.ProtoEnum(Name=@"DelMoneyAction_Trade", Value=602)]
      DelMoneyAction_Trade = 602,
            
      [global::ProtoBuf.ProtoEnum(Name=@"DelMoneyAction_Booth", Value=603)]
      DelMoneyAction_Booth = 603,
            
      [global::ProtoBuf.ProtoEnum(Name=@"DelMoneyAction_Shop", Value=604)]
      DelMoneyAction_Shop = 604,
            
      [global::ProtoBuf.ProtoEnum(Name=@"DelMoneyAction_Die", Value=605)]
      DelMoneyAction_Die = 605,
            
      [global::ProtoBuf.ProtoEnum(Name=@"DelMoneyAction_ChatWorld", Value=606)]
      DelMoneyAction_ChatWorld = 606,
            
      [global::ProtoBuf.ProtoEnum(Name=@"DelMoneyAction_StorMove", Value=607)]
      DelMoneyAction_StorMove = 607,
            
      [global::ProtoBuf.ProtoEnum(Name=@"DelMoneyAction_LearnSkill", Value=608)]
      DelMoneyAction_LearnSkill = 608,
            
      [global::ProtoBuf.ProtoEnum(Name=@"DelMoneyAction_SubalternLearnSkill", Value=609)]
      DelMoneyAction_SubalternLearnSkill = 609,
            
      [global::ProtoBuf.ProtoEnum(Name=@"DelMoneyAction_DoubleSkill", Value=610)]
      DelMoneyAction_DoubleSkill = 610,
            
      [global::ProtoBuf.ProtoEnum(Name=@"DelMoneyAction_Script", Value=611)]
      DelMoneyAction_Script = 611,
            
      [global::ProtoBuf.ProtoEnum(Name=@"DelMoneyAction_Quest", Value=612)]
      DelMoneyAction_Quest = 612,
            
      [global::ProtoBuf.ProtoEnum(Name=@"DelMoneyAction_EquipImprove", Value=613)]
      DelMoneyAction_EquipImprove = 613,
            
      [global::ProtoBuf.ProtoEnum(Name=@"DelMoneyAction_CreateArmy", Value=614)]
      DelMoneyAction_CreateArmy = 614,
            
      [global::ProtoBuf.ProtoEnum(Name=@"DelMoneyAction_CreateGuild", Value=615)]
      DelMoneyAction_CreateGuild = 615,
            
      [global::ProtoBuf.ProtoEnum(Name=@"DelMoneyAction_ChangeGuildName", Value=616)]
      DelMoneyAction_ChangeGuildName = 616,
            
      [global::ProtoBuf.ProtoEnum(Name=@"DelMoneyAction_Contribution", Value=617)]
      DelMoneyAction_Contribution = 617,
            
      [global::ProtoBuf.ProtoEnum(Name=@"DelMoneyAction_GuildFinishCoolTime", Value=618)]
      DelMoneyAction_GuildFinishCoolTime = 618,
            
      [global::ProtoBuf.ProtoEnum(Name=@"DelMoneyAction_RepariMachine", Value=619)]
      DelMoneyAction_RepariMachine = 619,
            
      [global::ProtoBuf.ProtoEnum(Name=@"DelMoneyAction_SubalternRecruit", Value=620)]
      DelMoneyAction_SubalternRecruit = 620,
            
      [global::ProtoBuf.ProtoEnum(Name=@"DelMoneyAction_SubalternAnneal", Value=621)]
      DelMoneyAction_SubalternAnneal = 621,
            
      [global::ProtoBuf.ProtoEnum(Name=@"DelMoneyAction_SubalternPickupExp", Value=622)]
      DelMoneyAction_SubalternPickupExp = 622,
            
      [global::ProtoBuf.ProtoEnum(Name=@"DelMoneyAction_MailMoney", Value=623)]
      DelMoneyAction_MailMoney = 623,
            
      [global::ProtoBuf.ProtoEnum(Name=@"DelMoneyAction_MailPickItem", Value=624)]
      DelMoneyAction_MailPickItem = 624,
            
      [global::ProtoBuf.ProtoEnum(Name=@"DelMoneyAction_Astrology", Value=625)]
      DelMoneyAction_Astrology = 625,
            
      [global::ProtoBuf.ProtoEnum(Name=@"DelMoneyAction_ExtendStarPackage", Value=626)]
      DelMoneyAction_ExtendStarPackage = 626,
            
      [global::ProtoBuf.ProtoEnum(Name=@"DelMoneyAction_Crystal", Value=627)]
      DelMoneyAction_Crystal = 627,
            
      [global::ProtoBuf.ProtoEnum(Name=@"DelMoneyAction_MazeMoney", Value=628)]
      DelMoneyAction_MazeMoney = 628,
            
      [global::ProtoBuf.ProtoEnum(Name=@"DelMoneyAction_Soldiers", Value=629)]
      DelMoneyAction_Soldiers = 629,
            
      [global::ProtoBuf.ProtoEnum(Name=@"DelMoneyAction_Architecture", Value=630)]
      DelMoneyAction_Architecture = 630,
            
      [global::ProtoBuf.ProtoEnum(Name=@"DelMoneyAction_RecruitmentSoldiers", Value=641)]
      DelMoneyAction_RecruitmentSoldiers = 641,
            
      [global::ProtoBuf.ProtoEnum(Name=@"DelMoneyAction_PickUpStgedrop", Value=642)]
      DelMoneyAction_PickUpStgedrop = 642,
            
      [global::ProtoBuf.ProtoEnum(Name=@"DelMoneyAction_Mall", Value=643)]
      DelMoneyAction_Mall = 643,
            
      [global::ProtoBuf.ProtoEnum(Name=@"DelMoneyAction_WerwolfImprove", Value=644)]
      DelMoneyAction_WerwolfImprove = 644,
            
      [global::ProtoBuf.ProtoEnum(Name=@"DelMoneyAction_AuctionGoods", Value=645)]
      DelMoneyAction_AuctionGoods = 645,
            
      [global::ProtoBuf.ProtoEnum(Name=@"DelMoneyAction_BidGoods", Value=646)]
      DelMoneyAction_BidGoods = 646,
            
      [global::ProtoBuf.ProtoEnum(Name=@"DelMoneyAction_SignIn", Value=647)]
      DelMoneyAction_SignIn = 647,
            
      [global::ProtoBuf.ProtoEnum(Name=@"DelMoneyAction_Pray", Value=648)]
      DelMoneyAction_Pray = 648,
            
      [global::ProtoBuf.ProtoEnum(Name=@"DelMoneyAction_EnableSorceress", Value=649)]
      DelMoneyAction_EnableSorceress = 649,
            
      [global::ProtoBuf.ProtoEnum(Name=@"DelMoneyAction_ClearPVAICD", Value=650)]
      DelMoneyAction_ClearPVAICD = 650,
            
      [global::ProtoBuf.ProtoEnum(Name=@"DelMoneyAction_BuyVip", Value=651)]
      DelMoneyAction_BuyVip = 651,
            
      [global::ProtoBuf.ProtoEnum(Name=@"DelMoneyAction_VipRightsBuy", Value=652)]
      DelMoneyAction_VipRightsBuy = 652,
            
      [global::ProtoBuf.ProtoEnum(Name=@"DelMoneyAction_Compass", Value=653)]
      DelMoneyAction_Compass = 653,
            
      [global::ProtoBuf.ProtoEnum(Name=@"DelMoneyAction_TreasureChest", Value=654)]
      DelMoneyAction_TreasureChest = 654,
            
      [global::ProtoBuf.ProtoEnum(Name=@"DelMoneyAction_Wipe", Value=655)]
      DelMoneyAction_Wipe = 655,
            
      [global::ProtoBuf.ProtoEnum(Name=@"DelMoneyAction_Max", Value=656)]
      DelMoneyAction_Max = 656
    }
  
}