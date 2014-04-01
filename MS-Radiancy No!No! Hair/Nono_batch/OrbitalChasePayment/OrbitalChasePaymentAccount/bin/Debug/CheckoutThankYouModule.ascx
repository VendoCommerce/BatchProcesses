<%@ Control Language="C#" CodeFile="CheckoutThankYouModule.ascx.cs"
    Inherits="Mediachase.eCF.PublicStore.MasterTemplates.Default.Modules.CheckoutThankYouModule" %>
   <%@ Register Assembly="System.Web.Extensions, Version=1.0.61025.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
    Namespace="System.Web.UI" TagPrefix="asp" %>
    <link href="/css/global.css" rel="stylesheet" type="text/css" media="all" /> <script type="text/javascript" src="/js/popup2.js"></script>
 <script type="text/javascript" src="/js/CSSPopUp.js"></script>
  <asp:ScriptManager ID="Scriptmanager1" runat="server"></asp:ScriptManager>


<div id="included" style="display: none; background: url(/b4/images/zquietbig.jpg) no-repeat;
        width: 500px; height: 500px;">
        <a href="#" onclick="Popup.hide('included')" style="position: relative; left: 430px;
            top: 8px; color: #000; font-size: 13px;">CLOSE X</a></div>    
    <div id="blanket" style="display: none;">
</div>

<!--
Start of DoubleClick Floodlight Tag: Please do not remove
Activity name of this tag: ZQuiet New Confirmation Page
URL of the webpage where the tag is expected to be placed: http://zquiet.com/confirm
This tag must be placed between the <body> and </body> tags, as close as possible to the opening tag.
Creation Date: 02/22/2011
-->

<asp:Literal ID="DFAPixel" runat="server"></asp:Literal>
<asp:Literal ID="DFAPixelFrame" runat="server"></asp:Literal>

<!-- End of DoubleClick Floodlight Tag: Please do not remove -->
<div id="FriendDiv" style="display:none; width: 500px; height: auto; font-size: 13px; color: #5a5a5a;">

    <asp:UpdatePanel ID="UpdatePanel2" UpdateMode="Conditional" runat="server">
        <ContentTemplate>
             <div style="position: relative; background: url(/b4/images/friendoverlay_top.png) no-repeat;
        width: 516px; height: 84px;">
        <a href="#" onClick="popup('FriendDiv')" style="position: absolute; left: 396px;
            top: 28px; color: #000; font-size: 13px; width: 83px; height: 29px;"></a>
    </div>
    <div style="position: relative; background: url(/b4/images/friendoverlay_bg.png) repeat-y;
        width: 406px; padding: 0 55px;">
        <p style="color: #0e4d71; font-weight: bold; font-size: 16px; padding-bottom: 20px;">
            Thank You for Telling a Friend about ZQuiet.</p>
        <p>
            Share your success with a friend! Now that you've found relief, tell your friends
            and family about ZQuiet so they can find some peace and quiet, too!</p>
        <p>&nbsp;
            </p>
            <p>
            
            </p>
        <asp:Literal ID="_SuccessMesg" runat="server"></asp:Literal>
        <table width="93%" border="0" cellspacing="4" cellpadding="0">
            <tr>
                <td width="33%">
                    Your Name:
                </td>
                <td width="67%">
                <input type="hidden" id="myHiddenInput" runat="server" value="false" />
                    <asp:Literal ID="_YourNameError" runat="server"></asp:Literal>
                    <%--      <input type="text" name="textfield" id="textfield" class="friendinput" runat="server" />--%>
                    <asp:TextBox ID="txtYourName" CssClass="friendinput" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    Your Email:
                </td>
                <td>
                    <%--    <input type="text" name="textfield2" id="textfield2" class="friendinput" runat="server" />--%>
                    <asp:Literal ID="_YourEmailError" runat="server"></asp:Literal>
                    <asp:TextBox ID="txtYourEmail" CssClass="friendinput" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    Friend's Name:
                </td>
                <td>
                    <%--           <input type="text" name="textfield3" id="textfield3" class="friendinput" runat="server" />--%>
                    <asp:Literal ID="_FriendsNameError" runat="server"></asp:Literal>
                    <asp:TextBox ID="txtFriendsName" CssClass="friendinput" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    Friend's Email:
                </td>
                <td>
                    <%--    <input type="text" name="textfield4" id="textfield4" class="friendinput" runat="server" />--%>
                    <asp:Literal ID="_FriendsEmailError" runat="server"></asp:Literal>
                    <asp:TextBox ID="txtFriendsEmail" CssClass="friendinput" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td valign="top">
                    Message:
                </td>
                <td>
                    <%--         <textarea name="textarea" id="textarea" cols="45" rows="5" class="friendinput" runat="server"></textarea>--%>
                    <asp:Literal ID="_MessageError" runat="server"></asp:Literal>
                    <asp:TextBox ID="txtMessage" Columns="45" Rows="5" TextMode="MultiLine" CssClass="friendinput"
                        runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>&nbsp;
                    
                </td>
                <td>
                    <%-- <input type="submit" name="button" id="button" value="Send Email" />--%>
                    <asp:Button ID="btnSubmit" Text="Send Email" runat="server" OnClick="btnSubmit_Click" />
                </td>
            </tr>
            <tr>
                <td>&nbsp;
                    
                </td>
                <td>&nbsp;
                    
                </td>
            </tr>
        </table>
    </div>
    <div style="position: relative; background: url(/b4/images/friendoverlay_bottom.png) no-repeat;
        width: 516px; height: 57px;">
    </div>

        </ContentTemplate>
    </asp:UpdatePanel>
    </div>
  
 <div id="header">
         <a id="nav-friend" href="#" onclick="popup('FriendDiv')">        </a>
        
        <a id="espanol" href="http://www.zinroncar.com">Para Espa&ntilde;ol</a><a id="nav-home" href="/index.aspx"></a><a id="nav-whyitworks" href="/whyitworks.aspx">
        </a><a id="nav-testimonials" href="/testimonials.aspx"></a><a id="nav-faq" href="/faq.aspx">
        </a><a id="nav-about" href="/customer_service.aspx"></a><a id="nav-try" href="#tryitnow"></a>
        <a id="nav-logo" href="/index.aspx"></a>
<p id="phone_number">
            For Phone Orders Call: (800) 928-2489  </p>
    </div>
    <!-- end header -->


<div id="stretch_container">
<div id="receipt_content"  style="background: url(/b4/images/receipt_bg.png) no-repeat; height: auto; width: 881px; position:relative;padding-top: 120px; padding-left: 80px;">
<asp:HyperLink ID="SmartSell" runat="server" Visible="false"></asp:HyperLink>
            <asp:Label ID="test" runat="server" Visible="false"></asp:Label>
            
<table width="819" border="0" cellspacing="0" cellpadding="0" id="receipt_table1">
  <tr>
    <td width="76%" valign="top" style="padding: 20px 0; border-top: 1px dotted #ccc;"><strong>Description</strong></td>
    <td width="12%" valign="top" style="padding: 20px 0; border-top: 1px dotted #ccc;"><strong>Quantity</strong></td>
    <td width="12%" valign="top" style="padding: 20px 0; border-top: 1px dotted #ccc;"><strong>Total</strong></td>
  </tr>
  <asp:Literal ID="LiteralTableRows" runat="server"></asp:Literal>
  <tr>
   <td valign="top" style="font-size: 12px; font-style:italic;padding: 20px 0; border-top: 1px dotted #ccc; border-bottom: 1px dotted #ccc;">
   <asp:Panel ID="PanelItemInfo" Visible="false" runat="server">
   *If you are unhappy with the ZQuiet in any way, simply return the product within 30 days and you will not be 
      <br />
      charged another dime.  If you do decide to keep your ZQuiet after the 30 day trial period, simply do nothing 
      <br />
      and the credit card you used today will be charged a one-time fee of $59.95 (plus any applicable sales tax).</asp:Panel></td>
    <td valign="top" style="padding: 20px 0; border-top: 1px dotted #ccc; border-bottom: 1px dotted #ccc;"> Subtotal:<br />
      S &amp; P: <br />
      Tax: <br />
      Total: </td>
    <td valign="top" style="padding: 20px 0; border-top: 1px dotted #ccc; border-bottom: 1px dotted #ccc;">
      $<asp:Literal ID="LiteralSubTotal" runat="server"></asp:Literal><br />
      $<asp:Literal ID="LiteralShipping" runat="server"></asp:Literal><br />
      $<asp:Literal ID="LiteralTax" runat="server"></asp:Literal><br />
      $<asp:Literal ID="LiteralTotal" runat="server"></asp:Literal></td>
  </tr>
</table>
<table width="683" border="0" cellspacing="0" cellpadding="0" id="receipt_table2" style="margin-top: 20px;">
  <tr>
    <td colspan="2" valign="top"><strong>Shipping Information:</strong></td>
    <td colspan="2" valign="top"><strong>Billing Information:</strong></td>
    </tr>
  <tr>
    <td width="132" valign="top">Name: <br />
      Address: <br />
      Address 2: <br />
      City: <br />
      State: <br />
      Zip Code: <br />
      Country: <br />
      Email Address: </td>
    <td width="198" valign="top"><asp:Literal ID="LiteralName" runat="server"></asp:Literal><br />
      <asp:Literal ID="LiteralAddress" runat="server"></asp:Literal><br />
      <asp:Literal ID="LiteralAddress2" runat="server"></asp:Literal><br />
        <asp:Literal ID="LiteralCity" runat="server"></asp:Literal><br />
        <asp:Literal ID="LiteralState" runat="server"></asp:Literal><br />
        <asp:Literal ID="LiteralZip" runat="server"></asp:Literal><br />
        <asp:Literal ID="LiteralCountry" runat="server"></asp:Literal><br />
        <asp:Literal ID="LiteralEmail" runat="server"></asp:Literal><br />
        </td>
    <td width="132" valign="top">Name: <br />
      Address: <br />
      Address 2: <br />
      City: <br />
      State: <br />
      Zip Code:<br />
      Country: 
      </td>
    <td width="221" valign="top">
    
     <asp:Literal ID="LiteralName_b" runat="server">
     </asp:Literal><br /><asp:Literal ID="LiteralAddress_b" runat="server"></asp:Literal><br />
     <asp:Literal ID="LiteralAddress2_b" runat="server"></asp:Literal><br />
     <asp:Literal ID="LiteralCity_b" runat="server"></asp:Literal><br />
     <asp:Literal ID="LiteralState_b" runat="server"></asp:Literal><br />
     <asp:Literal ID="LiteralZip_b" runat="server"></asp:Literal><br />
     <asp:Literal ID="LiteralCountry_b" runat="server"></asp:Literal>      
        
      </td>
  </tr>
</table>
</div>



</div>

<div id="receipt_bottom" style="position: relative;margin: 0 auto;padding: 0;width: 961px;">
<img src="/b4/images/receipt_bottom.png" width="961" height="41" /></div>
<div id="footer">
<p><a href="/index.aspx">Home</a>&nbsp;&nbsp;|&nbsp;&nbsp;<a href="/whyitworks.aspx">Why  It Works</a>&nbsp;&nbsp;|&nbsp;&nbsp;<a href="/testimonials.aspx">Testimonials</a>&nbsp;&nbsp;|&nbsp;&nbsp;<a href="/faq.aspx">FAQs</a>&nbsp;&nbsp;|&nbsp;&nbsp;<a href="/program_details.aspx">Trial Program Details</a>&nbsp;&nbsp;|&nbsp;&nbsp;<a href="/customer_service.aspx">Customer Service</a>&nbsp;&nbsp;|&nbsp;&nbsp;<a href="/privacypolicy.aspx">Privacy Policy</a>&nbsp;&nbsp;|&nbsp;&nbsp;<a href="javascript: void(0);" onclick="window.open('/terms.html', 'terms', 'width=680, height=570, scrollbars=1');">Terms and Conditions</a></p>
<p>&nbsp;</p>
<p>Copyright © 2010. Sleeping Well, LLC. All rights reserved.</p>
  
</div>
<!-- end footer -->


<script type="text/javascript">
    var gaJsHost = (("https:" == document.location.protocol) ? "https://ssl." : "http://www.");
    document.write(unescape("%3Cscript src='" + gaJsHost + "google-analytics.com/ga.js' type='text/javascript'%3E%3C/script%3E"));
</script>
    <asp:Literal ID="LitPixel2" runat="server"></asp:Literal>
<script type="text/javascript" src="https://pixel2.edgeadx.net/event/js?mt_id=102272&mt_adid=100396&v1=&v2=&v3=&s1=&s2=&s3="></script>
    <asp:Literal ID="GoogleAnalytics" runat="server"></asp:Literal>
