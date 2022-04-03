namespace ProjectManagement.Models
{
    public class Notification
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public int TaskId { get; set; }

        public int ProjectId { get; set; }

        public DateTime DateNotification { get; set; }

    }
}
