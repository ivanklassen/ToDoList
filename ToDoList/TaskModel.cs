using System.ComponentModel.DataAnnotations;

namespace ToDoList
{
    public class TaskModel
    {
        [Required]
        [MinLength(4)]
        public string? Title { get; set; }

        public string? Description { get; set; }

        public DateTimeOffset? Deadline { get; set; }

        public Priority? Priority { get; set; }
    }
}
