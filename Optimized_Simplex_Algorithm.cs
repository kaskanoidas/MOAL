using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Mixed Optimisation Algorithm TM Gludis 2014, Created by: Rolandas Rimkus
namespace Mixed_Optimisation_Algorithm_Library
{
    // NOTE: try using tail_Recursion where it is implemented correctly. Otherwise to avoid stack-overflow use loop like below.
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
        Data_Possibilitys Data;
        Task Global_Task;

        public string Optimized_Simplex_Algorithm_Start(Task task)
        {
            Global_Task = task;
            Simplex_Deep_Cycle();
            return Return_Optimized_Simplex_Algorithm();
        }
        private string Return_Optimized_Simplex_Algorithm()
        {
            return Answer;
        }
        private List<Tuple<int, int>> Reset_Best_Answer_Data()
        {
            // 0 index == Sum value and residual, other's unknown and value
            List<Tuple<int, int>> newAnswer = new List<Tuple<int, int>>() { };
            newAnswer.Add(new Tuple<int, int>(0, Global_Task.Rezults.Sum()));
            for (int i = 0; i < Global_Task.Unknown_Multipliers[0].Count; i++)
            {
                newAnswer.Add(new Tuple<int, int>(i+1, 0));
            }
            return newAnswer;
        }
        private void Simplex_Deep_Cycle()
        {
            Best_Answer_Data = Reset_Best_Answer_Data();
            Residual_Back = new List<int>(Global_Task.Rezults); // hardcoded results
            Start_Sum_Process = false;
            Continue_Calculation = true;
            while (Continue_Calculation == true) // FIX HERE LOOP MAKES VALUES NEGATIVE
            {
                Data = new Data_Possibilitys();
                Make_Simplex_Tabel();
                Continue_Calculation = false;
                Simplex_Cycle(Table);
                Start_Sum_Process = true;
            }
        }
        private void Simplex_Cycle(Simplex_Table _Table)
        {
            Table_Possibility start = new Table_Possibility();
            start.Table = Make_Simplex_Tabel_Copy(_Table);
            start.Residual.AddRange(Residual_Back);
            Data.Tables.Add(start);
            Data.Values.Add(new Value_Possibility());
            while (Data.Tables.Count != 0)
            {
                int count = Data.Tables.Count;
                for (int i = 0; i < count; i++)
                {
                    Find_Max_Index(Data.Tables[i].Table, Data.Tables[i].Residual);
                    Data.Tables.RemoveAt(i);
                    Data.Values.RemoveAt(i);
                    i--; count--;
                }
                count = Data.Tables.Count;
                for (int i = 0; i < count; i++)
                {
                    Find_Min_Selected_Indicators_Result(Data.Tables[i].Table, Data.Tables[i].Residual, Data.Values[i].Max_Index);
                    Data.Tables.RemoveAt(i);
                    Data.Values.RemoveAt(i);
                    i--; count--;
                }
                count = Data.Tables.Count;
                for (int i = 0; i < count; i++)
                {
                    if (Simplex_Cycle_Step(ref Data.Tables[i].Table, ref Data.Tables[i].Residual, i) == false)
                    {
                        Data.Tables.RemoveAt(i);
                        Data.Values.RemoveAt(i);
                        i--; count--;
                    }
                }
            }
        }
        private void Find_Max_Index(Simplex_Table _Table, List<int> _Residual)
        {
            int Row_Count = _Table.Rows.Count - 1;
            List<double> Row = _Table.Rows[Row_Count].Row_Values;
            int count = Row.Count;
            double max = -1;
            int mx = -1;
            for (int i = 0; i < count; i++)
            {
                if (max < Row[i])
                {
                    max = Row[i];
                    mx = i;
                }
            }
            if (mx != -1)
            {
                for (int i = 0; i < count; i++)
                {
                    if (max == Row[i])
                    {
                        Value_Possibility VP = new Value_Possibility();
                        VP.Max_Index = i;
                        VP.Min_Selected_Indicators_Result = -1;
                        VP.Min_Selected_Indicator = -1;
                        Data.Values.Add(VP);
                        Table_Possibility TP = new Table_Possibility();
                        TP.Table = Make_Simplex_Tabel_Copy(_Table);
                        TP.Residual.AddRange(_Residual);
                        Data.Tables.Add(TP);
                    }
                }
            }
        }
        private void Find_Min_Selected_Indicators_Result(Simplex_Table _Table, List<int> _Residual, int Max_Index)
        {
            double min = -1;
            int mn = -1;
            List<Row> Row_List = _Table.Rows;
            int count = Row_List.Count - 1;
            for (int i = 0; i < count; i++)
            {
                double value = _Table.Selected_Indicators_Result[i] / Row_List[i].Row_Values[Max_Index];
                if (value >= 0)
                {
                    min = value;
                    mn = i;
                    i = count;
                }
            }
            if (mn != -1)
            {
                for (int i = 0; i < count; i++)
                {
                    if (Row_List[i].Row_Values[Max_Index] >= 0)
                    {
                        double tikrinti = _Table.Selected_Indicators_Result[i] / Row_List[i].Row_Values[Max_Index];
                        if (tikrinti < min && tikrinti > 0)
                        {
                            min = tikrinti;
                            mn = i;
                        }
                    }
                }
                for (int i = 0; i < count; i++)
                {
                    if (Row_List[i].Row_Values[Max_Index] > 0)
                    {
                        double tikrinti = _Table.Selected_Indicators_Result[i] / Row_List[i].Row_Values[Max_Index];
                        if (tikrinti == min)
                        {
                            Value_Possibility newValue = new Value_Possibility();
                            newValue.Max_Index = Max_Index;
                            newValue.Min_Selected_Indicator = i;
                            newValue.Min_Selected_Indicators_Result = min;
                            Data.Values.Add(newValue);
                            Table_Possibility TP = new Table_Possibility();
                            TP.Table = Make_Simplex_Tabel_Copy(_Table);
                            TP.Residual.AddRange(_Residual);
                            Data.Tables.Add(TP);
                        }
                    }
                }
            }
            else
            {

            }
        }
        private Boolean Simplex_Cycle_Step(ref Simplex_Table _Table, ref List<int> Residual, int Index)
        {
            Boolean stop = false;
            if (Check_Symplex_Rules(_Table) == true)
            {
                Residual = new List<int> { };
                int Max_Index = Data.Values[Index].Max_Index;
                int Min_Selected_Indicator = Data.Values[Index].Min_Selected_Indicator;
                int Min_Selected_Indicators_Result = Convert.ToInt32(Math.Floor(Data.Values[Index].Min_Selected_Indicators_Result));
                if (Min_Selected_Indicator == -1 && Min_Selected_Indicators_Result == -1)
                {
                    return false;
                }
                else
                {
                    _Table.Selected_Indicator[Min_Selected_Indicator] = Max_Index;
                    Transform_Main_Row(Min_Selected_Indicator, Max_Index, ref _Table);
                    Transform_Other_Rows(Min_Selected_Indicator, Max_Index, ref _Table);
                    Transform_Main_Collumn(Min_Selected_Indicator, Max_Index, ref _Table);
                    Recount_Selected_Indicators_Result(Min_Selected_Indicator, Max_Index, Min_Selected_Indicators_Result, ref _Table, ref Residual);
                    stop = Test_Simplex_Answer(_Table, Residual);
                    if (stop == false)
                    {
                        return false;
                    }
                    return true;
                }
            }
            return false;
        }
        private void Make_Simplex_Tabel()
        {
            Table = new Simplex_Table(); Table_Back = new Simplex_Table();         // Back = hard-copy of table
            Row row; Row row_Back;

            for (int i = 0; i < Global_Task.Rezults.Count; i++)
            {
                Table.Selected_Indicator.Add(-1);
                Table_Back.Selected_Indicator.Add(-1);
            }
            Table.Selected_Indicators_Result.AddRange(Residual_Back);
            Table_Back.Selected_Indicators_Result.AddRange(Residual_Back);

            for (int i = 0; i < Global_Task.Unknown_Multipliers.Count; i++)
            {
                row = new Row(); row_Back = new Row();
                for (int j = 0; j < Global_Task.Unknown_Multipliers[i].Count; j++)
                {
                    row.Row_Values.Add(Global_Task.Unknown_Multipliers[i][j]);
                    row_Back.Row_Values.Add(Global_Task.Unknown_Multipliers[i][j]);
                }
                Table.Rows.Add(row); Table_Back.Rows.Add(row_Back);
            }

            row = new Row(); row_Back = new Row();
            for (int j = 0; j < Global_Task.Unknown_Multipliers[0].Count; j++)
            {
                row.Row_Values.Add(1);
                row_Back.Row_Values.Add(1);
            }
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
                Best_Answer_Data = Reset_Best_Answer_Data();
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
            for (int i = 0; i < Global_Task.Rezults.Count; i++)
            {
                for (int j = 0; j < Global_Task.Unknown_Multipliers[i].Count; j++) 
                {
                    if (j != Global_Task.Unknown_Multipliers[i].Count - 1)
                    {
                        Answer += Global_Task.Unknown_Multipliers[i][j] + "x" + (j+1) + " + ";
                    }
                    else
                    {
                        Answer += Global_Task.Unknown_Multipliers[i][j] + "x" + (j+1) + " = ";
                    }
                }
                Answer += Global_Task.Rezults[i] + "  The residual is " + Residual[i] + "\n";
            }
            Answer += "Sum of the residuals: " + (Residual.Sum()) + "\n";
            Answer += "Sum of the unknows values: " + Sum_Answers + "\n";
            Answer += "\n";
        }
    }
    public class Data_Possibilitys
    {
        public List<Table_Possibility> Tables = new List<Table_Possibility>();
        public List<Value_Possibility> Values = new List<Value_Possibility>();
    }
    public class Table_Possibility
    {
        public Simplex_Table Table = new Simplex_Table();
        public List<int> Residual = new List<int>();
    }
    public class Value_Possibility
    {
        public int Max_Index = 0;
        public int Min_Selected_Indicator = 0;
        public double Min_Selected_Indicators_Result = 0;
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
