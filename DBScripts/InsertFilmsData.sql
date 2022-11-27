SET IDENTITY_INSERT FilmStuff ON

INSERT INTO FilmStuff (Id, FirstName, LastName)
VALUES	(1,		'Tom',		'Holland' ),						-- Actor
		(2,		'Benedict', 'Cumberbatch'),						-- Actor
		(3,		'Marisa',	'Tomei'),							-- Actor
		(4,		'Willem',	'Dafoe'),							-- Actor
		(5,		'Alfred',	'Molina'),							-- Actor
		(6,		'Jamie',	'Foxx'),							-- Actor
		(7,		'Rhys',		'Ifans'),							-- Actor
		(8,		'Jacob',	'Batalon'),							-- Actor
		(9,		'Andrew',	'Garfield'),						-- Actor
		(10,	'Tobey',	'Maguire'),							-- Actor
		(11,	'Jon',		'Favreau'),							-- Actor
		(12,	'Benedict', 'Wong'),							-- Actor

		(23,	'Elizabeth','Olsen'),							-- Actor
		(24,	'Chiwetel', 'Ejiofor'),							-- Actor
		(25,	'Xochitl',	'Gomez'),							-- Actor
		(26,	'Michael',	'Stuhlbarg'),						-- Actor
		(27,	'Rachel',	'McAdams'),							-- Actor
		(28,	'Patrick',	'Stewart'),							-- Actor
		(29,	'John',		'Krasinski'),						-- Actor
		(30,	'Hayley',	'Atwell'),							-- Actor
		(31,	'Anson',	'Mount'),							-- Actor
		(32,	'Lashana',	'Lynch'),							-- Actor
		(33,	'Jett',		'Klyne'),							-- Actor
		(34,	'Julian',	'Hilliard'),						-- Actor
		
		(15,	'Jon',		'Watts'),							-- Director

		(35,	'Sam',		'Raimi'),							-- Director

		(16,	'Chris',	'McKenna'),							-- ScreenWriter
		(17,	'Erik',		'Sommers'),							-- ScreenWriter
		(18,	'Stan',		'Lee'),								-- ScreenWriter
		(19,	'Steve',	'Ditko'),							-- ScreenWriter

		(36,	'Michael',	'Waldron'),							-- ScreenWriter

		(20,	'Victoria', 'Alonso'),							-- Producer
		(21,	'Avi',		'Arad'),							-- Producer
		(22,	'Mitchell',	'Bell')								-- Producer

INSERT INTO FilmStuff(Id, FirstName)
VALUES (13,		'Zendaya')										-- Actor

INSERT INTO FilmStuff (Id, FirstName, MiddleName, LastName)
VALUES	(14,		'Thomas',	'Haden',		'Church'),		-- Actor
		(37,		'Eric',		'Hauserman',	'Carroll')		-- Producer

SET IDENTITY_INSERT FilmStuff OFF


SET IDENTITY_INSERT Films ON

INSERT INTO Films (Id, Type, Name, OriginalName, Year, Path, Description)
VALUES	(1,	0,	'Человек-паук: Нет пути домой', 'Spider-Man: No Way Home', 2021, '../filmIcons/SpiderManNoWayHome.jpg',
		'Жизнь и репутация Питера Паркера оказываются под угрозой, поскольку Мистерио раскрыл всему миру тайну личности Человека-паука. Пытаясь исправить ситуацию, Питер обращается за помощью к Стивену Стрэнджу, но вскоре всё становится намного опаснее.'),
		(2,	0, 'Доктор Стрэндж: В мультивселенной безумия', 'Doctor Strange in the Multiverse of Madness', 2022, '../filmIcons/DoctorStrangeMultiversOfMadness.jpg',
		'Доктор Стрэндж при помощи Вонга спасает от гигантского осьминога девушку-подростка по имени Америка Чавес, которая при сильном испуге может открывать порталы в параллельные вселенные. Пытаясь уберечь новую знакомую от злой силы, жаждущей заполучить её способность, Доктор вместе с Америкой пускается в путешествие по мультивселенной.')

SET IDENTITY_INSERT Films OFF

-- 0 - Actor, 1 - Director, 2 - ScreenWriter, 3 - Producer
INSERT INTO StuffToFilm (StuffId, FilmId, Role)
VALUES	(1,		1, 0),
		(2,		1, 0),
		(3,		1, 0),
		(4,		1, 0),
		(5,		1, 0),
		(6,		1, 0),
		(7,		1, 0),
		(8,		1, 0),
		(9,		1, 0),
		(10,	1, 0),
		(11,	1, 0),
		(12,	1, 0),
		(13,	1, 0),
		(14,	1, 0),

		(15,	1, 1),

		(16,	1, 2),
		(17,	1, 2),
		(18,	1, 2),
		(19,	1, 2),

		(20,	1, 3),
		(21,	1, 3),
		(22,	1, 3),

		(2,		2, 0),
		(23,	2, 0),
		(24,	2, 0),
		(12,	2, 0),
		(25,	2, 0),
		(26,	2, 0),
		(27,	2, 0),
		(28,	2, 0),
		(29,	2, 0),
		(30,	2, 0),
		(31,	2, 0),
		(32,	2, 0),
		(33,	2, 0),
		(34,	2, 0),

		(35,	2, 1),

		(36,	2, 2),
		(18,	2, 2),
		(19,	2, 2),

		(20,	2, 3),
		(22,	2, 3),
		(37,	2, 3)