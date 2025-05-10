CREATE DATABASE TravelEase;
drop database TravelEase
USE TravelEase;

CREATE TABLE Destination (
    DestID INT IDENTITY(1, 1) PRIMARY KEY,
    Location VARCHAR(50),
    City VARCHAR(50) NOT NULL,
    Country VARCHAR(50) NOT NULL,
    JoinDate DATE NOT NULL
)

CREATE TABLE Hotel (
    HUsername VARCHAR(50) PRIMARY KEY,
    Name VARCHAR(50) NOT NULL
)

CREATE TABLE Room (
    HUsername VARCHAR(50) NOT NULL FOREIGN KEY REFERENCES Hotel(Husername),
    RoomNumber INT NOT NULL,
    Price INT,
)

ALTER TABLE Room ADD CONSTRAINT PK_Room PRIMARY KEY (HUsername, RoomNumber)

CREATE TABLE Room_Service(
    HUsername VARCHAR(50) NOT NULL,
    RoomNumber INT NOT NULL,
    ServiceType VARCHAR(30) NOT NULL
)

ALTER TABLE Room_Service ADD CONSTRAINT PK_Room_Service PRIMARY KEY (HUsername, RoomNumber, ServiceType)

ALTER TABLE Room_Service ADD CONSTRAINT FK_Service_Room FOREIGN KEY (HUsername, RoomNumber) REFERENCES Room(HUsername, RoomNumber)

CREATE TABLE [User](
    Username VARCHAR(30) PRIMARY KEY,
    Email VARCHAR(50) NOT NULL UNIQUE, -- Candidate Key
    Password VARCHAR(128) NOT NULL, -- Hashed Password (sha512)
    JoinDate DATETIME NOT NULL
)

CREATE TABLE Operator(
    Username VARCHAR(30) PRIMARY KEY
)

CREATE TABLE Admin(
    Username VARCHAR(30) PRIMARY KEY
)

CREATE TABLE Traveller(
    Username VARCHAR(30) PRIMARY KEY,
    FName VARCHAR(30),
    LName VARCHAR(30),
    Nationality VARCHAR(30) NOT NULL,
    Age INT NOT NULL,
    Budget INT NOT NULL
)

CREATE TABLE Room_Booking(
    HUsername VARCHAR(50) NOT NULL,
    RoomID INT NOT NULL,
    Username VARCHAR(30) NOT NULL FOREIGN KEY REFERENCES Traveller(Username),
    StartDate DATETIME NOT NULL,
    BookingDate DATETIME,
    EndDate DATETIME
)

ALTER TABLE Room_Booking ADD CONSTRAINT PK_Room_Booking PRIMARY KEY (HUsername, RoomID, Username, StartDate)
ALTER TABLE Room_Booking ADD CONSTRAINT FK_Room_Booking_Room FOREIGN KEY (HUsername, RoomID) REFERENCES Room(HUsername, RoomNumber)

CREATE TABLE Categories(
    CatID INT IDENTITY (1, 1) PRIMARY KEY,
    Type VARCHAR(30) NOT NULL
)

CREATE TABLE Trip(
    TripID INT IDENTITY (1, 1) PRIMARY KEY,
    Title VARCHAR(50) NOT NULL,
    Type VARCHAR(30) NOT NULL,
    CancelStatus BINARY DEFAULT 0 NOT NULL,
    CancellationPolicy VARCHAR(20) CHECK (CancellationPolicy IN ('Refundable', 'Non-Refundable')),
    GroupSize INT NOT NULL,
    StartDate DATETIME NOT NULL,
    EndDate DATETIME NOT NULL,
    PriceRange INT,
    OperatorUsername VARCHAR(30) NOT NULL FOREIGN KEY REFERENCES Operator(Username)
)

CREATE TABLE Traveller_Wishlist(
    Username VARCHAR(30) NOT NULL FOREIGN KEY REFERENCES Traveller(Username),
    TripID INT NOT NULL FOREIGN KEY REFERENCES Trip(TripID)
)

ALTER TABLE Traveller_Wishlist ADD CONSTRAINT PK_Traveller_Wishlist PRIMARY KEY (Username, TripID)

CREATE TABLE Traveller_Cart(
    Username VARCHAR(30) NOT NULL FOREIGN KEY REFERENCES Traveller(Username),
    TripID INT NOT NULL FOREIGN KEY REFERENCES Trip(TripID)
)

ALTER TABLE Traveller_Cart ADD CONSTRAINT PK_Traveller_Cart PRIMARY KEY (Username, TripID)

CREATE TABLE Trip_Booking(
    Username VARCHAR(30) NOT NULL FOREIGN KEY REFERENCES Traveller(Username),
    TripID INT NOT NULL FOREIGN KEY REFERENCES Trip(TripID)
)

ALTER TABLE Trip_Booking ADD CONSTRAINT PK_Trip_Booking PRIMARY KEY (Username, TripID)

CREATE TABLE Trip_Itinerary (
    TripID INT NOT NULL FOREIGN KEY REFERENCES Trip(TripID),
    [Event] VARCHAR(30) NOT NULL,
    EventStartDate DATETIME NOT NULL,
    EventEndDate DATETIME NOT NULL
)

ALTER TABLE Trip_Itinerary ADD CONSTRAINT PK_Trip_Itinerary PRIMARY KEY (TripID, Event, EventStartDate)

CREATE TABLE Accessibility_Options(
    AccessibilityID INT IDENTITY (1, 1) PRIMARY KEY,
    [Option] VARCHAR(30) NOT NULL
)

CREATE TABLE Trip_Accessibility(
    TripID INT NOT NULL FOREIGN KEY REFERENCES Trip(TripID),
    AccessibilityID INT NOT NULL FOREIGN KEY REFERENCES Accessibility_Options(AccessibilityID)
)

ALTER TABLE Trip_Accessibility ADD CONSTRAINT PK_Trip_Accessibility PRIMARY KEY (TripID, AccessibilityID)

CREATE TABLE Trip_Categories(
    TripID INT NOT NULL FOREIGN KEY REFERENCES Trip(TripID),
    CatID INT NOT NULL FOREIGN KEY REFERENCES Categories(CatID)
)

ALTER TABLE Trip_Categories ADD CONSTRAINT PK_Trip_Categories PRIMARY KEY (TripID, CatID)

CREATE TABLE Inclusions(
	IncID INT IDENTITY (1, 1) PRIMARY KEY,
	[Type] Varchar(50) NOT NULL,
	Price int check(Price > 0)
)

CREATE TABLE Trip_Inclusions( -- Not normalized but normalization not needed in my opinion
    TripID INT NOT NULL FOREIGN KEY REFERENCES Trip(TripID),
    IncType VARCHAR(30) NOT NULL,
    IncPrice INT
)

ALTER TABLE Trip_Inclusions ADD CONSTRAINT PK_Trip_Inclusions PRIMARY KEY (TripID, IncType)

CREATE TABLE Trip_Transportation(
    TripID INT NOT NULL FOREIGN KEY REFERENCES Trip(TripID),
    TransportationType VARCHAR(30) NOT NULL,
    EstimatedStart DATETIME NOT NULL,
    EstimatedEnd DATETIME NOT NULL,
    Start DATETIME,
    [End] DATETIME
)

ALTER TABLE Trip_Transportation ADD CONSTRAINT PK_Trip_Transportation PRIMARY KEY (TripID, TransportationType, EstimatedStart)

CREATE TABLE Trip_Hotels(
    TripID INT NOT NULL FOREIGN KEY REFERENCES Trip(TripID),
    HUsername VARCHAR(50) NOT NULL FOREIGN KEY REFERENCES Hotel(HUsername)
)

ALTER TABLE Trip_Hotels ADD CONSTRAINT PK_Trip_Hotels PRIMARY KEY (TripID, HUsername)

CREATE TABLE Trip_Destination(
    TripID INT NOT NULL FOREIGN KEY REFERENCES Trip(TripID),
    DestID INT NOT NULL FOREIGN KEY REFERENCES Destination(DestID)
)

ALTER TABLE Trip_Destination ADD CONSTRAINT PK_Trip_Destination PRIMARY KEY (TripID, DestID)

CREATE TABLE Review(
    ReviewID INT IDENTITY (1, 1) PRIMARY KEY,
    Stars INT NOT NULL CHECK (Stars BETWEEN 1 AND 5),
    Feedback VARCHAR(256),
    Response VARCHAR(256),
    ReviewTime DATETIME NOT NULL,
    ResponseTime DATETIME,
    Reviewer VARCHAR(30) NOT NULL FOREIGN KEY REFERENCES Traveller(Username)
)

CREATE TABLE TripReview(
    ReviewID INT PRIMARY KEY,
    TripID INT NOT NULL FOREIGN KEY REFERENCES Trip(TripID)
)

CREATE TABLE HotelReview(
    ReviewID INT PRIMARY KEY,
    HUsername VARCHAR(50) NOT NULL FOREIGN KEY REFERENCES Hotel(HUsername)
)

CREATE TABLE DestinationReview(
    ReviewID INT PRIMARY KEY,
    DestinationID INT NOT NULL FOREIGN KEY REFERENCES Destination(DestID)
)
