// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: BackstitchCoordinates.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace SM.Service {

  /// <summary>Holder for reflection information generated from BackstitchCoordinates.proto</summary>
  public static partial class BackstitchCoordinatesReflection {

    #region Descriptor
    /// <summary>File descriptor for BackstitchCoordinates.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static BackstitchCoordinatesReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "ChtCYWNrc3RpdGNoQ29vcmRpbmF0ZXMucHJvdG8SB3BhdHRlcm4iRwoVQmFj",
            "a3N0aXRjaENvb3JkaW5hdGVzEgoKAlgxGAEgASgNEgoKAlkxGAIgASgNEgoK",
            "AlgyGAMgASgNEgoKAlkyGAQgASgNQg2qAgpTTS5TZXJ2aWNlYgZwcm90bzM="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { },
          new pbr::GeneratedClrTypeInfo(null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::SM.Service.BackstitchCoordinates), global::SM.Service.BackstitchCoordinates.Parser, new[]{ "X1", "Y1", "X2", "Y2" }, null, null, null)
          }));
    }
    #endregion

  }
  #region Messages
  public sealed partial class BackstitchCoordinates : pb::IMessage<BackstitchCoordinates> {
    private static readonly pb::MessageParser<BackstitchCoordinates> _parser = new pb::MessageParser<BackstitchCoordinates>(() => new BackstitchCoordinates());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<BackstitchCoordinates> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::SM.Service.BackstitchCoordinatesReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public BackstitchCoordinates() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public BackstitchCoordinates(BackstitchCoordinates other) : this() {
      x1_ = other.x1_;
      y1_ = other.y1_;
      x2_ = other.x2_;
      y2_ = other.y2_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public BackstitchCoordinates Clone() {
      return new BackstitchCoordinates(this);
    }

    /// <summary>Field number for the "X1" field.</summary>
    public const int X1FieldNumber = 1;
    private uint x1_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint X1 {
      get { return x1_; }
      set {
        x1_ = value;
      }
    }

    /// <summary>Field number for the "Y1" field.</summary>
    public const int Y1FieldNumber = 2;
    private uint y1_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint Y1 {
      get { return y1_; }
      set {
        y1_ = value;
      }
    }

    /// <summary>Field number for the "X2" field.</summary>
    public const int X2FieldNumber = 3;
    private uint x2_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint X2 {
      get { return x2_; }
      set {
        x2_ = value;
      }
    }

    /// <summary>Field number for the "Y2" field.</summary>
    public const int Y2FieldNumber = 4;
    private uint y2_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint Y2 {
      get { return y2_; }
      set {
        y2_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as BackstitchCoordinates);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(BackstitchCoordinates other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (X1 != other.X1) return false;
      if (Y1 != other.Y1) return false;
      if (X2 != other.X2) return false;
      if (Y2 != other.Y2) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (X1 != 0) hash ^= X1.GetHashCode();
      if (Y1 != 0) hash ^= Y1.GetHashCode();
      if (X2 != 0) hash ^= X2.GetHashCode();
      if (Y2 != 0) hash ^= Y2.GetHashCode();
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
      if (X1 != 0) {
        output.WriteRawTag(8);
        output.WriteUInt32(X1);
      }
      if (Y1 != 0) {
        output.WriteRawTag(16);
        output.WriteUInt32(Y1);
      }
      if (X2 != 0) {
        output.WriteRawTag(24);
        output.WriteUInt32(X2);
      }
      if (Y2 != 0) {
        output.WriteRawTag(32);
        output.WriteUInt32(Y2);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (X1 != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(X1);
      }
      if (Y1 != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(Y1);
      }
      if (X2 != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(X2);
      }
      if (Y2 != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(Y2);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(BackstitchCoordinates other) {
      if (other == null) {
        return;
      }
      if (other.X1 != 0) {
        X1 = other.X1;
      }
      if (other.Y1 != 0) {
        Y1 = other.Y1;
      }
      if (other.X2 != 0) {
        X2 = other.X2;
      }
      if (other.Y2 != 0) {
        Y2 = other.Y2;
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
          case 8: {
            X1 = input.ReadUInt32();
            break;
          }
          case 16: {
            Y1 = input.ReadUInt32();
            break;
          }
          case 24: {
            X2 = input.ReadUInt32();
            break;
          }
          case 32: {
            Y2 = input.ReadUInt32();
            break;
          }
        }
      }
    }

  }

  #endregion

}

#endregion Designer generated code
