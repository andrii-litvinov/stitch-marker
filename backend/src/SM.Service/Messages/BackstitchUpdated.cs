// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: BackstitchUpdated.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace SM.Service {

  /// <summary>Holder for reflection information generated from BackstitchUpdated.proto</summary>
  public static partial class BackstitchUpdatedReflection {

    #region Descriptor
    /// <summary>File descriptor for BackstitchUpdated.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static BackstitchUpdatedReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "ChdCYWNrc3RpdGNoVXBkYXRlZC5wcm90bxIHcGF0dGVybhoNQ29tbWFuZC5w",
            "cm90byJsChFCYWNrc3RpdGNoVXBkYXRlZBIRCglzb3VyY2VfaWQYASABKAkS",
            "NAoMYmFja3N0aXRjaGVzGAIgAygLMh4ucGF0dGVybi5CYWNrc3RpdGNoQ29v",
            "cmRpbmF0ZXMSDgoGbWFya2VkGAMgASgIQg2qAgpTTS5TZXJ2aWNlUABiBnBy",
            "b3RvMw=="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { global::SM.Service.Command.CommandReflection.Descriptor, },
          new pbr::GeneratedClrTypeInfo(null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::SM.Service.BackstitchUpdated), global::SM.Service.BackstitchUpdated.Parser, new[]{ "SourceId", "Backstitches", "Marked" }, null, null, null)
          }));
    }
    #endregion

  }
  #region Messages
  public sealed partial class BackstitchUpdated : pb::IMessage<BackstitchUpdated> {
    private static readonly pb::MessageParser<BackstitchUpdated> _parser = new pb::MessageParser<BackstitchUpdated>(() => new BackstitchUpdated());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<BackstitchUpdated> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::SM.Service.BackstitchUpdatedReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public BackstitchUpdated() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public BackstitchUpdated(BackstitchUpdated other) : this() {
      sourceId_ = other.sourceId_;
      backstitches_ = other.backstitches_.Clone();
      marked_ = other.marked_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public BackstitchUpdated Clone() {
      return new BackstitchUpdated(this);
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
    private static readonly pb::FieldCodec<global::SM.Service.Command.BackstitchCoordinates> _repeated_backstitches_codec
        = pb::FieldCodec.ForMessage(18, global::SM.Service.Command.BackstitchCoordinates.Parser);
    private readonly pbc::RepeatedField<global::SM.Service.Command.BackstitchCoordinates> backstitches_ = new pbc::RepeatedField<global::SM.Service.Command.BackstitchCoordinates>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::RepeatedField<global::SM.Service.Command.BackstitchCoordinates> Backstitches {
      get { return backstitches_; }
    }

    /// <summary>Field number for the "marked" field.</summary>
    public const int MarkedFieldNumber = 3;
    private bool marked_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Marked {
      get { return marked_; }
      set {
        marked_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as BackstitchUpdated);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(BackstitchUpdated other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (SourceId != other.SourceId) return false;
      if(!backstitches_.Equals(other.backstitches_)) return false;
      if (Marked != other.Marked) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (SourceId.Length != 0) hash ^= SourceId.GetHashCode();
      hash ^= backstitches_.GetHashCode();
      if (Marked != false) hash ^= Marked.GetHashCode();
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
      if (Marked != false) {
        output.WriteRawTag(24);
        output.WriteBool(Marked);
      }
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
      if (Marked != false) {
        size += 1 + 1;
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(BackstitchUpdated other) {
      if (other == null) {
        return;
      }
      if (other.SourceId.Length != 0) {
        SourceId = other.SourceId;
      }
      backstitches_.Add(other.backstitches_);
      if (other.Marked != false) {
        Marked = other.Marked;
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
            SourceId = input.ReadString();
            break;
          }
          case 18: {
            backstitches_.AddEntriesFrom(input, _repeated_backstitches_codec);
            break;
          }
          case 24: {
            Marked = input.ReadBool();
            break;
          }
        }
      }
    }

  }

  #endregion

}

#endregion Designer generated code
