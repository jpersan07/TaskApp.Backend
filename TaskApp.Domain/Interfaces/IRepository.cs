namespace TaskApp.Domain.Interfaces;

public interface IRepository<T>
{
  public Task<IEnumerable<T>> GetAll();
  public Task<T?> GetById(int id);
  public Task Add(T entity);
  public Task Delete(int id);
  public Task Update(T entity);
}
