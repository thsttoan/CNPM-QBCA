namespace QBCA.Models
{
    public class DifficultyLevel
    {
        public int DifficultyLevelID { get; set; }
        public int SubjectID { get; set; }
        public string LevelName { get; set; }

        public Subject Subject { get; set; }
        public ICollection<Question> Questions { get; set; }
    }
}