using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using HIT.UES.Login;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HIT.UES.Exam
{
    
    public class ExamQuestionSet : DatabaseType
    {
        #region Static Error Messages
        public static string NoUseAuthority = "You do not have the authority to use current question set.";
        public int ExamQuestionSetID { get; private set; }
        public string QuestionSetName { get; private set; }
        public string IndexWord { get; private set; }
        public Teacher Creator { get; private set; }
        public DateTime LastModifyTime { get; private set; }
        public virtual List<Teacher> AuthorizedOperators { get; private set; }
        public virtual List<Teacher> AuthorizedUsers { get; private set; }
        public virtual List<ExamQuestion> QuestionSet { get; private set; }
        #endregion

        #region Creation Methods
        public ExamQuestionSet()
        {

        }
        public ExamQuestionSet(string name, string indexWord, Teacher creator)
        {
            QuestionSetName = name;
            IndexWord = indexWord;
            Creator = creator;
            LastModifyTime = DateTime.Now;
            AuthorizedOperators = new List<Teacher>();
            AuthorizedUsers = new List<Teacher>();
            AuthorizedOperators.Add(creator);
            AuthorizedUsers.Add(creator);
            QuestionSet = new List<ExamQuestion>();
        }
        public static ExamQuestionSet CreateExamQuestionSet(string name, string indexWord, Teacher creator, out string errorMessage)
        {
            if (creator.DepartmentAdminAuthority)
            {
                errorMessage = null;
                var qs = new ExamQuestionSet(name, indexWord, creator);
                Settings.SaveDataCreation(qs);
                return qs;
            }
            else
            {
                errorMessage = Exam.NotDepartmentAdmin;
                return null;
            }
        }
        internal void ModifyQuestionSet(string name, string indexWord)
        {
            QuestionSetName = name;
            IndexWord = indexWord;
            LastModifyTime = DateTime.Now;
            Settings.SaveDataModification(this);
        }
        public void ModifyQuestionSet(string name, string indexWord, Teacher teacher, out string errorMessage)
        {
            if (teacher != Creator)
            {
                errorMessage = Exam.NotCreator;
            }
            else
            {
                errorMessage = null;
                ModifyQuestionSet(name, indexWord);
            }

        }
        #endregion

        #region Authority
        internal void AddToAuthorizedOperators(Teacher teacher)
        {
            AuthorizedOperators.Add(teacher);
            AuthorizedUsers.Add(teacher);
        }
        public bool HasOperateAuthority(Teacher teacher) => AuthorizedOperators.Contains(teacher);
        internal void RemoveFromAuthorizedOperators(Teacher teacher)
        {
            AuthorizedOperators.Remove(teacher);
            AuthorizedUsers.Remove(teacher);
        }
        internal void AddToAuthorizedUsers(Teacher teacher) => AuthorizedUsers.Add(teacher);
        public bool HasUseAuthority(Teacher teacher) => AuthorizedUsers.Contains(teacher);
        internal void RemoveFromAuthorizedUsers(Teacher teacher) => AuthorizedUsers.Remove(teacher);

        public void AddToOperators(Teacher creator, Teacher teacher, out string errorMessage)
        {
            if (creator != Creator)
                errorMessage = "You are not the creator of this question set and you have no authority to add other teachers" +
                    "to the operator or user list of this question set.";
            else if (HasOperateAuthority(teacher))
                errorMessage = "The teacher has already had this authority.";
            else
            {
                AddToAuthorizedOperators(teacher);
                Settings.SaveDataModification(this);
                errorMessage = null;
            }
        }
        public void RemoveFromOperators(Teacher creator, Teacher teacher, out string errorMessage)
        {
            if (creator != Creator)
                errorMessage = "You are not the creator of this question set and you have no authority to add other teachers" +
                    "to the operator or user list of this question set.";
            else if (!HasOperateAuthority(teacher))
                errorMessage = "The teacher does not have this authority.";
            else
            {
                RemoveFromAuthorizedOperators(teacher);
                Settings.SaveDataModification(this);
                errorMessage = null;
            }
        }
        public void AddToUsers(Teacher creator, Teacher teacher, out string errorMessage)
        {
            if (creator != Creator)
                errorMessage = "You are not the creator of this question set and you have no authority to add other teachers" +
                    "to the operator or user list of this question set.";
            else if (HasUseAuthority(teacher))
                errorMessage = "The teacher has already had this authority.";
            else
            {
                AddToAuthorizedUsers(teacher);
                Settings.SaveDataModification(this);
                errorMessage = null;
            }
        }
        public void RemoveFromUsers(Teacher creator, Teacher teacher, out string errorMessage)
        {
            if (creator != Creator)
                errorMessage = "You are not the creator of this question set and you have no authority to add other teachers" +
                    "to the operator or user list of this question set.";
            else if (!HasUseAuthority(teacher))
                errorMessage = "The teacher does not have this authority.";
            else
            {
                RemoveFromAuthorizedUsers(teacher);
                Settings.SaveDataModification(this);
                errorMessage = null;
            }
        }
        #endregion

        #region Add Question
        internal List<ExamQuestion> FindSimilarQuestions(string indexWord)
        {
            var ans = new List<ExamQuestion>();
            var indexes = indexWord.Split(' ', '+');
            foreach (var question in ans)
            {
                foreach (var index in indexes)
                {
                    if (question.GetQuestionString().Contains(index) || question.IndexWord.Contains(index))
                    {
                        ans.Add(question);
                        break;
                    }
                }
            }
            return ans;
        }

        internal void AddQuestion(ExamQuestion question)
        {
            if (!QuestionSet.Contains(question))
            {
                QuestionSet.Add(question);
                LastModifyTime = DateTime.Now;
                Settings.SaveDataModification(this);
            }
        }
        /// <summary>
        /// Under construction.
        /// </summary>
        /// <param name="question"></param>
        internal void RemoveQuestion(ExamQuestion question)
        {
            bool val = QuestionSet.Remove(question);
            if (val) LastModifyTime = DateTime.Now;
            Settings.SaveDataModification(this);
        }
        #endregion

        #region Query Questions
        public (List<ExamQuestion>, string) GetAllExamQuestions(Teacher teacher)
        {
            if (HasUseAuthority(teacher)) return (QuestionSet, null);
            else return (null, NoUseAuthority);
        }
        internal protected List<ExamQuestion> GetExamQuestion(Predicate<ExamQuestion> filter)
        {
            var query = from b in QuestionSet where filter(b) select b;
            return query.ToList();
        }
        public (List<ExamQuestion>, string) GetExamQuestion(Predicate<ExamQuestion> filter, Teacher teacher)
        {
            if (HasUseAuthority(teacher)) return (GetExamQuestion(filter), null);
            else return (null, NoUseAuthority);
        }
        internal protected List<ExamQuestion> GetExamQuestion(string indexWord)
            => GetExamQuestion((question) => question.IndexWord.Contains(indexWord));
        public (List<ExamQuestion>, string) GetExamQuestion(string indexWord, Teacher teacher)
        {
            if (HasUseAuthority(teacher)) return (GetExamQuestion(indexWord), null);
            else return (null, NoUseAuthority);
        }
        #endregion

        #region Query Question Set
        public static List<ExamQuestionSet> GetAllQuestionSets()
            => Settings.uesContext.ExamQuestionSets.ToList();
        public static List<ExamQuestionSet> GetQuestionSet(Predicate<ExamQuestionSet> filter)
            => (from b in Settings.uesContext.ExamQuestionSets where filter(b) select b).ToList();
        public static List<ExamQuestionSet> GetQuestionSet(string indexWord)
            => GetQuestionSet((eqs) => eqs.IndexWord.Contains(indexWord) || eqs.QuestionSetName.Contains(indexWord));
        #endregion

        #region Override and Implemented Members
        public override string CastObjectToJson()
            => JsonConvert.SerializeObject(this, new JsonSerializerSettings
            { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
        public override XmlDocument CastObjectToXml()
            => JsonConvert.DeserializeXmlNode(CastObjectToJson());

        public override int GetHashCode() => ExamQuestionSetID;
        public override bool Equals(object obj)
        {
            if (obj is ExamQuestionSet set)
                return set.ExamQuestionSetID == ExamQuestionSetID;
            else return false;
        }
        #endregion
    }
}
