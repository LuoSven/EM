select a.ApplyId as Id,b.UserName,c.Degree as DegreeId ,c.Major,c.SchoolName,d.Position,(e.ContactEmail+','+f.LoginEmail) as Emails from TB_S_Position a
join TB_S_Account b on a.UserId=b.UserId
join TB_Stu_Education c on a.UserId=c.UserId
join TB_Position_Element d on a.PositionId=d.PositionId
join TB_Position_Detail e on a.PositionId=e.PositionId 
join TB_Enterprise_Account f on f.EnterpriseId=a.EnterpriseId
where a.ApplyId=62499 and e.IsSendNewResume=1
update TB_Enterprise_Account set LoginEmail='Mirkmf110@1653.com'   where EnterpriseId=1