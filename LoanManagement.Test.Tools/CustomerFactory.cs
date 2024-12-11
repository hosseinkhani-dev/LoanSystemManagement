using LoanManagement.Entities;

namespace LoanManagement.Tests.Tools
{
    public static class CustomerFactory
    {
        public static Customer CreateCustomer(
            int score = 0,
            bool active = false)
        {
            return new Customer()
            {
                FirstName = Generator.GenerateString(),
                LastName = Generator.GenerateString(),
                NationalCode = Generator.GenerateString(),
                PhoneNumber = Generator.GenerateString(),
                Score = score,
                IsActive = active,
            };
        }

        
    }
}
