//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from: TaskCommand.proto
// Note: requires additional types generated from: TaskDefine.proto
// Note: requires additional types generated from: ErrorCode.proto
namespace ProtoCmd
{
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"SendQuestQuestClientCmd")]
  public partial class SendQuestQuestClientCmd : global::ProtoBuf.IExtensible
  {
    public SendQuestQuestClientCmd() {}
    
    private ProtoCmd.QuestData _quest = null;
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"quest", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public ProtoCmd.QuestData quest
    {
      get { return _quest; }
      set { _quest = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"SendQuestElementQuestClientCmd")]
  public partial class SendQuestElementQuestClientCmd : global::ProtoBuf.IExtensible
  {
    public SendQuestElementQuestClientCmd() {}
    
    private ProtoCmd.QuestElementBase _elem = null;
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"elem", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public ProtoCmd.QuestElementBase elem
    {
      get { return _elem; }
      set { _elem = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"SendQuestListQuestClientCmd")]
  public partial class SendQuestListQuestClientCmd : global::ProtoBuf.IExtensible
  {
    public SendQuestListQuestClientCmd() {}
    
    private readonly global::System.Collections.Generic.List<ProtoCmd.QuestData> _quests = new global::System.Collections.Generic.List<ProtoCmd.QuestData>();
    [global::ProtoBuf.ProtoMember(1, Name=@"quests", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<ProtoCmd.QuestData> quests
    {
      get { return _quests; }
    }
  
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"RefreshQuestStateQuestClientCmd")]
  public partial class RefreshQuestStateQuestClientCmd : global::ProtoBuf.IExtensible
  {
    public RefreshQuestStateQuestClientCmd() {}
    
    private uint _quest_id = default(uint);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"quest_id", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(uint))]
    public uint quest_id
    {
      get { return _quest_id; }
      set { _quest_id = value; }
    }
    private ProtoCmd.QuestState _state = ProtoCmd.QuestState.QuestState_None;
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"state", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(ProtoCmd.QuestState.QuestState_None)]
    public ProtoCmd.QuestState state
    {
      get { return _state; }
      set { _state = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"DelQuestQuestClientCmd")]
  public partial class DelQuestQuestClientCmd : global::ProtoBuf.IExtensible
  {
    public DelQuestQuestClientCmd() {}
    
    private uint _quest_id = default(uint);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"quest_id", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(uint))]
    public uint quest_id
    {
      get { return _quest_id; }
      set { _quest_id = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"FindNPCQuestClientCmd")]
  public partial class FindNPCQuestClientCmd : global::ProtoBuf.IExtensible
  {
    public FindNPCQuestClientCmd() {}
    
    private uint _quest_id = default(uint);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"quest_id", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(uint))]
    public uint quest_id
    {
      get { return _quest_id; }
      set { _quest_id = value; }
    }
    private uint _npc_id = default(uint);
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"npc_id", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(uint))]
    public uint npc_id
    {
      get { return _npc_id; }
      set { _npc_id = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"RequestAcceptQuestQuestClientCmd")]
  public partial class RequestAcceptQuestQuestClientCmd : global::ProtoBuf.IExtensible
  {
    public RequestAcceptQuestQuestClientCmd() {}
    
    private uint _quest_id = default(uint);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"quest_id", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(uint))]
    public uint quest_id
    {
      get { return _quest_id; }
      set { _quest_id = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"RequestFinishQuestQuestClientCmd")]
  public partial class RequestFinishQuestQuestClientCmd : global::ProtoBuf.IExtensible
  {
    public RequestFinishQuestQuestClientCmd() {}
    
    private uint _quest_id = default(uint);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"quest_id", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(uint))]
    public uint quest_id
    {
      get { return _quest_id; }
      set { _quest_id = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"QuestNotifyQuestClientCmd")]
  public partial class QuestNotifyQuestClientCmd : global::ProtoBuf.IExtensible
  {
    public QuestNotifyQuestClientCmd() {}
    
    private uint _quest_id = default(uint);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"quest_id", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(uint))]
    public uint quest_id
    {
      get { return _quest_id; }
      set { _quest_id = value; }
    }
    private ProtoCmd.ErrorCode _ret = ProtoCmd.ErrorCode.ERR_SUCCESS;
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"ret", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(ProtoCmd.ErrorCode.ERR_SUCCESS)]
    public ProtoCmd.ErrorCode ret
    {
      get { return _ret; }
      set { _ret = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
}