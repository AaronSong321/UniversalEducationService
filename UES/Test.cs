using HIT.UES;
using HIT.UES.Login;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HIT.UES
{
    [NotMapped]
    public class Test
    {
        public Test()
        {
            Settings.InitDatabase("UES.TestDatabse");
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
            foreach (var person in people)
            {
                Console.WriteLine($"Student ID={person.StudentID}, name={person.PersonName}.");
            }
            var teachers = (from b in Settings.uesContext.Teachers select b).ToList();
            foreach (var t in teachers)
                Console.WriteLine($"Teacher ID={t.StudentID}, name={t.PersonName}, IsDepartmentAdmin={t.DepartmentAdminAuthority}.");
        }

        public void CounterTest()
        {
            double CountdownTimeLeft = 1000;
            const int CountTimeStep= 100;
            while (CountdownTimeLeft > 0.01)
            {
                double timeToWait = Math.Min(CountdownTimeLeft, CountTimeStep);
                Thread.Sleep((int)(timeToWait * 10));
                CountdownTimeLeft -= timeToWait;
                Console.WriteLine($"Time left: {CountdownTimeLeft}");
            }
            Console.WriteLine($"Time left: {CountdownTimeLeft}");
        }
    }
}
