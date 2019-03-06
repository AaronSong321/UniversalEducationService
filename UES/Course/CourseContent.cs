using HIT.UES.Login;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace HIT.UES.Course
{
    public class CourseContent : DatabaseType
    {
        public int CourseContentID { get; private set; }
        public CourseSection SuperiorSection { get; private set; }
        public int Order { get; set; }
        public Teacher Creator { get; private set; }
        public DateTime CreationTime { get; private set; }

        public CourseContent()
        {

        }
        public CourseContent(int order, Teacher creator, CourseSection superior)
        {
            Order = order;
            Creator = creator;
            SuperiorSection = superior;
        }

        #region Inherited and implemented members
        public override string CastObjectToJson()
            => JsonConvert.SerializeObject(this);
        public override XmlDocument CastObjectToXml()
            => JsonConvert.DeserializeXmlNode(CastObjectToJson());
        public override int GetHashCode() => CourseContentID;
        public override bool Equals(object obj)
        {
            if (obj is CourseContent content)
                return CourseContentID == content.CourseContentID;
            else return false;
        }        
        #endregion
    }

    public class TextContent : CourseContent
    {
        public string Text { get; private set; }

        public TextContent() : base()
        {

        }
        public TextContent(int order, Teacher creator, string text, CourseSection superior) : base(order, creator, superior)
        {
            Text = text;
        }
    }

    public class FileContent : CourseContent
    {
        public FileStream File { get; set; }

        public FileContent()
        {
            throw new NotImplementedException("Haven't figured out how to store a file through database.");
        }
    }

    public class Symposium : CourseContent
    {
        public class Comment : DatabaseType
        {
            public Student Commenter { get; private set; }
            public Student Commentee { get; private set; }
            public string Text { get; private set; }
            public DateTime CreationTime { get; private set; }
            public int Order { get;  set; }
            public Symposium SuperiorSymposium { get; private set; }

            public Comment()
            {
                CreationTime = DateTime.Now;
            }
            public Comment(Student commenter, Student commentee, string text, int order)
            {
                Commenter = commenter;
                Commentee = commentee;
                Text = text;
                CreationTime = DateTime.Now;
                Order = order;
            }
            public override string CastObjectToJson()
                => JsonConvert.SerializeObject(this);
            public override XmlDocument CastObjectToXml()
                => JsonConvert.DeserializeXmlNode(CastObjectToJson());
        }

        public int SymposiumID { get; private set; }
        public string Question { get; set; }
        public List<Comment> Comments { get; private set; }

        public Symposium() : base()
        {
            Comments = new List<Comment>();
        }
        public Symposium(string question, int order, Teacher teacher, CourseSection superior) : base(order, teacher, superior)
        {
            Question = question;
            Comments = new List<Comment>();
        }

        public Comment AddComment(string text, Student commenter, Student commentee = null)
        {
            var a = new Comment(commenter, commentee, text, Comments.Count + 1);
            Comments.Add(a);
            Settings.SaveDataCreation(a);
            Settings.SaveDataModification(this);
            return a;
        }
    }
}
