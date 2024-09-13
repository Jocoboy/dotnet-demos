INSERT INTO aspnetcoredemo.studentscore (StudentId, `Year`, TotalGrade) VALUES(1, 2022, 30.0);

INSERT INTO aspnetcoredemo.course (Id, Name, Credit, `Type`) VALUES(1, '高等数学', 5.0, 0);
INSERT INTO aspnetcoredemo.course (Id, Name, Credit, `Type`) VALUES(2, '物理学应用', 3.0, 0);
INSERT INTO aspnetcoredemo.course (Id, Name, Credit, `Type`) VALUES(3, 'Web基础', 2.0, 1);
INSERT INTO aspnetcoredemo.course (Id, Name, Credit, `Type`) VALUES(4, '体育基础', 4.0, 0);

INSERT INTO aspnetcoredemo.studentcourse (StudentId, CourseId, CreationTime) VALUES(1, 1, '2023-08-07 00:00:00');
INSERT INTO aspnetcoredemo.studentcourse (StudentId, CourseId, CreationTime) VALUES(1, 3, '2023-08-25 01:17:24');

INSERT INTO aspnetcoredemo.studentscore (StudentId, `Year`, TotalGrade) VALUES(1, 2022, 30.0);