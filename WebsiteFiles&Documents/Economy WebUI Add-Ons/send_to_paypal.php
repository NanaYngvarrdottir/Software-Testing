<?PHP

//Use gzip if it is supported
if (substr_count($_SERVER['HTTP_ACCEPT_ENCODING'], 'gzip'))
    ob_start("ob_gzhandler"); else
    ob_start();
session_start();

include("settings/stardust.php");

?>
<html>
	<head>
		<title>Transfering you to PayPal, Please stand by..</title>
	</head>
	<body onload="document.getElementById('thisform').submit();">
		<table width="100%" height="100%">
			<tr><td align="center" valign="middle"><img src="images/StarDust/loading.gif" /></td></tr>
		</table>
		<form action="https://<?=$payPalURL?>/cgi-bin/webscr" method="post" name="ppform" id="thisform">
			<input type="hidden" name="cmd" value="<?=$_SESSION[purchase_type]?>" />
			
<?
	if ($_SESSION[purchase_type] == '_xclick-subscriptions')
	{?>
			<input type="hidden" name="a3" value="<?=$_SESSION[paypalAmount]?>" />
			<input type="hidden" name="p3" value="1" />
			<input type="hidden" name="t3" value="M" />
			<input type="hidden" name="src" value="1" />
			<input type="hidden" name="sra" value="1" />
			<input type="hidden" name="modify" value="0" />
	<?}else{?>
			<input type="hidden" name="amount" value="<?=$_SESSION[paypalAmount]?>" />
			<input type="hidden" name="quantity" value="1" />
	<?}
?>
			
			<input type="hidden" name="notify_url" value="<?=$notifyURL?>" />
			
			<input type="hidden" name="upload" value="1" />
			<input type="hidden" name="business" value="<?=$payPalAccount?>" />
			<input type="hidden" name="currency_code" value="USD" />
			
			<input type="hidden" name="item_name" value="<?=$_SESSION[paypalPurchaseItem]?>" />
			<input type="hidden" name="no_shipping" value="1" />
			
			<input type="hidden" name="custom" value="<?=$_SESSION[purchase_id]?>" />
			<INPUT TYPE="hidden" NAME="return" value="<?=$returnURL?>">
			<input type="image" name="submit" src="https://<?=$payPalURL?>/en_US/i/logo/PayPal_mark_37x23.gif" align="left" style="margin-right:7px;">
		</form>
</body>
</html>

<!--//document.getElementById('thisform').submit();-->