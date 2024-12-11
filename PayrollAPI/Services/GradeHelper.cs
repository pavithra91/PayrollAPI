namespace PayrollAPI.Services
{
    public static class GradeHelper
    {
        public static int GetGradeValue(string gradeCode)
        {
            switch (gradeCode)
            {
                // Executive Grades (A)
                case "A1": return 1;
                case "A2": return 2;
                case "A3": return 3;
                case "A4": return 4;
                case "A5": return 5;
                case "A6": return 6;
                case "A7": return 7;

                // B Grades
                case "B1": return 8;
                case "B2": return 9;
                case "B3": return 10;
                case "B4": return 11;

                // C Grades
                case "C1": return 12;
                case "C2": return 13;
                case "C3": return 14;
                case "C4": return 15;

                default: return int.MaxValue; // Return a high value for undefined grades
            }
        }

        public static List<string> GetEligibleGrades(string gradeCode)
        {
            int gradeValue = GetGradeValue(gradeCode);
            List<string> eligibleGrades = new List<string>();

            // Logic based on grade levels
            if (gradeCode == "A1") eligibleGrades.AddRange(new[] { "A1", "A2", "A3" });
            else if (gradeCode == "A2") eligibleGrades.AddRange(new[] { "A2", "A3", "A4" });
            else if (gradeCode == "A3") eligibleGrades.AddRange(new[] { "A3", "A4", "A5" });
            else if (gradeCode == "A4") eligibleGrades.AddRange(new[] { "A4", "A5", "A6" });
            else if (gradeCode == "A5") eligibleGrades.AddRange(new[] { "A5", "A6", "A7" });
            else if (gradeCode == "A6") eligibleGrades.AddRange(new[] { "A6", "A7", "B1" });
            else if (gradeCode == "A7") eligibleGrades.AddRange(new[] { "A7", "B1", "B2" });
            else if (gradeCode == "B1") eligibleGrades.AddRange(new[] { "B1", "B2", "B3" });
            else if (gradeCode == "B2") eligibleGrades.AddRange(new[] { "B2", "B3", "B4" });
            else if (gradeCode == "B3") eligibleGrades.AddRange(new[] { "B3", "B4", "C1" });
            else if (gradeCode == "B4") eligibleGrades.AddRange(new[] { "B4", "C1" });
            else if (gradeCode == "C1") eligibleGrades.AddRange(new[] { "C1", "C2", "C3" });
            else if (gradeCode == "C2") eligibleGrades.AddRange(new[] { "C2", "C3", "C4" });
            else if (gradeCode == "C3") eligibleGrades.AddRange(new[] { "C3", "C4" });
            else if (gradeCode == "C4") eligibleGrades.AddRange(new[] { "C4" });

            return eligibleGrades;
        }
    }
}
