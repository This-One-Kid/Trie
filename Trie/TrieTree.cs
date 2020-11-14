using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Trie
{
    public class TrieNode
    {
        public char Letter { get; private set; }
        public Dictionary<char, TrieNode> Children { get; private set; }
        public bool isWord { get; set; }
        public TrieNode(char c)
        {
            Children = new Dictionary<char, TrieNode>();
            isWord = false;
            Letter = c;
        }
    }
    public class TrieTree
    {
        public TrieNode Root = new TrieNode('$');
        public int Count { get; private set; }
        public TrieTree()
        {
            Clear();
        }
        public void Clear()
        {
            Count = 0;
            Root = new TrieNode('$');
        }
        public bool Insert(string word)
        {
            TrieNode temp = Root;
 
            for (int i = 0; i < word.Length; i++)
            {
                if (temp.Children.ContainsKey(word[i]))
                {
                    temp = temp.Children[word[i]];
                }
                else
                {
                    temp.Children.Add(word[i], new TrieNode(word[i]));
                    temp = temp.Children[word[i]];
                }
            }
            if (temp.isWord)
            {
                return false;
            }
            temp.isWord = true;
            Count++;
            return true;
        }
        public bool Remove(string word)
        {
            var node = Search(word);

            if (node is null)
            {
                return false;
            }

            if (node.isWord)
            {
                node.isWord = false;
                Count--;
                return true;
            }
            return false;
        }
        public bool Contains(string word)
        {
            TrieNode temp = Root;
            for (int i = 0; i < word.Length; i++)
            {
                if (!temp.Children.ContainsKey(word[i]))
                {
                    return false;
                }
                else
                {
                    temp = temp.Children[word[i]];
                }
                if (i == word.Length - 1)
                {
                    return true;
                }
            }
            return false;
        }

        public TrieNode Search(string prefix)
        {
            TrieNode temp = Root;
            for (int i = 0; i < prefix.Length; i++)
            {
                if (temp.Children.ContainsKey(prefix[i]))
                {
                    temp = temp.Children[prefix[i]];
                }
                else
                {
                    return null;
                }
            }

            return temp;

        }
        public List<string> GetAllMatchingPrefixes(string prefix)
        {
            List<string> list = new List<string>();
            TrieNode temp = Search(prefix);

            if (temp == null)
            {
                return list;
            }


            DepthSearch(temp, list, prefix);

            return list;
        }

        public List<string> GetAllWords()
        {
            List<string> words = new List<string>();

            DepthSearch(Root, words, "");

            return words;
        }

        private void DepthSearch(TrieNode currNode, List<string> list, string pre)
        {

            if (currNode == null)
            {
                return;
            }

            if (currNode.isWord)
            {
                list.Add(pre);
            }

            foreach (var kvp in currNode.Children)
            {
                DepthSearch(kvp.Value, list, pre + kvp.Key);
            }
        }
    }
}
