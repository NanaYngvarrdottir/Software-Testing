<?
include("settings/stardust.php");


if ($_GET[purchase_id] != "")
{
	$_SESSION[purchase_id] = cleanQuery($_GET[purchase_id]);
}
	
if (!$_SESSION[USERID])
{
	$_SESSION[next_page] = 'getcurrency';
	header("Location: index.php?page=login&btn=webui_menu_item_login&next_page=prepaypal&purchase_id=$_SESSION[purchase_id]");
}

if ($_SESSION[purchase_id] != '')
{
	$found = array();
	$found[0] = json_encode(array('Method' => 'CheckPurchaseStatus', 'WebPassword' => md5(WIREDUX_PASSWORD), 'purchase_id' => $_SESSION[purchase_id], 'principalId' => $_SESSION[USERID]));
	$do_post_requested = stardust_do_post_request($found);
	$recieved = json_decode($do_post_requested);
	
	// echo '<pre>';
	// var_dump($recieved);
	// var_dump($do_post_requested);
	// echo("****".$recieved->{'FailNumber'}."#*********");
	// echo '</pre>';
	
	if ($recieved->{'FailNumber'} != "0") 
	{
		if ($recieved->{'Reason'} != "")
		{
			echo($recieved->{'Reason'});
			if ($recieved->{'FailNumber'} == 2)
			{?>
				</br>
				<b>How to fix this:</b><br/>
				1) Please logout<br/>
				2) Log back in as the correct user<br/>
				3) Then re-open the URL given to you in your local chat window.<br/>
				<br/>
			<?}
		}
		else
		{
			echo("Unknown Issues.. Please try again in a bit.");
		}
	}
	else
	{
		echo("here");
		$_SESSION[paypalAmount] = $recieved->{'USDAmount'} / 100;
		echo("here2");
		if ($recieved->{'PurchaseType'} == "1")
		{
			$_SESSION[paypalPurchaseItem] = "Currency Purchase of ".$recieved->{'Amount'};
			$_SESSION[purchase_type] = "_xclick";
		}
		else
		{
			$_SESSION[paypalPurchaseItem] = $recieved->{'RegionName'} . " Region";
			$_SESSION[purchase_type] = "_xclick-subscriptions";
		}
		echo("here3");
		?>
		<table>
			<thead>
				<tr>
					<td>
						<h1>Purchase with PayPal</h1>
					</td>
				</tr>
			</thead>
			<tr>
				<td colspan="2">Please review your purchase. If you are statified with it please click the Purchase Button.</td>
			</tr>
			<tr class="odd">
				<td>You are buying</td>
				<td><?=$_SESSION[paypalPurchaseItem]?></td>
			</tr>
			<tr class="even">
				<td>You are paying</td>
				<td>$<?=$_SESSION[paypalAmount]?> USD</td>
			</tr>
			<tr class="odd">
				<?if ($recieved->{'PurchaseType'} == 1){?>
					<td colspan="2">* An addition fee of <?=($AmountAdditionPerfectage * 100)?>% + $0.30 has also been applied.</td>
				<?}else{?>
					<td colspan="2">* You will be charged this fee Monthly</td>
				<?}?>
			</tr>
			<tr>
				<td colspan="2">
					<a href="send_to_paypal.php"><img align="right" style="float:right" src="images/StarDust/paypal-purchase-button.png" /></a>
				</td>
			</tr>
			<tr>
				<td colspan="2"><h2>FAQ</h2></td>
			</tr>
			<tr class="odd">
				<td colspan="2">Q: How long does it take to get my purchase</td>
			</tr>
			<tr class="even">
				<td colspan="2">A: Regions can take a couple days, but in most cases your purchase will be sent to you right away.<br/>
				If there are any complication during the purchase they could be placed on hold, but we will resolve the issue ASAP!</td>
			</tr>
			<tr class="odd">
				<td colspan="2">Q: Why do you charge any addition <?=($AmountAdditionPerfectage * 100)?>%?</td>
			</tr>
			<tr class="even">
				<td colspan="2">A: These are just fees that PayPal Charges us.</td>
			</tr>
		</table>
		<?
	}
}
else
{
	?>
	<table>
		<thead>
			<tr>
				<td>
					<h1>Purchase G$</h1>
				</td>
			</tr>
		</thead>
		<tr>
			<td>
				If you would like to purchase G$, please login to your viewer and click you G$ Balance. <br/>Web interface to get G$ coming soon.
			</td>
		</tr>
	</table>
	<?
}

?>

