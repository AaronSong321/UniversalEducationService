using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using HIT.UES.Login;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HIT.UES.Exam
{
    public class ExamPaperInstance : DatabaseType
    {
        #region Static Error Messages
        public static string NotCandidate = "You are not the candidate of this exam paper.";
        public static string ExamFinished = "Your exam has finished. You cannot answer the questions now.";
        public static string ExamClosed = "The exam has closed. You cannot answer the questions now.";
        public static string StudentDidnotSubmit = "The student has not yet submitted the exam paper instance.";
        public static string TeacherAlreadySubmit = "The answer has been submitted and you need not duplicate the operation.";
        public static string NotOngoing = "The exam has not started yet.";
        public static string NoTotalScore = "There is no total score.";
        #endregion

        #region Members and Properties
        public int ExamPaperInstanceID { get; private set; }
        public ExamPaper SuperiorExamPaper { get; private set; }
        public Student Candidate { get; private set; }
        public double ExamDuration { get; private set; }
        public double CountdownTimeLeft { get; private set; }
        public bool Ongoing { get; private set; }
        public DateTime? ExamStartTime { get; private set; }
        public ushort MaxScore { get; private set; }
        public double? StudentScore { get; private set; }
        public bool StudentSubmitted { get; private set; }
        public bool TeacherSubmitted { get; private set; }
        public Teacher Examiner { get; private set; }
        public DateTime? AnswerCheckTime { get; private set; }
        public List<StudentAnswerRecord> Answers { get; }
        //public AutoAnswerCheckRule AnswerCheckRule { get; private set; }
        private Thread CountdownTimeThread { get; set; }
        #endregion

        //Should use bulk insert here
        #region Creation and Basic Information
        public ExamPaperInstance()
        {
            CountdownTimeThread = null;
        }
        public ExamPaperInstance(ExamPaper superior, Student candidate)
        {
            SuperiorExamPaper = superior;
            Candidate = candidate;
            ExamDuration = superior.ExamDuration;
            CountdownTimeLeft = ExamDuration;
            Ongoing = false;
            ExamStartTime = null;
            MaxScore = superior.MaxScore;
            StudentScore = null;
            StudentSubmitted = false;
            TeacherSubmitted = false;
            Examiner = null;
            AnswerCheckTime = null;
            Answers = new List<StudentAnswerRecord>();

            foreach (var record in superior.QuestionSet)
            {
                var m = new StudentAnswerRecord(this, record, candidate);
                Answers.Add(m);
                Settings.SaveDataCreation(m);
            }

            CountdownTimeThread = null;
        }
        #endregion

        public List<QuestionChooseRecord> GetAllQuestions() => SuperiorExamPaper.QuestionSet;


        #region Start an Exam
        internal (List<QuestionChooseRecord>, List<StudentAnswerRecord>, double) StartExamPaperInstance()
        {
            Ongoing = true;
            ExamStartTime = DateTime.Now;
            Settings.SaveDataModification(this);
            CountdownTimeThread = new Thread(CountTimeLeft);
            CountdownTimeThread.Start();
            return (GetAllQuestions(), Answers, CountdownTimeLeft);
        }
        public (List<QuestionChooseRecord>, List<StudentAnswerRecord>, double, string) StartExamPaperInstance(Student student)
        {
            if (student != Candidate)
                return (null, null, 0, NotCandidate);
            if (SuperiorExamPaper.SuperiorExam.StudentSubmitDeadline > DateTime.Now)
                return (null, null, 0, ExamClosed);
            if (StudentSubmitted)
                return (null, null, 0, ExamFinished);
            var (a, b, c) = StartExamPaperInstance();
            return (a, b, c, null);
        }
        public static double CountTimeStep = 10;
        internal void CountTimeLeft()
        {
            while (CountdownTimeLeft > 0.01)
            {
                double timeToWait = Math.Min(CountdownTimeLeft, CountTimeStep);
                Thread.Sleep((int)(timeToWait * 1000));
                CountdownTimeLeft -= timeToWait;
                Settings.SaveDataModification(this);
            }
            if (!StudentSubmitted)
                StudentSubmit();
        }
        /// <summary>
        /// Used in case of internet failure
        /// </summary>
        /// <returns></returns>
        internal (List<QuestionChooseRecord>, List<StudentAnswerRecord>, double) GetCurrentOngoingExamState()
        {
            return (GetAllQuestions(), Answers, CountdownTimeLeft);
        }
        public (List<QuestionChooseRecord>, List<StudentAnswerRecord>, double) GetCurrentOngoingExamState(Student student,
            out string errorMessage)
        {
            if (student != Candidate)
                errorMessage = NotCandidate;
            else if (SuperiorExamPaper.SuperiorExam.StudentSubmitDeadline > DateTime.Now)
                errorMessage = ExamClosed;
            else if (StudentSubmitted)
                errorMessage = ExamFinished;
            else if (!Ongoing)
                errorMessage = NotOngoing;
            else
            {
                errorMessage = null;
                return GetCurrentOngoingExamState();
            }
            return (null, null, 0);
        }
        #endregion

        #region Answer Questions and Submit Instance
        public List<StudentAnswerRecord> CheckQuestionAnswered(Student student, out string errorMessage)
        {
            if (student != Candidate)
                errorMessage = NotCandidate;
            else if (SuperiorExamPaper.SuperiorExam.StudentSubmitDeadline > DateTime.Now)
                errorMessage = ExamClosed;
            else if (StudentSubmitted)
                errorMessage = ExamFinished;
            else if (!Ongoing)
                errorMessage = NotOngoing;
            else
            {
                errorMessage = null;
                return (from b in Answers where b.Answered == false select b).ToList();
            }
            return null;
        }

        internal void StudentSubmit()
        {
            Ongoing = false;
            StudentSubmitted = true;
            if (CountdownTimeThread.ThreadState == ThreadState.Running)
            {
                CountdownTimeThread.Abort();
                CountdownTimeLeft = 0;
            }
            Settings.SaveDataModification(this);
        }
        public void StudentSubmit(Student student, out string errorMessage)
        {
            if (student != Candidate)
                errorMessage = NotCandidate;
            else if (SuperiorExamPaper.SuperiorExam.StudentSubmitDeadline > DateTime.Now)
                errorMessage = ExamClosed;
            else if (StudentSubmitted)
                errorMessage = ExamFinished;
            else if (!Ongoing)
                errorMessage = NotOngoing;
            else
            {
                StudentSubmit();
                errorMessage = null;
            }
        }
        #endregion

        #region Teacher
        internal void TeacherSubmit()
        {
            TeacherSubmitted = true;
            Settings.SaveDataModification(this);
        }
        public void TeacherSubmit(Teacher teacher, out string errorMessage)
        {
            if (!SuperiorExamPaper.SuperiorExam.HasExamineAuthority(teacher))
                errorMessage = Exam.NoExamineAuthority;
            else if (!StudentSubmitted)
                errorMessage = StudentDidnotSubmit;
            else if (TeacherSubmitted)
                errorMessage = TeacherAlreadySubmit;
            else if (StudentScore == null)
                errorMessage = NoTotalScore;
            else
            {
                TeacherSubmit();
                errorMessage = null;

            }
        }
        #endregion
        #region Score and Document
        public double? CalculateTotalScore(out string errorMessage)
        {
            if (!StudentSubmitted)
            {
                errorMessage = "The exam has not been finished yet.";
                return null;
            }
            else
            {
                double score = 0;
                foreach (var sar in Answers)
                {
                    if (sar.Score == null)
                    {
                        errorMessage = $"Question {sar.Order} has not been given a score yet.";
                        return null;
                    }
                    else
                        score += sar.Score.Value;
                }
                errorMessage = null;
                StudentScore = score;
                Settings.SaveDataModification(this);
                return score;
            }
        }
        public void GeneratePDF(out string errorMessage)
        {
            if (!StudentSubmitted)
            {
                errorMessage = StudentDidnotSubmit;
            }
            else
            {
                throw new NotImplementedException();
            }
        }
        #endregion

        public void AutoSave()
        {
            throw new NotImplementedException("");
        }

        #region Override and Implemented Members
        public override string CastObjectToJson()
            => JsonConvert.SerializeObject(this, new JsonSerializerSettings
            { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
        public override XmlDocument CastObjectToXml()
            => JsonConvert.DeserializeXmlNode(CastObjectToJson());

        public override int GetHashCode()
            => ExamPaperInstanceID;
        public override bool Equals(object obj)
        {
            if (obj is ExamPaperInstance instance)
                return ExamPaperInstanceID == instance.ExamPaperInstanceID;
            else
                return false;
        }
        #endregion
    }
}
