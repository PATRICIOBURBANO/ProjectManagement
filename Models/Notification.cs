namespace ProjectManagement.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public bool Status { get; set; } = false; /*Determine if the notification is new or opened*/
        public bool isImportant { get; set; } = false; /*Determine if it is important or not*/
        public int? TaskId { get; set; }
        public virtual TaskProject Task { get; set; }
        public string? UserId { get; set; }   
        public virtual ApplicationUser User { get; set; }
        public int? ProjectId { get; set; }
        public virtual Project Project { get; set; }
        public DateTime DateNotification { get; set; }

        public Notification(Project project, string contentNotification)
        {
            Content = contentNotification;
            User = project.User;
            UserId = project.UserId;
            Project = project;
            ProjectId = project.Id;
            DateNotification = DateTime.Now;
        }

        public Notification(Project project, string contentNotification, TaskProject task)
        {
            Content = contentNotification;
            User = project.User;
            UserId = project.UserId;
            Task = task;
            TaskId = task.Id;
            Project = project;
            ProjectId = project.Id;
            DateNotification = DateTime.Now;
        }
    }
}
