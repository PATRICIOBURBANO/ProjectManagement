namespace ProjectManagement.Models
{
    public class Project
    {

        public int Id { get; set; } 
        public string Name { get; set; }
        public string Content { get; set; }
        public int CompletedPercentage { get; set; }
        public float Budget { get; set; }
        public DateTime Created { get; set; }
        public ICollection<Task> Tasks { get; set; }   

        public Project()
        {

            Tasks = new List<Task> ();  

        }


    }
}
