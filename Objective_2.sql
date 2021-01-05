use AdventureWorks2017

-- Task 1
SELECT TOP (8) * 
FROM HumanResources.Department 
ORDER BY Name DESC;
GO

-- Task 2
SELECT NationalIDNumber, BusinessEntityID, JobTitle, BirthDate, HireDate 
FROM HumanResources.Employee 
WHERE (YEAR(BirthDate) + 22 =  YEAR(HireDate));
GO

--Task 3
SELECT BusinessEntityID, NationalIDNumber, JobTitle, BirthDate, MaritalStatus
FROM HumanResources.Employee
WHERE (MaritalStatus = 'M') AND (JobTitle = 'Design Engineer' OR JobTitle = 'Tool Designer' OR JobTitle = 'Engineering Manager' OR JobTitle = 'Production Control Manager')
ORDER BY BirthDate;
GO

--Task 4
SELECT BusinessEntityID, JobTitle, Gender, BirthDate, HireDate 
FROM HumanResources.Employee
WHERE MONTH(HireDate) = 3 AND DAY(HireDate) = 5 
ORDER BY BusinessEntityID 
OFFSET 1 ROWS 
FETCH NEXT 5 ROWS ONLY;
GO

--Task 5
SELECT BusinessEntityID, JobTitle, Gender, HireDate, REPLACE (LoginID, 'adventure-works', 'adventure-works2017') LoginID 
FROM HumanResources.Employee
WHERE Gender = 'f' AND DATEPART(dw,HireDate) = 4;
GO

--Task 6
SELECT SUM(VacationHours) AS VacationSuminHours, SUM (SickLeaveHours) AS SiknessSumHours
FROM HumanResources.Employee;
GO

--Task 7
SELECT DISTINCT TOP(8) JobTitle, 
REVERSE((SELECT TOP (1) value FROM STRING_SPLIT(REVERSE(JobTitle), ' '))) AS LastWord 
FROM HumanResources.Employee 
ORDER BY JobTitle DESC;
GO

--Task 8
SELECT BusinessEntityID, JobTitle, Gender, BirthDate, HireDate
FROM HumanResources.Employee
WHERE JobTitle LIKE '%Control%'
GO



