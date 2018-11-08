// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: BackstitchesMarked.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace Service {

  /// <summary>Holder for reflection information generated from BackstitchesMarked.proto</summary>
  public static partial class BackstitchesMarkedReflection {

    #region Descriptor
    /// <summary>File descriptor for BackstitchesMarked.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static BackstitchesMarkedReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "ChhCYWNrc3RpdGNoZXNNYXJrZWQucHJvdG8SB3BhdHRlcm4aEkJhY2tzdGl0",
            "Y2hJZC5wcm90byJUChJCYWNrc3RpdGNoZXNNYXJrZWQSEQoJc291cmNlX2lk",
            "GAEgASgJEisKDGJhY2tzdGl0Y2hlcxgCIAMoCzIVLnBhdHRlcm4uQmFja3N0",
            "aXRjaElkQgqqAgdTZXJ2aWNlUABiBnByb3RvMw=="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { global::Service.BackstitchIdReflection.Descriptor, },
          new pbr::GeneratedClrTypeInfo(null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::Service.BackstitchesMarked), global::Service.BackstitchesMarked.Parser, new[]{ "SourceId", "Backstitches" }, null, null, null)
          }));
    }
    #endregion

  }
  #region Messages
  public sealed partial class BackstitchesMarked : pb::IMessage<BackstitchesMarked> {
    private static readonly pb::MessageParser<BackstitchesMarked> _parser = new pb::MessageParser<BackstitchesMarked>(() => new BackstitchesMarked());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<BackstitchesMarked> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Service.BackstitchesMarkedReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public BackstitchesMarked() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public BackstitchesMarked(BackstitchesMarked other) : this() {
      sourceId_ = other.sourceId_;
      backstitches_ = other.backstitches_.Clone();
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public BackstitchesMarked Clone() {
      return new BackstitchesMarked(this);
    }

    /// <summary>Field number for the "source_id" field.</summary>
    public const int SourceIdFieldNumber = 1;
    private string sourceId_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string SourceId {
      get { return sourceId_; }
      set {
        sourceId_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "backstitches" field.</summary>
    public const int BackstitchesFieldNumber = 2;
    private static readonly pb::FieldCodec<global::Service.BackstitchId> _repeated_backstitches_codec
        = pb::FieldCodec.ForMessage(18, global::Service.BackstitchId.Parser);
    private readonly pbc::RepeatedField<global::Service.BackstitchId> backstitches_ = new pbc::RepeatedField<global::Service.BackstitchId>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::RepeatedField<global::Service.BackstitchId> Backstitches {
      get { return backstitches_; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as BackstitchesMarked);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(BackstitchesMarked other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (SourceId != other.SourceId) return false;
      if(!backstitches_.Equals(other.backstitches_)) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (SourceId.Length != 0) hash ^= SourceId.GetHashCode();
      hash ^= backstitches_.GetHashCode();
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
      if (SourceId.Length != 0) {
        output.WriteRawTag(10);
        output.WriteString(SourceId);
      }
      backstitches_.WriteTo(output, _repeated_backstitches_codec);
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (SourceId.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(SourceId);
      }
      size += backstitches_.CalculateSize(_repeated_backstitches_codec);
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(BackstitchesMarked other) {
      if (other == null) {
        return;
      }
      if (other.SourceId.Length != 0) {
        SourceId = other.SourceId;
      }
      backstitches_.Add(other.backstitches_);
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
            SourceId = input.ReadString();
            break;
          }
          case 18: {
            backstitches_.AddEntriesFrom(input, _repeated_backstitches_codec);
            break;
          }
        }
      }
    }

  }

  #endregion

}

#endregion Designer generated code
