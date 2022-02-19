IF NOT EXISTS(SELECT *
FROM sys.databases
WHERE name = 'JarsDatabase')
  BEGIN
  CREATE DATABASE JarsDatabase

END;
GO
USE [JarsDatabase]
GO
IF OBJECT_ID('Account', 'U') IS NULL
BEGIN
  CREATE TABLE [Account]
  (
    [ID] varchar(128),
    --PK--
    [IsAdmin] bit,
    [Email] varchar(max),
    [DisplayName] nvarchar(max),
    [PhotoUrl] varchar(max),
    [LastLoginDate] DateTime,

    PRIMARY KEY ([ID]),
  );
END;
GO
IF OBJECT_ID('CategoryWallet', 'U') IS NULL
BEGIN
  CREATE TABLE [CategoryWallet]
  (
    [ID] int IDENTITY(1, 1),
    --PK--
    [Name] nvarchar(max),
    [ParentCategoryID] int,
    [CurrentCategoryLevel] int,

    PRIMARY KEY ([ID]),

    FOREIGN KEY ([ParentCategoryID]) REFERENCES CategoryWallet([ID]),
  );
END;
GO
IF OBJECT_ID('Wallet', 'U') IS NULL
BEGIN
  CREATE TABLE [Wallet]
  (
    [ID] int IDENTITY(1, 1),
    --PK--
    [Name] nvarchar(max),
    [StartDate] DateTime,
    [WalletAmount] money,
    [Percentage] Decimal,
    [AccountID] varchar(128),
    --FK--
    [CategoryWalletID] int,

    PRIMARY KEY ([ID]),

    FOREIGN KEY ([AccountID]) REFERENCES Account([ID]),
    FOREIGN KEY ([CategoryWalletID]) REFERENCES CategoryWallet([ID]),
  );
END;
GO
IF OBJECT_ID('Category', 'U') IS NULL
BEGIN
  CREATE TABLE [Category]
  (
    [ID] int IDENTITY(1, 1),
    --PK--
    [Name] nvarchar(max),
    [ParentCategoryID] int,
    [CurrentCategoryLevel] int,

    PRIMARY KEY ([ID]),

    FOREIGN KEY ([ParentCategoryID]) REFERENCES Category([ID]),
  );

END;
GO
IF OBJECT_ID('Note', 'U') IS NULL
BEGIN

  CREATE TABLE [Note]
  (
    [ID] int IDENTITY(1, 1),
    --PK--
    [AddedDate] DateTime,
    [Comments] nvarchar(max),
    [Image] varchar(max),
    [TransactionID] int,
    --FK--
    [ContractID] int,
    --FK-
	[Latitude] float,
	[Longitude] float,

    PRIMARY KEY ([ID]),
    --FOREIGN KEY ([TransactionID]) REFERENCES Transaction([ID]),
    --FOREIGN KEY ([ContractID]) REFERENCES Contract([ID]),
  );
END;
GO
IF OBJECT_ID('ScheduleType', 'U') IS NULL
BEGIN
  CREATE TABLE [ScheduleType]
  (
    [ID] int IDENTITY(1, 1),
    --PK--
    [Name] nvarchar(max),

    PRIMARY KEY ([ID])
  );
END;
GO
IF OBJECT_ID('Contract', 'U') IS NULL
BEGIN
  CREATE TABLE [Contract]
  (
    [ID] int IDENTITY(1, 1),
    --PK--
    [AccountID] varchar(128),
    [ScheduleTypeID] int,
    --FK--
    [NoteID] int,
    --FK--
    [StartDate] DateTime,
    [EndDate] DateTime,
    [Amount] Money,
	[Name] nvarchar(max),

    PRIMARY KEY ([ID]),

    FOREIGN KEY ([AccountID]) REFERENCES Account([ID]),
    FOREIGN KEY ([NoteID]) REFERENCES Note([ID]),
    FOREIGN KEY ([ScheduleTypeID]) REFERENCES ScheduleType([ID]),
  );

END;
GO
IF OBJECT_ID('Bill', 'U') IS NULL
BEGIN
  CREATE TABLE [Bill]
  (
    [ID] int IDENTITY(1, 1),
    --PK--
    [Date] DateTime,
    [Name] nvarchar(max),
    [Amount] money,
    [LeftAmount] money,
    [CategoryID] int,
    --FK--
    [ContractID] int,
    --FK--

    PRIMARY KEY ([ID]),

    FOREIGN KEY ([CategoryID]) REFERENCES Category([ID]),
    FOREIGN KEY ([ContractID]) REFERENCES [Contract]([ID]),
  );
END;
IF OBJECT_ID('Transaction', 'U') IS NULL
BEGIN
  CREATE TABLE [Transaction]
  (
    [ID] int IDENTITY(1, 1),
    [WalletID] int,
    [TransactionDate] DateTime,
    [NoteID] int,
    --FK--
    [BillID] int,
    --FK--
    [Amount] money,

    PRIMARY KEY ([ID]),

    FOREIGN KEY ([NoteID]) REFERENCES Note([ID]),
    FOREIGN KEY ([WalletID]) REFERENCES Wallet([ID]),
    FOREIGN KEY ([BillID]) REFERENCES Bill([ID]),
  );
END;
IF OBJECT_ID('BillDetail', 'U') IS NULL
BEGIN
  CREATE TABLE [BillDetail]
  (
    [ID] int IDENTITY(1, 1),
    --PK--
    [ItemName] nvarchar(max),
    [Price] money,
    [Quantity] int,
    [BillID] int,
    --FK--

    PRIMARY KEY ([ID]),

    FOREIGN KEY ([BillID]) REFERENCES Bill([ID]),
  );
END;

IF OBJECT_ID('Note', 'U') IS NOT NULL
BEGIN
IF(OBJECT_ID('FK_TransactionID_ID_Transaction', 'F') IS NULL)
ALTER TABLE [Note]
   ADD CONSTRAINT FK_TransactionID_ID_Transaction FOREIGN KEY ([TransactionID])
      REFERENCES [Transaction](ID)
END;

IF OBJECT_ID('Note', 'U') IS NOT NULL
BEGIN
IF(OBJECT_ID('FK_ContractID_ID_Contract', 'F') IS NULL)
ALTER TABLE [Note]
  ADD CONSTRAINT FK_ContractID_ID_Contract FOREIGN KEY ([ContractID])
      REFERENCES [Contract](ID)
END;

IF OBJECT_ID('AccountDevice', 'U') IS NULL
BEGIN
  CREATE TABLE [AccountDevice]
  (
    [FcmToken] varchar(1000),
    --PK--
    [AccountID] varchar(128),
    --FK--
	[LastActiveDate] DateTime,

    PRIMARY KEY ([FcmToken]),

    FOREIGN KEY ([AccountId]) REFERENCES Account([ID]),
  );
END;