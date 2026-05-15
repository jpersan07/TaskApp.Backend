using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using TaskApp.Domain.Interfaces;

namespace TaskApp.Infrastructure.Repositories;
/// <summary>
/// A class to add, remove or get that implements the inferfaces from TaskAp.Domain/Interfaces, we use it with 
/// TaskApp(Tasks), Users, SubTask, Categories and Tag
/// </summary>
/// <typeparam name="T">The type of class we are going to get info, add or remove</typeparam>
public class Repository<T> : IRepository<T> where T : class
{
  /// <summary>
  /// The DbContext
  /// </summary>
  protected AppDbContext _context;
  /// <summary>
  /// A method to add to the DB a new TaskApp(Tasks), Users, SubTask, Categories or Tag
  /// </summary>
  /// <param name="entity">The type of data we are going to work with</param>
  /// <returns></returns>
  public async Task Add(T entity)
  {
    try
    {
      await _context.Set<T>().AddAsync(entity);
      await _context.SaveChangesAsync();
    }
    catch (DbUpdateException ex)
    {
      throw new Exception($"Error adding entity '{entity}' to database", ex);
    }
  }
  /// <summary>
  /// A method to remove from the DB a TaskApp(Tasks), Users, SubTask, Categories or Tag
  /// </summary>
  /// <param name="id">The id of the type of entity that is going to deal the method with</param>
  /// <returns></returns>
  public async Task Delete(int id)
  {
    var entity = await _context.Set<T>().FindAsync(id);

    if (entity != null)
    {
      try
      {
        _context.Set<T>().Remove(entity!);
        await _context.SaveChangesAsync();
      }
      catch (DbUpdateException ex)
      {
        throw new Exception($"Error deleting entity '{id}' to database", ex);
      }
    }
  }
  /// <summary>
  /// A method to GetAll entities of TaskApp(Tasks), Users, SubTask, Categories or Tag
  /// </summary>
  /// <returns></returns>
  public async Task<IEnumerable<T>> GetAll()
  {
    try
    {
      return await _context.Set<T>().ToListAsync();
    }
    catch (DbException ex)
    {
      throw new Exception("Error getting entities from database", ex);
    }
  }
  /// <summary>
  /// A method to get a TaskApp(Tasks), Users, SubTask, Categories or Tag by its ID
  /// </summary>
  /// <param name="id">The id of the entity you are searching for</param>
  /// <returns></returns>
  public async Task<T?> GetById(int id)
  {
    try
    {
      return await _context.Set<T>().FindAsync(id);
    }
    catch (DbException ex)
    {
      throw new Exception($"Error getting entity '{id}' from database", ex);
    }
  }
  /// <summary>
  /// A method to update a Task
  /// </summary>
  /// <param name="entity">The type of data we are going to work with</param>
  /// <returns></returns>
  public async Task Update(T entity)
  {
    try
    {
      _context.Set<T>().Update(entity);
      await _context.SaveChangesAsync();
    }
    catch (DbUpdateException ex)
    {
      throw new Exception($"Error updating entity '{entity}' to database", ex);
    }
  }

  /// <summary>
  /// The starter of the DB
  /// </summary>
  /// <param name="context">The DbContext object</param>
  public Repository(AppDbContext context)
  {
    _context = context;
  }
}
