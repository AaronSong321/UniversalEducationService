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
    public class MultipleChoiceQuestion : ExamQuestion
    {
        public static ushort OptionNumberMax = 5;
        public static ushort OptionNumberMin = 2;

        #region Members and Properties
        public ushort OptionNumber { get; private set; }
        public ushort CorrectOptionNumber { get; private set; }
        public string QuestionTrunk { get; private set; }
        public string OptionA { get; private set; }
        public string OptionB { get; private set; }
        public string OptionC { get; private set; }
        public string OptionD { get; private set; }
        public string OptionE { get; private set; }
        public string CorrectAnswer { get; private set; }
        #endregion

        #region Creation and Basic information
        public MultipleChoiceQuestion()
        {

        }
        public MultipleChoiceQuestion(Teacher creator, ExamQuestionSet superiorSet, string explanation, string indexWord, 
            ushort optionNumber, ushort correctOptionNumber, QuestionType type)
            : base(creator, superiorSet, explanation, indexWord, type)
        {
            OptionNumber = optionNumber;
            CorrectOptionNumber = correctOptionNumber;
        }
        public MultipleChoiceQuestion(Teacher creator, MultipleChoiceQuestion other)
            :base (creator, other)
        {
            OptionNumber = other.OptionNumber;
            CorrectOptionNumber = other.CorrectOptionNumber;
            QuestionTrunk = other.QuestionTrunk;
            OptionA = other.OptionA;
            OptionB = other.OptionB;
            OptionC = other.OptionC;
            OptionD = other.OptionD;
            OptionE = other.OptionE;
            CorrectAnswer = other.CorrectAnswer;
        }
        public static (MultipleChoiceQuestion, string) CreateMultipleChoiceQuestion(Teacher creator, ExamQuestionSet superiorSet,
            string explanation, string indexWord, QuestionType type, ushort optionNumber, ushort correctOptionNumber)
        {
            if (!superiorSet.HasOperateAuthority(creator)) return (null, $"The teacher {creator.PersonName} does not have" +
                    $"the authority to add a question to the current question set.");
            if (optionNumber > OptionNumberMax) return (null, $"A multiple choice question cannot have more than " +
                    $"{OptionNumberMax} options. You are expecting {optionNumber} options.");
            if (optionNumber < OptionNumberMin) return (null, $"A multiple choice question cannot have less than " +
                    $"{OptionNumberMin} options. You are expecting {optionNumber} options.");
            if (correctOptionNumber > optionNumber) return (null, $"The number of correct options cannot surpass the number" +
                    $" of options.");
            if (correctOptionNumber < 1) return (null, $"There should be at least one correct option.");

            if (type != QuestionType.SingleChoice && type != QuestionType.MultipleChoice && type != QuestionType.DisorientedChoice)
                return (null, $"Please choose a valid subtype of multiple choice question.");
            var newQuestion = new MultipleChoiceQuestion(creator, superiorSet, explanation, indexWord, optionNumber,
                correctOptionNumber, type);
            Settings.SaveDataCreation(newQuestion);
            return (newQuestion, "Creation success.");
        }
        public static MultipleChoiceQuestion DuplicateMultipleChoiceQuestion(Teacher creator, MultipleChoiceQuestion question,
            out string errorMessage)
        {
            if (question.SuperiorQuestionSet.HasOperateAuthority(creator))
            {
                var n = new MultipleChoiceQuestion(creator, question);
                Settings.SaveDataCreation(n);
                errorMessage = null;
                return n;
            }
            else
            {
                errorMessage = $"The teacher {creator.PersonName} does not have" +
                    $"the authority to add a question to the current question set.";
                return null;
            }
        }
        #endregion

        #region Set Question and Answer
        private void SetQuestion(string trunk, string a, string b, string c, string d, string e)
        {
            QuestionTrunk = trunk;
            if (OptionNumber >= 1) OptionA = a;
            if (OptionNumber >= 2) OptionB = b;
            if (OptionNumber >= 3) OptionC = c;
            if (OptionNumber >= 4) OptionD = d;
            if (OptionNumber >= 5) OptionE = e;
            LastModifyTime = DateTime.Now;
            Settings.SaveDataModification(this);
        }
        public void SetQuestion(string trunk, string a, string b, string c, string d, string e, Teacher teacher, 
            out string errorMessage)
        {
            if (teacher == Creator)
            {
                if (Finished)
                {
                    errorMessage = FinishedQuestionIsReadonly;
                    return;
                }
                List<char> nullChars = new List<char>();

                if (a == null)
                    nullChars.Add('a');
                if (b == null)
                    nullChars.Add('b');
                if (OptionNumber >= 3 && c == null)
                    nullChars.Add('c');
                if (OptionNumber >= 4 && d == null)
                    nullChars.Add('d');
                if (OptionNumber >= 5 && e == null)
                    nullChars.Add('e');
                if (nullChars.Count == 0)
                {
                    SetQuestion(trunk, a, b, c, d, e);
                    errorMessage = null;
                    return;
                }
                else
                {
                    if (nullChars.Count == 1)
                    {
                        errorMessage = $"Option {nullChars[0]} cannot be null.";
                    }
                    else
                    {
                        errorMessage = $"Option {nullChars[0]}";
                        for (int i = 1; i < nullChars.Count; i++)
                            errorMessage += $", {nullChars[i]}";
                        errorMessage += " cannot be null";
                    }
                    return;
                }
            }
            else
            {
                errorMessage = OperatorNotCreator;
            }
        }

        public virtual bool CheckAnswerValidity(string answer, out string invalidity)
        {
            if (answer.Length > OptionNumber)
            {
                invalidity = "Too many options are chosen.";
                return false;
            }
            answer = answer.ToUpper();
            var validOptions = "AB";
            if (OptionNumber >= 3) validOptions += "C";
            if (OptionNumber >= 4) validOptions += "D";
            if (OptionNumber >= 5) validOptions += "E";
            foreach (var character in answer)
                if (!validOptions.Contains(character))
                {
                    invalidity = $"The answer include invalid character {character}. Valid characters for this " +
                        $"question are {validOptions}.";
                    return false;
                }
            bool[] chosen = new bool[5] { false, false, false, false, false };
            foreach (var character in answer)
            {
                var num = character - 'A';
                if (chosen[num])
                {
                    invalidity = $"Duplicate option {character}.";
                    return false;
                }
                chosen[num] = true;
            }
            invalidity = null;
            return true;
        }
        internal virtual bool CheckAndSetAnswer(in string answer, out string invalidity)
        {
            if (Finished)
            {
                invalidity = FinishedQuestionIsReadonly;
                return false;
            }
            var validity = CheckAnswerValidity(answer, out invalidity);
            if (validity)
            {
                CorrectAnswer = answer;
                LastModifyTime = DateTime.Now;
                Settings.SaveDataModification(this);
            }
            return validity;
        }
        public bool SetMultipleChoiceAnswer(in string answer, in Teacher teacher, out string invalidity)
        {
            if (teacher == Creator)
            {
                return CheckAndSetAnswer(answer, out invalidity);
            }
            else
            {
                invalidity = OperatorNotCreator;
                return false;
            }
        }
        #endregion

        #region Inherited and Implemeted Members
        public override string CastObjectToJson()
            => JsonConvert.SerializeObject(this, new JsonSerializerSettings
            { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
        public override XmlDocument CastObjectToXml()
            => JsonConvert.DeserializeXmlNode(CastObjectToJson());

        public override string GetAnswerString() => CorrectAnswer;

        public override string GetQuestionString()
        {
            var ans = $"{QuestionTrunk} (   )\nA. {OptionA}\nB. {OptionB}\n";
            if (OptionNumber >= 3) ans += $"C. {OptionC}\n";
            if (OptionNumber >= 4) ans += $"D. {OptionD}\n";
            if (OptionNumber >= 5) ans += $"E. {OptionE}\n";
            return ans;
        }
        #endregion
    }
}
