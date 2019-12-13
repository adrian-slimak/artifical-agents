// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: UnityOutput.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace ProtoObjects {

  /// <summary>Holder for reflection information generated from UnityOutput.proto</summary>
  public static partial class UnityOutputReflection {

    #region Descriptor
    /// <summary>File descriptor for UnityOutput.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static UnityOutputReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "ChFVbml0eU91dHB1dC5wcm90bxIUY29tbXVuaWNhdG9yX29iamVjdHMimAQK",
            "EFVuaXR5T3V0cHV0UHJvdG8SQgoGYnJhaW5zGAEgAygLMjIuY29tbXVuaWNh",
            "dG9yX29iamVjdHMuVW5pdHlPdXRwdXRQcm90by5CcmFpbnNFbnRyeRJkChVp",
            "bml0aWFsaXphdGlvbl9vdXRwdXQYAiABKAsyRS5jb21tdW5pY2F0b3Jfb2Jq",
            "ZWN0cy5Vbml0eU91dHB1dFByb3RvLlVuaXR5SW5pdGlhbGl6YXRpb25PdXRw",
            "dXRQcm90bxqgAQoOQWdlbnRMaXN0UHJvdG8SWgoKYWdlbnRzTGlzdBgEIAMo",
            "CzJGLmNvbW11bmljYXRvcl9vYmplY3RzLlVuaXR5T3V0cHV0UHJvdG8uQWdl",
            "bnRMaXN0UHJvdG8uT2JzZXJ2YXRpb25Qcm90bxoyChBPYnNlcnZhdGlvblBy",
            "b3RvEh4KEm9ic2VydmF0aW9uc1ZlY3RvchgDIAMoAkICEAEaUQoeVW5pdHlJ",
            "bml0aWFsaXphdGlvbk91dHB1dFByb3RvEgwKBG5hbWUYBSABKAkSDwoHdmVy",
            "c2lvbhgGIAEoCRIQCghsb2dfcGF0aBgHIAEoCRpkCgtCcmFpbnNFbnRyeRIL",
            "CgNrZXkYASABKAkSRAoFdmFsdWUYAiABKAsyNS5jb21tdW5pY2F0b3Jfb2Jq",
            "ZWN0cy5Vbml0eU91dHB1dFByb3RvLkFnZW50TGlzdFByb3RvOgI4AUIPqgIM",
            "UHJvdG9PYmplY3RzYgZwcm90bzM="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { },
          new pbr::GeneratedClrTypeInfo(null, null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::ProtoObjects.UnityOutputProto), global::ProtoObjects.UnityOutputProto.Parser, new[]{ "Brains", "InitializationOutput" }, null, null, null, new pbr::GeneratedClrTypeInfo[] { new pbr::GeneratedClrTypeInfo(typeof(global::ProtoObjects.UnityOutputProto.Types.AgentListProto), global::ProtoObjects.UnityOutputProto.Types.AgentListProto.Parser, new[]{ "AgentsList" }, null, null, null, new pbr::GeneratedClrTypeInfo[] { new pbr::GeneratedClrTypeInfo(typeof(global::ProtoObjects.UnityOutputProto.Types.AgentListProto.Types.ObservationProto), global::ProtoObjects.UnityOutputProto.Types.AgentListProto.Types.ObservationProto.Parser, new[]{ "ObservationsVector" }, null, null, null, null)}),
            new pbr::GeneratedClrTypeInfo(typeof(global::ProtoObjects.UnityOutputProto.Types.UnityInitializationOutputProto), global::ProtoObjects.UnityOutputProto.Types.UnityInitializationOutputProto.Parser, new[]{ "Name", "Version", "LogPath" }, null, null, null, null),
            null, })
          }));
    }
    #endregion

  }
  #region Messages
  public sealed partial class UnityOutputProto : pb::IMessage<UnityOutputProto> {
    private static readonly pb::MessageParser<UnityOutputProto> _parser = new pb::MessageParser<UnityOutputProto>(() => new UnityOutputProto());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<UnityOutputProto> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::ProtoObjects.UnityOutputReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public UnityOutputProto() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public UnityOutputProto(UnityOutputProto other) : this() {
      brains_ = other.brains_.Clone();
      initializationOutput_ = other.initializationOutput_ != null ? other.initializationOutput_.Clone() : null;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public UnityOutputProto Clone() {
      return new UnityOutputProto(this);
    }

    /// <summary>Field number for the "brains" field.</summary>
    public const int BrainsFieldNumber = 1;
    private static readonly pbc::MapField<string, global::ProtoObjects.UnityOutputProto.Types.AgentListProto>.Codec _map_brains_codec
        = new pbc::MapField<string, global::ProtoObjects.UnityOutputProto.Types.AgentListProto>.Codec(pb::FieldCodec.ForString(10, ""), pb::FieldCodec.ForMessage(18, global::ProtoObjects.UnityOutputProto.Types.AgentListProto.Parser), 10);
    private readonly pbc::MapField<string, global::ProtoObjects.UnityOutputProto.Types.AgentListProto> brains_ = new pbc::MapField<string, global::ProtoObjects.UnityOutputProto.Types.AgentListProto>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::MapField<string, global::ProtoObjects.UnityOutputProto.Types.AgentListProto> Brains {
      get { return brains_; }
    }

    /// <summary>Field number for the "initialization_output" field.</summary>
    public const int InitializationOutputFieldNumber = 2;
    private global::ProtoObjects.UnityOutputProto.Types.UnityInitializationOutputProto initializationOutput_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::ProtoObjects.UnityOutputProto.Types.UnityInitializationOutputProto InitializationOutput {
      get { return initializationOutput_; }
      set {
        initializationOutput_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as UnityOutputProto);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(UnityOutputProto other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (!Brains.Equals(other.Brains)) return false;
      if (!object.Equals(InitializationOutput, other.InitializationOutput)) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      hash ^= Brains.GetHashCode();
      if (initializationOutput_ != null) hash ^= InitializationOutput.GetHashCode();
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
      brains_.WriteTo(output, _map_brains_codec);
      if (initializationOutput_ != null) {
        output.WriteRawTag(18);
        output.WriteMessage(InitializationOutput);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      size += brains_.CalculateSize(_map_brains_codec);
      if (initializationOutput_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(InitializationOutput);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(UnityOutputProto other) {
      if (other == null) {
        return;
      }
      brains_.Add(other.brains_);
      if (other.initializationOutput_ != null) {
        if (initializationOutput_ == null) {
          InitializationOutput = new global::ProtoObjects.UnityOutputProto.Types.UnityInitializationOutputProto();
        }
        InitializationOutput.MergeFrom(other.InitializationOutput);
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
            brains_.AddEntriesFrom(input, _map_brains_codec);
            break;
          }
          case 18: {
            if (initializationOutput_ == null) {
              InitializationOutput = new global::ProtoObjects.UnityOutputProto.Types.UnityInitializationOutputProto();
            }
            input.ReadMessage(InitializationOutput);
            break;
          }
        }
      }
    }

    #region Nested types
    /// <summary>Container for nested types declared in the UnityOutputProto message type.</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static partial class Types {
      public sealed partial class AgentListProto : pb::IMessage<AgentListProto> {
        private static readonly pb::MessageParser<AgentListProto> _parser = new pb::MessageParser<AgentListProto>(() => new AgentListProto());
        private pb::UnknownFieldSet _unknownFields;
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public static pb::MessageParser<AgentListProto> Parser { get { return _parser; } }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public static pbr::MessageDescriptor Descriptor {
          get { return global::ProtoObjects.UnityOutputProto.Descriptor.NestedTypes[0]; }
        }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        pbr::MessageDescriptor pb::IMessage.Descriptor {
          get { return Descriptor; }
        }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public AgentListProto() {
          OnConstruction();
        }

        partial void OnConstruction();

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public AgentListProto(AgentListProto other) : this() {
          agentsList_ = other.agentsList_.Clone();
          _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
        }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public AgentListProto Clone() {
          return new AgentListProto(this);
        }

        /// <summary>Field number for the "agentsList" field.</summary>
        public const int AgentsListFieldNumber = 4;
        private static readonly pb::FieldCodec<global::ProtoObjects.UnityOutputProto.Types.AgentListProto.Types.ObservationProto> _repeated_agentsList_codec
            = pb::FieldCodec.ForMessage(34, global::ProtoObjects.UnityOutputProto.Types.AgentListProto.Types.ObservationProto.Parser);
        private readonly pbc::RepeatedField<global::ProtoObjects.UnityOutputProto.Types.AgentListProto.Types.ObservationProto> agentsList_ = new pbc::RepeatedField<global::ProtoObjects.UnityOutputProto.Types.AgentListProto.Types.ObservationProto>();
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public pbc::RepeatedField<global::ProtoObjects.UnityOutputProto.Types.AgentListProto.Types.ObservationProto> AgentsList {
          get { return agentsList_; }
        }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public override bool Equals(object other) {
          return Equals(other as AgentListProto);
        }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public bool Equals(AgentListProto other) {
          if (ReferenceEquals(other, null)) {
            return false;
          }
          if (ReferenceEquals(other, this)) {
            return true;
          }
          if(!agentsList_.Equals(other.agentsList_)) return false;
          return Equals(_unknownFields, other._unknownFields);
        }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public override int GetHashCode() {
          int hash = 1;
          hash ^= agentsList_.GetHashCode();
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
          agentsList_.WriteTo(output, _repeated_agentsList_codec);
          if (_unknownFields != null) {
            _unknownFields.WriteTo(output);
          }
        }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public int CalculateSize() {
          int size = 0;
          size += agentsList_.CalculateSize(_repeated_agentsList_codec);
          if (_unknownFields != null) {
            size += _unknownFields.CalculateSize();
          }
          return size;
        }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public void MergeFrom(AgentListProto other) {
          if (other == null) {
            return;
          }
          agentsList_.Add(other.agentsList_);
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
              case 34: {
                agentsList_.AddEntriesFrom(input, _repeated_agentsList_codec);
                break;
              }
            }
          }
        }

        #region Nested types
        /// <summary>Container for nested types declared in the AgentListProto message type.</summary>
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public static partial class Types {
          public sealed partial class ObservationProto : pb::IMessage<ObservationProto> {
            private static readonly pb::MessageParser<ObservationProto> _parser = new pb::MessageParser<ObservationProto>(() => new ObservationProto());
            private pb::UnknownFieldSet _unknownFields;
            [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
            public static pb::MessageParser<ObservationProto> Parser { get { return _parser; } }

            [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
            public static pbr::MessageDescriptor Descriptor {
              get { return global::ProtoObjects.UnityOutputProto.Types.AgentListProto.Descriptor.NestedTypes[0]; }
            }

            [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
            pbr::MessageDescriptor pb::IMessage.Descriptor {
              get { return Descriptor; }
            }

            [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
            public ObservationProto() {
              OnConstruction();
            }

            partial void OnConstruction();

            [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
            public ObservationProto(ObservationProto other) : this() {
              observationsVector_ = other.observationsVector_.Clone();
              _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
            }

            [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
            public ObservationProto Clone() {
              return new ObservationProto(this);
            }

            /// <summary>Field number for the "observationsVector" field.</summary>
            public const int ObservationsVectorFieldNumber = 3;
            private static readonly pb::FieldCodec<float> _repeated_observationsVector_codec
                = pb::FieldCodec.ForFloat(26);
            private readonly pbc::RepeatedField<float> observationsVector_ = new pbc::RepeatedField<float>();
            [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
            public pbc::RepeatedField<float> ObservationsVector {
              get { return observationsVector_; }
            }

            [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
            public override bool Equals(object other) {
              return Equals(other as ObservationProto);
            }

            [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
            public bool Equals(ObservationProto other) {
              if (ReferenceEquals(other, null)) {
                return false;
              }
              if (ReferenceEquals(other, this)) {
                return true;
              }
              if(!observationsVector_.Equals(other.observationsVector_)) return false;
              return Equals(_unknownFields, other._unknownFields);
            }

            [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
            public override int GetHashCode() {
              int hash = 1;
              hash ^= observationsVector_.GetHashCode();
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
              observationsVector_.WriteTo(output, _repeated_observationsVector_codec);
              if (_unknownFields != null) {
                _unknownFields.WriteTo(output);
              }
            }

            [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
            public int CalculateSize() {
              int size = 0;
              size += observationsVector_.CalculateSize(_repeated_observationsVector_codec);
              if (_unknownFields != null) {
                size += _unknownFields.CalculateSize();
              }
              return size;
            }

            [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
            public void MergeFrom(ObservationProto other) {
              if (other == null) {
                return;
              }
              observationsVector_.Add(other.observationsVector_);
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
                  case 26:
                  case 29: {
                    observationsVector_.AddEntriesFrom(input, _repeated_observationsVector_codec);
                    break;
                  }
                }
              }
            }

          }

        }
        #endregion

      }

      public sealed partial class UnityInitializationOutputProto : pb::IMessage<UnityInitializationOutputProto> {
        private static readonly pb::MessageParser<UnityInitializationOutputProto> _parser = new pb::MessageParser<UnityInitializationOutputProto>(() => new UnityInitializationOutputProto());
        private pb::UnknownFieldSet _unknownFields;
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public static pb::MessageParser<UnityInitializationOutputProto> Parser { get { return _parser; } }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public static pbr::MessageDescriptor Descriptor {
          get { return global::ProtoObjects.UnityOutputProto.Descriptor.NestedTypes[1]; }
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
          version_ = other.version_;
          logPath_ = other.logPath_;
          _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
        }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public UnityInitializationOutputProto Clone() {
          return new UnityInitializationOutputProto(this);
        }

        /// <summary>Field number for the "name" field.</summary>
        public const int NameFieldNumber = 5;
        private string name_ = "";
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public string Name {
          get { return name_; }
          set {
            name_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
          }
        }

        /// <summary>Field number for the "version" field.</summary>
        public const int VersionFieldNumber = 6;
        private string version_ = "";
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public string Version {
          get { return version_; }
          set {
            version_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
          }
        }

        /// <summary>Field number for the "log_path" field.</summary>
        public const int LogPathFieldNumber = 7;
        private string logPath_ = "";
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public string LogPath {
          get { return logPath_; }
          set {
            logPath_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
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
          if (Version != other.Version) return false;
          if (LogPath != other.LogPath) return false;
          return Equals(_unknownFields, other._unknownFields);
        }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public override int GetHashCode() {
          int hash = 1;
          if (Name.Length != 0) hash ^= Name.GetHashCode();
          if (Version.Length != 0) hash ^= Version.GetHashCode();
          if (LogPath.Length != 0) hash ^= LogPath.GetHashCode();
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
            output.WriteRawTag(42);
            output.WriteString(Name);
          }
          if (Version.Length != 0) {
            output.WriteRawTag(50);
            output.WriteString(Version);
          }
          if (LogPath.Length != 0) {
            output.WriteRawTag(58);
            output.WriteString(LogPath);
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
          if (Version.Length != 0) {
            size += 1 + pb::CodedOutputStream.ComputeStringSize(Version);
          }
          if (LogPath.Length != 0) {
            size += 1 + pb::CodedOutputStream.ComputeStringSize(LogPath);
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
          if (other.Version.Length != 0) {
            Version = other.Version;
          }
          if (other.LogPath.Length != 0) {
            LogPath = other.LogPath;
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
              case 42: {
                Name = input.ReadString();
                break;
              }
              case 50: {
                Version = input.ReadString();
                break;
              }
              case 58: {
                LogPath = input.ReadString();
                break;
              }
            }
          }
        }

      }

    }
    #endregion

  }

  #endregion

}

#endregion Designer generated code