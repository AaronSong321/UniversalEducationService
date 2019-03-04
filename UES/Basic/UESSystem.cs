using HIT.UES.Exam;
using HIT.UES.Login;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIT.UES
{
    public class UESSystem: UESObject
    {
        public LoginSubsystem LoginSubsystem;
        public ExamSubsystem ExamSubsystem;
    }
}
