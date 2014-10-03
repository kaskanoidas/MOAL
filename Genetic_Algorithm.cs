using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Collections.Concurrent;
using System.Diagnostics;

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
        int Thread_Count_Running;
        private AutoResetEvent Wait_For_Thread_End = new AutoResetEvent(false);
        ConcurrentStack<Solution> Solution_List_Threading = new ConcurrentStack<Solution> { };
        ConcurrentStack<int> Thread_End_Counter = new ConcurrentStack<int> { };
        string Type; int count; double argument; int NumberOfThreads;
        CountdownEvent e;

        public string Genetic_Algorithm_Start(Task task, List<Solution> Solutions, int _NumberOfThreads)
        {
            Stopwatch time = Stopwatch.StartNew();
            Task = task;
            Solution_List = new List<Solution>(Solutions);
            Generate_Task();
            NumberOfThreads = _NumberOfThreads;
            ThreadPool.SetMaxThreads(_NumberOfThreads + 1, 4);
            Thread MainThread = new Thread(new ThreadStart(Genetic_Algorithm_Loop));
            MainThread.Start();
            MainThread.Join();
            string answ = Return_Genetic_Algorithm();
            time.Stop();
            return answ + "\n Time taken for calculation: " + time.ElapsedMilliseconds.ToString() + " Milliseconds";
        }
        private string Return_Genetic_Algorithm()
        {
            Answer = "The best solution that the Genetic algorithm calculated: \n \n";
            for (int i = 0; i < Solution_List[0].Unknowns.Count; i++)
            {
                if (Solution_List[0].Unknowns[i] > 0)
                {
                    Answer += "x" + (i + 1) + " = " + Solution_List[0].Unknowns[i] + "; ";
                }
            }
            Answer += "\n";
            for (int i = 0; i < Task.Rezults.Count; i++)
            {
                for (int j = 0; j < Task.Unknown_Multipliers[i].Count; j++)
                {
                    if (j != Task.Unknown_Multipliers[i].Count - 1)
                    {
                        Answer += Task.Unknown_Multipliers[i][j] + "x" + (j + 1) + " + ";
                    }
                    else
                    {
                        Answer += Task.Unknown_Multipliers[i][j] + "x" + (j + 1) + " = ";
                    }
                }
                Answer += Task.Rezults[i] + "  The residual is " + Solution_List[0].Residuals[i] + "\n";
            }
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
            int NumberOfLoops = 100; // change this number for longer but better solution.
            int CurrentLoopNumber = 0;
            int How_Many_To_Generate = (Number_Of_Unique_Unkowns * Number_Of_Results * 1000 ) - Solution_List.Count; // 100
            if (How_Many_To_Generate > 0)
            {
                Generate_Starting_Solutions(How_Many_To_Generate);
                Validate_Solutions(0.9);
            }
            else
            {
                Validate_Solutions(0);
                Solution_List.RemoveRange(Solution_List.Count - How_Many_To_Generate, How_Many_To_Generate);
            }
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
            Solution_List_Threading = new ConcurrentStack<Solution>() { };
            count = Number_To_Make / NumberOfThreads;
            using (e = new CountdownEvent(NumberOfThreads))
            {
                for (int i = 0; i < NumberOfThreads; i++)
                {
                    ThreadPool.QueueUserWorkItem(new WaitCallback(Generate_Starting_Solutions));
                }
                e.Wait();
            }
            Solution_List.AddRange(Solution_List_Threading.ToList());
        }
        private void Generate_Starting_Solutions(object Thread_Info)
        {
            Random Random_Number = new Random();
            for (int Made_Items = 0; Made_Items < count; Made_Items++)
            {
                Solution New_Solution = new Solution(Task.Rezults, Task.Rezults.Sum());
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
                Solution_List_Threading.Push(New_Solution);
            }
            e.Signal();
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
        private void Generate_Children_Solutions(int Generation_Ratio, double Generation_Aggression)
        {
            Solution_List_Threading = new ConcurrentStack<Solution>() { };
            if (NumberOfThreads != 1)
            {
                count = 1;
            }
            else
            {
                count = Generation_Ratio;
                Generation_Ratio = 1;
            }
            argument = Generation_Aggression;
            using (e = new CountdownEvent(Generation_Ratio))
            {
                for (int i = 0; i < Generation_Ratio; i++)
                {
                    ThreadPool.QueueUserWorkItem(new WaitCallback(Generate_Children_Solutions));
                }
                e.Wait();
            }
            Solution_List.AddRange(Solution_List_Threading);
        }
        private void Generate_Children_Solutions(object Thread_Info)
        {
            Random Random_Number = new Random();
            int Original_Count = Solution_List.Count;
            for (int i = 0; i < Original_Count; i++)
            {
                for (int j = 0; j < count; j++)
                {
                    Solution New_Solution = new Solution(Solution_List[i]);
                    for (int h = 0; h < New_Solution.Unknowns.Count; h++)
                    {
                        int New_Value = Convert.ToInt32(New_Solution.Unknowns[h] * argument);
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
                    Solution_List_Threading.Push(New_Solution);
                }
            }
            e.Signal();
        }
        private void Validate_Solutions(double Removal_Ratio)
        {
            Solution_List_Threading = new ConcurrentStack<Solution>() { };
            List<Solution> Holder = new List<Solution>() { };
            count = Convert.ToInt32(Solution_List.Count * (1 - Removal_Ratio));
            int Count_Original = count;
            for (int n = 0; n < Count_Original; n++)
            {
                using (e = new CountdownEvent(NumberOfThreads))
                {
                    for (int i = 0; i < NumberOfThreads; i++)
                    {
                        ThreadPool.QueueUserWorkItem(new WaitCallback(Validate_Solutions), i);
                    }
                    e.Wait();
                }
                int max = -1;
                int mx = -1;
                for (int i = 0; i < Solution_List_Threading.Count; i++)
                {
                    if (max < Solution_List_Threading.ElementAt(i).Unknowns_Sum)
                    {
                        max = Solution_List_Threading.ElementAt(i).Unknowns_Sum;
                        mx = i;
                    }
                }
                Holder.Add(new Solution(Solution_List_Threading.ElementAt(mx)));
                Solution_List_Threading.Clear();
                Solution_List.RemoveAt(mx);
                count = Convert.ToInt32(Solution_List.Count * (1 - Removal_Ratio));
            }
            Solution_List = new List<Solution>(Holder); Holder.Clear();
        }
        private void Validate_Solutions(object Thread_Info)
        {
            int start = count / NumberOfThreads * (int)Thread_Info;
            int end = count / NumberOfThreads * ((int)Thread_Info + 1);
            if ((int)Thread_Info < Solution_List.Count)
            {
                if (end == 0)
                {
                    Solution_List_Threading.Push(new Solution(Solution_List[(int)Thread_Info]));
                }
                else
                {
                    int max = -1;
                    int mx = -1;
                    for (int i = start; i < end; i++)
                    {
                        if (max < Solution_List[i].Unknowns_Sum)
                        {
                            max = Solution_List[i].Unknowns_Sum;
                            mx = i;
                        }
                    }
                    Solution_List_Threading.Push(new Solution(Solution_List[mx]));
                }
            }
            e.Signal();
        }
    }
}
