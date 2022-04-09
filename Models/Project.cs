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
        public PriorityP ProjectPriority { get; set; }

        public Project()
        {
            Tasks = new HashSet<TaskProject>();
            Notifications = new HashSet<Notification>();
        }
        public void CreateNotification(Project project)
        {

            var now = DateTime.Now;

            if(project.DateEnd < now && project.Tasks.Any(t => t.IsFinished == false)) /*will get a notification if a project passed a deadline with any unfinished tasks.*/
            {
                if(!project.Notifications.Any(n => n.ProjectId == project.Id))
                {
                    string contentNotification = $"Project: {project.Name} has finished with {project.Tasks.Count(t => t.IsFinished == false)} pending tasks.";
                    Notification notification = new Notification(project, contentNotification);
                    project.Notifications.Add(notification);
                }
            }

            if((project.Tasks.Any(t => t.IsFinished == true)) || project.IsFinished == true) /*- gets a notification whenever a task or a project is completed.*/
            {
                if (project.Tasks.Any(t => t.IsFinished == true))
                {
                    List<TaskProject> taskFinished = project.Tasks.Where(t => t.IsFinished == true).ToList();
                    foreach (var task in taskFinished)
                    {
                        if(!project.Notifications.Any(n => n.TaskId == task.Id))
                        {
                            string contentNotification = $"Task: {task.Title} has been finished by: {task.UserName}";
                            Notification notification = new Notification(project, contentNotification, task);
                            project.Notifications.Add(notification);
                        }
                    }
                }

                if(project.IsFinished == true)
                {
                    if(!project.Notifications.Any(n => n.ProjectId == project.Id))
                    {
                        string contentNotification = $"Project: {project.Name} is complete with {project.Tasks.Count(t => t.IsFinished == true)} tasks done.";
                        Notification notification = new Notification(project, contentNotification);
                        project.Notifications.Add(notification);
                    }
                }
            }
        }
    }
    public enum PriorityP
    {
        High,
        Medium,
        Low
    }


}
