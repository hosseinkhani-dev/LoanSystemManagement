using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoanManagement.Services.Loans.Contracts.DTOs
{
    public class GetTotalInterestAndPenaltyDto
    {
        public decimal TotalInterest { get; set; }
        public decimal TotalLatePenalty { get; set; }
    }
}
