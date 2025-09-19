using CNPM_QBCA.Models;
using QBCA.Models;

namespace CNPM_QBCA.Services
{
    public class TaskService
    {
        private static List<TaskModel> tasks = new List<TaskModel>
        {
        
        };
        private int userId;

        public List<TaskModel> GetTasksForUser(string user)
        {
            return tasks.Where(t => t.AssignedTo == userId).ToList();
        }

        public TaskModel GetTaskById(int id)
        {
            return tasks.FirstOrDefault(t => t.TaskModelId == id);
        }

        public void UpdateTaskStatus(int id, string status)
        {
            var task = tasks.FirstOrDefault(t => t.TaskModelId == id);
            if (task != null) task.Status = status;
        }
    }
}
