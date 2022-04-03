namespace ProjectManagement.Models
{
    public class Project
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string Content { get; set; } = null!;

        public int CompletedPercentage { get; set; }

        public float Budget { get; set; }

        public DateTime DateBegin = DateTime.Now;

        public DateTime DateEnd = DateTime.Now;

        public ApplicationUser? User { get; set; }

        public bool IsFinished { get; set; } = false;

        public ICollection<TaskProject> Tasks { get; set; }

        public ICollection<Notification> Notifications { get; set; }

        public ICollection<Project> Projects { get; set; }


        public Project()
        {

            Tasks = new List<TaskProject>();
            Notifications = new List<Notification>();
            Projects = new List<Project>();

        }


    }
}
