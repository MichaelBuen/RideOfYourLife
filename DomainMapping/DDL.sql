/*
create database RideOfYourLife;

drop table ReservationAdditional;
drop table Reservation;

drop table TitleLocalization;
drop table Title;

drop table TextResourceLocalization;
drop table TextResource;

drop table NationalityLocalization;
drop table Nationality;

drop table AgeBracketLocalization;
drop table AgeBracket;
*/





create table Title (
	TitleId int not null identity(1,1),
	CreatedOn datetimeoffset not null,

	constraint pk_Title primary key (TitleId)
);


create table TitleLocalization (
	TitleLocalizationId int not null identity(1,1), 

	TitleId int not null,
	LanguageCultureCode varchar(6) not null,

	TitleAbbreviation nvarchar(10) not null,
	TitleDescription nvarchar(100) not null,

	-- Reason for not making composite TitleId+LanguageCultureCode a clustered primray key:
	-- http://www.ienablemuch.com/2012/11/page-splitting-with-clustered-composite.html

	constraint pk_TitleLocalization primary key (TitleLocalizationId),

	constraint uk_TitleLocalization unique (TitleId, LanguageCultureCode),
	constraint fk_TitleLocalization_TitleId foreign key(TitleId) references Title
);


insert into Title(CreatedOn) values(getutcdate());
declare @Mr int = scope_identity();
insert into TitleLocalization(TitleId, LanguageCultureCode, TitleAbbreviation, TitleDescription) values(@Mr, 'en', 'Mr.', 'Mister');
insert into TitleLocalization(TitleId, LanguageCultureCode, TitleAbbreviation, TitleDescription) values(@Mr, 'zh', N'先生', 'Xiansheng');

insert into Title(CreatedOn) values(getutcdate());
declare @Mrs int = scope_identity();
insert into TitleLocalization(TitleId, LanguageCultureCode, TitleAbbreviation, TitleDescription) values(@Mrs, 'en', 'Mrs.', 'Missus');
insert into TitleLocalization(TitleId, LanguageCultureCode, TitleAbbreviation, TitleDescription) values(@Mrs, 'zh', N'太太', 'Taitai');

insert into Title(CreatedOn) values(getutcdate());
declare @Dr int = scope_identity();
insert into TitleLocalization(TitleId, LanguageCultureCode, TitleAbbreviation, TitleDescription) values(@Dr, 'en', 'Dr.', 'Doctor');



go



if exists(select 1 from information_schema.routines where specific_schema = 'dbo' and specific_name = 'GetTitleLocalization' and routine_type = 'FUNCTION')
	drop function dbo.GetTitleLocalization;
GO



create function dbo.GetTitleLocalization(@LanguageCode varchar(6))
returns table -- (TitleId, LanguageCultureCode, TitleAbbreviation, TitleDescription, ActualLanguageCode)
as
return
	with a as
	(
		select 
			TheRank = 
			rank() over (partition by TitleId 
				order by
				case LanguageCultureCode
				when @LanguageCode then 1
				when 'en' then 2
				else 3
				end)				
			, *
			, ActualLanguageCultureCode = LanguageCultureCode 
			
		from dbo.TitleLocalization tl
	)
	select
		-- composite key for ORM..
		a.TitleId, LanguageCultureCode = @LanguageCode
		-- ..composite key
		
		, a.TitleAbbreviation
		, a.TitleDescription
		
		, a.ActualLanguageCultureCode
		
	from a
	where TheRank = 1

GO





/*

select * from dbo.Title;

select * from dbo.TitleLocalization;

select * from dbo.GetTitleLocalization('en');

select * from dbo.GetTitleLocalization('zh');

*/





create table TextResource (
	TextResourceId int not null,
	IsMarkdown bit not null,
	CreatedOn datetimeoffset not null,

	constraint pk_TextResource primary key (TextResourceId)	
);


alter table TextResource
add constraint df_TextResource_IsMarkdown default 0 for IsMarkdown;



create table TextResourceLocalization (
	TextResourceLocalizationId int not null identity(1,1), 

	TextResourceId int not null,
	LanguageCultureCode varchar(6) not null,

	TextResourceValue nvarchar(3500) not null,    

	-- Reason for not making composite TextResourceId+LanguageCultureCode a clustered primray key:
	-- http://www.ienablemuch.com/2012/11/page-splitting-with-clustered-composite.html

	constraint pk_TextResourceLocalization primary key (TextResourceLocalizationId),
	
	constraint uk_TextResourceLocalization unique (TextResourceId, LanguageCultureCode),
	constraint fk_TextResourceLocalization_TextResourceId foreign key(TextResourceId) references TextResource
);




insert into TextResource(TextResourceId, CreatedOn) values(1, getutcdate()); 
insert into TextResourceLocalization(TextResourceId, LanguageCultureCode, TextResourceValue) values(1, 'en', 'PERSONAL INFORMATION');
insert into TextResourceLocalization(TextResourceId, LanguageCultureCode, TextResourceValue) values(1, 'zh', N'个人信息')



insert into TextResource(TextResourceId, CreatedOn) values(2, getutcdate()); 
insert into TextResourceLocalization(TextResourceId, LanguageCultureCode, TextResourceValue) values(2, 'en', 'Compulsory field');
insert into TextResourceLocalization(TextResourceId, LanguageCultureCode, TextResourceValue) values(2, 'zh', N'必填');


insert into TextResource(TextResourceId, CreatedOn) values(3, getutcdate()); 
insert into TextResourceLocalization(TextResourceId, LanguageCultureCode, TextResourceValue) values(3, 'en', 'Title');
insert into TextResourceLocalization(TextResourceId, LanguageCultureCode, TextResourceValue) values(3, 'zh', N'性别');


insert into TextResource(TextResourceId, CreatedOn) values(4, getutcdate()); 
insert into TextResourceLocalization(TextResourceId, LanguageCultureCode, TextResourceValue) values(4, 'en', 'Guest Name');
insert into TextResourceLocalization(TextResourceId, LanguageCultureCode, TextResourceValue) values(4, 'zh', N'客户姓名');


insert into TextResource(TextResourceId, IsMarkdown, CreatedOn) values(5, 1, getutcdate()); 
insert into TextResourceLocalization(TextResourceId, LanguageCultureCode, TextResourceValue) values(5, 'en', 'IMPORTANT!!');
insert into TextResourceLocalization(TextResourceId, LanguageCultureCode, TextResourceValue) values(5, 'zh', N'重要!!');


insert into TextResource(TextResourceId, CreatedOn) values(6, getutcdate()); 
insert into TextResourceLocalization(TextResourceId, LanguageCultureCode, TextResourceValue) values(6, 'en', 'E-mail Address');
insert into TextResourceLocalization(TextResourceId, LanguageCultureCode, TextResourceValue) values(6, 'zh', N'邮箱地址');


insert into TextResource(TextResourceId, CreatedOn) values(7, getutcdate()); 
insert into TextResourceLocalization(TextResourceId, LanguageCultureCode, TextResourceValue) values(7, 'en', 'Contactable Telephone OR Mobile No');
insert into TextResourceLocalization(TextResourceId, LanguageCultureCode, TextResourceValue) values(7, 'zh', N'联系电话或手机号码');


insert into TextResource(TextResourceId, CreatedOn) values(8, getutcdate()); 
insert into TextResourceLocalization(TextResourceId, LanguageCultureCode, TextResourceValue) values(8, 'en', 'Fax No');
insert into TextResourceLocalization(TextResourceId, LanguageCultureCode, TextResourceValue) values(8, 'zh', N'传真号码');


insert into TextResource(TextResourceId, CreatedOn) values(9, getutcdate()); 
insert into TextResourceLocalization(TextResourceId, LanguageCultureCode, TextResourceValue) values(9, 'en', 'Nationality');
insert into TextResourceLocalization(TextResourceId, LanguageCultureCode, TextResourceValue) values(9, 'zh', N'国籍');


insert into TextResource(TextResourceId, CreatedOn) values(10, getutcdate()); 
insert into TextResourceLocalization(TextResourceId, LanguageCultureCode, TextResourceValue) values(10, 'en', 'Date of Ride');
insert into TextResourceLocalization(TextResourceId, LanguageCultureCode, TextResourceValue) values(10, 'zh', N'出发日期');


insert into TextResource(TextResourceId, CreatedOn) values(11, getutcdate()); 
insert into TextResourceLocalization(TextResourceId, LanguageCultureCode, TextResourceValue) values(11, 'en', 'Age');
insert into TextResourceLocalization(TextResourceId, LanguageCultureCode, TextResourceValue) values(11, 'zh', N'年龄');


insert into TextResource(TextResourceId, CreatedOn) values(12, getutcdate()); 
insert into TextResourceLocalization(TextResourceId, LanguageCultureCode, TextResourceValue) values(12, 'en', 'The Philippine TEST');
insert into TextResourceLocalization(TextResourceId, LanguageCultureCode, TextResourceValue) values(12, 'zh', N'菲律宾');


insert into TextResource(TextResourceId, CreatedOn) values(13, getutcdate()); 
insert into TextResourceLocalization(TextResourceId, LanguageCultureCode, TextResourceValue) values(13, 'en', 'RESERVATION FORM');
insert into TextResourceLocalization(TextResourceId, LanguageCultureCode, TextResourceValue) values(13, 'zh', N'预订表格');


insert into TextResource(TextResourceId, CreatedOn) values(14, getutcdate()); 
insert into TextResourceLocalization(TextResourceId, LanguageCultureCode, TextResourceValue) values(14, 'en', 'Terms and Conditions');
insert into TextResourceLocalization(TextResourceId, LanguageCultureCode, TextResourceValue) values(14, 'zh', N'条款及条规');


insert into TextResource(TextResourceId, CreatedOn) values(15, getutcdate()); 
insert into TextResourceLocalization(TextResourceId, LanguageCultureCode, TextResourceValue) values(15, 'en', 'Correspondence Email Address');
insert into TextResourceLocalization(TextResourceId, LanguageCultureCode, TextResourceValue) values(15, 'zh', N'电子邮件地址');


insert into TextResource(TextResourceId, CreatedOn) values(16, getutcdate()); 
insert into TextResourceLocalization(TextResourceId, LanguageCultureCode, TextResourceValue) values(16, 'en', 'Pls furnish complete e-mail address so that our reply could reach you');
insert into TextResourceLocalization(TextResourceId, LanguageCultureCode, TextResourceValue) values(16, 'zh', N'请填写完整的邮件地址，以便我们能够及时回复');


insert into TextResource(TextResourceId, CreatedOn) values(17, getutcdate()); 
insert into TextResourceLocalization(TextResourceId, LanguageCultureCode, TextResourceValue) values(17, 'en', 'Children');
insert into TextResourceLocalization(TextResourceId, LanguageCultureCode, TextResourceValue) values(17, 'zh', N'儿童');


insert into TextResource(TextResourceId, CreatedOn) values(18, getutcdate()); 
insert into TextResourceLocalization(TextResourceId, LanguageCultureCode, TextResourceValue) values(18, 'en', 'Reservation Details');
insert into TextResourceLocalization(TextResourceId, LanguageCultureCode, TextResourceValue) values(18, 'zh', N'预订详细资料');



insert into TextResource(TextResourceId, CreatedOn) values(19, getutcdate()); 
insert into TextResourceLocalization(TextResourceId, LanguageCultureCode, TextResourceValue) values(19, 'en', 'PHILIPPINE TEST');
insert into TextResourceLocalization(TextResourceId, LanguageCultureCode, TextResourceValue) values(19, 'zh', N'菲律宾测试');


insert into TextResource(TextResourceId, CreatedOn) values(20, getutcdate()); 
insert into TextResourceLocalization(TextResourceId, LanguageCultureCode, TextResourceValue) values(20, 'en', 'Add Guest');
insert into TextResourceLocalization(TextResourceId, LanguageCultureCode, TextResourceValue) values(20, 'zh', N'增加');


insert into TextResource(TextResourceId, CreatedOn) values(21, getutcdate()); 
insert into TextResourceLocalization(TextResourceId, LanguageCultureCode, TextResourceValue) values(21, 'en', 'Reserve');
insert into TextResourceLocalization(TextResourceId, LanguageCultureCode, TextResourceValue) values(21, 'zh', N'预订');

insert into TextResource(TextResourceId, CreatedOn) values(22, getutcdate()); 
insert into TextResourceLocalization(TextResourceId, LanguageCultureCode, TextResourceValue) values(22, 'en', 'Required!');
insert into TextResourceLocalization(TextResourceId, LanguageCultureCode, TextResourceValue) values(22, 'zh', N'必填!');


insert into TextResource(TextResourceId, CreatedOn) values(23, getutcdate()); 
insert into TextResourceLocalization(TextResourceId, LanguageCultureCode, TextResourceValue) values(23, 'en', 'Reservation Id');
insert into TextResourceLocalization(TextResourceId, LanguageCultureCode, TextResourceValue) values(23, 'zh', N'预订序号');


insert into TextResource(TextResourceId, CreatedOn) values(24, getutcdate()); 
insert into TextResourceLocalization(TextResourceId, LanguageCultureCode, TextResourceValue) values(24, 'en', 'Make New Reservation');
insert into TextResourceLocalization(TextResourceId, LanguageCultureCode, TextResourceValue) values(24, 'zh', N'增加新预订');


insert into TextResource(TextResourceId, IsMarkdown, CreatedOn) values(25, 1, getutcdate()); 

insert into TextResourceLocalization(TextResourceId, LanguageCultureCode, TextResourceValue) values(25, 'en', 
'+ Voucher is valid for the date of Ride only
+ Voucher is quoted in Philippine Peso
+ Voucher to be exchange for Ride ticket in full upon payment
+ Voucher is non-redeemable in conjunction with other promotion
+ **For reservation of ride timings, please contact Philippine TEST at Mobile# : (63) 928 4547376**
+ Operating Hours : 9am to 6pm on hourly basis except Monday
+ All passenger must be a minimum 1.3 metres in height
+ For passenger safety and comfort, all rides are subject to cancellation by the operator with full refund in the event of adverse weather conditions');

insert into TextResourceLocalization(TextResourceId, LanguageCultureCode, TextResourceValue) values(25, 'zh', 
'+ Lorem ipsum dolor sit amet, consectetuer adipiscing elit. 
+ Aenean commodo ligula eget dolor. Aenean massa. Cum sociis 
+ natoque penatibus et magnis dis parturient montes, nascetur 
+ ridiculus mus. Donec quam felis, ultricies nec, pellentesque
+ **Qeu, pretium quis, sem. Nulla consequat massa quis Enim : (63) 928 4547376**
+ Donec pede justo, fringilla vel, aliquet nec, vulputate eget, arcu. 
+ In enim justo, rhoncus ut, imperdiet a, venenatis vitae, justo. 
Nullam dictum felis eu pede mollis pretium');


insert into TextResource(TextResourceId, IsMarkdown, CreatedOn) values(26, 1,  getutcdate()); 
insert into TextResourceLocalization(TextResourceId, LanguageCultureCode, TextResourceValue) values(26, 'en', 
'If you encounter any returned e-mail OR difficulties sending your booking details through this form, you may send your booking details to our help desk at our main reservation office at e-mail address

*At your services always !*');

insert into TextResourceLocalization(TextResourceId, LanguageCultureCode, TextResourceValue) values(26, 'zh', 
'Nam quam nunc, blandit vel, luctus pulvinar, hendrerit id, lorem. Maecenas nec odio et ante tincidunt tempus. Donec

*Quidquid Latine Dictum Sit, Altum Videtur!*');



go


if exists(select 1 from information_schema.routines where specific_schema = 'dbo' and specific_name = 'GetTextResourceLocalization' and routine_type = 'FUNCTION')
	drop function dbo.GetTextResourceLocalization;
GO



create function dbo.GetTextResourceLocalization(@LanguageCode varchar(6))
returns table -- (TextResourceId, LanguageCultureCode, TextResourceValue, ActualLanguageCode)
as
return
	with a as
	(
		select 
			TheRank = 
			rank() over (partition by TextResourceId 
				order by
				case LanguageCultureCode
				when @LanguageCode then 1
				when 'en' then 2
				else 3
				end)				
			, *
			, ActualLanguageCultureCode = LanguageCultureCode 
			
		from dbo.TextResourceLocalization tl
	)
	select
		-- composite key for ORM..
		a.TextResourceId, LanguageCultureCode = @LanguageCode
		-- ..composite key
		
		, a.TextResourceValue
		
		, a.ActualLanguageCultureCode
		
	from a
	where TheRank = 1

GO



/*

select * from dbo.TextResource;

select * from dbo.TextResourceLocalization;

select * from dbo.GetTextResourceLocalization('en');

select * from dbo.GetTextResourceLocalization('zh');

*/




create table Nationality (
	NationalityId int not null identity(1,1),
	CreatedOn datetimeoffset null,

	constraint pk_Nationality primary key (NationalityId)
);


create table NationalityLocalization (
	NationalityLocalizationId int not null identity(1,1), 

	NationalityId int not null,
	LanguageCultureCode varchar(6) not null,
	
	NationalityName nvarchar(100) null,

	-- Reason for not making composite TitleId+LanguageCultureCode a clustered primray key:
	-- http://www.ienablemuch.com/2012/11/page-splitting-with-clustered-composite.html

	constraint pk_NationalityLocalization primary key (NationalityLocalizationId),

	constraint uk_NationalityLocalization unique (NationalityId, LanguageCultureCode),

	constraint fk_NationalityLocalization_TitleId foreign key(NationalityId) references Nationality
);



go


insert into Nationality(CreatedOn) values(getutcdate());
declare @American int = scope_identity();
insert into NationalityLocalization(NationalityId, LanguageCultureCode, NationalityName) values(@American, 'en', 'American');
insert into NationalityLocalization(NationalityId, LanguageCultureCode, NationalityName) values(@American, 'zh', N'美国');

insert into Nationality(CreatedOn) values(getutcdate());
declare @Canadian int = scope_identity();
insert into NationalityLocalization(NationalityId, LanguageCultureCode, NationalityName) values(@Canadian, 'en', 'Canadian');
insert into NationalityLocalization(NationalityId, LanguageCultureCode, NationalityName) values(@Canadian, 'zh', N'加拿大');

insert into Nationality(CreatedOn) values(getutcdate());
declare @Chinese int = scope_identity();
insert into NationalityLocalization(NationalityId, LanguageCultureCode, NationalityName) values(@Chinese, 'en', 'Chinese');
insert into NationalityLocalization(NationalityId, LanguageCultureCode, NationalityName) values(@Chinese, 'zh', N'中国');

insert into Nationality(CreatedOn) values(getutcdate());
declare @Filipino int = scope_identity();
insert into NationalityLocalization(NationalityId, LanguageCultureCode, NationalityName) values(@Filipino, 'en', 'Filipino');
insert into NationalityLocalization(NationalityId, LanguageCultureCode, NationalityName) values(@Filipino, 'zh', N'菲律宾');


go




if exists(select 1 from information_schema.routines where specific_schema = 'dbo' and specific_name = 'GetNationalityLocalization' and routine_type = 'FUNCTION')
	drop function dbo.GetNationalityLocalization;
GO


create function dbo.GetNationalityLocalization(@LanguageCode varchar(6))
returns table -- (NationalityId, LanguageCultureCode, NationalityName, ActualLanguageCode)
as
return
	with a as
	(
		select 
			TheRank = 
			rank() over (partition by NationalityId 
				order by
				case LanguageCultureCode
				when @LanguageCode then 1
				when 'en' then 2
				else 3
				end)				
			, *
			, ActualLanguageCultureCode = LanguageCultureCode 
			
		from dbo.NationalityLocalization tl
	)
	select
		-- composite key for ORM..
		a.NationalityId, LanguageCultureCode = @LanguageCode
		-- ..composite key
		
		, a.NationalityName
		
		, a.ActualLanguageCultureCode
		
	from a
	where TheRank = 1

GO


/*

select * from dbo.Nationality;

select * from dbo.NationalityLocalization;

select * from dbo.GetNationalityLocalization('en');

select * from dbo.GetNationalityLocalization('zh');

*/



create table AgeBracket
(
	AgeBracketId int not null,

	CreatedOn datetimeoffset not null,
	MinimumAge int null,
	MaximumAge int null,

	constraint pk_AgeBracket primary key (AgeBracketId)
);



create table AgeBracketLocalization (
	AgeBracketLocalizationId int not null identity(1,1), 

	AgeBracketId int not null,
	LanguageCultureCode varchar(6) not null,
	
	AgeBracketDescription nvarchar(50) null,

	-- Reason for not making composite TitleId+LanguageCultureCode a clustered primray key:
	-- http://www.ienablemuch.com/2012/11/page-splitting-with-clustered-composite.html

	constraint pk_AgeBracketLocalization primary key (AgeBracketLocalizationId),

	constraint uk_AgeBracketLocalization unique (AgeBracketId, LanguageCultureCode),

	constraint fk_AgeBracketLocalization_AgeBracketId foreign key(AgeBracketId) references AgeBracket
);


go


declare @adult int = 1;
insert into AgeBracket(AgeBracketId, CreatedOn) values(@adult, getutcdate());
insert into AgeBracketLocalization(AgeBracketId, LanguageCultureCode, AgeBracketDescription) values(@adult, 'en', 'Adult');
insert into AgeBracketLocalization(AgeBracketId, LanguageCultureCode, AgeBracketDescription) values(@adult, 'zh', N'大人');


declare @kid int = 2;
insert into AgeBracket(AgeBracketId, CreatedOn, MinimumAge, MaximumAge) values(@kid, getutcdate(), 1, 10);
insert into AgeBracketLocalization(AgeBracketId, LanguageCultureCode, AgeBracketDescription) values(@kid, 'en', 'Kid');


declare @youngTeenager int = 3;
insert into AgeBracket(AgeBracketId, CreatedOn, MinimumAge, MaximumAge) values(@youngTeenager, getutcdate(), 11, 16);
insert into AgeBracketLocalization(AgeBracketId, LanguageCultureCode, AgeBracketDescription) values(@youngTeenager, 'en', 'Young Teenager');


go


-- update dbo.AgeBracket set MinimumAge= 12345678, MaximumAge=23456781 where AgeBracketId = 3 



if exists(select 1 from information_schema.routines where specific_schema = 'dbo' and specific_name = 'GetAgeBracketLocalization' and routine_type = 'FUNCTION')
	drop function dbo.GetAgeBracketLocalization;
GO


create function dbo.GetAgeBracketLocalization(@LanguageCode varchar(6))
returns table -- (NationalityId, LanguageCultureCode, NationalityName, ActualLanguageCode)
as
return
	with a as
	(
		select 
			TheRank = 
			rank() over (partition by AgeBracketId 
				order by
				case LanguageCultureCode
				when @LanguageCode then 1
				when 'en' then 2
				else 3
				end)				
			, *
			, ActualLanguageCultureCode = LanguageCultureCode 
			
		from dbo.AgeBracketLocalization tl
	)
	select
		-- composite key for ORM..
		a.AgeBracketId, LanguageCultureCode = @LanguageCode
		-- ..composite key
		
		, a.AgeBracketDescription
		
		, a.ActualLanguageCultureCode
		
	from a
	where TheRank = 1

GO


/*

select * from dbo.AgeBracket;

select * from dbo.AgeBracketLocalization;

select * from dbo.GetAgeBracketLocalization('en');

select * from dbo.GetAgeBracketLocalization('zh');

*/



create table Reservation
(
	ReservationId int not null identity(1,1),
	TitleId int not null,	
	GuestName nvarchar(100) not null, -- not required
	EmailAddress nvarchar(100) not null, -- required
	FaxNumber nvarchar(100) not null, -- not required
	ContactNumber nvarchar(100) not null, -- not required
	NationalityId int not null, -- required
	DateOfRide datetime null, -- not required
	CreatedOn datetimeoffset not null,

	constraint pk_Reservation primary key(ReservationId),

	constraint fk_Reservation_TitleId foreign key(TitleId) references Title,
	constraint fk_Reservation_NationalityId foreign key(NationalityId) references Nationality
);

alter table Reservation
add constraint df_Reservation_GuestName default '' for GuestName;

alter table Reservation
add constraint df_Reservation_ContactNumber default '' for ContactNumber;



create table ReservationAdditional
(
	ReservationId int not null, -- we put this on top, to emphasize that this domain model is not an aggregate root
	
	ReservationAdditionalId int not null identity(1,1),

	GuestName nvarchar(100) not null,
	AgeBracketId int not null, 
	NationalityId int not null,

	constraint fk_ReservationAdditional_Reservation foreign key(ReservationId) references Reservation,

	constraint pk_ReservationAdditional primary key (ReservationAdditionalId),
		
	constraint fk_ReservationAdditional_AgeBracketId foreign key(AgeBracketId) references AgeBracket,

	constraint fk_NationalityId foreign key(NationalityId) references Nationality
);



alter table ReservationAdditional
add constraint df_ReservationAdditional_GuestName default '' for GuestName;


go


insert into Reservation(TitleId, GuestName, EmailAddress, ContactNumber, FaxNumber, NationalityId, DateOfRide, CreatedOn)
values (1 /*Mr.*/, 'Batman', 'batman@batcave.com', '5201314', '168', 1/*American*/, '2014-08-25', getutcdate());

declare @r int = scope_identity();

insert into ReservationAdditional(ReservationId, GuestName, AgeBracketId, NationalityId) values
(@r, 'Robin', 3/*young teenager*/, 1),
(@r, 'Catwoman', 1/*adult*/, 2),
(@r, 'Flash', 2/*kid*/, 3);

go


/*
select * from Reservation;
select * from ReservationAdditional;
*/


