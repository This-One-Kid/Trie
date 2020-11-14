using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Trie
{
    class Program
    {
        static void Main(string[] args)
        {
            //Do the "suggest words" 
            string path = "fulldictionary.json";
            TrieTree trie = new TrieTree();
            var jsonString = File.ReadAllText(path);
            var words = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonString);

            foreach (var kvp in words)
            {
                trie.Insert(kvp.Key);
            }
            bool check = true;
            while (check)
            {
                Console.WriteLine("Please write a prefix of a word, and all words having that prefix will be given. If you enter a full word, then the\ndefinition will be printed out.");
                string userInput = Console.ReadLine();
                if (trie.Search(userInput) != null)
                {
                    if (trie.Search(userInput).isWord)
                    {
                        Console.WriteLine($"Definition: {words[userInput]}");
                    }
                    List<string> answers = trie.GetAllMatchingPrefixes(userInput);
                    Console.WriteLine($"Here are all of the words with the prefix '{userInput}'");
                    for (int i = 0; i < answers.Count; i++)
                    {
                        Console.WriteLine(answers[i]);
                    }
                    while (true)
                    {
                        Console.WriteLine("If you would like to exit this, enter the character 'y'\nIf not, enter the character 'n'");
                        userInput = Console.ReadLine().ToLower();
                        if (userInput == "y")
                        {
                            check = false;
                            Console.WriteLine("Ok, have a great day!");
                            break;
                        }
                        else if (userInput == "n")
                        {
                            break;
                        }
                        else
                        {
                            Console.WriteLine("That is not a valid input. Please try again.");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("I'm sorry, but that is not a prefix to any word in the dictionary. Please input a different word.");
                }
            }
        }
    }
}
