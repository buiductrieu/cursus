	
			CREATE TRIGGER trg_Transaction_Log
			ON [Transactions]
			AFTER INSERT, UPDATE, DELETE
			AS
			BEGIN
				-- INSERT Operation Logging
				IF EXISTS (SELECT 1 FROM inserted) AND NOT EXISTS (SELECT 1 FROM deleted)
				BEGIN
					INSERT INTO TransactionHistories (UserId, TrackedTransactionId, Description, DateCreated)
					SELECT 
						UserId, 
						TransactionId, 
						'Inserted a new transaction with ID: ' + CAST(TransactionId AS NVARCHAR(50)), 
						GETDATE()
					FROM inserted;
				END

				-- DELETE Operation Logging
				IF EXISTS (SELECT 1 FROM deleted) AND NOT EXISTS (SELECT 1 FROM inserted)
				BEGIN
					INSERT INTO TransactionHistories (UserId, TrackedTransactionId, Description, DateCreated)
					SELECT 
						UserId, 
						TransactionId, 
						'Deleted the transaction with ID: ' + CAST(TransactionId AS NVARCHAR(50)), 
						GETDATE()
					FROM deleted;
				END
				-- UPDATE Operation Logging
				IF 
				EXISTS (SELECT 1 FROM inserted) AND 
				EXISTS (SELECT 1 FROM deleted) AND (
				((SELECT Amount FROM inserted) != (SELECT Amount FROM deleted)) OR 
				((SELECT UserId FROM inserted) != (SELECT UserId FROM deleted)))
				BEGIN
					IF ((SELECT Amount FROM inserted) != (SELECT Amount FROM deleted))
					BEGIN 
					INSERT INTO TransactionHistories (UserId, TrackedTransactionId, Description, DateCreated)
					SELECT 
						i.UserId, 
						i.TransactionId, 
						'Updated transaction with ID: ' + CAST(i.TransactionId AS NVARCHAR(50)) + 
						' | Previous Amount: ' + CAST(d.Amount AS NVARCHAR(50)) + 
						' | New Amount: ' + CAST(i.Amount AS NVARCHAR(50)), 
						GETDATE()
					FROM inserted i
					JOIN deleted d ON i.TransactionId = d.TransactionId;
					END
				END
			END;


		-- Parent Categories
		INSERT INTO Categories ([Name], [Description], [DateCreated], IsParent, ParentCategory, [Status]) 
		VALUES 
		('Frontend Development', 'Courses covering HTML, CSS, and JavaScript', GETDATE(), 1, NULL, 1),
		('Backend Development', 'Courses on server-side technologies and databases', GETDATE(), 1, NULL, 1),
		('Data Science', 'Courses about data manipulation, analysis, and visualization', GETDATE(), 1, NULL, 1),
		('Mobile Development', 'Courses focused on Android and iOS app development', GETDATE(), 1, NULL, 1);

		-- Sub-Categories for Frontend Development
		INSERT INTO Categories ([Name], [Description], [DateCreated], IsParent, ParentCategory, [Status]) 
		VALUES 
		('HTML & CSS', 'Learn the building blocks of web development', GETDATE(), 0, 1, 1),
		('JavaScript Basics', 'Introduction to JavaScript programming', GETDATE(), 0, 1, 1),
		('React.js', 'Build modern UI with React framework', GETDATE(), 0, 1, 1);

		-- Sub-Categories for Backend Development
		INSERT INTO Categories ([Name], [Description], [DateCreated], IsParent, ParentCategory, [Status]) 
		VALUES 
		('Node.js', 'Server-side JavaScript runtime environment', GETDATE(), 0, 2, 1),
		('Databases', 'Learn SQL and NoSQL databases', GETDATE(), 0, 2, 1),
		('API Development', 'Design RESTful and GraphQL APIs', GETDATE(), 0, 2, 1);

		-- Sub-Categories for Data Science
		INSERT INTO Categories ([Name], [Description], [DateCreated], IsParent, ParentCategory, [Status]) 
		VALUES 
		('Python for Data Science', 'Data science using Python libraries', GETDATE(), 0, 3, 1),
		('Machine Learning', 'Introduction to machine learning algorithms', GETDATE(), 0, 3, 1),
		('Data Visualization', 'Create visual insights from data', GETDATE(), 0, 3, 1);

		-- Sub-Categories for Mobile Development
		INSERT INTO Categories ([Name], [Description], [DateCreated], IsParent, ParentCategory, [Status]) 
		VALUES 
		('Android Development', 'Build Android apps using Kotlin', GETDATE(), 0, 4, 1),
		('iOS Development', 'Learn to create apps for iOS using Swift', GETDATE(), 0, 4, 1),
		('Flutter', 'Cross-platform mobile development with Flutter', GETDATE(), 0, 4, 1);



		-- Courses for Frontend Development
		INSERT INTO [Cursus].[dbo].[Courses] 
		([Name], [Description], [CategoryId], [DateCreated], [DateModified], [Status], [Price], [Discount], [StartedDate], [Rating], [InstructorInfoId], [IsApprove]) 
		VALUES 
		('HTML & CSS Mastery', 'Master HTML & CSS from scratch to advanced techniques', 1, GETDATE(), GETDATE(), 1, 49.99, 10.00, '2024-11-01', 0, 1, 1),
		('JavaScript for Beginners', 'Introduction to core JavaScript concepts', 1, GETDATE(), GETDATE(), 1, 59.99, 15.00, '2024-10-30', 0, 1, 1),
		('React.js in Practice', 'Learn React.js by building real-world projects', 1, GETDATE(), GETDATE(), 1, 79.99, 20.00, '2024-12-01', 0, 1, 1);

		-- Courses for Backend Development
		INSERT INTO [Cursus].[dbo].[Courses] 
		([Name], [Description], [CategoryId], [DateCreated], [DateModified], [Status], [Price], [Discount], [StartedDate], [Rating], [InstructorInfoId], [IsApprove]) 
		VALUES 
		('Node.js Crash Course', 'Master server-side JavaScript with Node.js', 2, GETDATE(), GETDATE(), 1, 69.99, 10.00, '2024-11-05', 0, 1, 1),
		('Advanced SQL Queries', 'Write efficient SQL for complex databases', 2, GETDATE(), GETDATE(), 1, 39.99, 5.00, '2024-10-25', 0, 1, 1),
		('Building REST APIs', 'Design scalable APIs using Node.js', 2, GETDATE(), GETDATE(), 1, 79.99, 25.00, '2024-12-10', 0, 1, 1);

		-- Courses for Data Science
		INSERT INTO [Cursus].[dbo].[Courses] 
		([Name], [Description], [CategoryId], [DateCreated], [DateModified], [Status], [Price], [Discount], [StartedDate], [Rating], [InstructorInfoId], [IsApprove]) 
		VALUES 
		('Python for Data Science', 'Analyze data using Python libraries', 3, GETDATE(), GETDATE(), 1, 99.99, 30.00, '2024-11-20', 0, 1, 1),
		('Intro to Machine Learning', 'Learn the basics of machine learning algorithms', 3, GETDATE(), GETDATE(), 1, 89.99, 20.00, '2024-12-15', 0, 1, 1),
		('Data Visualization in Python', 'Create insightful visualizations from data', 3, GETDATE(), GETDATE(), 1, 69.99, 15.00, '2024-10-28', 0, 1, 1);

		-- Courses for Mobile Development
		INSERT INTO [Cursus].[dbo].[Courses] 
		([Name], [Description], [CategoryId], [DateCreated], [DateModified], [Status], [Price], [Discount], [StartedDate], [Rating], [InstructorInfoId], [IsApprove]) 
		VALUES 
		('Android Development with Kotlin', 'Build Android apps from scratch', 4, GETDATE(), GETDATE(), 1, 89.99, 25.00, '2024-11-10', 0, 1, 1),
		('iOS Development with Swift', 'Create iOS apps using Swift', 4, GETDATE(), GETDATE(), 1, 99.99, 30.00, '2024-12-05', 0, 1, 1),
		('Flutter for Cross-Platform Apps', 'Learn Flutter for mobile app development', 4, GETDATE(), GETDATE(), 1, 79.99, 20.00, '2024-11-15', 0, 1, 1);


		-- Steps for Course 1: HTML & CSS Mastery
		INSERT INTO [Cursus].[dbo].[Steps] 
		([CourseId], [Name], [Order], [Description], [DateCreated]) 
		VALUES 
		(1, 'Introduction to HTML & CSS', 1, 'Overview of web structure and styling basics', GETDATE()),
		(1, 'Building a Web Page', 2, 'Create a static webpage using HTML and CSS', GETDATE()),
		(1, 'Responsive Design Techniques', 3, 'Make your web page mobile-friendly', GETDATE());

		-- Steps for Course 2: JavaScript for Beginners
		INSERT INTO [Cursus].[dbo].[Steps] 
		([CourseId], [Name], [Order], [Description], [DateCreated]) 
		VALUES 
		(2, 'Introduction to JavaScript', 1, 'Learn JavaScript syntax and concepts', GETDATE()),
		(2, 'DOM Manipulation', 2, 'Use JavaScript to interact with HTML elements', GETDATE()),
		(2, 'Basic Events Handling', 3, 'Respond to user actions on a webpage', GETDATE());

		-- Steps for Course 3: React.js in Practice
		INSERT INTO [Cursus].[dbo].[Steps] 
		([CourseId], [Name], [Order], [Description], [DateCreated]) 
		VALUES 
		(3, 'Setting up React.js', 1, 'Initialize a React project', GETDATE()),
		(3, 'Building Components', 2, 'Create and reuse UI components', GETDATE()),
		(3, 'Managing State', 3, 'Handle state with useState and useEffect', GETDATE());

		-- Steps for Course 4: Node.js Crash Course
		INSERT INTO [Cursus].[dbo].[Steps] 
		([CourseId], [Name], [Order], [Description], [DateCreated]) 
		VALUES 
		(4, 'Introduction to Node.js', 1, 'Setup Node.js and explore its features', GETDATE()),
		(4, 'Creating a Simple Server', 2, 'Build a basic web server with Node.js', GETDATE()),
		(4, 'Handling Requests and Responses', 3, 'Manage HTTP requests and responses', GETDATE());

		-- Steps for Course 5: Advanced SQL Queries
		INSERT INTO [Cursus].[dbo].[Steps] 
		([CourseId], [Name], [Order], [Description], [DateCreated]) 
		VALUES 
		(5, 'SQL Basics', 1, 'Review basic SQL commands', GETDATE()),
		(5, 'Complex Joins', 2, 'Work with multi-table joins', GETDATE()),
		(5, 'Optimizing Queries', 3, 'Improve query performance with indexes', GETDATE());

		-- Steps for Course 6: Building REST APIs
		INSERT INTO [Cursus].[dbo].[Steps] 
		([CourseId], [Name], [Order], [Description], [DateCreated]) 
		VALUES 
		(6, 'Introduction to REST', 1, 'Learn the basics of RESTful APIs', GETDATE()),
		(6, 'Creating API Endpoints', 2, 'Design API routes and methods', GETDATE()),
		(6, 'Testing APIs', 3, 'Use tools to test and debug APIs', GETDATE());

		-- Steps for Course 7: Python for Data Science
		INSERT INTO [Cursus].[dbo].[Steps] 
		([CourseId], [Name], [Order], [Description], [DateCreated]) 
		VALUES 
		(7, 'Setting up Python', 1, 'Install Python and libraries', GETDATE()),
		(7, 'Data Manipulation', 2, 'Work with pandas and NumPy', GETDATE()),
		(7, 'Data Visualization', 3, 'Create plots with Matplotlib', GETDATE());

		-- Steps for Course 8: Intro to Machine Learning
		INSERT INTO [Cursus].[dbo].[Steps] 
		([CourseId], [Name], [Order], [Description], [DateCreated]) 
		VALUES 
		(8, 'Introduction to ML', 1, 'Explore machine learning basics', GETDATE()),
		(8, 'Building a Model', 2, 'Train a simple ML model', GETDATE()),
		(8, 'Evaluating the Model', 3, 'Assess model performance', GETDATE());

		-- Steps for Course 9: Data Visualization in Python
		INSERT INTO [Cursus].[dbo].[Steps] 
		([CourseId], [Name], [Order], [Description], [DateCreated]) 
		VALUES 
		(9, 'Data Plotting Basics', 1, 'Learn to create basic plots', GETDATE()),
		(9, 'Advanced Plot Customization', 2, 'Customize plots with themes', GETDATE()),
		(9, 'Interactive Visualizations', 3, 'Build interactive plots', GETDATE());

		-- Steps for Course 10: Android Development with Kotlin
		INSERT INTO [Cursus].[dbo].[Steps] 
		([CourseId], [Name], [Order], [Description], [DateCreated]) 
		VALUES 
		(10, 'Setting up Android Studio', 1, 'Install Android Studio and Kotlin', GETDATE()),
		(10, 'Creating Your First App', 2, 'Develop a simple Android app', GETDATE()),
		(10, 'Handling User Input', 3, 'Manage user interactions', GETDATE());

		-- Steps for Course 11: iOS Development with Swift
		INSERT INTO [Cursus].[dbo].[Steps] 
		([CourseId], [Name], [Order], [Description], [DateCreated]) 
		VALUES 
		(11, 'Introduction to Swift', 1, 'Learn the basics of Swift programming', GETDATE()),
		(11, 'Building a Simple App', 2, 'Create a basic iOS application', GETDATE()),
		(11, 'Implementing Navigation', 3, 'Add navigation between views', GETDATE());

		-- Steps for Course 12: Flutter for Cross-Platform Apps
		INSERT INTO [Cursus].[dbo].[Steps] 
		([CourseId], [Name], [Order], [Description], [DateCreated]) 
		VALUES 
		(12, 'Setting up Flutter', 1, 'Install Flutter SDK', GETDATE()),
		(12, 'Creating a Flutter App', 2, 'Build a cross-platform app', GETDATE()),
		(12, 'Managing State in Flutter', 3, 'Handle state with Provider', GETDATE());

		INSERT INTO [Cursus].[dbo].[Vouchers] 
       ([VoucherCode], [IsValid], [Name], [CreateDate], [ExpireDate], [Percentage])
		VALUES 
       ('SUMMER2024', 1, 'Giảm giá mùa hè 2024', GETDATE(), DATEADD(DAY, 30, GETDATE()), 10),
       ('BACKTOSCHOOL', 1, 'Ưu đãi tựu trường', GETDATE(), DATEADD(DAY, 60, GETDATE()), 15),
       ('NEWYEAR2025', 1, 'Khuyến mãi năm mới 2025', GETDATE(), DATEADD(DAY, 90, GETDATE()), 20),
       ('BLACKFRIDAY', 1, 'Giảm giá Black Friday', GETDATE(), DATEADD(DAY, 15, GETDATE()), 50),
       ('FREESHIP', 1, 'Miễn phí vận chuyển', GETDATE(), DATEADD(DAY, 45, GETDATE()), 5);



		
