//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from: SkillDefine.proto
namespace ProtoCmd
{
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"SkillProperty")]
  public partial class SkillProperty : global::ProtoBuf.IExtensible
  {
    public SkillProperty() {}
    
    private uint _skill_key = default(uint);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"skill_key", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(uint))]
    public uint skill_key
    {
      get { return _skill_key; }
      set { _skill_key = value; }
    }
    private uint _cold_time = default(uint);
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"cold_time", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(uint))]
    public uint cold_time
    {
      get { return _cold_time; }
      set { _cold_time = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"GeneResearchRobLog")]
  public partial class GeneResearchRobLog : global::ProtoBuf.IExtensible
  {
    public GeneResearchRobLog() {}
    
    private byte[] _rober = null;
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"rober", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public byte[] rober
    {
      get { return _rober; }
      set { _rober = value; }
    }
    private uint _item = default(uint);
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"item", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(uint))]
    public uint item
    {
      get { return _item; }
      set { _item = value; }
    }
    private uint _count = default(uint);
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"count", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(uint))]
    public uint count
    {
      get { return _count; }
      set { _count = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
    [global::ProtoBuf.ProtoContract(Name=@"SkillLearnCostType")]
    public enum SkillLearnCostType
    {
            
      [global::ProtoBuf.ProtoEnum(Name=@"SkillLearnCostType_None", Value=0)]
      SkillLearnCostType_None = 0,
            
      [global::ProtoBuf.ProtoEnum(Name=@"SkillLearnCostType_Money", Value=1)]
      SkillLearnCostType_Money = 1,
            
      [global::ProtoBuf.ProtoEnum(Name=@"SkillLearnCostType_Diamond", Value=2)]
      SkillLearnCostType_Diamond = 2,
            
      [global::ProtoBuf.ProtoEnum(Name=@"SkillLearnCostType_Max", Value=3)]
      SkillLearnCostType_Max = 3
    }
  
    [global::ProtoBuf.ProtoContract(Name=@"GeneResearchType")]
    public enum GeneResearchType
    {
            
      [global::ProtoBuf.ProtoEnum(Name=@"GeneResearchType_None", Value=0)]
      GeneResearchType_None = 0,
            
      [global::ProtoBuf.ProtoEnum(Name=@"GeneResearchType_Human", Value=1)]
      GeneResearchType_Human = 1,
            
      [global::ProtoBuf.ProtoEnum(Name=@"GeneResearchType_Wolf", Value=2)]
      GeneResearchType_Wolf = 2,
            
      [global::ProtoBuf.ProtoEnum(Name=@"GeneResearchType_Vampire", Value=3)]
      GeneResearchType_Vampire = 3
    }
  
}