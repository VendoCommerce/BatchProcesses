
/****** Object:  StoredProcedure [dbo].[GetSalesTaxRate]    Script Date: 01/21/2010 11:11:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetSalesTaxRate]
(
	@ipState varchar(10)
)
AS
BEGIN
	SET NOCOUNT ON
	
	select percentage/100  as taxrate
	from taxvalue 
	where taxregionid 
	in( select taxregionid from taxregion 
	where stateprovinceid 
	in  ( select stateprovinceid from stateprovince 
	where ltrim(rtrim([name]))=ltrim(rtrim(@ipState))  ))
END



