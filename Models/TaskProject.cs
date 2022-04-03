namespace ProjectManagement.Models
{
    public class TaskProject
    {
        public int Id { get; set; }

        public string Title { get; set; } = null!;

        public string Content { get; set; } = null!;
        
        public int ProjectId { get; set; }  

        public DateTime TaskDate { get; set; }

        public bool IsFinished { get; set; } = false;

    }
}
