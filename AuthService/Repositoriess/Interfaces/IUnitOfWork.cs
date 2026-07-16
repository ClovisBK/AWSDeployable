namespace AuthService.Repositoriess.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        ICategoryRepository Categories { get; }
        IBookRepository Books { get; }
        ILoanRepository Loans { get; }
        IReservationRepository Reservations { get; }

        Task<int> CompleteAsync();
    }
}
