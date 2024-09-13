-- aspnetcoredemo.student definition

CREATE TABLE `student` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Name` varchar(20) NOT NULL COMMENT '姓名',
  `StudentLevel` int NOT NULL COMMENT '年级（0幼儿园 1小学 2中学）',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;


-- aspnetcoredemo.course definition

CREATE TABLE `course` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Name` varchar(100) NOT NULL COMMENT '课程',
  `Credit` float NOT NULL COMMENT '学分',
  `Type` int NOT NULL DEFAULT '0' COMMENT '类型（0基础课，1专业课）',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;


-- aspnetcoredemo.studentcourse definition

CREATE TABLE `studentcourse` (
  `StudentId` int NOT NULL,
  `CourseId` int NOT NULL,
  `CreationTime` datetime NOT NULL COMMENT '选课时间',
  PRIMARY KEY (`StudentId`,`CourseId`),
  KEY `studentcourse_course_FK` (`CourseId`),
  CONSTRAINT `studentcourse_course_FK` FOREIGN KEY (`CourseId`) REFERENCES `course` (`Id`),
  CONSTRAINT `studentcourse_student_FK` FOREIGN KEY (`StudentId`) REFERENCES `student` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;


-- aspnetcoredemo.studentscore definition

CREATE TABLE `studentscore` (
  `StudentId` int NOT NULL,
  `Year` int NOT NULL COMMENT '学年',
  `TotalGrade` float NOT NULL COMMENT '当年获得总学分',
  PRIMARY KEY (`StudentId`,`Year`),
  CONSTRAINT `studentscore_student_FK` FOREIGN KEY (`StudentId`) REFERENCES `student` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;