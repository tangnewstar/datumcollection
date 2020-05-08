// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: event.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace DatumCollection.EventBus
{

    /// <summary>Holder for reflection information generated from event.proto</summary>
    public static partial class EventReflection
    {

        #region Descriptor
        /// <summary>File descriptor for event.proto</summary>
        public static pbr::FileDescriptor Descriptor
        {
            get { return descriptor; }
        }
        private static pbr::FileDescriptor descriptor;

        static EventReflection()
        {
            byte[] descriptorData = global::System.Convert.FromBase64String(
                string.Concat(
                  "CgtldmVudC5wcm90bxIdRGF0dW1Db2xsZWN0aW9uLkNvcmUuRXZlbnRCdXMi",
                  "NgoFRXZlbnQSDAoEVHlwZRgBIAEoCRIRCglUaW1lc3RhbXAYAiABKAMSDAoE",
                  "RGF0YRgDIAEoCWIGcHJvdG8z"));
            descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
                new pbr::FileDescriptor[] { },
                new pbr::GeneratedClrTypeInfo(null, null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::DatumCollection.EventBus.Event), global::DatumCollection.EventBus.Event.Parser, new[]{ "Type", "Timestamp", "Data" }, null, null, null, null)
                }));
        }
        #endregion

    }
    #region Messages
    public sealed partial class Event : pb::IMessage<Event>
    {
        private static readonly pb::MessageParser<Event> _parser = new pb::MessageParser<Event>(() => new Event());
        private pb::UnknownFieldSet _unknownFields;
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public static pb::MessageParser<Event> Parser { get { return _parser; } }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public static pbr::MessageDescriptor Descriptor
        {
            get { return global::DatumCollection.EventBus.EventReflection.Descriptor.MessageTypes[0]; }
        }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        pbr::MessageDescriptor pb::IMessage.Descriptor
        {
            get { return Descriptor; }
        }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public Event()
        {
            OnConstruction();
        }

        partial void OnConstruction();

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public Event(Event other) : this()
        {
            type_ = other.type_;
            timestamp_ = other.timestamp_;
            data_ = other.data_;
            _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
        }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public Event Clone()
        {
            return new Event(this);
        }

        /// <summary>Field number for the "Type" field.</summary>
        public const int TypeFieldNumber = 1;
        private string type_ = "";
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public string Type
        {
            get { return type_; }
            set
            {
                type_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
            }
        }

        /// <summary>Field number for the "Timestamp" field.</summary>
        public const int TimestampFieldNumber = 2;
        private long timestamp_;
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public long Timestamp
        {
            get { return timestamp_; }
            set
            {
                timestamp_ = value;
            }
        }

        /// <summary>Field number for the "Data" field.</summary>
        public const int DataFieldNumber = 3;
        private string data_ = "";
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public string Data
        {
            get { return data_; }
            set
            {
                data_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
            }
        }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public override bool Equals(object other)
        {
            return Equals(other as Event);
        }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public bool Equals(Event other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }
            if (ReferenceEquals(other, this))
            {
                return true;
            }
            if (Type != other.Type) return false;
            if (Timestamp != other.Timestamp) return false;
            if (Data != other.Data) return false;
            return Equals(_unknownFields, other._unknownFields);
        }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public override int GetHashCode()
        {
            int hash = 1;
            if (Type.Length != 0) hash ^= Type.GetHashCode();
            if (Timestamp != 0L) hash ^= Timestamp.GetHashCode();
            if (Data.Length != 0) hash ^= Data.GetHashCode();
            if (_unknownFields != null)
            {
                hash ^= _unknownFields.GetHashCode();
            }
            return hash;
        }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public override string ToString()
        {
            return pb::JsonFormatter.ToDiagnosticString(this);
        }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public void WriteTo(pb::CodedOutputStream output)
        {
            if (Type.Length != 0)
            {
                output.WriteRawTag(10);
                output.WriteString(Type);
            }
            if (Timestamp != 0L)
            {
                output.WriteRawTag(16);
                output.WriteInt64(Timestamp);
            }
            if (Data.Length != 0)
            {
                output.WriteRawTag(26);
                output.WriteString(Data);
            }
            if (_unknownFields != null)
            {
                _unknownFields.WriteTo(output);
            }
        }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public int CalculateSize()
        {
            int size = 0;
            if (Type.Length != 0)
            {
                size += 1 + pb::CodedOutputStream.ComputeStringSize(Type);
            }
            if (Timestamp != 0L)
            {
                size += 1 + pb::CodedOutputStream.ComputeInt64Size(Timestamp);
            }
            if (Data.Length != 0)
            {
                size += 1 + pb::CodedOutputStream.ComputeStringSize(Data);
            }
            if (_unknownFields != null)
            {
                size += _unknownFields.CalculateSize();
            }
            return size;
        }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public void MergeFrom(Event other)
        {
            if (other == null)
            {
                return;
            }
            if (other.Type.Length != 0)
            {
                Type = other.Type;
            }
            if (other.Timestamp != 0L)
            {
                Timestamp = other.Timestamp;
            }
            if (other.Data.Length != 0)
            {
                Data = other.Data;
            }
            _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
        }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public void MergeFrom(pb::CodedInputStream input)
        {
            uint tag;
            while ((tag = input.ReadTag()) != 0)
            {
                switch (tag)
                {
                    default:
                        _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
                        break;
                    case 10:
                        {
                            Type = input.ReadString();
                            break;
                        }
                    case 16:
                        {
                            Timestamp = input.ReadInt64();
                            break;
                        }
                    case 26:
                        {
                            Data = input.ReadString();
                            break;
                        }
                }
            }
        }

    }

    #endregion

}

#endregion Designer generated code
