// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: GetThumbnail.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace Service {

  /// <summary>Holder for reflection information generated from GetThumbnail.proto</summary>
  public static partial class GetThumbnailReflection {

    #region Descriptor
    /// <summary>File descriptor for GetThumbnail.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static GetThumbnailReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "ChJHZXRUaHVtYm5haWwucHJvdG8SB3BhdHRlcm4aDVBhdHRlcm4ucHJvdG8i",
            "XAoMR2V0VGh1bWJuYWlsEgoKAmlkGAEgASgJEg0KBXdpZHRoGAIgASgFEg4K",
            "BmhlaWdodBgDIAEoBRIhCgdwYXR0ZXJuGAQgASgLMhAucGF0dGVybi5QYXR0",
            "ZXJuQgqqAgdTZXJ2aWNlUABiBnByb3RvMw=="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { global::Service.PatternReflection.Descriptor, },
          new pbr::GeneratedClrTypeInfo(null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::Service.GetThumbnail), global::Service.GetThumbnail.Parser, new[]{ "Id", "Width", "Height", "Pattern" }, null, null, null)
          }));
    }
    #endregion

  }
  #region Messages
  public sealed partial class GetThumbnail : pb::IMessage<GetThumbnail> {
    private static readonly pb::MessageParser<GetThumbnail> _parser = new pb::MessageParser<GetThumbnail>(() => new GetThumbnail());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<GetThumbnail> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Service.GetThumbnailReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public GetThumbnail() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public GetThumbnail(GetThumbnail other) : this() {
      id_ = other.id_;
      width_ = other.width_;
      height_ = other.height_;
      pattern_ = other.pattern_ != null ? other.pattern_.Clone() : null;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public GetThumbnail Clone() {
      return new GetThumbnail(this);
    }

    /// <summary>Field number for the "id" field.</summary>
    public const int IdFieldNumber = 1;
    private string id_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string Id {
      get { return id_; }
      set {
        id_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "width" field.</summary>
    public const int WidthFieldNumber = 2;
    private int width_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int Width {
      get { return width_; }
      set {
        width_ = value;
      }
    }

    /// <summary>Field number for the "height" field.</summary>
    public const int HeightFieldNumber = 3;
    private int height_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int Height {
      get { return height_; }
      set {
        height_ = value;
      }
    }

    /// <summary>Field number for the "pattern" field.</summary>
    public const int PatternFieldNumber = 4;
    private global::Service.Pattern pattern_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Service.Pattern Pattern {
      get { return pattern_; }
      set {
        pattern_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as GetThumbnail);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(GetThumbnail other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (Id != other.Id) return false;
      if (Width != other.Width) return false;
      if (Height != other.Height) return false;
      if (!object.Equals(Pattern, other.Pattern)) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (Id.Length != 0) hash ^= Id.GetHashCode();
      if (Width != 0) hash ^= Width.GetHashCode();
      if (Height != 0) hash ^= Height.GetHashCode();
      if (pattern_ != null) hash ^= Pattern.GetHashCode();
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
      if (Id.Length != 0) {
        output.WriteRawTag(10);
        output.WriteString(Id);
      }
      if (Width != 0) {
        output.WriteRawTag(16);
        output.WriteInt32(Width);
      }
      if (Height != 0) {
        output.WriteRawTag(24);
        output.WriteInt32(Height);
      }
      if (pattern_ != null) {
        output.WriteRawTag(34);
        output.WriteMessage(Pattern);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (Id.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(Id);
      }
      if (Width != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(Width);
      }
      if (Height != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(Height);
      }
      if (pattern_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(Pattern);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(GetThumbnail other) {
      if (other == null) {
        return;
      }
      if (other.Id.Length != 0) {
        Id = other.Id;
      }
      if (other.Width != 0) {
        Width = other.Width;
      }
      if (other.Height != 0) {
        Height = other.Height;
      }
      if (other.pattern_ != null) {
        if (pattern_ == null) {
          pattern_ = new global::Service.Pattern();
        }
        Pattern.MergeFrom(other.Pattern);
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
            Id = input.ReadString();
            break;
          }
          case 16: {
            Width = input.ReadInt32();
            break;
          }
          case 24: {
            Height = input.ReadInt32();
            break;
          }
          case 34: {
            if (pattern_ == null) {
              pattern_ = new global::Service.Pattern();
            }
            input.ReadMessage(pattern_);
            break;
          }
        }
      }
    }

  }

  #endregion

}

#endregion Designer generated code