//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from: TaskDefine.proto
namespace ProtoCmd
{
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"QuestBase")]
  public partial class QuestBase : global::ProtoBuf.IExtensible
  {
    public QuestBase() {}
    
    private uint _quest_id = default(uint);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"quest_id", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(uint))]
    public uint quest_id
    {
      get { return _quest_id; }
      set { _quest_id = value; }
    }
    private ProtoCmd.QuestType _type = ProtoCmd.QuestType.QuestType_None;
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"type", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(ProtoCmd.QuestType.QuestType_None)]
    public ProtoCmd.QuestType type
    {
      get { return _type; }
      set { _type = value; }
    }
    private ProtoCmd.QuestState _state = ProtoCmd.QuestState.QuestState_None;
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"state", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(ProtoCmd.QuestState.QuestState_None)]
    public ProtoCmd.QuestState state
    {
      get { return _state; }
      set { _state = value; }
    }
    private int _need_level = default(int);
    [global::ProtoBuf.ProtoMember(4, IsRequired = false, Name=@"need_level", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int need_level
    {
      get { return _need_level; }
      set { _need_level = value; }
    }
    private int _quality = default(int);
    [global::ProtoBuf.ProtoMember(5, IsRequired = false, Name=@"quality", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int quality
    {
      get { return _quality; }
      set { _quality = value; }
    }
    private int _difficulty = default(int);
    [global::ProtoBuf.ProtoMember(6, IsRequired = false, Name=@"difficulty", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int difficulty
    {
      get { return _difficulty; }
      set { _difficulty = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"QuestRewardPair")]
  public partial class QuestRewardPair : global::ProtoBuf.IExtensible
  {
    public QuestRewardPair() {}
    
    private ProtoCmd.QuestRewardType _type = ProtoCmd.QuestRewardType.QuestRewardType_None;
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"type", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(ProtoCmd.QuestRewardType.QuestRewardType_None)]
    public ProtoCmd.QuestRewardType type
    {
      get { return _type; }
      set { _type = value; }
    }
    private uint _val = default(uint);
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"val", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(uint))]
    public uint val
    {
      get { return _val; }
      set { _val = value; }
    }
    private int _val_type = default(int);
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"val_type", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int val_type
    {
      get { return _val_type; }
      set { _val_type = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"QuestElementNeeds")]
  public partial class QuestElementNeeds : global::ProtoBuf.IExtensible
  {
    public QuestElementNeeds() {}
    
    private uint _id = default(uint);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"id", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(uint))]
    public uint id
    {
      get { return _id; }
      set { _id = value; }
    }
    private uint _num = default(uint);
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"num", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(uint))]
    public uint num
    {
      get { return _num; }
      set { _num = value; }
    }
    private uint _add1 = default(uint);
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"add1", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(uint))]
    public uint add1
    {
      get { return _add1; }
      set { _add1 = value; }
    }
    private uint _cur_num = default(uint);
    [global::ProtoBuf.ProtoMember(4, IsRequired = false, Name=@"cur_num", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(uint))]
    public uint cur_num
    {
      get { return _cur_num; }
      set { _cur_num = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"QuestElementBase")]
  public partial class QuestElementBase : global::ProtoBuf.IExtensible
  {
    public QuestElementBase() {}
    
    private uint _quest_id = default(uint);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"quest_id", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(uint))]
    public uint quest_id
    {
      get { return _quest_id; }
      set { _quest_id = value; }
    }
    private uint _element_id = default(uint);
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"element_id", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(uint))]
    public uint element_id
    {
      get { return _element_id; }
      set { _element_id = value; }
    }
    private ProtoCmd.QuestElementType _type = ProtoCmd.QuestElementType.QuestElementType_None;
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"type", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(ProtoCmd.QuestElementType.QuestElementType_None)]
    public ProtoCmd.QuestElementType type
    {
      get { return _type; }
      set { _type = value; }
    }
    private readonly global::System.Collections.Generic.List<ProtoCmd.QuestElementNeeds> _needs = new global::System.Collections.Generic.List<ProtoCmd.QuestElementNeeds>();
    [global::ProtoBuf.ProtoMember(4, Name=@"needs", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<ProtoCmd.QuestElementNeeds> needs
    {
      get { return _needs; }
    }
  
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"QuestData")]
  public partial class QuestData : global::ProtoBuf.IExtensible
  {
    public QuestData() {}
    
    private ProtoCmd.QuestBase _base = null;
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"base", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public ProtoCmd.QuestBase @base
    {
      get { return _base; }
      set { _base = value; }
    }
    private readonly global::System.Collections.Generic.List<ProtoCmd.QuestElementBase> _elem = new global::System.Collections.Generic.List<ProtoCmd.QuestElementBase>();
    [global::ProtoBuf.ProtoMember(2, Name=@"elem", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<ProtoCmd.QuestElementBase> elem
    {
      get { return _elem; }
    }
  
    private readonly global::System.Collections.Generic.List<ProtoCmd.QuestRewardPair> _rewards = new global::System.Collections.Generic.List<ProtoCmd.QuestRewardPair>();
    [global::ProtoBuf.ProtoMember(3, Name=@"rewards", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<ProtoCmd.QuestRewardPair> rewards
    {
      get { return _rewards; }
    }
  
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
    [global::ProtoBuf.ProtoContract(Name=@"QuestType")]
    public enum QuestType
    {
            
      [global::ProtoBuf.ProtoEnum(Name=@"QuestType_None", Value=0)]
      QuestType_None = 0,
            
      [global::ProtoBuf.ProtoEnum(Name=@"QuestType_Main", Value=1)]
      QuestType_Main = 1,
            
      [global::ProtoBuf.ProtoEnum(Name=@"QuestType_Daily", Value=2)]
      QuestType_Daily = 2,
            
      [global::ProtoBuf.ProtoEnum(Name=@"QuestType_Branch", Value=3)]
      QuestType_Branch = 3,
            
      [global::ProtoBuf.ProtoEnum(Name=@"QuestType_Copy", Value=4)]
      QuestType_Copy = 4,
            
      [global::ProtoBuf.ProtoEnum(Name=@"QuestType_Weekly", Value=5)]
      QuestType_Weekly = 5,
            
      [global::ProtoBuf.ProtoEnum(Name=@"QuestType_Activity", Value=6)]
      QuestType_Activity = 6,
            
      [global::ProtoBuf.ProtoEnum(Name=@"QuestType_Festival", Value=7)]
      QuestType_Festival = 7,
            
      [global::ProtoBuf.ProtoEnum(Name=@"QuestType_Loop", Value=8)]
      QuestType_Loop = 8,
            
      [global::ProtoBuf.ProtoEnum(Name=@"QuestType_Max", Value=9)]
      QuestType_Max = 9
    }
  
    [global::ProtoBuf.ProtoContract(Name=@"QuestElementType")]
    public enum QuestElementType
    {
            
      [global::ProtoBuf.ProtoEnum(Name=@"QuestElementType_None", Value=0)]
      QuestElementType_None = 0,
            
      [global::ProtoBuf.ProtoEnum(Name=@"QuestElementType_Monster", Value=1)]
      QuestElementType_Monster = 1,
            
      [global::ProtoBuf.ProtoEnum(Name=@"QuestElementType_CollectItem", Value=2)]
      QuestElementType_CollectItem = 2,
            
      [global::ProtoBuf.ProtoEnum(Name=@"QuestElementType_MonsterItem", Value=3)]
      QuestElementType_MonsterItem = 3,
            
      [global::ProtoBuf.ProtoEnum(Name=@"QuestElementType_TriggerOper", Value=4)]
      QuestElementType_TriggerOper = 4,
            
      [global::ProtoBuf.ProtoEnum(Name=@"QuestElementType_TriggerData", Value=5)]
      QuestElementType_TriggerData = 5,
            
      [global::ProtoBuf.ProtoEnum(Name=@"QuestElementType_Max", Value=6)]
      QuestElementType_Max = 6
    }
  
    [global::ProtoBuf.ProtoContract(Name=@"QuestTriggerOperType")]
    public enum QuestTriggerOperType
    {
            
      [global::ProtoBuf.ProtoEnum(Name=@"QuestTriggerOperType_None", Value=0)]
      QuestTriggerOperType_None = 0,
            
      [global::ProtoBuf.ProtoEnum(Name=@"QuestTriggerOperType_EquipImprove", Value=1)]
      QuestTriggerOperType_EquipImprove = 1,
            
      [global::ProtoBuf.ProtoEnum(Name=@"QuestTriggerOperType_ClearFB", Value=2)]
      QuestTriggerOperType_ClearFB = 2,
            
      [global::ProtoBuf.ProtoEnum(Name=@"QuestTriggerOperType_Astrology", Value=3)]
      QuestTriggerOperType_Astrology = 3,
            
      [global::ProtoBuf.ProtoEnum(Name=@"QuestTriggerOperType_FindNPC", Value=4)]
      QuestTriggerOperType_FindNPC = 4,
            
      [global::ProtoBuf.ProtoEnum(Name=@"QuestOperationType_Max", Value=5)]
      QuestOperationType_Max = 5
    }
  
    [global::ProtoBuf.ProtoContract(Name=@"QuestTriggerDataType")]
    public enum QuestTriggerDataType
    {
            
      [global::ProtoBuf.ProtoEnum(Name=@"QuestTriggerDataType_None", Value=0)]
      QuestTriggerDataType_None = 0,
            
      [global::ProtoBuf.ProtoEnum(Name=@"QuestTriggerDataType_PlayerLv", Value=1)]
      QuestTriggerDataType_PlayerLv = 1,
            
      [global::ProtoBuf.ProtoEnum(Name=@"QuestTriggerDataType_SoldierLv", Value=2)]
      QuestTriggerDataType_SoldierLv = 2,
            
      [global::ProtoBuf.ProtoEnum(Name=@"QuestTriggerDataType_BuildingLv", Value=3)]
      QuestTriggerDataType_BuildingLv = 3,
            
      [global::ProtoBuf.ProtoEnum(Name=@"QuestTriggerDataType_SoldierRecruit", Value=4)]
      QuestTriggerDataType_SoldierRecruit = 4,
            
      [global::ProtoBuf.ProtoEnum(Name=@"QuestTriggerDataType_Max", Value=5)]
      QuestTriggerDataType_Max = 5
    }
  
    [global::ProtoBuf.ProtoContract(Name=@"QuestRewardType")]
    public enum QuestRewardType
    {
            
      [global::ProtoBuf.ProtoEnum(Name=@"QuestRewardType_None", Value=0)]
      QuestRewardType_None = 0,
            
      [global::ProtoBuf.ProtoEnum(Name=@"QuestRewardType_Money", Value=1)]
      QuestRewardType_Money = 1,
            
      [global::ProtoBuf.ProtoEnum(Name=@"QuestRewardType_Item", Value=2)]
      QuestRewardType_Item = 2,
            
      [global::ProtoBuf.ProtoEnum(Name=@"QuestRewardType_Exp", Value=3)]
      QuestRewardType_Exp = 3,
            
      [global::ProtoBuf.ProtoEnum(Name=@"QuestRewardType_Sprite", Value=4)]
      QuestRewardType_Sprite = 4,
            
      [global::ProtoBuf.ProtoEnum(Name=@"QuestRewardType_Soul", Value=5)]
      QuestRewardType_Soul = 5,
            
      [global::ProtoBuf.ProtoEnum(Name=@"QuestRewardType_Max", Value=6)]
      QuestRewardType_Max = 6
    }
  
    [global::ProtoBuf.ProtoContract(Name=@"QuestState")]
    public enum QuestState
    {
            
      [global::ProtoBuf.ProtoEnum(Name=@"QuestState_None", Value=0)]
      QuestState_None = 0,
            
      [global::ProtoBuf.ProtoEnum(Name=@"QuestState_CanGive", Value=1)]
      QuestState_CanGive = 1,
            
      [global::ProtoBuf.ProtoEnum(Name=@"QuestState_Doing", Value=2)]
      QuestState_Doing = 2,
            
      [global::ProtoBuf.ProtoEnum(Name=@"QuestState_CanSubmit", Value=3)]
      QuestState_CanSubmit = 3,
            
      [global::ProtoBuf.ProtoEnum(Name=@"QuestState_Finish", Value=4)]
      QuestState_Finish = 4,
            
      [global::ProtoBuf.ProtoEnum(Name=@"QuestState_Delete", Value=5)]
      QuestState_Delete = 5,
            
      [global::ProtoBuf.ProtoEnum(Name=@"QuestState_Fail", Value=6)]
      QuestState_Fail = 6,
            
      [global::ProtoBuf.ProtoEnum(Name=@"QuestState_NeedDoing", Value=7)]
      QuestState_NeedDoing = 7,
            
      [global::ProtoBuf.ProtoEnum(Name=@"QuestState_NeedFinish", Value=8)]
      QuestState_NeedFinish = 8,
            
      [global::ProtoBuf.ProtoEnum(Name=@"QuestState_Max", Value=9)]
      QuestState_Max = 9
    }
  
}