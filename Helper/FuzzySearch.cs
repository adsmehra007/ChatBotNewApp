using ChatBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChatBot.Helper
{
    public class FuzzySearch
    {
        public static int LevenshteinDistance(string src, string dest)
        {
            int[,] d = new int[src.Length + 1, dest.Length + 1];
            int i, j, cost;
            char[] str1 = src.ToCharArray();
            char[] str2 = dest.ToCharArray();

            for (i = 0; i <= str1.Length; i++)
            {
                d[i, 0] = i;
            }
            for (j = 0; j <= str2.Length; j++)
            {
                d[0, j] = j;
            }
            for (i = 1; i <= str1.Length; i++)
            {
                for (j = 1; j <= str2.Length; j++)
                {

                    if (str1[i - 1] == str2[j - 1])
                        cost = 0;
                    else
                        cost = 1;

                    d[i, j] =
                        Math.Min(
                            d[i - 1, j] + 1,              // Deletion
                            Math.Min(
                                d[i, j - 1] + 1,          // Insertion
                                d[i - 1, j - 1] + cost)); // Substitution

                    if ((i > 1) && (j > 1) && (str1[i - 1] ==
                        str2[j - 2]) && (str1[i - 2] == str2[j - 1]))
                    {
                        d[i, j] = Math.Min(d[i, j], d[i - 2, j - 2] + cost);
                    }
                }
            }

            return d[str1.Length, str2.Length];
        }

        public static List<Patient> Search(string word,List<Patient> wordList,double fuzzyness)
        {
            List<Patient> foundWords = new List<Patient>();

            foreach (Patient s in wordList)
            {
                try
                {
                    // Calculate the Levenshtein-distance:
                    int levenshteinDistance =
                        LevenshteinDistance(word, s.Name);

                    // Length of the longer string:
                    int length = Math.Max(word.Length, s.Name.Length);

                    // Calculate the score:
                    double score = 1.0 - (double)levenshteinDistance / length;

                    // Match?
                    if (score > fuzzyness)
                        foundWords.Add(s);
                }
                catch (Exception)
                {

                }
               
            }
            return foundWords;
        }
        public static List<Patient> LNSearch(string word, List<Patient> wordList, double fuzzyness)
        {
            List<Patient> foundWords = new List<Patient>();

            foreach (Patient s in wordList)
            {
                try
                {
                    // Calculate the Levenshtein-distance:
                    int levenshteinDistance =
                        LevenshteinDistance(word, s.LastName);

                    // Length of the longer string:
                    int length = Math.Max(word.Length, s.LastName.Length);

                    // Calculate the score:
                    double score = 1.0 - (double)levenshteinDistance / length;

                    // Match?
                    if (score > fuzzyness)
                        foundWords.Add(s);
                }
                catch (Exception)
                {

                }
               
            }
            return foundWords;
        }
    }
}