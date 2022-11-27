CREATE TRIGGER DSN_Accounts ON Accounts
INSTEAD OF DELETE AS
BEGIN
	UPDATE Comments
	SET AuthorId = null
	WHERE AuthorId in (SELECT Id FROM deleted)

	
	UPDATE Posts
	SET AuthorId = null
	WHERE AuthorId in (SELECT Id FROM deleted)

	DELETE FROM Accounts
	WHERE Id in (SELECT Id FROM deleted)
END