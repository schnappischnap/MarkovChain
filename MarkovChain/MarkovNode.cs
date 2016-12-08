using System;
using System.Collections.Generic;
using System.Linq;

namespace Schnappischnap.Markov
{
    public class MarkovNode<T> : IEquatable<MarkovNode<T>>
    {
        private readonly T[] items;

        public MarkovNode(IEnumerable<T> items)
        {
            this.items = items.ToArray();
        }

        public bool Equals(MarkovNode<T> other)
        {
            if (other == null)
                return false;

            if (items.Length != other.items.Length)
                return false;

            for (int i = 0; i < items.Length; i++)
            {
                if (!items[i].Equals(other.items[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public override int GetHashCode()
        {
            int hash = 19;
            foreach (T item in items)
            {
                hash = hash * 31 + item.GetHashCode();
            }
            return hash;
        }
    }
}
