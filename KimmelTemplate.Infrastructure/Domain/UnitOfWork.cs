using KimmelTemplate.Common.Exceptions;
using KimmelTemplate.Domain;
using KimmelTemplate.Infrastructure.DataModel.Context;

namespace KimmelTemplate.Infrastructure.Domain
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly TodosContext _context;

        public UnitOfWork(TodosContext context)
        {
            _context = context;
        }

        public async Task Save()
        {
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new DatabaseException(ex.Message, ex.InnerException);
            }
        }
    }
}
