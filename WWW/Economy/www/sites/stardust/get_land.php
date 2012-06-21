<?
if (!$_SESSION[USERID]) header("Location: index.php?page=Home");
?>
<div id="content" style="border-top-left-radius: 10px 10px; border-top-right-radius: 10px 10px; border-bottom-left-radius: 10px 10px; border-bottom-right-radius: 10px 10px; ">
	<div id="annonce10" style="border-top-left-radius: 10px 10px; border-top-right-radius: 10px 10px; border-bottom-left-radius: 10px 10px; border-bottom-right-radius: 10px 10px; background-color: rgb(34, 34, 34); ">
		<table cellpadding="8" cellspacing="8">
			<tr>
				<td colspan="2">
					<center>Choose the type of land you would like to buy.</center>
				</td>
			</tr>
			<tr>
				<td width="50%">

								
								<center>Get yourself a Island.</center>
		<?
			$DbLink->query("SELECT id, name, description, price FROM stardust_subscriptions Where active='1' ORDER BY price ASC ");
			$counter = 0;
			while (list($id, $name, $description, $price,) = $DbLink->next_record()) { $counter++;
		?>

								<form action="index.php?page=getregion" method="post">
									<div id="step<?=$counter?>" style=" width:85%; border-top-left-radius: 10px 10px; border-top-right-radius: 10px 10px; border-bottom-left-radius: 10px 10px; border-bottom-right-radius: 10px 10px; background-color: rgb(0, 0, 0); ">
										<h3>Name: <?=$name?></h3> 
										<p><img align="right" src="images/SimImageStub.jpg" /><?=$description?></p>
										<input type="hidden" name="id" value="<?=$id?>" />
										<input type="hidden" name="button_id" value="<?=$button_id?>" />
										<input type="Submit" value="Get It for only <?=$price / 100.0?>" />
									</div>
								</form><br/>
		<?
		}
		?>
				</td>
				<td valign="top" width="50%">
					<ul>
						<li>No virutal servers, real server hardware!</li>
						<li>Only running two 256x256 regions per core!</li>
						<li>Varible Regions (sims can be many sizes, not just 256</li>
							<ul><li>64x64, 128x128, 256x256, 512x512</li></ul></li>
						<li>Extra Estate Management Options
							<ul>
								<li>Default Draw Distance</li>
								<li>Force Draw Distance</li>
								<li>Allow Minimap</li>
								<li>May May More</li>
							</ul>
						</li>
						<li>Secure!</li>
						<li>Choose your default avatar when you sign up</li>
						<li>Latest opensource physics engine!</li>
						<li>No purchase fee</li>
						<li>No set up fee</li>
						<li>Free condo in SL with purchases of 256 or larger region</li>
						<li>Daily roll back capabilites as far as 7 days</li>
						<li>Unlimited renaminmg of sim</li>
						<li>Three free moves</li>
					</ul>
				</td>
			</tr>
			<tr>
				<td colspan="2">
					Note: Certain viewers are required for Islands larger than 256x256
				</td>
			</tr>
		</table>
	</div>
</div>