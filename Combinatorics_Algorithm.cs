using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Combinatorics.Collections;

// Mixed Optimisation Algorithm TM Gludis 2014, Created by: Rolandas Rimkus
namespace Mixed_Optimisation_Algorithm_Library
{
    // In Package Manager Console write:
    // PM> Install-Package Combinatorics
    // Then add using Combinatorics.Collections like at Line: 6
    class Combinatorics_Algorithm
    {
        // Global variables
        List<Combinations<string>> combinationsList;
        List<long> combinationsCounts;
        List<long> combinationsTimeTaken;

        public string Combinatorics_Algorithm_Start()
        {
            Create_Combinations();
            return Return_Combinations();
        }
        private void Create_Combinations()
        {
            combinationsList = new List<Combinations<string>> { };
            combinationsCounts = new List<long> { };
            combinationsTimeTaken = new List<long> { };
            DateTime Start = DateTime.Now;
            for (int i = 1; i < 18; i++)
            {
                List<string> inputSet = new List<string> { };
                for (int j = 97; j < 101; j++) // chars from a to d
                    inputSet.Add(((char)j).ToString());
                Combinations<string> combinations = new Combinations<string>(inputSet, i, GenerateOption.WithRepetition);
                combinationsList.Add(combinations);
                combinationsCounts.Add(combinations.Count);
            }
            DateTime End = DateTime.Now;
            combinationsTimeTaken.Add((End.Ticks - Start.Ticks) / 10000);
        }
        private string Return_Combinations()
        {
            string TimeString = "Time taken for the creation of the structure: " + combinationsTimeTaken[0] + " Milliseconds (1000 = 1 second)" + '\n';
            string CombinationString = "Number of possibilities in each combination:" + '\n';
            for (int i = 0; i < combinationsCounts.Count; i++)
                CombinationString += (i+1).ToString() + ") " + combinationsCounts[i].ToString() + '\n';
            CombinationString += '\n' + "Combinations:" + '\n';
            int Num = 1;
            foreach (Combinations<string> combination in combinationsList)
            {
                DateTime Start = DateTime.Now;
                foreach (IList<string> Element in combination)
                {
                    foreach (string String in Element)
                        CombinationString += String + " ";
                    CombinationString += '\n';
                }
                CombinationString += '\n';
                DateTime End = DateTime.Now;
                long ticks = (End.Ticks - Start.Ticks) / 10000;
                combinationsTimeTaken.Add(ticks);
                TimeString += Num.ToString() + ") " + (ticks).ToString() + '\n';
                Num++;
            }
            CombinationString = TimeString + CombinationString;
            return CombinationString;
        }
    }
}
