-- 01. Find Names of All Employees by First Name
USE SoftUni;
SELECT [FirstName],[LastName]
FROM [Employees]
WHERE [FirstName] LIKE 'Sa%'

-- 02.Find Names of All employees by Last Name 
SELECT [FirstName],[LastName]
FROM [Employees]
WHERE [LastName] LIKE '%ei%'

-- 03. Find First Names of All Employess
SELECT [FirstName]
FROM [Employees]
WHERE [DepartmentID]  = 3 
OR [DepartmentID]  = 10
AND [HireDate]
BETWEEN '1995'
AND '2005';

-- 04. Find All Employees Except Engineers
SELECT [FirstName],[LastName]
FROM [Employees]
WHERE [JobTitle] NOT LIKE '%engineer%' ;

-- 05. Find Towns with Name Length
SELECT [Name] 
FROM [Towns]
WHERE LEN([Name]) = 5 OR LEN([Name]) = 6
ORDER BY [Name] ASC;

-- 06. Find Towns Starting With
SELECT [TownID],[Name] 
FROM [Towns]
WHERE [Name] LIKE '[M,K,B,E]%' 
ORDER BY [Name] ASC;

-- 07. Find Towns Not Starting With
SELECT [TownID],[Name] 
FROM [Towns]
WHERE [Name] LIKE '[^R,D,B]%' 
ORDER BY [Name] ASC;

-- 08. Create View Employees Hired After
CREATE 
VIEW V_EmployeesHiredAfter2000
AS
SELECT [FirstName],[LastName]
FROM [Employees]
WHERE [HireDate] >= '2001';

-- 09. Length of Last Name
SELECT [FirstName],[LastName]
FROM [Employees]
WHERE LEN([LastName]) = 5;

-- 10. Rank Employees by Salary
SELECT [EmployeeID],[FirstName],[LastName], [Salary],
DENSE_RANK() OVER (PARTITION BY [Salary] ORDER BY [EmployeeID]) AS [Rank]
FROM [Employees]
WHERE [Salary] BETWEEN 10000 AND 50000
ORDER BY [Salary] DESC;

-- 11. Find All Employees with Rank 2 

SELECT *
FROM (SELECT [EmployeeID],[FirstName],[LastName],[Salary],
       DENSE_RANK() over (partition by Salary ORDER BY EmployeeID) AS Rank
       FROM Employees
       WHERE Salary BETWEEN 10000 AND 50000) AS [Rank2]
WHERE Rank = 2
ORDER BY Salary DESC;
