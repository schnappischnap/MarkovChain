using System;
using System.IO;
using Schnappischnap.Markov;


static class Program
{
    static void Main()
    {
        // Initialise a Markov chain of type 'char'.
        MarkovChain<char> chain = new MarkovChain<char>(3);

        // Add all the words from 'wordlist.txt' to the chain.
        foreach(string line in File.ReadLines("wordlist.txt"))
        {
            chain.AddItems(line);
        }

        // Generate and print a new word every time the return key is pressed.
        while(true)
        {
            Console.Write(string.Join("", chain.Generate()));
            Console.ReadLine();
        }
    }
}