using AuthService.Data;
using AuthService.Repositoriess.Interfaces;

namespace AuthService.Repositoriess.Implementations
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
     
        public ICategoryRepository Categories { get; private set; }
        public IBookRepository Books { get; private set; }
        public ILoanRepository Loans { get; private set; }
        public IReservationRepository Reservations { get; private set; }


        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            Categories = new CategoryRepository(context);
            Books = new BookRepository(context);
            Loans = new LoanRepository(context);
            Reservations = new ReservationRepository(context);
        }

        public async Task<int> CompleteAsync()
          => await _context.SaveChangesAsync();

        public void Dispose() => _context.Dispose();
        
          
        
    }
}
