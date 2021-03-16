--CREATE TABLE Item
--			(
--Id INT PRIMARY KEY NOT NULL IDENTITY(0,1),
--ItemName NVARCHAR(30),
--TotalCost money,
--UnitCost money,
--Quantity FLOAT(25) NOT NULL,
--Units NVARCHAR(25),
--Garnish BIT NOT NULL,
--[User] NVARCHAR(450) FOREIGN KEY REFERENCES AspNetUsers(Id)
--);

--INSERT INTO Item
--VALUES ('Vodka', 7.50, .01, 750, 'ml', 0, '80eb1983-8d7f-44cc-bbcd-a8b6a466102a');

--DROP TABLE Item;
--UPDATE Item 
--SET Id = 0 
--WHERE Id = 2;