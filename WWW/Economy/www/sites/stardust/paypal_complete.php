<?
include("settings/stardust.php");


$tx = $_GET[tx];
$st = $_GET[st];
$amt = $_GET[amt];
$cc = $_GET[cc];
$cm = $_GET[cm];
$item_number = $_GET[item_number];


function Problem($message, $otherData)
{
	$date_arr = getdate();
	$date = "$date_arr[mday].$date_arr[mon].$date_arr[year]";
	$sendto = $incompletePurchaseEmailandErrors;
	$subject = "Currency Issue";
	$body .= "There was a issue with a currency Purchase:\n";
	$body .= $message . "\n";
	$body .= $otherData . "\n";
	$header = "From: " . SYSMAIL . "\r\n";
	$mail_status = mail($sendto, $subject, $body, $header);
}

// read the post from PayPal system and add 'cmd'
$req = 'cmd=_notify-synch';
$tx_token = $_GET['tx'];
if ($tx_token != "")
{
	$found = array();
	$found[0] = json_encode(array('Method' => 'Validate', 'WebPassword' => md5(WIREDUX_PASSWORD), 'tx' => $tx_token));
	$do_post_requested = stardust_do_post_request($found);
	$recieved = json_decode($do_post_requested);

	// echo '<pre>';
	// var_dump($recieved);
	// var_dump($do_post_requested);
	// echo '</pre>';

	if ($recieved->{'Verified'} == "True") 
	{
		echo("Thank you, Your purchase is complete.");
	}
	else
	{
		echo("There appears to be a problem with your purchase.");
	}
}
else
{
?>
Page 404
<?
}
?>