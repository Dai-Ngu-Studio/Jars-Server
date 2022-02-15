IF NOT EXISTS(SELECT *
FROM sys.databases
WHERE name = 'JarsDatabase')
  BEGIN
  CREATE DATABASE JarsDatabase

END;
GO
USE [JarsDatabase]
GO
IF OBJECT_ID('AccountType', 'U') IS NULL
BEGIN
  CREATE TABLE [AccountType]
  (
    [ID] int IDENTITY(1, 1),
    --PK--
    [Name] nvarchar(max),

    PRIMARY KEY ([ID])
  );
END;
GO
IF OBJECT_ID('Account', 'U') IS NULL
BEGIN
  CREATE TABLE [Account]
  (
    [ID] varchar(128),
    --PK--
    [AccountTypeID] int,
    --FK--
    [Email] varchar(max),
    [DisplayName] nvarchar(max),
    [PhotoUrl] varchar(max),
    [LastLoginDate] DateTime,

    PRIMARY KEY ([ID]),

    FOREIGN KEY ([AccountTypeID]) REFERENCES AccountType([ID]),
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

    PRIMARY KEY ([ID]),

    FOREIGN KEY ([AccountID]) REFERENCES Account([ID]),
  );
END;
GO
IF OBJECT_ID('CategoryWallet', 'U') IS NULL
BEGIN
  CREATE TABLE [CategoryWallet]
  (
    [ID] int IDENTITY(1, 1),
    --PK--
    [WalletID] int,
    --FK--
    [Name] nvarchar(max),
    [ParentCategoryID] int,
    [CurrentCategoryLevel] int,

    PRIMARY KEY ([ID]),

    FOREIGN KEY ([WalletID]) REFERENCES [Wallet]([ID]),
    FOREIGN KEY ([ParentCategoryID]) REFERENCES CategoryWallet([ID]),
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

    PRIMARY KEY ([ID]),
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
    [CategoryID] int,
    [NoteID] int,
    --FK--
    [StartDate] DateTime,
    [EndDate] DateTime,
    [Amount] Money,

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
    FOREIGN KEY ([ContractID]) REFERENCES Contract([ID]),
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