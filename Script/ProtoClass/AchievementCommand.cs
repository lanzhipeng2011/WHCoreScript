//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from: AchievementCommand.proto
// Note: requires additional types generated from: AchievementDefine.proto
namespace ProtoCmd
{
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"FinishAchievementData")]
  public partial class FinishAchievementData : global::ProtoBuf.IExtensible
  {
    public FinishAchievementData() {}
    
    private uint _id = default(uint);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"id", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(uint))]
    public uint id
    {
      get { return _id; }
      set { _id = value; }
    }
    private uint _time = default(uint);
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"time", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(uint))]
    public uint time
    {
      get { return _time; }
      set { _time = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"DoingAchievementData")]
  public partial class DoingAchievementData : global::ProtoBuf.IExtensible
  {
    public DoingAchievementData() {}
    
    private uint _id = default(uint);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"id", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(uint))]
    public uint id
    {
      get { return _id; }
      set { _id = value; }
    }
    private uint _type = default(uint);
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"type", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(uint))]
    public uint type
    {
      get { return _type; }
      set { _type = value; }
    }
    private uint _value = default(uint);
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"value", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(uint))]
    public uint value
    {
      get { return _value; }
      set { _value = value; }
    }
    private uint _condition = default(uint);
    [global::ProtoBuf.ProtoMember(4, IsRequired = false, Name=@"condition", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(uint))]
    public uint condition
    {
      get { return _condition; }
      set { _condition = value; }
    }
    private ProtoCmd.AchievementStatus _status = ProtoCmd.AchievementStatus.AchievementStatus_NotActive;
    [global::ProtoBuf.ProtoMember(5, IsRequired = false, Name=@"status", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(ProtoCmd.AchievementStatus.AchievementStatus_NotActive)]
    public ProtoCmd.AchievementStatus status
    {
      get { return _status; }
      set { _status = value; }
    }
    private ProtoCmd.AchievementRewardType _reward_type = ProtoCmd.AchievementRewardType.AchievementRewardType_None;
    [global::ProtoBuf.ProtoMember(6, IsRequired = false, Name=@"reward_type", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(ProtoCmd.AchievementRewardType.AchievementRewardType_None)]
    public ProtoCmd.AchievementRewardType reward_type
    {
      get { return _reward_type; }
      set { _reward_type = value; }
    }
    private uint _reward_value = default(uint);
    [global::ProtoBuf.ProtoMember(7, IsRequired = false, Name=@"reward_value", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(uint))]
    public uint reward_value
    {
      get { return _reward_value; }
      set { _reward_value = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"RefreshAchievementListClientCmd")]
  public partial class RefreshAchievementListClientCmd : global::ProtoBuf.IExtensible
  {
    public RefreshAchievementListClientCmd() {}
    
    private readonly global::System.Collections.Generic.List<ProtoCmd.FinishAchievementData> _finish_list = new global::System.Collections.Generic.List<ProtoCmd.FinishAchievementData>();
    [global::ProtoBuf.ProtoMember(1, Name=@"finish_list", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<ProtoCmd.FinishAchievementData> finish_list
    {
      get { return _finish_list; }
    }
  
    private readonly global::System.Collections.Generic.List<ProtoCmd.DoingAchievementData> _doing_list = new global::System.Collections.Generic.List<ProtoCmd.DoingAchievementData>();
    [global::ProtoBuf.ProtoMember(2, Name=@"doing_list", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<ProtoCmd.DoingAchievementData> doing_list
    {
      get { return _doing_list; }
    }
  
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"UpdateDoingAchievementDataClientCmd")]
  public partial class UpdateDoingAchievementDataClientCmd : global::ProtoBuf.IExtensible
  {
    public UpdateDoingAchievementDataClientCmd() {}
    
    private ProtoCmd.DoingAchievementData _data = null;
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"data", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public ProtoCmd.DoingAchievementData data
    {
      get { return _data; }
      set { _data = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"AddFinishAchievementClientCmd")]
  public partial class AddFinishAchievementClientCmd : global::ProtoBuf.IExtensible
  {
    public AddFinishAchievementClientCmd() {}
    
    private ProtoCmd.FinishAchievementData _data = null;
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"data", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public ProtoCmd.FinishAchievementData data
    {
      get { return _data; }
      set { _data = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"TakenAchievementRewardClientCmd")]
  public partial class TakenAchievementRewardClientCmd : global::ProtoBuf.IExtensible
  {
    public TakenAchievementRewardClientCmd() {}
    
    private uint _id = default(uint);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"id", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(uint))]
    public uint id
    {
      get { return _id; }
      set { _id = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"ResultTakenAchievementRewardClientCmd")]
  public partial class ResultTakenAchievementRewardClientCmd : global::ProtoBuf.IExtensible
  {
    public ResultTakenAchievementRewardClientCmd() {}
    
    private uint _ret = default(uint);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"ret", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(uint))]
    public uint ret
    {
      get { return _ret; }
      set { _ret = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
}