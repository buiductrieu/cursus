IF OBJECT_ID('dbo.vw_TotalSalesRevenue', 'V') IS NOT NULL
    DROP VIEW dbo.vw_TotalSalesRevenue;
GO
CREATE VIEW dbo.vw_TotalSalesRevenue
WITH SCHEMABINDING AS
SELECT 
    COUNT_BIG(*) AS RecordCount,
    COUNT_BIG(ci.CartItemsId) AS TotalSales,
    SUM(CASE WHEN o.Status = 1 THEN o.PaidAmount ELSE 0 END) AS TotalRevenue,
    o.Status,
    o.DateCreated
FROM dbo.CartItems ci
INNER JOIN dbo.[Order] o ON ci.CartId = o.CartId
WHERE o.Status = 1
GROUP BY o.Status, o.DateCreated;
GO
CREATE UNIQUE CLUSTERED INDEX IDX_vw_TotalSalesRevenue 
ON dbo.vw_TotalSalesRevenue (Status, DateCreated);
GO
