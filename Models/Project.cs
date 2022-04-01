namespace ProjectManagement.Models
{
    public class Project
    {

        int Id { get; set; } 

        string Name { get; set; }

        int CompletedPercentage { get; set; }   
        
        float Budget { get; set; }  
        
        ICollection<Task> Tasks { get; set; }   

        public Project ()
        {

            Tasks = new List<Task> ();  

        }


    }
}
