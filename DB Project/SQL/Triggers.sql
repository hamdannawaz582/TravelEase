USE TravelEase;

CREATE TABLE TripAudit
(
    AuditID            INT IDENTITY (1,1) PRIMARY KEY,
    TripID             INT,
    Title              VARCHAR(50),
    Type               VARCHAR(30),
    CancelStatus       BINARY,
    CancellationPolicy VARCHAR(20),
    GroupSize          INT,
    StartDate          DATETIME,
    EndDate            DATETIME,
    PriceRange         INT,
    OperatorUsername   VARCHAR(30),
    ActionType         VARCHAR(10), -- 'INSERT', 'UPDATE', 'DELETE'
    ActionDate         DATETIME,
    ActionBy           VARCHAR(128)
)

CREATE TABLE TripBookingAudit
(
    AuditID    INT IDENTITY (1,1) PRIMARY KEY,
    Username   VARCHAR(30),
    TripID     INT,
    ActionType VARCHAR(10), -- 'INSERT', 'UPDATE', 'DELETE'
    ActionDate DATETIME,
    ActionBy   VARCHAR(128)
)

GO

CREATE TRIGGER TripTrigger
    ON Trip
    AFTER INSERT, UPDATE, DELETE AS
BEGIN
    IF EXISTS (SELECT * FROM inserted) AND EXISTS (SELECT * FROM deleted) -- UPDATE
        BEGIN
            INSERT INTO TripAudit (TripID, Title, Type, CancelStatus, CancellationPolicy, GroupSize, StartDate, EndDate,
                                   PriceRange, OperatorUsername, ActionType, ActionDate, ActionBy)
            SELECT TripID,
                   Title,
                   Type,
                   CancelStatus,
                   CancellationPolicy,
                   GroupSize,
                   StartDate,
                   EndDate,
                   PriceRange,
                   OperatorUsername,
                   'UPDATE',
                   GETDATE(),
                   SYSTEM_USER
            FROM inserted
        END
    ELSE
        IF EXISTS (SELECT * FROM inserted) -- INSERT
            BEGIN
                INSERT INTO TripAudit (TripID, Title, Type, CancelStatus, CancellationPolicy, GroupSize, StartDate,
                                       EndDate, PriceRange, OperatorUsername, ActionType, ActionDate, ActionBy)
                SELECT TripID,
                       Title,
                       Type,
                       CancelStatus,
                       CancellationPolicy,
                       GroupSize,
                       StartDate,
                       EndDate,
                       PriceRange,
                       OperatorUsername,
                       'INSERT',
                       GETDATE(),
                       SYSTEM_USER
                FROM inserted
            END
        ELSE
            IF EXISTS (SELECT * FROM deleted) -- DELETE
                BEGIN
                    INSERT INTO TripAudit (TripID, Title, Type, CancelStatus, CancellationPolicy, GroupSize, StartDate,
                                           EndDate, PriceRange, OperatorUsername, ActionType, ActionDate, ActionBy)
                    SELECT TripID,
                           Title,
                           Type,
                           CancelStatus,
                           CancellationPolicy,
                           GroupSize,
                           StartDate,
                           EndDate,
                           PriceRange,
                           OperatorUsername,
                           'DELETE',
                           GETDATE(),
                           SYSTEM_USER
                    FROM deleted
                END

END;

GO

CREATE TRIGGER TripBookingTrigger
    ON Trip_Booking
    AFTER INSERT, UPDATE, DELETE AS
BEGIN
    IF EXISTS (SELECT * FROM inserted) AND EXISTS (SELECT * FROM deleted) -- UPDATE
        BEGIN
            INSERT INTO TripBookingAudit (Username, TripID, ActionType, ActionDate, ActionBy)
            SELECT Username, TripID, 'UPDATE', GETDATE(), SYSTEM_USER
            FROM inserted
        END
    ELSE
        IF EXISTS (SELECT * FROM inserted) -- INSERT
            BEGIN
                INSERT INTO TripBookingAudit (Username, TripID, ActionType, ActionDate, ActionBy)
                SELECT Username, TripID, 'INSERT', GETDATE(), SYSTEM_USER
                FROM inserted
            END
        ELSE
            IF EXISTS (SELECT * FROM deleted) -- DELETE
                BEGIN
                    INSERT INTO TripBookingAudit (Username, TripID, ActionType, ActionDate, ActionBy)
                    SELECT Username, TripID, 'DELETE', GETDATE(), SYSTEM_USER
                    FROM deleted
                END
END;

GO

CREATE TABLE UserAudit
(
    AuditID    INT IDENTITY (1,1) PRIMARY KEY,
    Username   VARCHAR(30),
    Email      VARCHAR(50),
    Password   VARCHAR(128),
    JoinDate   DATETIME,
    ActionType VARCHAR(10),
    ActionDate DATETIME,
    ActionBy   VARCHAR(128)
)

CREATE TABLE AdminAudit
(
    AuditID    INT IDENTITY (1,1) PRIMARY KEY,
    Username   VARCHAR(30),
    ActionType VARCHAR(10),
    ActionDate DATETIME,
    ActionBy   VARCHAR(128)
)

CREATE TABLE HotelAudit
(
    AuditID    INT IDENTITY (1,1) PRIMARY KEY,
    HUsername  VARCHAR(50),
    Name       VARCHAR(50),
    ActionType VARCHAR(10),
    ActionDate DATETIME,
    ActionBy   VARCHAR(128)
)

CREATE TABLE OperatorAudit
(
    AuditID    INT IDENTITY (1,1) PRIMARY KEY,
    Username   VARCHAR(30),
    ActionType VARCHAR(10),
    ActionDate DATETIME,
    ActionBy   VARCHAR(128)
)

CREATE TABLE TravellerAudit
(
    AuditID     INT IDENTITY (1,1) PRIMARY KEY,
    Username    VARCHAR(30),
    FName       VARCHAR(30),
    LName       VARCHAR(30),
    Nationality VARCHAR(30),
    Age         INT,
    Budget      INT,
    ActionType  VARCHAR(10),
    ActionDate  DATETIME,
    ActionBy    VARCHAR(128)
)

GO

CREATE TRIGGER UserTrigger
    ON [User]
    AFTER INSERT, UPDATE, DELETE AS
BEGIN
    IF EXISTS (SELECT * FROM inserted) AND EXISTS (SELECT * FROM deleted) -- UPDATE
        BEGIN
            INSERT INTO UserAudit (Username, Email, Password, JoinDate, ActionType, ActionDate, ActionBy)
            SELECT Username, Email, Password, JoinDate, 'UPDATE', GETDATE(), SYSTEM_USER
            FROM inserted
        END
    ELSE
        IF EXISTS (SELECT * FROM inserted) -- INSERT
            BEGIN
                INSERT INTO UserAudit (Username, Email, Password, JoinDate, ActionType, ActionDate, ActionBy)
                SELECT Username, Email, Password, JoinDate, 'INSERT', GETDATE(), SYSTEM_USER
                FROM inserted
            END
        ELSE
            IF EXISTS (SELECT * FROM deleted) -- DELETE
                BEGIN
                    INSERT INTO UserAudit (Username, Email, Password, JoinDate, ActionType, ActionDate, ActionBy)
                    SELECT Username, Email, Password, JoinDate, 'DELETE', GETDATE(), SYSTEM_USER
                    FROM deleted
                END
END;

GO

CREATE TRIGGER AdminTrigger
    ON Admin
    AFTER INSERT, UPDATE, DELETE AS
BEGIN
    IF EXISTS (SELECT * FROM inserted) AND EXISTS (SELECT * FROM deleted) -- UPDATE
        BEGIN
            INSERT INTO AdminAudit (Username, ActionType, ActionDate, ActionBy)
            SELECT Username, 'UPDATE', GETDATE(), SYSTEM_USER
            FROM inserted
        END
    ELSE
        IF EXISTS (SELECT * FROM inserted) -- INSERT
            BEGIN
                INSERT INTO AdminAudit (Username, ActionType, ActionDate, ActionBy)
                SELECT Username, 'INSERT', GETDATE(), SYSTEM_USER
                FROM inserted
            END
        ELSE
            IF EXISTS (SELECT * FROM deleted) -- DELETE
                BEGIN
                    INSERT INTO AdminAudit (Username, ActionType, ActionDate, ActionBy)
                    SELECT Username, 'DELETE', GETDATE(), SYSTEM_USER
                    FROM deleted
                END
END;

GO

CREATE TRIGGER TravellerTrigger
    ON Traveller
    AFTER INSERT, UPDATE, DELETE AS
BEGIN
    IF EXISTS (SELECT * FROM inserted) AND EXISTS (SELECT * FROM deleted) -- UPDATE
        BEGIN
            INSERT INTO TravellerAudit (Username, FName, LName, Nationality, Age, Budget, ActionType, ActionDate,
                                        ActionBy)
            SELECT Username,
                   FName,
                   LName,
                   Nationality,
                   Age,
                   Budget,
                   'UPDATE',
                   GETDATE(),
                   SYSTEM_USER
            FROM inserted
        END
    ELSE
        IF EXISTS (SELECT * FROM inserted) -- INSERT
            BEGIN
                INSERT INTO TravellerAudit (Username, FName, LName, Nationality, Age, Budget, ActionType, ActionDate,
                                            ActionBy)
                SELECT Username,
                       FName,
                       LName,
                       Nationality,
                       Age,
                       Budget,
                       'INSERT',
                       GETDATE(),
                       SYSTEM_USER
                FROM inserted
            END
        ELSE
            IF EXISTS (SELECT * FROM deleted) -- DELETE
                BEGIN
                    INSERT INTO TravellerAudit (Username, FName, LName, Nationality, Age, Budget, ActionType,
                                                ActionDate, ActionBy)
                    SELECT Username,
                           FName,
                           LName,
                           Nationality,
                           Age,
                           Budget,
                           'DELETE',
                           GETDATE(),
                           SYSTEM_USER
                    FROM deleted
                END
END;

GO

CREATE TRIGGER HotelTrigger
    ON Hotel
    AFTER INSERT, UPDATE, DELETE AS
BEGIN
    IF EXISTS (SELECT * FROM inserted) AND EXISTS (SELECT * FROM deleted) -- UPDATE
        BEGIN
            INSERT INTO HotelAudit (HUsername, Name, ActionType, ActionDate, ActionBy)
            SELECT HUsername, Name, 'UPDATE', GETDATE(), SYSTEM_USER
            FROM inserted
        END
    ELSE
        IF EXISTS (SELECT * FROM inserted) -- INSERT
            BEGIN
                INSERT INTO HotelAudit (HUsername, Name, ActionType, ActionDate, ActionBy)
                SELECT HUsername, Name, 'INSERT', GETDATE(), SYSTEM_USER
                FROM inserted
            END
        ELSE
            IF EXISTS (SELECT * FROM deleted) -- DELETE
                BEGIN
                    INSERT INTO HotelAudit (HUsername, Name, ActionType, ActionDate, ActionBy)
                    SELECT HUsername, Name, 'DELETE', GETDATE(), SYSTEM_USER
                    FROM deleted
                END
END;

GO

CREATE TRIGGER OperatorTrigger
    ON Operator
    AFTER INSERT, UPDATE, DELETE AS
BEGIN
    IF EXISTS (SELECT * FROM inserted) AND EXISTS (SELECT * FROM deleted) -- UPDATE
        BEGIN
            INSERT INTO OperatorAudit (Username, ActionType, ActionDate, ActionBy)
            SELECT Username, 'UPDATE', GETDATE(), SYSTEM_USER
            FROM inserted
        END
    ELSE
        IF EXISTS (SELECT * FROM inserted) -- INSERT
            BEGIN
                INSERT INTO OperatorAudit (Username, ActionType, ActionDate, ActionBy)
                SELECT Username, 'INSERT', GETDATE(), SYSTEM_USER
                FROM inserted
            END
        ELSE
            IF EXISTS (SELECT * FROM deleted) -- DELETE
                BEGIN
                    INSERT INTO OperatorAudit (Username, ActionType, ActionDate, ActionBy)
                    SELECT Username, 'DELETE', GETDATE(), SYSTEM_USER
                    FROM deleted
                END
END;

GO

CREATE TABLE DestinationAudit
(
    AuditID    INT IDENTITY (1,1) PRIMARY KEY,
    DestID     INT,
    Location   VARCHAR(50),
    City       VARCHAR(50),
    Country    VARCHAR(50),
    JoinDate   DATE,
    ActionType VARCHAR(10),
    ActionDate DATETIME,
    ActionBy   VARCHAR(128)
)

CREATE TABLE TripDestinationAudit
(
    AuditID    INT IDENTITY (1,1) PRIMARY KEY,
    TripID     INT,
    DestID     INT,
    ActionType VARCHAR(10),
    ActionDate DATETIME,
    ActionBy   VARCHAR(128)
)

CREATE TABLE DestinationReviewAudit
(
    AuditID       INT IDENTITY (1,1) PRIMARY KEY,
    ReviewID      INT,
    DestinationID INT,
    ActionType    VARCHAR(10),
    ActionDate    DATETIME,
    ActionBy      VARCHAR(128)
)

GO

CREATE TRIGGER DestinationTrigger
    ON Destination
    AFTER INSERT, UPDATE, DELETE AS
BEGIN
    IF EXISTS (SELECT * FROM inserted) AND EXISTS (SELECT * FROM deleted) -- UPDATE
        BEGIN
            INSERT INTO DestinationAudit (DestID, Location, City, Country, JoinDate, ActionType, ActionDate, ActionBy)
            SELECT DestID,
                   Location,
                   City,
                   Country,
                   JoinDate,
                   'UPDATE',
                   GETDATE(),
                   SYSTEM_USER
            FROM inserted
        END
    ELSE
        IF EXISTS (SELECT * FROM inserted) -- INSERT
            BEGIN
                INSERT INTO DestinationAudit (DestID, Location, City, Country, JoinDate, ActionType, ActionDate,
                                              ActionBy)
                SELECT DestID,
                       Location,
                       City,
                       Country,
                       JoinDate,
                       'INSERT',
                       GETDATE(),
                       SYSTEM_USER
                FROM inserted
            END
        ELSE
            IF EXISTS (SELECT * FROM deleted) -- DELETE
                BEGIN
                    INSERT INTO DestinationAudit (DestID, Location, City, Country, JoinDate, ActionType, ActionDate,
                                                  ActionBy)
                    SELECT DestID,
                           Location,
                           City,
                           Country,
                           JoinDate,
                           'DELETE',
                           GETDATE(),
                           SYSTEM_USER
                    FROM deleted
                END
END;

GO

CREATE TRIGGER TripDestinationTrigger
    ON Trip_Destination
    AFTER INSERT, UPDATE, DELETE AS
BEGIN
    IF EXISTS (SELECT * FROM inserted) AND EXISTS (SELECT * FROM deleted) -- UPDATE
        BEGIN
            INSERT INTO TripDestinationAudit (TripID, DestID, ActionType, ActionDate, ActionBy)
            SELECT TripID, DestID, 'UPDATE', GETDATE(), SYSTEM_USER
            FROM inserted
        END
    ELSE
        IF EXISTS (SELECT * FROM inserted) -- INSERT
            BEGIN
                INSERT INTO TripDestinationAudit (TripID, DestID, ActionType, ActionDate, ActionBy)
                SELECT TripID, DestID, 'INSERT', GETDATE(), SYSTEM_USER
                FROM inserted
            END
        ELSE
            IF EXISTS (SELECT * FROM deleted) -- DELETE
                BEGIN
                    INSERT INTO TripDestinationAudit (TripID, DestID, ActionType, ActionDate, ActionBy)
                    SELECT TripID, DestID, 'DELETE', GETDATE(), SYSTEM_USER
                    FROM deleted
                END
END;

GO

CREATE TRIGGER DestinationReviewTrigger
    ON DestinationReview
    AFTER INSERT, UPDATE, DELETE AS
BEGIN
    IF EXISTS (SELECT * FROM inserted) AND EXISTS (SELECT * FROM deleted) -- UPDATE
        BEGIN
            INSERT INTO DestinationReviewAudit (ReviewID, DestinationID, ActionType, ActionDate, ActionBy)
            SELECT ReviewID, DestinationID, 'UPDATE', GETDATE(), SYSTEM_USER
            FROM inserted
        END
    ELSE
        IF EXISTS (SELECT * FROM inserted) -- INSERT
            BEGIN
                INSERT INTO DestinationReviewAudit (ReviewID, DestinationID, ActionType, ActionDate, ActionBy)
                SELECT ReviewID, DestinationID, 'INSERT', GETDATE(), SYSTEM_USER
                FROM inserted
            END
        ELSE
            IF EXISTS (SELECT * FROM deleted) -- DELETE
                BEGIN
                    INSERT INTO DestinationReviewAudit (ReviewID, DestinationID, ActionType, ActionDate, ActionBy)
                    SELECT ReviewID, DestinationID, 'DELETE', GETDATE(), SYSTEM_USER
                    FROM deleted
                END
END;

GO

CREATE TABLE ReviewAudit
(
    AuditID      INT IDENTITY (1,1) PRIMARY KEY,
    ReviewID     INT,
    Stars        INT,
    Feedback     VARCHAR(256),
    Response     VARCHAR(256),
    ReviewTime   DATETIME,
    ResponseTime DATETIME,
    Reviewer     VARCHAR(30),
    ActionType   VARCHAR(10),
    ActionDate   DATETIME,
    ActionBy     VARCHAR(128)
)

CREATE TABLE HotelReviewAudit
(
    AuditID    INT IDENTITY (1,1) PRIMARY KEY,
    ReviewID   INT,
    HUsername  VARCHAR(50),
    ActionType VARCHAR(10),
    ActionDate DATETIME,
    ActionBy   VARCHAR(128)
)

CREATE TABLE TripReviewAudit
(
    AuditID    INT IDENTITY (1,1) PRIMARY KEY,
    ReviewID   INT,
    TripID     INT,
    ActionType VARCHAR(10),
    ActionDate DATETIME,
    ActionBy   VARCHAR(128)
)

GO

CREATE TRIGGER ReviewTrigger
    ON Review
    AFTER INSERT, UPDATE, DELETE AS
BEGIN
    IF EXISTS (SELECT * FROM inserted) AND EXISTS (SELECT * FROM deleted) -- UPDATE
        BEGIN
            INSERT INTO ReviewAudit (ReviewID, Stars, Feedback, Response, ReviewTime, ResponseTime, Reviewer,
                                     ActionType, ActionDate, ActionBy)
            SELECT ReviewID,
                   Stars,
                   Feedback,
                   Response,
                   ReviewTime,
                   ResponseTime,
                   Reviewer,
                   'UPDATE',
                   GETDATE(),
                   SYSTEM_USER
            FROM inserted
        END
    ELSE
        IF EXISTS (SELECT * FROM inserted) -- INSERT
            BEGIN
                INSERT INTO ReviewAudit (ReviewID, Stars, Feedback, Response, ReviewTime, ResponseTime, Reviewer,
                                         ActionType, ActionDate, ActionBy)
                SELECT ReviewID,
                       Stars,
                       Feedback,
                       Response,
                       ReviewTime,
                       ResponseTime,
                       Reviewer,
                       'INSERT',
                       GETDATE(),
                       SYSTEM_USER
                FROM inserted
            END
        ELSE
            IF EXISTS (SELECT * FROM deleted) -- DELETE
                BEGIN
                    INSERT INTO ReviewAudit (ReviewID, Stars, Feedback, Response, ReviewTime, ResponseTime, Reviewer,
                                             ActionType, ActionDate, ActionBy)
                    SELECT ReviewID,
                           Stars,
                           Feedback,
                           Response,
                           ReviewTime,
                           ResponseTime,
                           Reviewer,
                           'DELETE',
                           GETDATE(),
                           SYSTEM_USER
                    FROM deleted
                END
END;

GO

CREATE TRIGGER HotelReviewTrigger
    ON HotelReview
    AFTER INSERT, UPDATE, DELETE AS
BEGIN
    IF EXISTS (SELECT * FROM inserted) AND EXISTS (SELECT * FROM deleted) -- UPDATE
        BEGIN
            INSERT INTO HotelReviewAudit (ReviewID, HUsername, ActionType, ActionDate, ActionBy)
            SELECT ReviewID, HUsername, 'UPDATE', GETDATE(), SYSTEM_USER
            FROM inserted
        END
    ELSE
        IF EXISTS (SELECT * FROM inserted) -- INSERT
            BEGIN
                INSERT INTO HotelReviewAudit (ReviewID, HUsername, ActionType, ActionDate, ActionBy)
                SELECT ReviewID, HUsername, 'INSERT', GETDATE(), SYSTEM_USER
                FROM inserted
            END
        ELSE
            IF EXISTS (SELECT * FROM deleted) -- DELETE
                BEGIN
                    INSERT INTO HotelReviewAudit (ReviewID, HUsername, ActionType, ActionDate, ActionBy)
                    SELECT ReviewID, HUsername, 'DELETE', GETDATE(), SYSTEM_USER
                    FROM deleted
                END
END;

GO

CREATE TRIGGER TripReviewTrigger
    ON TripReview
    AFTER INSERT, UPDATE, DELETE AS
BEGIN
    IF EXISTS (SELECT * FROM inserted) AND EXISTS (SELECT * FROM deleted) -- UPDATE
        BEGIN
            INSERT INTO TripReviewAudit (ReviewID, TripID, ActionType, ActionDate, ActionBy)
            SELECT ReviewID, TripID, 'UPDATE', GETDATE(), SYSTEM_USER
            FROM inserted
        END
    ELSE
        IF EXISTS (SELECT * FROM inserted) -- INSERT
            BEGIN
                INSERT INTO TripReviewAudit (ReviewID, TripID, ActionType, ActionDate, ActionBy)
                SELECT ReviewID, TripID, 'INSERT', GETDATE(), SYSTEM_USER
                FROM inserted
            END
        ELSE
            IF EXISTS (SELECT * FROM deleted) -- DELETE
                BEGIN
                    INSERT INTO TripReviewAudit (ReviewID, TripID, ActionType, ActionDate, ActionBy)
                    SELECT ReviewID, TripID, 'DELETE', GETDATE(), SYSTEM_USER
                    FROM deleted
                END
END;

GO

CREATE TABLE TripHotelsAudit
(
    AuditID    INT IDENTITY (1,1) PRIMARY KEY,
    TripID     INT,
    HUsername  VARCHAR(50),
    ActionType VARCHAR(10),
    ActionDate DATETIME,
    ActionBy   VARCHAR(128)
)

CREATE TABLE TripInclusionsAudit
(
    AuditID    INT IDENTITY (1,1) PRIMARY KEY,
    TripID     INT,
    IncType    VARCHAR(30),
    IncPrice   INT,
    ActionType VARCHAR(10),
    ActionDate DATETIME,
    ActionBy   VARCHAR(128)
)

CREATE TABLE TripItineraryAudit
(
    AuditID        INT IDENTITY (1,1) PRIMARY KEY,
    TripID         INT,
    Event          VARCHAR(30),
    EventStartDate DATETIME,
    EventEndDate   DATETIME,
    ActionType     VARCHAR(10),
    ActionDate     DATETIME,
    ActionBy       VARCHAR(128)
)

GO

CREATE TRIGGER TripHotelsTrigger
    ON Trip_Hotels
    AFTER INSERT, UPDATE, DELETE AS
BEGIN
    IF EXISTS (SELECT * FROM inserted) AND EXISTS (SELECT * FROM deleted) -- UPDATE
        BEGIN
            INSERT INTO TripHotelsAudit (TripID, HUsername, ActionType, ActionDate, ActionBy)
            SELECT TripID, HUsername, 'UPDATE', GETDATE(), SYSTEM_USER
            FROM inserted
        END
    ELSE
        IF EXISTS (SELECT * FROM inserted) -- INSERT
            BEGIN
                INSERT INTO TripHotelsAudit (TripID, HUsername, ActionType, ActionDate, ActionBy)
                SELECT TripID, HUsername, 'INSERT', GETDATE(), SYSTEM_USER
                FROM inserted
            END
        ELSE
            IF EXISTS (SELECT * FROM deleted) -- DELETE
                BEGIN
                    INSERT INTO TripHotelsAudit (TripID, HUsername, ActionType, ActionDate, ActionBy)
                    SELECT TripID, HUsername, 'DELETE', GETDATE(), SYSTEM_USER
                    FROM deleted
                END
END;

GO

CREATE TRIGGER TripInclusionsTrigger
    ON Trip_Inclusions
    AFTER INSERT, UPDATE, DELETE AS
BEGIN
    IF EXISTS (SELECT * FROM inserted) AND EXISTS (SELECT * FROM deleted) -- UPDATE
        BEGIN
            INSERT INTO TripInclusionsAudit (TripID, IncType, IncPrice, ActionType, ActionDate, ActionBy)
            SELECT TripID, IncType, IncPrice, 'UPDATE', GETDATE(), SYSTEM_USER
            FROM inserted
        END
    ELSE
        IF EXISTS (SELECT * FROM inserted) -- INSERT
            BEGIN
                INSERT INTO TripInclusionsAudit (TripID, IncType, IncPrice, ActionType, ActionDate, ActionBy)
                SELECT TripID, IncType, IncPrice, 'INSERT', GETDATE(), SYSTEM_USER
                FROM inserted
            END
        ELSE
            IF EXISTS (SELECT * FROM deleted) -- DELETE
                BEGIN
                    INSERT INTO TripInclusionsAudit (TripID, IncType, IncPrice, ActionType, ActionDate, ActionBy)
                    SELECT TripID, IncType, IncPrice, 'DELETE', GETDATE(), SYSTEM_USER
                    FROM deleted
                END
END;

GO

CREATE TRIGGER TripItineraryTrigger
    ON Trip_Itinerary
    AFTER INSERT, UPDATE, DELETE AS
BEGIN
    IF EXISTS (SELECT * FROM inserted) AND EXISTS (SELECT * FROM deleted) -- UPDATE
        BEGIN
            INSERT INTO TripItineraryAudit (TripID, Event, EventStartDate, EventEndDate, ActionType, ActionDate,
                                            ActionBy)
            SELECT TripID, Event, EventStartDate, EventEndDate, 'UPDATE', GETDATE(), SYSTEM_USER
            FROM inserted
        END
    ELSE
        IF EXISTS (SELECT * FROM inserted) -- INSERT
            BEGIN
                INSERT INTO TripItineraryAudit (TripID, Event, EventStartDate, EventEndDate, ActionType, ActionDate,
                                                ActionBy)
                SELECT TripID, Event, EventStartDate, EventEndDate, 'INSERT', GETDATE(), SYSTEM_USER
                FROM inserted
            END
        ELSE
            IF EXISTS (SELECT * FROM deleted) -- DELETE
                BEGIN
                    INSERT INTO TripItineraryAudit (TripID, Event, EventStartDate, EventEndDate, ActionType, ActionDate,
                                                    ActionBy)
                    SELECT TripID, Event, EventStartDate, EventEndDate, 'DELETE', GETDATE(), SYSTEM_USER
                    FROM deleted
                END
END;

GO

CREATE TABLE TripTransportationAudit
(
    AuditID            INT IDENTITY (1,1) PRIMARY KEY,
    TripID             INT,
    TransportationType VARCHAR(30),
    EstimatedStart     DATETIME,
    EstimatedEnd       DATETIME,
    Start              DATETIME,
    [End]              DATETIME,
    ActionType         VARCHAR(10),
    ActionDate         DATETIME,
    ActionBy           VARCHAR(128)
)

CREATE TABLE TripCategoriesAudit
(
    AuditID    INT IDENTITY (1,1) PRIMARY KEY,
    TripID     INT,
    CatID      INT,
    ActionType VARCHAR(10),
    ActionDate DATETIME,
    ActionBy   VARCHAR(128)
)

CREATE TABLE TripAccessibilityAudit
(
    AuditID         INT IDENTITY (1,1) PRIMARY KEY,
    TripID          INT,
    AccessibilityID INT,
    ActionType      VARCHAR(10),
    ActionDate      DATETIME,
    ActionBy        VARCHAR(128)
)

CREATE TABLE CategoriesAudit
(
    AuditID    INT IDENTITY (1,1) PRIMARY KEY,
    CatID      INT,
    Type       VARCHAR(30),
    ActionType VARCHAR(10),
    ActionDate DATETIME,
    ActionBy   VARCHAR(128)
)

CREATE TABLE AccessibilityOptionsAudit
(
    AuditID         INT IDENTITY (1,1) PRIMARY KEY,
    AccessibilityID INT,
    [Option]        VARCHAR(30),
    ActionType      VARCHAR(10),
    ActionDate      DATETIME,
    ActionBy        VARCHAR(128)
)

GO

CREATE TRIGGER TripTransportationTrigger
    ON Trip_Transportation
    AFTER INSERT, UPDATE, DELETE AS
BEGIN
    IF EXISTS (SELECT * FROM inserted) AND EXISTS (SELECT * FROM deleted) -- UPDATE
        BEGIN
            INSERT INTO TripTransportationAudit (TripID, TransportationType, EstimatedStart, EstimatedEnd, Start, [End],
                                                 ActionType, ActionDate, ActionBy)
            SELECT TripID,
                   TransportationType,
                   EstimatedStart,
                   EstimatedEnd,
                   Start,
                   [End],
                   'UPDATE',
                   GETDATE(),
                   SYSTEM_USER
            FROM inserted
        END
    ELSE
        IF EXISTS (SELECT * FROM inserted) -- INSERT
            BEGIN
                INSERT INTO TripTransportationAudit (TripID, TransportationType, EstimatedStart, EstimatedEnd, Start,
                                                     [End], ActionType, ActionDate, ActionBy)
                SELECT TripID,
                       TransportationType,
                       EstimatedStart,
                       EstimatedEnd,
                       Start,
                       [End],
                       'INSERT',
                       GETDATE(),
                       SYSTEM_USER
                FROM inserted
            END
        ELSE
            IF EXISTS (SELECT * FROM deleted) -- DELETE
                BEGIN
                    INSERT INTO TripTransportationAudit (TripID, TransportationType, EstimatedStart, EstimatedEnd,
                                                         Start, [End], ActionType, ActionDate, ActionBy)
                    SELECT TripID,
                           TransportationType,
                           EstimatedStart,
                           EstimatedEnd,
                           Start,
                           [End],
                           'DELETE',
                           GETDATE(),
                           SYSTEM_USER
                    FROM deleted
                END
END;

GO

CREATE TRIGGER TripCategoriesTrigger
    ON Trip_Categories
    AFTER INSERT, UPDATE, DELETE AS
BEGIN
    IF EXISTS (SELECT * FROM inserted) AND EXISTS (SELECT * FROM deleted) -- UPDATE
        BEGIN
            INSERT INTO TripCategoriesAudit (TripID, CatID, ActionType, ActionDate, ActionBy)
            SELECT TripID, CatID, 'UPDATE', GETDATE(), SYSTEM_USER
            FROM inserted
        END
    ELSE
        IF EXISTS (SELECT * FROM inserted) -- INSERT
            BEGIN
                INSERT INTO TripCategoriesAudit (TripID, CatID, ActionType, ActionDate, ActionBy)
                SELECT TripID,
                       CatID,
                       'INSERT',
                       GETDATE(),
                       SYSTEM_USER
                FROM inserted
            END
        ELSE
            IF EXISTS (SELECT * FROM deleted) -- DELETE
                BEGIN
                    INSERT INTO TripCategoriesAudit (TripID, CatID, ActionType, ActionDate, ActionBy)
                    SELECT TripID,
                           CatID,
                           'DELETE',
                           GETDATE(),
                           SYSTEM_USER
                    FROM deleted
                END
END;

GO

CREATE TRIGGER TripAccessibilityTrigger
    ON Trip_Accessibility
    AFTER INSERT, UPDATE, DELETE AS
BEGIN
    IF EXISTS (SELECT * FROM inserted) AND EXISTS (SELECT * FROM deleted) -- UPDATE
        BEGIN
            INSERT INTO TripAccessibilityAudit (TripID, AccessibilityID, ActionType, ActionDate, ActionBy)
            SELECT TripID,
                   AccessibilityID,
                   'UPDATE',
                   GETDATE(),
                   SYSTEM_USER
            FROM inserted
        END
    ELSE
        IF EXISTS (SELECT * FROM inserted) -- INSERT
            BEGIN
                INSERT INTO TripAccessibilityAudit (TripID, AccessibilityID, ActionType, ActionDate, ActionBy)
                SELECT TripID,
                       AccessibilityID,
                       'INSERT',
                       GETDATE(),
                       SYSTEM_USER
                FROM inserted
            END
        ELSE
            IF EXISTS (SELECT * FROM deleted) -- DELETE
                BEGIN
                    INSERT INTO TripAccessibilityAudit (TripID, AccessibilityID, ActionType, ActionDate, ActionBy)
                    SELECT TripID,
                           AccessibilityID,
                           'DELETE',
                           GETDATE(),
                           SYSTEM_USER
                    FROM deleted
                END
END;

GO

CREATE TRIGGER CategoriesTrigger
    ON Categories
    AFTER INSERT, UPDATE, DELETE AS
BEGIN
    IF EXISTS (SELECT * FROM inserted) AND EXISTS (SELECT * FROM deleted) -- UPDATE
        BEGIN
            INSERT INTO CategoriesAudit (CatID, Type, ActionType, ActionDate, ActionBy)
            SELECT CatID,
                   Type,
                   'UPDATE',
                   GETDATE(),
                   SYSTEM_USER
            FROM inserted
        END
    ELSE
        IF EXISTS (SELECT * FROM inserted) -- INSERT
            BEGIN
                INSERT INTO CategoriesAudit (CatID, Type, ActionType, ActionDate, ActionBy)
                SELECT CatID,
                       Type,
                       'INSERT',
                       GETDATE(),
                       SYSTEM_USER
                FROM inserted
            END
        ELSE
            IF EXISTS (SELECT * FROM deleted) -- DELETE
                BEGIN
                    INSERT INTO CategoriesAudit (CatID, Type, ActionType, ActionDate, ActionBy)
                    SELECT CatID,
                           Type,
                           'DELETE',
                           GETDATE(),
                           SYSTEM_USER
                    FROM deleted
                END
END;

GO

CREATE TRIGGER AccessibilityOptionsTrigger
    ON Accessibility_Options
    AFTER INSERT, UPDATE, DELETE AS
BEGIN
    IF EXISTS (SELECT * FROM inserted) AND EXISTS (SELECT * FROM deleted) -- UPDATE
        BEGIN
            INSERT INTO AccessibilityOptionsAudit (AccessibilityID, [Option], ActionType, ActionDate, ActionBy)
            SELECT AccessibilityID,
                   [Option],
                   'UPDATE',
                   GETDATE(),
                   SYSTEM_USER
            FROM inserted
        END
    ELSE
        IF EXISTS (SELECT * FROM inserted) -- INSERT
            BEGIN
                INSERT INTO AccessibilityOptionsAudit (AccessibilityID, [Option], ActionType, ActionDate, ActionBy)
                SELECT AccessibilityID,
                       [Option],
                       'INSERT',
                       GETDATE(),
                       SYSTEM_USER
                FROM inserted
            END
        ELSE
            IF EXISTS (SELECT * FROM deleted) -- DELETE
                BEGIN
                    INSERT INTO AccessibilityOptionsAudit (AccessibilityID, [Option], ActionType, ActionDate, ActionBy)
                    SELECT AccessibilityID,
                           [Option],
                           'DELETE',
                           GETDATE(),
                           SYSTEM_USER
                    FROM deleted
                END
END;

GO

CREATE TABLE RoomAudit
(
    AuditID    INT IDENTITY (1,1) PRIMARY KEY,
    HUsername  VARCHAR(50),
    RoomNumber INT,
    Price      INT,
    ActionType VARCHAR(10),
    ActionDate DATETIME,
    ActionBy   VARCHAR(128)
)

CREATE TABLE RoomBookingAudit
(
    AuditID     INT IDENTITY (1,1) PRIMARY KEY,
    HUsername   VARCHAR(50),
    RoomID      INT,
    Username    VARCHAR(30),
    StartDate   DATETIME,
    BookingDate DATETIME,
    EndDate     DATETIME,
    ActionType  VARCHAR(10),
    ActionDate  DATETIME,
    ActionBy    VARCHAR(128)
)

CREATE TABLE RoomServiceAudit
(
    AuditID     INT IDENTITY (1,1) PRIMARY KEY,
    HUsername   VARCHAR(50),
    RoomNumber  INT,
    ServiceType VARCHAR(30),
    ActionType  VARCHAR(10),
    ActionDate  DATETIME,
    ActionBy    VARCHAR(128)
)

GO

CREATE TRIGGER RoomTrigger
    ON Room
    AFTER INSERT, UPDATE, DELETE AS
BEGIN
    IF EXISTS (SELECT * FROM inserted) AND EXISTS (SELECT * FROM deleted) -- UPDATE
        BEGIN
            INSERT INTO RoomAudit (HUsername, RoomNumber, Price, ActionType, ActionDate, ActionBy)
            SELECT HUsername,
                   RoomNumber,
                   Price,
                   'UPDATE',
                   GETDATE(),
                   SYSTEM_USER
            FROM inserted
        END
    ELSE
        IF EXISTS (SELECT * FROM inserted) -- INSERT
            BEGIN
                INSERT INTO RoomAudit (HUsername, RoomNumber, Price, ActionType, ActionDate, ActionBy)
                SELECT HUsername,
                       RoomNumber,
                       Price,
                       'INSERT',
                       GETDATE(),
                       SYSTEM_USER
                FROM inserted
            END
        ELSE
            IF EXISTS (SELECT * FROM deleted) -- DELETE
                BEGIN
                    INSERT INTO RoomAudit (HUsername, RoomNumber, Price, ActionType, ActionDate, ActionBy)
                    SELECT HUsername,
                           RoomNumber,
                           Price,
                           'DELETE',
                           GETDATE(),
                           SYSTEM_USER
                    FROM deleted
                END
END;

GO

CREATE TRIGGER RoomBookingTrigger
    ON Room_Booking
    AFTER INSERT, UPDATE, DELETE AS
BEGIN
    IF EXISTS (SELECT * FROM inserted) AND EXISTS (SELECT * FROM deleted) -- UPDATE
        BEGIN
            INSERT INTO RoomBookingAudit (HUsername, RoomID, Username, StartDate, BookingDate, EndDate,
                                          ActionType, ActionDate, ActionBy)
            SELECT HUsername,
                   RoomID,
                   Username,
                   StartDate,
                   BookingDate,
                   EndDate,
                   'UPDATE',
                   GETDATE(),
                   SYSTEM_USER
            FROM inserted
        END
    ELSE
        IF EXISTS (SELECT * FROM inserted) -- INSERT
            BEGIN
                INSERT INTO RoomBookingAudit (HUsername, RoomID, Username, StartDate, BookingDate, EndDate,
                                              ActionType, ActionDate, ActionBy)
                SELECT HUsername,
                       RoomID,
                       Username,
                       StartDate,
                       BookingDate,
                       EndDate,
                       'INSERT',
                       GETDATE(),
                       SYSTEM_USER
                FROM inserted
            END
        ELSE
            IF EXISTS (SELECT * FROM deleted) -- DELETE
                BEGIN
                    INSERT INTO RoomBookingAudit (HUsername, RoomID, Username, StartDate, BookingDate, EndDate,
                                                  ActionType, ActionDate, ActionBy)
                    SELECT HUsername,
                           RoomID,
                           Username,
                           StartDate,
                           BookingDate,
                           EndDate,
                           'DELETE',
                           GETDATE(),
                           SYSTEM_USER
                    FROM deleted
                END
END;

GO

CREATE TRIGGER RoomServiceTrigger
    ON Room_Service
    AFTER INSERT, UPDATE, DELETE AS
BEGIN
    IF EXISTS (SELECT * FROM inserted) AND EXISTS (SELECT * FROM deleted) -- UPDATE
        BEGIN
            INSERT INTO RoomServiceAudit (HUsername, RoomNumber, ServiceType,
                                          ActionType, ActionDate, ActionBy)
            SELECT HUsername,
                   RoomNumber,
                   ServiceType,
                   'UPDATE',
                   GETDATE(),
                   SYSTEM_USER
            FROM inserted
        END
    ELSE
        IF EXISTS (SELECT * FROM inserted) -- INSERT
            BEGIN
                INSERT INTO RoomServiceAudit (HUsername, RoomNumber, ServiceType,
                                              ActionType, ActionDate, ActionBy)
                SELECT HUsername,
                       RoomNumber,
                       ServiceType,
                       'INSERT',
                       GETDATE(),
                       SYSTEM_USER
                FROM inserted
            END
        ELSE
            IF EXISTS (SELECT * FROM deleted) -- DELETE
                BEGIN
                    INSERT INTO RoomServiceAudit (HUsername, RoomNumber, ServiceType,
                                                  ActionType, ActionDate, ActionBy)
                    SELECT HUsername,
                           RoomNumber,
                           ServiceType,
                           'DELETE',
                           GETDATE(),
                           SYSTEM_USER
                    FROM deleted
                END
END;

GO

CREATE TABLE TravellerCartAudit
(
    AuditID    INT IDENTITY (1,1) PRIMARY KEY,
    Username   VARCHAR(30),
    TripID     INT,
    ActionType VARCHAR(10),
    ActionDate DATETIME,
    ActionBy   VARCHAR(128)
)

CREATE TABLE TravellerWishlistAudit
(
    AuditID    INT IDENTITY (1,1) PRIMARY KEY,
    Username   VARCHAR(30),
    TripID     INT,
    ActionType VARCHAR(10),
    ActionDate DATETIME,
    ActionBy   VARCHAR(128)
)

GO

CREATE TRIGGER TravellerCartTrigger
    ON Traveller_Cart
    AFTER INSERT, UPDATE, DELETE AS
BEGIN
    IF EXISTS (SELECT * FROM inserted) AND EXISTS (SELECT * FROM deleted) -- UPDATE
        BEGIN
            INSERT INTO TravellerCartAudit (Username, TripID, ActionType, ActionDate, ActionBy)
            SELECT Username,
                   TripID,
                   'UPDATE',
                   GETDATE(),
                   SYSTEM_USER
            FROM inserted
        END
    ELSE
        IF EXISTS (SELECT * FROM inserted) -- INSERT
            BEGIN
                INSERT INTO TravellerCartAudit (Username, TripID, ActionType, ActionDate, ActionBy)
                SELECT Username,
                       TripID,
                       'INSERT',
                       GETDATE(),
                       SYSTEM_USER
                FROM inserted
            END
        ELSE
            IF EXISTS (SELECT * FROM deleted) -- DELETE
                BEGIN
                    INSERT INTO TravellerCartAudit (Username, TripID, ActionType, ActionDate, ActionBy)
                    SELECT Username,
                           TripID,
                           'DELETE',
                           GETDATE(),
                           SYSTEM_USER
                    FROM deleted
                END
END;

GO

CREATE TRIGGER TravellerWishlistTrigger
    ON Traveller_Wishlist
    AFTER INSERT, UPDATE, DELETE AS
BEGIN
    IF EXISTS (SELECT * FROM inserted) AND EXISTS (SELECT * FROM deleted) -- UPDATE
        BEGIN
            INSERT INTO TravellerWishlistAudit (Username, TripID, ActionType, ActionDate, ActionBy)
            SELECT Username,
                   TripID,
                   'UPDATE',
                   GETDATE(),
                   SYSTEM_USER
            FROM inserted
        END
    ELSE
        IF EXISTS (SELECT * FROM inserted) -- INSERT
            BEGIN
                INSERT INTO TravellerWishlistAudit (Username, TripID, ActionType, ActionDate, ActionBy)
                SELECT Username,
                       TripID,
                       'INSERT',
                       GETDATE(),
                       SYSTEM_USER
                FROM inserted
            END
        ELSE
            IF EXISTS (SELECT * FROM deleted) -- DELETE
                BEGIN
                    INSERT INTO TravellerWishlistAudit (Username, TripID, ActionType, ActionDate, ActionBy)
                    SELECT Username,
                           TripID,
                           'DELETE',
                           GETDATE(),
                           SYSTEM_USER
                    FROM deleted
                END
END;

GO