namespace LoanManagement.Infrastructures.Applications
{
    public interface UnitOfWork
    {
        Task CommitAsync();
    }
}
