using Microsoft.AspNetCore.Mvc.Rendering;
using QBCA.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QBCA.ViewModels
{
    public class AssignTaskViewModel
    {
        public int TaskID { get; set; }

        // Tên tác vụ (dùng cả cho tạo và hiển thị)
        [Required(ErrorMessage = "Task title is required.")]
        [Display(Name = "Task Title")]
        public string TaskType { get; set; }

        [Required(ErrorMessage = "Please select an assignee.")]
        [Display(Name = "Assign To")]
        public int AssignedToID { get; set; }

        [Display(Name = "Assigned To")]
        public string AssignedToName { get; set; }

        [Display(Name = "Assigned By")]
        public int AssignedByID { get; set; }

        public string AssignedByName { get; set; }  // Optional, for Details view

        [Required(ErrorMessage = "Please select a status.")]
        [Display(Name = "Status")]
        public string Status { get; set; }

        [Required(ErrorMessage = "Please provide a deadline.")]
        [Display(Name = "Deadline")]
        [DataType(DataType.DateTime)]
        public DateTime Deadline { get; set; }

        [Display(Name = "Assigned Date")]
        [DataType(DataType.DateTime)]
        public DateTime AssignedAt { get; set; }

        // Dropdown chọn giảng viên
        public List<User> AllLecturers { get; set; } = new();
        public IEnumerable<SelectListItem> LecturerOptions =>
            AllLecturers.Select(l => new SelectListItem
            {
                Value = l.UserID.ToString(),
                Text = l.FullName
            });
    }
}
