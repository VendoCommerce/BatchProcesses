
/****** Object:  StoredProcedure [dbo].[GetCustomerAddress]    Script Date: 01/21/2010 11:10:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetCustomerAddress]
(
	@ipAddressID int
)
AS
BEGIN
	SET NOCOUNT ON
	DECLARE @Err int
	select * from dbo.v_Address where AddressId =  @ipAddressID
		
END



