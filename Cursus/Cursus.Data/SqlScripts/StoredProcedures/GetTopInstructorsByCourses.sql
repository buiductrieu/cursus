CREATE OR ALTER PROCEDURE GetTopInstructorsByCourses
    @TopN INT,
    @StartDate DATE,
    @EndDate DATE,
    @PageNumber INT,
    @PageSize INT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;
    WITH TopInstructors AS (
        SELECT TOP (@TopN)
            vc.InstructorId,
            vc.InstructorName,
            vc.TotalCoursesSold,
            MAX(vc.LastOrderDate) AS LastOrderDate
        FROM vw_TopInstructorsByCourses vc
        WHERE (vc.LastOrderDate >= @StartDate OR @StartDate IS NULL)
          AND (vc.LastOrderDate <= @EndDate OR @EndDate IS NULL)
        GROUP BY vc.InstructorId, vc.InstructorName, vc.TotalCoursesSold
        ORDER BY vc.TotalCoursesSold DESC, MAX(vc.LastOrderDate) DESC
    )
    SELECT 
        InstructorId,
        InstructorName,
        TotalCoursesSold,
        LastOrderDate
    FROM TopInstructors
    ORDER BY TotalCoursesSold DESC, LastOrderDate DESC
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
END;
GO
