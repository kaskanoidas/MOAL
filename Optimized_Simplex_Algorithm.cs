using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Mixed Optimisation Algorithm TM Gludis 2014, Created by: Rolandas Rimkus
namespace Mixed_Optimisation_Algorithm_Library
{
    class Optimized_Simplex_Algorithm
    {
        // Global variables
        Simplex_Table Table;
        Simplex_Table Table_Back;
        List<Tuple<int, int>> Best_Answer_Data;
        List<int> Residual_Back;
        string Answer;
        Boolean Continue_Calculation;
        Boolean Start_Sum_Process;

        // Equation system we used (hardcoded):
        // 3x1 + 5x2 + 8x3 + 10x4 + 18x5 = 3600
        // 7x1 + x2  + 9x3 + 11x4 + 10x5 = 5010
        // 9x1 + 3x2 + 2x3 +  8x4 +  0x5 = 3000
        public string Optimized_Simplex_Algorithm_Start()
        {
            Simplex_Deep_Cycle();
            return Return_Optimized_Simplex_Algorithm();
        }
        private string Return_Optimized_Simplex_Algorithm()
        {
            return Answer;
        }
        private void Reset_Best_Answer_Data()
        {
            Best_Answer_Data = new List<Tuple<int, int>>() // 0 index == Sum value and residual, other's unknown and value
            {
                new Tuple<int,int>(0,3600 + 5010 + 3000),
                new Tuple<int,int>(1,0), new Tuple<int,int>(2,0), new Tuple<int,int>(3,0), 
                new Tuple<int,int>(4,0), new Tuple<int,int>(5,0)
            };
        }
        private void Simplex_Deep_Cycle()
        {
            Reset_Best_Answer_Data();
            Residual_Back = new List<int>() { 3600, 5010, 3000 }; // hardcoded results
            Start_Sum_Process = false;
            Continue_Calculation = true;
            while (Continue_Calculation == true)
            {
                Make_Simplex_Tabel();
                int Number_Of_Cycles = Table.Rows[Table.Rows.Count - 1].Row_Values.Count; // Math.Min(500, ...); for recursion
                Continue_Calculation = false;
                Simplex_Cycle(Table, Number_Of_Cycles);
                Start_Sum_Process = true;
            }
        }
        private void Simplex_Cycle(Simplex_Table Lentele, int count)
        {
            for (int i = 0; i < count; i++) // FIX
            {
                Simplex_Cycle_Step(Lentele, i);
            }
        }
        private void Simplex_Cycle_Step(Simplex_Table Table, int count)
        {
            Simplex_Table Table_Back = new Simplex_Table();
            Table_Back = Make_Simplex_Tabel_Copy(Table);
            Boolean stop = false;
            while (Check_Symplex_Rules(Table_Back) == true)
            {
                List<int> Residual = new List<int> { };
                int Max_Index = Find_Max_Index(Table_Back, count);
                Tuple<int, double> back = Find_Min_Selected_Indicators_Result(Max_Index, Table_Back);
                int Min_Selected_Indicator = back.Item1;
                int Min_Selected_Indicators_Result = Convert.ToInt32(Math.Floor(back.Item2));
                if (Min_Selected_Indicator == -1 && Min_Selected_Indicators_Result == -1)
                {
                    break;
                }
                else
                {
                    Table_Back.Selected_Indicator[Min_Selected_Indicator] = Max_Index;
                    Transform_Main_Row(Min_Selected_Indicator, Max_Index, ref Table_Back);
                    Transform_Other_Rows(Min_Selected_Indicator, Max_Index, ref Table_Back);
                    Transform_Main_Collumn(Min_Selected_Indicator, Max_Index, ref Table_Back);
                    Recount_Selected_Indicators_Result(Min_Selected_Indicator, Max_Index, Min_Selected_Indicators_Result, ref Table_Back, ref Residual);
                    stop = Test_Simplex_Answer(Table_Back, Residual);
                }
            }
        }
        private void Make_Simplex_Tabel()
        {
            Table = new Simplex_Table(); Table_Back = new Simplex_Table();         // Back = hard-copy of table
            Row row; Row row_Back;

            Table.Selected_Indicator.AddRange(new List<int>() { -1, -1, -1 });
            Table_Back.Selected_Indicator.AddRange(new List<int>() { -1, -1, -1 });
            Table.Selected_Indicators_Result.AddRange(Residual_Back);
            Table_Back.Selected_Indicators_Result.AddRange(Residual_Back);

            row = new Row(); row_Back = new Row();
            row.Row_Values.AddRange(new List<double>() { 3, 5, 8, 10, 18 });
            row_Back.Row_Values.AddRange(new List<double>() { 3, 5, 8, 10, 18 });
            Table.Rows.Add(row); Table_Back.Rows.Add(row_Back);

            row = new Row(); row_Back = new Row();
            row.Row_Values.AddRange(new List<double>() { 7, 1, 9, 11, 10 });
            row_Back.Row_Values.AddRange(new List<double>() { 7, 1, 9, 11, 10 });
            Table.Rows.Add(row); Table_Back.Rows.Add(row_Back);

            row = new Row(); row_Back = new Row();
            row.Row_Values.AddRange(new List<double>() { 9, 3, 2, 8, 0 });
            row_Back.Row_Values.AddRange(new List<double>() { 9, 3, 2, 8, 0 });
            Table.Rows.Add(row); Table_Back.Rows.Add(row_Back);

            row = new Row(); row_Back = new Row();
            row.Row_Values.AddRange(new List<double>() { 1, 1, 1, 1, 1 });
            row_Back.Row_Values.AddRange(new List<double>() { 1, 1, 1, 1, 1 });
            Table.Rows.Add(row); Table_Back.Rows.Add(row_Back);
        }
        private Simplex_Table Make_Simplex_Tabel_Copy(Simplex_Table Table_To_Copy)
        {
            Simplex_Table New_Table = new Simplex_Table();
            New_Table.Selected_Indicator.AddRange(Table_To_Copy.Selected_Indicator);
            New_Table.Selected_Indicators_Result.AddRange(Table_To_Copy.Selected_Indicators_Result);

            List<Row> TableList = Table_To_Copy.Rows; int count = TableList.Count;
            for (int i = 0; i < count; i++)
            {
                List<double> DoubleList = TableList[i].Row_Values;
                Row row = new Row();
                row.Row_Values.AddRange(DoubleList);
                New_Table.Rows.Add(row);
            }
            return New_Table;
        }
        private Boolean Check_Symplex_Rules(Simplex_Table Table)
        {
            int count = Table.Selected_Indicators_Result.Count;
            for (int i = 0; i < count; i++)
            {
                if (Table.Selected_Indicators_Result[i] < 0)
                {
                    return false;
                }
            }
            int Row_Count = Table.Rows.Count - 1;
            List<double> Row = Table.Rows[Row_Count].Row_Values;
            count = Row.Count;
            for (int i = 0; i < count; i++)
            {
                double tikrinimui = Row[i];
                if (Math.Round(tikrinimui, 2, MidpointRounding.ToEven) > 0)   // FIX : change to new logic that detects if the number is near 0 and does nothing.
                {
                    return true;
                }
            }
            return false;
        }
        private int Find_Max_Index(Simplex_Table Table, int Cycle_Number)
        {
            // FIX : Cycle_Number remove???
            int Row_Count = Table.Rows.Count - 1;
            List<double> Row = Table.Rows[Row_Count].Row_Values;
            int count = Row.Count;
            double max = 0;
            int mx = 0;
            for (int i = 0; i < count; i++)
            {
                if (max < Row[i])
                {
                    max = Row[i];
                    mx = i;
                }
            }
            List<int> List_Index_Equal_Index = new List<int>() { };
            for (int i = 0; i < count; i++)
            {
                if (max == Row[i])
                {
                    List_Index_Equal_Index.Add(i);
                }
            }
            if (List_Index_Equal_Index.Count > 1)
            {
                // FIX : add recursion here for each of the List_Index_Equal_Index 
                return mx;
            }
            else
            {
                return mx;
            }
        }
        private Tuple<int, double> Find_Min_Selected_Indicators_Result(int Max_Index, Simplex_Table Table)
        {
            double min = -1;
            int mn = 0;
            List<Row> Row_List = Table.Rows;
            int count = Row_List.Count - 1;
            for (int i = 0; i < count; i++)
            {
                double value = Table.Selected_Indicators_Result[i] / Row_List[i].Row_Values[Max_Index];
                if (value > 0)
                {
                    min = value;
                    i = count;
                }
            }
            if (min != -1)
            {
                for (int i = 0; i < count; i++)
                {
                    if (Row_List[i].Row_Values[Max_Index] > 0)
                    {
                        double tikrinti = Table.Selected_Indicators_Result[i] / Row_List[i].Row_Values[Max_Index];
                        if (tikrinti < min && tikrinti > 0)
                        {
                            min = tikrinti;
                            mn = i;
                        }
                    }
                }
                List<int> List_Selected_Indicators_Result = new List<int>() { };
                for (int i = 0; i < count; i++)
                {
                    if (Row_List[i].Row_Values[Max_Index] > 0)
                    {
                        double tikrinti = Table.Selected_Indicators_Result[i] / Row_List[i].Row_Values[Max_Index];
                        if (tikrinti == min)
                        {
                            List_Selected_Indicators_Result.Add(i);
                        }
                    }
                }
                if (List_Selected_Indicators_Result.Count > 1)
                {
                    // FIX: Add Recursion here.
                    return Tuple.Create(mn, min);
                }
                else
                {
                    return Tuple.Create(mn, min);
                }
            }
            else
            {
                return Tuple.Create(-1, -1.0);
            }
        }
        private void Transform_Main_Row(int Min_Selected_Indicators_Result, int Max_Index, ref Simplex_Table Table)
        {
            List<double> Row = Table.Rows[Min_Selected_Indicators_Result].Row_Values;
            double Max_Index_Value = Row[Max_Index];
            for (int i = 0; i < Row.Count; i++)
            {
                Row[i] = Math.Round((Row[i] / Max_Index_Value), 10); // FIX : Remove rounding
            }
        }
        private void Transform_Other_Rows(int Min_Selected_Indicators_Result, int Max_Index, ref Simplex_Table Table)
        {
            List<Row> Row = Table.Rows;
            int count = Row.Count;
            for (int i = 0; i < count; i++)
            {
                if (i != Min_Selected_Indicators_Result)
                {
                    List<double> Row2 = Row[i].Row_Values;
                    int count2 = Row2.Count;
                    for (int j = 0; j < count2; j++)
                    {
                        if (j != Max_Index)
                        {
                            double Calc = Row2[j] - Row2[Max_Index] * Table.Rows[Min_Selected_Indicators_Result].Row_Values[j];
                            Row2[j] = Math.Round(Calc, 10);     // FIX : Remove rounding
                        }
                    }
                }
            }
        }
        private void Transform_Main_Collumn(int Min_Selected_Indicators_Result, int Max_Index, ref Simplex_Table Table)
        {
            List<Row> Row = Table.Rows;
            int count = Row.Count;
            for (int i = 0; i < count; i++)
            {
                if (i != Min_Selected_Indicators_Result)
                {
                    Row[i].Row_Values[Max_Index] = 0;
                }
            }
        }
        private void Recount_Selected_Indicators_Result(int Min_Selected_Indicators_Result, int Max_Index, int Min_Selected_Indicators_Result_Value, ref Simplex_Table Table, ref List<int> Residual)
        {
            Table.Selected_Indicators_Result[Min_Selected_Indicators_Result] = Min_Selected_Indicators_Result_Value;
            List<Row> Row = Table_Back.Rows;
            int count = Row.Count - 1;
            for (int i = 0; i < count; i++)
            {
                int Resid = Table_Back.Selected_Indicators_Result[i] - Min_Selected_Indicators_Result_Value * Convert.ToInt32(Row[i].Row_Values[Max_Index]);
                Residual.Add(Resid);
            }
            count = Residual.Count;
            for (int i = 0; i < count; i++)
            {
                if (i != Min_Selected_Indicators_Result)
                {
                    if (Table.Selected_Indicator[i] >= 0)
                    {
                        double min = Convert.ToDouble(Residual[i]);
                        List<Row> Row2 = Table_Back.Rows;
                        int count2 = Row2.Count - 1;
                        for (int j = 0; j < count2; j++)
                        {
                            if (Row2[j].Row_Values[Table.Selected_Indicator[i]] > 0)
                            {
                                double kiekdalinti = Convert.ToDouble(Residual[j]) / Row2[j].Row_Values[Table.Selected_Indicator[i]]; // ????????????????????
                                if (kiekdalinti < min)
                                {
                                    min = kiekdalinti;
                                }
                            }
                        }
                        Table.Selected_Indicators_Result[i] = Convert.ToInt32(Math.Floor(min));
                        for (int j = 0; j < Residual.Count; j++)
                        {
                            Residual[j] -= Table.Selected_Indicators_Result[i] * Convert.ToInt32(Row2[j].Row_Values[Table.Selected_Indicator[i]]); //?
                        }
                    }
                }
            }
            count = Residual.Count;
            for (int i = 0; i < Residual.Count; i++)
            {
                if (i != Min_Selected_Indicators_Result)
                {
                    if (Table.Selected_Indicator[i] < 0)
                    {
                        Table.Selected_Indicators_Result[i] = Residual[i];
                    }
                }
            }
        }
        private Boolean Test_Simplex_Answer(Simplex_Table Table, List<int> Residual)
        {
            int Sum_Answers = 0;
            List<int> Row = Table.Selected_Indicators_Result;
            int count = Row.Count;
            for (int i = 0; i < count; i++)
            {
                if (Table.Selected_Indicator[i] >= 0)
                {
                    if (Row[i] < 0)
                    {
                        return false;
                    }
                    Sum_Answers += Row[i];
                }
            }
            if (Sum_Answers > Best_Answer_Data[0].Item1)
            {
                Reset_Best_Answer_Data();
                int resid = 0;
                for(int i = 0; i < Residual.Count; i++)
                {
                    resid += Residual[i];
                    Residual_Back[i] = Residual[i];
                }
                Best_Answer_Data[0] = new Tuple<int,int>(Sum_Answers, resid);
                for (int i = 0; i < count; i++)
                {
                    if (Table.Selected_Indicator[i] >= 0)
                    {
                        if (Row[i] > 0)
                        {
                            Best_Answer_Data[Table.Selected_Indicator[i] + 1] = new Tuple<int, int>(Table.Selected_Indicator[i] + 1, Row[i]);
                        }
                    }
                }
                Prepare_For_Data_Reading(Table, Residual, Sum_Answers);
                Continue_Calculation = true;
            }
            else if (Start_Sum_Process == true && Sum_Answers > 0)
            {
                int resid = 0;
                for (int i = 0; i < Residual.Count; i++)
                {
                    resid += Residual[i];
                    Residual_Back[i] = Residual[i];
                }
                Best_Answer_Data[0] = new Tuple<int, int>(Sum_Answers + Best_Answer_Data[0].Item1, resid);
                for (int i = 0; i < count; i++)
                {
                    if (Table.Selected_Indicator[i] >= 0)
                    {
                        if (Row[i] > 0)
                        {
                            Best_Answer_Data[Table.Selected_Indicator[i] + 1] = new Tuple<int, int>(Table.Selected_Indicator[i] + 1, Row[i] + Best_Answer_Data[Table.Selected_Indicator[i] + 1].Item2);
                        }
                    }
                }
                Prepare_For_Data_Reading(Table, Residual, Sum_Answers);
                Continue_Calculation = true;
            }
            return true;
        }
        private void Prepare_For_Data_Reading(Simplex_Table Table, List<int> Residual, int Sum_Answers)
        {
            Answer = "The best solution that the Simplex algorithm calculated: \n \n";
            List<int> Row = Table.Selected_Indicators_Result;
            int count = Row.Count;
            for (int i = 0; i < count; i++)
            {
                if (Table.Selected_Indicator[i] >= 0)
                {
                    if (Row[i] > 0)
                    {
                        Answer += "x" + (Table.Selected_Indicator[i] + 1) + " = " + Row[i] + "; ";
                    }  
                }
            }
            Answer += "\n";
            Answer += "3x1 + 5x2 + 8x3 + 10x4 + 18x5 = 3600" + "  The residual is " + Residual[0] + "\n";
            Answer += "7x1 + x2  + 9x3 + 11x4 + 10x5 = 5010" + "  The residual is " + Residual[1] + "\n";
            Answer += "9x1 + 3x2 + 2x3 +  8x4 +  0x5 = 3000" + "  The residual is " + Residual[2] + "\n";
            Answer += "Sum of the residuals: " + (Residual[0] + Residual[1] + Residual[2]) + "\n";
            Answer += "Sum of the unknows values: " + Sum_Answers + "\n";
            Answer += "\n";
        }
    }
    public class Data_Possibilitys
    {
        List<Table_Possibility> Tables = new List<Table_Possibility>();
        List<Value_Possibility> Values = new List<Value_Possibility>();
    }
    public class Table_Possibility
    {
        Simplex_Table Table = new Simplex_Table();
        List<int> Residual = new List<int>();
        List<Tuple<int, int>> Best_Answer = new List<Tuple<int, int>>();
    }
    public class Value_Possibility
    {
        int Selected_Indicator = 0;
        int Selected_Indicators_Result = 0;
    }
    public class Simplex_Table
    {
        public List<int> Selected_Indicator = new List<int> { }; //if == -1 it is a residual if > 0 then it is an indicator
        public List<Row> Rows = new List<Row> { };
        public List<int> Selected_Indicators_Result = new List<int> { };
    }
    public class Row
    {
        public List<double> Row_Values = new List<double> { };
    }
}
