﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            char[] delimiterChars = { ']', '[', '}', '{','(', ')',' ', ',', '.', '\t' };

            string text = "1\t2 10:4: 20, (10)   5 7  ";
            System.Console.WriteLine($"Original text: '{text}'");
            try
            {
                string[] words = text.Split(delimiterChars);
                words = words.Where(x => !string.IsNullOrEmpty(x)).ToArray();
                System.Console.WriteLine($"{words.Length} words in text:");
                //int[] numArr = new int[] { };
                List<int> numArrList = new List<int>();
                int[] numArrStep = new int[0];
                string[] tempStr = new string[] { };
                int firstVal = new int { };
                int step = new int { };
                int secondVal = new int { };
                int count = new int { };
                for (int i = 0; i < words.Length; i++)
                {
                    if (words[i].Contains(":") == true)
                    {
                        tempStr = words[i].Split(':');
                        firstVal = int.Parse(tempStr[0]);
                        step = int.Parse(tempStr[1]);
                        secondVal = int.Parse(tempStr[2]);
                        for (int val = firstVal; val <= secondVal; val = (val + step))
                        {
                            numArrList.Add(val);
                        }
                    }
                    else
                    {
                        numArrList.Add(int.Parse(words[i]));
                    }
                    Console.WriteLine("Value of i: {0}", i);
                }
                int[] numArr = numArrList.ToArray();
                Array.Sort(numArr);
            }
            catch
            {
                Console.WriteLine("wrong syntaxis");
                Console.ReadKey();
                
            }
        }
    }
}
