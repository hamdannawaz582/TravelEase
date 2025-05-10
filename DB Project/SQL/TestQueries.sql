--==============================TRAVELLER=============================
--====================================================================
--for checking data
SELECT * FROM [User];
SELECT * FROM Traveller;
SELECT * FROM Trip_Booking;
SELECT * FROM Trip_Itinerary;
--IbnBatuta's Password is 'worldtraveller123'
--===========================
DECLARE @IndianTour INT = (SELECT TripID FROM Trip WHERE Title = 'Indian Heritage Tour');
DECLARE @TurkishTour INT = (SELECT TripID FROM Trip WHERE Title = 'Turkish Tour');
-- 1. Get profile information for a specific user
SELECT u.Username, u.Email, u.JoinDate, t.FName, t.LName, t.Nationality, t.Age, t.Budget
FROM [User] u
         JOIN Traveller t ON u.Username = t.Username
WHERE u.Username = 'IbnBatuta';

-- 2. Get upcoming trips for the user
SELECT t.TripID, t.Title, t.Type, t.CancellationPolicy,
       t.StartDate, t.EndDate, t.PriceRange,
       d.City + ', ' + d.Country AS Destination
FROM Trip t
         JOIN Trip_Booking tb ON t.TripID = tb.TripID
         JOIN Trip_Destination td ON t.TripID = td.TripID
         JOIN Destination d ON td.DestID = d.DestID
WHERE tb.Username = 'IbnBatuta' AND t.StartDate > GETDATE();

-- 3. Get travel history for the user
SELECT t.TripID, t.Title, t.StartDate, t.EndDate
FROM Trip t
         JOIN Trip_Booking tb ON t.TripID = tb.TripID
WHERE tb.Username = 'IbnBatuta' AND t.EndDate < GETDATE();

-- 4. Get itineraries for specific trips
-- For Indian Heritage Tour (future trip)
SELECT ti.TripID, t.Title, ti.Event, ti.EventStartDate, ti.EventEndDate
FROM Trip_Itinerary ti
         JOIN Trip t ON ti.TripID = t.TripID
WHERE t.TripID = @IndianTour
ORDER BY ti.EventStartDate;

-- For Turkish Tour (past trip)
SELECT ti.TripID, t.Title, ti.Event, ti.EventStartDate, ti.EventEndDate
FROM Trip_Itinerary ti
         JOIN Trip t ON ti.TripID = t.TripID
WHERE t.TripID = @TurkishTour
ORDER BY ti.EventStartDate;

-- 5. Find all users booked for Indian Heritage Tour
SELECT u.Username, u.Email, t.FName, t.LName
FROM [User] u
         JOIN Traveller t ON u.Username = t.Username
         JOIN Trip_Booking tb ON u.Username = tb.Username
         JOIN Trip tr ON tb.TripID = tr.TripID
WHERE tr.TripID = @IndianTour;

-- 6. Check which hotels the user will stay at during trips
SELECT t.Title, h.Name, h.HUsername
FROM Trip t
         JOIN Trip_Booking tb ON t.TripID = tb.TripID
         JOIN Trip_Hotels th ON t.TripID = th.TripID
         JOIN Hotel h ON th.HUsername = h.HUsername
WHERE tb.Username = 'IbnBatuta';

-- 7. Get all trip itineraries organized by Event date
SELECT t.Title, ti.Event, ti.EventStartDate, ti.EventEndDate
FROM Trip_Itinerary ti
         JOIN Trip t ON ti.TripID = t.TripID
ORDER BY ti.EventStartDate;
--====================================================================
--====================================================================