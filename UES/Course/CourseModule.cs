using HIT.UES.Login;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIT.UES.Course
{
    public class CourseModule: UESModule 
    {
        public Course CreateCourse(Teacher admin, string courseName, string indexWord, string description)
            => new Course();
    }
}
