CREATE TABLE Sessions(
	Id uniqueidentifier not null,
	Unlimited bit not null default(0),
	AccountId int not null unique,
	Login nvarchar(50) not null unique,
	CreateDateTime datetime not null,
	Expires datetime,
	constraint pk_session_id primary key(Id)
)