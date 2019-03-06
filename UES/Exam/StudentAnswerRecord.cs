using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using HIT.UES.Login;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HIT.UES.Exam
{
    public class StudentAnswerRecord : DatabaseType
    {
        public static string ScoreBelowZero = "The score you are giving is below zero. Think about it twice.";
        public static string ScoreAboveMax(int maxScore, double score)
            => $"The score {score} you are giving is above the maximum score {maxScore}.";

        public int StudentAnswerRecordID { get; private set; }
        public ExamPaperInstance SuperiorExamPaperInstance { get; private set; }
        public int Order { get; private set; }
        public Student Candidate { get; private set; }
        public int MaxScore { get; private set; }
        public double? Score { get; private set; }
        public bool Answered { get; private set; }
        public bool Checked { get; private set; }
        public ExamQuestion SuperiorQuestion { get; }
        public string Answer { get; private set; }

        public StudentAnswerRecord()
        {
            Score = null;
            Answered = false;
            Checked = false;
        }
        public StudentAnswerRecord(ExamPaperInstance instance, QuestionChooseRecord record, Student student)
        {
            SuperiorExamPaperInstance = instance;
            Order = record.QuestionOrder;
            Candidate = student;
            MaxScore = record.MaxScore;
            Score = null;
            Answered = false;
            Checked = false;
        }
        
        public void StoreAnswer(Student student, string answer, out string errorMessage)
        {
            if (Candidate != student)
            {
                errorMessage = ExamPaperInstance.NotCandidate;
            }
            else if (!SuperiorExamPaperInstance.Ongoing)
            {
                errorMessage = ExamPaperInstance.ExamFinished;
            }
            else
            {
                Answer = answer;
                Answered = true;
                Settings.SaveDataModification(this);
                errorMessage = null;
            }
        }

        public void GiveScore(Teacher teacher, double score, out string errorMessage)
        {
            if (!SuperiorExamPaperInstance.SuperiorExamPaper.SuperiorExam.HasExamineAuthority(teacher))
            {
                errorMessage = Exam.NoExamineAuthority;
            }
            else if (!SuperiorExamPaperInstance.StudentSubmitted)
                errorMessage = "The exam is not finished and you cannot give a score.";
            else if (score > MaxScore)
            {
                errorMessage = ScoreAboveMax(MaxScore, score);
            }
            else if (score < 0)
            {
                errorMessage = ScoreBelowZero;
            }
            else
            {
                Score = score;
                Checked = true;
                Settings.SaveDataModification(this);
                errorMessage = null;
            }
        }
        public void GiveMaxScore(Teacher teacher, out string errorMessage)
            => GiveScore(teacher, MaxScore, out errorMessage);
        internal void GiveScore(AutoCheckRule rule, double score)
        {
            if (!SuperiorExamPaperInstance.StudentSubmitted)
                return;
            Score = score;
            Checked = true;
            Settings.SaveDataModification(this);
        }
        internal void GiveMaxScore(AutoCheckRule rule) => GiveScore(rule, MaxScore);

        public override string CastObjectToJson()
            => JsonConvert.SerializeObject(this, new JsonSerializerSettings
            { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
        public override XmlDocument CastObjectToXml()
            => JsonConvert.DeserializeXmlNode(CastObjectToJson());
    }
}
