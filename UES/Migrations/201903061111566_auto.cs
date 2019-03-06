namespace UES.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class auto : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Administrators",
                c => new
                    {
                        AdministratorID = c.Int(nullable: false, identity: true),
                        AdminName = c.String(),
                    })
                .PrimaryKey(t => t.AdministratorID);
            
            CreateTable(
                "dbo.ExamPaperInstances",
                c => new
                    {
                        ExamPaperInstanceID = c.Int(nullable: false, identity: true),
                        ExamDuration = c.Double(nullable: false),
                        CountdownTimeLeft = c.Double(nullable: false),
                        Ongoing = c.Boolean(nullable: false),
                        ExamStartTime = c.DateTime(),
                        StudentScore = c.Double(),
                        StudentSubmitted = c.Boolean(nullable: false),
                        TeacherSubmitted = c.Boolean(nullable: false),
                        AnswerCheckTime = c.DateTime(),
                        Candidate_StudentID = c.Int(),
                        Examiner_StudentID = c.Int(),
                        SuperiorExamPaper_ExamPaperID = c.Int(),
                        Exam_ExamID = c.Int(),
                    })
                .PrimaryKey(t => t.ExamPaperInstanceID)
                .ForeignKey("dbo.Students", t => t.Candidate_StudentID)
                .ForeignKey("dbo.Students", t => t.Examiner_StudentID)
                .ForeignKey("dbo.ExamPapers", t => t.SuperiorExamPaper_ExamPaperID)
                .ForeignKey("dbo.Exams", t => t.Exam_ExamID)
                .Index(t => t.Candidate_StudentID)
                .Index(t => t.Examiner_StudentID)
                .Index(t => t.SuperiorExamPaper_ExamPaperID)
                .Index(t => t.Exam_ExamID);
            
            CreateTable(
                "dbo.StudentAnswerRecords",
                c => new
                    {
                        StudentAnswerRecordID = c.Int(nullable: false, identity: true),
                        Score = c.Double(),
                        Answered = c.Boolean(nullable: false),
                        Checked = c.Boolean(nullable: false),
                        Answer = c.String(),
                        Candidate_StudentID = c.Int(),
                        SuperiorExamPaperInstance_ExamPaperInstanceID = c.Int(),
                    })
                .PrimaryKey(t => t.StudentAnswerRecordID)
                .ForeignKey("dbo.Students", t => t.Candidate_StudentID)
                .ForeignKey("dbo.ExamPaperInstances", t => t.SuperiorExamPaperInstance_ExamPaperInstanceID)
                .Index(t => t.Candidate_StudentID)
                .Index(t => t.SuperiorExamPaperInstance_ExamPaperInstanceID);
            
            CreateTable(
                "dbo.Students",
                c => new
                    {
                        StudentID = c.Int(nullable: false, identity: true),
                        PersonName = c.String(maxLength: 30),
                        TeacherID = c.Int(),
                        DepartmentAdminAuthority = c.Boolean(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                        ExamQuestionSet_ExamQuestionSetID = c.Int(),
                        ExamQuestionSet_ExamQuestionSetID1 = c.Int(),
                        Exam_ExamID = c.Int(),
                        Exam_ExamID1 = c.Int(),
                        Exam_ExamID2 = c.Int(),
                    })
                .PrimaryKey(t => t.StudentID)
                .ForeignKey("dbo.ExamQuestionSets", t => t.ExamQuestionSet_ExamQuestionSetID)
                .ForeignKey("dbo.ExamQuestionSets", t => t.ExamQuestionSet_ExamQuestionSetID1)
                .ForeignKey("dbo.Exams", t => t.Exam_ExamID)
                .ForeignKey("dbo.Exams", t => t.Exam_ExamID1)
                .ForeignKey("dbo.Exams", t => t.Exam_ExamID2)
                .Index(t => t.ExamQuestionSet_ExamQuestionSetID)
                .Index(t => t.ExamQuestionSet_ExamQuestionSetID1)
                .Index(t => t.Exam_ExamID)
                .Index(t => t.Exam_ExamID1)
                .Index(t => t.Exam_ExamID2);
            
            CreateTable(
                "dbo.ExamPapers",
                c => new
                    {
                        ExamPaperID = c.Int(nullable: false, identity: true),
                        ExamPaperName = c.String(),
                        Description = c.String(),
                        IndexWord = c.String(),
                        CreationTime = c.DateTime(nullable: false),
                        LastModifyTime = c.DateTime(nullable: false),
                        ExamDuration = c.Single(nullable: false),
                        Finished = c.Boolean(nullable: false),
                        Creator_StudentID = c.Int(),
                        SuperiorExam_ExamID = c.Int(),
                    })
                .PrimaryKey(t => t.ExamPaperID)
                .ForeignKey("dbo.Students", t => t.Creator_StudentID)
                .ForeignKey("dbo.Exams", t => t.SuperiorExam_ExamID)
                .Index(t => t.Creator_StudentID)
                .Index(t => t.SuperiorExam_ExamID);
            
            CreateTable(
                "dbo.QuestionChooseRecords",
                c => new
                    {
                        QuestionChooseRecordID = c.Int(nullable: false, identity: true),
                        ChooseFinished = c.Boolean(nullable: false),
                        Question_ExamQuestionID = c.Int(),
                        SuperiorExamPaper_ExamPaperID = c.Int(),
                    })
                .PrimaryKey(t => t.QuestionChooseRecordID)
                .ForeignKey("dbo.ExamQuestions", t => t.Question_ExamQuestionID)
                .ForeignKey("dbo.ExamPapers", t => t.SuperiorExamPaper_ExamPaperID)
                .Index(t => t.Question_ExamQuestionID)
                .Index(t => t.SuperiorExamPaper_ExamPaperID);
            
            CreateTable(
                "dbo.ExamQuestions",
                c => new
                    {
                        ExamQuestionID = c.Int(nullable: false, identity: true),
                        ExamQuestionType = c.Int(nullable: false),
                        IndexWord = c.String(),
                        CreationTime = c.DateTime(nullable: false),
                        LastModifyTime = c.DateTime(nullable: false),
                        Explanation = c.String(),
                        Finished = c.Boolean(nullable: false),
                        TotalScore = c.Single(nullable: false),
                        TotalCanonicalScore = c.Single(nullable: false),
                        QuestionTrunk = c.String(),
                        CorrectAnswer = c.String(),
                        QuestionTrunk1 = c.String(),
                        OptionA = c.String(),
                        OptionB = c.String(),
                        OptionC = c.String(),
                        OptionD = c.String(),
                        OptionE = c.String(),
                        CorrectAnswer1 = c.String(),
                        QuestionTrunk2 = c.String(),
                        CorrectAnswer2 = c.Boolean(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                        Creator_StudentID = c.Int(),
                        SuperiorQuestionSet_ExamQuestionSetID = c.Int(),
                    })
                .PrimaryKey(t => t.ExamQuestionID)
                .ForeignKey("dbo.Students", t => t.Creator_StudentID)
                .ForeignKey("dbo.ExamQuestionSets", t => t.SuperiorQuestionSet_ExamQuestionSetID)
                .Index(t => t.Creator_StudentID)
                .Index(t => t.SuperiorQuestionSet_ExamQuestionSetID);
            
            CreateTable(
                "dbo.ExamQuestionSets",
                c => new
                    {
                        ExamQuestionSetID = c.Int(nullable: false, identity: true),
                        QuestionSetName = c.String(),
                        IndexWord = c.String(),
                        LastModifyTime = c.DateTime(nullable: false),
                        Creator_StudentID = c.Int(),
                    })
                .PrimaryKey(t => t.ExamQuestionSetID)
                .ForeignKey("dbo.Students", t => t.Creator_StudentID)
                .Index(t => t.Creator_StudentID);
            
            CreateTable(
                "dbo.Exams",
                c => new
                    {
                        ExamID = c.Int(nullable: false, identity: true),
                        ExamName = c.String(maxLength: 30),
                        Department = c.String(maxLength: 30),
                        IndexWord = c.String(maxLength: 50),
                        Description = c.String(),
                        CreationTime = c.DateTime(nullable: false),
                        LastModifyTime = c.DateTime(nullable: false),
                        AllowSignInTime = c.DateTime(nullable: false),
                        AllowAttendTime = c.DateTime(nullable: false),
                        ExamDuration = c.Double(nullable: false),
                        StudentSubmitDeadline = c.DateTime(nullable: false),
                        TeacherSubmitDeadline = c.DateTime(nullable: false),
                        ScorePublicTime = c.DateTime(nullable: false),
                        ExamPaperGenerationDeadline = c.DateTime(nullable: false),
                        Creator_StudentID = c.Int(),
                        LastOperator_StudentID = c.Int(),
                    })
                .PrimaryKey(t => t.ExamID)
                .ForeignKey("dbo.Students", t => t.Creator_StudentID)
                .ForeignKey("dbo.Students", t => t.LastOperator_StudentID)
                .Index(t => t.Creator_StudentID)
                .Index(t => t.LastOperator_StudentID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ExamPaperInstances", "Exam_ExamID", "dbo.Exams");
            DropForeignKey("dbo.Students", "Exam_ExamID2", "dbo.Exams");
            DropForeignKey("dbo.Exams", "LastOperator_StudentID", "dbo.Students");
            DropForeignKey("dbo.Exams", "Creator_StudentID", "dbo.Students");
            DropForeignKey("dbo.ExamPapers", "SuperiorExam_ExamID", "dbo.Exams");
            DropForeignKey("dbo.Students", "Exam_ExamID1", "dbo.Exams");
            DropForeignKey("dbo.Students", "Exam_ExamID", "dbo.Exams");
            DropForeignKey("dbo.QuestionChooseRecords", "SuperiorExamPaper_ExamPaperID", "dbo.ExamPapers");
            DropForeignKey("dbo.QuestionChooseRecords", "Question_ExamQuestionID", "dbo.ExamQuestions");
            DropForeignKey("dbo.ExamQuestions", "SuperiorQuestionSet_ExamQuestionSetID", "dbo.ExamQuestionSets");
            DropForeignKey("dbo.ExamQuestionSets", "Creator_StudentID", "dbo.Students");
            DropForeignKey("dbo.Students", "ExamQuestionSet_ExamQuestionSetID1", "dbo.ExamQuestionSets");
            DropForeignKey("dbo.Students", "ExamQuestionSet_ExamQuestionSetID", "dbo.ExamQuestionSets");
            DropForeignKey("dbo.ExamQuestions", "Creator_StudentID", "dbo.Students");
            DropForeignKey("dbo.ExamPaperInstances", "SuperiorExamPaper_ExamPaperID", "dbo.ExamPapers");
            DropForeignKey("dbo.ExamPapers", "Creator_StudentID", "dbo.Students");
            DropForeignKey("dbo.ExamPaperInstances", "Examiner_StudentID", "dbo.Students");
            DropForeignKey("dbo.ExamPaperInstances", "Candidate_StudentID", "dbo.Students");
            DropForeignKey("dbo.StudentAnswerRecords", "SuperiorExamPaperInstance_ExamPaperInstanceID", "dbo.ExamPaperInstances");
            DropForeignKey("dbo.StudentAnswerRecords", "Candidate_StudentID", "dbo.Students");
            DropIndex("dbo.Exams", new[] { "LastOperator_StudentID" });
            DropIndex("dbo.Exams", new[] { "Creator_StudentID" });
            DropIndex("dbo.ExamQuestionSets", new[] { "Creator_StudentID" });
            DropIndex("dbo.ExamQuestions", new[] { "SuperiorQuestionSet_ExamQuestionSetID" });
            DropIndex("dbo.ExamQuestions", new[] { "Creator_StudentID" });
            DropIndex("dbo.QuestionChooseRecords", new[] { "SuperiorExamPaper_ExamPaperID" });
            DropIndex("dbo.QuestionChooseRecords", new[] { "Question_ExamQuestionID" });
            DropIndex("dbo.ExamPapers", new[] { "SuperiorExam_ExamID" });
            DropIndex("dbo.ExamPapers", new[] { "Creator_StudentID" });
            DropIndex("dbo.Students", new[] { "Exam_ExamID2" });
            DropIndex("dbo.Students", new[] { "Exam_ExamID1" });
            DropIndex("dbo.Students", new[] { "Exam_ExamID" });
            DropIndex("dbo.Students", new[] { "ExamQuestionSet_ExamQuestionSetID1" });
            DropIndex("dbo.Students", new[] { "ExamQuestionSet_ExamQuestionSetID" });
            DropIndex("dbo.StudentAnswerRecords", new[] { "SuperiorExamPaperInstance_ExamPaperInstanceID" });
            DropIndex("dbo.StudentAnswerRecords", new[] { "Candidate_StudentID" });
            DropIndex("dbo.ExamPaperInstances", new[] { "Exam_ExamID" });
            DropIndex("dbo.ExamPaperInstances", new[] { "SuperiorExamPaper_ExamPaperID" });
            DropIndex("dbo.ExamPaperInstances", new[] { "Examiner_StudentID" });
            DropIndex("dbo.ExamPaperInstances", new[] { "Candidate_StudentID" });
            DropTable("dbo.Exams");
            DropTable("dbo.ExamQuestionSets");
            DropTable("dbo.ExamQuestions");
            DropTable("dbo.QuestionChooseRecords");
            DropTable("dbo.ExamPapers");
            DropTable("dbo.Students");
            DropTable("dbo.StudentAnswerRecords");
            DropTable("dbo.ExamPaperInstances");
            DropTable("dbo.Administrators");
        }
    }
}
