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
    public class TrueFalseQuestion : ExamQuestion
    {
        #region Members and Properties
        public string QuestionTrunk { get; private set; }
        public bool? CorrectAnswer { get; private set; }
        #endregion

        #region Creation Methods
        public TrueFalseQuestion()
        {
            CorrectAnswer = null;
        }
        public TrueFalseQuestion(Teacher creator, ExamQuestionSet superiorSet, string explanation, string indexWord)
            : base (creator, superiorSet, explanation, indexWord, QuestionType.TrueFalse)
        {
            CorrectAnswer = null;
        }
        public TrueFalseQuestion(Teacher creator, TrueFalseQuestion other)
            : base (creator, other)
        {
            CorrectAnswer = other.CorrectAnswer;
            QuestionTrunk = other.QuestionTrunk;
        }
        public static TrueFalseQuestion CreateTrueFalseQuestion(Teacher creator, ExamQuestionSet superiorSet, string explanation,
            string indexWord, out string errorMessage)
        {
            if (superiorSet.HasOperateAuthority(creator))
            {
                var n = new TrueFalseQuestion(creator, superiorSet, explanation, indexWord);
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
        public static TrueFalseQuestion CreateTrueFalseQuestion(Teacher teacher, TrueFalseQuestion other, out string errorMessage)
        {
            if (other.SuperiorQuestionSet.HasOperateAuthority(teacher))
            {
                var n = new TrueFalseQuestion(teacher, other);
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
        private void SetQuestion(string trunk)
        {
            QuestionTrunk = trunk;
            LastModifyTime = DateTime.Now;
            Settings.SaveDataModification(this);
        }
        public void SetQuestion(string trunk, Teacher teacher, out string errorMessage)
        {
            if (teacher == Creator)
            {
                if (Finished)
                {
                    errorMessage = FinishedQuestionIsReadonly;
                    return;
                }
                SetQuestion(trunk);
                errorMessage = null;
            }
            else
                errorMessage = OperatorNotCreator;
        }
        public override string GetAnswerString() => CorrectAnswer.ToString();
        public override string GetQuestionString() => QuestionTrunk;
        public void SetAnswer(bool answer, Teacher teacher, out string errorMessage)
        {
            if (teacher != Creator)
            {
                CorrectAnswer = answer;
                errorMessage = null;
            }
            else
            {
                errorMessage = OperatorNotCreator;
            }
        }
        #endregion

        #region Override and Implemented Members
        public override string CastObjectToJson()
            => JsonConvert.SerializeObject(this, new JsonSerializerSettings
            { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
        public override XmlDocument CastObjectToXml()
            => JsonConvert.DeserializeXmlNode(CastObjectToJson());
        #endregion

    }
}
