
/****** Object:  StoredProcedure [dbo].[GetOrderSKU]    Script Date: 01/21/2010 20:36:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetOrderSKU]
(
	@ipOrderID int
)
AS
BEGIN
	SET NOCOUNT ON
	
	select * 
	from dbo.v_OrderSKU 
	where orderid = @ipOrderID
		
END


