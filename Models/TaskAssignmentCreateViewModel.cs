﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QBCA.Models
{
    public class TaskAssignmentCreateViewModel
    {
        public int AssignmentID { get; set; }

        [Required(ErrorMessage = "Assignee is required.")]
        [Display(Name = "Assign To")]
        public int AssignedTo { get; set; }

        [Required(ErrorMessage = "Task type is required.")]
        [Display(Name = "Task Type")]
        public string TaskType { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        public string Status { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Due Date")]
        public DateTime? DueDate { get; set; }

        // Trường này sẽ được gán ở controller dựa vào người đăng nhập
        public int AssignedBy { get; set; }

        // Danh sách hỗ trợ dropdown
        public List<User> AllUsers { get; set; } = new List<User>();

        public List<string> TaskTypeOptions { get; set; } = new List<string>
        {
            "Review", "CreateQuestion", "ApprovePlan"
        };

        public List<string> StatusOptions { get; set; } = new List<string>
        {
            "Pending", "In Progress", "Completed"

        };
    }
}
