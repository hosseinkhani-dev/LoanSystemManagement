using LoanManagement.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoanManagement.Tests.Tools
{
    public static class LoanTypeFactory
    {
        public static LoanType CreateLoanType()
        {
            return new LoanType
            {
                Name = Generator.GenerateString(),
                Amount = Generator.GenerateDecimal(),
                InterestRate = Generator.GenerateDecimal(),
                RepaymentPeriod = Generator.GenerateByte(),
                MonthlyRepayment = Generator.GenerateDecimal()
            };
        }
    }
}
