USE AdventureWorks2017
GO

--Task 1
SELECT HumanResources.Employee.BusinessEntityID, 
       HumanResources.Employee.JobTitle, 
	   HumanResources.EmployeePayHistory.Rate AS MaxRate
FROM   HumanResources.Employee 
       JOIN 
	   HumanResources.EmployeePayHistory 
ON     HumanResources.Employee.BusinessEntityID = HumanResources.EmployeePayHistory.BusinessEntityID
WHERE  HumanResources.EmployeePayHistory.Rate = (SELECT MAX (HumanResources.EmployeePayHistory.Rate) 
                                                 FROM  HumanResources.EmployeePayHistory 
											     WHERE HumanResources.Employee.BusinessEntityID = HumanResources.EmployeePayHistory.BusinessEntityID)
GO

--Task 2
SELECT HumanResources.Employee.BusinessEntityID, 
       HumanResources.Employee.JobTitle, 
	   HumanResources.EmployeePayHistory.Rate,
	   DENSE_RANK() OVER(ORDER BY EmployeePayHistory.Rate) AS RateRank
FROM   HumanResources.Employee
       JOIN
       HumanResources.EmployeePayHistory
ON     HumanResources.Employee.BusinessEntityID = HumanResources.EmployeePayHistory.BusinessEntityID;
GO

--Task 3
SELECT HumanResources.EmployeePayHistory.BusinessEntityID, HumanResources.Employee.JobTitle, COUNT(*) AS RateCount
FROM   HumanResources.Employee  
       JOIN 
	     HumanResources.EmployeePayHistory 
ON     HumanResources.Employee.BusinessEntityID = HumanResources.EmployeePayHistory.BusinessEntityID
GROUP BY HumanResources.EmployeePayHistory.BusinessEntityID, HumanResources.Employee.JobTitle
HAVING COUNT(*) > 1;
GO

--Task 4
SELECT HumanResources.Department.DepartmentID, HumanResources.Department.Name, COUNT(*) AS EmployeeCount
FROM   HumanResources.EmployeeDepartmentHistory
       JOIN
	     HumanResources.Department
ON     HumanResources.EmployeeDepartmentHistory.DepartmentID = HumanResources.Department.DepartmentID
       JOIN
       HumanResources.Employee
ON     HumanResources.EmployeeDepartmentHistory.BusinessEntityID = HumanResources.Employee.BusinessEntityID
GROUP BY HumanResources.Department.DepartmentID, HumanResources.Department.Name
HAVING COUNT(*) > 1;
GO

--TASK 5  
SELECT HumanResources.Employee.BusinessEntityID,
       HumanResources.Employee.JobTitle, 
	     HumanResources.EmployeePayHistory.Rate,
	     ISNULL(LAG (HumanResources.EmployeePayHistory.Rate) OVER (PARTITION BY HumanResources.EmployeePayHistory.BusinessEntityID ORDER BY HumanResources.EmployeePayHistory.BusinessEntityID), 0) AS PrevRate,
	    (HumanResources.EmployeePayHistory.Rate - ISNULL(LAG (HumanResources.EmployeePayHistory.Rate) OVER (PARTITION BY HumanResources.EmployeePayHistory.BusinessEntityID ORDER BY HumanResources.EmployeePayHistory.BusinessEntityID), 0)) AS DiffRate
FROM   HumanResources.Employee 
       JOIN 
	     HumanResources.EmployeePayHistory 
ON     HumanResources.Employee.BusinessEntityID = HumanResources.EmployeePayHistory.BusinessEntityID;
GO

--TASK 6											      
SELECT HumanResources.Department.Name,
       HumanResources.Employee.BusinessEntityID, 
	   HumanResources.EmployeePayHistory.Rate,
	   MAX (HumanResources.EmployeePayHistory.Rate) OVER(PARTITION BY HumanResources.Department.Name) AS MaxInDepartment,
	   DENSE_RANK() OVER(PARTITION BY HumanResources.Department.Name ORDER BY HumanResources.EmployeePayHistory.Rate) AS RateGroup
FROM   HumanResources.EmployeeDepartmentHistory
       JOIN
	     HumanResources.Department
ON     HumanResources.EmployeeDepartmentHistory.DepartmentID = HumanResources.Department.DepartmentID
       JOIN
       HumanResources.Employee
ON     HumanResources.EmployeeDepartmentHistory.BusinessEntityID = HumanResources.Employee.BusinessEntityID
       JOIN
	   HumanResources.EmployeePayHistory
ON     HumanResources.Employee.BusinessEntityID = HumanResources.EmployeePayHistory.BusinessEntityID
WHERE  HumanResources.EmployeeDepartmentHistory.EndDate IS NULL;
GO


--Task 7
SELECT HumanResources.EmployeeDepartmentHistory.BusinessEntityID,
       HumanResources.Employee.JobTitle, 
	   HumanResources.Shift.Name,
	   HumanResources.Shift.StartTime,
	   HumanResources.Shift.EndTime
FROM   HumanResources.Shift
       JOIN
	   HumanResources.EmployeeDepartmentHistory
ON     HumanResources.Shift.ShiftID = HumanResources.EmployeeDepartmentHistory.ShiftID
       JOIN
	   HumanResources.Employee
ON     HumanResources.EmployeeDepartmentHistory.BusinessEntityID = HumanResources.Employee.BusinessEntityID
WHERE HumanResources.Shift.Name = 'Evening';
GO

--Task 8
SELECT HumanResources.Employee.BusinessEntityID,
       HumanResources.Employee.JobTitle,
	     HumanResources.Department.Name AS DepName,
	     HumanResources.EmployeeDepartmentHistory.StartDate,
	     HumanResources.EmployeeDepartmentHistory.EndDate,
		DATEDIFF (YEAR, EmployeeDepartmentHistory.StartDate, ISNULL(EmployeeDepartmentHistory.EndDate, (CONVERT(date, GETDATE())))) AS Experince
FROM   HumanResources.Employee 
       JOIN
	     HumanResources.EmployeeDepartmentHistory
ON     HumanResources.Employee.BusinessEntityID = HumanResources.EmployeeDepartmentHistory.BusinessEntityID
       JOIN
	   HumanResources.Department
ON     EmployeeDepartmentHistory.DepartmentID = HumanResources.Department.DepartmentID;
GO


