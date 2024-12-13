\connect employeedb

CREATE TABLE Employees (
    Id SERIAL PRIMARY KEY,            
    FirstName VARCHAR(100) NOT NULL,  
    LastName VARCHAR(100) NOT NULL,   
    Email VARCHAR(255) UNIQUE NOT NULL,
    CONSTRAINT chk_email_format CHECK (Email ~* '^[^@\s]+@[^@\s]+\.[^@\s]+$')
);
CREATE TABLE TimeEntries (
    Id SERIAL PRIMARY KEY,           
    EmployeeId INT NOT NULL,         
    Date DATE NOT NULL,             
    HoursWorked INT NOT NULL CHECK (HoursWorked >= 0), 
    CONSTRAINT FK_Employees FOREIGN KEY (EmployeeId)
        REFERENCES Employees (Id) ON DELETE CASCADE 
);

