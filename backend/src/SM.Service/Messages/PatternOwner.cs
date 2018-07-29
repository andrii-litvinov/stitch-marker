// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: PatternOwner.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace SM.Service.Messages {

  /// <summary>Holder for reflection information generated from PatternOwner.proto</summary>
  public static partial class PatternOwnerReflection {

    #region Descriptor
    /// <summary>File descriptor for PatternOwner.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static PatternOwnerReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "ChJQYXR0ZXJuT3duZXIucHJvdG8SB3BhdHRlcm4iNAoMUGF0dGVybk93bmVy",
            "EhIKCnJlcXVlc3RfaWQYASABKAkSEAoIb3duZXJfaWQYAiABKAlCFqoCE1NN",
            "LlNlcnZpY2UuTWVzc2FnZXNiBnByb3RvMw=="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { },
          new pbr::GeneratedClrTypeInfo(null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::SM.Service.Messages.PatternOwner), global::SM.Service.Messages.PatternOwner.Parser, new[]{ "RequestId", "OwnerId" }, null, null, null)
          }));
    }
    #endregion

  }
  #region Messages
  public sealed partial class PatternOwner : pb::IMessage<PatternOwner> {
    private static readonly pb::MessageParser<PatternOwner> _parser = new pb::MessageParser<PatternOwner>(() => new PatternOwner());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<PatternOwner> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::SM.Service.Messages.PatternOwnerReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public PatternOwner() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public PatternOwner(PatternOwner other) : this() {
      requestId_ = other.requestId_;
      ownerId_ = other.ownerId_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public PatternOwner Clone() {
      return new PatternOwner(this);
    }

    /// <summary>Field number for the "request_id" field.</summary>
    public const int RequestIdFieldNumber = 1;
    private string requestId_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string RequestId {
      get { return requestId_; }
      set {
        requestId_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "owner_id" field.</summary>
    public const int OwnerIdFieldNumber = 2;
    private string ownerId_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string OwnerId {
      get { return ownerId_; }
      set {
        ownerId_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as PatternOwner);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(PatternOwner other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (RequestId != other.RequestId) return false;
      if (OwnerId != other.OwnerId) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (RequestId.Length != 0) hash ^= RequestId.GetHashCode();
      if (OwnerId.Length != 0) hash ^= OwnerId.GetHashCode();
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
      if (RequestId.Length != 0) {
        output.WriteRawTag(10);
        output.WriteString(RequestId);
      }
      if (OwnerId.Length != 0) {
        output.WriteRawTag(18);
        output.WriteString(OwnerId);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (RequestId.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(RequestId);
      }
      if (OwnerId.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(OwnerId);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(PatternOwner other) {
      if (other == null) {
        return;
      }
      if (other.RequestId.Length != 0) {
        RequestId = other.RequestId;
      }
      if (other.OwnerId.Length != 0) {
        OwnerId = other.OwnerId;
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
            RequestId = input.ReadString();
            break;
          }
          case 18: {
            OwnerId = input.ReadString();
            break;
          }
        }
      }
    }

  }

  #endregion

}

#endregion Designer generated code
