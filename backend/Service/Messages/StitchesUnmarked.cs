// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: StitchesUnmarked.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace Service {

  /// <summary>Holder for reflection information generated from StitchesUnmarked.proto</summary>
  public static partial class StitchesUnmarkedReflection {

    #region Descriptor
    /// <summary>File descriptor for StitchesUnmarked.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static StitchesUnmarkedReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "ChZTdGl0Y2hlc1VubWFya2VkLnByb3RvEgdwYXR0ZXJuGg5TdGl0Y2hJZC5w",
            "cm90byJKChBTdGl0Y2hlc1VubWFya2VkEhEKCXNvdXJjZV9pZBgBIAEoCRIj",
            "CghzdGl0Y2hlcxgCIAMoCzIRLnBhdHRlcm4uU3RpdGNoSWRCCqoCB1NlcnZp",
            "Y2VQAGIGcHJvdG8z"));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { global::Service.StitchIdReflection.Descriptor, },
          new pbr::GeneratedClrTypeInfo(null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::Service.StitchesUnmarked), global::Service.StitchesUnmarked.Parser, new[]{ "SourceId", "Stitches" }, null, null, null)
          }));
    }
    #endregion

  }
  #region Messages
  public sealed partial class StitchesUnmarked : pb::IMessage<StitchesUnmarked> {
    private static readonly pb::MessageParser<StitchesUnmarked> _parser = new pb::MessageParser<StitchesUnmarked>(() => new StitchesUnmarked());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<StitchesUnmarked> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Service.StitchesUnmarkedReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public StitchesUnmarked() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public StitchesUnmarked(StitchesUnmarked other) : this() {
      sourceId_ = other.sourceId_;
      stitches_ = other.stitches_.Clone();
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public StitchesUnmarked Clone() {
      return new StitchesUnmarked(this);
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

    /// <summary>Field number for the "stitches" field.</summary>
    public const int StitchesFieldNumber = 2;
    private static readonly pb::FieldCodec<global::Service.StitchId> _repeated_stitches_codec
        = pb::FieldCodec.ForMessage(18, global::Service.StitchId.Parser);
    private readonly pbc::RepeatedField<global::Service.StitchId> stitches_ = new pbc::RepeatedField<global::Service.StitchId>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::RepeatedField<global::Service.StitchId> Stitches {
      get { return stitches_; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as StitchesUnmarked);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(StitchesUnmarked other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (SourceId != other.SourceId) return false;
      if(!stitches_.Equals(other.stitches_)) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (SourceId.Length != 0) hash ^= SourceId.GetHashCode();
      hash ^= stitches_.GetHashCode();
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
      stitches_.WriteTo(output, _repeated_stitches_codec);
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
      size += stitches_.CalculateSize(_repeated_stitches_codec);
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(StitchesUnmarked other) {
      if (other == null) {
        return;
      }
      if (other.SourceId.Length != 0) {
        SourceId = other.SourceId;
      }
      stitches_.Add(other.stitches_);
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
            stitches_.AddEntriesFrom(input, _repeated_stitches_codec);
            break;
          }
        }
      }
    }

  }

  #endregion

}

#endregion Designer generated code
