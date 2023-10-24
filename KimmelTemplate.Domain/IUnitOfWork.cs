namespace KimmelTemplate.Domain
{
    public interface IUnitOfWork
    {
        Task Save();
    }
}
