-- public."AbpClaimTypes" definition

-- Drop table

-- DROP TABLE public."AbpClaimTypes";

CREATE TABLE public."AbpClaimTypes" (
	"Id" uuid NOT NULL,
	"Name" varchar(256) NOT NULL,
	"Required" bool NOT NULL,
	"IsStatic" bool NOT NULL,
	"Regex" varchar(512) NULL,
	"RegexDescription" varchar(128) NULL,
	"Description" varchar(256) NULL,
	"ValueType" int4 NOT NULL,
	"ExtraProperties" text NOT NULL,
	"ConcurrencyStamp" varchar(40) NOT NULL,
	CONSTRAINT "PK_AbpClaimTypes" PRIMARY KEY ("Id")
);


-- public."AbpLinkUsers" definition

-- Drop table

-- DROP TABLE public."AbpLinkUsers";

CREATE TABLE public."AbpLinkUsers" (
	"Id" uuid NOT NULL,
	"SourceUserId" uuid NOT NULL,
	"SourceTenantId" uuid NULL,
	"TargetUserId" uuid NOT NULL,
	"TargetTenantId" uuid NULL,
	CONSTRAINT "PK_AbpLinkUsers" PRIMARY KEY ("Id")
);
CREATE UNIQUE INDEX "IX_AbpLinkUsers_SourceUserId_SourceTenantId_TargetUserId_Targe~" ON public."AbpLinkUsers" USING btree ("SourceUserId", "SourceTenantId", "TargetUserId", "TargetTenantId");


-- public."AbpPermissionGrants" definition

-- Drop table

-- DROP TABLE public."AbpPermissionGrants";

CREATE TABLE public."AbpPermissionGrants" (
	"Id" uuid NOT NULL,
	"TenantId" uuid NULL,
	"Name" varchar(128) NOT NULL,
	"ProviderName" varchar(64) NOT NULL,
	"ProviderKey" varchar(64) NOT NULL,
	CONSTRAINT "PK_AbpPermissionGrants" PRIMARY KEY ("Id")
);
CREATE UNIQUE INDEX "IX_AbpPermissionGrants_TenantId_Name_ProviderName_ProviderKey" ON public."AbpPermissionGrants" USING btree ("TenantId", "Name", "ProviderName", "ProviderKey");


-- public."AbpPermissionGroups" definition

-- Drop table

-- DROP TABLE public."AbpPermissionGroups";

CREATE TABLE public."AbpPermissionGroups" (
	"Id" uuid NOT NULL,
	"Name" varchar(128) NOT NULL,
	"DisplayName" varchar(256) NOT NULL,
	"ExtraProperties" text NULL,
	CONSTRAINT "PK_AbpPermissionGroups" PRIMARY KEY ("Id")
);
CREATE UNIQUE INDEX "IX_AbpPermissionGroups_Name" ON public."AbpPermissionGroups" USING btree ("Name");


-- public."AbpPermissions" definition

-- Drop table

-- DROP TABLE public."AbpPermissions";

CREATE TABLE public."AbpPermissions" (
	"Id" uuid NOT NULL,
	"GroupName" varchar(128) NOT NULL,
	"Name" varchar(128) NOT NULL,
	"ParentName" varchar(128) NULL,
	"DisplayName" varchar(256) NOT NULL,
	"IsEnabled" bool NOT NULL,
	"MultiTenancySide" int2 NOT NULL,
	"Providers" varchar(128) NULL,
	"StateCheckers" varchar(256) NULL,
	"ExtraProperties" text NULL,
	CONSTRAINT "PK_AbpPermissions" PRIMARY KEY ("Id")
);
CREATE INDEX "IX_AbpPermissions_GroupName" ON public."AbpPermissions" USING btree ("GroupName");
CREATE UNIQUE INDEX "IX_AbpPermissions_Name" ON public."AbpPermissions" USING btree ("Name");


-- public."AbpRoles" definition

-- Drop table

-- DROP TABLE public."AbpRoles";

CREATE TABLE public."AbpRoles" (
	"Id" uuid NOT NULL,
	"TenantId" uuid NULL,
	"Name" varchar(256) NOT NULL,
	"NormalizedName" varchar(256) NOT NULL,
	"IsDefault" bool NOT NULL,
	"IsStatic" bool NOT NULL,
	"IsPublic" bool NOT NULL,
	"EntityVersion" int4 NOT NULL,
	"ExtraProperties" text NOT NULL,
	"ConcurrencyStamp" varchar(40) NOT NULL,
	CONSTRAINT "PK_AbpRoles" PRIMARY KEY ("Id")
);
CREATE INDEX "IX_AbpRoles_NormalizedName" ON public."AbpRoles" USING btree ("NormalizedName");


-- public."AbpSecurityLogs" definition

-- Drop table

-- DROP TABLE public."AbpSecurityLogs";

CREATE TABLE public."AbpSecurityLogs" (
	"Id" uuid NOT NULL,
	"TenantId" uuid NULL,
	"ApplicationName" varchar(96) NULL,
	"Identity" varchar(96) NULL,
	"Action" varchar(96) NULL,
	"UserId" uuid NULL,
	"UserName" varchar(256) NULL,
	"TenantName" varchar(64) NULL,
	"ClientId" varchar(64) NULL,
	"CorrelationId" varchar(64) NULL,
	"ClientIpAddress" varchar(64) NULL,
	"BrowserInfo" varchar(512) NULL,
	"CreationTime" timestamptz NOT NULL,
	"ExtraProperties" text NOT NULL,
	"ConcurrencyStamp" varchar(40) NOT NULL,
	CONSTRAINT "PK_AbpSecurityLogs" PRIMARY KEY ("Id")
);
CREATE INDEX "IX_AbpSecurityLogs_TenantId_Action" ON public."AbpSecurityLogs" USING btree ("TenantId", "Action");
CREATE INDEX "IX_AbpSecurityLogs_TenantId_ApplicationName" ON public."AbpSecurityLogs" USING btree ("TenantId", "ApplicationName");
CREATE INDEX "IX_AbpSecurityLogs_TenantId_Identity" ON public."AbpSecurityLogs" USING btree ("TenantId", "Identity");
CREATE INDEX "IX_AbpSecurityLogs_TenantId_UserId" ON public."AbpSecurityLogs" USING btree ("TenantId", "UserId");


-- public."AbpSessions" definition

-- Drop table

-- DROP TABLE public."AbpSessions";

CREATE TABLE public."AbpSessions" (
	"Id" uuid NOT NULL,
	"SessionId" varchar(128) NOT NULL,
	"Device" varchar(64) NOT NULL,
	"DeviceInfo" varchar(64) NULL,
	"TenantId" uuid NULL,
	"UserId" uuid NOT NULL,
	"ClientId" varchar(64) NULL,
	"IpAddresses" varchar(256) NULL,
	"SignedIn" timestamptz NOT NULL,
	"LastAccessed" timestamptz NULL,
	CONSTRAINT "PK_AbpSessions" PRIMARY KEY ("Id")
);
CREATE INDEX "IX_AbpSessions_Device" ON public."AbpSessions" USING btree ("Device");
CREATE INDEX "IX_AbpSessions_SessionId" ON public."AbpSessions" USING btree ("SessionId");
CREATE INDEX "IX_AbpSessions_TenantId_UserId" ON public."AbpSessions" USING btree ("TenantId", "UserId");


-- public."AbpSettingDefinitions" definition

-- Drop table

-- DROP TABLE public."AbpSettingDefinitions";

CREATE TABLE public."AbpSettingDefinitions" (
	"Id" uuid NOT NULL,
	"Name" varchar(128) NOT NULL,
	"DisplayName" varchar(256) NOT NULL,
	"Description" varchar(512) NULL,
	"DefaultValue" varchar(2048) NULL,
	"IsVisibleToClients" bool NOT NULL,
	"Providers" varchar(1024) NULL,
	"IsInherited" bool NOT NULL,
	"IsEncrypted" bool NOT NULL,
	"ExtraProperties" text NULL,
	CONSTRAINT "PK_AbpSettingDefinitions" PRIMARY KEY ("Id")
);
CREATE UNIQUE INDEX "IX_AbpSettingDefinitions_Name" ON public."AbpSettingDefinitions" USING btree ("Name");


-- public."AbpSettings" definition

-- Drop table

-- DROP TABLE public."AbpSettings";

CREATE TABLE public."AbpSettings" (
	"Id" uuid NOT NULL,
	"Name" varchar(128) NOT NULL,
	"Value" varchar(2048) NOT NULL,
	"ProviderName" varchar(64) NULL,
	"ProviderKey" varchar(64) NULL,
	CONSTRAINT "PK_AbpSettings" PRIMARY KEY ("Id")
);
CREATE UNIQUE INDEX "IX_AbpSettings_Name_ProviderName_ProviderKey" ON public."AbpSettings" USING btree ("Name", "ProviderName", "ProviderKey");


-- public."AbpUserDelegations" definition

-- Drop table

-- DROP TABLE public."AbpUserDelegations";

CREATE TABLE public."AbpUserDelegations" (
	"Id" uuid NOT NULL,
	"TenantId" uuid NULL,
	"SourceUserId" uuid NOT NULL,
	"TargetUserId" uuid NOT NULL,
	"StartTime" timestamptz NOT NULL,
	"EndTime" timestamptz NOT NULL,
	CONSTRAINT "PK_AbpUserDelegations" PRIMARY KEY ("Id")
);


-- public."AbpUsers" definition

-- Drop table

-- DROP TABLE public."AbpUsers";

CREATE TABLE public."AbpUsers" (
	"Id" uuid NOT NULL,
	"TenantId" uuid NULL,
	"UserName" varchar(256) NOT NULL,
	"NormalizedUserName" varchar(256) NOT NULL,
	"Name" varchar(64) NULL,
	"Surname" varchar(64) NULL,
	"Email" varchar(256) NOT NULL,
	"NormalizedEmail" varchar(256) NOT NULL,
	"EmailConfirmed" bool DEFAULT false NOT NULL,
	"PasswordHash" varchar(256) NULL,
	"SecurityStamp" varchar(256) NOT NULL,
	"IsExternal" bool DEFAULT false NOT NULL,
	"PhoneNumber" varchar(16) NULL,
	"PhoneNumberConfirmed" bool DEFAULT false NOT NULL,
	"IsActive" bool NOT NULL,
	"TwoFactorEnabled" bool DEFAULT false NOT NULL,
	"LockoutEnd" timestamptz NULL,
	"LockoutEnabled" bool DEFAULT false NOT NULL,
	"AccessFailedCount" int4 DEFAULT 0 NOT NULL,
	"ShouldChangePasswordOnNextLogin" bool NOT NULL,
	"EntityVersion" int4 NOT NULL,
	"LastPasswordChangeTime" timestamptz NULL,
	"ExtraProperties" text NOT NULL,
	"ConcurrencyStamp" varchar(40) NOT NULL,
	"CreationTime" timestamptz NOT NULL,
	"CreatorId" uuid NULL,
	"LastModificationTime" timestamptz NULL,
	"LastModifierId" uuid NULL,
	"IsDeleted" bool DEFAULT false NOT NULL,
	"DeleterId" uuid NULL,
	"DeletionTime" timestamptz NULL,
	"Type" int4 DEFAULT 0 NULL, -- 用户类型（0 Admin)
	CONSTRAINT "PK_AbpUsers" PRIMARY KEY ("Id")
);
CREATE INDEX "IX_AbpUsers_Email" ON public."AbpUsers" USING btree ("Email");
CREATE INDEX "IX_AbpUsers_NormalizedEmail" ON public."AbpUsers" USING btree ("NormalizedEmail");
CREATE INDEX "IX_AbpUsers_NormalizedUserName" ON public."AbpUsers" USING btree ("NormalizedUserName");
CREATE INDEX "IX_AbpUsers_UserName" ON public."AbpUsers" USING btree ("UserName");

-- Column comments

COMMENT ON COLUMN public."AbpUsers"."Type" IS '用户类型（0 Admin)';


-- public."AbpOrganizationUnits" definition

-- Drop table

-- DROP TABLE public."AbpOrganizationUnits";

CREATE TABLE public."AbpOrganizationUnits" (
	"Id" uuid NOT NULL,
	"TenantId" uuid NULL,
	"ParentId" uuid NULL,
	"Code" varchar(95) NOT NULL,
	"DisplayName" varchar(128) NOT NULL,
	"EntityVersion" int4 NOT NULL,
	"ExtraProperties" text NOT NULL,
	"ConcurrencyStamp" varchar(40) NOT NULL,
	"CreationTime" timestamptz NOT NULL,
	"CreatorId" uuid NULL,
	"LastModificationTime" timestamptz NULL,
	"LastModifierId" uuid NULL,
	"IsDeleted" bool DEFAULT false NOT NULL,
	"DeleterId" uuid NULL,
	"DeletionTime" timestamptz NULL,
	CONSTRAINT "PK_AbpOrganizationUnits" PRIMARY KEY ("Id"),
	CONSTRAINT "FK_AbpOrganizationUnits_AbpOrganizationUnits_ParentId" FOREIGN KEY ("ParentId") REFERENCES public."AbpOrganizationUnits"("Id")
);
CREATE INDEX "IX_AbpOrganizationUnits_Code" ON public."AbpOrganizationUnits" USING btree ("Code");
CREATE INDEX "IX_AbpOrganizationUnits_ParentId" ON public."AbpOrganizationUnits" USING btree ("ParentId");


-- public."AbpRoleClaims" definition

-- Drop table

-- DROP TABLE public."AbpRoleClaims";

CREATE TABLE public."AbpRoleClaims" (
	"Id" uuid NOT NULL,
	"RoleId" uuid NOT NULL,
	"TenantId" uuid NULL,
	"ClaimType" varchar(256) NOT NULL,
	"ClaimValue" varchar(1024) NULL,
	CONSTRAINT "PK_AbpRoleClaims" PRIMARY KEY ("Id"),
	CONSTRAINT "FK_AbpRoleClaims_AbpRoles_RoleId" FOREIGN KEY ("RoleId") REFERENCES public."AbpRoles"("Id") ON DELETE CASCADE
);
CREATE INDEX "IX_AbpRoleClaims_RoleId" ON public."AbpRoleClaims" USING btree ("RoleId");


-- public."AbpUserClaims" definition

-- Drop table

-- DROP TABLE public."AbpUserClaims";

CREATE TABLE public."AbpUserClaims" (
	"Id" uuid NOT NULL,
	"UserId" uuid NOT NULL,
	"TenantId" uuid NULL,
	"ClaimType" varchar(256) NOT NULL,
	"ClaimValue" varchar(1024) NULL,
	CONSTRAINT "PK_AbpUserClaims" PRIMARY KEY ("Id"),
	CONSTRAINT "FK_AbpUserClaims_AbpUsers_UserId" FOREIGN KEY ("UserId") REFERENCES public."AbpUsers"("Id") ON DELETE CASCADE
);
CREATE INDEX "IX_AbpUserClaims_UserId" ON public."AbpUserClaims" USING btree ("UserId");


-- public."AbpUserLogins" definition

-- Drop table

-- DROP TABLE public."AbpUserLogins";

CREATE TABLE public."AbpUserLogins" (
	"UserId" uuid NOT NULL,
	"LoginProvider" varchar(64) NOT NULL,
	"TenantId" uuid NULL,
	"ProviderKey" varchar(196) NOT NULL,
	"ProviderDisplayName" varchar(128) NULL,
	CONSTRAINT "PK_AbpUserLogins" PRIMARY KEY ("UserId", "LoginProvider"),
	CONSTRAINT "FK_AbpUserLogins_AbpUsers_UserId" FOREIGN KEY ("UserId") REFERENCES public."AbpUsers"("Id") ON DELETE CASCADE
);
CREATE INDEX "IX_AbpUserLogins_LoginProvider_ProviderKey" ON public."AbpUserLogins" USING btree ("LoginProvider", "ProviderKey");


-- public."AbpUserOrganizationUnits" definition

-- Drop table

-- DROP TABLE public."AbpUserOrganizationUnits";

CREATE TABLE public."AbpUserOrganizationUnits" (
	"UserId" uuid NOT NULL,
	"OrganizationUnitId" uuid NOT NULL,
	"TenantId" uuid NULL,
	"CreationTime" timestamptz NOT NULL,
	"CreatorId" uuid NULL,
	CONSTRAINT "PK_AbpUserOrganizationUnits" PRIMARY KEY ("OrganizationUnitId", "UserId"),
	CONSTRAINT "FK_AbpUserOrganizationUnits_AbpOrganizationUnits_OrganizationU~" FOREIGN KEY ("OrganizationUnitId") REFERENCES public."AbpOrganizationUnits"("Id") ON DELETE CASCADE,
	CONSTRAINT "FK_AbpUserOrganizationUnits_AbpUsers_UserId" FOREIGN KEY ("UserId") REFERENCES public."AbpUsers"("Id") ON DELETE CASCADE
);
CREATE INDEX "IX_AbpUserOrganizationUnits_UserId_OrganizationUnitId" ON public."AbpUserOrganizationUnits" USING btree ("UserId", "OrganizationUnitId");


-- public."AbpUserRoles" definition

-- Drop table

-- DROP TABLE public."AbpUserRoles";

CREATE TABLE public."AbpUserRoles" (
	"UserId" uuid NOT NULL,
	"RoleId" uuid NOT NULL,
	"TenantId" uuid NULL,
	CONSTRAINT "PK_AbpUserRoles" PRIMARY KEY ("UserId", "RoleId"),
	CONSTRAINT "FK_AbpUserRoles_AbpRoles_RoleId" FOREIGN KEY ("RoleId") REFERENCES public."AbpRoles"("Id") ON DELETE CASCADE,
	CONSTRAINT "FK_AbpUserRoles_AbpUsers_UserId" FOREIGN KEY ("UserId") REFERENCES public."AbpUsers"("Id") ON DELETE CASCADE
);
CREATE INDEX "IX_AbpUserRoles_RoleId_UserId" ON public."AbpUserRoles" USING btree ("RoleId", "UserId");


-- public."AbpUserTokens" definition

-- Drop table

-- DROP TABLE public."AbpUserTokens";

CREATE TABLE public."AbpUserTokens" (
	"UserId" uuid NOT NULL,
	"LoginProvider" varchar(64) NOT NULL,
	"Name" varchar(128) NOT NULL,
	"TenantId" uuid NULL,
	"Value" text NULL,
	CONSTRAINT "PK_AbpUserTokens" PRIMARY KEY ("UserId", "LoginProvider", "Name"),
	CONSTRAINT "FK_AbpUserTokens_AbpUsers_UserId" FOREIGN KEY ("UserId") REFERENCES public."AbpUsers"("Id") ON DELETE CASCADE
);


-- public."AbpOrganizationUnitRoles" definition

-- Drop table

-- DROP TABLE public."AbpOrganizationUnitRoles";

CREATE TABLE public."AbpOrganizationUnitRoles" (
	"RoleId" uuid NOT NULL,
	"OrganizationUnitId" uuid NOT NULL,
	"TenantId" uuid NULL,
	"CreationTime" timestamptz NOT NULL,
	"CreatorId" uuid NULL,
	CONSTRAINT "PK_AbpOrganizationUnitRoles" PRIMARY KEY ("OrganizationUnitId", "RoleId"),
	CONSTRAINT "FK_AbpOrganizationUnitRoles_AbpOrganizationUnits_OrganizationU~" FOREIGN KEY ("OrganizationUnitId") REFERENCES public."AbpOrganizationUnits"("Id") ON DELETE CASCADE,
	CONSTRAINT "FK_AbpOrganizationUnitRoles_AbpRoles_RoleId" FOREIGN KEY ("RoleId") REFERENCES public."AbpRoles"("Id") ON DELETE CASCADE
);
CREATE INDEX "IX_AbpOrganizationUnitRoles_RoleId_OrganizationUnitId" ON public."AbpOrganizationUnitRoles" USING btree ("RoleId", "OrganizationUnitId");

--↑ ABP Part End ↑--

--↓ Business Part ↓--

-- public."Courses" definition

-- Drop table

-- DROP TABLE public."Courses";

CREATE TABLE public."Courses" (
	"Id" uuid NOT NULL,
	"Name" varchar(100) NOT NULL, -- 课程名
	"Credit" float4 NOT NULL, -- 学分
	"Type" int4 DEFAULT 0 NOT NULL, -- 类型（0基础课，1专业课）
	CONSTRAINT "PK_Courses" PRIMARY KEY ("Id")
);

-- Column comments

COMMENT ON COLUMN public."Courses"."Name" IS '课程名';
COMMENT ON COLUMN public."Courses"."Credit" IS '学分';
COMMENT ON COLUMN public."Courses"."Type" IS '类型（0基础课，1专业课）';


-- public."Students" definition

-- Drop table

-- DROP TABLE public."Students";

CREATE TABLE public."Students" (
	"Id" uuid NOT NULL,
	"Name" varchar(20) NOT NULL, -- 姓名
	"StudentLevel" int4 NOT NULL, -- 年级（0幼儿园 1小学 2中学）
	CONSTRAINT "PK_Student" PRIMARY KEY ("Id")
);

-- Column comments

COMMENT ON COLUMN public."Students"."Name" IS '姓名';
COMMENT ON COLUMN public."Students"."StudentLevel" IS '年级（0幼儿园 1小学 2中学）';


-- public."StudentCourses" definition

-- Drop table

-- DROP TABLE public."StudentCourses";

CREATE TABLE public."StudentCourses" (
	"StudentId" uuid NOT NULL,
	"CourseId" uuid NOT NULL,
	"CreationTime" timestamptz NOT NULL, -- 选课时间
	CONSTRAINT "PK_StudentCourses" PRIMARY KEY ("StudentId", "CourseId"),
	CONSTRAINT "FK_Courses_Id" FOREIGN KEY ("CourseId") REFERENCES public."Courses"("Id"),
	CONSTRAINT "FK_Students_Id" FOREIGN KEY ("StudentId") REFERENCES public."Students"("Id") ON DELETE CASCADE
);

-- Column comments

COMMENT ON COLUMN public."StudentCourses"."CreationTime" IS '选课时间';


-- public."StudentScores" definition

-- Drop table

-- DROP TABLE public."StudentScores";

CREATE TABLE public."StudentScores" (
	"StudentId" uuid NOT NULL,
	"Year" int4 NOT NULL, -- 学年
	"TotalGrade" float4 NOT NULL, -- 当年获得总学分
	CONSTRAINT "PK_StudentScores" PRIMARY KEY ("StudentId", "Year"),
	CONSTRAINT "FK_StudentScores_Student_StudentId" FOREIGN KEY ("StudentId") REFERENCES public."Students"("Id") ON DELETE CASCADE
);

-- Column comments

COMMENT ON COLUMN public."StudentScores"."Year" IS '学年';
COMMENT ON COLUMN public."StudentScores"."TotalGrade" IS '当年获得总学分';