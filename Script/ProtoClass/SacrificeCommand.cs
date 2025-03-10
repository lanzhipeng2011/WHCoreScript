//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from: SacrificeCommand.proto
// Note: requires additional types generated from: SacrificeDefine.proto
namespace ProtoCmd
{
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"SendAllCardsClientCmd")]
  public partial class SendAllCardsClientCmd : global::ProtoBuf.IExtensible
  {
    public SendAllCardsClientCmd() {}
    
    private readonly global::System.Collections.Generic.List<ProtoCmd.CardInfo> _card = new global::System.Collections.Generic.List<ProtoCmd.CardInfo>();
    [global::ProtoBuf.ProtoMember(1, Name=@"card", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<ProtoCmd.CardInfo> card
    {
      get { return _card; }
    }
  
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"SendAllSpreadsClientCmd")]
  public partial class SendAllSpreadsClientCmd : global::ProtoBuf.IExtensible
  {
    public SendAllSpreadsClientCmd() {}
    
    private readonly global::System.Collections.Generic.List<ProtoCmd.SpreadInfo> _spread = new global::System.Collections.Generic.List<ProtoCmd.SpreadInfo>();
    [global::ProtoBuf.ProtoMember(1, Name=@"spread", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<ProtoCmd.SpreadInfo> spread
    {
      get { return _spread; }
    }
  
    private readonly global::System.Collections.Generic.List<uint> _skill_id = new global::System.Collections.Generic.List<uint>();
    [global::ProtoBuf.ProtoMember(2, Name=@"skill_id", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public global::System.Collections.Generic.List<uint> skill_id
    {
      get { return _skill_id; }
    }
  
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"SendSpreadAttrClientCmd")]
  public partial class SendSpreadAttrClientCmd : global::ProtoBuf.IExtensible
  {
    public SendSpreadAttrClientCmd() {}
    
    private uint _id;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"id", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public uint id
    {
      get { return _id; }
      set { _id = value; }
    }
    private readonly global::System.Collections.Generic.List<ProtoCmd.SpreadAttrInfo> _attr = new global::System.Collections.Generic.List<ProtoCmd.SpreadAttrInfo>();
    [global::ProtoBuf.ProtoMember(2, Name=@"attr", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<ProtoCmd.SpreadAttrInfo> attr
    {
      get { return _attr; }
    }
  
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"SendLeftFreeWashEdgeNumClientCmd")]
  public partial class SendLeftFreeWashEdgeNumClientCmd : global::ProtoBuf.IExtensible
  {
    public SendLeftFreeWashEdgeNumClientCmd() {}
    
    private uint _num;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"num", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public uint num
    {
      get { return _num; }
      set { _num = value; }
    }
    private uint _cost;
    [global::ProtoBuf.ProtoMember(2, IsRequired = true, Name=@"cost", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public uint cost
    {
      get { return _cost; }
      set { _cost = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"ReqCallCardClientCmd")]
  public partial class ReqCallCardClientCmd : global::ProtoBuf.IExtensible
  {
    public ReqCallCardClientCmd() {}
    
    private uint _comp_id;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"comp_id", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public uint comp_id
    {
      get { return _comp_id; }
      set { _comp_id = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"RspCallCardClientCmd")]
  public partial class RspCallCardClientCmd : global::ProtoBuf.IExtensible
  {
    public RspCallCardClientCmd() {}
    
    private ProtoCmd.CardInfo _card;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"card", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public ProtoCmd.CardInfo card
    {
      get { return _card; }
      set { _card = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"ReqLoadCardClientCmd")]
  public partial class ReqLoadCardClientCmd : global::ProtoBuf.IExtensible
  {
    public ReqLoadCardClientCmd() {}
    
    private uint _spread_id;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"spread_id", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public uint spread_id
    {
      get { return _spread_id; }
      set { _spread_id = value; }
    }
    private uint _comp_id;
    [global::ProtoBuf.ProtoMember(2, IsRequired = true, Name=@"comp_id", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public uint comp_id
    {
      get { return _comp_id; }
      set { _comp_id = value; }
    }
    private uint _slot;
    [global::ProtoBuf.ProtoMember(3, IsRequired = true, Name=@"slot", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public uint slot
    {
      get { return _slot; }
      set { _slot = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"RspLoadCardClientCmd")]
  public partial class RspLoadCardClientCmd : global::ProtoBuf.IExtensible
  {
    public RspLoadCardClientCmd() {}
    
    private uint _spread_id;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"spread_id", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public uint spread_id
    {
      get { return _spread_id; }
      set { _spread_id = value; }
    }
    private uint _comp_id;
    [global::ProtoBuf.ProtoMember(2, IsRequired = true, Name=@"comp_id", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public uint comp_id
    {
      get { return _comp_id; }
      set { _comp_id = value; }
    }
    private uint _slot;
    [global::ProtoBuf.ProtoMember(3, IsRequired = true, Name=@"slot", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public uint slot
    {
      get { return _slot; }
      set { _slot = value; }
    }
    private readonly global::System.Collections.Generic.List<uint> _skill_id = new global::System.Collections.Generic.List<uint>();
    [global::ProtoBuf.ProtoMember(4, Name=@"skill_id", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public global::System.Collections.Generic.List<uint> skill_id
    {
      get { return _skill_id; }
    }
  
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"ReqUnloadCardClientCmd")]
  public partial class ReqUnloadCardClientCmd : global::ProtoBuf.IExtensible
  {
    public ReqUnloadCardClientCmd() {}
    
    private uint _spread_id;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"spread_id", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public uint spread_id
    {
      get { return _spread_id; }
      set { _spread_id = value; }
    }
    private uint _comp_id;
    [global::ProtoBuf.ProtoMember(2, IsRequired = true, Name=@"comp_id", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public uint comp_id
    {
      get { return _comp_id; }
      set { _comp_id = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"RspUnloadCardClientCmd")]
  public partial class RspUnloadCardClientCmd : global::ProtoBuf.IExtensible
  {
    public RspUnloadCardClientCmd() {}
    
    private uint _spread_id;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"spread_id", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public uint spread_id
    {
      get { return _spread_id; }
      set { _spread_id = value; }
    }
    private uint _comp_id;
    [global::ProtoBuf.ProtoMember(2, IsRequired = true, Name=@"comp_id", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public uint comp_id
    {
      get { return _comp_id; }
      set { _comp_id = value; }
    }
    private readonly global::System.Collections.Generic.List<uint> _skill_id = new global::System.Collections.Generic.List<uint>();
    [global::ProtoBuf.ProtoMember(3, Name=@"skill_id", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public global::System.Collections.Generic.List<uint> skill_id
    {
      get { return _skill_id; }
    }
  
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"ReqUpgradeCardClientCmd")]
  public partial class ReqUpgradeCardClientCmd : global::ProtoBuf.IExtensible
  {
    public ReqUpgradeCardClientCmd() {}
    
    private uint _comp_id;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"comp_id", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public uint comp_id
    {
      get { return _comp_id; }
      set { _comp_id = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"RspUpgradeCardClientCmd")]
  public partial class RspUpgradeCardClientCmd : global::ProtoBuf.IExtensible
  {
    public RspUpgradeCardClientCmd() {}
    
    private uint _comp_id;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"comp_id", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public uint comp_id
    {
      get { return _comp_id; }
      set { _comp_id = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"ReqWashCardEdgeClientCmd")]
  public partial class ReqWashCardEdgeClientCmd : global::ProtoBuf.IExtensible
  {
    public ReqWashCardEdgeClientCmd() {}
    
    private uint _comp_id;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"comp_id", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public uint comp_id
    {
      get { return _comp_id; }
      set { _comp_id = value; }
    }
    private uint _wash_edges;
    [global::ProtoBuf.ProtoMember(2, IsRequired = true, Name=@"wash_edges", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public uint wash_edges
    {
      get { return _wash_edges; }
      set { _wash_edges = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"RspWashCardEdgeClientCmd")]
  public partial class RspWashCardEdgeClientCmd : global::ProtoBuf.IExtensible
  {
    public RspWashCardEdgeClientCmd() {}
    
    private ProtoCmd.CardInfo _card;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"card", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public ProtoCmd.CardInfo card
    {
      get { return _card; }
      set { _card = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"ReqLoadSkillClientCmd")]
  public partial class ReqLoadSkillClientCmd : global::ProtoBuf.IExtensible
  {
    public ReqLoadSkillClientCmd() {}
    
    private uint _skill_id;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"skill_id", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public uint skill_id
    {
      get { return _skill_id; }
      set { _skill_id = value; }
    }
    private uint _slot;
    [global::ProtoBuf.ProtoMember(2, IsRequired = true, Name=@"slot", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public uint slot
    {
      get { return _slot; }
      set { _slot = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"RspLoadSkillClientCmd")]
  public partial class RspLoadSkillClientCmd : global::ProtoBuf.IExtensible
  {
    public RspLoadSkillClientCmd() {}
    
    private readonly global::System.Collections.Generic.List<uint> _skill_id = new global::System.Collections.Generic.List<uint>();
    [global::ProtoBuf.ProtoMember(1, Name=@"skill_id", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public global::System.Collections.Generic.List<uint> skill_id
    {
      get { return _skill_id; }
    }
  
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"DropCardClientCmd")]
  public partial class DropCardClientCmd : global::ProtoBuf.IExtensible
  {
    public DropCardClientCmd() {}
    
    private uint _comp_id;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"comp_id", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public uint comp_id
    {
      get { return _comp_id; }
      set { _comp_id = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"SendPsychicDrawCardsClientCmd")]
  public partial class SendPsychicDrawCardsClientCmd : global::ProtoBuf.IExtensible
  {
    public SendPsychicDrawCardsClientCmd() {}
    
    private readonly global::System.Collections.Generic.List<uint> _draw_cards = new global::System.Collections.Generic.List<uint>();
    [global::ProtoBuf.ProtoMember(1, Name=@"draw_cards", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public global::System.Collections.Generic.List<uint> draw_cards
    {
      get { return _draw_cards; }
    }
  
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"SendCancelDrawCardCDTimeClientCmd")]
  public partial class SendCancelDrawCardCDTimeClientCmd : global::ProtoBuf.IExtensible
  {
    public SendCancelDrawCardCDTimeClientCmd() {}
    
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"ReqPsychicTurnCardClientCmd")]
  public partial class ReqPsychicTurnCardClientCmd : global::ProtoBuf.IExtensible
  {
    public ReqPsychicTurnCardClientCmd() {}
    
    private uint _index;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"index", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public uint index
    {
      get { return _index; }
      set { _index = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"SendPsychicTurnCardClientCmd")]
  public partial class SendPsychicTurnCardClientCmd : global::ProtoBuf.IExtensible
  {
    public SendPsychicTurnCardClientCmd() {}
    
    private uint _card_id;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"card_id", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public uint card_id
    {
      get { return _card_id; }
      set { _card_id = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"ReqPsychicDrawCardClientCmd")]
  public partial class ReqPsychicDrawCardClientCmd : global::ProtoBuf.IExtensible
  {
    public ReqPsychicDrawCardClientCmd() {}
    
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"SendDrawCardCountDownClientCmd")]
  public partial class SendDrawCardCountDownClientCmd : global::ProtoBuf.IExtensible
  {
    public SendDrawCardCountDownClientCmd() {}
    
    private uint _time;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"time", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public uint time
    {
      get { return _time; }
      set { _time = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"ReqPsychicChallengeClientCmd")]
  public partial class ReqPsychicChallengeClientCmd : global::ProtoBuf.IExtensible
  {
    public ReqPsychicChallengeClientCmd() {}
    
    private uint _index;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"index", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public uint index
    {
      get { return _index; }
      set { _index = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"SendPsychicChallengeRemainCountClientCmd")]
  public partial class SendPsychicChallengeRemainCountClientCmd : global::ProtoBuf.IExtensible
  {
    public SendPsychicChallengeRemainCountClientCmd() {}
    
    private uint _remain;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"remain", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public uint remain
    {
      get { return _remain; }
      set { _remain = value; }
    }
    private uint _extra;
    [global::ProtoBuf.ProtoMember(2, IsRequired = true, Name=@"extra", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public uint extra
    {
      get { return _extra; }
      set { _extra = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
}