// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: mlagents/envs/communicator_objects/unity_initialization_output.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace MLAgents.CommunicatorObjects {

  /// <summary>Holder for reflection information generated from mlagents/envs/communicator_objects/unity_initialization_output.proto</summary>
  public static partial class UnityInitializationOutputReflection {

    #region Descriptor
    /// <summary>File descriptor for mlagents/envs/communicator_objects/unity_initialization_output.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static UnityInitializationOutputReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "CkRtbGFnZW50cy9lbnZzL2NvbW11bmljYXRvcl9vYmplY3RzL3VuaXR5X2lu",
            "aXRpYWxpemF0aW9uX291dHB1dC5wcm90bxIUY29tbXVuaWNhdG9yX29iamVj",
            "dHMaOW1sYWdlbnRzL2VudnMvY29tbXVuaWNhdG9yX29iamVjdHMvYnJhaW5f",
            "cGFyYW1ldGVycy5wcm90bxo/bWxhZ2VudHMvZW52cy9jb21tdW5pY2F0b3Jf",
            "b2JqZWN0cy9lbnZpcm9ubWVudF9wYXJhbWV0ZXJzLnByb3RvIsYBCh5Vbml0",
            "eUluaXRpYWxpemF0aW9uT3V0cHV0UHJvdG8SDAoEbmFtZRgBIAEoCRJEChBi",
            "cmFpbl9wYXJhbWV0ZXJzGAIgAygLMiouY29tbXVuaWNhdG9yX29iamVjdHMu",
            "QnJhaW5QYXJhbWV0ZXJzUHJvdG8SUAoWZW52aXJvbm1lbnRfcGFyYW1ldGVy",
            "cxgDIAEoCzIwLmNvbW11bmljYXRvcl9vYmplY3RzLkVudmlyb25tZW50UGFy",
            "YW1ldGVyc1Byb3RvQh+qAhxNTEFnZW50cy5Db21tdW5pY2F0b3JPYmplY3Rz",
            "YgZwcm90bzM="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { global::MLAgents.CommunicatorObjects.BrainParametersReflection.Descriptor, global::MLAgents.CommunicatorObjects.EnvironmentParametersReflection.Descriptor, },
          new pbr::GeneratedClrTypeInfo(null, null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::MLAgents.CommunicatorObjects.UnityInitializationOutputProto), global::MLAgents.CommunicatorObjects.UnityInitializationOutputProto.Parser, new[]{ "Name", "BrainParameters", "EnvironmentParameters" }, null, null, null, null)
          }));
    }
    #endregion

  }
  #region Messages
  public sealed partial class UnityInitializationOutputProto : pb::IMessage<UnityInitializationOutputProto> {
    private static readonly pb::MessageParser<UnityInitializationOutputProto> _parser = new pb::MessageParser<UnityInitializationOutputProto>(() => new UnityInitializationOutputProto());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<UnityInitializationOutputProto> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::MLAgents.CommunicatorObjects.UnityInitializationOutputReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public UnityInitializationOutputProto() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public UnityInitializationOutputProto(UnityInitializationOutputProto other) : this() {
      name_ = other.name_;
      brainParameters_ = other.brainParameters_.Clone();
      environmentParameters_ = other.environmentParameters_ != null ? other.environmentParameters_.Clone() : null;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public UnityInitializationOutputProto Clone() {
      return new UnityInitializationOutputProto(this);
    }

    /// <summary>Field number for the "name" field.</summary>
    public const int NameFieldNumber = 1;
    private string name_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string Name {
      get { return name_; }
      set {
        name_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "brain_parameters" field.</summary>
    public const int BrainParametersFieldNumber = 2;
    private static readonly pb::FieldCodec<global::MLAgents.CommunicatorObjects.BrainParametersProto> _repeated_brainParameters_codec
        = pb::FieldCodec.ForMessage(18, global::MLAgents.CommunicatorObjects.BrainParametersProto.Parser);
    private readonly pbc::RepeatedField<global::MLAgents.CommunicatorObjects.BrainParametersProto> brainParameters_ = new pbc::RepeatedField<global::MLAgents.CommunicatorObjects.BrainParametersProto>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::RepeatedField<global::MLAgents.CommunicatorObjects.BrainParametersProto> BrainParameters {
      get { return brainParameters_; }
    }

    /// <summary>Field number for the "environment_parameters" field.</summary>
    public const int EnvironmentParametersFieldNumber = 3;
    private global::MLAgents.CommunicatorObjects.EnvironmentParametersProto environmentParameters_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::MLAgents.CommunicatorObjects.EnvironmentParametersProto EnvironmentParameters {
      get { return environmentParameters_; }
      set {
        environmentParameters_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as UnityInitializationOutputProto);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(UnityInitializationOutputProto other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (Name != other.Name) return false;
      if(!brainParameters_.Equals(other.brainParameters_)) return false;
      if (!object.Equals(EnvironmentParameters, other.EnvironmentParameters)) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (Name.Length != 0) hash ^= Name.GetHashCode();
      hash ^= brainParameters_.GetHashCode();
      if (environmentParameters_ != null) hash ^= EnvironmentParameters.GetHashCode();
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
      if (Name.Length != 0) {
        output.WriteRawTag(10);
        output.WriteString(Name);
      }
      brainParameters_.WriteTo(output, _repeated_brainParameters_codec);
      if (environmentParameters_ != null) {
        output.WriteRawTag(26);
        output.WriteMessage(EnvironmentParameters);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (Name.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(Name);
      }
      size += brainParameters_.CalculateSize(_repeated_brainParameters_codec);
      if (environmentParameters_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(EnvironmentParameters);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(UnityInitializationOutputProto other) {
      if (other == null) {
        return;
      }
      if (other.Name.Length != 0) {
        Name = other.Name;
      }
      brainParameters_.Add(other.brainParameters_);
      if (other.environmentParameters_ != null) {
        if (environmentParameters_ == null) {
          EnvironmentParameters = new global::MLAgents.CommunicatorObjects.EnvironmentParametersProto();
        }
        EnvironmentParameters.MergeFrom(other.EnvironmentParameters);
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
            Name = input.ReadString();
            break;
          }
          case 18: {
            brainParameters_.AddEntriesFrom(input, _repeated_brainParameters_codec);
            break;
          }
          case 26: {
            if (environmentParameters_ == null) {
              EnvironmentParameters = new global::MLAgents.CommunicatorObjects.EnvironmentParametersProto();
            }
            input.ReadMessage(EnvironmentParameters);
            break;
          }
        }
      }
    }

  }

  #endregion

}

#endregion Designer generated code