//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from: Lottery.proto
namespace ProtoCmd
{
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"ReqLotteryRankingListClientCmd")]
  public partial class ReqLotteryRankingListClientCmd : global::ProtoBuf.IExtensible
  {
    public ReqLotteryRankingListClientCmd() {}
    
    private uint _playerid = default(uint);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"playerid", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(uint))]
    public uint playerid
    {
      get { return _playerid; }
      set { _playerid = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"RefreshLotteryRankingListClientCmd")]
  public partial class RefreshLotteryRankingListClientCmd : global::ProtoBuf.IExtensible
  {
    public RefreshLotteryRankingListClientCmd() {}
    
    private uint _count = default(uint);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"count", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(uint))]
    public uint count
    {
      get { return _count; }
      set { _count = value; }
    }
    private readonly global::System.Collections.Generic.List<uint> _ranking = new global::System.Collections.Generic.List<uint>();
    [global::ProtoBuf.ProtoMember(2, Name=@"ranking", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public global::System.Collections.Generic.List<uint> ranking
    {
      get { return _ranking; }
    }
  
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"UpdateJackpotClientCmd")]
  public partial class UpdateJackpotClientCmd : global::ProtoBuf.IExtensible
  {
    public UpdateJackpotClientCmd() {}
    
    private uint _jackpot = default(uint);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"jackpot", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(uint))]
    public uint jackpot
    {
      get { return _jackpot; }
      set { _jackpot = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"UpdateMyLotteryCountClientCmd")]
  public partial class UpdateMyLotteryCountClientCmd : global::ProtoBuf.IExtensible
  {
    public UpdateMyLotteryCountClientCmd() {}
    
    private uint _count = default(uint);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"count", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(uint))]
    public uint count
    {
      get { return _count; }
      set { _count = value; }
    }
    private bool _is_add = default(bool);
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"is_add", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(default(bool))]
    public bool is_add
    {
      get { return _is_add; }
      set { _is_add = value; }
    }
    private uint _add_num = default(uint);
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"add_num", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(uint))]
    public uint add_num
    {
      get { return _add_num; }
      set { _add_num = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"RefreshScratchRecordClientCmd")]
  public partial class RefreshScratchRecordClientCmd : global::ProtoBuf.IExtensible
  {
    public RefreshScratchRecordClientCmd() {}
    
    private uint _max_num = default(uint);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"max_num", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(uint))]
    public uint max_num
    {
      get { return _max_num; }
      set { _max_num = value; }
    }
    private readonly global::System.Collections.Generic.List<uint> _num = new global::System.Collections.Generic.List<uint>();
    [global::ProtoBuf.ProtoMember(2, Name=@"num", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public global::System.Collections.Generic.List<uint> num
    {
      get { return _num; }
    }
  
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"ScratchClientCmd")]
  public partial class ScratchClientCmd : global::ProtoBuf.IExtensible
  {
    public ScratchClientCmd() {}
    
    private uint _playerid = default(uint);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"playerid", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(uint))]
    public uint playerid
    {
      get { return _playerid; }
      set { _playerid = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"ScratchResultCliemtCmd")]
  public partial class ScratchResultCliemtCmd : global::ProtoBuf.IExtensible
  {
    public ScratchResultCliemtCmd() {}
    
    private uint _num = default(uint);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"num", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(uint))]
    public uint num
    {
      get { return _num; }
      set { _num = value; }
    }
    private bool _enable = default(bool);
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"enable", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(default(bool))]
    public bool enable
    {
      get { return _enable; }
      set { _enable = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"RefreshLotteryRewardClientCmd")]
  public partial class RefreshLotteryRewardClientCmd : global::ProtoBuf.IExtensible
  {
    public RefreshLotteryRewardClientCmd() {}
    
    private bool _enable = default(bool);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"enable", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(default(bool))]
    public bool enable
    {
      get { return _enable; }
      set { _enable = value; }
    }
    private uint _first_count = default(uint);
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"first_count", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(uint))]
    public uint first_count
    {
      get { return _first_count; }
      set { _first_count = value; }
    }
    private uint _first_money = default(uint);
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"first_money", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(uint))]
    public uint first_money
    {
      get { return _first_money; }
      set { _first_money = value; }
    }
    private uint _lucky_count = default(uint);
    [global::ProtoBuf.ProtoMember(4, IsRequired = false, Name=@"lucky_count", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(uint))]
    public uint lucky_count
    {
      get { return _lucky_count; }
      set { _lucky_count = value; }
    }
    private uint _lucky_money = default(uint);
    [global::ProtoBuf.ProtoMember(5, IsRequired = false, Name=@"lucky_money", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(uint))]
    public uint lucky_money
    {
      get { return _lucky_money; }
      set { _lucky_money = value; }
    }
    private uint _other_count = default(uint);
    [global::ProtoBuf.ProtoMember(6, IsRequired = false, Name=@"other_count", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(uint))]
    public uint other_count
    {
      get { return _other_count; }
      set { _other_count = value; }
    }
    private uint _other_money = default(uint);
    [global::ProtoBuf.ProtoMember(7, IsRequired = false, Name=@"other_money", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(uint))]
    public uint other_money
    {
      get { return _other_money; }
      set { _other_money = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"TakeLotteryRewardClientCmd")]
  public partial class TakeLotteryRewardClientCmd : global::ProtoBuf.IExtensible
  {
    public TakeLotteryRewardClientCmd() {}
    
    private uint _playerid = default(uint);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"playerid", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(uint))]
    public uint playerid
    {
      get { return _playerid; }
      set { _playerid = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
}