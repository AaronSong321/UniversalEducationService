using HIT.UES.Login;
using HIT.UES.Server.ServiceDeclaration;
using HIT.UES.Server.StandardServiceProvider;
using RestSharp;
using System;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Linq;
using System.Text.RegularExpressions;

namespace HIT.UES.Server.Service
{
    public class RestServerTest
    {
        public string Port = "7788";
        private WebServiceHost host;

        public void BootServer()
        {
            Settings.InitDatabase("ChemistryDatabase");
            PersonCreate();
            DemonstratePerson();
            CreateExam();
            DemonstrateExam();

            host = new WebServiceHost(typeof(LoginService), new Uri($"http://127.0.0.1:{Port}/"));
            try
            {
                host.Open();
                Console.ReadKey();
                host.Close();
            }
            catch (CommunicationException cex)
            {
                Console.WriteLine($"Communication Exception caught: {cex.Message}");
                host.Close();
            }
        }

        private bool ExistStudent(string studentName)
        {
            var ans = false;
            foreach (var student in Settings.uesContext.Students)
                if (student.PersonName == studentName)
                {
                    ans = true;
                    break;
                }
            return ans;
        }
        private bool ExistsAdmin(string adminName)
        {
            var ans = false;
            foreach (var admin in Settings.uesContext.Administrators)
                if (admin.AdminName==adminName)
                {
                    ans = true;
                    break;
                }
            return ans;
        }
        public void PersonCreate()
        {
            if (!ExistStudent("Jack"))
                Settings.SaveDataCreation(new Student("Jack","aaa0001"));
            if (!ExistStudent("Frank"))
                Settings.SaveDataCreation(new Student("Frank", "aaa0002"));
            if (!ExistStudent("Henry"))
                Settings.SaveDataCreation(new Student("Henry", "aaa0003"));
            if (!ExistStudent("Zhanshen"))
                Settings.SaveDataCreation(new Teacher("Zhanshen", "aaa0004"));
            if (!ExistStudent("Wang"))
                Settings.SaveDataCreation(new Teacher("Wang", "aaa0005"));
            if (!ExistsAdmin("Zhou"))
                Settings.SaveDataCreation(new Administrator("Zhou", "admin001"));

            var admin = Settings.uesContext.Administrators;
            var zhous = (from b in admin where b.AdminName == "Zhou" select b).ToList();
            if (zhous.Count != 1)
            {
                Console.WriteLine($"There are {zhous.Count} administrators named Zhou.");
            }
            var zhou = zhous[0];
            var zhanshen = (from b in Settings.uesContext.Teachers where b.PersonName == "Zhanshen" select b).ToList()[0];
            zhou.GrantDepartmentAdminAuthority(zhanshen, out string a);
            if (a != null)
                Console.WriteLine(Settings.uesContext.Entry(zhanshen).State);
            Settings.uesContext.SaveChanges();
        }
        public void DemonstratePerson()
        {
            var people = (from b in Settings.uesContext.Students select b).ToList();
            var dummyPattern = new Regex("\\{.*\\}");
            foreach (var person in people)
            {
                if (dummyPattern.IsMatch(person.PersonName))
                {
                    Settings.SaveDataRemove(person);
                }
            }

            foreach (var person in people)
            {
                Console.WriteLine($"Student ID={person.StudentID}, name={person.PersonName}.");
            }
            var teachers = (from b in Settings.uesContext.Teachers select b).ToList();
            foreach (var t in teachers)
                Console.WriteLine($"Teacher ID={t.StudentID}, name={t.PersonName}, IsDepartmentAdmin={t.DepartmentAdminAuthority}.");
        }

        private bool ExistsExam(string name)
        {
            bool ans = false;
            foreach (var a in Settings.uesContext.Exams)
            {
                if (a.ExamName == name)
                {
                    ans = true;
                    break;
                }
            }
            return ans;
        }
        private void CreateExam()
        {
            var zhanshenList = (from b in Settings.uesContext.Teachers where b.PersonName == "Zhanshen" select b).ToList();
            if (zhanshenList.Count != 1)
            {
                return;
            }
            var zhanshen = zhanshenList[0];
            if (!ExistsExam("Programming language (C++)"))
            {
                Settings.SaveDataCreation(new Exam.Exam("Programming language (C++)", zhanshen, "Computer Science", "CS OOP C++", "Programming language C++" +
                    "by zhanshen, welcome to select"));
            }
            if (!ExistsExam("Database System"))
                Settings.SaveDataCreation(new Exam.Exam("Database System", zhanshen, "Computer Science", "Database MySql", "welcome to " +
                    "database system coures by zhanshen", 100));
        }

        private void DemonstrateExam()
        {
            var exams = (from b in Settings.uesContext.Exams select b).ToList();
            Console.WriteLine($"{exams.Count} exams found.");
            foreach (var e in exams)
            {
                Console.WriteLine($"Exam id={e.ExamID}, name={e.ExamName}, teacher={e.Creator.PersonName}");
            }
        }
    }
}
