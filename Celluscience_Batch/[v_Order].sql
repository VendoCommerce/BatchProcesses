
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER VIEW [dbo].[v_Order]
AS
SELECT     dbo.[Order].OrderId, dbo.[Order].Email, dbo.[Order].SubTotal, dbo.[Order].PaymentType, dbo.[Order].CreditCardType, dbo.[Order].CreditCardName, 
                      dbo.[Order].CreditCardNumber, dbo.[Order].CreditCardExpired, dbo.[Order].CustomerDiscount, dbo.[Order].Processed, dbo.[Order].BillingAddressId, 
                      dbo.[Order].CustomerId, dbo.[Order].Created, dbo.[Order].Completed, dbo.[Order].ShippingCost, dbo.[Order].CurrencyId, dbo.[Order].CreditCardCSC, 
                      dbo.[Order].AffiliateId, dbo.[Order].AffiliatePercent, dbo.[Order].BankAccountNumber, dbo.[Order].BankRoutingNumber, dbo.[Order].BankAccountType, 
                      dbo.[Order].BankName, dbo.[Order].BankAccountName, dbo.[Order].LicenseNumber, dbo.[Order].LicenseDOB, dbo.[Order].LicenseState, 
                      dbo.[Order].CheckNumber, dbo.[Order].Tax, dbo.[Order].TextResponse, dbo.[Order].AuthorizationCode, dbo.[Order].ConfirmationCode, 
                      dbo.[Order].OrderStatusId, dbo.[Order].Charge, dbo.[Order].OrderAuthorizationCode, dbo.[Order].LastEditDate, dbo.[Order].ExpirationDate, 
                      dbo.CustomerAccount.UserName AS CustomerUserName, dbo.CustomerAccount.Email AS CustomerEmail, 
                      dbo.CustomerAccount.ShippingAddressId AS CustomerShippingAddressId, dbo.CustomerAccount.BillingAddressId AS CustomerBillingAddressId, 
                      dbo.OrderShipment.ShippingMethod
FROM         dbo.CustomerAccount INNER JOIN
                      dbo.[Order] ON dbo.CustomerAccount.CustomerId = dbo.[Order].CustomerId INNER JOIN
                      dbo.OrderShipment ON dbo.[Order].OrderId = dbo.OrderShipment.OrderId
GO

SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

