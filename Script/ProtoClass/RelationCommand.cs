//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from: RelationCommand.proto
// Note: requires additional types generated from: RelationDefine.proto
namespace ProtoCmd
{
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"RelationClientCmd")]
  public partial class RelationClientCmd : global::ProtoBuf.IExtensible
  {
    public RelationClientCmd() {}
    
    private uint _re_type = default(uint);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"re_type", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(uint))]
    public uint re_type
    {
      get { return _re_type; }
      set { _re_type = value; }
    }
    private uint _re_id = default(uint);
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"re_id", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(uint))]
    public uint re_id
    {
      get { return _re_id; }
      set { _re_id = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
}