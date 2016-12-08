using System;
using System.Collections.Generic;
using System.Linq;

namespace Schnappischnap.Markov
{
    public class MarkovChain<T> where T : IEquatable<T>
    {
        private readonly int order;
        private Dictionary<MarkovNode<T>, Dictionary<T, int>> chain;
        private Dictionary<MarkovNode<T>, int> terminalWeights;

        public MarkovChain(int order)
        {
            this.order = order;
            this.chain = new Dictionary<MarkovNode<T>, Dictionary<T, int>>();
            this.terminalWeights = new Dictionary<MarkovNode<T>, int>();
        }

        /// <summary>
        /// Adds items to the Markov chain, including termini.
        /// </summary>
        /// <param name="items">The items to add.</param>
        public void AddItems(IEnumerable<T> items)
        {
            Queue<T> previous = new Queue<T>();

            // Add each item to the chain.
            foreach (T item in items)
            {
                AddItem(previous, item);

                previous.Enqueue(item);
                if (previous.Count > order)
                    previous.Dequeue();
            }

            // Add to the terminal weights
            MarkovNode<T> key = new MarkovNode<T>(previous);
            if (!terminalWeights.ContainsKey(key))
                terminalWeights.Add(key, 0);
            terminalWeights[key]++;
        }

        /// <summary>
        /// Adds an item to the Markov chain.
        /// </summary>
        /// <param name="previous">The previous items in the chain.</param>
        /// <param name="item">The item to add.</param>
        private void AddItem(Queue<T> previous, T item)
        {
            if (previous.Count > order)
                throw new ArgumentException("The queue is longer than the Markov chain order", "previous");

            MarkovNode<T> key = new MarkovNode<T>(previous);
            if (!chain.ContainsKey(key))
                chain.Add(key, new Dictionary<T, int>());
            if (!chain[key].ContainsKey(item))
                chain[key].Add(item, 0);
            chain[key][item]++;
        }

        /// <summary>
        /// Gets an series of items, generated from the Markov chain.
        /// </summary>
        /// <returns>An IEnumerable of items.</returns>
        public IEnumerable<T> Generate()
        {
            Random random = new Random();
            Queue<T> previous = new Queue<T>();

            while (true)
            {
                while (previous.Count > order)
                    previous.Dequeue();

                MarkovNode<T> key = new MarkovNode<T>(previous);

                Dictionary<T, int> weights;
                // If terminus is reached.
                if (!chain.TryGetValue(key, out weights))
                    yield break;

                int terminalWeight = 0;
                terminalWeights.TryGetValue(key, out terminalWeight);

                int sumWeights = weights.Sum(w => w.Value);
                int randomValue = random.Next(sumWeights + terminalWeight) + 1;

                // If terminus is chosen.
                if (randomValue > sumWeights)
                    yield break;

                // Loop through the chain, adding the weights of each step.
                // When 'randomValue' is less than or equal to the cumulative
                // weights so far, the current item is chosen.
                int cumulativeWeight = 0;
                foreach (var pair in weights)
                {
                    cumulativeWeight += pair.Value;
                    if (randomValue <= cumulativeWeight)
                    {
                        yield return pair.Key;
                        previous.Enqueue(pair.Key);
                        break;
                    }
                }
            }
        }
    }
}
