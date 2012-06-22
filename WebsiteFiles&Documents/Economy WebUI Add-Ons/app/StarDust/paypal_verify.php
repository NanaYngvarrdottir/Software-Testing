<?
include("settings/config.php");
include("settings/json.php");
include("settings/mysql.php");
include("settings/stardust.php");

$query_string = "";
if ($_POST) {
  $kv = array();
  foreach ($_POST as $key => $value) {
    $kv[] = "$key=$value";
  }
  $query_string = join("&", $kv);
}
else {
  $query_string = $_SERVER['QUERY_STRING'];
}

//-----------------------------------MAIL--------------------------------------
$date_arr = getdate();
$date = "$date_arr[mday].$date_arr[mon].$date_arr[year]";
$sendto = $incompletePurchaseEmailandErrors;
$subject = "Debug info";
$body .= "Here is the debug info from paypal_verify.php.\n";
$body .= "-" . $query_string . "-";
$header = "From: " . SYSMAIL . "\r\n";
$mail_status = mail($sendto, $subject, $body, $header);

?>