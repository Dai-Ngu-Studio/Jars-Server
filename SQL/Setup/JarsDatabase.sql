USE [master]
GO

CREATE DATABASE JarsDatabase
GO

USE [JarsDatabase]
GO

CREATE TABLE [AccountType] (
  [ID] int,							--PK--
  [Name] nvarchar,

  PRIMARY KEY ([ID])
);

CREATE TABLE [Account] (
  [ID] varchar,							--PK--
  [AccountTypeID] int,				--FK--
  [UserName] varchar,
  [FirstName] nvarchar,
  [LastName] nvarchar,
  [LastLoginDate] DateTime,

  PRIMARY KEY ([ID]),

  FOREIGN KEY ([AccountTypeID]) REFERENCES AccountType([ID]),
);

CREATE TABLE [Wallet] (
  [ID] int,							--PK--
  [Name] nvarchar,
  [StartDate] DateTime,
  [WalletAmount] money,
  [Percentage] Decimal,
  [AccountID] varchar,					--FK--

  PRIMARY KEY ([ID]),

  FOREIGN KEY ([AccountID]) REFERENCES Account([ID]),
);

CREATE TABLE [CategoryWallet] (
  [ID] int,							--PK--
  [WalletID] int,					--FK--
  [Name] nvarchar,
  [ParentCategoryID] int,
  [CurrentCategoryLevel] int,

  PRIMARY KEY ([ID]),

  FOREIGN KEY ([WalletID]) REFERENCES [Wallet]([ID]),
  FOREIGN KEY ([ID]) REFERENCES CategoryWallet([ID]),
);

CREATE TABLE [Category] (
  [ID] int,							--PK--
  [Name] nvarchar,
  [ParentCategoryID] int,
  [CurrentCategoryLevel] int,

  PRIMARY KEY ([ID]),

  FOREIGN KEY ([ID]) REFERENCES Category([ID]),
);

CREATE TABLE [Bill] (
  [ID] int,							--PK--
  [Date] DateTime,
  [Name] nvarchar,
  [Amount] money,
  [RecurringTransactionID] int,		
  [LeftAmount] money,
  [CategoryID] int,					--FK--

  PRIMARY KEY ([ID]),

  FOREIGN KEY ([CategoryID]) REFERENCES Category([ID]),
);

CREATE TABLE [BillDetail] (
  [ID] int,							--PK--
  [ItemName] nvarchar,
  [Price] money,
  [Quantity] int,
  [BillID] int,						--FK--

  PRIMARY KEY ([ID]),

  FOREIGN KEY ([BillID]) REFERENCES Bill([ID]),
);

CREATE TABLE [Note] (
  [ID] int,							--PK--
  [AddedDate] DateTime,
  [Comments] nvarchar,
  [Image] varchar,

  PRIMARY KEY ([ID]),
);

CREATE TABLE [Transaction] (
  [ID] int,
  [WalletID] int,
  [TransactionDate] DateTime,
  [CategoryID] int,					--FK--
  [NoteID] int,						--FK--
  [BillID] int,						--FK--
  [Amount] money,

  PRIMARY KEY ([ID]),

  FOREIGN KEY ([NoteID]) REFERENCES Note([ID]),
  FOREIGN KEY ([WalletID]) REFERENCES Wallet([ID]),
  FOREIGN KEY ([BillID]) REFERENCES Bill([ID]),
);

CREATE TABLE [ScheduleType] (
  [ID] int,							--PK--
  [Name] nvarchar,

  PRIMARY KEY ([ID])
);

CREATE TABLE [Contract] (
  [ID] int,							--PK--
  [AccountID] varchar,
  [ScheduleTypeID] int,				--FK--
  [CategoryID] int,
  [NoteID] int,						--FK--
  [StartDate] DateTime,
  [EndDate] DateTime,
  [Amount] Money,
  [BillID] int,						--FK--

  PRIMARY KEY ([ID]),

  FOREIGN KEY ([NoteID]) REFERENCES Note([ID]),
  FOREIGN KEY ([ScheduleTypeID]) REFERENCES ScheduleType([ID]),
  FOREIGN KEY ([BillID]) REFERENCES Bill([ID]),
);