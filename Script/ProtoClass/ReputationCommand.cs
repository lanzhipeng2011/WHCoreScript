//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from: ReputationCommand.proto
namespace ProtoCmd
{
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"RefreshMyReputationClientCmd")]
  public partial class RefreshMyReputationClientCmd : global::ProtoBuf.IExtensible
  {
    public RefreshMyReputationClientCmd() {}
    
    private readonly global::System.Collections.Generic.List<uint> _reputation = new global::System.Collections.Generic.List<uint>();
    [global::ProtoBuf.ProtoMember(1, Name=@"reputation", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public global::System.Collections.Generic.List<uint> reputation
    {
      get { return _reputation; }
    }
  
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"ReputationExchangeItemClientCmd")]
  public partial class ReputationExchangeItemClientCmd : global::ProtoBuf.IExtensible
  {
    public ReputationExchangeItemClientCmd() {}
    
    private uint _type = default(uint);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"type", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(uint))]
    public uint type
    {
      get { return _type; }
      set { _type = value; }
    }
    private uint _item_baseid = default(uint);
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"item_baseid", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(uint))]
    public uint item_baseid
    {
      get { return _item_baseid; }
      set { _item_baseid = value; }
    }
    private uint _num = default(uint);
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"num", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(uint))]
    public uint num
    {
      get { return _num; }
      set { _num = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"RespondExchangeItemClientCmd")]
  public partial class RespondExchangeItemClientCmd : global::ProtoBuf.IExtensible
  {
    public RespondExchangeItemClientCmd() {}
    
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
  
    [global::ProtoBuf.ProtoContract(Name=@"ReputationType")]
    public enum ReputationType
    {
            
      [global::ProtoBuf.ProtoEnum(Name=@"ReputationType_0", Value=0)]
      ReputationType_0 = 0,
            
      [global::ProtoBuf.ProtoEnum(Name=@"ReputationType_1", Value=1)]
      ReputationType_1 = 1,
            
      [global::ProtoBuf.ProtoEnum(Name=@"ReputationType_2", Value=2)]
      ReputationType_2 = 2,
            
      [global::ProtoBuf.ProtoEnum(Name=@"ReputationType_3", Value=3)]
      ReputationType_3 = 3,
            
      [global::ProtoBuf.ProtoEnum(Name=@"ReputationType_4", Value=4)]
      ReputationType_4 = 4,
            
      [global::ProtoBuf.ProtoEnum(Name=@"ReputationType_5", Value=5)]
      ReputationType_5 = 5,
            
      [global::ProtoBuf.ProtoEnum(Name=@"ReputationType_6", Value=6)]
      ReputationType_6 = 6,
            
      [global::ProtoBuf.ProtoEnum(Name=@"ReputationType_7", Value=7)]
      ReputationType_7 = 7,
            
      [global::ProtoBuf.ProtoEnum(Name=@"ReputationType_8", Value=8)]
      ReputationType_8 = 8,
            
      [global::ProtoBuf.ProtoEnum(Name=@"ReputationType_9", Value=9)]
      ReputationType_9 = 9,
            
      [global::ProtoBuf.ProtoEnum(Name=@"ReputationType_10", Value=10)]
      ReputationType_10 = 10,
            
      [global::ProtoBuf.ProtoEnum(Name=@"ReputationType_11", Value=11)]
      ReputationType_11 = 11,
            
      [global::ProtoBuf.ProtoEnum(Name=@"ReputationType_ALL", Value=12)]
      ReputationType_ALL = 12
    }
  
}