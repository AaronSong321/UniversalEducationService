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
    public class FreeResponseQuestion : ExamQuestion
    {
        public string QuestionTrunk { get; private set; }
        public string CorrectAnswer { get; private set; }
        //public List<(double, Teacher)> BackupScore { get; private set; }

        #region Creation Methods
        public FreeResponseQuestion()
        {
            //BackupScore = new List<(double, Teacher)>();
        }
        public FreeResponseQuestion(Teacher creator, ExamQuestionSet superiorSet, string explanation, 
            string indexWord)
            : base(creator, superiorSet, explanation, indexWord, QuestionType.FreeResponse)
        {
            //BackupScore = new List<(double, Teacher)>();
        }
        public FreeResponseQuestion(Teacher creator, FreeResponseQuestion other)
            :base (creator, other)
        {
            QuestionTrunk = other.QuestionTrunk;
            CorrectAnswer = other.CorrectAnswer;
        }
        public static FreeResponseQuestion CreateFreeResponseQuestion(Teacher creator, ExamQuestionSet superiorSet,
            string explanation, string indexWord, out string errorMessage)
        {
            if (superiorSet.HasOperateAuthority(creator))
            {
                var n = new FreeResponseQuestion(creator, superiorSet, explanation, indexWord);
                errorMessage = null;
                Settings.SaveDataCreation(n);
                return n;
            }
            else
            {
                errorMessage = $"You are not an administrator of this question set, so you cannot create any question here.";
                return null;
            }
        }
        public static FreeResponseQuestion CreateFreeResponseQuestion(Teacher creator, FreeResponseQuestion other,
            out string errorMessage)
        {
            if (other.SuperiorQuestionSet.HasOperateAuthority(creator))
            {
                var n = new FreeResponseQuestion(creator, other);
                errorMessage = null;
                Settings.SaveDataCreation(n);
                return n;
            }
            else
            {
                errorMessage = $"You are not an administrator of this question set, so you cannot create any question here.";
                return null;
            }
        }
        #endregion

        #region Question and Answer
        internal void SetQuestion(string trunk)
        {
            QuestionTrunk = trunk;
            LastModifyTime = DateTime.Now;
            Settings.SaveDataModification(this);
        }
        public void SetQuestion(string trunk, Teacher teacher, out string errorMessage)
        {
            if (teacher == Creator)
            {
                SetQuestion(trunk);
                errorMessage = null;
            }
            else
                errorMessage = OperatorNotCreator;
        }
        public void SetAnswer(string answer, Teacher teacher, out string errorMessage)
        {
            if (teacher != Creator)
            {
                errorMessage = OperatorNotCreator;
            }
            else if (Finished)
            {
                errorMessage = FinishedQuestionIsReadonly;
            }
            else
            {
                CorrectAnswer = answer;
                errorMessage = null;
                Settings.SaveDataModification(this);
            }
        }
        #endregion

        #region Override and Implemented Members
        public override string CastObjectToJson()
            => JsonConvert.SerializeObject(this, new JsonSerializerSettings
            { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
        public override XmlDocument CastObjectToXml()
            => JsonConvert.DeserializeXmlNode(CastObjectToJson());
        public override string GetAnswerString() => CorrectAnswer;
        public override string GetQuestionString() => QuestionTrunk;
        #endregion
    }
}
