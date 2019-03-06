using HIT.UES.Login;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIT.UES.Course
{
    [NotMapped]
    public class CourseModule: UESModule 
    {
        public Course CreateCourse(Teacher admin, string courseName, string indexWord, string description, out string errorMessage)
            => Course.CreateCourse(courseName, indexWord, admin, description, out errorMessage);
        public CourseChapter CreateChapter(Course course, string name, string indexWord, int order, Teacher creator)
            => course.CreateChapter(name, indexWord, creator, order);
        public CourseSection CreateSection(Course course, string name, Teacher creator, CourseChapter chapter, int order)
            => course.CreateSection(name, creator, chapter, order);
        public TextContent CreateTextContent(Course course, Teacher creator, string text, CourseSection section, int order)
            => course.CreateTextContent(creator, text, section, order);
        public Symposium CreateSymposium(Course course, Teacher creator, CourseSection section, string question, int order)
            => course.CreateSymposium(creator, section, question, order);
        public Symposium.Comment CreateComment(Symposium symposium, string text, Student commenter, Student commentee = null)
            => symposium.AddComment(text, commenter, commentee);
    }
}
