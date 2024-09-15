INSERT INTO aspnetcoredemo.studentscore (StudentId, `Year`, TotalGrade) VALUES(1, 2022, 30.0);

INSERT INTO aspnetcoredemo.course (Id, Name, Credit, `Type`) VALUES(1, '高等数学', 5.0, 0);
INSERT INTO aspnetcoredemo.course (Id, Name, Credit, `Type`) VALUES(2, '物理学应用', 3.0, 0);
INSERT INTO aspnetcoredemo.course (Id, Name, Credit, `Type`) VALUES(3, 'Web基础', 2.0, 1);
INSERT INTO aspnetcoredemo.course (Id, Name, Credit, `Type`) VALUES(4, '体育基础', 4.0, 0);

INSERT INTO aspnetcoredemo.studentcourse (Id, StudentId, CourseId, CreationTime) VALUES(1, 1, 1, '2023-08-07 00:00:00');
INSERT INTO aspnetcoredemo.studentcourse (Id, StudentId, CourseId, CreationTime) VALUES(2, 1, 3, '2023-08-25 01:16:03');

INSERT INTO aspnetcoredemo.studentscore (Id, StudentId, `Year`, TotalGrade) VALUES(1, 1, 2022, 30.0);

INSERT INTO aspnetcoredemo.sysuser (Id, RoleCode, UserLgnId, UserName, UserPwd, IsLock, Remark, LoginNum, LastLoginDate, RegDate) VALUES(141, 'admin', 'jocoboy', 'Jocoboy', '96e79218965eb72c92a549dd5a330112', 0, '默认密码：111111', NULL, NULL, '2024-09-15 00:00:00');

INSERT INTO aspnetcoredemo.person (Id, Phone, Pwd, RegDate, LoginNum, LastLoginDate) VALUES(1, '17367102860', '96e79218965eb72c92a549dd5a330112', '2024-09-15 00:00:00', 0, NULL);