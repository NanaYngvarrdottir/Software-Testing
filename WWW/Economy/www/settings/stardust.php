<?
##################### STARDUST #########################
define("STARDUST_SERVICE_URL","http://192.168.1.170:8007/StarDustWebUI");
$banker_id = '0b14870b-35d0-4238-8839-11d7fee7d74c';

$incompletePurchaseEmailandErrors = 'robert.skidmore@gmail.com, wendellthor@yahoo.com, djwyand@yahoo.com';

$payPalURL = 'www.sandbox.paypal.com';
$auth_token = "eK2d7-QOTrnwhqaJI2n-SpJ98p5Zb6V62lNekl8BdH-1-Q_PUCwzaDvokeW";
$payPalAccount = 'your_paypal_account_email@gmail.com';
$notifyURL = 'http://localhost/app/StarDust/paypal_verify.php';
$returnURL = 'http://localhost/index.php?page=paypalcomplete';

$AmountAdditionPerfectage = 0.0291;

function generateGuid($include_braces = false) {
	if (function_exists('com_create_guid')) {
		if ($include_braces === true) {
			return com_create_guid();
		} else {
			return substr(com_create_guid(), 1, 36);
		}
	} else {
		mt_srand((double) microtime() * 10000);
		$charid = strtoupper(md5(uniqid(rand(), true)));
	   
		$guid = substr($charid,  0, 8) . '-' .
				substr($charid,  8, 4) . '-' .
				substr($charid, 12, 4) . '-' .
				substr($charid, 16, 4) . '-' .
				substr($charid, 20, 12);
 
		if ($include_braces) {
			$guid = '{' . $guid . '}';
		}
   
		return $guid;
	}
}

function stardust_do_post_request($found) {
    $params = array('http' => array(
            'method' => 'POST',
            'content' => implode(',', $found)
            ));
    if ($optional_headers !== null) {
        $params['http']['header'] = $optional_headers;
    }
    $ctx = stream_context_create($params);
    $timeout = 15;
    $old = ini_set('default_socket_timeout', $timeout);
    $fp = @fopen(STARDUST_SERVICE_URL, 'rb', false, $ctx);
    ini_set('default_socket_timeout', $old);
    if ($fp) {
        stream_set_timeout($fp, $timeout);
        stream_set_blocking($fp, 3);
    } else
        //throw new Exception("Problem with " . STARDUST_SERVICE_URL . ", $php_errormsg");
        return false;
    $response = @stream_get_contents($fp);
    if ($response === false) {
        //throw new Exception("Problem reading data from " . STARDUST_SERVICE_URL . ", $php_errormsg");
    }
    return $response;
}
?>