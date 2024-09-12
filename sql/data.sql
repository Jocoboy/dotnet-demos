INSERT INTO public."AbpPermissionGroups" ("Id", "Name", "DisplayName", "ExtraProperties") VALUES('3a14e455-241c-c71d-511c-d7fc9dd8a289'::uuid, 'ABPDemo', 'F:ABPDemo', '{}');

INSERT INTO public."AbpPermissions" ("Id", "GroupName", "Name", "ParentName", "DisplayName", "IsEnabled", "MultiTenancySide", "Providers", "StateCheckers", "ExtraProperties") VALUES('3a14ef14-8561-1b60-cd40-d035a04e2ca5'::uuid, 'ABPDemo', 'ABPDemo.SystemSetting', NULL, 'L:Default,系统设置（账号管理）', true, 3, NULL, NULL, '{}');

INSERT INTO public."AbpSettingDefinitions" ("Id", "Name", "DisplayName", "Description", "DefaultValue", "IsVisibleToClients", "Providers", "IsInherited", "IsEncrypted", "ExtraProperties") VALUES('3a14f304-5f55-5b25-d79f-afcc1f9f1265'::uuid, 'ABPDemo.ResetPassword', 'F:ABPDemo.ResetPassword', NULL, '111111', false, NULL, true, false, '{}');

--↑ ABP auto-generated. Delete it if you build and run your project first.↑--

INSERT INTO public."AbpSettings" ("Id", "Name", "Value", "ProviderName", "ProviderKey") VALUES('3a14f6cf-609c-cca2-064e-894fd05f06ae'::uuid, 'ABPDemo.ResetPassword', '123456', 'G', NULL);

INSERT INTO public."AbpPermissionGrants" ("Id", "TenantId", "Name", "ProviderName", "ProviderKey") VALUES('3a14ef19-3f51-3944-82da-7e061e4b6301'::uuid, NULL, 'ABPDemo.SystemSetting', 'U', '3a14ef19-3a3a-ca37-f749-240487c2fa3e');

INSERT INTO public."AbpUsers" ("Id", "TenantId", "UserName", "NormalizedUserName", "Name", "Surname", "Email", "NormalizedEmail", "EmailConfirmed", "PasswordHash", "SecurityStamp", "IsExternal", "PhoneNumber", "PhoneNumberConfirmed", "IsActive", "TwoFactorEnabled", "LockoutEnd", "LockoutEnabled", "AccessFailedCount", "ShouldChangePasswordOnNextLogin", "EntityVersion", "LastPasswordChangeTime", "ExtraProperties", "ConcurrencyStamp", "CreationTime", "CreatorId", "LastModificationTime", "LastModifierId", "IsDeleted", "DeleterId", "DeletionTime", "Type") VALUES('3a14ef19-3a3a-ca37-f749-240487c2fa3e'::uuid, NULL, 'Jocoboy', 'JOCOBOY', 'jocoboy', NULL, '', '', false, 'AQAAAAIAAYagAAAAEL77+4oV57pGk/WTgTHy7d9VfzPVqMuwyShpyv694vpXLVN1+Y8SY1g2kSeVY9u4sA==', 'RZBXIFC3WM54IN7V3T7G463J624CLFJE', false, NULL, false, true, false, NULL, true, 0, false, 1, '2024-09-11 01:12:44.968', '{}', 'f6cf944f12a94c2c8c27cf88dc7ef48c', '2024-09-11 01:12:45.508', NULL, '2024-09-11 01:20:08.345', NULL, false, NULL, NULL, 0);
INSERT INTO public."AbpUsers" ("Id", "TenantId", "UserName", "NormalizedUserName", "Name", "Surname", "Email", "NormalizedEmail", "EmailConfirmed", "PasswordHash", "SecurityStamp", "IsExternal", "PhoneNumber", "PhoneNumberConfirmed", "IsActive", "TwoFactorEnabled", "LockoutEnd", "LockoutEnabled", "AccessFailedCount", "ShouldChangePasswordOnNextLogin", "EntityVersion", "LastPasswordChangeTime", "ExtraProperties", "ConcurrencyStamp", "CreationTime", "CreatorId", "LastModificationTime", "LastModifierId", "IsDeleted", "DeleterId", "DeletionTime", "Type") VALUES('3a14e47e-35b1-b4e9-d5b0-8abf46c47e60'::uuid, NULL, 'admin', 'ADMIN', 'admin', NULL, 'admin@abp.io', 'ADMIN@ABP.IO', false, 'AQAAAAIAAYagAAAAEOk2TWcK8ZV3CrpFGg1HakwHDraiHAx4zEG9V2lyfjaEHisHKFAXwcFmoSpAZAUURQ==', 'J7ZVIPVE6HDWYF45GMTJUFBH4CME6C2T', false, NULL, false, true, false, NULL, true, 0, false, 4, '2024-09-11 01:22:01.889', '{}', 'adec7a92e3714dd0809e34a5f29ce69b', '2024-09-08 23:47:13.891', NULL, '2024-09-11 01:22:01.896', NULL, false, NULL, NULL, 0);

INSERT INTO public."AbpRoles" ("Id", "TenantId", "Name", "NormalizedName", "IsDefault", "IsStatic", "IsPublic", "EntityVersion", "ExtraProperties", "ConcurrencyStamp") VALUES('3a14e47e-39ea-fa3d-7d29-712738513bf4'::uuid, NULL, 'admin', 'ADMIN', false, true, true, 2, '{}', '5ad0ff804cea4efb96fa2267f8e0cd2d');

INSERT INTO public."AbpUserRoles" ("UserId", "RoleId", "TenantId") VALUES('3a14e47e-35b1-b4e9-d5b0-8abf46c47e60'::uuid, '3a14e47e-39ea-fa3d-7d29-712738513bf4'::uuid, NULL);


--↑ ABP Part End ↑--

--↓ Business Part ↓--

INSERT INTO public."Courses" ("Id", "Name", "Credit", "Type") VALUES('7e0d8ffa-91de-4f50-8b17-110eca932db5'::uuid, '高等数学', 5.0, 0);
INSERT INTO public."Courses" ("Id", "Name", "Credit", "Type") VALUES('8bc046e0-3bb6-4042-b164-b48969650a48'::uuid, '物理学应用', 3.0, 0);
INSERT INTO public."Courses" ("Id", "Name", "Credit", "Type") VALUES('3276be35-e01b-410c-a052-d9e3bdbb880b'::uuid, 'Web基础', 2.0, 1);
INSERT INTO public."Courses" ("Id", "Name", "Credit", "Type") VALUES('021b35f0-0f86-4086-af58-3d890f7b09c1'::uuid, '体育基础', 4.0, 0);

INSERT INTO public."Students" ("Id", "Name", "StudentLevel") VALUES('e95095d9-4eee-4f34-a3f4-dd12f2d5d8b7'::uuid, '张飞', 0);

INSERT INTO public."StudentCourses" ("StudentId", "CourseId", "CreationTime") VALUES('e95095d9-4eee-4f34-a3f4-dd12f2d5d8b7'::uuid, '7e0d8ffa-91de-4f50-8b17-110eca932db5'::uuid, '2023-08-07 00:00:00.000');
INSERT INTO public."StudentCourses" ("StudentId", "CourseId", "CreationTime") VALUES('e95095d9-4eee-4f34-a3f4-dd12f2d5d8b7'::uuid, '3276be35-e01b-410c-a052-d9e3bdbb880b'::uuid, '2023-08-25 01:17:24.837');

INSERT INTO public."StudentScores" ("StudentId", "Year", "TotalGrade") VALUES('e95095d9-4eee-4f34-a3f4-dd12f2d5d8b7'::uuid, 2022, 30.0);