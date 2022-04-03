namespace ProjectManagement.Models
{
    public class Project
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public int CompletedPercentage { get; set; }

        public float Budget { get; set; }

        public DateTime DateBegin = DateTime.Now;

        public DateTime DateEnd = DateTime.Now;

        public bool IsFinished { get; set; } = false;

        public ICollection<Task> Tasks { get; set; }

        public ICollection<Notification> Notifications { get; set; }

        public ICollection<Project> Projects { get; set; }


        public Project()
        {

            Tasks = new List<Task>();
            Notifications = new List<Notification>();
            Projects = new List<Project>();

        }


    }
}
