// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: mlagents/envs/communicator_objects/command.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace MLAgents.CommunicatorObjects {

  /// <summary>Holder for reflection information generated from mlagents/envs/communicator_objects/command.proto</summary>
  public static partial class CommandReflection {

    #region Descriptor
    /// <summary>File descriptor for mlagents/envs/communicator_objects/command.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static CommandReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "CjBtbGFnZW50cy9lbnZzL2NvbW11bmljYXRvcl9vYmplY3RzL2NvbW1hbmQu",
            "cHJvdG8SFGNvbW11bmljYXRvcl9vYmplY3RzKi0KDENvbW1hbmRQcm90bxII",
            "CgRTVEVQEAASCQoFUkVTRVQQARIICgRRVUlUEAJCH6oCHE1MQWdlbnRzLkNv",
            "bW11bmljYXRvck9iamVjdHNiBnByb3RvMw=="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { },
          new pbr::GeneratedClrTypeInfo(new[] {typeof(global::MLAgents.CommunicatorObjects.CommandProto), }, null));
    }
    #endregion

  }
  #region Enums
  public enum CommandProto {
    [pbr::OriginalName("STEP")] Step = 0,
    [pbr::OriginalName("RESET")] Reset = 1,
    [pbr::OriginalName("QUIT")] Quit = 2,
    [pbr::OriginalName("EPISODE_COMPLETED")] EpisodeCompleted = 3,
  }

  #endregion

}

#endregion Designer generated code
