// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: StitchUpdated.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace SM.Service {

  /// <summary>Holder for reflection information generated from StitchUpdated.proto</summary>
  public static partial class StitchUpdatedReflection {

    #region Descriptor
    /// <summary>File descriptor for StitchUpdated.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static StitchUpdatedReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "ChNTdGl0Y2hVcGRhdGVkLnByb3RvEgdwYXR0ZXJuGhZTdGl0Y2hBY3Rpb25E",
            "YXRhLnByb3RvIl4KDVN0aXRjaFVwZGF0ZWQSEQoJc291cmNlX2lkGAEgASgJ",
            "EioKBnN0aXRjaBgCIAEoCzIaLnBhdHRlcm4uU3RpdGNoQ29vcmRpbmF0ZXMS",
            "DgoGbWFya2VkGAMgASgIQg2qAgpTTS5TZXJ2aWNlUABiBnByb3RvMw=="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { global::SM.Service.StitchActionDataReflection.Descriptor, },
          new pbr::GeneratedClrTypeInfo(null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::SM.Service.StitchUpdated), global::SM.Service.StitchUpdated.Parser, new[]{ "SourceId", "Stitch", "Marked" }, null, null, null)
          }));
    }
    #endregion

  }
  #region Messages
  public sealed partial class StitchUpdated : pb::IMessage<StitchUpdated> {
    private static readonly pb::MessageParser<StitchUpdated> _parser = new pb::MessageParser<StitchUpdated>(() => new StitchUpdated());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<StitchUpdated> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::SM.Service.StitchUpdatedReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public StitchUpdated() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public StitchUpdated(StitchUpdated other) : this() {
      sourceId_ = other.sourceId_;
      Stitch = other.stitch_ != null ? other.Stitch.Clone() : null;
      marked_ = other.marked_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public StitchUpdated Clone() {
      return new StitchUpdated(this);
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

    /// <summary>Field number for the "stitch" field.</summary>
    public const int StitchFieldNumber = 2;
    private global::SM.Service.StitchCoordinates stitch_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::SM.Service.StitchCoordinates Stitch {
      get { return stitch_; }
      set {
        stitch_ = value;
      }
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
      return Equals(other as StitchUpdated);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(StitchUpdated other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (SourceId != other.SourceId) return false;
      if (!object.Equals(Stitch, other.Stitch)) return false;
      if (Marked != other.Marked) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (SourceId.Length != 0) hash ^= SourceId.GetHashCode();
      if (stitch_ != null) hash ^= Stitch.GetHashCode();
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
      if (stitch_ != null) {
        output.WriteRawTag(18);
        output.WriteMessage(Stitch);
      }
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
      if (stitch_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(Stitch);
      }
      if (Marked != false) {
        size += 1 + 1;
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(StitchUpdated other) {
      if (other == null) {
        return;
      }
      if (other.SourceId.Length != 0) {
        SourceId = other.SourceId;
      }
      if (other.stitch_ != null) {
        if (stitch_ == null) {
          stitch_ = new global::SM.Service.StitchCoordinates();
        }
        Stitch.MergeFrom(other.Stitch);
      }
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
            if (stitch_ == null) {
              stitch_ = new global::SM.Service.StitchCoordinates();
            }
            input.ReadMessage(stitch_);
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