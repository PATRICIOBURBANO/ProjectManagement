namespace ProjectManagement.Models
{
    public class TaskProject
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;
        public int? ProjectId { get; set; }
        public virtual Project Project { get; set; }
        public string? UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
        public DateTime DateBegin { get; set; }
        public DateTime? DateEnd { get; set; }
        public bool IsFinished { get; set; } = false;
        public int CompletedPercentage { get; set; }

        public Priority TaskPriority { get; set; }

    
    }
    public enum Priority
    {
        High,
        Medium,
        Low
    }
}
