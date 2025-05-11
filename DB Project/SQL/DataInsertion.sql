USE TravelEase;

INSERT INTO Destination (Location, City, Country, JoinDate) VALUES
('Beachfront Resort', 'Miami', 'USA', '2021-05-12'),
('Mountain Lodge', 'Aspen', 'USA', '2020-11-01'),
('Historic Center', 'Rome', 'Italy', '2022-01-15'),
('City Center', 'Paris', 'France', '2021-07-22'),
('Old Town', 'Prague', 'Czech Republic', '2023-03-30'),
('Lakeside Retreat', 'Lucerne', 'Switzerland', '2021-04-18'),
('Safari Base', 'Nairobi', 'Kenya', '2020-12-05'),
('Downtown', 'Tokyo', 'Japan', '2022-06-10'),
('Harbor View', 'Sydney', 'Australia', '2023-02-08'),
('Island Getaway', 'Bali', 'Indonesia', '2020-09-14'),
('Urban Hub', 'New York', 'USA', '2021-08-25'),
('Riverbank Stay', 'Budapest', 'Hungary', '2021-10-03'),
('Castle View', 'Edinburgh', 'UK', '2020-10-22'),
('Northern Lights', 'Reykjavik', 'Iceland', '2022-12-01'),
('Temple Visit', 'Kyoto', 'Japan', '2023-01-11'),
('Tropical Escape', 'Phuket', 'Thailand', '2020-11-29'),
('Desert Adventure', 'Dubai', 'UAE', '2021-03-15'),
('Wine Country', 'Cape Town', 'South Africa', '2022-04-07'),
('Scenic Fjord', 'Bergen', 'Norway', '2021-06-19'),
('Cultural Center', 'Barcelona', 'Spain', '2022-08-23'),
('Seaside Village', 'Nice', 'France', '2023-03-01'),
('Rainforest EcoStay', 'Manaus', 'Brazil', '2021-12-17'),
('Downtown Arts', 'Melbourne', 'Australia', '2020-10-10'),
('Winter Wonderland', 'Quebec City', 'Canada', '2022-11-26'),
('Hiking Trails', 'Queenstown', 'New Zealand', '2021-09-05');

INSERT INTO Hotel (HUsername, Name) VALUES
('miami_resort', 'Sunset Beach Resort'),
('aspen_lodge', 'Aspen Snow Lodge'),
('rome_historic', 'Hotel Roma Centro'),
('paris_palace', N'Château Lumière'),
('prague_oldtown', 'Golden Clock Inn'),
('lucerne_lake', 'Lakeview Paradise'),
('nairobi_safari', 'Savannah Plains Hotel'),
('tokyo_downtown', 'Tokyo Grand Stay'),
('sydney_harbor', 'Harbor Bay Suites'),
('bali_getaway', 'Bali Dream Villas'),
('nyc_urban', 'Manhattan Hub Hotel'),
('budapest_river', 'Danube View Hotel'),
('edinburgh_castle', 'Castle Rock Inn'),
('reykjavik_lights', 'Aurora Suites'),
('kyoto_temple', 'Zen Garden Hotel'),
('phuket_escape', 'Coral Cove Resort'),
('dubai_desert', 'Desert Mirage Hotel'),
('cape_wine', 'Cape Vine Retreat'),
('bergen_fjord', 'Fjord Serenity Inn'),
('barcelona_culture', 'Casa de Cultura Hotel'),
('nice_seaside', 'Azure Coast Hotel'),
('manaus_rainforest', 'Amazon Canopy Lodge'),
('melbourne_arts', 'ArtHouse Stay'),
('quebec_winter', 'Snowflake Suites'),
('queenstown_hike', 'Trailhead Hotel');

INSERT INTO Room (HUsername, RoomNumber, Price) VALUES
('miami_resort', 101, 120),
('miami_resort', 102, 140),
('aspen_lodge', 201, 200),
('rome_historic', 301, 110),
('rome_historic', 302, 130),
('paris_palace', 401, 250),
('paris_palace', 402, 260),
('prague_oldtown', 501, 90),
('lucerne_lake', 601, 160),
('nairobi_safari', 701, 180),
('tokyo_downtown', 801, 210),
('tokyo_downtown', 802, 210),
('sydney_harbor', 901, 190),
('bali_getaway', 1001, 130),
('nyc_urban', 1101, 220),
('nyc_urban', 1102, 240),
('budapest_river', 1201, 100),
('edinburgh_castle', 1301, 115),
('reykjavik_lights', 1401, 160),
('kyoto_temple', 1501, 150),
('phuket_escape', 1601, 100),
('dubai_desert', 1701, 230),
('cape_wine', 1801, 120),
('bergen_fjord', 1901, 140),
('barcelona_culture', 2001, 170),
('nice_seaside', 2101, 160),
('manaus_rainforest', 2201, 95),
('melbourne_arts', 2301, 180),
('quebec_winter', 2401, 150),
('queenstown_hike', 2501, 135);

INSERT INTO Room_Service (HUsername, RoomNumber, ServiceType) VALUES
('miami_resort', 101, 'Cleaning'),
('miami_resort', 101, 'Food Delivery'),
('aspen_lodge', 201, 'Laundry'),
('rome_historic', 301, 'Cleaning'),
('rome_historic', 302, 'Food Delivery'),
('paris_palace', 401, 'Spa Booking'),
('prague_oldtown', 501, 'Wake-up Call'),
('lucerne_lake', 601, 'Mini Bar Restock'),
('tokyo_downtown', 801, 'Room Upgrade'),
('tokyo_downtown', 802, 'Laundry'),
('bali_getaway', 1001, 'Cleaning'),
('nyc_urban', 1102, 'Food Delivery'),
('budapest_river', 1201, 'Wake-up Call'),
('edinburgh_castle', 1301, 'Room Upgrade'),
('reykjavik_lights', 1401, 'Cleaning'),
('kyoto_temple', 1501, 'Tea Service'),
('dubai_desert', 1701, 'Desert Tour Booking'),
('barcelona_culture', 2001, 'Bike Rental'),
('manaus_rainforest', 2201, 'Nature Tour Booking'),
('quebec_winter', 2401, 'Heater Request');

DECLARE @a INT = 1;
WHILE @a <= 100 -- user001 - user100
BEGIN
    INSERT INTO [User] (Username, Email, Password, JoinDate)
    VALUES (
        CONCAT('user', RIGHT('000' + CAST(@a AS VARCHAR), 3)),
        CONCAT('user', RIGHT('000' + CAST(@a AS VARCHAR), 3), '@example.com'),
        CONVERT(VARCHAR(128), HASHBYTES('SHA2_512', CONCAT('Password', @a)), 2),
        DATEADD(DAY, -ABS(CHECKSUM(NEWID())) % 1000, GETDATE())
    );
    SET @a += 1;
END
select* from [User];
DECLARE @b INT = 1;
WHILE @b <= 20
BEGIN
    INSERT INTO [Operator] (Username)
    VALUES (
        CONCAT('user', RIGHT('000' + CAST(@b AS VARCHAR), 3))
    );
    SET @b += 1;
END

DECLARE @c INT = 21;
WHILE @c <= 30
BEGIN
    INSERT INTO [Admin] (Username)
    VALUES (
        CONCAT('user', RIGHT('000' + CAST(@c AS VARCHAR), 3))
    );
    SET @c += 1;
END

DECLARE @d INT = 31;
WHILE @d <= 100
BEGIN
    INSERT INTO Traveller (Username, FName, LName, Nationality, Age, Budget)
    VALUES (
        CONCAT('user', RIGHT('000' + CAST(@d AS VARCHAR), 3)),
        CHOOSE((ABS(CHECKSUM(NEWID())) % 10) + 1, 'Alex', 'Sam', 'Taylor', 'Jordan', 'Chris', 'Morgan', 'Jamie', 'Casey', 'Riley', 'Sky'),
        CHOOSE((ABS(CHECKSUM(NEWID())) % 10) + 1, 'Smith', 'Johnson', 'Lee', 'Patel', 'Garcia', 'Kim', 'Brown', 'Singh', 'Lopez', 'Davis'),
        CASE
            (ABS(CHECKSUM(NEWID())) % 10) + 1
            WHEN 1 THEN 'USA'
            WHEN 2 THEN 'UK'
            WHEN 3 THEN 'Canada'
            WHEN 4 THEN 'India'
            WHEN 5 THEN 'Japan'
            WHEN 6 THEN 'Brazil'
            WHEN 7 THEN 'Germany'
            WHEN 8 THEN 'France'
            WHEN 9 THEN 'Australia'
            WHEN 10 THEN 'Italy'
            ELSE 'Pakistan'
            END,
        18 + (ABS(CHECKSUM(NEWID())) % 48), -- 18–65
        500 + (ABS(CHECKSUM(NEWID())) % 9501) -- 500–10000
    );
    SET @d += 1;
END


INSERT INTO Categories (Type) VALUES
('Adventure'),
('Relaxation'),
('Cultural'),
('Wildlife'),
('Beach'),
('City Tour'),
('Hiking & Trekking'),
('Cruise'),
('Skiing'),
('Historical');

DECLARE @i INT = 1;
WHILE @i <= 75
BEGIN
    INSERT INTO Trip (
        Title, Type, CancelStatus, CancellationPolicy, GroupSize, StartDate, EndDate, PriceRange, OperatorUsername
    )
    VALUES (
        CONCAT('Trip Package ', @i),

            CASE (ABS(CHECKSUM(NEWID())) % 10) + 1
                WHEN 1 THEN 'Group'
                WHEN 2 THEN 'Private'
                WHEN 3 THEN 'Solo'
                WHEN 4 THEN 'Luxury'
                WHEN 5 THEN 'Budget'
                WHEN 6 THEN 'Family'
                WHEN 7 THEN 'Road Trip'
                WHEN 8 THEN 'Volunteer'
                WHEN 9 THEN 'Eco'
                WHEN 10 THEN 'Relaxing'
                ELSE 'Misc'
                END,
        0,
        CASE (ABS(CHECKSUM(NEWID())) % 2) + 1 WHEN 1 THEN 'Refundable' ELSE 'Non-Refundable' END,
        5 + (ABS(CHECKSUM(NEWID())) % 21), -- 5–25
        DATEADD(DAY, (ABS(CHECKSUM(NEWID())) % 90), GETDATE()),
        DATEADD(DAY, (ABS(CHECKSUM(NEWID())) % 10) + 5, DATEADD(DAY, (ABS(CHECKSUM(NEWID())) % 90), GETDATE())),
        500 + (ABS(CHECKSUM(NEWID())) % 4501),
        CONCAT('user', RIGHT('000' + CAST(1 + (ABS(CHECKSUM(NEWID())) % 20) AS VARCHAR), 3))
    );
    SET @i += 1;
END

INSERT INTO Accessibility_Options ([Option])
VALUES
('Wheelchair Accessible'),
('Hearing Assistance'),
('Braille Signs'),
('Elevator Access'),
('Sign Language Interpreter'),
('Ramps for Mobility Impairment'),
('Low Vision Support'),
('Priority Seating'),
('Accessible Restrooms'),
('Visual Aid Support');

--	Trip Booking
DECLARE @n INT = 1;
WHILE @n <= 100
BEGIN
    INSERT INTO Trip_Booking (Username, TripID) SELECT TOP 1 Username, TripID FROM Traveller CROSS JOIN Trip ORDER BY NEWID()
    SET @n += 1
END

--	Traveller_Wishlist
DECLARE @o INT = 1;
WHILE @o <= 100
BEGIN
		INSERT INTO Traveller_Wishlist (Username, TripID)
		SELECT top 1 t.Username, TripID FROM Traveller t
		JOIN Trip t1 ON t1.TripID = (
			SELECT TOP 1 TripID FROM Trip
			WHERE TripID NOT IN (
				SELECT TripID FROM Trip_Booking
			)
			ORDER BY NEWID()
		)
		ORDER BY NEWID()
	SET @o += 1
END

--	Traveller_Cart
DECLARE @p INT = 1;
WHILE @p <= 100
BEGIN
		INSERT INTO Traveller_Cart (Username, TripID)
		SELECT top 1 t.Username, TripID FROM Traveller t
		JOIN Trip t1 ON t1.TripID = (
			SELECT TOP 1 TripID FROM Trip
			WHERE TripID NOT IN (
				SELECT TripID FROM Trip_Booking
			)
			ORDER BY NEWID()
		)
		ORDER BY NEWID()
	SET @p += 1
END

-- Trip_Accessibility
DECLARE @q INT = 1;
WHILE @q <= 130
BEGIN
		--	Some Trips offer no accessibilities
		INSERT INTO Trip_Accessibility (TripID, AccessibilityID)
		SELECT top 1 TripID, AccessibilityID
		FROM Trip CROSS Join Accessibility_Options ORDER BY NEWID();
    SET @q += 1
END

--	Trip_Categories
DECLARE @r INT = (Select top 1 TripID from Trip);
Declare @s INT = @r + 225
WHILE @r <= @s 
BEGIN
    INSERT INTO Trip_Categories (TripID, CatID)
    VALUES (
			(@r)%75 + 1,
            (ABS(CHECKSUM(NEWID())) % 10) + 1
    );
    SET @r += 1;
END

INSERT INTO Inclusions([Type], Price)
VALUES
('Airport Transfers', 30),
('Daily Breakfast', 70),
('Guided City Tour', 50),
('Hotel Accommodation', 700),
('Adventure Activities', 80),
('Cultural Experiences', 40),
('Entrance Fees to Attractions', 25),
('Travel Insurance', 60),
('24/7 Customer Support', 1),
('Local SIM Card or Wi-Fi Access', 20);

--	Trip_Inclusions
DECLARE @t INT = 1;
WHILE @t <= 300
BEGIN
		--	Some trips offer no inclusions
		INSERT INTO Trip_Inclusions (TripID, IncType, IncPrice)
		SELECT top 1 TripID, i.[Type], i.Price FROM Trip t
		CROSS Join Inclusions i ORDER BY NEWID()
	SET @t += 1
END

--	Trip_Destination
DECLARE @u INT = 1;
WHILE @u <= 300
BEGIN
		INSERT INTO Trip_Destination(TripID, DestID)
		SELECT top 1 TripID, DestID FROM Trip
		CROSS Join Destination ORDER BY NEWID()
	SET @u += 1
END

--	Trip_Hotels
DECLARE @v INT = 1;
WHILE @v <= 200
BEGIN
		INSERT INTO Trip_Hotels(TripID, HUsername)
		SELECT top 1 TripID, HUsername FROM Trip
		CROSS Join Hotel ORDER BY NEWID()
	SET @v += 1
END

--	Trip_Destination
DECLARE @w INT = 1;
WHILE @w <= 200
BEGIN
		INSERT INTO Trip_Destination(TripID, DestID)
		SELECT top 1 TripID, DestID FROM Trip
		CROSS Join Destination ORDER BY NEWID()
	SET @w += 1
END

--	Trip Itinerary
create table Itinerary(
	ID INT IDENTITY (1, 1) PRIMARY KEY,
	Type varchar(50) NOT NULL
)

Insert into Itinerary(Type)
values
('Airport Transfer'), ('Welcome Dinner'), ('City Tour'), ('Museum Visit'),
('Free Time'), ('Guided Hike'), ('Boat Excursion'), ('Cooking Class'),
('Historical Site'), ('Local Market'), ('Wine Tasting'), ('Sunset Cruise'),
('Departure Prep'), ('Group Lunch'), ('Photography Tour'), ('Cultural Show');

--	Trip Itineraries(only runs 75 times not 75+)
DECLARE @x INT = (Select top 1 TripID from Trip);
Declare @y INT = @x + 150
WHILE @x <= @y 
BEGIN
	Declare @SDate DateTime = (Select StartDate from Trip where TripID = @x)
	Set @SDate = (DATEDIFF(day, -(ABS(CHECKSUM(NEWID())) % 4 + 1), @SDate));
	Declare @EDate DateTime = (DATEDIFF(day, -(ABS(CHECKSUM(NEWID())) % 4 + 1), @SDate))

    INSERT INTO Trip_Itinerary(TripID, Event, EventStartDate, EventEndDate)
    VALUES (
			(@x)%75 + 1,
            (Select top 1 Type from Itinerary order by NEWID()),
			(@SDate),
			(@EDate)
    );
    SET @x += 1;
END


--	Trip_Transportation
Create table Transportation(
		ID INT IDENTITY (1, 1) PRIMARY KEY,
		Type varchar(25) NOT NULL
)
Insert Into Transportation
Values
('Flight'), ('Train'), ('Bus'), ('Private Car'), 
('Boat'), ('Subway'), ('Taxi'), ('Shuttle'), 
('Ferry'), ('Walking Tour'), ('Bicycle'), ('Tram');

Declare @z int = (Select top 1 TripID from Trip)
Declare @aa INT = 1
WHILE @aa <= @z + 300 
BEGIN
	set @SDate = (Select StartDate from Trip where TripID = @aa % 75 + 1)
	set @EDate = (Select EndDate from Trip where TripID = @aa % 75 + 1)
	Set @SDate = DATEADD(day, (ABS(CHECKSUM(NEWID())) % 3 + 1), @SDate)
	Set @EDate = DATEADD(day, -(ABS(CHECKSUM(NEWID())) % 3 + 1), @EDate)

	Insert into Trip_Transportation(TripID, TransportationType, EstimatedStart, EstimatedEnd, Start, [End])
	Values (
		@aa % 75 + 1,
		(Select top 1 Type from Transportation order by NEWID()),
		(@SDate), (@EDate),
		(DATEADD(day, (ABS(CHECKSUM(NEWID())) % 2), @SDate)),
		(DATEADD(day, (ABS(CHECKSUM(NEWID())) % 2), @EDate))
	)
    SET @aa += 1;
END


create table TempRev(
	ID INT IDENTITY (1, 1) PRIMARY KEY,
    Stars INT NOT NULL CHECK (Stars BETWEEN 1 AND 5),
    Feedback VARCHAR(256),
    Response VARCHAR(256),
)

INSERT INTO TempRev (Stars, Feedback, Response)
VALUES
-- 5-star reviews (30)
(5, 'Absolutely breathtaking views at every turn', 'We''re delighted you enjoyed the scenery!'),
(5, 'The perfect blend of adventure and relaxation', 'We aim to provide balanced experiences!'),
(5, 'Exceeded all our expectations in every way', 'Your satisfaction is our greatest reward!'),
(5, 'Impeccable service from start to finish', 'Exceptional service is our standard!'),
(5, 'The photos don''t do this place justice', 'Nature''s beauty speaks for itself!'),
(5, 'Every detail was thoughtfully considered', 'We care about the little things!'),
(5, 'Created memories that will last a lifetime', 'Making memories is why we do this!'),
(5, 'Worth every penny and then some', 'We strive to deliver great value!'),
(5, 'Can''t wait to come back next year', 'We''ll be ready for your return!'),
(5, 'The ideal getaway from everyday life', 'Escaping routine is important to us!'),
(5, 'Cultural experiences were authentic and meaningful', 'We cherish local traditions!'),
(5, 'Perfect for families with kids of all ages', 'Family-friendly is our specialty!'),
(5, 'The culinary experiences were outstanding', 'Local flavors are our pride!'),
(5, 'Staff went above and beyond for us', 'Our team loves to impress!'),
(5, 'Clean, comfortable, and conveniently located', 'The basics done right!'),

-- 4-star reviews (40)
(4, 'Wonderful experience with just a few minor hiccups', 'We''re always improving!'),
(4, 'Great value for what we paid', 'Quality matters to us!'),
(4, 'Would definitely recommend to friends', 'Word-of-mouth means everything!'),
(4, 'Beautiful location with excellent amenities', 'We''re proud of our facilities!'),
(4, 'Staff were friendly and attentive', 'Our team appreciates your notice!'),
(4, 'Good variety of activities available', 'We offer something for everyone!'),
(4, 'Comfortable accommodations in prime location', 'Location is everything!'),
(4, 'Delicious food with local flavors', 'We source ingredients locally!'),
(4, 'Clean and well-maintained throughout', 'Hygiene is our priority!'),
(4, 'Easy to access major attractions', 'Convenience is key!'),
(4, 'Reliable transportation options nearby', 'Getting around should be easy!'),
(4, 'Peaceful atmosphere despite being central', 'Oasis in the city!'),
(4, 'Good mix of guided and free time', 'Balance is important!'),
(4, 'Helpful recommendations from staff', 'We know the area well!'),
(4, 'Quality equipment provided for activities', 'We maintain our gear!'),

-- 3-star reviews (20)
(3, 'Nice enough but nothing extraordinary', 'We''ll work to impress you more!'),
(3, 'Met our basic expectations', 'We aim to exceed expectations!'),
(3, 'Good value for budget travelers', 'Affordable quality is our goal!'),
(3, 'Some highlights mixed with average experiences', 'We''ll work on consistency!'),
(3, 'Adequate for a short stay', 'We hope to welcome you longer!'),
(3, 'Room for improvement in some areas', 'We''re always evolving!'),
(3, 'Not bad, but could be better', 'We appreciate constructive feedback!'),
(3, 'Average compared to similar places', 'We strive to stand out!'),
(3, 'Decent option if price is right', 'We review our pricing regularly!'),
(3, 'Satisfactory but not memorable', 'We want to create memories!'),

-- 2-star reviews (7)
(2, 'Several disappointing aspects to our stay', 'We take feedback seriously!'),
(2, 'Expected better for the price point', 'We''re reviewing our value!'),
(2, 'Multiple small issues added up', 'We''ll address each concern!'),
(2, 'Below industry standards in some ways', 'We''re raising our standards!'),
(2, 'Wouldn''t return based on this experience', 'We hope for another chance!'),
(2, 'Photos were misleading about reality', 'We''ll update our marketing!'),
(2, 'Poor communication from staff', 'Additional training underway!'),

-- 1-star reviews (3)
(1, 'Complete waste of time and money', 'We want to make this right!'),
(1, 'Worst travel experience we''ve had', 'We''re urgently investigating!'),
(1, 'Nothing as advertised or promised', 'We''re addressing these issues!')

--	Inserting into Reviews
set @i  = (SELECT TOP 1 ID FROM TempRev)
declare @k int = (Select Count(*) from TempRev)
declare @j int = @i  
DECLARE @StartDate DATE 
Declare @Usr varchar(30)

WHILE @j <= @i + 100
BEGIN
    SELECT TOP 1 @StartDate = t.StartDate, @Usr=Username FROM Trip_Booking tb
    JOIN Trip t ON t.TripID = tb.TripID
    ORDER BY NEWID()

    INSERT INTO Review (Stars, Feedback, Response, ReviewTime, ResponseTime, Reviewer)
    SELECT 
        tr.Stars, 
        tr.Feedback, 
        tr.Response,
        DATEADD(day, -(ABS(CHECKSUM(NEWID())) % 10 + 5), @StartDate),
        DATEADD(day, (ABS(CHECKSUM(NEWID())) % 5 + 1), @StartDate),
        (@Usr)
    FROM 
        TempRev tr
    WHERE 
        tr.ID = @j % @k;
    
    SET @j = @j + 1
END


--	TripReview
set @i = 1;
WHILE @i <= 150
BEGIN
		INSERT INTO TripReview(ReviewID, TripID)
		SELECT top 1 ReviewID, TripID FROM Review
		CROSS Join Trip order by NEWID()
	SET @i += 1
END


truncate table TempRev
INSERT INTO TempRev (Stars, Feedback, Response)
VALUES
-- Hotel Reviews
(5, 'Absolutely stunning hotel with impeccable service!', 'We love pampering our guests!'),
(4, 'Beautiful room but minibar prices too high', 'We''ll review our pricing policy'),
(5, 'Best hotel bed I''ve ever slept in!', 'Our beds are legendary!'),
(4, 'Gorgeous pool area but crowded at peak times', 'We''ve added more pool chairs'),
(5, 'The concierge went above and beyond for us!', 'Our team aims to impress!'),
(4, 'Charming decor and excellent location', 'Location is our pride!'),
(3, 'Cute but walls were paper thin', 'Soundproofing upgrades coming soon'),
(5, 'Personalized service made us feel special', 'Every guest is VIP here!'),
(2, 'Expected more for a 4-star rating', 'We''ll work to meet expectations'),
(4, 'Great rooftop bar with amazing views', 'Sunset cocktails are the best!'),
(3, 'Basic but clean and affordable', 'We focus on value!'),
(2, 'Noisy AC unit kept us awake', 'Maintenance has fixed this now'),
(3, 'Good for one night but not more', 'We appreciate your feedback'),
(1, 'Dirty bathroom and rude staff', 'Deep cleaning scheduled weekly now'),
(3, 'Expected less for what we paid', 'We review prices regularly'),
(5, 'Paradise! Will return every year!', 'Your return is our honor!'),
(4, 'Amazing kids club kept children happy', 'Happy kids, happy parents!'),
(5, 'Private beach was worth every penny', 'Our beach is magical!'),
(3, 'Too many hidden resort fees', 'We''ve simplified our pricing'),
(2, 'Overcrowded pools and long wait for dinner', 'We''ve expanded dining options'),
(4, 'Perfect workstation in room', 'Business travelers are our specialty!'),
(5, 'Flawless WiFi and great meeting rooms', 'Reliable tech is our promise'),
(3, 'Comfortable but breakfast ended too early', 'Breakfast hours extended now'),
(4, 'Excellent airport shuttle service', 'Punctuality is our trademark!'),
(1, 'Conference facilities were subpar', 'We''ve upgraded our AV equipment'),
(5, 'Spa treatments were heavenly!', 'Our spa team thanks you!'),
(4, 'Historic charm with modern comforts', 'We cherish our heritage!'),
(2, 'Room service forgot our order twice', 'Service training intensified now'),
(5, 'Pet-friendly policies made our trip!', 'We love furry guests too!'),
(3, 'Good hotel but terrible neighborhood', 'We''ve improved local guidance'),
(4, 'Magical Christmas decorations everywhere!', 'Holiday spirit is our joy!'),
(5, 'New Year''s Eve party was epic!', 'Wait till next year''s party!'),
(3, 'Valentine''s package was overpriced', 'We''ve adjusted our packages'),
(4, 'Great spring break destination hotel', 'Youthful energy welcome here!'),
(1, 'Summer stay was unbearably hot', 'AC system fully upgraded now'),
(5, 'Best hotel in the city!', 'NULL'),
(4, 'Great value boutique hotel', 'NULL'),
(2, 'Construction noise ruined our stay', 'NULL'),
(3, 'Average experience, nothing special', 'NULL'),
(5, 'Will recommend to all my friends!', 'NULL'),
(1, 'Worst customer service I''ve experienced', 'We''re retraining all staff'),
(5, 'Turn-down service was a lovely touch', 'Details make the difference!'),
(4, 'Great gym facilities for a hotel', 'Fitness is important to us'),
(2, 'Elevators were constantly out of order', 'New elevators being installed'),
(3, 'Average hotel with average amenities', 'We''re planning renovations soon'),
(5, 'Loved the complimentary champagne arrival', 'We enjoy welcoming guests!'),
(4, 'Perfect location for downtown exploring', 'Location is everything!'),
(1, 'Found bed bugs in our room', 'We''ve exterminated thoroughly'),
(5, 'Butler service exceeded all expectations', 'Butlers love to impress!'),
(2, 'Fire alarm went off 3 times', 'System has been repaired'),
(5, 'The presidential suite exceeded all expectations', 'We aim to impress our VIP guests'),
(4, 'Champagne welcome was a lovely touch', 'First impressions matter to us'),
(5, 'Private butler anticipated our every need', 'Discreet service is our specialty'),
(4, 'Designer toiletries were a luxurious detail', 'We partner with premium brands'),
(5, 'Turndown service with handmade chocolates', 'Sweet dreams are guaranteed'),
(4, 'Local artist gallery in the lobby', 'We showcase neighborhood talent'),
(3, 'Retro decor was stylish but impractical', 'Form meets function in our redesign'),
(5, 'Handwritten weather forecast with morning coffee', 'Personal touches make the difference'),
(2, 'Noise from nearby club was disruptive', 'Soundproofing upgrades complete'),
(4, 'Rooftop garden was a peaceful oasis', 'Urban escapes are our specialty'),
(3, 'Functional room with comfortable bed', 'We focus on essential comforts'),
(2, 'Thin walls and noisy neighbors', 'Guest conduct policies reinforced'),
(3, 'Basic but clean for the price', 'Hygiene is never compromised'),
(1, 'Broken AC made sleep impossible', 'All units now serviced'),
(3, 'No frills but great location', 'Location can''t be beat'),
(5, 'Infinity pool with stunning sunset views', 'Nature provides the best decor'),
(4, 'Kids'' club staff were phenomenal', 'Family memories start here'),
(5, 'Beachfront cabanas worth every penny', 'Paradise is our address'),
(3, 'Buffet quality varied significantly', 'Chef supervision increased'),
(2, 'Overbooked during peak season', 'Reservation system upgraded')
--70

create table TempRoom(
	ID INT IDENTITY (1, 1) PRIMARY KEY,
    HUsername VARCHAR(50) NOT NULL FOREIGN KEY REFERENCES Hotel(Husername),
    RoomNumber INT NOT NULL
)
Insert into TempRoom
select HUsername, RoomNumber from Room

create table TempTB(
	ID INT IDENTITY (1, 1) PRIMARY KEY,
    Username VARCHAR(30) NOT NULL FOREIGN KEY REFERENCES Traveller(Username),
    TripID INT NOT NULL FOREIGN KEY REFERENCES Trip(TripID)
)
Insert into TempTB
select Username, TripID from Trip_Booking

--	Room_Booking
set @i = (Select top 1 ID from TempRoom)
WHILE @i <= 150
BEGIN
		Set @SDate = (Select StartDate from Trip t Join TempTB tb on t.TripID=tb.TripID where ID=@i % (Select Count(*) from Trip_Booking))
		Set @EDate = (Select EndDate from Trip t Join TempTB tb on t.TripID=tb.TripID where ID=@i % (Select Count(*) from Trip_Booking))

		Insert into Room_Booking(HUsername, RoomID, Username, StartDate, BookingDate, EndDate)
		Values(
			(Select HUsername from TempRoom where ID=@i % (Select Count(*) from Room)),
			(Select RoomNumber from TempRoom where ID=@i % (Select Count(*) from Room)),
			(Select Username from TempTB where ID=@i % (Select Count(*) from Trip_Booking)),
			@SDate,
			(DATEADD(day, -(ABS(CHECKSUM(NEWID())) % 10 + 5), @SDate)),
			@EDate
		)
	SET @i += 1
END

drop table TempTB
drop table TempRoom

--	
set @i = (SELECT TOP 1 ID FROM TempRev)
set @j = @i
declare @tt varchar(30)

WHILE @j <= @i + 70
BEGIN
	set @tt = (Select top 1 Username from Room_Booking order by NEWID())
	set @StartDate = (Select top 1 EndDate from Room_Booking where Username=@tt)

    INSERT INTO Review (Stars, Feedback, Response, ReviewTime, ResponseTime, Reviewer)
    SELECT 
        tr.Stars, 
        tr.Feedback, 
        tr.Response,
        DATEADD(day, (ABS(CHECKSUM(NEWID())) % 5 + 1), @StartDate),
        DATEADD(day, (ABS(CHECKSUM(NEWID())) % 10 + 5), @StartDate),
        (@tt)
    FROM 
        TempRev tr
    WHERE 
        tr.ID = @j;
    
    SET @j = @j + 1
END

set @i = 1;
WHILE @i <= 70
BEGIN
		Insert Into HotelReview(ReviewID, HUsername)
		Values (
			(@i % 70 + 99 + (Select top 1 ReviewID from Review)),
			(Select top 1 HUsername from Hotel order by NEWID())
		)

	SET @i += 1
END



truncate table TempRev
INSERT INTO TempRev (Stars, Feedback, Response)
VALUES
(5, 'This destination took my breath away - the natural beauty is unparalleled', 'We''re so glad you appreciated our beautiful location!'),
(5, 'The local culture here is vibrant and authentic', 'We take pride in sharing our cultural heritage!'),
(5, 'Every corner of this city reveals new architectural wonders', 'Our historic buildings tell amazing stories!'),
(5, 'The landscapes here look like they''re from a postcard', 'Nature truly blessed our region!'),
(5, 'The food scene in this destination is world-class', 'Our local cuisine is our pride and joy!'),
(5, 'The beaches here are the most pristine I''ve ever seen', 'We work hard to maintain our natural beauty!'),
(5, 'This mountain destination offers views that heal the soul', 'The mountains inspire us daily!'),
(5, 'The local markets here are bursting with color and life', 'Our artisans love sharing their crafts!'),
(5, 'The historical sites here are incredibly well-preserved', 'We cherish our cultural treasures!'),
(5, 'The sunsets in this location are magical', 'Nature''s daily show never disappoints!'),
(5, 'The parks and green spaces here are breathtaking', 'We prioritize preserving natural spaces!'),
(5, 'This coastal town has the most charming waterfront', 'The sea is our greatest treasure!'),
(5, 'The local festivals here are unforgettable experiences', 'We love celebrating our traditions!'),
(5, 'The hiking trails here lead to spectacular vistas', 'Our trails showcase nature''s beauty!'),
(5, 'The architecture here tells centuries of fascinating history', 'Every building has a story to tell!'),
(5, 'The vineyards in this region produce exceptional wines', 'Our terroir is world-renowned!'),
(5, 'The wildlife encounters here are once-in-a-lifetime', 'We protect our natural inhabitants!'),
(5, 'The museums here are world-class cultural institutions', 'We preserve our artistic legacy!'),
(5, 'The city skyline here is absolutely iconic', 'Our urban landscape inspires awe!'),
(5, 'The geothermal features here are nature''s wonder', 'Geological marvels surround us!'),
(5, 'The fall foliage in this region is spectacular', 'Seasonal beauty is our hallmark!'),
(5, 'The desert landscapes here are mesmerizing', 'Our arid beauty is unique!'),
(5, 'The local music scene here is incredibly vibrant', 'Our cultural heartbeat is strong!'),
(5, 'The winter scenery here is like a fairy tale', 'Snow transforms our landscape magically!'),
(5, 'The gardens here are horticultural masterpieces', 'We cultivate beauty with care!'),

-- 4-star destination reviews (30)
(4, 'This destination has wonderful charm with a few crowded areas', 'We''re working to manage visitor flow!'),
(4, 'The cultural sites here are impressive but some need restoration', 'Preservation efforts are ongoing!'),
(4, 'Beautiful beaches though some areas need cleaner maintenance', 'Beach cleanup crews are active daily!'),
(4, 'The downtown area is vibrant but can get noisy at night', 'We''re implementing noise ordinances!'),
(4, 'Excellent museums though some exhibits need updating', 'Rotating exhibits coming soon!'),
(4, 'The natural beauty is stunning but some trails need better marking', 'Trail improvements underway!'),
(4, 'Great food scene though some restaurants are overpriced', 'We''ll share more budget options!'),
(4, 'The architecture is amazing but some areas are too touristy', 'We promote authentic experiences!'),
(4, 'Lovely parks though some facilities need maintenance', 'Park upgrades scheduled!'),
(4, 'The local markets are colorful but some vendors are pushy', 'Vendor training programs in place!'),
(4, 'Wonderful historic district though parking is difficult', 'New parking solutions coming!'),
(4, 'The coastal views are spectacular but beach access is limited', 'More access points planned!'),
(4, 'Great hiking though some paths are too strenuous for beginners', 'Better signage coming!'),
(4, 'The vineyards are beautiful but tours book up quickly', 'Expanding tour availability!'),
(4, 'Fascinating cultural sites but some need better explanations', 'New interpretive signs coming!'),
(4, 'The wildlife is amazing but sightings aren''t guaranteed', 'We protect natural habitats!'),
(4, 'Excellent public art but some pieces need restoration', 'Art conservation program active!'),
(4, 'The skyline views are great but some observation decks are crowded', 'Timed tickets being implemented!'),
(4, 'Unique geological features but some areas need better safety rails', 'Safety improvements underway!'),
(4, 'Beautiful seasonal colors but peak times are very busy', 'We suggest shoulder season visits!'),
(4, 'Stunning desert landscapes but some trails lack shade', 'Shade structures coming!'),
(4, 'Vibrant music scene but some venues are hard to find', 'Better cultural maps coming!'),
(4, 'Magical winter scenery but some roads need better plowing', 'Winter maintenance increased!'),
(4, 'Lovely gardens though some sections were under renovation', 'Renovations complete next season!'),
(4, 'Great local festivals but some events sell out quickly', 'More tickets being released!'),
(4, 'Charming waterfront but some piers need repairs', 'Waterfront revitalization coming!'),
(4, 'Excellent wine region but some tastings are expensive', 'More affordable options coming!'),
(4, 'Fascinating history but some sites have limited hours', 'Expanding operating hours!'),
(4, 'Beautiful national park but some facilities are dated', 'Park upgrades in progress!'),
(4, 'Unique local crafts but some shops are overpriced', 'We''ll share authentic artisan markets!'),

-- 3-star destination reviews (10)
(3, 'This destination is nice but not particularly unique', 'We''re working on more distinctive experiences!'),
(3, 'The attractions here are decent but nothing extraordinary', 'New experiences coming soon!'),
(3, 'Average beaches compared to other destinations', 'Beach enhancement program starting!'),
(3, 'The cultural offerings are adequate but limited', 'Cultural expansion planned!'),
(3, 'Some nice views but often obscured by development', 'View corridor protections coming!'),
(3, 'The food is okay but lacks standout options', 'Culinary innovation underway!'),
(3, 'Historical sites are interesting but poorly maintained', 'Restoration projects approved!'),
(3, 'Natural areas are pleasant but too developed', 'Conservation efforts expanding!'),
(3, 'The shopping is decent but very commercialized', 'More authentic options coming!'),
(3, 'Good for a short visit but not a destination', 'We''re enhancing our offerings!'),

-- 2-star destination reviews (4)
(2, 'This destination is overrated and overcrowded', 'We''re addressing visitor management!'),
(2, 'Natural beauty marred by poor maintenance', 'Cleanup initiatives launching!'),
(2, 'Cultural sites are run-down and disappointing', 'Major renovations planned!'),
(2, 'Local attractions don''t live up to the hype', 'We''re raising our standards!'),

-- 1-star destination reviews (1)
(1, 'Nothing special about this destination at all', 'We''re completely reimagining our offerings!')

set @i = (SELECT TOP 1 ID FROM TempRev)
set @j = @i  
Declare @EndDate Date
Declare @TID INT

WHILE @j <= @i + 100
BEGIN
    SELECT TOP 1 @StartDate = t.StartDate, @Usr=Username, @TID=t.TripID FROM Trip_Booking tb
    JOIN Trip t ON t.TripID = tb.TripID
    ORDER BY NEWID()

	set @StartDate = (DATEADD(day, -(ABS(CHECKSUM(NEWID())) % 10 + 5), @StartDate));
	set @EndDate = DATEADD(day, -(ABS(CHECKSUM(NEWID())) % 5 + 1), (Select EndDate from Trip where TripID=@TID))

    INSERT INTO Review (Stars, Feedback, Response, ReviewTime, ResponseTime, Reviewer)
    SELECT 
        tr.Stars, 
        tr.Feedback, 
        tr.Response,
        @StartDate,
        @EndDate,
        (@Usr)
    FROM 
        TempRev tr
    WHERE 
        tr.ID = @j;
    
    SET @j = @j + 1
END

--	DestInation Reviews
set @i =0;
WHILE @i <= 70
BEGIN
		INSERT INTO DestinationReview(ReviewID, DestinationID)
		Values (
			(@i % 70 + 99 + 70 + (Select top 1 ReviewID from Review)),
			(Select top 1 DestID from Destination order by NEWID())
		)
		SET @i += 1
END