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
