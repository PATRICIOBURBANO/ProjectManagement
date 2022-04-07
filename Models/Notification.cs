namespace ProjectManagement.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public bool Status { get; set; } /*Determine if the notification is new or opened*/
        public int? TaskId { get; set; }
        public string? UserName { get; set; }   
        public virtual TaskProject Task { get; set; }
        public int? ProjectId { get; set; }
        public virtual Project Project { get; set; }
        public DateTime DateNotification { get; set; }
    }
}
