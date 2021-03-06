// Generated by the protocol buffer compiler.  DO NOT EDIT!
// source: PBMsgAlgo.proto
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace PBWrapMsgAlgo {

  /// <summary>Holder for reflection information generated from PBMsgAlgo.proto</summary>
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
  public static partial class PBMsgAlgoReflection {

    #region Descriptor
    /// <summary>File descriptor for PBMsgAlgo.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static PBMsgAlgoReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "Cg9QQk1zZ0FsZ28ucHJvdG8SDVBCV3JhcE1zZ0FsZ28irwEKDlBCTXNnQWxn",
            "b09yZGVyEg4KBnN5bWJvbBgBIAEoCRITCgtldWl0eU1hcmtldBgCIAEoCRIW",
            "Cg5vcmRlckRpcmVjdGlvbhgDIAEoCRIRCglwcmljZVR5cGUYBCABKAkSDAoE",
            "c2l6ZRgFIAEoBRIQCghFcXRPckZ1dBgGIAEoCBISCgpmdXR1cmVGbGFnGAcg",
            "ASgJEhkKEWZ1dHVyZUFjY291bnRUeXBlGAggASgJIrECChJQQk1zZ0FsZ29P",
            "cmRlckxpc3QSEQoJbWVzc2FnZUlkGAEgASgFEhYKDm1lc3NhZ2VTZXJ2aWNl",
            "GAIgASgJEi8KC21lc3NhZ2VUeXBlGAMgASgOMhouUEJXcmFwTXNnQWxnby5N",
            "ZXNzYWdlVHlwZRIVCg1wYXJlbnRPcmRlcklkGAQgASgJEjYKD2JhdE5ld09y",
            "ZGVyTGlzdBgFIAMoCzIdLlBCV3JhcE1zZ0FsZ28uUEJNc2dBbGdvT3JkZXIS",
            "GwoTZXF0QnV5Q2hhbmdlUGVyY2VudBgGIAEoARIcChRlcXRTZWxsQ2hhbmdl",
            "UGVyY2VudBgHIAEoARIZChFmdXRCdXlDaGFuZ2VQb2ludBgIIAEoERIaChJm",
            "dXRTZWxsQ2hhbmdlUG9pbnQYCSABKBEiWAoLUEJNc2dDYW5kbGUSEQoJdGlt",
            "ZXN0YW1wGAEgASgJEgwKBG9wZW4YAiABKAUSDQoFY2xvc2UYAyABKAUSDAoE",
            "aGlnaBgEIAEoBRILCgNsb3cYBSABKAUiTAoQUEJNc2dDYW5kbGVRdWV1ZRIL",
            "CgNyaWMYASABKAkSKwoHY2FuZGxlcxgCIAMoCzIaLlBCV3JhcE1zZ0FsZ28u",
            "UEJNc2dDYW5kbGUqYQoLTWVzc2FnZVR5cGUSDgoKQkFUQ0hfWkVSTxAAEhMK",
            "D0JBVENIX05FV19PUkRFUhABEhUKEUJBVENIX1JFVFJZX09SREVSEAISFgoS",
            "QkFUQ0hfQ0FOQ0VMX09SREVSEANCIAoPUHJvdG9CdWZNZXNzYWdlQg1QQldy",
            "YXBNc2dBbGdvYgZwcm90bzM="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { },
          new pbr::GeneratedClrTypeInfo(new[] {typeof(global::PBWrapMsgAlgo.MessageType), }, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::PBWrapMsgAlgo.PBMsgAlgoOrder), global::PBWrapMsgAlgo.PBMsgAlgoOrder.Parser, new[]{ "Symbol", "EuityMarket", "OrderDirection", "PriceType", "Size", "EqtOrFut", "FutureFlag", "FutureAccountType" }, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::PBWrapMsgAlgo.PBMsgAlgoOrderList), global::PBWrapMsgAlgo.PBMsgAlgoOrderList.Parser, new[]{ "MessageId", "MessageService", "MessageType", "ParentOrderId", "BatNewOrderList", "EqtBuyChangePercent", "EqtSellChangePercent", "FutBuyChangePoint", "FutSellChangePoint" }, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::PBWrapMsgAlgo.PBMsgCandle), global::PBWrapMsgAlgo.PBMsgCandle.Parser, new[]{ "Timestamp", "Open", "Close", "High", "Low" }, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::PBWrapMsgAlgo.PBMsgCandleQueue), global::PBWrapMsgAlgo.PBMsgCandleQueue.Parser, new[]{ "Ric", "Candles" }, null, null, null)
          }));
    }
    #endregion

  }
  #region Enums
  public enum MessageType {
    [pbr::OriginalName("BATCH_ZERO")] BatchZero = 0,
    [pbr::OriginalName("BATCH_NEW_ORDER")] BatchNewOrder = 1,
    [pbr::OriginalName("BATCH_RETRY_ORDER")] BatchRetryOrder = 2,
    [pbr::OriginalName("BATCH_CANCEL_ORDER")] BatchCancelOrder = 3,
  }

  #endregion

  #region Messages
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
  public sealed partial class PBMsgAlgoOrder : pb::IMessage<PBMsgAlgoOrder> {
    private static readonly pb::MessageParser<PBMsgAlgoOrder> _parser = new pb::MessageParser<PBMsgAlgoOrder>(() => new PBMsgAlgoOrder());
    public static pb::MessageParser<PBMsgAlgoOrder> Parser { get { return _parser; } }

    public static pbr::MessageDescriptor Descriptor {
      get { return global::PBWrapMsgAlgo.PBMsgAlgoReflection.Descriptor.MessageTypes[0]; }
    }

    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    public PBMsgAlgoOrder() {
      OnConstruction();
    }

    partial void OnConstruction();

    public PBMsgAlgoOrder(PBMsgAlgoOrder other) : this() {
      symbol_ = other.symbol_;
      euityMarket_ = other.euityMarket_;
      orderDirection_ = other.orderDirection_;
      priceType_ = other.priceType_;
      size_ = other.size_;
      eqtOrFut_ = other.eqtOrFut_;
      futureFlag_ = other.futureFlag_;
      futureAccountType_ = other.futureAccountType_;
    }

    public PBMsgAlgoOrder Clone() {
      return new PBMsgAlgoOrder(this);
    }

    /// <summary>Field number for the "symbol" field.</summary>
    public const int SymbolFieldNumber = 1;
    private string symbol_ = "";
    /// <summary>
    /// 股票代码
    /// </summary>
    public string Symbol {
      get { return symbol_; }
      set {
        symbol_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "euityMarket" field.</summary>
    public const int EuityMarketFieldNumber = 2;
    private string euityMarket_ = "";
    /// <summary>
    /// 上海、深圳
    /// </summary>
    public string EuityMarket {
      get { return euityMarket_; }
      set {
        euityMarket_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "orderDirection" field.</summary>
    public const int OrderDirectionFieldNumber = 3;
    private string orderDirection_ = "";
    /// <summary>
    /// 买卖
    /// </summary>
    public string OrderDirection {
      get { return orderDirection_; }
      set {
        orderDirection_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "priceType" field.</summary>
    public const int PriceTypeFieldNumber = 4;
    private string priceType_ = "";
    /// <summary>
    /// 价格类型，买一，卖一，市价等
    /// </summary>
    public string PriceType {
      get { return priceType_; }
      set {
        priceType_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "size" field.</summary>
    public const int SizeFieldNumber = 5;
    private int size_;
    /// <summary>
    /// 手数
    /// </summary>
    public int Size {
      get { return size_; }
      set {
        size_ = value;
      }
    }

    /// <summary>Field number for the "EqtOrFut" field.</summary>
    public const int EqtOrFutFieldNumber = 6;
    private bool eqtOrFut_;
    public bool EqtOrFut {
      get { return eqtOrFut_; }
      set {
        eqtOrFut_ = value;
      }
    }

    /// <summary>Field number for the "futureFlag" field.</summary>
    public const int FutureFlagFieldNumber = 7;
    private string futureFlag_ = "";
    public string FutureFlag {
      get { return futureFlag_; }
      set {
        futureFlag_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "futureAccountType" field.</summary>
    public const int FutureAccountTypeFieldNumber = 8;
    private string futureAccountType_ = "";
    public string FutureAccountType {
      get { return futureAccountType_; }
      set {
        futureAccountType_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    public override bool Equals(object other) {
      return Equals(other as PBMsgAlgoOrder);
    }

    public bool Equals(PBMsgAlgoOrder other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (Symbol != other.Symbol) return false;
      if (EuityMarket != other.EuityMarket) return false;
      if (OrderDirection != other.OrderDirection) return false;
      if (PriceType != other.PriceType) return false;
      if (Size != other.Size) return false;
      if (EqtOrFut != other.EqtOrFut) return false;
      if (FutureFlag != other.FutureFlag) return false;
      if (FutureAccountType != other.FutureAccountType) return false;
      return true;
    }

    public override int GetHashCode() {
      int hash = 1;
      if (Symbol.Length != 0) hash ^= Symbol.GetHashCode();
      if (EuityMarket.Length != 0) hash ^= EuityMarket.GetHashCode();
      if (OrderDirection.Length != 0) hash ^= OrderDirection.GetHashCode();
      if (PriceType.Length != 0) hash ^= PriceType.GetHashCode();
      if (Size != 0) hash ^= Size.GetHashCode();
      if (EqtOrFut != false) hash ^= EqtOrFut.GetHashCode();
      if (FutureFlag.Length != 0) hash ^= FutureFlag.GetHashCode();
      if (FutureAccountType.Length != 0) hash ^= FutureAccountType.GetHashCode();
      return hash;
    }

    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    public void WriteTo(pb::CodedOutputStream output) {
      if (Symbol.Length != 0) {
        output.WriteRawTag(10);
        output.WriteString(Symbol);
      }
      if (EuityMarket.Length != 0) {
        output.WriteRawTag(18);
        output.WriteString(EuityMarket);
      }
      if (OrderDirection.Length != 0) {
        output.WriteRawTag(26);
        output.WriteString(OrderDirection);
      }
      if (PriceType.Length != 0) {
        output.WriteRawTag(34);
        output.WriteString(PriceType);
      }
      if (Size != 0) {
        output.WriteRawTag(40);
        output.WriteInt32(Size);
      }
      if (EqtOrFut != false) {
        output.WriteRawTag(48);
        output.WriteBool(EqtOrFut);
      }
      if (FutureFlag.Length != 0) {
        output.WriteRawTag(58);
        output.WriteString(FutureFlag);
      }
      if (FutureAccountType.Length != 0) {
        output.WriteRawTag(66);
        output.WriteString(FutureAccountType);
      }
    }

    public int CalculateSize() {
      int size = 0;
      if (Symbol.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(Symbol);
      }
      if (EuityMarket.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(EuityMarket);
      }
      if (OrderDirection.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(OrderDirection);
      }
      if (PriceType.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(PriceType);
      }
      if (Size != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(Size);
      }
      if (EqtOrFut != false) {
        size += 1 + 1;
      }
      if (FutureFlag.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(FutureFlag);
      }
      if (FutureAccountType.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(FutureAccountType);
      }
      return size;
    }

    public void MergeFrom(PBMsgAlgoOrder other) {
      if (other == null) {
        return;
      }
      if (other.Symbol.Length != 0) {
        Symbol = other.Symbol;
      }
      if (other.EuityMarket.Length != 0) {
        EuityMarket = other.EuityMarket;
      }
      if (other.OrderDirection.Length != 0) {
        OrderDirection = other.OrderDirection;
      }
      if (other.PriceType.Length != 0) {
        PriceType = other.PriceType;
      }
      if (other.Size != 0) {
        Size = other.Size;
      }
      if (other.EqtOrFut != false) {
        EqtOrFut = other.EqtOrFut;
      }
      if (other.FutureFlag.Length != 0) {
        FutureFlag = other.FutureFlag;
      }
      if (other.FutureAccountType.Length != 0) {
        FutureAccountType = other.FutureAccountType;
      }
    }

    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            input.SkipLastField();
            break;
          case 10: {
            Symbol = input.ReadString();
            break;
          }
          case 18: {
            EuityMarket = input.ReadString();
            break;
          }
          case 26: {
            OrderDirection = input.ReadString();
            break;
          }
          case 34: {
            PriceType = input.ReadString();
            break;
          }
          case 40: {
            Size = input.ReadInt32();
            break;
          }
          case 48: {
            EqtOrFut = input.ReadBool();
            break;
          }
          case 58: {
            FutureFlag = input.ReadString();
            break;
          }
          case 66: {
            FutureAccountType = input.ReadString();
            break;
          }
        }
      }
    }

  }

  /// <summary>
  /// 批量下单
  /// </summary>
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
  public sealed partial class PBMsgAlgoOrderList : pb::IMessage<PBMsgAlgoOrderList> {
    private static readonly pb::MessageParser<PBMsgAlgoOrderList> _parser = new pb::MessageParser<PBMsgAlgoOrderList>(() => new PBMsgAlgoOrderList());
    public static pb::MessageParser<PBMsgAlgoOrderList> Parser { get { return _parser; } }

    public static pbr::MessageDescriptor Descriptor {
      get { return global::PBWrapMsgAlgo.PBMsgAlgoReflection.Descriptor.MessageTypes[1]; }
    }

    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    public PBMsgAlgoOrderList() {
      OnConstruction();
    }

    partial void OnConstruction();

    public PBMsgAlgoOrderList(PBMsgAlgoOrderList other) : this() {
      messageId_ = other.messageId_;
      messageService_ = other.messageService_;
      messageType_ = other.messageType_;
      parentOrderId_ = other.parentOrderId_;
      batNewOrderList_ = other.batNewOrderList_.Clone();
      eqtBuyChangePercent_ = other.eqtBuyChangePercent_;
      eqtSellChangePercent_ = other.eqtSellChangePercent_;
      futBuyChangePoint_ = other.futBuyChangePoint_;
      futSellChangePoint_ = other.futSellChangePoint_;
    }

    public PBMsgAlgoOrderList Clone() {
      return new PBMsgAlgoOrderList(this);
    }

    /// <summary>Field number for the "messageId" field.</summary>
    public const int MessageIdFieldNumber = 1;
    private int messageId_;
    /// <summary>
    /// Message Header
    /// </summary>
    public int MessageId {
      get { return messageId_; }
      set {
        messageId_ = value;
      }
    }

    /// <summary>Field number for the "messageService" field.</summary>
    public const int MessageServiceFieldNumber = 2;
    private string messageService_ = "";
    public string MessageService {
      get { return messageService_; }
      set {
        messageService_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "messageType" field.</summary>
    public const int MessageTypeFieldNumber = 3;
    private global::PBWrapMsgAlgo.MessageType messageType_ = 0;
    /// <summary>
    /// Message Body
    /// </summary>
    public global::PBWrapMsgAlgo.MessageType MessageType {
      get { return messageType_; }
      set {
        messageType_ = value;
      }
    }

    /// <summary>Field number for the "parentOrderId" field.</summary>
    public const int ParentOrderIdFieldNumber = 4;
    private string parentOrderId_ = "";
    public string ParentOrderId {
      get { return parentOrderId_; }
      set {
        parentOrderId_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "batNewOrderList" field.</summary>
    public const int BatNewOrderListFieldNumber = 5;
    private static readonly pb::FieldCodec<global::PBWrapMsgAlgo.PBMsgAlgoOrder> _repeated_batNewOrderList_codec
        = pb::FieldCodec.ForMessage(42, global::PBWrapMsgAlgo.PBMsgAlgoOrder.Parser);
    private readonly pbc::RepeatedField<global::PBWrapMsgAlgo.PBMsgAlgoOrder> batNewOrderList_ = new pbc::RepeatedField<global::PBWrapMsgAlgo.PBMsgAlgoOrder>();
    /// <summary>
    /// Batch Order
    /// </summary>
    public pbc::RepeatedField<global::PBWrapMsgAlgo.PBMsgAlgoOrder> BatNewOrderList {
      get { return batNewOrderList_; }
    }

    /// <summary>Field number for the "eqtBuyChangePercent" field.</summary>
    public const int EqtBuyChangePercentFieldNumber = 6;
    private double eqtBuyChangePercent_;
    public double EqtBuyChangePercent {
      get { return eqtBuyChangePercent_; }
      set {
        eqtBuyChangePercent_ = value;
      }
    }

    /// <summary>Field number for the "eqtSellChangePercent" field.</summary>
    public const int EqtSellChangePercentFieldNumber = 7;
    private double eqtSellChangePercent_;
    public double EqtSellChangePercent {
      get { return eqtSellChangePercent_; }
      set {
        eqtSellChangePercent_ = value;
      }
    }

    /// <summary>Field number for the "futBuyChangePoint" field.</summary>
    public const int FutBuyChangePointFieldNumber = 8;
    private int futBuyChangePoint_;
    public int FutBuyChangePoint {
      get { return futBuyChangePoint_; }
      set {
        futBuyChangePoint_ = value;
      }
    }

    /// <summary>Field number for the "futSellChangePoint" field.</summary>
    public const int FutSellChangePointFieldNumber = 9;
    private int futSellChangePoint_;
    public int FutSellChangePoint {
      get { return futSellChangePoint_; }
      set {
        futSellChangePoint_ = value;
      }
    }

    public override bool Equals(object other) {
      return Equals(other as PBMsgAlgoOrderList);
    }

    public bool Equals(PBMsgAlgoOrderList other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (MessageId != other.MessageId) return false;
      if (MessageService != other.MessageService) return false;
      if (MessageType != other.MessageType) return false;
      if (ParentOrderId != other.ParentOrderId) return false;
      if(!batNewOrderList_.Equals(other.batNewOrderList_)) return false;
      if (EqtBuyChangePercent != other.EqtBuyChangePercent) return false;
      if (EqtSellChangePercent != other.EqtSellChangePercent) return false;
      if (FutBuyChangePoint != other.FutBuyChangePoint) return false;
      if (FutSellChangePoint != other.FutSellChangePoint) return false;
      return true;
    }

    public override int GetHashCode() {
      int hash = 1;
      if (MessageId != 0) hash ^= MessageId.GetHashCode();
      if (MessageService.Length != 0) hash ^= MessageService.GetHashCode();
      if (MessageType != 0) hash ^= MessageType.GetHashCode();
      if (ParentOrderId.Length != 0) hash ^= ParentOrderId.GetHashCode();
      hash ^= batNewOrderList_.GetHashCode();
      if (EqtBuyChangePercent != 0D) hash ^= EqtBuyChangePercent.GetHashCode();
      if (EqtSellChangePercent != 0D) hash ^= EqtSellChangePercent.GetHashCode();
      if (FutBuyChangePoint != 0) hash ^= FutBuyChangePoint.GetHashCode();
      if (FutSellChangePoint != 0) hash ^= FutSellChangePoint.GetHashCode();
      return hash;
    }

    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    public void WriteTo(pb::CodedOutputStream output) {
      if (MessageId != 0) {
        output.WriteRawTag(8);
        output.WriteInt32(MessageId);
      }
      if (MessageService.Length != 0) {
        output.WriteRawTag(18);
        output.WriteString(MessageService);
      }
      if (MessageType != 0) {
        output.WriteRawTag(24);
        output.WriteEnum((int) MessageType);
      }
      if (ParentOrderId.Length != 0) {
        output.WriteRawTag(34);
        output.WriteString(ParentOrderId);
      }
      batNewOrderList_.WriteTo(output, _repeated_batNewOrderList_codec);
      if (EqtBuyChangePercent != 0D) {
        output.WriteRawTag(49);
        output.WriteDouble(EqtBuyChangePercent);
      }
      if (EqtSellChangePercent != 0D) {
        output.WriteRawTag(57);
        output.WriteDouble(EqtSellChangePercent);
      }
      if (FutBuyChangePoint != 0) {
        output.WriteRawTag(64);
        output.WriteSInt32(FutBuyChangePoint);
      }
      if (FutSellChangePoint != 0) {
        output.WriteRawTag(72);
        output.WriteSInt32(FutSellChangePoint);
      }
    }

    public int CalculateSize() {
      int size = 0;
      if (MessageId != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(MessageId);
      }
      if (MessageService.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(MessageService);
      }
      if (MessageType != 0) {
        size += 1 + pb::CodedOutputStream.ComputeEnumSize((int) MessageType);
      }
      if (ParentOrderId.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(ParentOrderId);
      }
      size += batNewOrderList_.CalculateSize(_repeated_batNewOrderList_codec);
      if (EqtBuyChangePercent != 0D) {
        size += 1 + 8;
      }
      if (EqtSellChangePercent != 0D) {
        size += 1 + 8;
      }
      if (FutBuyChangePoint != 0) {
        size += 1 + pb::CodedOutputStream.ComputeSInt32Size(FutBuyChangePoint);
      }
      if (FutSellChangePoint != 0) {
        size += 1 + pb::CodedOutputStream.ComputeSInt32Size(FutSellChangePoint);
      }
      return size;
    }

    public void MergeFrom(PBMsgAlgoOrderList other) {
      if (other == null) {
        return;
      }
      if (other.MessageId != 0) {
        MessageId = other.MessageId;
      }
      if (other.MessageService.Length != 0) {
        MessageService = other.MessageService;
      }
      if (other.MessageType != 0) {
        MessageType = other.MessageType;
      }
      if (other.ParentOrderId.Length != 0) {
        ParentOrderId = other.ParentOrderId;
      }
      batNewOrderList_.Add(other.batNewOrderList_);
      if (other.EqtBuyChangePercent != 0D) {
        EqtBuyChangePercent = other.EqtBuyChangePercent;
      }
      if (other.EqtSellChangePercent != 0D) {
        EqtSellChangePercent = other.EqtSellChangePercent;
      }
      if (other.FutBuyChangePoint != 0) {
        FutBuyChangePoint = other.FutBuyChangePoint;
      }
      if (other.FutSellChangePoint != 0) {
        FutSellChangePoint = other.FutSellChangePoint;
      }
    }

    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            input.SkipLastField();
            break;
          case 8: {
            MessageId = input.ReadInt32();
            break;
          }
          case 18: {
            MessageService = input.ReadString();
            break;
          }
          case 24: {
            messageType_ = (global::PBWrapMsgAlgo.MessageType) input.ReadEnum();
            break;
          }
          case 34: {
            ParentOrderId = input.ReadString();
            break;
          }
          case 42: {
            batNewOrderList_.AddEntriesFrom(input, _repeated_batNewOrderList_codec);
            break;
          }
          case 49: {
            EqtBuyChangePercent = input.ReadDouble();
            break;
          }
          case 57: {
            EqtSellChangePercent = input.ReadDouble();
            break;
          }
          case 64: {
            FutBuyChangePoint = input.ReadSInt32();
            break;
          }
          case 72: {
            FutSellChangePoint = input.ReadSInt32();
            break;
          }
        }
      }
    }

  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
  public sealed partial class PBMsgCandle : pb::IMessage<PBMsgCandle> {
    private static readonly pb::MessageParser<PBMsgCandle> _parser = new pb::MessageParser<PBMsgCandle>(() => new PBMsgCandle());
    public static pb::MessageParser<PBMsgCandle> Parser { get { return _parser; } }

    public static pbr::MessageDescriptor Descriptor {
      get { return global::PBWrapMsgAlgo.PBMsgAlgoReflection.Descriptor.MessageTypes[2]; }
    }

    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    public PBMsgCandle() {
      OnConstruction();
    }

    partial void OnConstruction();

    public PBMsgCandle(PBMsgCandle other) : this() {
      timestamp_ = other.timestamp_;
      open_ = other.open_;
      close_ = other.close_;
      high_ = other.high_;
      low_ = other.low_;
    }

    public PBMsgCandle Clone() {
      return new PBMsgCandle(this);
    }

    /// <summary>Field number for the "timestamp" field.</summary>
    public const int TimestampFieldNumber = 1;
    private string timestamp_ = "";
    public string Timestamp {
      get { return timestamp_; }
      set {
        timestamp_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "open" field.</summary>
    public const int OpenFieldNumber = 2;
    private int open_;
    public int Open {
      get { return open_; }
      set {
        open_ = value;
      }
    }

    /// <summary>Field number for the "close" field.</summary>
    public const int CloseFieldNumber = 3;
    private int close_;
    public int Close {
      get { return close_; }
      set {
        close_ = value;
      }
    }

    /// <summary>Field number for the "high" field.</summary>
    public const int HighFieldNumber = 4;
    private int high_;
    public int High {
      get { return high_; }
      set {
        high_ = value;
      }
    }

    /// <summary>Field number for the "low" field.</summary>
    public const int LowFieldNumber = 5;
    private int low_;
    public int Low {
      get { return low_; }
      set {
        low_ = value;
      }
    }

    public override bool Equals(object other) {
      return Equals(other as PBMsgCandle);
    }

    public bool Equals(PBMsgCandle other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (Timestamp != other.Timestamp) return false;
      if (Open != other.Open) return false;
      if (Close != other.Close) return false;
      if (High != other.High) return false;
      if (Low != other.Low) return false;
      return true;
    }

    public override int GetHashCode() {
      int hash = 1;
      if (Timestamp.Length != 0) hash ^= Timestamp.GetHashCode();
      if (Open != 0) hash ^= Open.GetHashCode();
      if (Close != 0) hash ^= Close.GetHashCode();
      if (High != 0) hash ^= High.GetHashCode();
      if (Low != 0) hash ^= Low.GetHashCode();
      return hash;
    }

    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    public void WriteTo(pb::CodedOutputStream output) {
      if (Timestamp.Length != 0) {
        output.WriteRawTag(10);
        output.WriteString(Timestamp);
      }
      if (Open != 0) {
        output.WriteRawTag(16);
        output.WriteInt32(Open);
      }
      if (Close != 0) {
        output.WriteRawTag(24);
        output.WriteInt32(Close);
      }
      if (High != 0) {
        output.WriteRawTag(32);
        output.WriteInt32(High);
      }
      if (Low != 0) {
        output.WriteRawTag(40);
        output.WriteInt32(Low);
      }
    }

    public int CalculateSize() {
      int size = 0;
      if (Timestamp.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(Timestamp);
      }
      if (Open != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(Open);
      }
      if (Close != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(Close);
      }
      if (High != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(High);
      }
      if (Low != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(Low);
      }
      return size;
    }

    public void MergeFrom(PBMsgCandle other) {
      if (other == null) {
        return;
      }
      if (other.Timestamp.Length != 0) {
        Timestamp = other.Timestamp;
      }
      if (other.Open != 0) {
        Open = other.Open;
      }
      if (other.Close != 0) {
        Close = other.Close;
      }
      if (other.High != 0) {
        High = other.High;
      }
      if (other.Low != 0) {
        Low = other.Low;
      }
    }

    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            input.SkipLastField();
            break;
          case 10: {
            Timestamp = input.ReadString();
            break;
          }
          case 16: {
            Open = input.ReadInt32();
            break;
          }
          case 24: {
            Close = input.ReadInt32();
            break;
          }
          case 32: {
            High = input.ReadInt32();
            break;
          }
          case 40: {
            Low = input.ReadInt32();
            break;
          }
        }
      }
    }

  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
  public sealed partial class PBMsgCandleQueue : pb::IMessage<PBMsgCandleQueue> {
    private static readonly pb::MessageParser<PBMsgCandleQueue> _parser = new pb::MessageParser<PBMsgCandleQueue>(() => new PBMsgCandleQueue());
    public static pb::MessageParser<PBMsgCandleQueue> Parser { get { return _parser; } }

    public static pbr::MessageDescriptor Descriptor {
      get { return global::PBWrapMsgAlgo.PBMsgAlgoReflection.Descriptor.MessageTypes[3]; }
    }

    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    public PBMsgCandleQueue() {
      OnConstruction();
    }

    partial void OnConstruction();

    public PBMsgCandleQueue(PBMsgCandleQueue other) : this() {
      ric_ = other.ric_;
      candles_ = other.candles_.Clone();
    }

    public PBMsgCandleQueue Clone() {
      return new PBMsgCandleQueue(this);
    }

    /// <summary>Field number for the "ric" field.</summary>
    public const int RicFieldNumber = 1;
    private string ric_ = "";
    public string Ric {
      get { return ric_; }
      set {
        ric_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "candles" field.</summary>
    public const int CandlesFieldNumber = 2;
    private static readonly pb::FieldCodec<global::PBWrapMsgAlgo.PBMsgCandle> _repeated_candles_codec
        = pb::FieldCodec.ForMessage(18, global::PBWrapMsgAlgo.PBMsgCandle.Parser);
    private readonly pbc::RepeatedField<global::PBWrapMsgAlgo.PBMsgCandle> candles_ = new pbc::RepeatedField<global::PBWrapMsgAlgo.PBMsgCandle>();
    public pbc::RepeatedField<global::PBWrapMsgAlgo.PBMsgCandle> Candles {
      get { return candles_; }
    }

    public override bool Equals(object other) {
      return Equals(other as PBMsgCandleQueue);
    }

    public bool Equals(PBMsgCandleQueue other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (Ric != other.Ric) return false;
      if(!candles_.Equals(other.candles_)) return false;
      return true;
    }

    public override int GetHashCode() {
      int hash = 1;
      if (Ric.Length != 0) hash ^= Ric.GetHashCode();
      hash ^= candles_.GetHashCode();
      return hash;
    }

    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    public void WriteTo(pb::CodedOutputStream output) {
      if (Ric.Length != 0) {
        output.WriteRawTag(10);
        output.WriteString(Ric);
      }
      candles_.WriteTo(output, _repeated_candles_codec);
    }

    public int CalculateSize() {
      int size = 0;
      if (Ric.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(Ric);
      }
      size += candles_.CalculateSize(_repeated_candles_codec);
      return size;
    }

    public void MergeFrom(PBMsgCandleQueue other) {
      if (other == null) {
        return;
      }
      if (other.Ric.Length != 0) {
        Ric = other.Ric;
      }
      candles_.Add(other.candles_);
    }

    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            input.SkipLastField();
            break;
          case 10: {
            Ric = input.ReadString();
            break;
          }
          case 18: {
            candles_.AddEntriesFrom(input, _repeated_candles_codec);
            break;
          }
        }
      }
    }

  }

  #endregion

}

#endregion Designer generated code
