using System;

namespace StrftimeParser
{
    public class WrappedInt : IEquatable<int>, IEquatable<WrappedInt>
    {
        private int _value;

        private int Value
        {
            get => _value;
            set
            {
                _value = value;
                IsDefault = false;
            }
        }

        public WrappedInt(int value)
        {
            _value = value;
        }

        private WrappedInt(int value, bool isDefault)
        {
            _value = value;
            IsDefault = isDefault;
        }

        public bool IsDefault { get; private set; } = true;

        public static implicit operator WrappedInt(int d) => new(d, false);
        public static implicit operator int(WrappedInt d) => d.Value;

        public static int operator %(WrappedInt v, int d)
        {
            return v.Value % d;
        }

        public static int operator /(WrappedInt v, int d)
        {
            return v.Value / d;
        }

        public bool Equals(WrappedInt other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return _value == other._value;
        }

        public bool Equals(int other)
        {
            return _value.Equals(other);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((WrappedInt)obj);
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }
    }
}