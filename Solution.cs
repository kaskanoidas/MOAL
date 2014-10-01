using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Mixed Optimisation Algorithm TM Gludis 2014, Created by: Rolandas Rimkus
namespace Mixed_Optimisation_Algorithm_Library
{
    public class Solution
    {
        public List<int> Unknowns;
        public List<int> Residuals;
        public int Unknowns_Sum;
        public int Residuals_Sum;

        public Solution()
        {
            this.Residuals_Sum = 0;
            this.Unknowns_Sum = 0;
            this.Residuals = new List<int>() { };
            this.Unknowns = new List<int>() { 0, 0, 0, 0, 0 };
        }
        public Solution(List<int> _Residuals, int Residuals_Sum)
        {
            this.Unknowns_Sum = 0;
            this.Unknowns = new List<int>() { 0, 0, 0, 0, 0 };
            this.Residuals = new List<int>(_Residuals);
            this.Residuals_Sum = Residuals_Sum;
        }
    }
}
