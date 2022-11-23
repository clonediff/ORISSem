CREATE TABLE Accounts(
	Id int not null identity(1, 1),
	Login varchar(50) not null unique,
	Email varchar(65) not null unique,
	Password varchar(50) not null,
	constraint pk_account_id primary key(Id)
)

CREATE TABLE Posts(
	Id int not null identity,
	PostTime datetime not null,
	AuthorId int,
	Content varchar(1000) not null,
	constraint pk_post_id primary key(Id),
	constraint fk_post_author_id foreign key(AuthorId)
		references Accounts(Id)
		on delete set null on update cascade
)

CREATE TABLE Comments(
	Id int not null identity,
	PostId int not null,
	CommentTime datetime not null,
	AuthorId int,
	Content varchar(200) not null,
	constraint pk_comment_id primary key(Id),
	constraint fk_post_id foreign key(PostId)
		references Posts(Id)
		on delete cascade on update cascade,
	constraint fk_comment_author_id foreign key(AuthorId)
		references Accounts(Id)
		--on delete set null on update cascade
)
-- Добавить ограничение, что дата коммента позже даты поста


CREATE TABLE Films(
	Id int not null identity,
	Type tinyint not null default(0),			-- 0 - фильм, 1 - сериал
	Name varchar(50) not null,
	Year int not null check(Year >= 1888),
	constraint pk_film_id primary key(Id)
)

CREATE TABLE FilmStuff(
	Id int not null identity,
	FirstName varchar(50) not null,		-- имя
	LastName varchar(50) not null,			-- Фамилия
	MiddleName varchar(50),				-- Отчество
	constraint pk_film_stuff_id primary key(Id)
)

CREATE TABLE StuffToFilm(
	Id int not null identity,
	StuffId int not null,
	FilmId int not null,
	Role tinyint not null,				-- enum в проекте
	constraint pk_stuff_to_film_id primary key(Id),
	constraint fk_stuff_if foreign key(StuffId)
		references FilmStuff(Id)
		on delete cascade on update cascade,
	constraint fk_film_id foreign key(FilmId)
		references Films(Id)
		on delete cascade on update cascade
)
-- Сделать primary key по Stuffid, FilmId