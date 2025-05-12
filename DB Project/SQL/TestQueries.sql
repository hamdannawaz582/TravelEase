--==============================TRAVELLER=============================
--====================================================================
--for checking data
SELECT * FROM [User];
SELECT * FROM Traveller;
SELECT * FROM Admin;
select* from Operator;
SELECT * FROM Trip_Booking;
SELECT * FROM Trip_Itinerary;
--IbnBatuta's password = 'worldtraveller123'
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
-- Search trips with various filters
SELECT DISTINCT t.TripID, t.Title, t.Type, t.CancellationPolicy,
                t.GroupSize, t.StartDate, t.EndDate, t.PriceRange,
                d.City + ', ' + d.Country AS Destination
FROM Trip t
         JOIN Trip_Destination td ON t.TripID = td.TripID
         JOIN Destination d ON td.DestID = d.DestID
WHERE (d.City LIKE '%Paris%' OR d.Country LIKE '%France%') -- Example destination
  AND t.StartDate >= '2023-06-01'  -- Example start date
  AND t.PriceRange <= 2000         -- Example max price
  AND t.GroupSize BETWEEN 5 AND 15; -- Example group size range

SELECT
    r.ReviewID,
    r.Stars,
    r.Feedback,
    r.Response,
    r.ReviewTime,
    r.ResponseTime,

    -- Determine review type and details
    CASE
        WHEN tr.ReviewID IS NOT NULL THEN 'Trip'
        WHEN hr.ReviewID IS NOT NULL THEN 'Hotel'
        WHEN dr.ReviewID IS NOT NULL THEN 'Destination'
        ELSE 'Unknown'
        END as ReviewType,

    -- Get entity name based on review type
    CASE
        WHEN tr.ReviewID IS NOT NULL THEN t.Title
        WHEN hr.ReviewID IS NOT NULL THEN h.Name
        WHEN dr.ReviewID IS NOT NULL THEN (d.City + ', ' + d.Country)
        ELSE NULL
        END as ReviewedItem

FROM Review r
         LEFT JOIN TripReview tr ON r.ReviewID = tr.ReviewID
         LEFT JOIN HotelReview hr ON r.ReviewID = hr.ReviewID
         LEFT JOIN DestinationReview dr ON r.ReviewID = dr.ReviewID

-- Join with entity tables
         LEFT JOIN Trip t ON tr.TripID = t.TripID
         LEFT JOIN Hotel h ON hr.HUsername = h.HUsername
         LEFT JOIN Destination d ON dr.DestinationID = d.DestID

WHERE r.Reviewer = 'user088'  -- Replace with the username you want to check
ORDER BY r.ReviewTime DESC;