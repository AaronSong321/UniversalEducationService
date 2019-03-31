using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using HIT.UES.Login;

namespace HIT.UES.Exam
{
    public abstract class ExamQuestion : DatabaseType
    {
        #region Static Error Messages
        public static string OperatorNotCreator = "You are not the creator of this question, so you cannot modify it. " +
            "Maybe you want to create a copy of this question and make a new one?";
        public static string FinishedQuestionIsReadonly = "A fisnished question is readonly and therefore cannot be modified." +
            "You can still duplicate the question and modify its copy.";
        #endregion

        #region Question Type
        public enum QuestionType { SingleChoice, MultipleChoice, DisorientedChoice, TrueFalse, FreeResponse, Other }

        public static string GetString(QuestionType type) =>
            type switch
        {
            QuestionType.SingleChoice => "SingleChoice",
            QuestionType.MultipleChoice => "MultipleChoice",
            QuestionType.DisorientedChoice => "DisorientedChoice",
            QuestionType.TrueFalse => "TrueOrFalse",
            QuestionType.FreeResponse => "FreeResponse",
            _ => throw new ArgumentException(message: "invalid enum param", paramName: nameof(type)),
        };
        public bool SatisfyQuestionType(QuestionType type)
        {
            if (this is MultipleChoiceQuestion)
            {
                if (ExamQuestionType == QuestionType.SingleChoice && type == QuestionType.SingleChoice)
                    return true;
                else if ((ExamQuestionType == QuestionType.MultipleChoice || ExamQuestionType == QuestionType.DisorientedChoice)
                    && (type == QuestionType.DisorientedChoice || type == QuestionType.MultipleChoice))
                    return true;
                else return false;
            }
            else if (this is TrueFalseQuestion) return type == QuestionType.TrueFalse;
            else if (this is FreeResponseQuestion) return type == QuestionType.FreeResponse;
            else return type == QuestionType.Other;
        }
        #endregion

        #region Members and Properties
        public QuestionType ExamQuestionType { get; private set; }
        public int ExamQuestionID { get; private set; }
        public Teacher Creator { get; private set; }
        public ExamQuestionSet SuperiorQuestionSet { get; private set; }
        public string IndexWord { get; private set; }
        public DateTime CreationTime { get; private set; }
        public DateTime LastModifyTime { get; protected set; }
        public string Explanation { get; private set; }
        //public File Picture { get; private set; }
        public bool Finished { get; private set; }
        #endregion

        #region Statistical Data
        public uint ReferencedTimes { get; private set; }
        public uint AnsweredTimes { get; private set; }
        public ulong TotalMaxScore { get; private set; }
        public float TotalScore { get; private set; }
        public float TotalCanonicalScore { get; private set; }
        #endregion

        #region Creation Methods
        public ExamQuestion()
        {

        }
        public ExamQuestion(Teacher creator, ExamQuestionSet superiorSet, string explanation, string indexWord, 
            QuestionType type)
        {
            Creator = creator;
            SuperiorQuestionSet = superiorSet;
            Explanation = explanation;
            IndexWord = indexWord;
            ExamQuestionType = type;
            Finished = false;
            CreationTime = DateTime.Now;
            LastModifyTime = DateTime.Now;

            ReferencedTimes = 0;
            AnsweredTimes = 0;
            TotalMaxScore = 0;
            TotalScore = 0;
            TotalCanonicalScore = 0;
        }
        public ExamQuestion(Teacher teacher, ExamQuestion other)
        {
            Creator = teacher;
            SuperiorQuestionSet = other.SuperiorQuestionSet;
            Explanation = other.Explanation;
            IndexWord = other.IndexWord;
            ExamQuestionType = other.ExamQuestionType;
            Finished = false;
            CreationTime = DateTime.Now;
            LastModifyTime = DateTime.Now;

            ReferencedTimes = 0;
            AnsweredTimes = 0;
            TotalMaxScore = 0;
            TotalScore = 0;
            TotalCanonicalScore = 0;
        }
        #endregion

        #region Modify Question and Answer
        internal protected virtual void ModifyExamQuestion(string explanation, string indexWord)
        {
            Explanation = explanation;
            IndexWord = indexWord;
            LastModifyTime = DateTime.Now;

            Settings.SaveDataModification(this);
        }
        public virtual void ModifyExamQuestion(in string explanation, in string indexWord, in Teacher teacher, out string errorMessage)
        {
            if (teacher == Creator)
            {
                if (Finished)
                {
                    errorMessage = FinishedQuestionIsReadonly;
                    return;
                }
                errorMessage = null;
                if (explanation == null && indexWord == null)
                    errorMessage = "Neither explanation nor index word is provided. Are you sure?";
                else if (explanation == null)
                    errorMessage = "No explanation is provided. Are you sure?";
                else if (indexWord == null)
                    errorMessage = "No index word is provided. Are you sure?";
                ModifyExamQuestion(explanation, indexWord);
            }
            else
            {
                errorMessage = OperatorNotCreator;
            }
        }
        public virtual void ConfirmQuestionCreation(Teacher teacher, out string errorMessage)
        {
            if (teacher == Creator)
            {
                if (GetQuestionString() != null && GetAnswerString() != null)
                {
                    errorMessage = null;
                    Finished = true;
                }
                else
                {
                    errorMessage = "You have not set the question or the answer.";
                }
            }
            else
                errorMessage = OperatorNotCreator;
        }
        public abstract string GetQuestionString();
        public abstract string GetAnswerString();
        public virtual void SetExplanation(string explanation, Teacher teacher, out string errorMessage)
        {
            if (teacher == Creator)
            {
                Explanation = explanation;
                LastModifyTime = DateTime.Now;
                Settings.SaveDataModification(this);
                errorMessage = null;
            }
            else
                errorMessage = OperatorNotCreator;
        }
        #endregion

        #region Data Statistics
        [NotMapped]
        public float GetScoringRate => TotalScore / TotalMaxScore;
        [NotMapped]
        public float GetCanonicalScoringRate => TotalCanonicalScore / AnsweredTimes;

        /*
        internal void StatisticsReferenceTime(ExamPaperInstance instance)
        {
            throw new NotImplementedException("Still under construction. The programmer does not yet know the optimal time of " +
                "invoking the method.\n Choosing a question, saving an exam paper, or use event system.");
        }
        internal void StatisticsScore(StudentAnswerRecord answerRecord, QuestionChooseRecord questionRecord)
        {
            throw new NotImplementedException("Stash this as well.");
        }
        */
        #endregion

        #region Override and Implemented Members
        public override int GetHashCode() => ExamQuestionID;
        public override bool Equals(object obj)
        {
            if (obj is ExamQuestion question)
                return question.ExamQuestionID == ExamQuestionID;
            else return false;
        }
        #endregion
    }
}
