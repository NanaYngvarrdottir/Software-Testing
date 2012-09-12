<html>
	<head>
		<title>Transfering you to PayPal, Please stand by..</title>
	</head>
</html>
<?include("settings/stardust.php");?>
<table width="100%" height="100%">
	<tr><td align="center" valign="middle"><img src="images/StarDust/loading.gif" /></td></tr>
</table>
<form action="https://<?=$payPalURL?>/cgi-bin/webscr" method="post" name="ppform" id="thisform">
	<input type="hidden" name="cmd" value="_xclick" />
	
	<input type="hidden" name="notify_url" value="<?=$notifyURL?>" />
	
	<input type="hidden" name="upload" value="1" />
	<input type="hidden" name="business" value="<?=$payPalAccount?>" />
	<input type="hidden" name="currency_code" value="USD" />
	
	<input type="hidden" name="item_name" value="G$ Currency Purchase" />
	<input type="hidden" name="amount" value="<?=$_SESSION[paypalAmount]?>" />
	<input type="hidden" name="quantity" value="1" />
	
	<input type="hidden" name="custom" value="<?=$_SESSION[purchase_id]?>" />
	<INPUT TYPE="hidden" NAME="return" value="<?=$returnURL?>">
	<input type="image" name="submit" src="https://<?=$payPalURL?>/en_US/i/logo/PayPal_mark_37x23.gif" align="left" style="margin-right:7px;">
</form>
<script language="Javascript">
	function Go()
	{
		var thisform = document.getElementById('thisform');
		thisform.submit();
	}
	//window.onload=Go; 
</script>