<?
if (!$_SESSION[USERID]) header("Location: index.php?page=Home");
include("settings/stardust.php");


$DbLink3 = new DB;
$DbLink3->query("SELECT RegionName FROM gridregions WHERE RegionName = '".cleanQuery($_POST[name])."'");
$DbLink2 = new DB;
$DbLink2->query("SELECT PrincipalID FROM stardust_purchased WHERE RegionName = '".cleanQuery($_POST[name])."' AND Complete = 1");

if ($DbLink2->num_rows() + $DbLink3->num_rows() > 0)
{
	header("Location: index.php?page=getregion&button_id=$_POST[button_id]&id=$_POST[id]&name=$_POST[name]&notes=$_POST[notes]&error=Name already in use");
}
else
{

	$found = array();
	$found[0] = json_encode(array('Method' => 'OrderSubscription', 'WebPassword' => md5(WIREDUX_PASSWORD), 'toId' => $_SESSION[USERID], 'regionName' => cleanQuery($_POST[name]), 'notes' => cleanQuery($_POST[notes]), 'subscription_id' => cleanQuery($_POST[id])));
	$do_post_requested = stardust_do_post_request($found);
	$recieved = json_decode($do_post_requested);

	if ($recieved->{'Verified'} == "true") 
	{?>
		<script language="Javascript">
			function Go()
			{
				window.location.href='index.php?page=prepaypal&purchase_id=<?=$recieved->{'purchaseID'}?>';
			}
			window.onload=Go; 
		</script>
	<?}
	else
	{
		if ($recieved->{'Reason'} != "")
		{
			echo($recieved->{'Reason'});
		}
		else
		{
			echo("Unknown Error. Please try again in a bit.");
		}
	}
}
?>
