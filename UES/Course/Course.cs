using HIT.UES.Login;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace HIT.UES.Course
{
    public class Course : DatabaseType, IOrderedDataContainer<CourseChapter>
    {
        #region Static Error Strings
        public static string NotDepartmentAdmin = "You are not the administrator of the department, and you cannot create or modify" +
            "the basic information of a course.";
        public static string DoesnotContain(Course course, CourseChapter chapter)
            => $"Course {course.CourseName} does not contain Chapter {chapter.ChapterName}.";
        public static string DoesnotContain(CourseChapter chapter, CourseSection section)
            => $"{chapter.ToString()} does not contain Section {section.ToString()}.";
        public static string DoesnotContain(CourseSection section, CourseContent content)
            => $"{section.ToString()} does not contain this content ID={content.CourseContentID}.";
        #endregion

        #region Fields and Properties
        public int CourseID { get; private set; }
        public string CourseName { get; private set; }
        public string IndexWord { get; private set; }
        public Teacher Creator { get; }
        public DateTime CreationTime { get; }
        public DateTime LastModifyTime { get; private set; }
        public string Description { get; set; }
        public virtual List<Teacher> AuthroizedTeacher { get; }
        public List<CourseChapter> Chapters { get; }
        #endregion

        #region Constructors
        public Course()
        {
            Chapters = new List<CourseChapter>();
            AuthroizedTeacher = new List<Teacher>();
        }
        public Course(string name, string indexWord, Teacher creator, string description)
        {
            CourseName = name;
            IndexWord = indexWord;
            Creator = creator;
            Description = description;
            CreationTime = DateTime.Now;
            LastModifyTime = CreationTime;
            AuthroizedTeacher = new List<Teacher> { creator };
            Chapters = new List<CourseChapter>();
        }
        public static Course CreateCourse(string name, string indexWord, Teacher teacher, string description, out string errorMessage)
        {
            if (!teacher.DepartmentAdminAuthority)
            {
                errorMessage = NotDepartmentAdmin;
                return null;
            }
            else
            {
                errorMessage = null;
                var a = new Course(name, indexWord, teacher, description);
                Settings.SaveDataCreation(a);
                return a;
            }
        }
        #endregion

        #region Query course
        //under construction
        #endregion

        #region Chapters, Sections, and Contents
        public CourseChapter CreateChapter(string name, string indexWord, Teacher creator, int order)
        {
            var a = new CourseChapter(name, indexWord, order, creator, this);
            Settings.SaveDataCreation(a);
            Chapters.Add(a);
            Settings.SaveDataModification(this);
            return a;
        }
        public CourseSection CreateSection(string name, Teacher creator, CourseChapter chapter, int order)
        {
            var a = new CourseSection(name, chapter, creator, order);
            chapter.InferiorSection.Add(a);
            Settings.SaveDataCreation(a);
            Settings.SaveDataModification(chapter);
            return a;
        }
        public TextContent CreateTextContent(Teacher creator, string text, CourseSection section, int order)
        {
            var a = new TextContent(order, creator, text, section);
            section.Contents.Add(a);
            Settings.SaveDataCreation(a);
            Settings.SaveDataModification(section);
            return a;
        }
        public Symposium CreateSymposium(Teacher creator, CourseSection section, string question, int order)
        {
            var a = new Symposium(question, order, creator, section);
            section.Contents.Add(a);
            Settings.SaveDataCreation(a);
            Settings.SaveDataModification(section);
            return a;
        }
        #endregion

        #region Inherited and Implemented Members
        public override string CastObjectToJson()
            => JsonConvert.SerializeObject(this, new JsonSerializerSettings
            { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
        public override XmlDocument CastObjectToXml()
            => JsonConvert.DeserializeXmlNode(CastObjectToJson());

        public void ChangeOrder(CourseChapter data, ushort newOrder)
        {
            if (Chapters.Contains(data))
                throw new InvalidOperationException($"The current course {CourseName} does not contains the chapter" +
                    $"{data.ChapterName}.");
            data.Order = newOrder;
            Settings.SaveDataModification(data);
        }

        public void ExchangeOrder(CourseChapter data1, CourseChapter data2)
        {
            if (Chapters.Contains(data1))
                throw new InvalidOperationException($"The current course {CourseName} does not contains the chapter" +
                    $"{data1.ChapterName}.");
            if (Chapters.Contains(data2))
                throw new InvalidOperationException($"The current course {CourseName} does not contains the chapter" +
                    $"{data2.ChapterName}.");

            int t = data1.Order;
            data1.Order = data2.Order;
            data2.Order = t;
            Settings.SaveDataModification(data1);
            Settings.SaveDataModification(data2);
        }

        public override string ToString()
            => $"Course {CourseName}";
        #endregion
    }
}
