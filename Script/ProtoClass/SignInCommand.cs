//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from: SignInCommand.proto
// Note: requires additional types generated from: GiftCommand.proto
namespace ProtoCmd
{
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"SignInReward")]
  public partial class SignInReward : global::ProtoBuf.IExtensible
  {
    public SignInReward() {}
    
    private uint _id = default(uint);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"id", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(uint))]
    public uint id
    {
      get { return _id; }
      set { _id = value; }
    }
    private bool _is_taken = default(bool);
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"is_taken", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(default(bool))]
    public bool is_taken
    {
      get { return _is_taken; }
      set { _is_taken = value; }
    }
    private ProtoCmd.SignInReward.RewardType _type = ProtoCmd.SignInReward.RewardType.MONEY;
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"type", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(ProtoCmd.SignInReward.RewardType.MONEY)]
    public ProtoCmd.SignInReward.RewardType type
    {
      get { return _type; }
      set { _type = value; }
    }
    private uint _value = default(uint);
    [global::ProtoBuf.ProtoMember(4, IsRequired = false, Name=@"value", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(uint))]
    public uint value
    {
      get { return _value; }
      set { _value = value; }
    }
    private uint _item_baseid = default(uint);
    [global::ProtoBuf.ProtoMember(5, IsRequired = false, Name=@"item_baseid", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(uint))]
    public uint item_baseid
    {
      get { return _item_baseid; }
      set { _item_baseid = value; }
    }
    [global::ProtoBuf.ProtoContract(Name=@"RewardType")]
    public enum RewardType
    {
            
      [global::ProtoBuf.ProtoEnum(Name=@"MONEY", Value=0)]
      MONEY = 0,
            
      [global::ProtoBuf.ProtoEnum(Name=@"DIAMOND", Value=1)]
      DIAMOND = 1,
            
      [global::ProtoBuf.ProtoEnum(Name=@"PHYSICAL", Value=2)]
      PHYSICAL = 2,
            
      [global::ProtoBuf.ProtoEnum(Name=@"ITEM", Value=3)]
      ITEM = 3
    }
  
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"RespondSignInOperatorClientCmd")]
  public partial class RespondSignInOperatorClientCmd : global::ProtoBuf.IExtensible
  {
    public RespondSignInOperatorClientCmd() {}
    
    private uint _op_type = default(uint);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"op_type", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(uint))]
    public uint op_type
    {
      get { return _op_type; }
      set { _op_type = value; }
    }
    private uint _ret = default(uint);
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"ret", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
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
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"RefreshSignInDataClientCmd")]
  public partial class RefreshSignInDataClientCmd : global::ProtoBuf.IExtensible
  {
    public RefreshSignInDataClientCmd() {}
    
    private bool _is_sign_today = default(bool);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"is_sign_today", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(default(bool))]
    public bool is_sign_today
    {
      get { return _is_sign_today; }
      set { _is_sign_today = value; }
    }
    private uint _total_sign_days = default(uint);
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"total_sign_days", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(uint))]
    public uint total_sign_days
    {
      get { return _total_sign_days; }
      set { _total_sign_days = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"SignInClientCmd")]
  public partial class SignInClientCmd : global::ProtoBuf.IExtensible
  {
    public SignInClientCmd() {}
    
    private bool _sign = (bool)true;
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"sign", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue((bool)true)]
    public bool sign
    {
      get { return _sign; }
      set { _sign = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
    [global::ProtoBuf.ProtoContract(Name=@"SignInOperatorType")]
    public enum SignInOperatorType
    {
            
      [global::ProtoBuf.ProtoEnum(Name=@"SignInOperatorType_SignIn", Value=1)]
      SignInOperatorType_SignIn = 1
    }
  
}