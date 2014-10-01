using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Mixed Optimisation Algorithm TM Gludis 2014, Created by: Rolandas Rimkus
namespace Mixed_Optimisation_Algorithm_Library
{
    // Equation system we used (hardcoded):
    // 3x1 + 5x2 + 8x3 + 10x4 + 18x5 = 3600
    // 7x1 + x2  + 9x3 + 11x4 + 10x5 = 5010
    // 9x1 + 3x2 + 2x3 +  8x4 +  0x5 = 3000
    public class Task
    {
        public List<int> Rezults;
        public List<List<int>> Unknown_Multipliers;
        public Task()
        {
            this.Rezults = new List<int>() { 3600, 5010, 3000 };
            this.Unknown_Multipliers = new List<List<int>>() 
            {
                new List<int>(){3,5,8,10,18},
                new List<int>(){7,1,9,11,10},
                new List<int>(){9,3,2,8,0},
            };
        }
        public Task(List<int> _Rezults, List<List<int>> _Unknown_Multipliers)
        {
            this.Rezults = new List<int>(_Rezults);
            this.Unknown_Multipliers = new List<List<int>>(_Unknown_Multipliers);
        }
    }
}
