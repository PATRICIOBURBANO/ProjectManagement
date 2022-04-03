namespace ProjectManagement.Models
{
    public class Task
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public int ProjectId { get; set; }

        public DateTime TaskDate { get; set; }

        public bool IsFinished { get; set; } = false;

    }
}
