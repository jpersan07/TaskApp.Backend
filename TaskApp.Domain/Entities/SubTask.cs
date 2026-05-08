namespace TaskApp.Domain.Entities;

public class SubTask
{
  public int Id { get; set; }
  public string Title { get; set; } = string.Empty;
  public int TaskId { get; set; }
  public AppTask Task { get; set; } = null!;
  public bool IsCompleted { get; set; } = false;
}