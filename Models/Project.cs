namespace ProjectManagement.Models
{
    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Content { get; set; } = null!;
        public int CompletedPercentage { get; set; }
        public float Budget { get; set; }
        public DateTime DateBegin { get; set; }
        public DateTime DateEnd { get; set; }
        public string? UserId { get; set; }
        public string? UserName { get; set; }
        public virtual ApplicationUser User { get; set; }
        public bool IsFinished { get; set; } = false;
        public virtual ICollection<TaskProject> Tasks { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
        public virtual ICollection<Project> Projects { get; set; }
        public PriorityP ProjectPriority { get; set; }

        public Project()
        {
            Tasks = new HashSet<TaskProject>();
            Notifications = new HashSet<Notification>();
            Projects = new HashSet<Project>();
        }

    }
    public enum PriorityP
    {
        High,
        Medium,
        Low
    }

}
