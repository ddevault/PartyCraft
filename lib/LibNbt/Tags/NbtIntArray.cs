using System;
using System.IO;
using System.Text;
using LibNbt.Queries;

namespace LibNbt.Tags
{
    public class NbtIntArray : NbtTag, INbtTagValue<int[]>
    {
        public int[] Value { get; set; }

        public int this[int index]
        {
            get { return Value[index]; }
            set { Value[index] = value; }
        }

        public NbtIntArray() : this("") { }
        public NbtIntArray(string tagName) : this(tagName, new int[] { }) { }
        public NbtIntArray(int[] value) : this("", value) { }
        public NbtIntArray(string tagName, int[] value)
        {
            Name = tagName;
            Value = new int[value.Length];
            Buffer.BlockCopy(value, 0, Value, 0, value.Length);
        }

        internal override void ReadTag(Stream readStream) { ReadTag(readStream, true); }
        internal override void ReadTag(Stream readStream, bool readName)
        {
            // First read the name of this tag
            Name = "";
            if (readName)
            {
                var name = new NbtString();
                name.ReadTag(readStream, false);

                Name = name.Value;
            }

            var length = new NbtInt();
            length.ReadTag(readStream, false);

            var buffer = new int[length.Value];
            int totalRead = 0;
            NbtInt tempInt = new NbtInt();
            while (totalRead < length.Value)
            {
                tempInt.ReadTag(readStream, false);
                buffer[totalRead] = tempInt.Value;
                totalRead++;
            }
            Value = buffer;
        }

        internal override void WriteTag(Stream writeStream) { WriteTag(writeStream, true); }
        internal override void WriteTag(Stream writeStream, bool writeName)
        {
            writeStream.WriteByte((byte)NbtTagType.TAG_Int_Array);
            if (writeName)
            {
                var name = new NbtString("", Name);
                name.WriteData(writeStream);
            }

            WriteData(writeStream);
        }

        internal override void WriteData(Stream writeStream)
        {
            var length = new NbtInt("", Value.Length);
            length.WriteData(writeStream);

            NbtInt tempInt = new NbtInt();
            for (int i = 0; i < Value.Length; i++)
            {
                tempInt.Value = Value[i];
                tempInt.WriteData(writeStream);
            }
        }

        internal override NbtTagType GetTagType()
        {
            return NbtTagType.TAG_Int_Array;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("TAG_Int_Array");
            if (Name.Length > 0)
            {
                sb.AppendFormat("(\"{0}\")", Name);
            }
            sb.AppendFormat(": [{0} bytes]", Value.Length);
            return sb.ToString();
        }
    }
}
