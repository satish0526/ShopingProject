
create schema Exam

create table Exam.Item
(
	[Id]					UniqueIdentifier	not null,
	[Name]                  Nvarchar(500)       not null,
	[Description]           Nvarchar(4000)      null,
	[Specifications]        Nvarchar(max)       null,
	[Status]                Nvarchar(100)       null,    -- Available, Out of stock
	CONSTRAINT [PK_Item] PRIMARY KEY CLUSTERED ([Id] ASC)
)

create table Exam.[Order]
(
	[Id]					UniqueIdentifier	not null,
	[CustomerId]            UniqueIdentifier	not null,
    [RefNumber]             Nvarchar(100)       not null, --// #1234
	[Remarks]               Nvarchar(2000)      null,
	[CreatedDate]           Datetime2(7)        null,
	CONSTRAINT [PK_Order] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_Order_Customer] FOREIGN KEY ([CustomerId])	REFERENCES Exam.[Customer]([Id]),	
)

create table Exam.[OrderDetail]
(
	[Id]					UniqueIdentifier	not null,
	[OrderId]               UniqueIdentifier	not null,
    [ItemId]                UniqueIdentifier	not null,
    [Quantity]              Int                 null,
    [UnitPrice]             Decimal             null,
	CONSTRAINT [PK_OrderDetails] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_OrderDetails_Order] FOREIGN KEY ([OrderId])	REFERENCES Exam.[Order]([Id]),	
	CONSTRAINT [FK_BankFileDetail_Item] FOREIGN KEY ([ItemId])	REFERENCES Exam.Item([Id])	
)

create table Exam.Customer
(
	[Id]					UniqueIdentifier	not null,
    [Name]                  Nvarchar(200)       not null,
	[Email]                 Nvarchar(200)       not null,
	[ContactNo]             Bigint              not null,
	CONSTRAINT [PK_Customers] PRIMARY KEY CLUSTERED ([Id] ASC)
)

create table Exam.CustomerAddress
(
	[Id]					UniqueIdentifier	not null,
    [CustomerId]        	UniqueIdentifier	not null,
	[AddressLine1]          Nvarchar(100)       null,
	[AddressLine2]          Nvarchar(100)       null,
	[AddressLine3]          Nvarchar(100)       null,
	[PostalCode]            Nvarchar(100)       not null,
	[Country]               Nvarchar(200)       not null,
	CONSTRAINT [PK_CustomerAddress] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_CustomerAddress_Order] FOREIGN KEY ([CustomerId]) REFERENCES Exam.Customer([Id]),	
)

