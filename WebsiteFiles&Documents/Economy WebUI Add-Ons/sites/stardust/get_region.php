<?
if (!$_SESSION[USERID]) header("Location: index.php?page=Home");
?>

<script language="Javascript">
	function validate()
	{
		var thisform = document.getElementById('thisform');
		var agree = thisform.elements['agree'];
		var regionname = thisform.elements['name'];
		if (agree.checked){ 
			if (regionname.value != ''){ 
				return true;
			} else { 
				alert('Please fill out a name for the region.'); 
				return false;
			} 
		}else{ 
			alert('Please read the TOS and agree with it.');
			return false;
		}
	}
</script>

<form method="post" action="index.php?page=buyregion" name="thisform" id="thisform" onsubmit="if (!validate()) return false;">
	<table id="Table_01" border="0" cellpadding="0" cellspacing="0" width="100%">
		<tr>
			<td width="14">
				<img src="images/redbox/RedBox_01.png" width="14" height="12" alt=""></td>
			<td background="images/redbox/RedBox_02.png">
				<img src="images/redbox/RedBox_02.png" width="37" height="12" alt=""></td>
			<td width="12">
				<img src="images/redbox/RedBox_03.png" width="13" height="12" alt=""></td>
		</tr>
		<tr>
			<td width="14" background="images/redbox/RedBox_04.png">
				<img src="images/redbox/RedBox_04.png" width="14" height="41" alt=""></td>
			<td>
				<table bgcolor="#9c0031" width="100%">
					<tr>
						<td align="right">Name for the Region</td>
						<td><input name="name" type="text" maxlength="36" value="<?=$_GET[name]?>" /></td>
					</tr>
					<tr>
						<td></td>
						<td>
<?if ($_GET[error] != ''){?>
							<div style="color:yellow"><?=$_GET[error]?></div>
<?}?>
						</td>
					</tr>
					<tr>
						<td colspan="2">Addition Notes</td>
					</tr>
					<tr>
						<td colspan="2">
							<textarea name="notes" cols=50 rows=5 maxlength="1024"><?=$_GET[notes]?></textarea>
						</td>
					</tr>
					<tr>
						<td valign="top" colspan="2">
							Terms of Service
						</td>
					</tr>
					<tr>
						<td colspan="2">
							<div style="width:450px;height:100px;overflow:auto;color:black;background-color:#cccccc;"><pre><? include("tos.txt"); ?></pre></div>
						</td>
					</tr>
					<tr>
						<td colspan="2">
							<input type="checkbox" name="agree" id="agree" value="1" /><label for="agree">I agree with the Terms of Service.</label>
						</td>
					</tr>
					<tr>
						<td></td>
						<td>
							<input type="submit" value="Get It!" />
							<input type="hidden" name="id" value="<?=$_POST[id]?><?=$_GET[id]?>" />
							<input type="hidden" name="button_id" value="<?=$_POST[button_id]?><?=$_GET[button_id]?>" />
						</td>
					</tr>
				</table>
			</td>
			<td width="12" background="images/redbox/RedBox_06.png">
				<img src="images/redbox/RedBox_06.png" width="13" height="41" alt=""></td>
		</tr>
		<tr>
			<td width="14">
				<img src="images/redbox/RedBox_07.png" width="14" height="11" alt=""></td>
			<td background="images/redbox/RedBox_08.png">
				<img src="images/redbox/RedBox_08.png" width="37" height="11" alt=""></td>
			<td width="12">
				<img src="images/redbox/RedBox_09.png" width="13" height="11" alt=""></td>
		</tr>
	</table>
</form>