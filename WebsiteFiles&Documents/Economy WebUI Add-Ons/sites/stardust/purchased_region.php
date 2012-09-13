<?
$DbLink2 = new DB;
$DbLink2->query("
	SELECT 
		rp.name,  
		rp.notes,
		rpp.name
	FROM 
		region_purchases as rp
	inner join 
		region_products as rpp on rp.id = rpp.product_id
	WHERE rp.name = '$_SESSION[REGIONNAME]'
");

list($regionname, $regionnotes, $typename) = $DbLink2->next_record();

//-----------------------------------MAIL--------------------------------------
$date_arr = getdate();
$date = "$date_arr[mday].$date_arr[mon].$date_arr[year]";
$sendto = 'djwyand@yahoo.com, wendellthor@yahoo.com, robert.skidmore@gmail.com';
$subject = "Region Purchased " . SYSNAME;
$body .= "Region Purchased on " . SYSNAME . ".\n";
$body .= "Purchasures Name: $_SESSION[USERNAME]\n";
$body .= "Region Name: $regionname\n";
$body .= "Region Type: $typename\n\n";
$body .= "\n\n\n";
$header = "From: " . SYSMAIL . "\r\n";
$mail_status = mail($sendto, $subject, $body, $header);
//-----------------------------MAIL END --------------------------------------
	
$DbLink3 = new DB;
$DbLink3->query("
	SELECT 
		Email
	FROM 
		useraccounts
	WHERE PrincipalID = '$_SESSION[USERID]'
");

list($email) = $DbLink3->next_record();
	
	//-----------------------------------MAIL--------------------------------------
	$date_arr = getdate();
	$date = "$date_arr[mday].$date_arr[mon].$date_arr[year]";
	$sendto = $email;
	$subject = "Your New Region";
	$body .= "Dear $_SESSION[USERNAME]\n\n"
	$body .= "I would like to thank you for your purchase.\n\n";
	$body .= "Region Purchased on " . SYSNAME . ".\n";
	$body .= "Purchasures Name: $_SESSION[USERNAME]\n";
	$body .= "Region Name: $regionname\n";
	$body .= "Region Type: $typename\n\n";
	$body .= "If you have any questions please check out our FAQ";
	$body .= "\n\n\n";
	$header = "From: " . SYSMAIL . "\r\n";
	$mail_status = mail($sendto, $subject, $body, $header);
	//-----------------------------MAIL END --------------------------------------

?>
<table>
	<tr>
		<td>Thanks you for your purchase. Your reagion should be up within 2 business days. If you have any questions, or concerns please see our contact us area.</td>
	</tr>
</table>