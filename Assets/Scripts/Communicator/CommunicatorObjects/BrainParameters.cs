// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: files/brain_parameters.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace UPC.CommunicatorObjects {

  /// <summary>Holder for reflection information generated from files/brain_parameters.proto</summary>
  public static partial class BrainParametersReflection {

    #region Descriptor
    /// <summary>File descriptor for files/brain_parameters.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static BrainParametersReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "ChxmaWxlcy9icmFpbl9wYXJhbWV0ZXJzLnByb3RvEhRjb21tdW5pY2F0b3Jf",
            "b2JqZWN0cyKrAgoUQnJhaW5QYXJhbWV0ZXJzUHJvdG8SEgoKYnJhaW5fbmFt",
            "ZRgBIAEoCRIUCgxhZ2VudHNfY291bnQYAiABKAUSIAoYb2JzZXJ2YXRpb25z",
            "X3ZlY3Rvcl9zaXplGAMgASgFEhsKE2FjdGlvbnNfdmVjdG9yX3NpemUYBCAB",
            "KAUSHwoXbW1mX29mZnNldF9vYnNlcnZhdGlvbnMYBSABKAUSHQoVbW1mX3Np",
            "emVfb2JzZXJ2YXRpb25zGAYgASgFEhoKEm1tZl9vZmZzZXRfYWN0aW9ucxgH",
            "IAEoBRIYChBtbWZfc2l6ZV9hY3Rpb25zGAggASgFEhoKEm1tZl9vZmZzZXRf",
            "Zml0bmVzcxgJIAEoBRIYChBtbWZfc2l6ZV9maXRuZXNzGAogASgFQhqqAhdV",
            "UEMuQ29tbXVuaWNhdG9yT2JqZWN0c2IGcHJvdG8z"));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { },
          new pbr::GeneratedClrTypeInfo(null, null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::UPC.CommunicatorObjects.BrainParametersProto), global::UPC.CommunicatorObjects.BrainParametersProto.Parser, new[]{ "BrainName", "AgentsCount", "ObservationsVectorSize", "ActionsVectorSize", "MmfOffsetObservations", "MmfSizeObservations", "MmfOffsetActions", "MmfSizeActions", "MmfOffsetFitness", "MmfSizeFitness" }, null, null, null, null)
          }));
    }
    #endregion

  }
  #region Messages
  public sealed partial class BrainParametersProto : pb::IMessage<BrainParametersProto> {
    private static readonly pb::MessageParser<BrainParametersProto> _parser = new pb::MessageParser<BrainParametersProto>(() => new BrainParametersProto());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<BrainParametersProto> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::UPC.CommunicatorObjects.BrainParametersReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public BrainParametersProto() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public BrainParametersProto(BrainParametersProto other) : this() {
      brainName_ = other.brainName_;
      agentsCount_ = other.agentsCount_;
      observationsVectorSize_ = other.observationsVectorSize_;
      actionsVectorSize_ = other.actionsVectorSize_;
      mmfOffsetObservations_ = other.mmfOffsetObservations_;
      mmfSizeObservations_ = other.mmfSizeObservations_;
      mmfOffsetActions_ = other.mmfOffsetActions_;
      mmfSizeActions_ = other.mmfSizeActions_;
      mmfOffsetFitness_ = other.mmfOffsetFitness_;
      mmfSizeFitness_ = other.mmfSizeFitness_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public BrainParametersProto Clone() {
      return new BrainParametersProto(this);
    }

    /// <summary>Field number for the "brain_name" field.</summary>
    public const int BrainNameFieldNumber = 1;
    private string brainName_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string BrainName {
      get { return brainName_; }
      set {
        brainName_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "agents_count" field.</summary>
    public const int AgentsCountFieldNumber = 2;
    private int agentsCount_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int AgentsCount {
      get { return agentsCount_; }
      set {
        agentsCount_ = value;
      }
    }

    /// <summary>Field number for the "observations_vector_size" field.</summary>
    public const int ObservationsVectorSizeFieldNumber = 3;
    private int observationsVectorSize_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int ObservationsVectorSize {
      get { return observationsVectorSize_; }
      set {
        observationsVectorSize_ = value;
      }
    }

    /// <summary>Field number for the "actions_vector_size" field.</summary>
    public const int ActionsVectorSizeFieldNumber = 4;
    private int actionsVectorSize_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int ActionsVectorSize {
      get { return actionsVectorSize_; }
      set {
        actionsVectorSize_ = value;
      }
    }

    /// <summary>Field number for the "mmf_offset_observations" field.</summary>
    public const int MmfOffsetObservationsFieldNumber = 5;
    private int mmfOffsetObservations_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int MmfOffsetObservations {
      get { return mmfOffsetObservations_; }
      set {
        mmfOffsetObservations_ = value;
      }
    }

    /// <summary>Field number for the "mmf_size_observations" field.</summary>
    public const int MmfSizeObservationsFieldNumber = 6;
    private int mmfSizeObservations_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int MmfSizeObservations {
      get { return mmfSizeObservations_; }
      set {
        mmfSizeObservations_ = value;
      }
    }

    /// <summary>Field number for the "mmf_offset_actions" field.</summary>
    public const int MmfOffsetActionsFieldNumber = 7;
    private int mmfOffsetActions_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int MmfOffsetActions {
      get { return mmfOffsetActions_; }
      set {
        mmfOffsetActions_ = value;
      }
    }

    /// <summary>Field number for the "mmf_size_actions" field.</summary>
    public const int MmfSizeActionsFieldNumber = 8;
    private int mmfSizeActions_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int MmfSizeActions {
      get { return mmfSizeActions_; }
      set {
        mmfSizeActions_ = value;
      }
    }

    /// <summary>Field number for the "mmf_offset_fitness" field.</summary>
    public const int MmfOffsetFitnessFieldNumber = 9;
    private int mmfOffsetFitness_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int MmfOffsetFitness {
      get { return mmfOffsetFitness_; }
      set {
        mmfOffsetFitness_ = value;
      }
    }

    /// <summary>Field number for the "mmf_size_fitness" field.</summary>
    public const int MmfSizeFitnessFieldNumber = 10;
    private int mmfSizeFitness_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int MmfSizeFitness {
      get { return mmfSizeFitness_; }
      set {
        mmfSizeFitness_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as BrainParametersProto);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(BrainParametersProto other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (BrainName != other.BrainName) return false;
      if (AgentsCount != other.AgentsCount) return false;
      if (ObservationsVectorSize != other.ObservationsVectorSize) return false;
      if (ActionsVectorSize != other.ActionsVectorSize) return false;
      if (MmfOffsetObservations != other.MmfOffsetObservations) return false;
      if (MmfSizeObservations != other.MmfSizeObservations) return false;
      if (MmfOffsetActions != other.MmfOffsetActions) return false;
      if (MmfSizeActions != other.MmfSizeActions) return false;
      if (MmfOffsetFitness != other.MmfOffsetFitness) return false;
      if (MmfSizeFitness != other.MmfSizeFitness) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (BrainName.Length != 0) hash ^= BrainName.GetHashCode();
      if (AgentsCount != 0) hash ^= AgentsCount.GetHashCode();
      if (ObservationsVectorSize != 0) hash ^= ObservationsVectorSize.GetHashCode();
      if (ActionsVectorSize != 0) hash ^= ActionsVectorSize.GetHashCode();
      if (MmfOffsetObservations != 0) hash ^= MmfOffsetObservations.GetHashCode();
      if (MmfSizeObservations != 0) hash ^= MmfSizeObservations.GetHashCode();
      if (MmfOffsetActions != 0) hash ^= MmfOffsetActions.GetHashCode();
      if (MmfSizeActions != 0) hash ^= MmfSizeActions.GetHashCode();
      if (MmfOffsetFitness != 0) hash ^= MmfOffsetFitness.GetHashCode();
      if (MmfSizeFitness != 0) hash ^= MmfSizeFitness.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (BrainName.Length != 0) {
        output.WriteRawTag(10);
        output.WriteString(BrainName);
      }
      if (AgentsCount != 0) {
        output.WriteRawTag(16);
        output.WriteInt32(AgentsCount);
      }
      if (ObservationsVectorSize != 0) {
        output.WriteRawTag(24);
        output.WriteInt32(ObservationsVectorSize);
      }
      if (ActionsVectorSize != 0) {
        output.WriteRawTag(32);
        output.WriteInt32(ActionsVectorSize);
      }
      if (MmfOffsetObservations != 0) {
        output.WriteRawTag(40);
        output.WriteInt32(MmfOffsetObservations);
      }
      if (MmfSizeObservations != 0) {
        output.WriteRawTag(48);
        output.WriteInt32(MmfSizeObservations);
      }
      if (MmfOffsetActions != 0) {
        output.WriteRawTag(56);
        output.WriteInt32(MmfOffsetActions);
      }
      if (MmfSizeActions != 0) {
        output.WriteRawTag(64);
        output.WriteInt32(MmfSizeActions);
      }
      if (MmfOffsetFitness != 0) {
        output.WriteRawTag(72);
        output.WriteInt32(MmfOffsetFitness);
      }
      if (MmfSizeFitness != 0) {
        output.WriteRawTag(80);
        output.WriteInt32(MmfSizeFitness);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (BrainName.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(BrainName);
      }
      if (AgentsCount != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(AgentsCount);
      }
      if (ObservationsVectorSize != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(ObservationsVectorSize);
      }
      if (ActionsVectorSize != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(ActionsVectorSize);
      }
      if (MmfOffsetObservations != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(MmfOffsetObservations);
      }
      if (MmfSizeObservations != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(MmfSizeObservations);
      }
      if (MmfOffsetActions != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(MmfOffsetActions);
      }
      if (MmfSizeActions != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(MmfSizeActions);
      }
      if (MmfOffsetFitness != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(MmfOffsetFitness);
      }
      if (MmfSizeFitness != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(MmfSizeFitness);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(BrainParametersProto other) {
      if (other == null) {
        return;
      }
      if (other.BrainName.Length != 0) {
        BrainName = other.BrainName;
      }
      if (other.AgentsCount != 0) {
        AgentsCount = other.AgentsCount;
      }
      if (other.ObservationsVectorSize != 0) {
        ObservationsVectorSize = other.ObservationsVectorSize;
      }
      if (other.ActionsVectorSize != 0) {
        ActionsVectorSize = other.ActionsVectorSize;
      }
      if (other.MmfOffsetObservations != 0) {
        MmfOffsetObservations = other.MmfOffsetObservations;
      }
      if (other.MmfSizeObservations != 0) {
        MmfSizeObservations = other.MmfSizeObservations;
      }
      if (other.MmfOffsetActions != 0) {
        MmfOffsetActions = other.MmfOffsetActions;
      }
      if (other.MmfSizeActions != 0) {
        MmfSizeActions = other.MmfSizeActions;
      }
      if (other.MmfOffsetFitness != 0) {
        MmfOffsetFitness = other.MmfOffsetFitness;
      }
      if (other.MmfSizeFitness != 0) {
        MmfSizeFitness = other.MmfSizeFitness;
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 10: {
            BrainName = input.ReadString();
            break;
          }
          case 16: {
            AgentsCount = input.ReadInt32();
            break;
          }
          case 24: {
            ObservationsVectorSize = input.ReadInt32();
            break;
          }
          case 32: {
            ActionsVectorSize = input.ReadInt32();
            break;
          }
          case 40: {
            MmfOffsetObservations = input.ReadInt32();
            break;
          }
          case 48: {
            MmfSizeObservations = input.ReadInt32();
            break;
          }
          case 56: {
            MmfOffsetActions = input.ReadInt32();
            break;
          }
          case 64: {
            MmfSizeActions = input.ReadInt32();
            break;
          }
          case 72: {
            MmfOffsetFitness = input.ReadInt32();
            break;
          }
          case 80: {
            MmfSizeFitness = input.ReadInt32();
            break;
          }
        }
      }
    }

  }

  #endregion

}

#endregion Designer generated code
