using Bogus;

namespace LoanManagement.Tests.Tools
{
    public static class Generator
    {
       private static Faker faker = new Faker();

        public static decimal GenerateDecimalNumber()
        {
            return faker.Random.Decimal();
        }

        public static string GenerateString()
        {
            return faker.Random.String(10);
        }
    }
}
