using System;
using System.Runtime.InteropServices;

namespace Physalia
{
    [StructLayout(LayoutKind.Explicit)]
    public struct FValue : IEquatable<FValue>
    {
        public enum Type : byte
        {
            Bool = 0,
            Int,
            UInt,
            Float,
        }

        [FieldOffset(0)]
        private readonly Type _valueType;
        [FieldOffset(1)]
        public bool Bool;
        [FieldOffset(1)]
        public int Int;
        [FieldOffset(1)]
        public uint UInt;
        [FieldOffset(1)]
        public float Float;

        public readonly Type ValueType => _valueType;

        public FValue(bool value) : this()
        {
            _valueType = Type.Bool;
            Bool = value;
        }

        public FValue(int value) : this()
        {
            _valueType = Type.Int;
            Int = value;
        }

        public FValue(uint value) : this()
        {
            _valueType = Type.UInt;
            UInt = value;
        }

        public FValue(float value) : this()
        {
            _valueType = Type.Float;
            Float = value;
        }

        public override readonly bool Equals(object obj)
        {
            return obj is FValue value && Equals(value);
        }

        public readonly bool Equals(FValue other)
        {
            return _valueType == other._valueType &&
                Int == other.Int;
        }

        public override readonly int GetHashCode()
        {
            return HashCode.Combine(_valueType, Int);
        }

        public static bool operator ==(FValue left, FValue right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(FValue left, FValue right)
        {
            return !(left == right);
        }

        public static implicit operator bool(FValue value) => value.Bool;
        public static implicit operator FValue(bool value) => new(value);

        public static implicit operator int(FValue value) => value.Int;
        public static implicit operator FValue(int value) => new(value);

        public static implicit operator uint(FValue value) => value.UInt;
        public static implicit operator FValue(uint value) => new(value);

        public static implicit operator float(FValue value) => value.Float;
        public static implicit operator FValue(float value) => new(value);
    }
}
