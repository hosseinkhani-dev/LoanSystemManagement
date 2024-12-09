using LoanManagement.Persistance.EF;

namespace LoanManagement.Tests.Spec.Infrastructions;

[Collection(nameof(ConfigurationFixture))]
public class EFDatabaseFixture : DatabaseFixture
{
    readonly ConfigurationFixture _configuration;

    public EFDatabaseFixture(ConfigurationFixture configurationFixture)
    {
        _configuration = configurationFixture;
    }

    public EFDbContext CreateDataContext()
    {
        return new EFDbContext(
            _configuration.Value.LoanManagementConnectionString);
    }
}
