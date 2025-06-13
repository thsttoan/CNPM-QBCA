using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QBCA.Models
{
    public class ExamCreateViewModel
    {
        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Distribution is required")]
        public int DistributionID { get; set; }

        [Required(ErrorMessage = "Start Date is required")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End Date is required")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }

        [Required(ErrorMessage = "At least one question must be selected")]
        [MinLength(1, ErrorMessage = "You must select at least one question.")]
        public List<int> QuestionIDs { get; set; } = new List<int>();
    }
}
