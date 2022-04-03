namespace ProjectManagement.Models
{
    public class Task
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime Created { get; set; }
        public string Content { get; set; }
        public Project ProjectId { get; set; }
    }
}
