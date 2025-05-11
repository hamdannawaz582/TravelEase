--==============================TRAVELLER=============================
--====================================================================
-- Insert Operators first (referenced in Trip)
INSERT INTO [User] (Username, Email, Password, JoinDate)
VALUES
    ('operator1', 'operator1@test.com', 'password123', '2022-01-15'),
    ('operator2', 'operator2@test.com', 'password123', '2022-02-20'),
    ('operator3', 'operator3@test.com', 'password123', '2022-03-10');

INSERT INTO Operator (Username)
VALUES
    ('operator1'),
    ('operator2'),
    ('operator3');

-- 1. Insert User and Traveller profile
INSERT INTO [User] (Username, Email, Password, JoinDate)
VALUES ('IbnBatuta', 'ibnbatuta@explorers.com', 'worldtraveller123', '2023-01-15');

INSERT INTO Traveller (Username, FName, LName, Nationality, Age, Budget)
VALUES ('IbnBatuta', 'Ibn', 'Batuta', 'Moroccan', 35, 15000);

-- 2. Insert Destinations
INSERT INTO Destination (City, Country, Location, JoinDate)
VALUES
    ('Istanbul', 'Turkey', 'Europe/Asia', '2022-01-10'),
    ('Cairo', 'Egypt', 'Africa', '2022-01-10'),
    ('Delhi', 'India', 'Asia', '2022-01-10'),
    ('Beijing', 'China', 'Asia', '2022-01-10'),
    ('Marrakech', 'Morocco', 'Africa', '2022-01-10');

-- Get the automatically generated DestIDs
DECLARE @Istanbul INT, @Cairo INT, @Delhi INT, @Beijing INT, @Marrakech INT;
SELECT @Istanbul = DestID FROM Destination WHERE City = 'Istanbul';
SELECT @Cairo = DestID FROM Destination WHERE City = 'Cairo';
SELECT @Delhi = DestID FROM Destination WHERE City = 'Delhi';
SELECT @Beijing = DestID FROM Destination WHERE City = 'Beijing';
SELECT @Marrakech = DestID FROM Destination WHERE City = 'Marrakech';

-- 3. Insert Hotels (simplified to match schema)
INSERT INTO Hotel (HUsername, Name)
VALUES
    ('ottoman_palace', 'Ottoman Palace'),
    ('pyramid_view', 'Pyramid View Resort'),
    ('taj_retreat', 'Taj Retreat'),
    ('forbidden_inn', 'Forbidden City Inn'),
    ('medina_riad', 'Medina Riad & Spa');

-- 4. Insert Trips (past and future)
-- Past trips
INSERT INTO Trip (Title, Type, CancelStatus, CancellationPolicy, GroupSize, StartDate, EndDate, PriceRange, OperatorUsername)
VALUES
    ('Turkish Delight Tour', 'Cultural', 0, 'Refundable', 15, '2023-03-10', '2023-03-17', 2500, 'operator1'),
    ('Egyptian Wonders', 'Historical', 0, 'Non-Refundable', 12, '2023-06-15', '2023-06-22', 3000, 'operator2'),
    ('Moroccan Magic', 'Adventure', 0, 'Refundable', 10, '2023-09-05', '2023-09-12', 2200, 'operator1');

-- Future trips
INSERT INTO Trip (Title, Type, CancelStatus, CancellationPolicy, GroupSize, StartDate, EndDate, PriceRange, OperatorUsername)
VALUES
    ('Indian Heritage Tour', 'Cultural', 0, 'Refundable', 18, '2025-04-20', '2025-04-30', 2800, 'operator3'),
    ('China Expedition', 'Adventure', 0, 'Refundable', 15, '2025-07-10', '2025-07-20', 3500, 'operator2');

-- Get the automatically generated TripIDs
DECLARE @TurkishTour INT, @EgyptianTour INT, @MoroccanTour INT, @IndianTour INT, @ChinaTour INT;
SELECT @TurkishTour = TripID FROM Trip WHERE Title = 'Turkish Delight Tour';
SELECT @EgyptianTour = TripID FROM Trip WHERE Title = 'Egyptian Wonders';
SELECT @MoroccanTour = TripID FROM Trip WHERE Title = 'Moroccan Magic';
SELECT @IndianTour = TripID FROM Trip WHERE Title = 'Indian Heritage Tour';
SELECT @ChinaTour = TripID FROM Trip WHERE Title = 'China Expedition';

-- 5. Link Trips with Destinations
INSERT INTO Trip_Destination (TripID, DestID)
VALUES
    (@TurkishTour, @Istanbul),
    (@EgyptianTour, @Cairo),
    (@MoroccanTour, @Marrakech),
    (@IndianTour, @Delhi),
    (@ChinaTour, @Beijing);

-- 6. Create Bookings for IbnBatuta
INSERT INTO Trip_Booking (Username, TripID)
VALUES
    ('IbnBatuta', @TurkishTour),
    ('IbnBatuta', @EgyptianTour),
    ('IbnBatuta', @MoroccanTour),
    ('IbnBatuta', @IndianTour),
    ('IbnBatuta', @ChinaTour);

-- 7. Link Trips with Hotels
INSERT INTO Trip_Hotels (TripID, HUsername)
VALUES
    (@TurkishTour, 'ottoman_palace'),
    (@EgyptianTour, 'pyramid_view'),
    (@MoroccanTour, 'medina_riad'),
    (@IndianTour, 'taj_retreat'),
    (@ChinaTour, 'forbidden_inn');

-- 8. Create Trip Itineraries
-- For Turkish Delight Tour (past)
INSERT INTO Trip_Itinerary (TripID, Event, EventStartDate, EventEndDate)
VALUES
    (@TurkishTour, 'Arrival and Hotel Check-in', '2023-03-10 14:00', '2023-03-10 16:00'),
    (@TurkishTour, 'Visit Blue Mosque', '2023-03-11 09:00', '2023-03-11 12:00'),
    (@TurkishTour, 'Grand Bazaar Shopping', '2023-03-12 10:00', '2023-03-12 15:00'),
    (@TurkishTour, 'Bosphorus Cruise', '2023-03-13 13:00', '2023-03-13 17:00'),
    (@TurkishTour, 'Return Flight', '2023-03-17 10:00', '2023-03-17 13:00');

-- For Indian Heritage Tour (future)
INSERT INTO Trip_Itinerary (TripID, Event, EventStartDate, EventEndDate)
VALUES
    (@IndianTour, 'Arrival in Delhi', '2025-04-20 10:00', '2025-04-20 12:00'),
    (@IndianTour, 'Taj Mahal Visit', '2025-04-22 08:00', '2025-04-22 14:00'),
    (@IndianTour, 'Red Fort Tour', '2025-04-24 09:00', '2025-04-24 13:00'),
    (@IndianTour, 'Local Cuisine Experience', '2025-04-26 18:00', '2025-04-26 22:00'),
    (@IndianTour, 'Departure from Delhi', '2025-04-30 15:00', '2025-04-30 18:00');

select* from Trip
DECLARE @ChinaTour INT = 80;
select * from Trip where TripID = @ChinaTour;
INSERT INTO Trip_Itinerary (TripID, Event, EventStartDate, EventEndDate)
VALUES
-- Day 1: Arrival and Welcome
(@ChinaTour, 'Airport Pickup nd HotelCheckin', '2025-05-10 10:00:00', '2025-05-10 14:00:00'),
(@ChinaTour, 'Welcome Dinner', '2025-05-10 18:00:00', '2025-05-10 20:30:00'),
(@ChinaTour, 'Great Wall of China Excursion', '2025-05-12 08:00:00', '2025-05-12 16:00:00'),
(@ChinaTour, 'Farewell Dinner', '2025-05-19 18:00:00', '2025-05-19 21:00:00'),
(@ChinaTour, 'HotelCheckout &AirportTransfer', '2025-05-20 09:00:00', '2025-05-20 12:00:00');
select* from Trip_Itinerary where TripID = @ChinaTour;
SELECT 'China Tour Itinerary created successfully.' AS Status;
--====================================================================
--====================================================================
-- 1. Insert Room data
INSERT INTO Room (HUsername, RoomNumber, Price)
VALUES
    ('ottoman_palace', 101, 150),
    ('ottoman_palace', 102, 180),
    ('pyramid_view', 201, 200),
    ('pyramid_view', 202, 250),
    ('taj_retreat', 301, 120),
    ('taj_retreat', 302, 160),
    ('forbidden_inn', 401, 180),
    ('forbidden_inn', 402, 220),
    ('medina_riad', 501, 140),
    ('medina_riad', 502, 190);

-- 2. Insert Room_Service data
INSERT INTO Room_Service (HUsername, RoomNumber, ServiceType)
VALUES
    ('ottoman_palace', 101, 'WiFi'),
    ('ottoman_palace', 101, 'Breakfast'),
    ('pyramid_view', 201, 'WiFi'),
    ('pyramid_view', 201, 'Room Service'),
    ('taj_retreat', 301, 'WiFi'),
    ('taj_retreat', 301, 'Pool Access'),
    ('forbidden_inn', 401, 'WiFi'),
    ('forbidden_inn', 401, 'Guided Tours'),
    ('medina_riad', 501, 'WiFi'),
    ('medina_riad', 501, 'Spa Access');

-- 3. Insert Room_Booking data
INSERT INTO Room_Booking (HUsername, RoomID, Username, StartDate, BookingDate, EndDate)
VALUES
    ('ottoman_palace', 101, 'IbnBatuta', '2023-03-10', '2023-02-15', '2023-03-17'),
    ('pyramid_view', 201, 'IbnBatuta', '2023-06-15', '2023-05-20', '2023-06-22'),
    ('medina_riad', 501, 'IbnBatuta', '2023-09-05', '2023-08-01', '2023-09-12'),
    ('taj_retreat', 301, 'IbnBatuta', '2025-04-20', '2025-01-10', '2025-04-30'),
    ('forbidden_inn', 401, 'IbnBatuta', '2025-07-10', '2025-03-05', '2025-07-20');

-- 4. Insert Categories data
INSERT INTO Categories (Type)
VALUES
    ('Beach'),
    ('Mountain'),
    ('Cultural'),
    ('Historical'),
    ('Adventure'),
    ('Relaxation'),
    ('Food & Wine');

-- 5. Link Trips with Categories
DECLARE @TurkishTour INT, @EgyptianTour INT, @MoroccanTour INT, @IndianTour INT, @ChinaTour INT;
SELECT @TurkishTour = TripID FROM Trip WHERE Title = 'Turkish Delight Tour';
SELECT @EgyptianTour = TripID FROM Trip WHERE Title = 'Egyptian Wonders';
SELECT @MoroccanTour = TripID FROM Trip WHERE Title = 'Moroccan Magic';
SELECT @IndianTour = TripID FROM Trip WHERE Title = 'Indian Heritage Tour';
SELECT @ChinaTour = TripID FROM Trip WHERE Title = 'China Expedition';

INSERT INTO Trip_Categories (TripID, CatID)
VALUES
    (@TurkishTour, 3), -- Cultural
    (@TurkishTour, 4), -- Historical
    (@EgyptianTour, 4), -- Historical
    (@MoroccanTour, 5), -- Adventure
    (@MoroccanTour, 3), -- Cultural
    (@IndianTour, 3), -- Cultural
    (@IndianTour, 4), -- Historical
    (@ChinaTour, 5), -- Adventure
    (@ChinaTour, 3); -- Cultural

-- 6. Insert Accessibility_Options data
INSERT INTO Accessibility_Options ([Option])
VALUES
    ('Wheelchair Accessible'),
    ('Sign Language Available'),
    ('Audio Descriptions'),
    ('No Stairs'),
    ('Dietary Accommodations');

-- 7. Link Trips with Accessibility Options
INSERT INTO Trip_Accessibility (TripID, AccessibilityID)
VALUES
    (@TurkishTour, 1), -- Wheelchair Accessible
    (@TurkishTour, 5), -- Dietary Accommodations
    (@EgyptianTour, 5), -- Dietary Accommodations
    (@MoroccanTour, 5), -- Dietary Accommodations
    (@IndianTour, 1), -- Wheelchair Accessible
    (@IndianTour, 2), -- Sign Language Available
    (@IndianTour, 5), -- Dietary Accommodations
    (@ChinaTour, 1), -- Wheelchair Accessible
    (@ChinaTour, 5); -- Dietary Accommodations

-- 8. Insert Inclusions data
INSERT INTO Inclusions ([Type], Price)
VALUES
    ('Guided Tours', 50),
    ('Airport Transfer', 30),
    ('Meals', 40),
    ('Equipment Rental', 20),
    ('Entry Tickets', 25);

-- 9. Link Trips with Inclusions
INSERT INTO Trip_Inclusions (TripID, IncType, IncPrice)
VALUES
    (@TurkishTour, 'Guided Tours', 50),
    (@TurkishTour, 'Airport Transfer', 30),
    (@TurkishTour, 'Meals', 40),
    (@EgyptianTour, 'Guided Tours', 60),
    (@EgyptianTour, 'Entry Tickets', 30),
    (@MoroccanTour, 'Guided Tours', 45),
    (@MoroccanTour, 'Meals', 35),
    (@IndianTour, 'Guided Tours', 55),
    (@IndianTour, 'Airport Transfer', 25),
    (@IndianTour, 'Meals', 40),
    (@ChinaTour, 'Guided Tours', 65),
    (@ChinaTour, 'Equipment Rental', 20),
    (@ChinaTour, 'Meals', 45);

-- 10. Insert Trip_Transportation data
INSERT INTO Trip_Transportation (TripID, TransportationType, EstimatedStart, EstimatedEnd, Start, [End])
VALUES
    (@TurkishTour, 'Flight', '2023-03-10 08:00', '2023-03-10 12:00', '2023-03-10 08:15', '2023-03-10 12:10'),
    (@TurkishTour, 'Bus', '2023-03-17 14:00', '2023-03-17 16:00', '2023-03-17 14:00', '2023-03-17 15:45'),
    (@EgyptianTour, 'Flight', '2023-06-15 09:00', '2023-06-15 13:00', '2023-06-15 09:10', '2023-06-15 13:20'),
    (@MoroccanTour, 'Train', '2023-09-05 10:00', '2023-09-05 15:00', '2023-09-05 10:05', '2023-09-05 15:10'),
    (@IndianTour, 'Flight', '2025-04-20 07:00', '2025-04-20 09:00', NULL, NULL),
    (@ChinaTour, 'Flight', '2025-07-10 06:00', '2025-07-10 12:00', NULL, NULL);

-- 11. Insert Reviews and Trip Reviews (for past trips)
INSERT INTO Review (Stars, Feedback, ReviewTime, Reviewer)
VALUES
    (5, 'Excellent tour of Istanbul! The Blue Mosque was breathtaking.', '2023-03-20', 'IbnBatuta'),
    (4, 'Great experience in Egypt. The pyramids were amazing.', '2023-07-01', 'IbnBatuta'),
    (5, 'Loved Marrakech! The food and culture were fantastic.', '2023-09-20', 'IbnBatuta');

DECLARE @Review1 INT, @Review2 INT, @Review3 INT;
SET @Review1 = SCOPE_IDENTITY();
SET @Review2 = @Review1 - 1;
SET @Review3 = @Review1 - 2;

INSERT INTO TripReview (ReviewID, TripID)
VALUES
    (@Review3, @TurkishTour),
    (@Review2, @EgyptianTour),
    (@Review1, @MoroccanTour);

-- 12. Insert Hotel Reviews
INSERT INTO Review (Stars, Feedback, ReviewTime, Reviewer)
VALUES
    (5, 'Ottoman Palace was luxurious and staff were very friendly.', '2023-03-18', 'IbnBatuta'),
    (3, 'Pyramid View had great location but rooms need updating.', '2023-06-23', 'IbnBatuta'),
    (4, 'Medina Riad was beautiful with excellent service.', '2023-09-15', 'IbnBatuta');

DECLARE @HotelReview1 INT, @HotelReview2 INT, @HotelReview3 INT;
SET @HotelReview1 = SCOPE_IDENTITY();
SET @HotelReview2 = @HotelReview1 - 1;
SET @HotelReview3 = @HotelReview1 - 2;

INSERT INTO HotelReview (ReviewID, HUsername)
VALUES
    (@HotelReview3, 'ottoman_palace'),
    (@HotelReview2, 'pyramid_view'),
    (@HotelReview1, 'medina_riad');

-- 13. Insert Traveller_Wishlist data
INSERT INTO Traveller_Wishlist (Username, TripID)
VALUES ('IbnBatuta', @ChinaTour);

-- 14. Insert Admin user
INSERT INTO [User] (Username, Email, Password, JoinDate)
VALUES ('admin1', 'admin@travelease.com', 'adminpass123', '2022-01-01');

INSERT INTO Admin (Username)
VALUES ('admin1');
