using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Mixed Optimisation Algorithm TM Gludis 2014, Created by: Rolandas Rimkus
namespace Mixed_Optimisation_Algorithm_Library
{
    class Genetic_Algorithm
    {
        Task Task = new Task();
        List<Solution> Solution_List;
        string Answer;
        int Number_Of_Unique_Unkowns;
        int Number_Of_Results;
        public string Genetic_Algorithm_Start()
        {
            Generate_Task();
            Genetic_Algorithm_Loop();
            return Return_Genetic_Algorithm();
        }
        private string Return_Genetic_Algorithm()
        {
            Answer = "The best solution that the Genetic algorithm calculated: \n \n";
            for (int i = 0; i < Solution_List[0].Unknowns.Count;i++ )
            {
                if (Solution_List[0].Unknowns[i] > 0)
                {
                    Answer += "x" + (i + 1) + " = " + Solution_List[0].Unknowns[i] + "; "; 
                }
            }
            Answer += "\n";
            Answer += "3x1 + 5x2 + 8x3 + 10x4 + 18x5 = 3600" + "  The residual is " + Solution_List[0].Residuals[0] + "\n";
            Answer += "7x1 + x2  + 9x3 + 11x4 + 10x5 = 5010" + "  The residual is " + Solution_List[0].Residuals[1] + "\n";
            Answer += "9x1 + 3x2 + 2x3 +  8x4 +  0x5 = 3000" + "  The residual is " + Solution_List[0].Residuals[2] + "\n";
            Answer += "Sum of the residuals: " + (Solution_List[0].Residuals_Sum) + "\n";
            Answer += "Sum of the unknows values: " + Solution_List[0].Unknowns_Sum + "\n";
            Answer += "\n";
            return Answer;
        }
        private void Generate_Task() 
        {
            Number_Of_Unique_Unkowns = Task.Unknown_Multipliers[0].Count;
            Number_Of_Results = Task.Rezults.Count;
        }
        private void Genetic_Algorithm_Loop()
        {
            int NumberOfLoops = 10000; // change this number for longer but better solution.
            int CurrentLoopNumber = 0;
            Generate_Starting_Solutions(Number_Of_Unique_Unkowns * Number_Of_Results);
            Validate_Solutions(0);
            int Best_Answer_Sum = Solution_List[0].Unknowns_Sum;
            while (CurrentLoopNumber != NumberOfLoops)
            {
                Generate_Children_Solutions(9, 0.9);
                Validate_Solutions(0.90);
                if (Best_Answer_Sum == Solution_List[0].Unknowns_Sum)
                {
                    CurrentLoopNumber++;
                }
                else
                {
                    CurrentLoopNumber = 0;
                    Best_Answer_Sum = Solution_List[0].Unknowns_Sum;
                }
            }
        }
        private void Generate_Starting_Solutions(int Number_To_Make)
        {
            Random Random_Number = new Random();
            Solution_List = new List<Solution>() { };
            for(int Made_Items = 0; Made_Items < Number_To_Make; Made_Items++)
            {
                Solution New_Solution = new Solution();
                List<int> UnusedUnkowns = new List<int>(Enumerable.Range(0,Number_Of_Unique_Unkowns));
                while (UnusedUnkowns.Count != 0)
                {
                    int Random_Unknown_Picked = Random_Number.Next(0, UnusedUnkowns.Count);
                    int Unknown_Value = Random_Number.Next(0, Find_Max_Unknown_Value(Random_Unknown_Picked, New_Solution) + 1);
                    if (Unknown_Value != 0)
                    {
                        Reduce_Residuals(Random_Unknown_Picked, Unknown_Value, ref New_Solution);
                        New_Solution.Unknowns[Random_Unknown_Picked] += Unknown_Value;
                        New_Solution.Unknowns_Sum += Unknown_Value;
                    }
                    else
                    {
                        UnusedUnkowns.RemoveAt(Random_Unknown_Picked);
                    }
                }
                Solution_List.Add(New_Solution);
            }
        }
        private int Find_Max_Unknown_Value(int Unknown_Nr, Solution _Solution)
        {
            int Max_Value = _Solution.Residuals[0] / Task.Unknown_Multipliers[0][Unknown_Nr];
            for (int i = 0; i < Number_Of_Results; i++)
            {
                if (Task.Unknown_Multipliers[i][Unknown_Nr] != 0)
                {
                    int Test_Value = _Solution.Residuals[i] / Task.Unknown_Multipliers[i][Unknown_Nr];
                    if (Test_Value < Max_Value)
                    {
                        Max_Value = Test_Value;
                    }

                }
            }
            return Max_Value;
        }
        private void Reduce_Residuals(int _Unknown_Nr, int _Unknown_Value, ref Solution _Solution)
        {
            for (int i = 0; i < _Solution.Residuals.Count; i++)
            {
                int SubTr = _Unknown_Value * Task.Unknown_Multipliers[i][_Unknown_Nr];
                _Solution.Residuals[i] -= SubTr;
                _Solution.Residuals_Sum -= SubTr;
            }
        }
        private void Generate_Children_Solutions(double Generation_Ratio, double Generation_Aggression)
        {
            Random Random_Number = new Random();
            int Original_Count = Solution_List.Count;
            for (int i = 0; i < Original_Count; i++)
            {
                for (int j = 0; j < Generation_Ratio; j++)
                {
                    Solution New_Solution = new Solution();
                    for (int h = 0; h < New_Solution.Unknowns.Count; h++)
                    {
                        int New_Value = Convert.ToInt32(New_Solution.Unknowns[h] * Generation_Aggression);
                        New_Solution.Unknowns[h] = New_Value;
                        Reduce_Residuals(h, New_Value - New_Solution.Unknowns[h], ref New_Solution);
                    }
                    List<int> UnusedUnkowns = new List<int>(Enumerable.Range(0, Number_Of_Unique_Unkowns));
                    while (UnusedUnkowns.Count != 0)
                    {
                        int Random_Unknown_Picked = Random_Number.Next(0, UnusedUnkowns.Count);
                        int Unknown_Value = Random_Number.Next(0, Find_Max_Unknown_Value(Random_Unknown_Picked, New_Solution) + 1);
                        if (Unknown_Value != 0)
                        {
                            Reduce_Residuals(Random_Unknown_Picked, Unknown_Value, ref New_Solution);
                            New_Solution.Unknowns[Random_Unknown_Picked] += Unknown_Value;
                            New_Solution.Unknowns_Sum += Unknown_Value;
                        }
                        else
                        {
                            UnusedUnkowns.RemoveAt(Random_Unknown_Picked);
                        }
                    }
                    Solution_List.Add(New_Solution);
                }
            }
        }
        private void Validate_Solutions(double Removal_Ratio)
        {
            int Number_Of_Kept_Solutions = Convert.ToInt32(Solution_List.Count * (1 - Removal_Ratio)); 
            List<Solution> Validated_Solution_List = new List<Solution>() { };
            for (int i = 0; i < Number_Of_Kept_Solutions; i++)
            {
                int max = -1;
                int mx = -1;
                for(int j = 0; j < Solution_List.Count; j++)
                {
                    if (max < Solution_List[j].Unknowns_Sum)
                    {
                        max = Solution_List[j].Unknowns_Sum;
                        mx = j;
                    }
                }
                Solution Solution_Holder = new Solution();
                Solution_Holder.Residuals.Clear(); Solution_Holder.Residuals.AddRange(Solution_List[mx].Residuals);
                Solution_Holder.Unknowns.Clear(); Solution_Holder.Unknowns.AddRange(Solution_List[mx].Unknowns);
                Solution_Holder.Residuals_Sum = Solution_List[mx].Residuals_Sum;
                Solution_Holder.Unknowns_Sum = Solution_List[mx].Unknowns_Sum;
                Validated_Solution_List.Add(Solution_Holder);
                Solution_List.RemoveAt(mx);
            }
            Solution_List = Validated_Solution_List;
        }
    }
    public class Solution
    {
        public List<int> Unknowns = new List<int>(){0,0,0,0,0};
        public List<int> Residuals = new List<int>(){3600,5010,3000};
        public int Unknowns_Sum = 0;
        public int Residuals_Sum = 3600 + 5010 + 3000;
    }
    public class Task
    {
        public List<int> Rezults = new List<int>(){3600,5010,3000};
        public List<List<int>> Unknown_Multipliers = new List<List<int>>() 
        {
            new List<int>(){3,5,8,10,18},
            new List<int>(){7,1,9,11,10},
            new List<int>(){9,3,2,8,0},
        };
    }
}
