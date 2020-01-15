﻿using System;

namespace maxbl4.Race.Logic
{
    public struct Id<T> : IEquatable<Id<T>>
    {
        #region Equality

        public bool Equals(Id<T> other)
        {
            return Value.Equals(other.Value);
        }

        public override bool Equals(object obj)
        {
            return obj is Id<T> other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public static bool operator ==(Id<T> left, Id<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Id<T> left, Id<T> right)
        {
            return !left.Equals(right);
        }

        #endregion

        public static Id<T> NewId()
        {
            return new Id<T>(Guid.NewGuid());
        }
            
        public Id(Guid value)
        {
            Value = value;
        }
            
        public Guid Value { get; }
            
        public static implicit operator Guid(Id<T> id)
        {
            return id.Value;
        }
            
        public static implicit operator Id<T>(Guid id)
        {
            return new Id<T>(id);
        }
    }
}