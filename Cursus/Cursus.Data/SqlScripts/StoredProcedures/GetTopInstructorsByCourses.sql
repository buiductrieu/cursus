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
        SELECT 
            vc.InstructorId,
            vc.InstructorName,
            vc.TotalCoursesSold,
            MAX(o.DateCreated) AS LastOrderDate
        FROM vw_TopInstructorsByCourses vc
        INNER JOIN dbo.[CartItems] ci ON vc.InstructorId = ci.CourseId
        INNER JOIN dbo.[Order] o ON ci.CartId = o.CartId
        WHERE  (o.DateCreated >= @StartDate OR @StartDate IS NULL)
            AND (o.DateCreated <= @EndDate OR @EndDate IS NULL)
        GROUP BY vc.InstructorId, vc.InstructorName, vc.TotalCoursesSold
        ORDER BY vc.TotalCoursesSold DESC
        OFFSET 0 ROWS FETCH NEXT @TopN ROWS ONLY
    )
    SELECT 
        InstructorId,
        InstructorName,
        TotalCoursesSold,
        LastOrderDate
    FROM TopInstructors
    ORDER BY TotalCoursesSold DESC
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
END;
GO
