
/****** Object:  StoredProcedure [dbo].[GetOrders]    Script Date: 01/21/2010 09:09:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetOrders]
(
	@ipCreationDate datetime
)
AS
BEGIN
	SET NOCOUNT ON
	DECLARE @Err int
	SELECT *
	FROM [dbo].[v_Order]
		where CONVERT(varchar(8), Created, 112) ='20100119'  @ipCreationDate
		
END











