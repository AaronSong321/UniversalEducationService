using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using HIT.UES.Login;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HIT.UES.Exam
{
    public class ExamPaper : DatabaseType, IOrderedDataContainer<QuestionChooseRecord>
    {
        #region Static Error Messages
        public static ushort paperTotalScore = 100;
        public static string MaxScoreViolation(float currentScore, float maxScore)
            => $"Maximum score violation: current score {(int)(currentScore+0.01)}, max score {(int)(maxScore+0.01)}";
        public static string QuestionUniquenessViolation(int duplicateNumber)
            => $"Question uniqueness violation: there are two questions in your exam paper that are same, " +
            $"duplicate question number {duplicateNumber}.";
        public static string QuestionOrderUniquenessViolation(int a, int b, int c)
            => $"Question order uniqueness violation: there are two questions in your exam paper that has the same order, " +
            $"question number {a} and {b}, the duplicate quesiton order is {c}.";
        public static string AlreadyFinished = "This exam paper has already been finished, and you cannot modify it" +
            "anymore. But you may create a copy of it.";
        public static string NotFinished = "This exam paper has not been finished, so you cannot create an instance of it " +
            "for a student.";
        #endregion

        #region Fields and Properties
        public int ExamPaperID { get; private set; }
        public Exam SuperiorExam { get; private set; }
        public string ExamPaperName { get; private set; }
        public string Description { get; private set; }
        public Teacher Creator { get; private set; }
        public string IndexWord { get; private set; }
        public DateTime CreationTime { get; private set; }
        public DateTime LastModifyTime { get; private set; }
        public ushort MaxScore { get; private set; }
        public float ExamDuration { get; private set; }
        public bool Finished { get; private set; }
        public List<QuestionChooseRecord> QuestionSet { get; private set; }
        public List<ExamPaperInstance> Instances { get; private set; }
        #endregion

        #region Constructors and Basic Information
        public ExamPaper()
        {

        }
        public ExamPaper(Exam superiorExam, Teacher creator, string indexWord, string description, float duration,
            ushort score = 100)
        {
            SuperiorExam = superiorExam;
            IndexWord = indexWord;
            Description = description;
            ExamDuration = duration;
            MaxScore = score;
            Finished = false;

            Creator = creator;
            CreationTime = DateTime.Now;
            LastModifyTime = CreationTime;
            QuestionSet = new List<QuestionChooseRecord>();
            Instances = new List<ExamPaperInstance>();
        }
        public static ExamPaper CreateExamPaper(Exam superiorExam, Teacher examAdmin, string indexWord, string description,
            float duration, ushort score, out string errorMessage)
        {
            if (superiorExam.HasAdminAuthority(examAdmin))
            {
                var a = new ExamPaper(superiorExam, examAdmin, indexWord, description, duration, score);
                Settings.SaveDataCreation(a);
                superiorExam.ExamPapers.Add(a);
                Settings.SaveDataModification(superiorExam);
                errorMessage = null;
                return a;
            }
            else
            {
                errorMessage = Exam.NoExamineAuthority;
                return null;
            }
        }

        public void ModifyExamPaper(Teacher teacher, string indexWord, string description, uint duration,
            ushort score, out string errorMessage)
        {
            if (teacher != Creator)
            {
                errorMessage = Exam.NotCreator;
            }
            else if (Finished)
            {
                errorMessage = AlreadyFinished;
            }
            else
            {
                IndexWord = indexWord;
                Description = description;
                ExamDuration = duration;
                MaxScore = score;
                LastModifyTime = DateTime.Now;
                errorMessage = null;
                Settings.SaveDataModification(this);
            }
        }
        public ExamPaper(Teacher teacher, ExamPaper examPaper)
        {
            SuperiorExam = examPaper.SuperiorExam;
            Creator = teacher;
            IndexWord = examPaper.IndexWord;
            Description = examPaper.Description;
            ExamDuration = examPaper.ExamDuration;
            MaxScore = examPaper.MaxScore;
            Finished = false;

            CreationTime = DateTime.Now;
            LastModifyTime = CreationTime;
            QuestionSet = new List<QuestionChooseRecord>();
            foreach (var record in examPaper.QuestionSet)
                QuestionSet.Add(new QuestionChooseRecord(this, record));
            Instances = new List<ExamPaperInstance>();
        }
        public static ExamPaper DuplicateExamPaper(Teacher teacher, ExamPaper other, out string errorMessage)
        {
            if (other.SuperiorExam.HasAdminAuthority(teacher))
            {
                errorMessage = null;
                var a = new ExamPaper(teacher, other);
                Settings.SaveDataCreation(a);
                other.SuperiorExam.ExamPapers.Add(a);
                Settings.SaveDataModification(a);
                return a;
            }
            else
            {
                errorMessage = "You are not the administrator of this exam and you cannot duplicate an exam paper.";
                return null;
            }
        }
        #endregion

        #region Choose Questions
        internal void ChooseQuestion(ExamQuestion q, ushort score, ushort order)
        {
            var record = new QuestionChooseRecord(this, score, order, q);
            QuestionSet.Add(record);
            Settings.SaveDataCreation(record);
            Settings.SaveDataModification(this);
        }
        internal void ChooseQuestion(ExamQuestion q, ushort score)
            => QuestionSet.Add(new QuestionChooseRecord(this, score, (ushort)QuestionSet.Count, q));

        public int GetQuestionNumber() => QuestionSet.Count;
        public bool ContainsQuestion(ExamQuestion q)
        {
            foreach (var record in QuestionSet)
                if (record.Question == q)
                    return true;
            return false;
        }
        #endregion

        #region Check Consistency
        [NotMapped]
        public float CurrentScore
        {
            get
            {
                float c = 0;
                foreach (var ans  in QuestionSet)
                    c += ans.MaxScore;
                return c;
            }
        }

        public bool CheckConsistency(out string inconsistency)
        {
            if (Math.Abs(CurrentScore-MaxScore) >= 0.01)
            {
                inconsistency = MaxScoreViolation(CurrentScore, MaxScore);
                return false;
            }
            int[] order = new int[GetQuestionNumber()];
            for (int i = 0; i < QuestionSet.Count; i++)
            {
                if (order[QuestionSet[i].QuestionOrder] != 0)
                {
                    inconsistency = QuestionOrderUniquenessViolation(order[QuestionSet[i].QuestionOrder], 
                        i, QuestionSet[i].QuestionOrder);
                    return false;
                }
                else
                {
                    order[QuestionSet[i].QuestionOrder] = i;
                }
            }
            HashSet<int> c = new HashSet<int>();
            foreach (var q in QuestionSet)
            {
                if (c.Contains(q.Question.ExamQuestionID))
                {
                    inconsistency = QuestionUniquenessViolation(q.Question.ExamQuestionID);
                    return false;
                }
                else
                    c.Add(q.Question.ExamQuestionID);
            }
            inconsistency = null;
            return true;
        }
        public void SaveExamPaper(Teacher creator)
        {
            if (!Finished && CheckConsistency(out _) && creator == Creator)
            {
                Finished = true;
                Settings.SaveDataModification(this);
            }
        }
        #endregion

        #region Generate Instances
        internal ExamPaperInstance GenerateExamPaperInstance(Student candidate, out string errorMessage)
        {
            if (Finished)
            {
                errorMessage = null;
                var a = new ExamPaperInstance(this, candidate);
                Settings.SaveDataCreation(a);
                Instances.Add(a);
                Settings.SaveDataModification(this);
                return a;
            }
            else
            {
                errorMessage = NotFinished;
                return null;
            }
        }
        #endregion

        #region Inherited and Implemented Members
        public override string CastObjectToJson()
            => JsonConvert.SerializeObject(this, new JsonSerializerSettings
            { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
        public override XmlDocument CastObjectToXml()
            => JsonConvert.DeserializeXmlNode(CastObjectToJson());

        public void ExchangeOrder(QuestionChooseRecord data1, QuestionChooseRecord data2)
        {
            int temp = data1.QuestionOrder;
            data1.ModifyOrder(data2.QuestionOrder);
            data2.ModifyOrder(temp);
            Settings.SaveDataModification(data1);
            Settings.SaveDataModification(data2);
        }

        public void ChangeOrder(QuestionChooseRecord data, ushort newOrder)
        {
            data.ModifyOrder(newOrder);
            Settings.SaveDataModification(data);
        }

        public override bool Equals(object obj)
        {
            if (obj is ExamPaper paper)
                return ExamPaperID == paper.ExamPaperID;
            else
                return base.Equals(obj);
        }
        public override int GetHashCode() => ExamPaperID;
        #endregion
    }
}
