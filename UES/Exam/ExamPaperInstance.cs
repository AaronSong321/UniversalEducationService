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
        public static string NotCandidate = "You are not the candidate of this exam paper.";
        public static string ExamFinished = "Your exam has finished. You cannot answer the questions now.";
        public static string ExamClosed = "The exam has closed. You cannot answer the questions now.";
        public static string StudentDidnotSubmit = "The student has not yet submitted the exam paper instance.";
        public static string TeacherAlreadySubmit = "The answer has been submitted and you need not duplicate the operation.";
        public static string NotOngoing = "The exam has not started yet.";

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
        //Generate dummy exam paper instances for tests.
        //Under construction

        public List<QuestionChooseRecord> GetAllQuestions() => SuperiorExamPaper.QuestionSet;

        /*
        public string AnswerQuestion(QuestionChooseRecord record, string answer, Student student)
        {
            ushort order = record.QuestionOrder;
            return Answers[order].StoreAnswer(student, answer);
        }
        //Don't know what is supposed to do here
        //Invoke AnswerQuestion once for all questions? Then the parameters?
        public void SaveAnswers()
        {
            throw new NotImplementedException("I don't know what to do here.");
        }
        */

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
        internal void TeacherSubmit()
        {
            TeacherSubmitted = true;
            Settings.SaveDataModification(this);
        }
        public string TeacherSubmit(Teacher teacher)
        {
            if (!SuperiorExamPaper.SuperiorExam.HasExamineAuthority(teacher))
                return Exam.NoExamineAuthority;
            if (!StudentSubmitted)
                return StudentDidnotSubmit;
            if (TeacherSubmitted)
                return TeacherAlreadySubmit;
            TeacherSubmit();
            return null;
        }

        public void AutoSave()
        {
            throw new NotImplementedException("");
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
    }
}
